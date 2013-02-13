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
	partial class WebSecurity
	{
		public delegate void UserSavedEventHandler(AjaxFormSubmittedValues form, User user);
		public event UserSavedEventHandler OnUserSaved;
		public event UserSavedEventHandler OnBeforeSaveUser;
		
		void OnValidateField(AjaxFormFieldValidationResponse formArgs)
		{
			if(formArgs.FormName == "UserEditForm")
				ValidateStandardUserField(formArgs, false);
		}

		void OnValidateForm(AjaxFormSubmittedValues form)
		{
			switch (form.FormName)
			{
				case "UserEditForm":
					ValidateStandardUserFormBlock(form.Blocks["MainUserFields"], form.RecordID, false, false);
					break;

				case "RoleEditForm":
					foreach (AjaxFormSubmittedValues.Field fld in form.Blocks["RoleDetails"].Fields.Values)
					{
						switch (fld.Name)
						{
							case "Name":
								if (fld.Value.Trim().Length == 0)
									fld.ErrorMessage = "A role name is required";
								break;
						}
					}
					break;
			}
		}

		void OnSaveForm(AjaxFormSubmittedValues form)
		{
			List<string> roleCodes = new List<string>(), permissionTypeCodes = new List<string>();
			switch (form.FormName)
			{
				case "UserEditForm":
					if (!SecurityProvider.DataLayer.DoesUserHavePermission(SecurityProvider.CurrentUser.UserID, PermissionType.UserAdministrator))
						return;
					AjaxFormSubmittedValues.Block block = form.Blocks["MainUserFields"];
					string pw = block.Fields["Password"].Value;
					bool enabled = block.Fields["Enabled"].Value == "True";
					if (pw.Length == 0) pw = null;
					User user;

					if (form.RecordID == null)
					{
						user = new User(
							SecurityProvider.ClientSpaceID,
							block.Fields["Username"].Value,
							pw,
							block.Fields["FirstName"].Value,
							block.Fields["Surname"].Value,
							block.Fields["Email"].Value,
							enabled, false, false, 0);
						Result result = SecurityProvider.DataLayer.Store(user);
						if (!result.Succeeded)
							throw new AjaxException(result.Message);
						if (OnUserSaved != null)
							OnUserSaved(form, user);

						form.RecordID = user.UserID;
					}
					else
					{
						user = User.Select(form.RecordID.Value);
						//if (!CurrentUser.CanModifyUser(user))
						//    throw new AjaxException("You don't have access to modify that user.");
						user.Username = block.Fields["Username"].Value;
						if (pw != null) user.Password = pw;
						user.FirstName = block.Fields["FirstName"].Value;
						user.Surname = block.Fields["Surname"].Value;
						user.Email = block.Fields["Email"].Value;
						user.Enabled = enabled;
						SecurityProvider.DataLayer.Store(user);
						//user.Save();
						if (OnUserSaved != null)
							OnUserSaved(form, user);

						if (user.Locked) return; // don't muck with permissions/roles
					}

					if (user.Username != SecurityProvider.CurrentUser.Username) // users can't alter their own permissions
					{
						if (form.Blocks.ContainsKey("Roles"))
							foreach (KeyValuePair<string, AjaxFormSubmittedValues.Field> kvp in form.Blocks["Roles"].Fields)
								if (SecurityProvider.CurrentUser.HasRole(kvp.Value.Name)) //make sure the logged in user has the right to assign this role
									if (kvp.Value.Value == "True")
										roleCodes.Add(kvp.Value.Name);
										//sql.AppendFormat("exec AssignUserToRole '{0}', '{1}'\r\n", user.UserID, kvp.Value.Name.Replace("'", "''"));
						if (form.Blocks.ContainsKey("Permissions"))
							foreach (KeyValuePair<string, AjaxFormSubmittedValues.Field> kvp in form.Blocks["Permissions"].Fields)
								if (SecurityProvider.CurrentUser.HasRole(kvp.Value.Name)) //make sure the logged in user has the right to assign this role
									if (kvp.Value.Value == "True")
										permissionTypeCodes.Add(kvp.Value.Name);
										//sql.AppendFormat("exec AssignPermission '{0}', null, '{1}'\r\n", kvp.Value.Name.Replace("'", "''"), user.UserID);
						//if (sql.Length == 0) return;

						SecurityProvider.DataLayer.SetRolesAndPermissionsForUser(user.UserID, roleCodes, permissionTypeCodes);
						//user.RevokeRolesAndPermissions(); // revoke any pre-existing permissions/roles before we assign the new ones
						//Database.Main.CreateCommand(sql.ToString(), CommandType.Text).ExecuteNonQuery();
					}
					break;

				case "RoleEditForm":
					if (!SecurityProvider.DataLayer.DoesUserHavePermission(SecurityProvider.CurrentUser.UserID, PermissionType.RoleAdministrator))
						return;
					block = form.Blocks["RoleDetails"];
					string name = block.Fields["Name"].Value;
					enabled = block.Fields["Enabled"].Value == "True";
					Role role;
					if (form.RecordID == null)
					{
						role = new Role();
						role.RoleID = DatabaseManager.GetUniqueID();
						role.RoleCode = role.RoleID.ToString(); // role codes are only used by system roles
						role.ClientSpaceID = SecurityProvider.ClientSpaceID;
					}
					else
					{
						role = Role.Select(form.RecordID.Value);
						if (role == null) return;
						if (role.Locked) return; // locked roles aren't supposed to be edited by users
					}
					role.Name = name;
					role.Enabled = enabled;
					SecurityProvider.DataLayer.Store(role);
					//((SecurityProvider)Core.Instance["SecurityProvider"]).SaveRole(role);

					//sql = new StringBuilder();
					if (form.Blocks.ContainsKey("Roles"))
						foreach (KeyValuePair<string, AjaxFormSubmittedValues.Field> kvp in form.Blocks["Roles"].Fields)
							if (SecurityProvider.CurrentUser.HasRole(kvp.Value.Name)) //make sure the logged in user has the right to assign this role
								if (kvp.Value.Value == "True")
									roleCodes.Add(kvp.Value.Name);
									//sql.AppendFormat("exec InheritRoleFrom '{0}', '{1}'\r\n", role.RoleID, kvp.Value.Name.Replace("'", "''"));
					if (form.Blocks.ContainsKey("Permissions"))
						foreach (KeyValuePair<string, AjaxFormSubmittedValues.Field> kvp in form.Blocks["Permissions"].Fields)
							if (SecurityProvider.CurrentUser.HasRole(kvp.Value.Name)) //make sure the logged in user has the right to assign this role
								if (kvp.Value.Value == "True")
									permissionTypeCodes.Add(kvp.Value.Name);
									//sql.AppendFormat("exec AssignPermission '{0}', null, '{1}'\r\n", kvp.Value.Name.Replace("'", "''"), role.RoleID);

					SecurityProvider.DataLayer.SetRolesAndPermissionsForRole(role.RoleID, roleCodes, permissionTypeCodes);
					//role.RevokeRolesAndPermissions(); // revoke any pre-existing permissions/roles before we assign the new ones
					//if (sql.Length == 0) return;
					//Database.Main.CreateCommand(sql.ToString(), CommandType.Text).ExecuteNonQuery();
					break;
			}
		}

		public void FillStandardUserFormBlock(AjaxFormFieldBlock block, bool plainTextPassword, bool multilingual, bool requireFullName, bool allowUsernameEditing)
		{
			FillStandardUserFormBlock(block, null, plainTextPassword, multilingual, requireFullName, allowUsernameEditing);
		}

		public void FillStandardUserFormBlock(AjaxFormFieldBlock block, User user, bool plainTextPassword, bool multilingual, bool requireFullName, bool allowUsernameEditing)
		{
			bool newUser = user == null;

			string labelUsername = multilingual ? "{?form-label-username?}" : "Username";
			string labelPassword = multilingual ? "{?form-label-password?}" : "Password";
			string labelFirstName = multilingual ? "{?form-label-firstname?}" : "FirstName";
			string labelSurname = multilingual ? "{?form-label-surname?}" : "Surname";
			string labelEmail = multilingual ? "{?form-label-email?}" : "Email";

			string errNoUsername = multilingual ? "{?form-error-require-username?}" : "Please enter a username";
			string errNoFirstName = multilingual ? "{?form-error-require-firstname?}" : "Please enter your first name";
			string errNoSurname = multilingual ? "{?form-error-require-surname?}" : "Please enter your surname";
			string errNoEmail = multilingual ? "{?form-error-require-email?}" : "Please enter your email address";
			string errNoPassword = multilingual ? "{?form-error-require-password?}" : "Please enter your email password";

			string fErr = "function(value){{if(value.length==0) return '{0}'; return null;}}";
			string pErr = !newUser ? null : string.Format(fErr, errNoPassword);
			string fnErr = !requireFullName ? null : string.Format(fErr, errNoFirstName);
			string snErr = !requireFullName ? null : string.Format(fErr, errNoSurname);

			if (newUser) user = new User();
			bool locked = user.Locked;
			if(allowUsernameEditing)
				block.Add(new AjaxFormInputField(labelUsername, "Username", 50, locked, null, "width:150px;", user.Username, null, string.Format(fErr, errNoUsername), true, 0));
			if (plainTextPassword)
				block.Add(new AjaxFormInputField(labelPassword, "Password", 50, false, null, "width:150px;", null, null, pErr, true, 1));
			else
				block.Add(new AjaxFormPasswordField(labelPassword, 50, null, "width:73px", 1, multilingual, newUser, !newUser));
			block.Add(new AjaxFormInputField(labelFirstName, "FirstName", 50, false, null, "width:150px;", user.FirstName, null, fnErr, true, 2));
			block.Add(new AjaxFormInputField(labelSurname, "Surname", 50, false, null, "width:150px;", user.Surname, null, snErr, true, 3));
			block.Add(new AjaxFormInputField(labelEmail, "Email", 100, false, null, "width:150px;", user.Email, null, string.Format(fErr, errNoEmail), true, 4));
		}

		public void ValidateStandardUserField(AjaxFormFieldValidationResponse formArgs, bool multilingual)
		{
			string msg = "";
			switch (formArgs.FieldName)
			{
				case "Username":
					if (SecurityProvider.DataLayer.IsUsernameTaken(SecurityProvider.ClientSpaceID, formArgs.FieldValue, formArgs.RecordID))
						msg = multilingual ? "{?form-error-username-already-exists?}" : "That username is already in use";
					break;

				case "Email":
					if (SecurityProvider.DataLayer.IsEmailAddressTaken(SecurityProvider.ClientSpaceID, formArgs.FieldValue, formArgs.RecordID))
						msg = multilingual ? "{?form-error-emailaddress-already-exists?}" : "That email address is already in use";
					else if(!StringUtilities.Validation.IsEmailAddress(formArgs.FieldValue))
						msg = multilingual ? "{?form-error-emailaddress-invalid?}" : "That is not an email address";
					break;
			}
			if (msg.Length > 0)
			{
				formArgs.IsValid = false;
				formArgs.ErrorMessage = msg;
			}
		}

		public void ValidateStandardUserFormBlock(AjaxFormSubmittedValues.Block block, long? userID, bool multilingual, bool requireFullName)
		{
			foreach (AjaxFormSubmittedValues.Field fld in block.Fields.Values)
			{
				switch (fld.Name)
				{
					case "Username":
						if (fld.Value.Trim().Length == 0)
							fld.ErrorMessage = multilingual ? "{?form-error-require-username?}" : "A username is required";
						else if (SecurityProvider.DataLayer.IsUsernameTaken(SecurityProvider.ClientSpaceID, fld.Value, userID))
							fld.ErrorMessage = multilingual ? "{?form-error-username-already-exists?}" : "That username is already in use";
						break;

					case "Password":
						if (userID == null && fld.Value.Length == 0)
							fld.ErrorMessage = multilingual ? "{?form-error-require-password?}" : "A password is required";
						break;

					case "Password1":
						if (block.Fields["Password2"].Value != fld.Value)
							fld.ErrorMessage = multilingual ? "{?form-error-different-passwords?}" : "The passwords entered must match.";
						else if (fld.Value.Length == 0 && userID == null)
							fld.ErrorMessage = multilingual ? "{?form-error-require-password?}" : "A password is required.";
						break;

					case "FirstName":
						if (fld.Value.Trim().Length == 0 && requireFullName)
							fld.ErrorMessage = multilingual ? "{?form-error-require-firstname?}" : "A first name is required";
						break;

					case "Surname":
						if (fld.Value.Trim().Length == 0 && requireFullName)
							fld.ErrorMessage = multilingual ? "{?form-error-require-surname?}" : "A surname is required";
						break;

					case "Email":
						if (fld.Value.Trim().Length == 0)
							fld.ErrorMessage = multilingual ? "{?form-error-require-email?}" : "An email address is required";
						else if (!StringUtilities.Validation.IsEmailAddress(fld.Value))
							fld.ErrorMessage = multilingual ? "{?form-error-emailaddress-invalid?}" : "That is not an email address";
						else if (SecurityProvider.DataLayer.IsEmailAddressTaken(SecurityProvider.ClientSpaceID, fld.Value, userID))
							fld.ErrorMessage = multilingual ? "{?form-error-emailaddress-already-exists?}" : "That email address is already in use";
						break;
				}
			}
		}

		public User SaveStandardUserFormDetails(AjaxFormSubmittedValues form, string blockName, bool? enabled)
		{
			AjaxFormSubmittedValues.Block block = form.Blocks[blockName];
			string pw;
			if (block.Fields.ContainsKey("Password1"))
				pw = block.Fields["Password1"].Value;
			else
				pw = block.Fields["Password"].Value;
			if (pw.Length == 0) pw = null;

			User user;
			if (form.RecordID == null)
			{
				user = new User(
					SecurityProvider.ClientSpaceID,
					block.Fields["Username"].Value,
					pw,
					block.Fields["FirstName"].Value,
					block.Fields["Surname"].Value,
					block.Fields["Email"].Value,
					enabled == null ? (block.Fields["Enabled"].Value == "True") : enabled.Value,
					false, false, 0);
				if (OnBeforeSaveUser != null)
					OnBeforeSaveUser(form, user);
				SecurityProvider.DataLayer.Store(user);
				form.RecordID = user.UserID;
			}
			else
			{
				long myuserid = SecurityProvider.CurrentUser.UserID;
				// string myoldusername = CurrentUser.Username;
				user = User.Select(form.RecordID.Value);
				// user.Username = block.Fields["Username"].Value;
				if (pw != null) user.Password = pw;
				user.FirstName = block.Fields["FirstName"].Value;
				user.Surname = block.Fields["Surname"].Value;
				user.Email = block.Fields["Email"].Value;
				user.Enabled = enabled == null ? (block.Fields["Enabled"].Value == "True") : enabled.Value;
				if (OnBeforeSaveUser != null)
					OnBeforeSaveUser(form, user);
				SecurityProvider.DataLayer.Store(user);

				/* we're not going to allow the user to change their username, so this code is commented out
				if (myuserid == user.UserID && (pw != null || user.Username != myoldusername)) // changing username or password causes login cookie to become invalid
					WebAuthentication.Instance.WriteAuthenticationCookie(
						user.Username,
						pw != null ? Crypto.EncryptOneWay(pw) : user.PasswordHash,
						WebAuthentication.Instance.StoreAjaxAuthKey(user.Username),
						1440); */
			}
			return user;
		}
	}
}
