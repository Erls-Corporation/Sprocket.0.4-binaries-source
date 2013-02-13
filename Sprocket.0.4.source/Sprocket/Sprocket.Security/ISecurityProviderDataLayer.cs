using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using Sprocket.Data;
using Sprocket.Security;

namespace Sprocket.Security
{
	public interface ISecurityProviderDataLayer
	{
		Type DatabaseHandlerType { get; }
		void InitialiseDatabase(Result result);
		Result InitialiseClientSpace(long clientSpaceID);

		bool Authenticate(string username, string passwordHash);
		bool Authenticate(long clientSpaceID, string username, string passwordHash);
		bool IsEmailAddressTaken(long clientSpaceID, string email);
		bool IsUsernameTaken(long clientSpaceID, string username);
		bool IsEmailAddressTaken(long clientSpaceID, string email, long? excludeUserID);
		bool IsUsernameTaken(long clientSpaceID, string username, long? excludeUserID);

		Result ActivateUser(string activationCode, out long userID);
		Result SetEmailChangeRequest(long userID, string newEmailAddress, string activationCode);
		EmailChangeRequest SelectEmailChangeRequest(long userID);

		Result Store(ClientSpace client);
		Result Store(User user);
		Result Store(Role role);
		Result Store(PermissionType permissionType);

		Result Delete(ClientSpace client);
		Result Delete(User user);
		Result Delete(Role role);
		Result Delete(PermissionType permissionType);

		ClientSpace SelectClientSpace(long clientSpaceID);
		User SelectUser(long clientSpaceID, string username);
		User SelectUser(long userID);
		Role SelectRole(long clientSpaceID, string roleCode);
		Role SelectRole(long roleID);

		event InterruptableEventHandler<ClientSpace> OnBeforeDeleteClientSpace;
		event NotificationEventHandler<ClientSpace> OnClientSpaceDeleted;
		event InterruptableEventHandler<User> OnBeforeDeleteUser;
		event NotificationEventHandler<User> OnUserDeleted;
		event InterruptableEventHandler<Role> OnBeforeDeleteRole;
		event NotificationEventHandler<Role> OnRoleDeleted;
		event InterruptableEventHandler<PermissionType> OnBeforeDeletePermissionType;
		event NotificationEventHandler<PermissionType> OnPermissionTypeDeleted;

		long? GetRoleIDFromRoleCode(Guid clientSpaceID, string roleCode);

		bool DoesRoleInheritRole(long thisRoleID, long doesItInheritRoleID);
		void InheritRoleFrom(long thisRoleID, long inheritFromRoleID);
		void DisinheritRoleFrom(long thisRoleID, long disinheritFromRoleID);
		List<Role> ListInheritedRoles(long thisRoleID);

		List<Role> ListUserRoles(long userID);
		bool IsUserInRole(long userID, string roleCode);
		void AssignRoleToUser(long userID, string roleCode);
		void UnassignRoleFromUser(long userID, string roleCode);

		void AssignPermissionToUser(long userID, string permissionTypeCode);
		void AssignPermissionToRole(long roleID, string permissionTypeCode);
		bool DoesUserHavePermission(long userID, string permissionTypeCode);

		void RemoveRolesAndPermissionsFromUser(long userID);
		void RemoveRolesAndPermissionsFromRole(long roleID);
		void SetRolesAndPermissionsForUser(long userID, List<string> roleCodes, List<string> permissionTypeCodes);
		void SetRolesAndPermissionsForRole(long roleID, List<string> roleCodes, List<string> permissionTypeCodes);

		List<Role> ListAccessibleRoles(long userID);
		//List<PermissionTypeState> ListPermissionsForUser(long userID);
		List<PermissionTypeState> ListPermissionsForRole(long roleID);
		List<PermissionTypeState> ListAllPermissionTypesAgainstUser(long userID);
		List<PermissionTypeState> ListAllPermissionTypesAgainstRole(long roleID);
		List<RoleState> ListAllRolesAgainstRole(long roleID);
		List<RoleState> ListAllRolesAgainstUser(long userID);
		List<Role> ListDescendentRoles(long roleID);

		List<User> FilterUsers(string partUsername, string partFirstName, string partSurname, string partEmail, int? maxResults, long? editableByUserID, bool? activated, out int totalMatches);
	}
}
