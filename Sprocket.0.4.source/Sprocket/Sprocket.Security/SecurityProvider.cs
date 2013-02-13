using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;
using System.Transactions;
using System.Web;

using Sprocket;
using Sprocket.Data;
using Sprocket.Mail;
using Sprocket.Utility;
using Sprocket.Web;

namespace Sprocket.Security
{
	[ModuleDependency(typeof(DatabaseManager))]
	[ModuleDependency(typeof(EmailHandler))]
	[ModuleTitle("Sprocket Security Provider")]
	[ModuleDescription("The default security implementation for Sprocket. Handles users, roles and permissions.")]
	public partial class SecurityProvider : ISprocketModule
	{
		public static SecurityProvider Instance
		{
			get { return (SecurityProvider)Core.Instance[typeof(SecurityProvider)].Module; }
		}

		public void AttachEventHandlers(ModuleRegistry registry)
		{
			DatabaseManager.Instance.OnDatabaseHandlerLoaded += new NotificationEventHandler<IDatabaseHandler>(Instance_OnDatabaseHandlerLoaded);
			DatabaseSetup.Instance.Completed += new EmptyHandler(DatabaseSetup_Completed);
			WebEvents.Instance.OnBeforeLoadExistingFile += new WebEvents.RequestedPathEventHandler(Instance_OnBeforeLoadExistingFile);
			WebAuthentication.Instance.OnValidatingLogin += new WebAuthentication.LoginAuthenticationHandler(WebAuthentication_OnValidatingLogin);
		}

		void DatabaseSetup_Completed()
		{
			VerifyClientSpaceID();
		}

		void WebAuthentication_OnValidatingLogin(string username, string passwordHash, Result result)
		{
			if (!dataLayer.Authenticate(username, passwordHash))
				result.SetFailed("Invalid username and/or password");
		}

		public static User CurrentUser
		{
			get
			{
				if (!WebAuthentication.Instance.IsLoggedIn)
					return null;
				if (CurrentRequest.Value["CurrentUser"] == null)
					CurrentRequest.Value["CurrentUser"] = User.Select(ClientSpaceID, WebAuthentication.Instance.CurrentUsername);
				return CurrentRequest.Value["CurrentUser"] as User;
			}
		}

		ISecurityProviderDataLayer dataLayer = null;
		public static ISecurityProviderDataLayer DataLayer
		{
			get { return Instance.dataLayer; }
		}

		void Instance_OnDatabaseHandlerLoaded(IDatabaseHandler source)
		{
			source.OnInitialise += new InterruptableEventHandler(Database_OnInitialise);
			foreach (Type t in Core.Modules.GetInterfaceImplementations(typeof(ISecurityProviderDataLayer)))
			{
				ISecurityProviderDataLayer layer = (ISecurityProviderDataLayer)Activator.CreateInstance(t);
				if (layer.DatabaseHandlerType == source.GetType())
				{
					dataLayer = layer;
					break;
				}
			}
		}

		void Database_OnInitialise(Result result)
		{
			if (!result.Succeeded)
				return;
			if (dataLayer == null)
				result.SetFailed("SecurityProvider has no implementation for " + DatabaseManager.DatabaseEngine.Title);
			else
			{
				dataLayer.InitialiseDatabase(result);
//				long forceClientInit = ClientSpaceID;
			}
		}

		void Instance_OnBeforeLoadExistingFile(HandleFlag handled)
		{
			if (SprocketPath.Value.ToLower() == "datastore/clientspace.id") // deny access
				handled.Set();
		}

		private static long clientSpaceID = -1;
		public static long ClientSpaceID
		{
			get
			{
				lock (WebUtility.GetSyncObject("DefaultClientSpaceID"))
				{
					if (clientSpaceID == -1)
						VerifyClientSpaceID();
					return clientSpaceID;
				}
			}
		}

		public static void VerifyClientSpaceID()
		{
			string path = WebUtility.MapPath("datastore/ClientSpace.ID");
			if (!File.Exists(path))
			{
				clientSpaceID = DatabaseManager.GetUniqueID();
				Result result = Instance.dataLayer.InitialiseClientSpace(clientSpaceID);
				if (!result.Succeeded)
					throw new Exception(result.Message);
				new FileInfo(path).Directory.Create();
				using (FileStream file = File.Create(path))
				{
					file.Write(BitConverter.GetBytes(clientSpaceID), 0, sizeof(long));
					file.Close();
				}
			}
			else
			{
				byte[] bytes = new byte[sizeof(long)];
				using (FileStream file = File.OpenRead(path))
				{
					file.Read(bytes, 0, bytes.Length);
					file.Close();
				}
				clientSpaceID = BitConverter.ToInt64(bytes, 0);
			}
		}

		public static string RequestUserActivation(long userID, string emailAddress)
		{
			string code = (Guid.NewGuid().ToString() + Guid.NewGuid()).Replace("-", "");
			Result r = DataLayer.SetEmailChangeRequest(userID, emailAddress, code);
			if (!r.Succeeded)
				throw new Exception(r.Message);
			return code;
		}

		//public static class RoleCodes
		//{
		//    public static readonly string SuperUser = "SUPERUSER";
		//}

		//public static class PermissionTypeCodes
		//{
		//    public static readonly string UserAdministrator = "USERADMINISTRATOR";
		//    public static readonly string RoleAdministrator = "ROLEADMINISTRATOR";
		//}
	}
}
