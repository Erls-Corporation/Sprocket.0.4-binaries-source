using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

using Sprocket;
using Sprocket.Data;
using Sprocket.Security;
using Sprocket.Web;
using Sprocket.Web.Controls;
using Sprocket.Utility;

namespace Sprocket.Web.CMS.Security
{
	[AjaxMethodHandler("WebSecurity")]
	partial class WebSecurity
	{
		void OnAjaxRequestAuthenticationCheck(System.Reflection.MethodInfo source, Result result)
		{
			if (!SecurityProvider.CurrentUser.Enabled)
			{
				result.SetFailed("Ajax method called failed because your account has been disabled.");
				return;
			}

			Attribute[] roleAttr = Attribute.GetCustomAttributes(source, typeof(RequiresRoleAttribute));
			Attribute[] permAttr = Attribute.GetCustomAttributes(source, typeof(RequiresPermissionAttribute));
			for (int i = 0; i < roleAttr.Length; i++)
				if (!SecurityProvider.CurrentUser.HasRole(((RequiresRoleAttribute)roleAttr[i]).RoleCode))
				{
					result.SetFailed("Ajax method call failed because you do not have one or more required roles.");
					return;
				}
			for (int i = 0; i < permAttr.Length; i++)
				if (!SecurityProvider.CurrentUser.HasPermission(((RequiresPermissionAttribute)permAttr[i]).PermissionTypeCode))
				{
					result.SetFailed("Ajax method call failed because you do not have one or more required permissions.");
					return;
				}
		}

		[AjaxMethod(RequiresAuthentication = true)]
		public bool IsUsernameTaken(string oldUsername, string newUsername)
		{
			return SecurityProvider.DataLayer.IsUsernameTaken(SecurityProvider.ClientSpaceID, newUsername, SecurityProvider.CurrentUser.UserID);
		}

		#region User Editing and Filtering
		public class UserFilterResults : IJSONEncoder
		{
			int matches;
			List<User> users;

			public UserFilterResults(int matches, List<User> users)
			{
				this.matches = matches;
				this.users = users;
			}

			public void WriteJSON(StringWriter writer)
			{
				writer.Write("{");
				JSON.EncodeNameValuePair(writer, "Matches", matches);
				writer.Write(",");
				JSON.EncodeString(writer, "Users");
				writer.Write(":[");
				int c = 0;
				foreach (User user in users)
				{
					if (c++ > 0) writer.Write(",");
					JSON.EncodeCustomObject(writer,
						new KeyValuePair<string, object>("Name", (user.FirstName + " " + user.Surname).Trim()),
						new KeyValuePair<string, object>("Username", user.Username),
						new KeyValuePair<string, object>("UserID", user.UserID)
					);
				}
				writer.Write("]}");
			}
		}

		[AjaxMethod(RequiresAuthentication = true)]
		[RequiresPermission("USERADMINISTRATOR")]
		public UserFilterResults FilterUsers(string username, string firstname, string surname, string email, int max)
		{
			int total;
			List<User> users = SecurityProvider.DataLayer.FilterUsers(username, firstname, surname, email, max, SecurityProvider.CurrentUser.UserID, null, out total);
			UserFilterResults results = new UserFilterResults(total, users);
			return results;
		}

		public delegate void UserEditFormLayoutHandler(long? userID, bool showErrors, AjaxForm form);
		public event UserEditFormLayoutHandler OnUserEditFormLayout;

