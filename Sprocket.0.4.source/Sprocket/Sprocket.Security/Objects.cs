using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;

using Sprocket;
using Sprocket.Data;
using Sprocket.Mail;
using Sprocket.Utility;
using Sprocket.Web.CMS.Script;

namespace Sprocket.Security
{
	public class ClientSpace : IEntity
	{
		private long clientSpaceID;
		private long? primaryUserID;
		private bool enabled;
		private string name;

		#region Class Properties

		public long ClientSpaceID
		{
			get { return clientSpaceID; }
			set { clientSpaceID = value; }
		}

		public long? PrimaryUserID
		{
			get { return primaryUserID; }
			set { primaryUserID = value; }
		}

		public bool Enabled
		{
			get { return enabled; }
			set { enabled = value; }
		}

		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		#endregion

		#region Constructor

		public ClientSpace() { }
		public ClientSpace(long clientSpaceID, long? primaryUserID, bool enabled, string name)
		{
			this.clientSpaceID = clientSpaceID;
			this.primaryUserID = primaryUserID;
			this.enabled = enabled;
			this.name = name;
		}
		public ClientSpace(IDataReader reader)
		{
			clientSpaceID = (long)reader["ClientSpaceID"];
			primaryUserID = reader["PrimaryUserID"] == DBNull.Value ? null : (long?)reader["ClientSpaceID"];
			enabled = (bool)reader["Enabled"];
			name = (string)reader["Name"];
		}

		#endregion

		public static ClientSpace Select(long clientSpaceID)
		{
			return SecurityProvider.DataLayer.SelectClientSpace(clientSpaceID);
		}
	}

	public class User : IEntity
	{
		#region Fields
		protected long userID = 0, clientSpaceID = 0;
		protected string username = "", passwordHash = "";
		protected string firstName = "", surname = "", email = "";
		protected bool enabled = true, locked = false, activated = false, deleted = false;
		protected bool hidden = false; // for special users used internally only
		protected int localTimeOffsetHours = 0;
		protected DateTime? activationReminderSent = null, lastAuthenticated = null;
		protected DateTime created = SprocketDate.Now;

		private Dictionary<string, PermissionState> permissions = null;
		//private Dictionary<string, PermissionState> roles = null;
		#endregion

		#region Constructor
		public User() { }

		public User(long clientSpaceID, string username,
			string password, string firstName, string surname, string email,
			bool enabled, bool locked, bool hidden, int localTimeOffsetHours)
		{
			this.userID = 0;
			this.clientSpaceID = clientSpaceID;
			this.username = username;
			this.passwordHash = Crypto.EncryptOneWay(password);
			this.firstName = firstName;
			this.surname = surname;
			this.email = email;
			this.enabled = enabled;
			this.locked = locked;
			this.deleted = false;
			this.hidden = hidden;
			this.activated = false;
			this.created = SprocketDate.Now;
			this.activationReminderSent = null;
			this.localTimeOffsetHours = localTimeOffsetHours;
		}

		public User(IDataReader reader)
		{
			userID = (long)reader["UserID"];
			clientSpaceID = (long)reader["ClientSpaceID"];
			username = (string)reader["Username"];
			passwordHash = (string)reader["PasswordHash"];
			firstName = (string)reader["FirstName"];
			surname = (string)reader["Surname"];
			email = (string)reader["Email"];
			enabled = (bool)reader["Enabled"];
			locked = (bool)reader["Locked"];
			deleted = (bool)reader["Deleted"];
			hidden = (bool)reader["Hidden"];
			activated = (bool)reader["Activated"];
			created = (DateTime)reader["Created"];
			activationReminderSent = reader["ActivationReminderSent"] == DBNull.Value ? null : (DateTime?)reader["ActivationReminderSent"];
			lastAuthenticated = reader["LastAuthenticated"] == DBNull.Value ? null : (DateTime?)reader["LastAuthenticated"];
			localTimeOffsetHours = (int)reader["LocalTimeOffsetHours"];
		}

		#endregion

		#region Class Properties

		public bool Activated
		{
			get { return activated; }
			set { activated = value; }
		}

		public DateTime? ActivationReminderSent
		{
			get { return activationReminderSent; }
			set { activationReminderSent = value; }
		}

