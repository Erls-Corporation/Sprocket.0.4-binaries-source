IF OBJECT_ID(N'dbo.InitialiseClientSpace') IS NOT NULL
	DROP PROCEDURE InitialiseClientSpace
go
CREATE PROCEDURE  dbo.InitialiseClientSpace
	@ClientSpaceID	bigint,
	@PasswordHash nvarchar(200)
AS
BEGIN
	IF NOT EXISTS(SELECT * FROM ClientSpaces WHERE ClientSpaceID = @ClientSpaceID)
	BEGIN
		DECLARE @newUserID bigint,
				@perm1ID bigint,
				@perm2ID bigint,
				@perm3ID bigint,
				@perm4ID bigint,
				@date datetime

		EXEC GetUniqueID @newUserID OUTPUT
		EXEC GetUniqueID @perm1ID OUTPUT
		EXEC GetUniqueID @perm2ID OUTPUT
		EXEC GetUniqueID @perm3ID OUTPUT
		EXEC GetUniqueID @perm4ID OUTPUT
		SET @date = GETDATE()

		EXEC dbo.StoreClientSpace @ClientSpaceID, 'SprocketCMS', 1, @newUserID
		EXEC dbo.StoreUser @newUserID, @ClientSpaceID, 'admin', @PasswordHash, 'System', 'Administrator', 'admin@localhost', 1, 0, 1, 0, 1, null, @date, null, 0
		IF NOT EXISTS (SELECT PermissionTypeID FROM PermissionTypes WHERE PermissionTypeCode='SUPERUSER')
			EXEC dbo.StorePermissionType @perm1ID, 'SUPERUSER', 'Unrestricted Access', 0
		IF NOT EXISTS (SELECT PermissionTypeID FROM PermissionTypes WHERE PermissionTypeCode='ACCESS_ADMIN')
			EXEC dbo.StorePermissionType @perm2ID, 'ACCESS_ADMIN', 'Allow General Administrative Access', 0
		IF NOT EXISTS (SELECT PermissionTypeID FROM PermissionTypes WHERE PermissionTypeCode='USERADMINISTRATOR')
			EXEC dbo.StorePermissionType @perm3ID, 'USERADMINISTRATOR', 'Access User Administration', 0
		IF NOT EXISTS (SELECT PermissionTypeID FROM PermissionTypes WHERE PermissionTypeCode='ROLEADMINISTRATOR')
			EXEC dbo.StorePermissionType @perm4ID, 'ROLEADMINISTRATOR', 'Create and Modify Roles', 0
		EXEC dbo.AssignPermissionToUser 'SUPERUSER', NULL, @newUserID
	END
END
go

IF OBJECT_ID(N'dbo.Authenticate') IS NOT NULL
	DROP PROCEDURE Authenticate
go
CREATE PROCEDURE dbo.Authenticate
	@ClientSpaceID	bigint,
	@Username		nvarchar(50),
	@PasswordHash	nvarchar(200),
	@IsValid		bit=NULL OUTPUT
AS
BEGIN
	SELECT @IsValid = u.Enabled
	  FROM Users u
 LEFT JOIN ClientSpaces c ON u.ClientSpaceID = c.ClientSpaceID
	 WHERE u.Username = @Username
	   AND u.PasswordHash = @PasswordHash
	   AND u.Enabled = 1
	   AND c.Enabled = 1
	   AND c.ClientSpaceID = @ClientSpaceID

	IF @IsValid IS NULL
		SET @IsValid = 0
	ELSE IF @IsValid = 1
	BEGIN
		DECLARE @dt DATETIME
		SET @dt = GETDATE()
		UPDATE Users
		   SET LastAuthenticated = @dt
		 WHERE Username = @Username
		   AND ClientSpaceID = @ClientSpaceID
	END
END
go

IF OBJECT_ID(N'dbo.IsEmailAddressAvailable') IS NOT NULL
	DROP PROCEDURE IsEmailAddressAvailable
go
CREATE PROCEDURE dbo.IsEmailAddressAvailable
	@ClientSpaceID	bigint,
	@Email			nvarchar(100),
	@ExcludeUserID	bigint=null,
	@Available		bit=null OUTPUT
AS
BEGIN
	IF EXISTS(
		SELECT *
		  FROM Users
		 WHERE Email = @Email
		   AND ClientSpaceID = @ClientSpaceID
		   AND (UserID <> @ExcludeUserID OR @ExcludeUserID IS NULL))
		SET @Available = 0
	ELSE
		SET @Available = 1
END
go

IF OBJECT_ID(N'dbo.IsUsernameAvailable') IS NOT NULL
	DROP PROCEDURE IsUsernameAvailable
go
CREATE PROCEDURE dbo.IsUsernameAvailable
	@ClientSpaceID	bigint,
	@Username		nvarchar(50),
	@ExcludeUserID	bigint=null,
	@Available		bit=null OUTPUT
AS
BEGIN
	IF EXISTS(
		SELECT *
		  FROM Users
		 WHERE Username = @Username
		   AND ClientSpaceID = @ClientSpaceID
		   AND (@ExcludeUserID IS NULL OR UserID <> @ExcludeUserID) )
		SET @Available = 0
	ELSE
		SET @Available = 1
