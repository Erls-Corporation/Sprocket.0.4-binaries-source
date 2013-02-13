IF OBJECT_ID(N'dbo.ContentBlock') IS NULL
CREATE TABLE dbo.WebNode
(
	WebNodeID bigint PRIMARY KEY,
	ClientSpaceID bigint FOREIGN KEY REFERENCES ClientSpaces(ClientSpaceID) ON DELETE CASCADE,
	ParentWebNodeID bigint NULL,
	IDCode nvarchar(200) NULL,
	TemplateCode nvarchar(200) NOT NULL,
	RequestPath nvarchar(500) NULL,
	ContentType nvarchar(200) NOT NULL,
	
)