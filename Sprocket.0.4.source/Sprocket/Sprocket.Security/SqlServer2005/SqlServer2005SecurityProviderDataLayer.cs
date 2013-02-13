using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using Sprocket.Data;
using Sprocket;
using Sprocket.Utility;
using System.Transactions;

namespace Sprocket.Security
{
	public class SqlServer2005SecurityProviderDataLayer : ISecurityProviderDataLayer
	{
		public Type DatabaseHandlerType
		{
			get { return typeof(SqlServer2005Database); }
		}

		public void InitialiseDatabase(Result result)
		{
			if (!result.Succeeded)
				return;

			SqlConnection conn = null;
			try
			{
				using (TransactionScope scope = new TransactionScope())
				{
					conn = (SqlConnection)DatabaseManager.DatabaseEngine.GetConnection();
					SqlServer2005Database db = (SqlServer2005Database)DatabaseManager.DatabaseEngine;
					Result r = db.ExecuteScript(conn, ResourceLoader.LoadTextResource("Sprocket.Security.SqlServer2005.tables.sql"));
					if (!r.Succeeded)
					{
						result.SetFailed(r.Message);
						return;
					}
					r = db.ExecuteScript(conn, ResourceLoader.LoadTextResource("Sprocket.Security.SqlServer2005.procedures.sql"));
					if (!r.Succeeded)
					{
						result.SetFailed(r.Message);
						return;
					}
					scope.Complete();
				}
			}
			finally
			{
				DatabaseManager.DatabaseEngine.ReleaseConnection(conn);
			}
			return;
		}

		public Result InitialiseClientSpace(long clientSpaceID)
		{
			SqlConnection conn = null;
			try
			{
				using (TransactionScope scope = new TransactionScope())
				{
					conn = (SqlConnection)DatabaseManager.DatabaseEngine.GetConnection();
					try
					{
						SqlCommand cmd = new SqlCommand("InitialiseClientSpace", conn);
						cmd.CommandType = CommandType.StoredProcedure;
						cmd.Parameters.Add(new SqlParameter("@ClientSpaceID", clientSpaceID));
						cmd.Parameters.Add(new SqlParameter("@PasswordHash", Crypto.EncryptOneWay("password")));
						cmd.ExecuteNonQuery();
					}

					catch (Exception ex)
					{
						return new Result("Failed to initialise ClientSpace: " + ex.Message);
					}
					scope.Complete();
				}
			}
			finally
			{
				DatabaseManager.DatabaseEngine.ReleaseConnection(conn);
			}
			return new Result();
		}

		public bool Authenticate(string username, string passwordHash)
		{
			return Authenticate(SecurityProvider.ClientSpaceID, username, passwordHash);
		}

		public bool Authenticate(long clientSpaceID, string username, string passwordHash)
		{
			using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Suppress))
			{
				using (SqlConnection conn = new SqlConnection(DatabaseManager.DatabaseEngine.ConnectionString))
				{
					conn.Open();
					SqlCommand cmd = new SqlCommand("Authenticate", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add(new SqlParameter("@ClientSpaceID", clientSpaceID));
					cmd.Parameters.Add(new SqlParameter("@Username", username));
					cmd.Parameters.Add(new SqlParameter("@PasswordHash", passwordHash));
					SqlParameter prm = new SqlParameter("@IsValid", SqlDbType.Bit);
					prm.Direction = ParameterDirection.Output;
					cmd.Parameters.Add(prm);
					cmd.ExecuteNonQuery();
					conn.Close();
					return (bool)prm.Value;
				}
			}
		}

		public bool IsEmailAddressTaken(long clientSpaceID, string email)
		{
			return IsEmailAddressTaken(clientSpaceID, email, 0);
		}

		public bool IsUsernameTaken(long clientSpaceID, string username)
		{
			return IsUsernameTaken(clientSpaceID, username, 0);
		}