		[AjaxMethod(RequiresAuthentication = true)]
		[RequiresPermission("USERADMINISTRATOR")]
		public AjaxForm GetUserEditForm(long? userID)
		{
			/* business rules:
			 * people with user administration access can only see user accounts that have a subset of the logged-in user's own roles/permissions
			 * user accounts containing roles or permissions that are not possessed by this user can NOT be altered by the current user
			 * the current user can only assign roles or permissions to other users if he/she has that role or permission
			 */
			string fErr = "function(value){{if(value.length==0) return 'Please enter a {0}'; return null;}}";
			string pErr = userID != null ? null : string.Format(fErr, "password");
			string username = null, firstname = null, surname = null, email = null, blockheading = null;
			bool enabled = true, locked = false;
			if (userID != null)
			{
				User user = User.Select(userID.Value);
				username = user.Username;
				firstname = user.FirstName;
				surname = user.Surname;
				email = user.Email;
				enabled = user.Enabled;
				locked = user.Locked;
			}
			blockheading = "User Details";
			AjaxForm form = new AjaxForm("UserEditForm");
			if (userID != null) form.RecordID = userID.Value;
			AjaxFormFieldBlock block = new AjaxFormFieldBlock("MainUserFields", blockheading);
			block.Add(new AjaxFormInputField("Username", "Username", 50, locked, null, "width:150px;", username, null, string.Format(fErr, "username"), true, 0));
			block.Add(new AjaxFormInputField("Password", "Password", 50, false, null, "width:150px;", null, null, pErr, true, 1));
			block.Add(new AjaxFormInputField("First Name", "FirstName", 50, false, null, "width:150px;", firstname, null, null, true, 2));
			block.Add(new AjaxFormInputField("Surname", "Surname", 50, false, null, "width:150px;", surname, null, null, true, 3));
			block.Add(new AjaxFormInputField("Email", "Email", 100, false, null, "width:150px;", email, null, string.Format(fErr, "valid email address"), true, 4));
			block.Add(new AjaxFormCheckboxField("User account is enabled", "Enabled", enabled, locked, null, null, false, 5));
			block.Rank = -10000;
			form.FieldBlocks.Add(block);

			if (!locked && username != SecurityProvider.CurrentUser.Username)
			{
				block = new AjaxFormFieldBlock("Roles", "Assigned Roles");
				block.Rank = 998;
				List<RoleState> roleStates = SecurityProvider.DataLayer.ListAllRolesAgainstUser(userID == null ? 0 : userID.Value);
				List<PermissionTypeState> permissions = SecurityProvider.DataLayer.ListAllPermissionTypesAgainstUser(userID == null ? 0 : userID.Value);

				//IDbCommand cmd = Database.Main.CreateCommand("ListRolePermissionStates", CommandType.StoredProcedure);
				//Database.Main.AddParameter(cmd, "@UserID", userID);
				//DataSet ds = Database.Main.GetDataSet(cmd);
				int c = 0;
				foreach (RoleState state in roleStates)
				{
					// check that the current user has access to assign the specified permission/role
					//if (CurrentUser.HasRole(...) continue;
					block.Add(new AjaxFormCheckboxField(state.Role.Name, state.Role.RoleCode, state.IsAccessible, false, null, null, false, c++));
				}
				if (c > 0) form.FieldBlocks.Add(block);

				block = new AjaxFormFieldBlock("Permissions", "Specific Assigned Permissions");
				block.Rank = 999;
				c = 0;
				foreach (PermissionTypeState state in permissions)
				{
					// check that the current user has access to assign the specified permission/role
					//if (!CurrentUser.HasPermission(row["PermissionTypeCode"].ToString())) continue;
					block.Add(new AjaxFormCheckboxField(state.PermissionType.Description, state.PermissionType.PermissionTypeCode,
						state.PermissionState == PermissionState.Specified, false, null, null, false, c++));
				}
				if (c > 0) form.FieldBlocks.Add(block);
			}
			block = new AjaxFormFieldBlock("SubmitButtons", null);
			AjaxFormButtonGroup buttons = new AjaxFormButtonGroup();
			block.Rank = 10000;
			buttons.AddSubmitButton(null, "Save", "SecurityInterface.OnUserSaved", null);
			if (userID != null)
			{
				if (!locked) buttons.AddButton(null, "Delete", "SecurityInterface.DeleteUser('" + userID.ToString() + "')");
				//buttons.AddButton(null, "Send Password", "SecurityInterface.SendPassword('" + userID.ToString() + "')");
				buttons.AddButton(null, "Cancel", "SecurityInterface.CancelUserEdit()");
			}
			block.Add(buttons);
			form.FieldBlocks.Add(block);

			if (OnUserEditFormLayout != null)
				OnUserEditFormLayout(userID, false, form);

			return form;
		}

		[AjaxMethod(RequiresAuthentication = true)]
		[RequiresPermission(PermissionType.UserAdministrator)]
		public Result DeleteUser(long userID)
		{
			User user = User.Select(userID);
			//if (!CurrentUser.CanModifyUser(user))
			//    return new Result("You don't have permission to modify this user.");
			if (user.Locked)
				return new Result("This user cannot be deleted.");
			return SecurityProvider.DataLayer.Delete(user);
		}

		[AjaxMethod(RequiresAuthentication = true)]
		[RequiresPermission(PermissionType.UserAdministrator)]
		public Result SendPasswordReminder(long userID)
		{
			User user = User.Select(userID);
			try
			{
				throw new NotImplementedException("not implemented yet.");
				//user.SendPasswordReminder(WebUtility.CacheTextFile("resources/passwordreminder.email.txt"));
				//return new Result();
			}
			catch(Exception ex)
			{
				return new Result(ex.Message);
			}
		}
		#endregion

		#region Role Editing
		public class RoleItem : IJSONEncoder
		{
			private string name;
			private long id;

			public RoleItem(string name, long id)
			{
				this.name = name;
				this.id = id;
			}

			public void WriteJSON(StringWriter writer)
			{
				JSON.EncodeCustomObject(writer,
					new KeyValuePair<string, object>("Name", name),
					new KeyValuePair<string, object>("RoleID", id.ToString())
				);
			}
		}

		[AjaxMethod(RequiresAuthentication = true)]
		[RequiresPermission(PermissionType.UserAdministrator)]
		[RequiresPermission(PermissionType.RoleAdministrator)]
		public List<RoleItem> GetAccessibleRoles()
		{
			List<Role> roles = SecurityProvider.DataLayer.ListAccessibleRoles(SecurityProvider.CurrentUser.UserID);
			List<RoleItem> items = new List<RoleItem>();
			foreach (Role role in roles)
				if (!role.Locked && !role.Hidden)
					items.Add(new RoleItem(role.Name, role.RoleID));
			return items;
		}

