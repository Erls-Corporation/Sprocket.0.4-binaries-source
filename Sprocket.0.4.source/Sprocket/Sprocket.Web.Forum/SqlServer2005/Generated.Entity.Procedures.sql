IF OBJECT_ID(N'dbo.Forum_Store') IS NOT NULL
	DROP PROCEDURE Forum_Store
go
CREATE PROCEDURE dbo.Forum_Store
	@ForumID bigint OUTPUT,
	@ForumCategoryID bigint,
	@ForumCode nvarchar(50) = null,
	@Name nvarchar(250),
	@Description nvarchar(max),
	@URLToken nvarchar(50) = null,
	@DateCreated datetime,
	@Rank int = null,
	@PostWriteAccess smallint,
	@ReplyWriteAccess smallint,
	@ReadAccess smallint,
	@PostWriteAccessRoleID bigint = null,
	@ReplyWriteAccessRoleID bigint = null,
	@ReadAccessRoleID bigint = null,
	@ModeratorRoleID bigint = null,
	@MarkupLevel smallint,
	@ShowSignatures bit = null,
	@AllowImagesInMessages bit,
	@AllowImagesInSignatures bit,
	@RequireModeration bit,
	@AllowVoting bit,
	@TopicDisplayOrder smallint,
	@Locked bit
AS
BEGIN
	IF NOT EXISTS (SELECT ForumID FROM Forum WHERE ForumID = @ForumID)
	BEGIN
		IF @ForumID = 0 OR @ForumID IS NULL
			EXEC GetUniqueID @ForumID OUTPUT
		INSERT INTO Forum
			(ForumID, ForumCategoryID, ForumCode, Name, Description, URLToken, DateCreated, Rank, PostWriteAccess, ReplyWriteAccess, ReadAccess, PostWriteAccessRoleID, ReplyWriteAccessRoleID, ReadAccessRoleID, ModeratorRoleID, MarkupLevel, ShowSignatures, AllowImagesInMessages, AllowImagesInSignatures, RequireModeration, AllowVoting, TopicDisplayOrder, Locked)
		VALUES
			(@ForumID, @ForumCategoryID, @ForumCode, @Name, @Description, @URLToken, @DateCreated, @Rank, @PostWriteAccess, @ReplyWriteAccess, @ReadAccess, @PostWriteAccessRoleID, @ReplyWriteAccessRoleID, @ReadAccessRoleID, @ModeratorRoleID, @MarkupLevel, @ShowSignatures, @AllowImagesInMessages, @AllowImagesInSignatures, @RequireModeration, @AllowVoting, @TopicDisplayOrder, @Locked)
	END
	ELSE
	BEGIN
		UPDATE Forum SET
			ForumCategoryID = @ForumCategoryID,
			ForumCode = @ForumCode,
			Name = @Name,
			Description = @Description,
			URLToken = @URLToken,
			DateCreated = @DateCreated,
			Rank = @Rank,
			PostWriteAccess = @PostWriteAccess,
			ReplyWriteAccess = @ReplyWriteAccess,
			ReadAccess = @ReadAccess,
			PostWriteAccessRoleID = @PostWriteAccessRoleID,
			ReplyWriteAccessRoleID = @ReplyWriteAccessRoleID,
			ReadAccessRoleID = @ReadAccessRoleID,
			ModeratorRoleID = @ModeratorRoleID,
			MarkupLevel = @MarkupLevel,
			ShowSignatures = @ShowSignatures,
			AllowImagesInMessages = @AllowImagesInMessages,
			AllowImagesInSignatures = @AllowImagesInSignatures,
			RequireModeration = @RequireModeration,
			AllowVoting = @AllowVoting,
			TopicDisplayOrder = @TopicDisplayOrder,
			Locked = @Locked
		WHERE ForumID = @ForumID
	END
END
go

IF OBJECT_ID(N'dbo.Forum_Select') IS NOT NULL
	DROP PROCEDURE Forum_Select
go
CREATE PROCEDURE dbo.Forum_Select
	@ForumID bigint
