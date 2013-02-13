IF OBJECT_ID(N'dbo.ClientSpaces') IS NULL
CREATE TABLE dbo.ClientSpaces
(
	ClientSpaceID	bigint				PRIMARY KEY,
	Name			nvarchar(100)		NOT NULL,
	Enabled			bit					NOT NULL,
	PrimaryUserID	bigint
)

go

IF OBJECT_ID(N'dbo.Users') IS NULL
CREATE TABLE dbo.Users
(
	UserID					bigint				PRIMARY KEY,
	ClientSpaceID			bigint				NOT NULL FOREIGN KEY REFERENCES ClientSpaces(ClientSpaceID) ON DELETE CASCADE,
	Username				nvarchar(50)		NOT NULL,
	PasswordHash			nvarchar(200)		NOT NULL,
	FirstName				nvarchar(50),
	Surname					nvarchar(50),
	Email					nvarchar(100),
	Enabled					bit					NOT NULL,
	Hidden					bit					NOT NULL, -- hides the user from being displayed (used internally by the system)
	Locked					bit					NOT NULL,
	Deleted					bit					NOT NULL,
	Activated				bit					NOT NULL,
	ActivationReminderSent	datetime,
	Created					datetime			NOT NULL,
	LastAuthenticated		datetime,
	LocalTimeOffsetHours	int					NOT NULL
	
	CONSTRAINT UC_Users_UsernameClientID UNIQUE CLUSTERED(ClientSpaceID, Username)
)

go

IF OBJECT_ID(N'dbo.Roles') IS NULL
CREATE TABLE dbo.Roles
(
	RoleID			bigint				PRIMARY KEY,
	RoleCode		nvarchar(100)		NOT NULL,
	ClientSpaceID	bigint				NOT NULL FOREIGN KEY REFERENCES ClientSpaces(ClientSpaceID) ON DELETE CASCADE,
	Name			nvarchar(100)		NOT NULL,
	Enabled			bit					NOT NULL,	-- disables the role without deleting it
	Locked			bit					NOT NULL,	-- prevents role from being modified by users
	Hidden			bit					NOT NULL	-- hides the role from users (for system roles)
	
	CONSTRAINT UC_Roles_RoleCodeClientID UNIQUE CLUSTERED(ClientSpaceID, RoleCode)
)

go

IF OBJECT_ID(N'dbo.RoleToRole') IS NULL
CREATE TABLE dbo.RoleToRole
(
	RoleID			bigint,
	InheritsRoleID	bigint,
	
	CONSTRAINT PK_RoleToRole PRIMARY KEY CLUSTERED(RoleID, InheritsRoleID)
)

go

IF OBJECT_ID(N'dbo.UserToRole') IS NULL
CREATE TABLE dbo.UserToRole
(
	RoleID		bigint,
	UserID		bigint FOREIGN KEY REFERENCES Users(UserID) ON DELETE CASCADE,
	
	CONSTRAINT PK_UserToRole PRIMARY KEY CLUSTERED(RoleID, UserID)
)

go

IF OBJECT_ID(N'dbo.PermissionTypes') IS NULL
CREATE TABLE dbo.PermissionTypes
(
	PermissionTypeID		bigint				PRIMARY KEY,
	PermissionTypeCode		nvarchar(100)		UNIQUE NOT NULL,
	Description				nvarchar(1000)		NOT NULL,
	DefaultValue			bit					NOT NULL
)

go

IF OBJECT_ID(N'dbo.Permissions') IS NULL
CREATE TABLE dbo.Permissions
(
	PermissionTypeID	bigint		NOT NULL,
	UserID				bigint		NULL FOREIGN KEY REFERENCES Users(UserID) ON DELETE CASCADE,
	RoleID				bigint		NULL,
	Value				bit			NOT NULL
	
	CONSTRAINT UNIQUE_Permissions UNIQUE(PermissionTypeID, UserID, RoleID)
)

go

IF OBJECT_ID(N'dbo.UserActivationRequests') IS NULL
CREATE TABLE dbo.UserActivationRequests
(
	UserID bigint PRIMARY KEY FOREIGN KEY REFERENCES Users(UserID) ON DELETE CASCADE,
	Email nvarchar(100) NOT NULL,
	ActivationCode nvarchar(100) NOT NULL,
	RequestDate datetime NOT NULL
)

go

IF OBJECT_ID(N'dbo.OnDeleteRole') IS NOT NULL
	DROP TRIGGER dbo.OnDeleteRole
go
CREATE TRIGGER dbo.OnDeleteRole ON dbo.Roles
FOR DELETE
AS
BEGIN
	DELETE FROM RoleToRole
	 WHERE RoleID IN (SELECT RoleID FROM deleted)
		OR InheritsRoleID IN (SELECT RoleID FROM deleted)
		
	DELETE FROM UserToRole
	 WHERE RoleID IN (SELECT RoleID FROM deleted)
		
	DELETE FROM [Permissions]
	 WHERE RoleID IN (SELECT RoleID FROM deleted)
END
go