		public DateTime? LastAuthenticated
		{
			get { return lastAuthenticated; }
			set { lastAuthenticated = value; }
		}

		public DateTime Created
		{
			get { return created; }
			set { created = value; }
		}

		public int LocalTimeOffsetHours
		{
			get { return localTimeOffsetHours; }
			set { localTimeOffsetHours = value; }
		}

		public long UserID
		{
			get { return userID; }
			set { userID = value; }
		}

		public long ClientSpaceID
		{
			get { return clientSpaceID; }
			set { clientSpaceID = value; }
		}

		public string Username
		{
			get { return username; }
			set { username = value; }
		}

		public string PasswordHash
		{
			get { return passwordHash; }
		}

		public string Password
		{
			set { passwordHash = Crypto.EncryptOneWay(value); }
		}

		public string FirstName
		{
			get { return firstName; }
			set { firstName = value; }
		}

		public string Surname
		{
			get { return surname; }
			set { surname = value; }
		}

		public string FullName
		{
			get { return (FirstName + " " + Surname).Trim(); }
		}

		public string Email
		{
			get { return email; }
			set { email = value; }
		}

		public bool Enabled
		{
			get { return enabled; }
			set { enabled = value; }
		}

		public bool Deleted
		{
			get { return deleted; }
			set { deleted = value; }
		}

		public bool Hidden
		{
			get { return hidden; }
			set { hidden = value; }
		}

		public bool Locked
		{
			get { return locked; }
			set { locked = value; }
		}

		#endregion

		public static User Select(long clientSpaceID, string username)
		{
			return SecurityProvider.DataLayer.SelectUser(clientSpaceID, username);
		}

		public static User Select(long userID)
		{
			return SecurityProvider.DataLayer.SelectUser(userID);
		}

		#region Methods

		public bool HasPermission(string permissionTypeCode)
		{
			if (permissions == null)
			{
				permissions = new Dictionary<string, PermissionState>();
				List<PermissionTypeState> list = SecurityProvider.DataLayer.ListAllPermissionTypesAgainstUser(userID);
				foreach (PermissionTypeState p in list)
					permissions.Add(p.PermissionType.PermissionTypeCode, p.PermissionState);
			}
			if (permissions.ContainsKey(PermissionType.SuperUser))
				if (permissions[PermissionType.SuperUser] != PermissionState.Disabled)
					return true;
			if(!permissions.ContainsKey(permissionTypeCode))
				return false;
			return permissions[permissionTypeCode] != PermissionState.Disabled;
				//SecurityProvider.DataLayer.DoesUserHavePermission(userID, permissionTypeCode);
		}

		public bool HasRole(string roleCode)
		{
			return SecurityProvider.DataLayer.IsUserInRole(userID, roleCode);
		}

		public MailAddress GetMailAddress(bool useFullName)
		{
			if (useFullName)
				return new MailAddress(email, FullName);
			else
				return new MailAddress(email, username);
		}

		#endregion
	}

	public class Role : IEntity, IPropertyEvaluatorExpression
	{
		private long roleID = 0, clientSpaceID = 0;
		private string roleCode = "", name = "";
		private bool enabled = true, hidden = false, locked = false;

		#region Constructor

		public Role()
		{
			roleID = 0;
			roleCode = "Role " + roleID;
		}

		public Role(IDataReader reader)
		{
			ClientSpaceID = (long)reader["ClientSpaceID"];
			Enabled = (bool)reader["Enabled"];
			Hidden = (bool)reader["Hidden"];
			Locked = (bool)reader["Locked"];
			Name = (string)reader["Name"];
			RoleCode = (string)reader["RoleCode"];
			RoleID = (long)reader["RoleID"];
		}

		#endregion

		#region Class Properties
		public long RoleID
		{
			get { return roleID; }
			set { roleID = value; }
		}

		public long ClientSpaceID
		{
			get { return clientSpaceID; }
			set { clientSpaceID = value; }
		}

		public string RoleCode
		{
			get { return roleCode; }
			set { roleCode = value; }
		}

		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		public bool Enabled
		{
			get { return enabled; }
			set { enabled = value; }
		}