AS
BEGIN
	SELECT *
	  FROM Forum
	 WHERE ForumID = @ForumID
END

go

IF OBJECT_ID(N'dbo.Forum_Delete') IS NOT NULL
	DROP PROCEDURE Forum_Delete
go
CREATE PROCEDURE dbo.Forum_Delete
	@ForumID bigint
AS
BEGIN
	DELETE
	  FROM Forum
	 WHERE ForumID = @ForumID
END
go
IF OBJECT_ID(N'dbo.ForumCategory_Store') IS NOT NULL
	DROP PROCEDURE ForumCategory_Store
go
CREATE PROCEDURE dbo.ForumCategory_Store
	@ForumCategoryID bigint OUTPUT,
	@ClientSpaceID bigint,
	@CategoryCode nvarchar(50) = null,
	@Name nvarchar(250),
	@URLToken nvarchar(50) = null,
	@DateCreated datetime,
	@Rank int = null,
	@InternalUseOnly bit
AS
BEGIN
	IF NOT EXISTS (SELECT ForumCategoryID FROM ForumCategory WHERE ForumCategoryID = @ForumCategoryID)
	BEGIN
		IF @ForumCategoryID = 0 OR @ForumCategoryID IS NULL
			EXEC GetUniqueID @ForumCategoryID OUTPUT
		INSERT INTO ForumCategory
			(ForumCategoryID, ClientSpaceID, CategoryCode, Name, URLToken, DateCreated, Rank, InternalUseOnly)
		VALUES
			(@ForumCategoryID, @ClientSpaceID, @CategoryCode, @Name, @URLToken, @DateCreated, @Rank, @InternalUseOnly)
	END
	ELSE
	BEGIN
		UPDATE ForumCategory SET
			ClientSpaceID = @ClientSpaceID,
			CategoryCode = @CategoryCode,
			Name = @Name,
			URLToken = @URLToken,
			DateCreated = @DateCreated,
			Rank = @Rank,
			InternalUseOnly = @InternalUseOnly
		WHERE ForumCategoryID = @ForumCategoryID
	END
END
go

IF OBJECT_ID(N'dbo.ForumCategory_Select') IS NOT NULL
	DROP PROCEDURE ForumCategory_Select
go
CREATE PROCEDURE dbo.ForumCategory_Select
	@ForumCategoryID bigint
AS
BEGIN
	SELECT *
	  FROM ForumCategory
	 WHERE ForumCategoryID = @ForumCategoryID
END

go

IF OBJECT_ID(N'dbo.ForumCategory_Delete') IS NOT NULL
	DROP PROCEDURE ForumCategory_Delete
go
CREATE PROCEDURE dbo.ForumCategory_Delete
	@ForumCategoryID bigint
AS
BEGIN
	DELETE
	  FROM ForumCategory
	 WHERE ForumCategoryID = @ForumCategoryID
END
go
IF OBJECT_ID(N'dbo.ForumTopic_Store') IS NOT NULL
	DROP PROCEDURE ForumTopic_Store
go
CREATE PROCEDURE dbo.ForumTopic_Store
	@ForumTopicID bigint OUTPUT,
	@ForumID bigint,
	@AuthorUserID bigint = null,
	@AuthorName nvarchar(100) = null,
	@Subject nvarchar(500),
	@URLToken nvarchar(200) = null,
	@DateCreated datetime,
	@Sticky bit,
	@ModerationState smallint,
	@Locked bit
AS
BEGIN
	IF NOT EXISTS (SELECT ForumTopicID FROM ForumTopic WHERE ForumTopicID = @ForumTopicID)
	BEGIN
		IF @ForumTopicID = 0 OR @ForumTopicID IS NULL
			EXEC GetUniqueID @ForumTopicID OUTPUT
		INSERT INTO ForumTopic
			(ForumTopicID, ForumID, AuthorUserID, AuthorName, Subject, URLToken, DateCreated, Sticky, ModerationState, Locked)
		VALUES
			(@ForumTopicID, @ForumID, @AuthorUserID, @AuthorName, @Subject, @URLToken, @DateCreated, @Sticky, @ModerationState, @Locked)
	END
	ELSE
	BEGIN
		UPDATE ForumTopic SET
			ForumID = @ForumID,
			AuthorUserID = @AuthorUserID,
			AuthorName = @AuthorName,
			Subject = @Subject,
			URLToken = @URLToken,
			DateCreated = @DateCreated,
			Sticky = @Sticky,
			ModerationState = @ModerationState,
			Locked = @Locked
		WHERE ForumTopicID = @ForumTopicID
	END
