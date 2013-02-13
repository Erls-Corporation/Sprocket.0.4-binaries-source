IF OBJECT_ID(N'dbo.ForumCategory') IS NULL
CREATE TABLE dbo.ForumCategory
(
	ForumCategoryID bigint PRIMARY KEY,
	ClientSpaceID bigint NOT NULL REFERENCES ClientSpaces(ClientSpaceID) ON DELETE CASCADE,
	CategoryCode nvarchar(50) NULL,

	Name nvarchar(250) NOT NULL,
	URLToken nvarchar(50) NULL, -- used for quick access from the address bar e.g. /mysite/[urltoken]/someforum/
	DateCreated datetime NOT NULL,
	Rank int NULL, -- for specifying which categories appear in which order

	InternalUseOnly bit NOT NULL -- allows for the use of categories that will never show up in a public forum list, e.g. for content and blog comments
)

IF OBJECT_ID(N'dbo.Forum') IS NULL
CREATE TABLE dbo.Forum
(
	ForumID bigint PRIMARY KEY,
	ForumCategoryID bigint NOT NULL FOREIGN KEY REFERENCES ForumCategory(ForumCategoryID) ON DELETE CASCADE,
	
	Name nvarchar(250) NOT NULL,
	URLToken nvarchar(50) NULL, -- used for quick access from the address bar e.g. /mysite/category/[urltoken]/
	DateCreated datetime NOT NULL,
	Rank int NULL, -- for specifying which forums appear in which order
	
	WriteAccess smallint NOT NULL, -- 0: admin only, 1: specified members only, 2: general members only, 3: anonymous posting
	ReadAccess smallint NOT NULL, -- 0: admin only, 1: only users who can post, 2: everybody
	MarkupLevel smallint NOT NULL, -- 0: none, 1: bbcode, 2: limited HTML, 3: full HTML (moderators/admin always have access to full HTML)
	ShowSignatures bit NULL,
	AllowImagesInMessages bit NOT NULL,
	AllowImagesInSignatures bit NOT NULL,
	RequireModeration bit NOT NULL, -- posts must be approved before they are visible
	AllowVoting bit NOT NULL, -- allows a boolean good/bad vote to be applied to topics in this forum
	TopicDisplayOrder smallint NOT NULL, -- 0: topic date, 1: topic last message date, 2: vote weight

	Locked bit NOT NULL -- prevents new posts
)

IF OBJECT_ID(N'dbo.ForumTopic') IS NULL
CREATE TABLE dbo.ForumTopic
(
	ForumTopicID bigint PRIMARY KEY,
	ForumID bigint NOT NULL FOREIGN KEY REFERENCES Forum(ForumID) ON DELETE CASCADE,
	AuthorUserID bigint NULL FOREIGN KEY REFERENCES Users(UserID) ON DELETE NO ACTION,

	Subject nvarchar(500) NOT NULL,
	DateCreated datetime NOT NULL,
	Sticky bit NOT NULL,
	ModerationState smallint NOT NULL, -- 0: unreviewed, 1: approved, 2: flagged for review
	Locked bit NOT NULL
)

IF OBJECT_ID(N'dbo.ForumTopicMessage') IS NULL
CREATE TABLE dbo.ForumTopicMessage
(
	ForumTopicMessageID bigint PRIMARY KEY,
	ForumTopicID bigint NOT NULL FOREIGN KEY REFERENCES ForumTopic(ForumTopicID) ON DELETE CASCADE,
	AuthorUserID bigint NOT NULL FOREIGN KEY REFERENCES Users(UserID) ON DELETE NO ACTION,
	
	DateCreated datetime NOT NULL,
	Body nvarchar(max) NOT NULL,
	MarkupType smallint NOT NULL -- 0: none, 1: bbcode, 2: limited HTML, 3: full HTML
)
	
go
CREATE INDEX IX_ForumTopic ON ForumTopic(DateCreated) WITH DROP_EXISTING
CREATE INDEX IX_ForumTopicMessage ON ForumTopicMessage(DateCreated) WITH DROP_EXISTING
go

IF OBJECT_ID(N'dbo.OnDeleteForumAuthor') IS NOT NULL
	DROP TRIGGER dbo.OnDeleteForumAuthor
go
CREATE TRIGGER dbo.OnDeleteForumAuthor ON dbo.Users
FOR DELETE
AS
BEGIN
	IF (SELECT COUNT(*) FROM deleted) > 0
	BEGIN
		DELETE FROM ForumTopic
		WHERE AuthorUserID IN (SELECT UserID FROM deleted)
		DELETE FROM ForumTopicMessage
		WHERE AuthorUserID IN (SELECT UserID FROM deleted)
	END
END
go
