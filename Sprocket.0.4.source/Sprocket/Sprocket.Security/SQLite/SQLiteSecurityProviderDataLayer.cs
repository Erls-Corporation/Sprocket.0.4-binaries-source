using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;
using System.Transactions;

using Sprocket.Data;
using Sprocket.Utility;

namespace Sprocket.Security.SQLite
{
	public class SQLiteSecurityProviderDataLayer : ISecurityProviderDataLayer
	{
		public Type DatabaseHandlerType
		{
			get { return typeof(SQLiteDatabase); }
		}

		private string GetSQL(string name)
		{
			return ResourceLoader.LoadTextResource("Sprocket.Security.SQLite." + name + ".sql");
		}

		public void InitialiseDatabase(Result result)
		{
			if (!result.Succeeded)
				return;

			using (SQLiteConnection connection = new SQLiteConnection(DatabaseManager.DatabaseEngine.ConnectionString))
			{
				connection.Open();
				using (TransactionScope scope = new TransactionScope())
				{
					try
					{
						SQLiteCommand cmd = connection.CreateCommand();
						cmd.CommandText = GetSQL("SQLite Tables");
						cmd.ExecuteNonQuery();

						cmd.CommandText = GetSQL("Insert First ClientSpace");
						cmd.Parameters.Add(new SQLiteParameter("@ClientSpaceID", SecurityProvider.ClientSpaceID));
						int n = cmd.ExecuteNonQuery();

						if (n > 0) // then a new client was inserted, so insert accompanying data
						{
						}
						scope.Complete();
					}
					catch (Exception ex)
					{
						result.SetFailed("Unable to initialise SQLite database for SecurityProvider: " + ex.Message);
					}
				}
			}
			return;
		}

		public bool Authenticate(string username, string passwordHash)
		{
			return Authenticate(SecurityProvider.ClientSpaceID, username, passwordHash);
		}

		public bool Authenticate(long clientSpaceID, string username, string passwordHash)
		{
			using (SQLiteConnection connection = new SQLiteConnection(DatabaseManager.DatabaseEngine.ConnectionString))
			{
				SQLiteCommand cmd = connection.CreateCommand();
				cmd.CommandText = GetSQL("Authenticate");
				cmd.Parameters.Add(new SQLiteParameter("@Username", username));
				cmd.Parameters.Add(new SQLiteParameter("@PasswordHash", passwordHash));
				cmd.Parameters.Add(new SQLiteParameter("@ClientSpaceID", clientSpaceID));
				SQLiteDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
				bool result;
				if (dr.Read())
					if (dr.IsDBNull(0))
						result = false;
					else
						result = Convert.ToBoolean(dr[0]);
				else
					result = false;
				dr.Close();
				return result;
			}
		}

		public bool IsEmailAddressTaken(long clientSpaceID, string email)
		{
			using (SQLiteConnection connection = new SQLiteConnection(DatabaseManager.DatabaseEngine.ConnectionString))
			{
				SQLiteCommand cmd = connection.CreateCommand();
				cmd.CommandText = GetSQL("IsEmailAddressTaken");
				cmd.Parameters.Add(new SQLiteParameter("@Email", email));
				cmd.Parameters.Add(new SQLiteParameter("@ClientSpaceID", clientSpaceID));
				SQLiteDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
				bool result;
				dr.Read();
				result = Convert.ToBoolean(dr[0]);
				dr.Close();
				return result;
			}
		}

		public bool IsUsernameTaken(long clientSpaceID, string username)
		{
			using (SQLiteConnection connection = new SQLiteConnection(DatabaseManager.DatabaseEngine.ConnectionString))
			{
				SQLiteCommand cmd = connection.CreateCommand();
				cmd.CommandText = GetSQL("IsUsernameTaken");
				cmd.Parameters.Add(new SQLiteParameter("@Username", username));
				cmd.Parameters.Add(new SQLiteParameter("@ClientSpaceID", clientSpaceID));
				SQLiteDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
				bool result;
				dr.Read();
				result = Convert.ToBoolean(dr[0]);
				dr.Close();
				return result;
			}
		}

