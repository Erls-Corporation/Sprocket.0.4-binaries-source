IF OBJECT_ID(N'dbo.SprocketFile') IS NULL
CREATE TABLE dbo.SprocketFile
(
	SprocketFileID		bigint PRIMARY KEY,
	ClientSpaceID		bigint NOT NULL,			-- the client website that owns the file
	FileData			varbinary(max) NOT NULL,	-- the raw image data
	FileTypeExtension	nvarchar(50) NOT NULL,		-- e.g. zip, txt or jpg
	OriginalFileName	nvarchar(300) NOT NULL,		-- the original uploaded filename
	ContentType			nvarchar(200) NOT NULL,		-- the uploaded content MIME type
	Title				nvarchar(500) NOT NULL,		-- an optional title for the file
	Description			nvarchar(max) NOT NULL,		-- an optional description for the file,
	UploadDate			datetime NOT NULL			-- the date/time the file was uploaded
)
go

IF OBJECT_ID(N'dbo.SprocketFile_Store') IS NOT NULL
	DROP PROCEDURE SprocketFile_Store
go
CREATE PROCEDURE dbo.SprocketFile_Store
	@SprocketFileID bigint OUTPUT,
	@ClientSpaceID bigint,
	@FileData varbinary(max) = null,
	@FileTypeExtension nvarchar(50),
	@OriginalFileName nvarchar(300),
	@ContentType nvarchar(200),
	@Title nvarchar(500),
	@Description nvarchar(max),
	@UploadDate datetime
AS
BEGIN
	IF NOT EXISTS (SELECT SprocketFileID FROM SprocketFile WHERE SprocketFileID = @SprocketFileID)
	BEGIN
		IF @SprocketFileID = 0 OR @SprocketFileID IS NULL
			EXEC GetUniqueID @SprocketFileID OUTPUT
		INSERT INTO SprocketFile
			(SprocketFileID, ClientSpaceID, FileData, FileTypeExtension, OriginalFileName, ContentType, Title, Description, UploadDate)
		VALUES
			(@SprocketFileID, @ClientSpaceID, @FileData, @FileTypeExtension, @OriginalFileName, @ContentType, @Title, @Description, @UploadDate)
	END
	ELSE
	BEGIN
		UPDATE SprocketFile SET
			ClientSpaceID = @ClientSpaceID,
			FileData = COALESCE(@FileData, FileData),
			FileTypeExtension = @FileTypeExtension,
			OriginalFileName = @OriginalFileName,
			ContentType = @ContentType,
			Title = @Title,
			Description = @Description,
			UploadDate = @UploadDate
		WHERE SprocketFileID = @SprocketFileID
	END
END
go

IF OBJECT_ID(N'dbo.SprocketFile_Select') IS NOT NULL
	DROP PROCEDURE SprocketFile_Select
go
CREATE PROCEDURE dbo.SprocketFile_Select
	@SprocketFileID bigint,
	@GetFileData bit
AS
BEGIN
	SELECT SprocketFileID, ClientSpaceID,
			CASE @GetFileData WHEN 1 THEN FileData ELSE NULL END as [FileData],
			LEN(FileData) AS [DataLength],
			FileTypeExtension, OriginalFileName, ContentType, Title, Description, UploadDate
	  FROM SprocketFile
	 WHERE SprocketFileID = @SprocketFileID
END

go

IF OBJECT_ID(N'dbo.SprocketFile_Delete') IS NOT NULL
	DROP PROCEDURE SprocketFile_Delete
go
CREATE PROCEDURE dbo.SprocketFile_Delete
	@SprocketFileID bigint
AS
BEGIN
	DELETE
	  FROM SprocketFile
	 WHERE SprocketFileID = @SprocketFileID
END
