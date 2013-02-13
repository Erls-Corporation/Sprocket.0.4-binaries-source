IF OBJECT_ID (N'dbo.UniqueIDMaker') IS NULL
CREATE TABLE dbo.UniqueIDMaker (
	NextID bigint identity primary key,
	n smallint
)

GO

IF OBJECT_ID (N'dbo.GetUniqueID') IS NOT NULL
   DROP PROCEDURE dbo.GetUniqueID

GO

CREATE PROCEDURE dbo.GetUniqueID
	@ID bigint OUTPUT
AS
BEGIN
	INSERT INTO UniqueIDMaker (n) VALUES (0)
	SET @ID = SCOPE_IDENTITY()
	DELETE FROM UniqueIDMaker
END