		public bool IsEmailAddressTaken(long clientSpaceID, string email, long? excludeUserID)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public bool IsUsernameTaken(long clientSpaceID, string username, long? excludeUserID)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public Result Store(ClientSpace client)
		{
			using (SQLiteConnection connection = new SQLiteConnection(DatabaseManager.DatabaseEngine.ConnectionString))
			{
				connection.Open();
				using (TransactionScope scope = new TransactionScope())
				{
					try
					{
						SQLiteCommand cmd = connection.CreateCommand();
						cmd.CommandText = GetSQL("Store ClientSpace");
						cmd.Parameters.Add(new SQLiteParameter("@ClientSpaceID", client.ClientSpaceID));
						cmd.Parameters.Add(new SQLiteParameter("@Name", client.Name));
						cmd.Parameters.Add(new SQLiteParameter("@Enabled", client.Enabled));
						cmd.Parameters.Add(new SQLiteParameter("@PrimaryUserID", client.PrimaryUserID));
						cmd.ExecuteNonQuery();
						scope.Complete();
					}
					catch(Exception ex)
					{
						return new Result("Unable to store ClientSpace: " + ex.Message);
					}
				}
			}
			return new Result();
		}

		public Result Store(User user)
		{
			using (SQLiteConnection connection = new SQLiteConnection(DatabaseManager.DatabaseEngine.ConnectionString))
			{
				connection.Open();
				using (TransactionScope scope = new TransactionScope())
				{
					try
					{
						SQLiteCommand cmd = connection.CreateCommand();
						cmd.CommandText = GetSQL("Store User");
						cmd.Parameters.Add(new SQLiteParameter("@UserID", user.UserID));
						cmd.Parameters.Add(new SQLiteParameter("@ClientSpaceID", user.ClientSpaceID));
						cmd.Parameters.Add(new SQLiteParameter("@Username", user.Username));
						cmd.Parameters.Add(new SQLiteParameter("@PasswordHash", user.PasswordHash));
						cmd.Parameters.Add(new SQLiteParameter("@FirstName", user.FirstName));
						cmd.Parameters.Add(new SQLiteParameter("@Surname", user.Surname));
						cmd.Parameters.Add(new SQLiteParameter("@Email", user.Email));
						cmd.Parameters.Add(new SQLiteParameter("@LocalTimeOffsetHours", user.LocalTimeOffsetHours));
						cmd.Parameters.Add(new SQLiteParameter("@Enabled", user.Enabled));
						cmd.Parameters.Add(new SQLiteParameter("@Hidden", user.Hidden));
						cmd.Parameters.Add(new SQLiteParameter("@Locked", user.Locked));
						cmd.Parameters.Add(new SQLiteParameter("@Deleted", user.Deleted));
						cmd.Parameters.Add(new SQLiteParameter("@Activated", user.Activated));
						cmd.Parameters.Add(new SQLiteParameter("@ActivationReminderSent", user.ActivationReminderSent));
						cmd.Parameters.Add(new SQLiteParameter("@Created", user.Created));
						cmd.ExecuteNonQuery();
						scope.Complete();
					}
					catch (Exception ex)
					{
						return new Result("Unable to store User: " + ex.Message);
					}
				}
			}
			return new Result();
		}

		public Result Store(Role role)
		{
			using (SQLiteConnection connection = new SQLiteConnection(DatabaseManager.DatabaseEngine.ConnectionString))
			{
				connection.Open();
				using (TransactionScope scope = new TransactionScope())
				{
					try
					{
						SQLiteCommand cmd = connection.CreateCommand();
						cmd.CommandText = GetSQL("Store Role");
						cmd.Parameters.Add(new SQLiteParameter("@RoleID", role.RoleID));
						cmd.Parameters.Add(new SQLiteParameter("@RoleCode", role.RoleCode));
						cmd.Parameters.Add(new SQLiteParameter("@ClientSpaceID", role.ClientSpaceID));
						cmd.Parameters.Add(new SQLiteParameter("@Name", role.Name));
						cmd.Parameters.Add(new SQLiteParameter("@Enabled", role.Enabled));
						cmd.Parameters.Add(new SQLiteParameter("@Locked", role.Locked));
						cmd.Parameters.Add(new SQLiteParameter("@Hidden", role.Hidden));
						cmd.ExecuteNonQuery();
						scope.Complete();
					}
					catch (Exception ex)
					{
						return new Result("Unable to store Role: " + ex.Message);
					}
				}
			}
			return new Result();
		}