		public bool Hidden
		{
			get { return hidden; }
			set { hidden = value; }
		}

		public bool Locked
		{
			get { return locked; }
			set { locked = value; }
		}
		#endregion

		public static Role Select(long roleID)
		{
			return SecurityProvider.DataLayer.SelectRole(roleID);
		}

		public static Role Select(long clientSpaceID, string roleCode)
		{
			return SecurityProvider.DataLayer.SelectRole(clientSpaceID, roleCode);
		}

		public bool IsValidPropertyName(string propertyName)
		{
			switch (propertyName)
			{
				case "roleid":
				case "rolecode":
				case "name":
				case "enabled":
				case "hidden":
				case "locked":
					return true;
				default:
					return false;
			}
		}

		public object EvaluateProperty(string propertyName, Token token, ExecutionState state)
		{
			switch (propertyName)
			{
				case "roleid":
					return RoleID;
				case "rolecode":
					return RoleCode;
				case "name":
					return name;
				case "enabled":
					return enabled;
				case "hidden":
					return hidden;
				case "locked":
					return locked;
				default:
					return VariableExpression.InvalidProperty;
			}
		}

		public object Evaluate(ExecutionState state, Token contextToken)
		{
			return name;
		}
	}

	public class PermissionType : IEntity
	{
		long permissionTypeID;
		string permissionTypeCode, description;
		bool defaultValue;

		#region Properties
		public long PermissionTypeID
		{
			get { return permissionTypeID; }
			set { permissionTypeID = value; }
		}

		public string PermissionTypeCode
		{
			get { return permissionTypeCode; }
			set { permissionTypeCode = value; }
		}

		public string Description
		{
			get { return description; }
			set { description = value; }
		}

		public bool DefaultValue
		{
			get { return defaultValue; }
			set { defaultValue = value; }
		}
		#endregion

		public PermissionType() { }
		public PermissionType(IDataReader reader)
		{
			permissionTypeID = (long)reader["PermissionTypeID"];
			permissionTypeCode = (string)reader["PermissionTypeCode"];
			description = (string)reader["Description"];
			defaultValue = (bool)reader["DefaultValue"];
		}

		public const string AdministrativeAccess = "ACCESS_ADMIN";
		public const string UserAdministrator = "USERADMINISTRATOR";
		public const string RoleAdministrator = "ROLEADMINISTRATOR";
		public const string SuperUser = "SUPERUSER";
	}

	public enum PermissionState
	{
		/// <summary>
		/// The permission is not accessible by the target
		/// </summary>
		Disabled,
		/// <summary>
		/// The permission has been explicitly enabled for the target
		/// </summary>
		Specified,
		/// <summary>
		/// The permission accessible to the target because of a role or inherited role
		/// </summary>
		Inherited
	}

	public class PermissionTypeState
	{
		private PermissionState permissionState;
		private PermissionType permissionType;

		public PermissionType PermissionType
		{
			get { return permissionType; }
		}

		public PermissionState PermissionState
		{
			get { return permissionState; }
		}

		public PermissionTypeState(PermissionType type, PermissionState state)
		{
			permissionType = type;
			permissionState = state;
		}
	}

	public class RoleState
	{
		private Role role;
		private bool isAccessible;
		private PermissionState state;

		public PermissionState State
		{
			get { return state; }
		}

		public Role Role
		{
			get { return role; }
		}

		public bool IsAccessible
		{
			get { return isAccessible; }
		}

		public RoleState(Role role, bool accessible, PermissionState state)
		{
			this.role = role;
			this.isAccessible = accessible;
			this.state = state;
		}
	}

	public class EmailChangeRequest
	{
		private string activationCode, email;
		private long userID;
		private DateTime requestDate;

		public DateTime RequestDate
		{
			get { return requestDate; }
		}

		public long UserID
		{
			get { return userID; }
		}

		public string ActivationCode
		{
			get { return activationCode; }
		}

		public string Email
		{
			get { return email; }
		}

		public EmailChangeRequest(IDataReader reader)
		{
			activationCode = reader["ActivationCode"].ToString();
			email = reader["Email"].ToString();
			userID = (long)reader["UserID"];
			requestDate = (DateTime)reader["RequestDate"];
		}
	}
}
