CREATE TABLE IF NOT EXISTS ClientSpaces
(
	ClientSpaceID			GUID				PRIMARY KEY,
	Name					TEXT(100)			NOT NULL,
	Enabled					BOOLEAN				NOT NULL,
	PrimaryUserID			INTEGER
);

CREATE TABLE IF NOT EXISTS Users
(
	UserID					INTEGER				PRIMARY KEY,
	ClientSpaceID			GUID				NOT NULL,
	Username				TEXT(50)			NOT NULL,
	PasswordHash			TEXT(200)			NOT NULL,
	FirstName				TEXT(50),
	Surname					TEXT(50),
	Email					TEXT(100),
	Enabled					BOOLEAN				NOT NULL,
	Locked					BOOLEAN				NOT NULL,
	Deleted					BOOLEAN				NOT NULL,
	Hidden					BOOLEAN				NOT NULL,
	Activated				BOOLEAN				NOT NULL,
	ActivationReminderSent	DATETIME,
	Created					DATETIME			NOT NULL,
	LocalTimeOffsetHours	INTEGER				NOT NULL
	
	UNIQUE (ClientSpaceID, Username, Deleted)
);

CREATE TABLE IF NOT EXISTS Roles
(
	RoleID					INTEGER				PRIMARY KEY,
	RoleCode				TEXT(100)			NOT NULL,
	ClientSpaceID			GUID				NOT NULL,
	Name					TEXT(100)			NOT NULL,
	Enabled					BOOLEAN				NOT NULL,	-- disables the role without deleting it
	Locked					BOOLEAN				NOT NULL,	-- prevents role from being modified by users
	Hidden					BOOLEAN				NOT NULL,	-- hides the role from users
	
	UNIQUE (ClientSpaceID, RoleCode)
);

CREATE TABLE IF NOT EXISTS RoleToRole
(
	RoleID					INTEGER,
	InheritsRoleID			INTEGER,
	
	PRIMARY KEY (RoleID, InheritsRoleID)
);

CREATE TABLE IF NOT EXISTS UserToRole
(
	RoleID					INTEGER,
	UserID					INTEGER,
	
	PRIMARY KEY (RoleID, UserID)
);

CREATE TABLE IF NOT EXISTS PermissionTypes
(
	PermissionTypeID		INTEGER				PRIMARY KEY,
	PermissionTypeCode		TEXT(100)			UNIQUE NOT NULL,
	Description				TEXT(1000)			NOT NULL,
	DefaultValue			BOOLEAN				NOT NULL
);

CREATE TABLE IF NOT EXISTS Permissions
(
	PermissionTypeID		INTEGER				NOT NULL,
	RoleID					INTEGER				NULL, -- One of either RoleID or UserID must have a value, but not both
	UserID					INTEGER				NULL,
	Value					BOOLEAN				NOT NULL,
	
	PRIMARY KEY (PermissionTypeID, RoleID, UserID)
);

DROP TRIGGER IF EXISTS trigger_ClientSpaces_Delete;
CREATE TRIGGER trigger_ClientSpaces_Delete BEFORE DELETE ON ClientSpaces
BEGIN
	DELETE FROM Roles WHERE ClientSpaceID = OLD.ClientSpaceID;
	DELETE FROM Users WHERE ClientSpaceID = OLD.ClientSpaceID;
END;

DROP TRIGGER IF EXISTS trigger_Users_Delete;
CREATE TRIGGER trigger_Users_Delete BEFORE DELETE ON Users
BEGIN
	DELETE FROM UserToRole WHERE UserID = OLD.UserID;
	DELETE FROM Permissions WHERE UserID = OLD.UserID;
END;

DROP TRIGGER IF EXISTS trigger_Roles_Delete;
CREATE TRIGGER trigger_Roles_Delete BEFORE DELETE ON Users
BEGIN
	DELETE FROM UserToRole WHERE RoleID = OLD.RoleID;
	DELETE FROM RoleToRole WHERE RoleID = OLD.RoleID;
	DELETE FROM Permissions WHERE RoleID = OLD.RoleID;
END;