END
go

IF OBJECT_ID(N'dbo.StoreClientSpace') IS NOT NULL
	DROP PROCEDURE StoreClientSpace
go
CREATE PROCEDURE dbo.StoreClientSpace
	@ClientSpaceID	bigint OUTPUT,
	@Name			nvarchar(100),
	@Enabled		bit,
	@PrimaryUserID	bigint
AS
BEGIN
	IF EXISTS(SELECT ClientSpaceID FROM ClientSpaces WHERE ClientSpaceID = @ClientSpaceID)
	BEGIN
		UPDATE ClientSpaces SET
			Name = COALESCE(@Name, Name),
			Enabled = COALESCE(@Enabled, Enabled),
			PrimaryUserID = COALESCE(@PrimaryUserID, PrimaryUserID)
		WHERE
			ClientSpaceID = @ClientSpaceID
	END
	ELSE
	BEGIN
		IF @ClientSpaceID = 0 OR @ClientSpaceID IS NULL
			EXEC GetUniqueID @ClientSpaceID OUTPUT
		INSERT INTO ClientSpaces
			(ClientSpaceID, Name, Enabled, PrimaryUserID)
		VALUES
			(@ClientSpaceID, @Name, @Enabled, @PrimaryUserID)
	END
END
go

IF OBJECT_ID(N'dbo.StoreUser') IS NOT NULL
	DROP PROCEDURE StoreUser
go
CREATE PROCEDURE dbo.StoreUser
	@UserID						bigint OUTPUT,
	@ClientSpaceID				bigint,
	@Username					nvarchar(50),
	@PasswordHash				nvarchar(200),
	@FirstName					nvarchar(50),
	@Surname					nvarchar(50),
	@Email						nvarchar(100),
	@Enabled					bit,
	@Hidden						bit,
	@Locked						bit,
	@Deleted					bit,
	@Activated					bit,
	@ActivationReminderSent		datetime,
	@Created					datetime,
	@LastAuthenticated			datetime,
	@LocalTimeOffsetHours		int
AS
BEGIN
	IF EXISTS(SELECT UserID FROM Users WHERE UserID = @UserID)
	BEGIN
		UPDATE Users SET
			ClientSpaceID = COALESCE(@ClientSpaceID, ClientSpaceID),
			Username = COALESCE(@Username, Username),
			PasswordHash = COALESCE(@PasswordHash, PasswordHash),
			FirstName = COALESCE(@FirstName, FirstName),
			Surname = COALESCE(@Surname, Surname),
			Email = COALESCE(@Email, Email),
			Enabled = COALESCE(@Enabled, Enabled),
			Hidden = COALESCE(@Hidden, Hidden),
			Locked = COALESCE(@Locked, Locked),
			Deleted = COALESCE(@Deleted, Deleted),
			Activated = COALESCE(@Activated, Activated),
			ActivationReminderSent = COALESCE(@ActivationReminderSent, ActivationReminderSent),
			Created = COALESCE(@Created, Created),
			LastAuthenticated = COALESCE(@LastAuthenticated, LastAuthenticated),
			LocalTimeOffsetHours = COALESCE(@LocalTimeOffsetHours, LocalTimeOffsetHours)
		WHERE
			UserID = @UserID
	END
	ELSE
	BEGIN
		IF @UserID = 0 OR @UserID IS NULL
			EXEC GetUniqueID @UserID OUTPUT
		INSERT INTO Users
			(UserID, ClientSpaceID, Username, PasswordHash, FirstName, Surname, Email, Enabled, Hidden, Locked, Deleted, Activated, ActivationReminderSent, Created, LastAuthenticated, LocalTimeOffsetHours)
		VALUES
			(@UserID, @ClientSpaceID, @Username, @PasswordHash, @FirstName, @Surname, @Email, @Enabled, @Hidden, @Locked, @Deleted, @Activated, @ActivationReminderSent, @Created, @LastAuthenticated, @LocalTimeOffsetHours)
	END
END
go

IF OBJECT_ID(N'dbo.StoreRole') IS NOT NULL
	DROP PROCEDURE StoreRole
go
CREATE PROCEDURE dbo.StoreRole
	@RoleID						bigint OUTPUT,
	@RoleCode					nvarchar(100),
	@ClientSpaceID				bigint,
	@Name						nvarchar(100),
	@Enabled					bit,
	@Locked						bit,
	@Hidden						bit
AS
BEGIN
	IF EXISTS(SELECT RoleID FROM Roles WHERE RoleID = @RoleID)
	BEGIN
		UPDATE Roles SET
			RoleCode = COALESCE(@RoleCode, RoleCode),
			ClientSpaceID = COALESCE(@ClientSpaceID, ClientSpaceID),
			Name = COALESCE(@Name, Name),
			Enabled = COALESCE(@Enabled, Enabled),
			Locked = COALESCE(@Locked, Locked),
			Hidden = COALESCE(@Hidden, Hidden)
		WHERE
			RoleID = @RoleID
	END
	ELSE
	BEGIN
		IF @RoleID = 0 OR @RoleID IS NULL
			EXEC GetUniqueID @RoleID OUTPUT
		INSERT INTO Roles
			(RoleID, RoleCode, ClientSpaceID, Name, Enabled, Locked, Hidden)
		VALUES
			(@RoleID, @RoleCode, @ClientSpaceID, @Name, @Enabled, @Locked, @Hidden)
	END
