IF EXISTS(SELECT id FROM sysobjects WHERE name='InsertSprocketFile' AND type='P')
	DROP PROCEDURE InsertSprocketFile
IF EXISTS(SELECT id FROM sysobjects WHERE name='UpdateSprocketFile' AND type='P')
	DROP PROCEDURE UpdateSprocketFile
IF EXISTS(SELECT id FROM sysobjects WHERE name='SelectSprocketFile' AND type='P')
	DROP PROCEDURE SelectSprocketFile
IF EXISTS(SELECT id FROM sysobjects WHERE name='DeleteSprocketFile' AND type='P')
	DROP PROCEDURE DeleteSprocketFile
IF EXISTS(SELECT id FROM sysobjects WHERE name='SelectSprocketFileByPath' AND type='P')
	DROP PROCEDURE SelectSprocketFileByPath
IF EXISTS(SELECT id FROM sysobjects WHERE name='SelectSprocketFilesByOwner' AND type='P')
	DROP PROCEDURE SelectSprocketFilesByOwner

go

CREATE PROCEDURE dbo.SelectSprocketFileByPath
	@SprocketPath nvarchar(200)
AS
BEGIN
	SELECT *
	  FROM SprocketFiles
	 WHERE SprocketPath = @SprocketPath
END

go

CREATE PROCEDURE dbo.InsertSprocketFile
	@OwnerID			uniqueidentifier,
	@ParentFileID		uniqueidentifier,
	@ClientID			uniqueidentifier,
	@FileTypeExtension	nvarchar(500),
	@SprocketPath		nvarchar(500),
	@ContentType		nvarchar(500),
	@CategoryCode		nvarchar(500),
	@ModuleRegCode		nvarchar(500),
	@Description		nvarchar(500),
	@SprocketFileID		uniqueidentifier,
	@UploadDate			datetime

AS
BEGIN
	INSERT INTO SprocketFiles
		(SprocketFileID, OwnerID, ParentFileID, ClientID, FileTypeExtension, SprocketPath, ContentType, CategoryCode, ModuleRegCode, Description, UploadDate)
	VALUES
		(@SprocketFileID, @OwnerID, @ParentFileID, @ClientID, @FileTypeExtension, @SprocketPath, @ContentType, @CategoryCode, @ModuleRegCode, @Description, @UploadDate)
END

go

CREATE PROCEDURE dbo.UpdateSprocketFile
	@OwnerID			uniqueidentifier,
	@ParentFileID		uniqueidentifier,
	@ClientID			uniqueidentifier,
	@FileTypeExtension	nvarchar(500),
	@SprocketPath		nvarchar(500),
	@ContentType		nvarchar(500),
	@CategoryCode		nvarchar(500),
	@ModuleRegCode		nvarchar(500),
	@Description		nvarchar(500),
	@SprocketFileID		uniqueidentifier,
	@UploadDate			datetime
AS
BEGIN
	UPDATE SprocketFiles SET
	ClientID = @ClientID,
	OwnerID = @OwnerID,
	ParentFileID = @ParentFileID,
	FileTypeExtension = @FileTypeExtension,
	SprocketPath = @SprocketPath,
	ContentType = @ContentType,
	CategoryCode = @CategoryCode,
	ModuleRegCode = @ModuleRegCode,
	Description = @Description,
	UploadDate = @UploadDate
	WHERE SprocketFileID = @SprocketFileID
END

go

CREATE PROCEDURE dbo.SelectSprocketFile
	@SprocketFileID	uniqueidentifier
AS
BEGIN
	SELECT *
	  FROM SprocketFiles
	 WHERE SprocketFileID = @SprocketFileID
END

go

CREATE PROCEDURE dbo.SelectSprocketFilesByOwner
	@OwnerID	uniqueidentifier
AS
BEGIN
	SELECT *
	  FROM SprocketFiles
	 WHERE OwnerID = @OwnerID
  ORDER BY UploadDate DESC
END

go

CREATE PROCEDURE dbo.DeleteSprocketFile
	@SprocketFileID	uniqueidentifier
AS
BEGIN
	DELETE
	  FROM SprocketFiles
	 WHERE SprocketFileID = @SprocketFileID
END