		public bool IsEmailAddressTaken(long clientSpaceID, string email, long? excludeUserID)
		{
			using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Suppress))
			{
				using (SqlConnection conn = new SqlConnection(DatabaseManager.DatabaseEngine.ConnectionString))
				{
					conn.Open();
					SqlCommand cmd = new SqlCommand("IsEmailAddressAvailable", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add(new SqlParameter("@ClientSpaceID", clientSpaceID));
					cmd.Parameters.Add(new SqlParameter("@Email", email));
					cmd.Parameters.Add(new SqlParameter("@ExcludeUserID", excludeUserID == null ? 0 : excludeUserID.Value));
					SqlParameter prm = new SqlParameter("@Available", SqlDbType.Bit);
					prm.Direction = ParameterDirection.Output;
					cmd.Parameters.Add(prm);
					cmd.ExecuteNonQuery();
					conn.Close();
					return !(bool)prm.Value;
				}
			}
		}

		public bool IsUsernameTaken(long clientSpaceID, string username, long? excludeUserID)
		{
			using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Suppress))
			{
				using (SqlConnection conn = new SqlConnection(DatabaseManager.DatabaseEngine.ConnectionString))
				{
					conn.Open();
					SqlCommand cmd = new SqlCommand("IsUsernameAvailable", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add(new SqlParameter("@ClientSpaceID", clientSpaceID));
					cmd.Parameters.Add(new SqlParameter("@Username", username));
					cmd.Parameters.Add(new SqlParameter("@ExcludeUserID", excludeUserID == null ? 0 : excludeUserID.Value));
					SqlParameter prm = new SqlParameter("@Available", SqlDbType.Bit);
					prm.Direction = ParameterDirection.Output;
					cmd.Parameters.Add(prm);
					cmd.ExecuteNonQuery();
					conn.Close();
					return !(bool)prm.Value;
				}
			}
		}

		public Result ActivateUser(string activationCode, out long userID)
		{
			SqlConnection conn = null;
			userID = 0;
			try
			{
				using (TransactionScope scope = new TransactionScope())
				{
					conn = (SqlConnection)DatabaseManager.DatabaseEngine.GetConnection();
					try
					{
						SqlCommand cmd = new SqlCommand("ActivateUser", conn);
						cmd.CommandType = CommandType.StoredProcedure;
						SqlParameter prm = new SqlParameter("@Success", SqlDbType.Bit);
						prm.Direction = ParameterDirection.Output;
						cmd.Parameters.Add(prm);
						SqlParameter prm2 = new SqlParameter("@UserID", SqlDbType.BigInt);
						prm2.Direction = ParameterDirection.Output;
						cmd.Parameters.Add(prm2);
						cmd.Parameters.Add(new SqlParameter("@ActivationCode", activationCode));
						for (int i = 0; i < cmd.Parameters.Count; i++)
							if (cmd.Parameters[i].Value == null)
								cmd.Parameters[i].Value = DBNull.Value;
						cmd.ExecuteNonQuery();
						if (!(bool)prm.Value)
							return new Result("Unable to find which user to activate. The activation code is not valid.");
						userID = (long)prm2.Value;
						scope.Complete();
					}
					catch (Exception ex)
					{
						return new Result(ex.Message);
					}
				}
			}
			finally
			{
				DatabaseManager.DatabaseEngine.ReleaseConnection(conn);
			}
			return new Result();
		}

		public Result SetEmailChangeRequest(long userID, string newEmailAddress, string activationCode)
		{
			SqlConnection conn = null;
			try
			{
				using (TransactionScope scope = new TransactionScope())
				{
					conn = (SqlConnection)DatabaseManager.DatabaseEngine.GetConnection();
					try
					{
						SqlCommand cmd = new SqlCommand("SetEmailChangeRequest", conn);
						cmd.CommandType = CommandType.StoredProcedure;
						cmd.Parameters.Add(new SqlParameter("@UserID", userID));
						cmd.Parameters.Add(new SqlParameter("@Email", newEmailAddress));
						cmd.Parameters.Add(new SqlParameter("@ActivationCode", activationCode));
						for (int i = 0; i < cmd.Parameters.Count; i++)
							if (cmd.Parameters[i].Value == null)
								cmd.Parameters[i].Value = DBNull.Value;
						cmd.ExecuteNonQuery();
						scope.Complete();
					}
					catch (Exception ex)
					{
						return new Result(ex.Message);
					}
				}
			}
			finally
			{
				DatabaseManager.DatabaseEngine.ReleaseConnection(conn);
			}
			return new Result();
		}

		public EmailChangeRequest SelectEmailChangeRequest(long userID)
		{
			using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Suppress))
			{
				using (SqlConnection conn = new SqlConnection(DatabaseManager.DatabaseEngine.ConnectionString))
				{
					conn.Open();
					SqlCommand cmd = new SqlCommand("SelectEmailChangeRequest", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add(new SqlParameter("@UserID", userID));
					SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
					EmailChangeRequest obj;
					if (!reader.Read())
						obj = null;
					else
						obj = new EmailChangeRequest(reader);
					reader.Close();
					return obj;
				}
			}
		}

		public Result Store(ClientSpace client)
		{
			SqlConnection conn = null;
			try
			{
				using (TransactionScope scope = new TransactionScope())
				{
					conn = (SqlConnection)DatabaseManager.DatabaseEngine.GetConnection();
					try
					{
						SqlCommand cmd = new SqlCommand("StoreClientSpace", conn);
						cmd.CommandType = CommandType.StoredProcedure;
						SqlParameter prm = new SqlParameter("@ClientSpaceID", client.ClientSpaceID);
						prm.Direction = ParameterDirection.InputOutput;
						cmd.Parameters.Add(prm);
						cmd.Parameters.Add(new SqlParameter("@Name", client.Name));
						cmd.Parameters.Add(new SqlParameter("@Enabled", client.Enabled));
						cmd.Parameters.Add(new SqlParameter("@PrimaryUserID", client.PrimaryUserID));
						for (int i = 0; i < cmd.Parameters.Count; i++)
							if (cmd.Parameters[i].Value == null)
								cmd.Parameters[i].Value = DBNull.Value;
						cmd.ExecuteNonQuery();
						client.ClientSpaceID = (long)prm.Value;
						scope.Complete();
					}
					catch (Exception ex)
					{
						return new Result(ex.Message);
					}
				}
			}
			finally
			{
				DatabaseManager.DatabaseEngine.ReleaseConnection(conn);
			}
			return new Result();
		}

		public Result Store(User user)
		{
			SqlConnection conn = null;
			try
			{
				using (TransactionScope scope = new TransactionScope())
				{
					conn = (SqlConnection)DatabaseManager.DatabaseEngine.GetConnection();
					try
					{
						SqlCommand cmd = new SqlCommand("StoreUser", conn);
						cmd.CommandType = CommandType.StoredProcedure;
						SqlParameter prm = new SqlParameter("@UserID", user.UserID);
						prm.Direction = ParameterDirection.InputOutput;
						cmd.Parameters.Add(prm);
						cmd.Parameters.Add(new SqlParameter("@ClientSpaceID", user.ClientSpaceID));
						cmd.Parameters.Add(new SqlParameter("@Username", user.Username));
						cmd.Parameters.Add(new SqlParameter("@PasswordHash", user.PasswordHash));
						cmd.Parameters.Add(new SqlParameter("@FirstName", user.FirstName));
						cmd.Parameters.Add(new SqlParameter("@Surname", user.Surname));
						cmd.Parameters.Add(new SqlParameter("@Email", user.Email));
						cmd.Parameters.Add(new SqlParameter("@Enabled", user.Enabled));
						cmd.Parameters.Add(new SqlParameter("@Hidden", user.Hidden));
						cmd.Parameters.Add(new SqlParameter("@Locked", user.Locked));
						cmd.Parameters.Add(new SqlParameter("@Deleted", user.Deleted));
						cmd.Parameters.Add(new SqlParameter("@Activated", user.Activated));
						cmd.Parameters.Add(new SqlParameter("@ActivationReminderSent", user.Activated));
						cmd.Parameters.Add(new SqlParameter("@Created", user.Created));
						cmd.Parameters.Add(new SqlParameter("@LastAuthenticated", user.LastAuthenticated));
						cmd.Parameters.Add(new SqlParameter("@LocalTimeOffsetHours", user.LocalTimeOffsetHours));
						for (int i = 0; i < cmd.Parameters.Count; i++)
							if (cmd.Parameters[i].Value == null)
								cmd.Parameters[i].Value = DBNull.Value;
						cmd.ExecuteNonQuery();
						user.UserID = (long)prm.Value;
						scope.Complete();
					}
					catch (Exception ex)
					{
						return new Result(ex.Message);
					}
				}
			}
			finally
			{
				DatabaseManager.DatabaseEngine.ReleaseConnection(conn);
			}
			return new Result();
		}

		public Result Store(Role role)
		{
			SqlConnection conn = null;
			try
			{
				using (TransactionScope scope = new TransactionScope())
				{
					conn = (SqlConnection)DatabaseManager.DatabaseEngine.GetConnection();
					try
					{
						SqlCommand cmd = new SqlCommand("StoreRole", conn);
						cmd.CommandType = CommandType.StoredProcedure;
						SqlParameter prm = new SqlParameter("@RoleID", role.RoleID);
						prm.Direction = ParameterDirection.InputOutput;
						cmd.Parameters.Add(prm);
						cmd.Parameters.Add(new SqlParameter("@RoleCode", role.RoleCode));
						cmd.Parameters.Add(new SqlParameter("@ClientSpaceID", role.ClientSpaceID));
						cmd.Parameters.Add(new SqlParameter("@Name", role.Name));
						cmd.Parameters.Add(new SqlParameter("@Enabled", role.Enabled));
						cmd.Parameters.Add(new SqlParameter("@Locked", role.Locked));
						cmd.Parameters.Add(new SqlParameter("@Hidden", role.Hidden));
						for (int i = 0; i < cmd.Parameters.Count; i++)
							if (cmd.Parameters[i].Value == null)
								cmd.Parameters[i].Value = DBNull.Value;
						cmd.ExecuteNonQuery();
						role.RoleID = (long)prm.Value;
						scope.Complete();
					}
					catch (Exception ex)
					{
						return new Result(ex.Message);
					}
				}
			}
			finally
			{
				DatabaseManager.DatabaseEngine.ReleaseConnection(conn);
			}
			return new Result();
		}

		public Result Store(PermissionType permissionType)
		{
			SqlConnection conn = null;
			try
			{
				using (TransactionScope scope = new TransactionScope())
				{
					conn = (SqlConnection)DatabaseManager.DatabaseEngine.GetConnection();
					try
					{
						SqlCommand cmd = new SqlCommand("StorePermissionType", conn);
						cmd.CommandType = CommandType.StoredProcedure;
						SqlParameter prm = new SqlParameter("@PermissionTypeID", permissionType.PermissionTypeID);
						prm.Direction = ParameterDirection.InputOutput;
						cmd.Parameters.Add(prm);
						cmd.Parameters.Add(new SqlParameter("@PermissionTypeCode", permissionType.PermissionTypeCode));
						cmd.Parameters.Add(new SqlParameter("@Description", permissionType.Description));
						cmd.Parameters.Add(new SqlParameter("@DefaultValue", permissionType.DefaultValue));
						for (int i = 0; i < cmd.Parameters.Count; i++)
							if (cmd.Parameters[i].Value == null)
								cmd.Parameters[i].Value = DBNull.Value;
						cmd.ExecuteNonQuery();
						permissionType.PermissionTypeID = (long)prm.Value;
						scope.Complete();
					}
					catch (Exception ex)
					{
						return new Result(ex.Message);
					}
				}
			}
			finally
			{
				DatabaseManager.DatabaseEngine.ReleaseConnection(conn);
			}
			return new Result();
		}

		public Result Delete(ClientSpace client)
		{
			Result result = new Result();
			if (OnBeforeDeleteClientSpace != null)
				OnBeforeDeleteClientSpace(client, result);
			SqlConnection conn = null;
			if (result.Succeeded)
			{
				try
				{
					using (TransactionScope scope = new TransactionScope())
					{
						conn = (SqlConnection)DatabaseManager.DatabaseEngine.GetConnection();
						try
						{
							SqlCommand cmd = new SqlCommand("DeleteClientSpace", conn);
							cmd.CommandType = CommandType.StoredProcedure;
							cmd.Parameters.Add(new SqlParameter("@ClientSpaceID", client.ClientSpaceID));
							cmd.ExecuteNonQuery();
							scope.Complete();
						}
						catch (Exception ex)
						{
							return new Result(ex.Message);
						}
					}
					if (OnClientSpaceDeleted != null)
						OnClientSpaceDeleted(client);
				}
				finally
				{
					DatabaseManager.DatabaseEngine.ReleaseConnection(conn);
				}
			}
			return result;
		}

		public Result Delete(User user)
		{
			Result result = new Result();
			if (OnBeforeDeleteUser != null)
				OnBeforeDeleteUser(user, result);
			SqlConnection conn = null;
			if (result.Succeeded)
			{
				try
				{
					using (TransactionScope scope = new TransactionScope())
					{
						conn = (SqlConnection)DatabaseManager.DatabaseEngine.GetConnection();
						try
						{
							SqlCommand cmd = new SqlCommand("DeleteUser", conn);
							cmd.CommandType = CommandType.StoredProcedure;
							cmd.Parameters.Add(new SqlParameter("@UserID", user.UserID));
							cmd.ExecuteNonQuery();
							scope.Complete();
						}
						catch (Exception ex)
						{
							return new Result(ex.Message);
						}
					}
				}
				finally
				{
					DatabaseManager.DatabaseEngine.ReleaseConnection(conn);
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
			SqlConnection conn = null;
			if (result.Succeeded)
			{
				try
				{
					using (TransactionScope scope = new TransactionScope())
					{
						conn = (SqlConnection)DatabaseManager.DatabaseEngine.GetConnection();
						try
						{
							SqlCommand cmd = new SqlCommand("DeleteRole", conn);
							cmd.CommandType = CommandType.StoredProcedure;
							cmd.Parameters.Add(new SqlParameter("@RoleID", role.RoleID));
							cmd.ExecuteNonQuery();
							scope.Complete();
						}
						catch (Exception ex)
						{
							return new Result(ex.Message);
						}
					}
				}
				finally
				{
					DatabaseManager.DatabaseEngine.ReleaseConnection(conn);
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
			SqlConnection conn = null;
			if (result.Succeeded)
			{
				try
				{
					using (TransactionScope scope = new TransactionScope())
					{
						conn = (SqlConnection)DatabaseManager.DatabaseEngine.GetConnection();
						try
						{
							SqlCommand cmd = new SqlCommand("DeletePermissionType", conn);
							cmd.CommandType = CommandType.StoredProcedure;
							cmd.Parameters.Add(new SqlParameter("@PermissionTypeID", permissionType.PermissionTypeID));
							cmd.ExecuteNonQuery();
							scope.Complete();
						}
						catch (Exception ex)
						{
							return new Result(ex.Message);
						}
					}
				}
				finally
				{
					DatabaseManager.DatabaseEngine.ReleaseConnection(conn);
				}
				if (OnPermissionTypeDeleted != null)
					OnPermissionTypeDeleted(permissionType);
			}
			return result;
		}

		public ClientSpace SelectClientSpace(long clientSpaceID)
		{
			using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Suppress))
			{
				using (SqlConnection conn = new SqlConnection(DatabaseManager.DatabaseEngine.ConnectionString))
				{
					conn.Open();
					SqlCommand cmd = new SqlCommand("SelectClientSpace", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add(new SqlParameter("@ClientSpaceID", clientSpaceID));
					SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
					ClientSpace cs;
					if (!reader.Read())
						cs = null;
					else
						cs = new ClientSpace(reader);
					reader.Close();
					return cs;
				}
			}
		}

		public User SelectUser(long clientSpaceID, string username)
		{
			using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Suppress))
			{
				using (SqlConnection conn = new SqlConnection(DatabaseManager.DatabaseEngine.ConnectionString))
				{
					conn.Open();
					SqlCommand cmd = new SqlCommand("SelectUser", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add(new SqlParameter("@ClientSpaceID", clientSpaceID));
					cmd.Parameters.Add(new SqlParameter("@Username", username));
					SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
					User user;
					if (!reader.Read())
						user = null;
					else
						user = new User(reader);
					reader.Close();
					return user;
				}
			}
		}

		public User SelectUser(long userID)
		{
			using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Suppress))
			{
				using (SqlConnection conn = new SqlConnection(DatabaseManager.DatabaseEngine.ConnectionString))
				{
					conn.Open();
					SqlCommand cmd = new SqlCommand("SelectUser", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add(new SqlParameter("@UserID", userID));
					SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
					User user;
					if (!reader.Read())
						user = null;
					else
						user = new User(reader);
					reader.Close();
					return user;
				}
			}
		}

		public Role SelectRole(long clientSpaceID, string roleCode)
		{
			using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Suppress))
			{
				using (SqlConnection conn = new SqlConnection(DatabaseManager.DatabaseEngine.ConnectionString))
				{
					conn.Open();
					SqlCommand cmd = new SqlCommand("SelectRole", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add(new SqlParameter("@ClientSpaceID", clientSpaceID));
					cmd.Parameters.Add(new SqlParameter("@RoleCode", roleCode));
					SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
					Role role;
					if (!reader.Read())
						role = null;
					else
						role = new Role(reader);
					reader.Close();
					return role;
				}
			}
		}

		public Role SelectRole(long roleID)
		{
			using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Suppress))
			{
				using (SqlConnection conn = new SqlConnection(DatabaseManager.DatabaseEngine.ConnectionString))
				{
					conn.Open();
					SqlCommand cmd = new SqlCommand("SelectRole", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add(new SqlParameter("@RoleID", roleID));
					SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
					Role role;
					if (!reader.Read())
						role = null;
					else
						role = new Role(reader);
					reader.Close();
					return role;
				}
			}
		}

		public event InterruptableEventHandler<ClientSpace> OnBeforeDeleteClientSpace;
		public event NotificationEventHandler<ClientSpace> OnClientSpaceDeleted;
		public event InterruptableEventHandler<User> OnBeforeDeleteUser;
		public event NotificationEventHandler<User> OnUserDeleted;
		public event InterruptableEventHandler<Role> OnBeforeDeleteRole;
		public event NotificationEventHandler<Role> OnRoleDeleted;
		public event InterruptableEventHandler<PermissionType> OnBeforeDeletePermissionType;
		public event NotificationEventHandler<PermissionType> OnPermissionTypeDeleted;

		public long? GetRoleIDFromRoleCode(Guid clientSpaceID, string roleCode)
		{
			using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Suppress))
			{
				using (SqlConnection conn = new SqlConnection(DatabaseManager.DatabaseEngine.ConnectionString))
				{
					conn.Open();
					SqlCommand cmd = new SqlCommand("GetRoleIDFromRoleCode", conn);
					cmd.Parameters.Add(new SqlParameter("@ClientSpaceID", clientSpaceID));
					cmd.Parameters.Add(new SqlParameter("@RoleCode", roleCode));
					SqlParameter prm = new SqlParameter("@RoleID", SqlDbType.BigInt);
					prm.Direction = ParameterDirection.Output;
					cmd.Parameters.Add(prm);
					cmd.ExecuteNonQuery();
					conn.Close();
					return (long)prm.Value;
				}
			}
		}

		public bool DoesRoleInheritRole(long thisRoleID, long doesItInheritRoleID)
		{
			using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Suppress))
			{
				using (SqlConnection conn = new SqlConnection(DatabaseManager.DatabaseEngine.ConnectionString))
				{
					conn.Open();
					SqlCommand cmd = new SqlCommand("DoesRoleInheritRole", conn);
					cmd.Parameters.Add(new SqlParameter("@ThisRoleID", thisRoleID));
					cmd.Parameters.Add(new SqlParameter("@DoesItInheritRoleID", doesItInheritRoleID));
					SqlParameter prm = new SqlParameter("@Result", SqlDbType.Bit);
					prm.Direction = ParameterDirection.Output;
					cmd.Parameters.Add(prm);
					cmd.ExecuteNonQuery();
					conn.Close();
					return (bool)prm.Value;
				}
			}
		}

		public void InheritRoleFrom(long thisRoleID, long inheritFromRoleID)
		{
			using (TransactionScope scope = new TransactionScope())
			{
				SqlConnection conn = (SqlConnection)DatabaseManager.DatabaseEngine.GetConnection();
				try
				{
					SqlCommand cmd = new SqlCommand("InheritRoleFrom", conn);
					cmd.Parameters.Add(new SqlParameter("@ThisRoleID", thisRoleID));
					cmd.Parameters.Add(new SqlParameter("@InheritFromRoleID", inheritFromRoleID));
					cmd.ExecuteNonQuery();
					scope.Complete();
				}
				finally
				{
					DatabaseManager.DatabaseEngine.ReleaseConnection(conn);
				}
			}
		}

		public void DisinheritRoleFrom(long thisRoleID, long disinheritFromRoleID)
		{
			using (TransactionScope scope = new TransactionScope())
			{
				SqlConnection conn = (SqlConnection)DatabaseManager.DatabaseEngine.GetConnection();
				try
				{
					SqlCommand cmd = new SqlCommand("DisinheritRoleFrom", conn);
					cmd.Parameters.Add(new SqlParameter("@RoleID", thisRoleID));
					cmd.Parameters.Add(new SqlParameter("@DisinheritRoleID", disinheritFromRoleID));
					cmd.ExecuteNonQuery();
					scope.Complete();
				}
				finally
				{
					DatabaseManager.DatabaseEngine.ReleaseConnection(conn);
				}
			}
		}

		public List<Role> ListInheritedRoles(long thisRoleID)
		{
			using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Suppress))
			{
				using (SqlConnection conn = new SqlConnection(DatabaseManager.DatabaseEngine.ConnectionString))
				{
					conn.Open();
					SqlCommand cmd = new SqlCommand("ListInheritedRoles", conn);
					cmd.Parameters.Add(new SqlParameter("@RoleID", thisRoleID));
					SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
					List<Role> list = new List<Role>();
					while (reader.Read())
						list.Add(new Role(reader));
					reader.Close();
					return list;
				}
			}
		}

		public List<Role> ListUserRoles(long userID)
		{
			using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Suppress))
			{
				using (SqlConnection conn = new SqlConnection(DatabaseManager.DatabaseEngine.ConnectionString))
				{
					conn.Open();
					SqlCommand cmd = new SqlCommand("ListUserRoles", conn);
					cmd.Parameters.Add(new SqlParameter("@UserID", userID));
					SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
					List<Role> list = new List<Role>();
					while (reader.Read())
						list.Add(new Role(reader));
					reader.Close();
					return list;
				}
			}
		}

		public bool IsUserInRole(long userID, string roleCode)
		{
			using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Suppress))
			{
				using (SqlConnection conn = new SqlConnection(DatabaseManager.DatabaseEngine.ConnectionString))
				{
					conn.Open();
					SqlCommand cmd = new SqlCommand("IsUserInRole", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add(new SqlParameter("@UserID", userID));
					cmd.Parameters.Add(new SqlParameter("@RoleCode", roleCode));
					SqlParameter prm = new SqlParameter("@IsUserInRole", SqlDbType.Bit);
					prm.Direction = ParameterDirection.Output;
					cmd.Parameters.Add(prm);
					cmd.ExecuteNonQuery();
					conn.Close();
					return (bool)prm.Value;
				}
			}
		}

		public void AssignRoleToUser(long userID, string roleCode)
		{
			using (TransactionScope scope = new TransactionScope())
			{
				SqlConnection conn = (SqlConnection)DatabaseManager.DatabaseEngine.GetConnection();
				try
				{
					SqlCommand cmd = new SqlCommand("AssignRoleToUser", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add(new SqlParameter("@UserID", userID));
					cmd.Parameters.Add(new SqlParameter("@RoleCode", roleCode));
					cmd.ExecuteNonQuery();
					scope.Complete();
				}
				finally
				{
					DatabaseManager.DatabaseEngine.ReleaseConnection(conn);
				}
			}
		}

		public void UnassignRoleFromUser(long userID, string roleCode)
		{
			using (TransactionScope scope = new TransactionScope())
			{
				SqlConnection conn = (SqlConnection)DatabaseManager.DatabaseEngine.GetConnection();
				try
				{
					SqlCommand cmd = new SqlCommand("UnassignRoleFromUser", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add(new SqlParameter("@UserID", userID));
					cmd.Parameters.Add(new SqlParameter("@RoleCode", roleCode));
					cmd.ExecuteNonQuery();
					scope.Complete();
				}
				finally
				{
					DatabaseManager.DatabaseEngine.ReleaseConnection(conn);
				}
			}
		}

		public void AssignPermissionToUser(long userID, string permissionTypeCode)
		{
			using (TransactionScope scope = new TransactionScope())
			{
				SqlConnection conn = (SqlConnection)DatabaseManager.DatabaseEngine.GetConnection();
				try
				{
					SqlCommand cmd = new SqlCommand("AssignPermissionToUser", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add(new SqlParameter("@UserID", userID));
					cmd.Parameters.Add(new SqlParameter("@PermissionTypeCode", permissionTypeCode));
					cmd.ExecuteNonQuery();
					scope.Complete();
				}
				finally
				{
					DatabaseManager.DatabaseEngine.ReleaseConnection(conn);
				}
			}
		}

		public void AssignPermissionToRole(long roleID, string permissionTypeCode)
		{
			using (TransactionScope scope = new TransactionScope())
			{
				SqlConnection conn = (SqlConnection)DatabaseManager.DatabaseEngine.GetConnection();
				try
				{
					SqlCommand cmd = new SqlCommand("AssignPermissionToRole", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add(new SqlParameter("@RoleID", roleID));
					cmd.Parameters.Add(new SqlParameter("@PermissionTypeCode", permissionTypeCode));
					cmd.ExecuteNonQuery();
					scope.Complete();
				}
				finally
				{
					DatabaseManager.DatabaseEngine.ReleaseConnection(conn);
				}
			}
		}

		public bool DoesUserHavePermission(long userID, string permissionTypeCode)
		{
			using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Suppress))
			{
				using (SqlConnection conn = new SqlConnection(DatabaseManager.DatabaseEngine.ConnectionString))
				{
					conn.Open();
					SqlCommand cmd = new SqlCommand("DoesUserHavePermission", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add(new SqlParameter("@UserID", userID));
					cmd.Parameters.Add(new SqlParameter("@PermissionTypeCode", permissionTypeCode));
					SqlParameter prm = new SqlParameter("@HasPermission", SqlDbType.Bit);
					prm.Direction = ParameterDirection.Output;
					cmd.Parameters.Add(prm);
					cmd.ExecuteNonQuery();
					return (bool)prm.Value;
				}
			}
		}

		public void RemoveRolesAndPermissionsFromUser(long userID)
		{
			using (TransactionScope scope = new TransactionScope())
			{
				SqlConnection conn = (SqlConnection)DatabaseManager.DatabaseEngine.GetConnection();
				try
				{
					SqlCommand cmd = new SqlCommand("RemoveRolesAndPermissionsFromUser", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add(new SqlParameter("@UserID", userID));
					cmd.ExecuteNonQuery();
					scope.Complete();
				}
				finally
				{
					DatabaseManager.DatabaseEngine.ReleaseConnection(conn);
				}
			}
		}

		public void RemoveRolesAndPermissionsFromRole(long roleID)
		{
			using (TransactionScope scope = new TransactionScope())
			{
				SqlConnection conn = (SqlConnection)DatabaseManager.DatabaseEngine.GetConnection();
				try
				{
					SqlCommand cmd = new SqlCommand("RemoveRolesAndPermissionsFromRole", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add(new SqlParameter("@RoleID", roleID));
					cmd.ExecuteNonQuery();
					scope.Complete();
				}
				finally
				{
					DatabaseManager.DatabaseEngine.ReleaseConnection(conn);
				}
			}
		}

		public void SetRolesAndPermissionsForUser(long userID, List<string> roleCodes, List<string> permissionTypeCodes)
		{
			SqlConnection conn = null;
			try
			{
				using (TransactionScope scope = new TransactionScope())
				{
					conn = (SqlConnection)DatabaseManager.DatabaseEngine.GetConnection();
					SqlCommand cmd = new SqlCommand("RemoveRolesAndPermissionsFromUser", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add(new SqlParameter("@UserID", userID));
					cmd.ExecuteNonQuery();

					cmd = new SqlCommand("AssignRoleToUser", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add(new SqlParameter("@UserID", userID));
					SqlParameter prm = new SqlParameter("@RoleCode", SqlDbType.NVarChar);
					cmd.Parameters.Add(prm);

					foreach (string s in roleCodes)
					{
						prm.Value = s;
						cmd.ExecuteNonQuery();
					}

					cmd = new SqlCommand("AssignPermissionToUser", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add(new SqlParameter("@UserID", userID));
					prm = new SqlParameter("@PermissionTypeCode", SqlDbType.NVarChar);
					cmd.Parameters.Add(prm);

					foreach (string s in permissionTypeCodes)
					{
						prm.Value = s;
						cmd.ExecuteNonQuery();
					}

					scope.Complete();
				}
			}
			finally
			{
				DatabaseManager.DatabaseEngine.ReleaseConnection(conn);
			}
		}

		public void SetRolesAndPermissionsForRole(long roleID, List<string> roleCodes, List<string> permissionTypeCodes)
		{
			SqlConnection conn = null;
			try
			{
				using (TransactionScope scope = new TransactionScope())
				{
					conn = (SqlConnection)DatabaseManager.DatabaseEngine.GetConnection();
					SqlCommand cmd = new SqlCommand("RemoveRolesAndPermissionsFromRole", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add(new SqlParameter("@RoleID", roleID));
					cmd.ExecuteNonQuery();

					cmd = new SqlCommand("InheritRoleFrom", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add(new SqlParameter("@ThisRoleID", roleID));
					SqlParameter prm = new SqlParameter("@InheritFromRoleCode", SqlDbType.NVarChar);
					cmd.Parameters.Add(prm);

					foreach (string s in roleCodes)
					{
						prm.Value = s;
						cmd.ExecuteNonQuery();
					}

					cmd = new SqlCommand("AssignPermissionToRole", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add(new SqlParameter("@RoleID", roleID));
					prm = new SqlParameter("@PermissionTypeCode", SqlDbType.NVarChar);
					cmd.Parameters.Add(prm);

					foreach (string s in permissionTypeCodes)
					{
						prm.Value = s;
						cmd.ExecuteNonQuery();
					}

					scope.Complete();
				}
			}
			finally
			{
				DatabaseManager.DatabaseEngine.ReleaseConnection(conn);
			}
		}

		public List<Role> ListAccessibleRoles(long userID)
		{
			using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Suppress))
			{
				using (SqlConnection conn = new SqlConnection(DatabaseManager.DatabaseEngine.ConnectionString))
				{
					conn.Open();
					SqlCommand cmd = new SqlCommand("ListAccessibleRolesForUser", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add(new SqlParameter("@UserID", userID));
					SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
					List<Role> list = new List<Role>();
					while (reader.Read())
						list.Add(new Role(reader));
					reader.Close();
					return list;
				}
			}
		}

		public List<PermissionTypeState> ListPermissionsForRole(long roleID)
		{
			using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Suppress))
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public List<PermissionTypeState> ListAllPermissionTypesAgainstUser(long userID)
		{
			using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Suppress))
			{
				using (SqlConnection conn = new SqlConnection(DatabaseManager.DatabaseEngine.ConnectionString))
				{
					conn.Open();
					SqlCommand cmd = new SqlCommand("ListPermissionsForUser", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add(new SqlParameter("@UserID", userID));
					SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
					List<PermissionTypeState> list = new List<PermissionTypeState>();
					while (reader.Read())
					{
						PermissionState state;
						if ((bool)reader["HasPermission"])
							if ((bool)reader["Inherited"])
								state = PermissionState.Inherited;
							else
								state = PermissionState.Specified;
						else
							state = PermissionState.Disabled;
						list.Add(new PermissionTypeState(new PermissionType(reader), state));
					}
					reader.Close();
					return list;
				}
			}
		}

		public List<PermissionTypeState> ListAllPermissionTypesAgainstRole(long roleID)
		{
			using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Suppress))
			{
				using (SqlConnection conn = new SqlConnection(DatabaseManager.DatabaseEngine.ConnectionString))
				{
					conn.Open();
					SqlCommand cmd = new SqlCommand("ListAllPermissionTypesAgainstRole", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add(new SqlParameter("@RoleID", roleID));
					SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
					List<PermissionTypeState> list = new List<PermissionTypeState>();
					while (reader.Read())
					{
						PermissionState state = PermissionState.Disabled;
						if (reader["Value"] != DBNull.Value)
							if ((bool)reader["Value"])
								state = PermissionState.Specified;
						list.Add(new PermissionTypeState(new PermissionType(reader), state));
					}
					reader.Close();
					return list;
				}
			}
		}

		public List<RoleState> ListAllRolesAgainstRole(long roleID)
		{
			using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Suppress))
			{
				using (SqlConnection conn = new SqlConnection(DatabaseManager.DatabaseEngine.ConnectionString))
				{
					conn.Open();
					SqlCommand cmd = new SqlCommand("ListAllRolesAgainstRole", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add(new SqlParameter("@RoleID", roleID));
					SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
					List<RoleState> list = new List<RoleState>();
					while (reader.Read())
					{
						PermissionState state;
						if ((bool)reader["Inherited"])
							state = PermissionState.Specified;
						else
							state = PermissionState.Disabled;
						list.Add(new RoleState(new Role(reader), true, state));
					}
					reader.Close();
					return list;
				}
			}
		}

		public List<RoleState> ListAllRolesAgainstUser(long userID)
		{
			using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Suppress))
			{
				using (SqlConnection conn = new SqlConnection(DatabaseManager.DatabaseEngine.ConnectionString))
				{
					conn.Open();
					SqlCommand cmd = new SqlCommand("ListAllRolesAgainstUser", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add(new SqlParameter("@UserID", userID));
					SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
					List<RoleState> list = new List<RoleState>();
					while (reader.Read())
						list.Add(new RoleState(new Role(reader), (bool)reader["HasRole"],
							(bool)reader["HasRole"] ? PermissionState.Specified : PermissionState.Disabled));
					reader.Close();
					return list;
				}
			}
		}

		public List<Role> ListDescendentRoles(long roleID)
		{
			using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Suppress))
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		public List<User> FilterUsers(string partUsername, string partFirstName, string partSurname, string partEmail, int? maxResults, long? editableByUserID, bool? activated, out int totalMatches)
		{
			using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Suppress))
			{
				using (SqlConnection conn = new SqlConnection(DatabaseManager.DatabaseEngine.ConnectionString))
				{
					conn.Open();
					if (maxResults == 0) maxResults = null;
					if (partUsername == "") partUsername = null;
					if (partFirstName == "") partFirstName = null;
					if (partSurname == "") partSurname = null;
					SqlCommand cmd = new SqlCommand("ListUsers", conn);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add(new SqlParameter("@Username", partUsername));
					cmd.Parameters.Add(new SqlParameter("@FirstName", partFirstName));
					cmd.Parameters.Add(new SqlParameter("@Surname", partSurname));
					cmd.Parameters.Add(new SqlParameter("@Email", partEmail));
					cmd.Parameters.Add(new SqlParameter("@Activated", activated));
					cmd.Parameters.Add(new SqlParameter("@EditableByUserID", editableByUserID));
					cmd.Parameters.Add(new SqlParameter("@MaxRecords", maxResults));
					SqlParameter prm = new SqlParameter("@TotalMatches", SqlDbType.Int);
					prm.Direction = ParameterDirection.Output;
					cmd.Parameters.Add(prm);
					List<User> users = new List<User>();
					SqlDataReader reader = cmd.ExecuteReader();
					while (reader.Read())
						users.Add(new User(reader));
					reader.Close();
					conn.Close();
					if (prm.Value != DBNull.Value)
						totalMatches = (int)prm.Value;
					else
						totalMatches = users.Count;
					return users;
				}
			}
		}
	}
}