END
go

IF OBJECT_ID(N'dbo.StorePermissionType') IS NOT NULL
	DROP PROCEDURE StorePermissionType
go
CREATE PROCEDURE dbo.StorePermissionType
	@PermissionTypeID			bigint OUTPUT,
	@PermissionTypeCode			nvarchar(100),
	@Description				nvarchar(1000),
	@DefaultValue				bit
AS
BEGIN
	IF EXISTS(SELECT PermissionTypeID FROM PermissionTypes WHERE PermissionTypeID = @PermissionTypeID)
	BEGIN
		UPDATE PermissionTypes SET
			PermissionTypeCode = COALESCE(@PermissionTypeCode, PermissionTypeCode),
			Description = COALESCE(@Description, Description),
			DefaultValue = COALESCE(@DefaultValue, DefaultValue)
		WHERE
			PermissionTypeID = @PermissionTypeID
	END
	ELSE
	BEGIN
		IF @PermissionTypeID = 0 OR @PermissionTypeID IS NULL
			EXEC GetUniqueID @PermissionTypeID OUTPUT
		INSERT INTO PermissionTypes
			(PermissionTypeID, PermissionTypeCode, Description, DefaultValue)
		VALUES
			(@PermissionTypeID, @PermissionTypeCode, @Description, @DefaultValue)
	END
END
go

IF OBJECT_ID(N'dbo.DeleteClientSpace') IS NOT NULL
	DROP PROCEDURE DeleteClientSpace
go
CREATE PROCEDURE dbo.DeleteClientSpace
	@ClientSpaceID bigint
AS
BEGIN
	DELETE
	  FROM ClientSpaces
	 WHERE ClientSpaceID = @ClientSpaceID
END
go

IF OBJECT_ID(N'dbo.DeleteUser') IS NOT NULL
	DROP PROCEDURE DeleteUser
go
CREATE PROCEDURE dbo.DeleteUser
	@UserID bigint
AS
BEGIN
	DELETE
	  FROM Users
	 WHERE UserID = @UserID
END
go

IF OBJECT_ID(N'dbo.DeleteRole') IS NOT NULL
	DROP PROCEDURE DeleteRole
go
CREATE PROCEDURE dbo.DeleteRole
	@RoleID bigint = null,
	@RoleCode nvarchar(100) = null
AS
BEGIN
	DELETE
	  FROM Roles
	 WHERE RoleID = @RoleID
		OR RoleCode = @RoleCode
END
go

IF OBJECT_ID(N'dbo.SelectClientSpace') IS NOT NULL
	DROP PROCEDURE SelectClientSpace
go
CREATE PROCEDURE dbo.SelectClientSpace
	@ClientSpaceID bigint
AS
	SELECT *
	  FROM ClientSpaces
	 WHERE ClientSpaceID = @ClientSpaceID
go

IF OBJECT_ID(N'dbo.SelectUser') IS NOT NULL
	DROP PROCEDURE SelectUser
go
CREATE PROCEDURE dbo.SelectUser
	@ClientSpaceID bigint=null,
	@Username nvarchar(50)=null,
	@UserID bigint=null
AS
	SELECT TOP 1 *
	  FROM Users
	 WHERE (Username = @Username
	   AND ClientSpaceID = @ClientSpaceID)
		OR UserID = @UserID
go

IF OBJECT_ID(N'dbo.SelectRole') IS NOT NULL
	DROP PROCEDURE SelectRole
go
CREATE PROCEDURE dbo.SelectRole
	@ClientSpaceID bigint=null,
	@RoleCode nvarchar(50)=null,
	@RoleID bigint=null
AS
	SELECT TOP 1 *
	  FROM Roles
	 WHERE (RoleCode = @RoleCode
	   AND ClientSpaceID = @ClientSpaceID)
		OR RoleID = @RoleID
go

IF OBJECT_ID(N'dbo.DeletePermissionType') IS NOT NULL
	DROP PROCEDURE DeletePermissionType
go
CREATE PROCEDURE dbo.DeletePermissionType
	@PermissionTypeID bigint = null,
	@PermissionTypeCode nvarchar(100) = null
AS
BEGIN
	DELETE
	  FROM PermissionTypes
	 WHERE PermissionTypeID = @PermissionTypeID
		OR PermissionTypeCode = @PermissionTypeCode
END
go

IF OBJECT_ID(N'dbo.GetRoleIDFromRoleCode') IS NOT NULL
	DROP PROCEDURE GetRoleIDFromRoleCode
go
CREATE PROCEDURE dbo.GetRoleIDFromRoleCode
	@ClientSpaceID bigint,
	@RoleCode nvarchar(100),
	@RoleID bigint OUTPUT