		public Result Store(PermissionType permissionType)
		{
			using (SQLiteConnection connection = new SQLiteConnection(DatabaseManager.DatabaseEngine.ConnectionString))
			{
				connection.Open();
				using (TransactionScope scope = new TransactionScope())
				{
					try
					{
						SQLiteCommand cmd = connection.CreateCommand();
						cmd.CommandText = GetSQL("Store PermissionType");
						cmd.Parameters.Add(new SQLiteParameter("@PermissionTypeID", permissionType.PermissionTypeID));
						cmd.Parameters.Add(new SQLiteParameter("@PermissionTypeCode", permissionType.PermissionTypeCode));
						cmd.Parameters.Add(new SQLiteParameter("@Description", permissionType.Description));
						cmd.Parameters.Add(new SQLiteParameter("@DefaultValue", permissionType.DefaultValue));
						cmd.ExecuteNonQuery();
						scope.Complete();
					}
					catch (Exception ex)
					{
						return new Result("Unable to store PermissionType: " + ex.Message);
					}
				}
			}
			return new Result();
		}

		public event InterruptableEventHandler<ClientSpace> OnBeforeDeleteClientSpace;
		public event NotificationEventHandler<ClientSpace> OnClientSpaceDeleted;
		public event InterruptableEventHandler<User> OnBeforeDeleteUser;
		public event NotificationEventHandler<User> OnUserDeleted;
		public event InterruptableEventHandler<Role> OnBeforeDeleteRole;
		public event NotificationEventHandler<Role> OnRoleDeleted;
		public event InterruptableEventHandler<PermissionType> OnBeforeDeletePermissionType;
		public event NotificationEventHandler<PermissionType> OnPermissionTypeDeleted;

		public Result Delete(ClientSpace client)
		{
			Result result = new Result();
			if (OnBeforeDeleteClientSpace != null)
				OnBeforeDeleteClientSpace(client, result);
			if (result.Succeeded)
			{
				using (SQLiteConnection connection = new SQLiteConnection(DatabaseManager.DatabaseEngine.ConnectionString))
				{
					connection.Open();
					using (TransactionScope scope = new TransactionScope())
					{
						try
						{
							SQLiteCommand cmd = connection.CreateCommand();
							cmd.CommandText = "DELETE FROM ClientSpaces WHERE ClientSpaceID = @ClientSpaceID";
							cmd.Parameters.Add(new SQLiteParameter("@ClientSpaceID", client.ClientSpaceID));
							cmd.ExecuteNonQuery();
							scope.Complete();
						}
						catch (Exception ex)
						{
							return new Result("Unable to delete ClientSpace: " + ex.Message);
						}
					}
				}
				if (OnClientSpaceDeleted != null)
					OnClientSpaceDeleted(client);
			}
			return result;
		}

		public Result Delete(User user)
		{
			Result result = new Result();
			if (OnBeforeDeleteUser != null)
				OnBeforeDeleteUser(user, result);
			if (result.Succeeded)
			{
				using (SQLiteConnection connection = new SQLiteConnection(DatabaseManager.DatabaseEngine.ConnectionString))
				{
					connection.Open();
					using (TransactionScope scope = new TransactionScope())
					{
						try
						{
							SQLiteCommand cmd = connection.CreateCommand();
							cmd.CommandText = "DELETE FROM Users WHERE UserID = @UserID";
							cmd.Parameters.Add(new SQLiteParameter("@UserID", user.UserID));
							cmd.ExecuteNonQuery();
							scope.Complete();
						}
						catch (Exception ex)
						{
							return new Result("Unable to delete User: " + ex.Message);
						}
					}
				}
				if (OnUserDeleted != null)
					OnUserDeleted(user);
			}
			return result;
		}

		public Result Delete(Role role)
		{
			Result result = new Result();
			if (OnBeforeDeleteRole != null)
				OnBeforeDeleteRole(role, result);
			if (result.Succeeded)
			{
				using (SQLiteConnection connection = new SQLiteConnection(DatabaseManager.DatabaseEngine.ConnectionString))
				{
					connection.Open();
					using (TransactionScope scope = new TransactionScope())
					{
						try
						{
							SQLiteCommand cmd = connection.CreateCommand();
							cmd.CommandText = "DELETE FROM Roles WHERE RoleID = @RoleID";
							cmd.Parameters.Add(new SQLiteParameter("@RoleID", role.RoleID));
							cmd.ExecuteNonQuery();
							scope.Complete();
						}
						catch (Exception ex)
						{
							return new Result("Unable to delete Role: " + ex.Message);
						}
					}
				}
				if (OnRoleDeleted != null)
					OnRoleDeleted(role);
			}
			return result;
		}