END
go

IF OBJECT_ID(N'dbo.ForumTopic_Select') IS NOT NULL
	DROP PROCEDURE ForumTopic_Select
go
CREATE PROCEDURE dbo.ForumTopic_Select
	@ForumTopicID bigint
AS
BEGIN
	SELECT *
	  FROM ForumTopic
	 WHERE ForumTopicID = @ForumTopicID
END

go

IF OBJECT_ID(N'dbo.ForumTopic_Delete') IS NOT NULL
	DROP PROCEDURE ForumTopic_Delete
go
CREATE PROCEDURE dbo.ForumTopic_Delete
	@ForumTopicID bigint
AS
BEGIN
	DELETE
	  FROM ForumTopic
	 WHERE ForumTopicID = @ForumTopicID
END
go
IF OBJECT_ID(N'dbo.ForumTopicMessage_Store') IS NOT NULL
	DROP PROCEDURE ForumTopicMessage_Store
go
CREATE PROCEDURE dbo.ForumTopicMessage_Store
	@ForumTopicMessageID bigint OUTPUT,
	@ForumTopicID bigint,
	@AuthorUserID bigint = null,
	@AuthorName nvarchar(100) = null,
	@DateCreated datetime,
	@BodySource nvarchar(max),
	@BodyOutput nvarchar(max),
	@ModerationState smallint,
	@MarkupType smallint
AS
BEGIN
	IF NOT EXISTS (SELECT ForumTopicMessageID FROM ForumTopicMessage WHERE ForumTopicMessageID = @ForumTopicMessageID)
	BEGIN
		IF @ForumTopicMessageID = 0 OR @ForumTopicMessageID IS NULL
			EXEC GetUniqueID @ForumTopicMessageID OUTPUT
		INSERT INTO ForumTopicMessage
			(ForumTopicMessageID, ForumTopicID, AuthorUserID, AuthorName, DateCreated, BodySource, BodyOutput, ModerationState, MarkupType)
		VALUES
			(@ForumTopicMessageID, @ForumTopicID, @AuthorUserID, @AuthorName, @DateCreated, @BodySource, @BodyOutput, @ModerationState, @MarkupType)
	END
	ELSE
	BEGIN
		UPDATE ForumTopicMessage SET
			ForumTopicID = @ForumTopicID,
			AuthorUserID = @AuthorUserID,
			AuthorName = @AuthorName,
			DateCreated = @DateCreated,
			BodySource = @BodySource,
			BodyOutput = @BodyOutput,
			ModerationState = @ModerationState,
			MarkupType = @MarkupType
		WHERE ForumTopicMessageID = @ForumTopicMessageID
	END
END
go

IF OBJECT_ID(N'dbo.ForumTopicMessage_Select') IS NOT NULL
	DROP PROCEDURE ForumTopicMessage_Select
go
CREATE PROCEDURE dbo.ForumTopicMessage_Select
	@ForumTopicMessageID bigint
AS
BEGIN
	SELECT *
	  FROM ForumTopicMessage
	 WHERE ForumTopicMessageID = @ForumTopicMessageID
END

go

IF OBJECT_ID(N'dbo.ForumTopicMessage_Delete') IS NOT NULL
	DROP PROCEDURE ForumTopicMessage_Delete
go
CREATE PROCEDURE dbo.ForumTopicMessage_Delete
	@ForumTopicMessageID bigint
AS
BEGIN
	DELETE
	  FROM ForumTopicMessage
	 WHERE ForumTopicMessageID = @ForumTopicMessageID
END
go