AS
BEGIN
	SELECT @RoleCode = RoleCode
	  FROM Roles
	 WHERE ClientSpaceID = @ClientSpaceID
	   AND RoleCode = @RoleCode
END
go

IF OBJECT_ID(N'dbo.DoesRoleInheritRole') IS NOT NULL
	DROP PROCEDURE DoesRoleInheritRole
go
CREATE PROCEDURE dbo.DoesRoleInheritRole
	@ThisRoleID bigint,
	@DoesItInheritRoleID bigint,
	@Result bit=null OUTPUT
AS
BEGIN
	CREATE TABLE #r ( RoleID uniqueidentifier )
	INSERT INTO #r (RoleID)
		SELECT InheritsRoleID
		  FROM RoleToRole
		 WHERE RoleID = @ThisRoleID
	WHILE @@ROWCOUNT > 0
		INSERT INTO #r (RoleID)
			SELECT InheritsRoleID
			  FROM RoleToRole
			 WHERE RoleID IN (SELECT RoleID FROM #r)
			   AND InheritsRoleID NOT IN (SELECT RoleID FROM #r)
	IF EXISTS (SELECT RoleID FROM #r WHERE RoleID = @DoesItInheritRoleID)
		SET @Result = 1
	ELSE
		SET @Result = 0
END
go

IF OBJECT_ID(N'dbo.InheritRoleFrom') IS NOT NULL
	DROP PROCEDURE InheritRoleFrom
go

CREATE PROCEDURE dbo.InheritRoleFrom
	@ThisRoleID bigint,
	@InheritFromRoleID bigint=null,
	@InheritFromRoleCode bigint=null
AS
BEGIN
	IF @InheritFromRoleID IS NULL
		SELECT @InheritFromRoleID = RoleID
		  FROM Roles
		 WHERE RoleCode = @InheritFromRoleCode
		   AND ClientSpaceID = (SELECT ClientSpaceID
								   FROM Roles
								  WHERE RoleID = @ThisRoleID)
	DECLARE @IsAncestor bit
	EXEC DoesRoleInheritRole @ThisRoleID, @InheritFromRoleID, @IsAncestor OUTPUT -- find out if it already inherits this roleid
	IF @IsAncestor = 0 BEGIN
		EXEC DoesRoleInheritRole @InheritFromRoleID, @ThisRoleID, @IsAncestor OUTPUT -- find out of the other roleid already inherits this roleid
		IF @IsAncestor = 0
			INSERT INTO RoleToRole (RoleID, InheritsRoleID)
				VALUES (@ThisRoleID, @InheritFromRoleID)
	END
END
go

IF OBJECT_ID(N'dbo.DisinheritRoleFrom') IS NOT NULL
	DROP PROCEDURE DisinheritRoleFrom
go
CREATE PROCEDURE dbo.DisinheritRoleFrom
	@RoleID				bigint,
	@DisinheritRoleID	bigint
AS
BEGIN
	DELETE
	  FROM RoleToRole
	 WHERE RoleID = @RoleID
	   AND InheritsRoleID = @DisinheritRoleID
END
go

IF OBJECT_ID(N'dbo.ListInheritedRoles') IS NOT NULL
	DROP PROCEDURE ListInheritedRoles
go
CREATE PROCEDURE dbo.ListInheritedRoles
	@RoleID bigint
AS
BEGIN
	SELECT *
	  FROM Roles
	 WHERE RoleID IN
		(SELECT InheritsRoleID
		   FROM RoleToRole
		  WHERE RoleID = @RoleID)
END
go

IF OBJECT_ID(N'dbo.ListUserRoles') IS NOT NULL
	DROP PROCEDURE ListUserRoles
go
CREATE PROCEDURE dbo.ListUserRoles
	@UserID bigint
AS
BEGIN
	SELECT *
	  FROM Roles
	 WHERE RoleID IN
		(SELECT RoleID
		   FROM UserToRole
		  WHERE UserID = @UserID)
END
go

IF OBJECT_ID(N'dbo.IsUserInRole') IS NOT NULL
	DROP PROCEDURE IsUserInRole
go
CREATE PROCEDURE dbo.IsUserInRole
	@UserID bigint,
	@RoleCode nvarchar(100),
	@IsUserInRole bit=NULL OUTPUT
AS
BEGIN
	DECLARE @SuperUser bit
	EXEC DoesUserHavePermission @UserID, 'SUPERUSER', @SuperUser OUTPUT
	IF @SuperUser = 1
		SET @IsUserInRole = 1
	ELSE
	BEGIN
		DECLARE @RoleID bigint
		SELECT @RoleID = RoleID
		  FROM Roles
		 WHERE RoleCode = @RoleCode
		   AND ClientSpaceID = (SELECT ClientSpaceID
							 FROM Users
							WHERE UserID = @UserID)
		
		CREATE TABLE #r ( RoleID bigint )
		INSERT INTO #r (RoleID)
			SELECT DISTINCT RoleID
			  FROM UserToRole
			 WHERE UserID = @UserID
		
		WHILE @@ROWCOUNT > 0
			INSERT INTO #r (RoleID)
				SELECT InheritsRoleID
				  FROM RoleToRole
				 WHERE RoleID IN (SELECT RoleID FROM #r)
				   AND InheritsRoleID NOT IN (SELECT RoleID FROM #r)
		
		DELETE FROM #r
		 WHERE RoleID IN
			(SELECT RoleID FROM Roles WHERE Enabled=0)
		
		IF EXISTS (SELECT RoleID FROM #r WHERE RoleID = @RoleID)
			SET @IsUserInRole = 1
		ELSE
			SET @IsUserInRole = 0
	END
END
go

IF OBJECT_ID(N'dbo.AssignRoleToUser') IS NOT NULL
	DROP PROCEDURE AssignRoleToUser
go
CREATE PROCEDURE dbo.AssignRoleToUser
	@UserID bigint,
	@RoleCode nvarchar(100)=null,
	@RoleID bigint=null
AS
BEGIN
	IF @RoleID IS NULL
		SELECT @RoleID = RoleID
		  FROM Roles
		 WHERE RoleCode = @RoleCode
		   AND ClientSpaceID = (SELECT ClientSpaceID
								  FROM Users
								 WHERE UserID = @UserID)
	IF NOT EXISTS
		(SELECT RoleID
		   FROM UserToRole
		  WHERE UserID = @UserID)
		INSERT INTO UserToRole (UserID, RoleID)
			VALUES (@UserID, @RoleID)
END
go

IF OBJECT_ID(N'dbo.UnassignRoleFromUser') IS NOT NULL
	DROP PROCEDURE UnassignRoleFromUser
go
CREATE PROCEDURE dbo.UnassignRoleFromUser
	@UserID bigint,
	@RoleCode nvarchar(100)
AS
BEGIN
	DECLARE @RoleID bigint
	SELECT @RoleID = RoleID
	  FROM Roles
	 WHERE RoleCode = @RoleCode
	   AND ClientSpaceID = (SELECT ClientSpaceID
							  FROM Users
							 WHERE UserID = @UserID)
	DELETE
	  FROM UserToRole
	 WHERE UserID = @UserID
	   AND RoleID = @RoleID
END
go

IF OBJECT_ID(N'dbo.AssignPermissionToUser') IS NOT NULL
	DROP PROCEDURE AssignPermissionToUser
go
CREATE PROCEDURE dbo.AssignPermissionToUser
	@PermissionTypeCode	nvarchar(100)=null,
	@PermissionTypeID	bigint=null,
	@UserID				bigint
AS
BEGIN
	IF @PermissionTypeID IS NULL
		SELECT @PermissionTypeID = PermissionTypeID
		  FROM PermissionTypes
		 WHERE PermissionTypeCode = @PermissionTypeCode

	DELETE FROM [Permissions]
	 WHERE UserID = @UserID
	   AND PermissionTypeID = @PermissionTypeID
	
	INSERT INTO [Permissions]
		(PermissionTypeID, UserID, Value)
	VALUES
		(@PermissionTypeID, @UserID, 1)
END
go

IF OBJECT_ID(N'dbo.AssignPermissionToRole') IS NOT NULL
	DROP PROCEDURE AssignPermissionToRole
go
CREATE PROCEDURE dbo.AssignPermissionToRole
	@PermissionTypeCode	nvarchar(100)=null,
	@PermissionTypeID	bigint=null,
	@RoleID				bigint
AS
BEGIN
	IF @PermissionTypeID IS NULL
		SELECT @PermissionTypeID = PermissionTypeID
		  FROM PermissionTypes
		 WHERE PermissionTypeCode = @PermissionTypeCode
	DELETE
	  FROM Permissions
	 WHERE RoleID = @RoleID
	   AND PermissionTypeID = @PermissionTypeID
	
	INSERT INTO Permissions
		(PermissionTypeID, RoleID, Value)
	VALUES
		(@PermissionTypeID, @RoleID, 1)
END
go

IF OBJECT_ID(N'dbo.DoesUserHavePermission') IS NOT NULL
	DROP PROCEDURE DoesUserHavePermission
go
CREATE PROCEDURE dbo.DoesUserHavePermission
	@UserID				bigint,
	@PermissionTypeCode	varchar(100),
	@HasPermission		bit=null OUTPUT
AS
BEGIN
	IF (SELECT PrimaryUserID
		  FROM ClientSpaces c
		 WHERE c.ClientSpaceID = (SELECT ClientSpaceID
									FROM Users
								   WHERE UserID = @UserID)) = @UserID
	BEGIN
		SET @HasPermission = 1
	END
	ELSE
	BEGIN
		CREATE TABLE #r ( RoleID bigint )
		INSERT INTO #r (RoleID)
			SELECT DISTINCT RoleID
			  FROM UserToRole
			 WHERE UserID = @UserID
		
		WHILE @@ROWCOUNT > 0
			INSERT INTO #r (RoleID)
				SELECT InheritsRoleID
				  FROM RoleToRole
				 WHERE RoleID IN (SELECT RoleID FROM #r)
				   AND InheritsRoleID NOT IN (SELECT RoleID FROM #r)
		
		DELETE FROM #r
		 WHERE RoleID IN
			(SELECT RoleID FROM Roles WHERE Enabled=0)
		
		IF EXISTS (SELECT Value
					 FROM [Permissions]
					WHERE (PermissionTypeID = (SELECT PermissionTypeID
												FROM PermissionTypes
											   WHERE PermissionTypeCode = @PermissionTypeCode)
					   OR PermissionTypeID = (SELECT PermissionTypeID
												FROM PermissionTypes
											   WHERE PermissionTypeCode = 'SUPERUSER'))
					  AND (UserID = @UserID
					   OR RoleID IN (SELECT RoleID FROM #r)))
			SET @HasPermission = 1
		ELSE
			SET @HasPermission = 0
	END
END
go

IF OBJECT_ID(N'dbo.RemoveRolesAndPermissionsFromUser') IS NOT NULL
	DROP PROCEDURE RemoveRolesAndPermissionsFromUser
go
CREATE PROCEDURE dbo.RemoveRolesAndPermissionsFromUser
	@UserID				bigint
AS
BEGIN
	DELETE FROM UserToRole
	 WHERE UserID = @UserID
	
	DELETE FROM [Permissions]
	 WHERE UserID = @UserID
END
go

IF OBJECT_ID(N'dbo.RemoveRolesAndPermissionsFromRole') IS NOT NULL
	DROP PROCEDURE RemoveRolesAndPermissionsFromRole
go
CREATE PROCEDURE dbo.RemoveRolesAndPermissionsFromRole
	@RoleID				bigint
AS
BEGIN
	DELETE FROM RoleToRole
	 WHERE RoleID = @RoleID
	
	DELETE FROM [Permissions]
	 WHERE RoleID = @RoleID
END
go

IF OBJECT_ID(N'dbo.ListAccessibleRolesForUser') IS NOT NULL
	DROP PROCEDURE ListAccessibleRolesForUser
go
CREATE PROCEDURE dbo.ListAccessibleRolesForUser
	@UserID	bigint
AS
BEGIN
	DECLARE @SuperUser bit
	EXEC IsUserInRole @UserID, 'SUPERUSER', @SuperUser OUTPUT
	IF @SuperUser = 1
	BEGIN
		SELECT *, CAST(0 AS bit) AS [Inherited]
		  FROM Roles
	  ORDER BY Name
	END
	ELSE
	BEGIN
		CREATE TABLE #r ( RoleID bigint, Inherited bit )
		INSERT INTO #r (RoleID, Inherited)
			SELECT DISTINCT RoleID, 0
			  FROM UserToRole
			 WHERE UserID = @UserID
		
		WHILE @@ROWCOUNT > 0
			INSERT INTO #r (RoleID, Inherited)
				SELECT InheritsRoleID, 1
				  FROM RoleToRole
				 WHERE RoleID IN (SELECT RoleID FROM #r)
				   AND InheritsRoleID NOT IN (SELECT RoleID FROM #r)

		DELETE FROM #r
		 WHERE RoleID IN
			(SELECT RoleID FROM Roles WHERE Enabled=0)

		SELECT r.*, Inherited
		  FROM #r
	 LEFT JOIN Roles r
			ON #r.RoleID = r.RoleID
	  ORDER BY r.Name
	END
END
go

IF OBJECT_ID(N'dbo.ListPermissionsForUser') IS NOT NULL
	DROP PROCEDURE ListPermissionsForUser
go
CREATE PROCEDURE dbo.ListPermissionsForUser
	@UserID bigint
AS
BEGIN
	CREATE TABLE #r ( RoleID bigint )
	INSERT INTO #r (RoleID)
		SELECT DISTINCT RoleID
		  FROM UserToRole
		 WHERE UserID = @UserID
	
	WHILE @@ROWCOUNT > 0
		INSERT INTO #r (RoleID)
			SELECT InheritsRoleID
			  FROM RoleToRole
			 WHERE RoleID IN (SELECT RoleID FROM #r)
			   AND InheritsRoleID NOT IN (SELECT RoleID FROM #r)

	DELETE FROM #r
	 WHERE RoleID IN
		(SELECT RoleID FROM Roles WHERE Enabled=0)
	
	SELECT pt.PermissionTypeID, PermissionTypeCode, Description, DefaultValue,
		   CAST(CASE WHEN COUNT(Value) > 0 THEN 1 ELSE 0 END AS bit) AS [HasPermission],
		   CAST(CASE WHEN p.UserID = @UserID OR COUNT(Value) = 0 THEN 0 ELSE 1 END AS bit) AS [Inherited]
	  FROM PermissionTypes pt
 LEFT JOIN [Permissions] p
		ON pt.PermissionTypeID = p.PermissionTypeID
	   AND (p.UserID = @UserID
			OR p.RoleID IN (SELECT RoleID FROM #r))
  GROUP BY pt.PermissionTypeID, PermissionTypeCode, Description, DefaultValue, p.UserID, p.RoleID
  ORDER BY pt.PermissionTypeID
END
go

IF OBJECT_ID(N'dbo.ListPermissionsForRole') IS NOT NULL
	DROP PROCEDURE ListPermissionsForRole
go
CREATE PROCEDURE dbo.ListPermissionsForRole
	@RoleID bigint,
	@ShowAllPermissions bit=0
AS
BEGIN
	SELECT *
	  FROM PermissionTypes pt
 LEFT JOIN [Permissions] p
		ON p.PermissionTypeID = pt.PermissionTypeID
	   AND p.RoleID = @RoleID
	 WHERE (@ShowAllPermissions = 1 OR (p.RoleID = @RoleID AND p.Value = 1))
  ORDER BY pt.PermissionTypeID
END
go

IF OBJECT_ID(N'dbo.ListAllRolesAgainstUser') IS NOT NULL
	DROP PROCEDURE ListAllRolesAgainstUser
go
CREATE PROCEDURE dbo.ListAllRolesAgainstUser
	@UserID bigint
AS
BEGIN
	SELECT r.*, CAST(CASE WHEN ur.RoleID IS NOT NULL THEN 1 ELSE 0 END AS bit) AS HasRole
	  FROM Roles r
 LEFT JOIN UserToRole ur
		ON r.RoleID = ur.RoleID
	   AND ur.UserID = @UserID
	 WHERE r.Enabled = 1
  ORDER BY r.Name
END
go

IF OBJECT_ID(N'dbo.ListAllRolesAgainstRole') IS NOT NULL
	DROP PROCEDURE ListAllRolesAgainstRole
go
CREATE PROCEDURE dbo.ListAllRolesAgainstRole
	@RoleID bigint
AS
BEGIN
	SELECT *, CAST(CASE WHEN rr.InheritsRoleID IS NULL THEN 0 ELSE 1 END AS bit) as [Inherited]
	  FROM Roles r
 LEFT JOIN RoleToRole rr
		ON r.RoleID = rr.InheritsRoleID
	   AND rr.RoleID = @RoleID
	 WHERE r.RoleID <> @RoleID
  ORDER BY r.Name
END
go

IF OBJECT_ID(N'dbo.ListAllPermissionTypesAgainstRole') IS NOT NULL
	DROP PROCEDURE ListAllPermissionTypesAgainstRole
go
CREATE PROCEDURE dbo.ListAllPermissionTypesAgainstRole
	@RoleID bigint
AS
BEGIN
	SELECT *
	  FROM PermissionTypes pt
 LEFT JOIN Permissions p
		ON p.PermissionTypeID = pt.PermissionTypeID
	   AND p.RoleID = @RoleID
  ORDER BY pt.PermissionTypeID
END

go

IF OBJECT_ID(N'dbo.ListUsers') IS NOT NULL
	DROP PROCEDURE ListUsers
go
CREATE PROCEDURE dbo.ListUsers
	@UserID		bigint = null,
	@ClientSpaceID	bigint = null,
	@Username	nvarchar(50) = null,
	@FirstName	nvarchar(50) = null,
	@Surname	nvarchar(50) = null,
	@Email		varchar(100) = null,
	@Enabled	bit = null,
	@Hidden		bit = null,
	@Locked		bit = null,
	@Deleted	bit = null,
	@Activated	bit = null,
	@MaxRecords	int = null,
	@ExactMatches bit = null,
	@EditableByUserID bigint = null,
	@TotalMatches int=null OUTPUT
AS
BEGIN
	IF @MaxRecords IS NULL AND @EditableByUserID IS NULL
	BEGIN
		SELECT *
		  FROM Users
		 WHERE	(@UserID IS NULL OR UserID = @UserID)
			AND (@ClientSpaceID IS NULL OR ClientSpaceID = @ClientSpaceID)
			AND (@Username IS NULL OR (Username LIKE '%' + @Username + '%' AND (@ExactMatches=0 OR @ExactMatches IS NULL)) OR @Username = Username )
			AND (@FirstName IS NULL OR (FirstName LIKE '%' + @FirstName + '%' AND (@ExactMatches=0 OR @ExactMatches IS NULL)) OR @FirstName = FirstName)
			AND (@Surname IS NULL OR (Surname LIKE '%' + @Surname + '%' AND (@ExactMatches=0 OR @ExactMatches IS NULL)) OR @Surname = Surname)
			AND (@Email IS NULL OR (Email LIKE '%' + @Email + '%' AND (@ExactMatches=0 OR @ExactMatches IS NULL)) OR @Email = Email)
			AND (@Enabled IS NULL OR Enabled = @Enabled)
			AND (@Hidden IS NULL OR Hidden = @Hidden)
			AND (@Locked IS NULL OR Locked = @Locked)
			AND (@Deleted IS NULL OR Deleted = @Deleted)
			AND (@Activated IS NULL OR Activated = @Activated)
		ORDER BY Username
	END
	ELSE
	BEGIN
		SELECT UserID
		  INTO #ids
		  FROM Users
		 WHERE	(@UserID IS NULL OR UserID = @UserID)
			AND (@ClientSpaceID IS NULL OR ClientSpaceID = @ClientSpaceID)
			AND (@Username IS NULL OR (Username LIKE '%' + @Username + '%' AND (@ExactMatches=0 OR @ExactMatches IS NULL)) OR @Username = Username )
			AND (@FirstName IS NULL OR (FirstName LIKE '%' + @FirstName + '%' AND (@ExactMatches=0 OR @ExactMatches IS NULL)) OR @FirstName = FirstName)
			AND (@Surname IS NULL OR (Surname LIKE '%' + @Surname + '%' AND (@ExactMatches=0 OR @ExactMatches IS NULL)) OR @Surname = Surname)
			AND (@Email IS NULL OR (Email LIKE '%' + @Email + '%' AND (@ExactMatches=0 OR @ExactMatches IS NULL)) OR @Email = Email)
			AND (@Enabled IS NULL OR Enabled = @Enabled)
			AND (@Hidden IS NULL OR Hidden = @Hidden)
			AND (@Locked IS NULL OR Locked = @Locked)
			AND (@Deleted IS NULL OR Deleted = @Deleted)
			AND (@Activated IS NULL OR Activated = @Activated)

		DECLARE @SuperUser bit
		EXEC IsUserInRole @EditableByUserID, 'SUPERUSER', @SuperUser OUTPUT

		IF @EditableByUserID IS NOT NULL AND @SuperUser = 0
		BEGIN
			-- get all roles available to the specified user
			CREATE TABLE #r ( RoleID bigint )
			INSERT INTO #r (RoleID)
				SELECT DISTINCT RoleID
				  FROM UserToRole
				 WHERE UserID = @EditableByUserID
			
			WHILE @@ROWCOUNT > 0
				INSERT INTO #r (RoleID)
					SELECT InheritsRoleID
					  FROM RoleToRole
					 WHERE RoleID IN (SELECT RoleID FROM #r)
					   AND InheritsRoleID NOT IN (SELECT RoleID FROM #r)
			
			-- remove the disabled roles from the list
			DELETE FROM #r
				  WHERE RoleID IN (SELECT RoleID
									 FROM Roles
									WHERE Enabled = 0)
			
			-- get all permissions tied to the above roles and to the specified user
			CREATE TABLE #p ( PermissionTypeID bigint )
			INSERT INTO #p
				 SELECT PermissionTypeID
				   FROM Permissions
				  WHERE UserID = @EditableByUserID
					 OR RoleID IN (SELECT RoleID FROM #r)

			-- remove all found users that have roles or permissions that the specified user does NOT have
			DELETE FROM #ids
				  WHERE UserID IN (SELECT UserID
									 FROM UserToRole
									WHERE RoleID NOT IN (SELECT RoleID FROM #r))
					 OR UserID IN (SELECT UserID
									 FROM Permissions
									WHERE Value = 1
									  AND PermissionTypeID NOT IN (SELECT PermissionTypeID FROM #p))
		END

		IF @MaxRecords IS NOT NULL
		BEGIN
			SELECT TOP (@MaxRecords) u.*
			  FROM #ids i
		 LEFT JOIN Users u
				ON i.UserID = u.UserID
		  ORDER BY u.Username
		END
		ELSE
		BEGIN
			SELECT u.*
			  FROM #ids i
		 LEFT JOIN Users u
				ON i.UserID = u.UserID
		  ORDER BY u.Username
		END
	END
END


GO

IF OBJECT_ID(N'dbo.ActivateUser') IS NOT NULL
	DROP PROCEDURE ActivateUser
go
CREATE PROCEDURE dbo.ActivateUser
	@ActivationCode nvarchar(100),
	@Success bit=null OUTPUT,
	@UserID bigint=null OUTPUT
AS
BEGIN
	SET @Success = 0
	DECLARE @Email nvarchar(100)
	
	SELECT @Email = Email,
		   @UserID = UserID
	  FROM UserActivationRequests
	 WHERE ActivationCode = @ActivationCode
	   
	IF @Email IS NOT NULL
	BEGIN
		UPDATE Users
		   SET Email = @Email,
			   Activated = 1
		 WHERE UserID = @UserID
		 
		IF @@ROWCOUNT > 0
		BEGIN
			DELETE
			  FROM UserActivationRequests
			 WHERE UserID = @UserID
			 
			SET @Success = 1
		END
	END
END

go

IF OBJECT_ID(N'dbo.SetEmailChangeRequest') IS NOT NULL
	DROP PROCEDURE SetEmailChangeRequest
go
CREATE PROCEDURE dbo.SetEmailChangeRequest
	@UserID bigint,
	@Email nvarchar(100),
	@ActivationCode nvarchar(100)
AS
BEGIN
	DELETE
	  FROM UserActivationRequests
	 WHERE UserID = @UserID
	
	INSERT INTO UserActivationRequests (UserID, Email, ActivationCode, RequestDate)
	VALUES (@UserID, @Email, @ActivationCode, (GETUTCDATE()))
END

go

IF OBJECT_ID(N'dbo.SelectEmailChangeRequest') IS NOT NULL
	DROP PROCEDURE SelectEmailChangeRequest
go
CREATE PROCEDURE dbo.SelectEmailChangeRequest
	@UserID bigint
AS
BEGIN
	SELECT *
	  FROM UserActivationRequests
	 WHERE UserID = @UserID
END