		[AjaxMethod(RequiresAuthentication = true)]
		[RequiresPermission("USERADMINISTRATOR")]
		[RequiresPermission("ROLEADMINISTRATOR")]
		public AjaxForm GetRoleEditForm(long? roleID)
		{
			Role role;
			if (roleID == null)
				role = new Role();
			else
			{
				role = Role.Select(roleID.Value);
				if (role == null)
					throw new AjaxException("The requested role does not exist in the database.");
				if (role.Locked)
					throw new AjaxException("This is a system role and cannot be modified.");
			}

			AjaxForm form = new AjaxForm("RoleEditForm");
			form.RecordID = roleID;

			AjaxFormFieldBlock block = new AjaxFormFieldBlock("RoleDetails", "Role Details");
			block.Add(new AjaxFormInputField("Role Name",
				"Name", 100, role.Locked, null, null, role.Name, null,
				"function(value){{if(value.length==0) return 'A name is required'; return null;}}",
				true, 0));
			block.Add(new AjaxFormCheckboxField("Role is enabled", "Enabled", role.Enabled, role.Locked, null, null, false, 1));
			block.Rank = 0;
			form.FieldBlocks.Add(block);

			//List<Guid> roleDescendents = new List<Guid>();
			//List<Role> roleDescendents = SecurityProvider.DataLayer.ListDescendentRoles(role.RoleID);
			//IDbCommand cmd = Database.Main.CreateCommand("ListDescendentRoles", CommandType.StoredProcedure);
			//Database.Main.AddParameter(cmd, "@RoleID", role.RoleID);
			//DataSet ds = Database.Main.GetDataSet(cmd);
			//foreach (Role role in roleDescendents)
			//    roleDescendents.Add((Guid)row["RoleID"]);

			List<RoleState> roles = SecurityProvider.DataLayer.ListAllRolesAgainstRole(role.RoleID);
			//cmd = Database.Main.CreateCommand("ListRoleToRoleAssignmentStates", CommandType.StoredProcedure);
			//Database.Main.AddParameter(cmd, "@RoleID", role.RoleID);
			//ds = Database.Main.GetDataSet(cmd);

			block = new AjaxFormFieldBlock("Roles", "Roles that this role should adopt");
			block.Rank = 1;
			int c = 0;
			foreach (RoleState r in roles)
				//if (CurrentUser.HasPermission(row["RoleCode"].ToString()) && !roleDescendents.Contains((Guid)row["RoleID"]))
					block.Add(new AjaxFormCheckboxField(
						r.Role.Name, r.Role.RoleCode, r.State == PermissionState.Inherited, r.Role.Locked, null, null, false, c++));
						//(bool)row["Inherited"], role.Locked, null, null, false, c++));
			if (block.Count > 0)
				form.FieldBlocks.Add(block);

			//cmd = Database.Main.CreateCommand("ListPermissionValuesForRole", CommandType.StoredProcedure);
			//Database.Main.AddParameter(cmd, "@RoleID", role.RoleID);
			//Database.Main.AddParameter(cmd, "@ShowAllPermissions", true);
			//ds = Database.Main.GetDataSet(cmd);

			block = new AjaxFormFieldBlock("Permissions", "Permission Settings");
			c  = 0;
			foreach (PermissionTypeState pts in SecurityProvider.DataLayer.ListAllPermissionTypesAgainstRole(role.RoleID))
				//if (CurrentUser.HasPermission(row["PermissionTypeCode"].ToString()))
				block.Add(new AjaxFormCheckboxField(
					pts.PermissionType.Description, pts.PermissionType.PermissionTypeCode, pts.PermissionState == PermissionState.Specified,
					role.Locked, null, null, false, c++));
					//row["Description"].ToString(), row["PermissionTypeCode"].ToString(),
					//row["Value"] == DBNull.Value ? false : (bool)row["Value"], role.Locked, null, null, false, c++));

			AjaxFormButtonGroup buttons = new AjaxFormButtonGroup();
			block.Rank = 2;
			buttons.Rank = 10000;
			buttons.AddSubmitButton(null, "Save", "SecurityInterface.OnRoleSaved", null);
			if (roleID != null)
				if (!role.Locked) buttons.AddButton(null, "Delete", "SecurityInterface.DeleteRole('" + roleID.ToString() + "')");
			buttons.AddButton(null, "Cancel", "$('security-permissionlist').innerHTML = '';");
			block.Add(buttons);

			if (block.Count > 0)
				form.FieldBlocks.Add(block);

			return form;
		}

		[AjaxMethod(RequiresAuthentication = true)]
		[RequiresPermission(PermissionType.RoleAdministrator)]
		[RequiresPermission(PermissionType.UserAdministrator)]
		public Result DeleteRole(long roleID)
		{
			Role role = Role.Select (roleID);
			if (!SecurityProvider.CurrentUser.HasRole(role.RoleCode))
				return new Result("You don't have permission to modify this role.");
			if (role.Locked)
				return new Result("This role cannot be deleted.");
			return SecurityProvider.DataLayer.Delete(role);
		}
		#endregion
	}
}
