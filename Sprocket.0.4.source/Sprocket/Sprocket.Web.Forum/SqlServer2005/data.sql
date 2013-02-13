DECLARE @permID bigint

IF NOT EXISTS (SELECT PermissionTypeID FROM PermissionTypes WHERE PermissionTypeCode='FORUM_CREATOR')
BEGIN
	EXEC GetUniqueID @permID OUTPUT
	EXEC dbo.StorePermissionType @permID, 'FORUM_CREATOR', 'Create, Edit and Delete Forums', 0
END