		public Result Delete(PermissionType permissionType)
		{
			Result result = new Result();
			if (OnBeforeDeletePermissionType != null)
				OnBeforeDeletePermissionType(permissionType, result);
			if (result.Succeeded)
			{
				using (SQLiteConnection connection = new SQLiteConnection(DatabaseManager.DatabaseEngine.ConnectionString))
				{
					connection.Open();
					using (TransactionScope scope = new TransactionScope())
					{
						try
						{
							SQLiteCommand cmd = connection.CreateCommand();
							cmd.CommandText = "DELETE FROM PermissionTypes WHERE PermissionTypeID = @PermissionTypeID";
							cmd.Parameters.Add(new SQLiteParameter("@PermissionTypeID", permissionType.PermissionTypeID));
							cmd.ExecuteNonQuery();
							scope.Complete();
						}
						catch (Exception ex)
						{
							return new Result("Unable to delete PermissionType: " + ex.Message);
						}
					}
				}
				if (OnPermissionTypeDeleted != null)
					OnPermissionTypeDeleted(permissionType);
			}
			return result;
		}

		public long? GetRoleIDFromRoleCode(Guid clientSpaceID, string roleCode)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public bool DoesRoleInheritRole(long thisRoleID, long doesItInheritRoleID)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void InheritRoleFrom(long thisRoleID, long inheritFromRoleID)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void DisinheritRoleFrom(long thisRoleID, long disinheritFromRoleID)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public List<Role> ListInheritedRoles(long thisRoleID)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public List<Role> ListUserRoles(long userID)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public bool IsUserInRole(long userID, string roleCode)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void AssignRoleToUser(long userID, string roleCode)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void UnassignRoleFromUser(long userID, string roleCode)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void AssignPermissionToUser(long userID, string permissionTypeCode)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void AssignPermissionToRole(long roleID, string permissionTypeCode)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public bool DoesUserHavePermission(long userID, string permissionTypeCode)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void RemoveRolesAndPermissionsFromUser(long userID)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void RemoveRolesAndPermissionsFromRole(long roleID)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public List<Role> ListAccessibleRoles(long userID)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public List<PermissionTypeState> ListPermissionsForUser(long userID)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public List<PermissionTypeState> ListPermissionsForRole(long roleID)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public List<PermissionTypeState> ListAllPermissionTypesAgainstUser(long userID)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public List<PermissionTypeState> ListAllPermissionTypesAgainstRole(long roleID)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public List<RoleState> ListAllRolesAgainstRole(long roleID)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public List<Role> ListDescendentRoles(long roleID)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public Result InitialiseClientSpace(long clientSpaceID)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public ClientSpace SelectClientSpace(long clientSpaceID)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public User SelectUser(long clientSpaceID, string username)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public User SelectUser(long userID)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public List<RoleState> ListAllRolesAgainstUser(long userID)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public Role SelectRole(long clientSpaceID, string roleCode)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		#region ISecurityProviderDataLayer Members


		public Role SelectRole(long roleID)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		#endregion

		#region ISecurityProviderDataLayer Members


		public void SetRolesAndPermissionsForUser(long userID, List<string> roleCodes, List<string> permissionTypeCodes)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void SetRolesAndPermissionsForRole(long roleID, List<string> roleCodes, List<string> permissionTypeCodes)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		#endregion

		#region ISecurityProviderDataLayer Members


		public List<User> FilterUsers(string partUsername, string partFirstName, string partSurname, string partEmail, int? maxResults, long? editableByUserID, bool? activated, out int totalMatches)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		#endregion

		#region ISecurityProviderDataLayer Members


		public Result ActivateUser(string activationCode, out long userID)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public Result SetEmailChangeRequest(long userID, string newEmailAddress, string activationCode)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		#endregion

		#region ISecurityProviderDataLayer Members


		public EmailChangeRequest SelectEmailChangeRequest(long userID)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		#endregion
	}
}
