using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

using Sprocket;
using Sprocket.Data;
using Sprocket.Web;
using Sprocket.Web.CMS.Script;


namespace Sprocket.Web.Forums
{
	public class ForumCategory : IJSONEncoder, IJSONReader
	{
		#region Constructor, Fields, Properties, JSON Methods
		#region Fields

		protected long forumCategoryID = 0;
		protected long clientSpaceID = 0;
		protected string categoryCode = null;
		protected string name = "";
		protected string uRLToken = null;
		protected DateTime dateCreated = DateTime.MinValue;
		protected int? rank = null;
		protected bool internalUseOnly = false;

		#endregion

		#region Properties

		///<summary>
		///Gets or sets the value for ForumCategoryID
		///</summary>
		public long ForumCategoryID
		{
			get { return forumCategoryID; }
			set { forumCategoryID = value; }
		}

		///<summary>
		///Gets or sets the value for ClientSpaceID
		///</summary>
		public long ClientSpaceID
		{
			get { return clientSpaceID; }
			set { clientSpaceID = value; }
		}

		///<summary>
		///Gets or sets the value for CategoryCode
		///</summary>
		public string CategoryCode
		{
			get { return categoryCode; }
			set { categoryCode = value; }
		}

		///<summary>
		///Gets or sets the value for Name
		///</summary>
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		///<summary>
		///Gets or sets the value for URLToken
		///</summary>
		public string URLToken
		{
			get { return uRLToken; }
			set { uRLToken = value; }
		}

		///<summary>
		///Gets or sets the value for DateCreated
		///</summary>
		public DateTime DateCreated
		{
			get { return dateCreated; }
			set { dateCreated = value; }
		}

		///<summary>
		///Gets or sets the value for Rank
		///</summary>
		public int? Rank
		{
			get { return rank; }
			set { rank = value; }
		}

		///<summary>
		///Gets or sets the value for InternalUseOnly
		///</summary>
		public bool InternalUseOnly
		{
			get { return internalUseOnly; }
			set { internalUseOnly = value; }
		}

		#endregion

		#region Constructors

		public ForumCategory()
		{
		}

		public ForumCategory(long forumCategoryID, long clientSpaceID, string categoryCode, string name, string uRLToken, DateTime dateCreated, int? rank, bool internalUseOnly)
		{
			this.forumCategoryID = forumCategoryID;
			this.clientSpaceID = clientSpaceID;
			this.categoryCode = categoryCode;
			this.name = name;
			this.uRLToken = uRLToken;
			this.dateCreated = dateCreated;
			this.rank = rank;
			this.internalUseOnly = internalUseOnly;
		}

		public ForumCategory(IDataReader reader)
		{
			if (reader["ForumCategoryID"] != DBNull.Value) forumCategoryID = (long)reader["ForumCategoryID"];
			if (reader["ClientSpaceID"] != DBNull.Value) clientSpaceID = (long)reader["ClientSpaceID"];
			if (reader["CategoryCode"] != DBNull.Value) categoryCode = (string)reader["CategoryCode"];
			if (reader["Name"] != DBNull.Value) name = (string)reader["Name"];
			if (reader["URLToken"] != DBNull.Value) uRLToken = (string)reader["URLToken"];
			if (reader["DateCreated"] != DBNull.Value) dateCreated = (DateTime)reader["DateCreated"];
			if (reader["Rank"] != DBNull.Value) rank = (int?)reader["Rank"];
			if (reader["InternalUseOnly"] != DBNull.Value) internalUseOnly = (bool)reader["InternalUseOnly"];
		}

		#endregion

		#region Clone
		public ForumCategory Clone()
		{
			ForumCategory copy = new ForumCategory();
			copy.forumCategoryID = forumCategoryID;
			copy.clientSpaceID = clientSpaceID;
			copy.categoryCode = categoryCode;
			copy.name = name;
			copy.uRLToken = uRLToken;
			copy.dateCreated = dateCreated;
			copy.rank = rank;
			copy.internalUseOnly = internalUseOnly;
			return copy;
		}
		#endregion

		#region JSON Methods

		/// <summary>
		/// Writes this entity out as a JSON formatted string
		/// </summary>
		public void WriteJSON(StringWriter writer)
		{
			writer.Write("{");
			JSON.EncodeNameValuePair(writer, "ForumCategoryID", forumCategoryID);
			writer.Write(",");
			JSON.EncodeNameValuePair(writer, "ClientSpaceID", clientSpaceID);
			writer.Write(",");
			JSON.EncodeNameValuePair(writer, "CategoryCode", categoryCode);
			writer.Write(",");
			JSON.EncodeNameValuePair(writer, "Name", name);
			writer.Write(",");
			JSON.EncodeNameValuePair(writer, "URLToken", uRLToken);
			writer.Write(",");
			JSON.EncodeNameValuePair(writer, "DateCreated", dateCreated);
			writer.Write(",");
			JSON.EncodeNameValuePair(writer, "Rank", rank);
			writer.Write(",");
			JSON.EncodeNameValuePair(writer, "InternalUseOnly", internalUseOnly);
			writer.Write("}");
		}

		public void LoadJSON(object json)
		{
			Dictionary<string, object> values = json as Dictionary<string, object>;
			if (values == null) return;
			forumCategoryID = (long)values["ForumCategoryID"];
			clientSpaceID = (long)values["ClientSpaceID"];
			categoryCode = (string)values["CategoryCode"];
			name = (string)values["Name"];
			uRLToken = (string)values["URLToken"];
			dateCreated = (DateTime)values["DateCreated"];
			rank = (int?)values["Rank"];
			internalUseOnly = (bool)values["InternalUseOnly"];
		}

		#endregion
		#endregion
	}

	public class Forum : IJSONEncoder, IJSONReader, IPropertyEvaluatorExpression
	{
		#region Constructor, Fields, Properties, JSON Methods
		#region Fields

		protected long forumID = 0;
		protected long forumCategoryID = 0;
		protected string forumCode = null;
		protected string name = "";
		protected string description = "";
		protected string uRLToken = null;
		protected DateTime dateCreated = DateTime.MinValue;
		protected int? rank = null;
		protected short postWriteAccess = 0;
		protected short replyWriteAccess = 0;
		protected short readAccess = 0;
		protected long? postWriteAccessRoleID = null;
		protected long? replyWriteAccessRoleID = null;
		protected long? readAccessRoleID = null;
		protected long? moderatorRoleID = null;
		protected short markupLevel = 0;
		protected bool? showSignatures = null;
		protected bool allowImagesInMessages = false;
		protected bool allowImagesInSignatures = false;
		protected bool requireModeration = false;
		protected bool allowVoting = false;
		protected short topicDisplayOrder = 0;
		protected bool locked = false;

		#endregion

		#region Properties

		///<summary>
		///Gets or sets the value for ForumID
		///</summary>
		public long ForumID
		{
			get { return forumID; }
			set { forumID = value; }
		}

		///<summary>
		///Gets or sets the value for ForumCategoryID
		///</summary>
		public long ForumCategoryID
		{
			get { return forumCategoryID; }
			set { forumCategoryID = value; }
		}

		///<summary>
		///Gets or sets the value for ForumCode
		///</summary>
		public string ForumCode
		{
			get { return forumCode; }
			set { forumCode = value; }
		}

		///<summary>
		///Gets or sets the value for Name
		///</summary>
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		///<summary>
		///Gets or sets the value for Description
		///</summary>
		public string Description
		{
			get { return description; }
			set { description = value; }
		}

		///<summary>
		///Gets or sets the value for URLToken
		///</summary>
		public string URLToken
		{
			get { return uRLToken; }
			set { uRLToken = value; }
		}

		///<summary>
		///Gets or sets the value for DateCreated
		///</summary>
		public DateTime DateCreated
		{
			get { return dateCreated; }
			set { dateCreated = value; }
		}

		///<summary>
		///Gets or sets the value for Rank
		///</summary>
		public int? Rank
		{
			get { return rank; }
			set { rank = value; }
		}

		///<summary>
		///Gets or sets the value for PostWriteAccess
		///</summary>
		public short PostWriteAccess
		{
			get { return postWriteAccess; }
			set { postWriteAccess = value; }
		}

		///<summary>
		///Gets or sets the value for ReplyWriteAccess
		///</summary>
		public short ReplyWriteAccess
		{
			get { return replyWriteAccess; }
			set { replyWriteAccess = value; }
		}

		///<summary>
		///Gets or sets the value for ReadAccess
		///</summary>
		public short ReadAccess
		{
			get { return readAccess; }
			set { readAccess = value; }
		}

		///<summary>
		///Gets or sets the value for PostWriteAccessRoleID
		///</summary>
		public long? PostWriteAccessRoleID
		{
			get { return postWriteAccessRoleID; }
			set { postWriteAccessRoleID = value; }
		}

		///<summary>
		///Gets or sets the value for ReplyWriteAccessRoleID
		///</summary>
		public long? ReplyWriteAccessRoleID
		{
			get { return replyWriteAccessRoleID; }
			set { replyWriteAccessRoleID = value; }
		}

		///<summary>
		///Gets or sets the value for ReadAccessRoleID
		///</summary>
		public long? ReadAccessRoleID
		{
			get { return readAccessRoleID; }
			set { readAccessRoleID = value; }
		}

		///<summary>
		///Gets or sets the value for ModeratorRoleID
		///</summary>
		public long? ModeratorRoleID
		{
			get { return moderatorRoleID; }
			set { moderatorRoleID = value; }
		}

		///<summary>
		///Gets or sets the value for MarkupLevel
		///</summary>
		public short MarkupLevel
		{
			get { return markupLevel; }
			set { markupLevel = value; }
		}

		///<summary>
		///Gets or sets the value for ShowSignatures
		///</summary>
		public bool? ShowSignatures
		{
			get { return showSignatures; }
			set { showSignatures = value; }
		}

		///<summary>
		///Gets or sets the value for AllowImagesInMessages
		///</summary>
		public bool AllowImagesInMessages
		{
			get { return allowImagesInMessages; }
			set { allowImagesInMessages = value; }
		}

		///<summary>
		///Gets or sets the value for AllowImagesInSignatures
		///</summary>
		public bool AllowImagesInSignatures
		{
			get { return allowImagesInSignatures; }
			set { allowImagesInSignatures = value; }
		}

		///<summary>
		///Gets or sets the value for RequireModeration
		///</summary>
		public bool RequireModeration
		{
			get { return requireModeration; }
			set { requireModeration = value; }
		}

		///<summary>
		///Gets or sets the value for AllowVoting
		///</summary>
		public bool AllowVoting
		{
			get { return allowVoting; }
			set { allowVoting = value; }
		}

		///<summary>
		///Gets or sets the value for TopicDisplayOrder
		///</summary>
		public short TopicDisplayOrder
		{
			get { return topicDisplayOrder; }
			set { topicDisplayOrder = value; }
		}

		///<summary>
		///Gets or sets the value for Locked
		///</summary>
		public bool Locked
		{
			get { return locked; }
			set { locked = value; }
		}

		#endregion

		#region Constructors

		public Forum()
		{
		}

		public Forum(long forumID, long forumCategoryID, string forumCode, string name, string description,
			string uRLToken, DateTime dateCreated, int? rank, short postWriteAccess, short replyWriteAccess,
			short readAccess, long? postWriteAccessRoleID, long? replyWriteAccessRoleID, long? readAccessRoleID,
			long? moderatorRoleID, short markupLevel, bool? showSignatures, bool allowImagesInMessages,
			bool allowImagesInSignatures, bool requireModeration, bool allowVoting, short topicDisplayOrder, bool locked)
		{
			this.forumID = forumID;
			this.forumCategoryID = forumCategoryID;
			this.forumCode = forumCode;
			this.name = name;
			this.description = description;
			this.uRLToken = uRLToken;
			this.dateCreated = dateCreated;
			this.rank = rank;
			this.postWriteAccess = postWriteAccess;
			this.replyWriteAccess = replyWriteAccess;
			this.readAccess = readAccess;
			this.postWriteAccessRoleID = postWriteAccessRoleID;
			this.replyWriteAccessRoleID = replyWriteAccessRoleID;
			this.readAccessRoleID = readAccessRoleID;
			this.moderatorRoleID = moderatorRoleID;
			this.markupLevel = markupLevel;
			this.showSignatures = showSignatures;
			this.allowImagesInMessages = allowImagesInMessages;
			this.allowImagesInSignatures = allowImagesInSignatures;
			this.requireModeration = requireModeration;
			this.allowVoting = allowVoting;
			this.topicDisplayOrder = topicDisplayOrder;
			this.locked = locked;
		}

		public Forum(IDataReader reader)
		{
			if (reader["ForumID"] != DBNull.Value) forumID = (long)reader["ForumID"];
			if (reader["ForumCategoryID"] != DBNull.Value) forumCategoryID = (long)reader["ForumCategoryID"];
			if (reader["ForumCode"] != DBNull.Value) forumCode = (string)reader["ForumCode"];
			if (reader["Name"] != DBNull.Value) name = (string)reader["Name"];
			if (reader["Description"] != DBNull.Value) description = (string)reader["Description"];
			if (reader["URLToken"] != DBNull.Value) uRLToken = (string)reader["URLToken"];
			if (reader["DateCreated"] != DBNull.Value) dateCreated = (DateTime)reader["DateCreated"];
			if (reader["Rank"] != DBNull.Value) rank = (int?)reader["Rank"];
			if (reader["PostWriteAccess"] != DBNull.Value) postWriteAccess = (short)reader["PostWriteAccess"];
			if (reader["ReplyWriteAccess"] != DBNull.Value) replyWriteAccess = (short)reader["ReplyWriteAccess"];
			if (reader["ReadAccess"] != DBNull.Value) readAccess = (short)reader["ReadAccess"];
			if (reader["PostWriteAccessRoleID"] != DBNull.Value) postWriteAccessRoleID = (long?)reader["PostWriteAccessRoleID"];
			if (reader["ReplyWriteAccessRoleID"] != DBNull.Value) replyWriteAccessRoleID = (long?)reader["ReplyWriteAccessRoleID"];
			if (reader["ReadAccessRoleID"] != DBNull.Value) readAccessRoleID = (long?)reader["ReadAccessRoleID"];
			if (reader["ModeratorRoleID"] != DBNull.Value) moderatorRoleID = (long?)reader["ModeratorRoleID"];
			if (reader["MarkupLevel"] != DBNull.Value) markupLevel = (short)reader["MarkupLevel"];
			if (reader["ShowSignatures"] != DBNull.Value) showSignatures = (bool?)reader["ShowSignatures"];
			if (reader["AllowImagesInMessages"] != DBNull.Value) allowImagesInMessages = (bool)reader["AllowImagesInMessages"];
			if (reader["AllowImagesInSignatures"] != DBNull.Value) allowImagesInSignatures = (bool)reader["AllowImagesInSignatures"];
			if (reader["RequireModeration"] != DBNull.Value) requireModeration = (bool)reader["RequireModeration"];
			if (reader["AllowVoting"] != DBNull.Value) allowVoting = (bool)reader["AllowVoting"];
			if (reader["TopicDisplayOrder"] != DBNull.Value) topicDisplayOrder = (short)reader["TopicDisplayOrder"];
			if (reader["Locked"] != DBNull.Value) locked = (bool)reader["Locked"];
		}

		#endregion

		#region Clone
		public Forum Clone()
		{
			Forum copy = new Forum();
			copy.forumID = forumID;
			copy.forumCategoryID = forumCategoryID;
			copy.forumCode = forumCode;
			copy.name = name;
			copy.description = description;
			copy.uRLToken = uRLToken;
			copy.dateCreated = dateCreated;
			copy.rank = rank;
			copy.postWriteAccess = postWriteAccess;
			copy.replyWriteAccess = replyWriteAccess;
			copy.readAccess = readAccess;
			copy.postWriteAccessRoleID = postWriteAccessRoleID;
			copy.replyWriteAccessRoleID = replyWriteAccessRoleID;
			copy.readAccessRoleID = readAccessRoleID;
			copy.moderatorRoleID = moderatorRoleID;
			copy.markupLevel = markupLevel;
			copy.showSignatures = showSignatures;
			copy.allowImagesInMessages = allowImagesInMessages;
			copy.allowImagesInSignatures = allowImagesInSignatures;
			copy.requireModeration = requireModeration;
			copy.allowVoting = allowVoting;
			copy.topicDisplayOrder = topicDisplayOrder;
			copy.locked = locked;
			return copy;
		}
		#endregion

		#region JSON Methods

		/// <summary>
		/// Writes this entity out as a JSON formatted string
		/// </summary>
		public void WriteJSON(StringWriter writer)
		{
			writer.Write("{");
			JSON.EncodeNameValuePair(writer, "ForumID", forumID);
			writer.Write(",");
			JSON.EncodeNameValuePair(writer, "ForumCategoryID", forumCategoryID);
			writer.Write(",");
			JSON.EncodeNameValuePair(writer, "ForumCode", forumCode);
			writer.Write(",");
			JSON.EncodeNameValuePair(writer, "Name", name);
			writer.Write(",");
			JSON.EncodeNameValuePair(writer, "Description", description);
			writer.Write(",");
			JSON.EncodeNameValuePair(writer, "URLToken", uRLToken);
			writer.Write(",");
			JSON.EncodeNameValuePair(writer, "DateCreated", dateCreated);
			writer.Write(",");
			JSON.EncodeNameValuePair(writer, "Rank", rank);
			writer.Write(",");
			JSON.EncodeNameValuePair(writer, "PostWriteAccess", postWriteAccess);
			writer.Write(",");
			JSON.EncodeNameValuePair(writer, "ReplyWriteAccess", replyWriteAccess);
			writer.Write(",");
			JSON.EncodeNameValuePair(writer, "ReadAccess", readAccess);
			writer.Write(",");
			JSON.EncodeNameValuePair(writer, "PostWriteAccessRoleID", postWriteAccessRoleID);
			writer.Write(",");
			JSON.EncodeNameValuePair(writer, "ReplyWriteAccessRoleID", replyWriteAccessRoleID);
			writer.Write(",");
			JSON.EncodeNameValuePair(writer, "ReadAccessRoleID", readAccessRoleID);
			writer.Write(",");
			JSON.EncodeNameValuePair(writer, "ModeratorRoleID", moderatorRoleID);
			writer.Write(",");
			JSON.EncodeNameValuePair(writer, "MarkupLevel", markupLevel);
			writer.Write(",");
			JSON.EncodeNameValuePair(writer, "ShowSignatures", showSignatures);
			writer.Write(",");
			JSON.EncodeNameValuePair(writer, "AllowImagesInMessages", allowImagesInMessages);
			writer.Write(",");
			JSON.EncodeNameValuePair(writer, "AllowImagesInSignatures", allowImagesInSignatures);
			writer.Write(",");
			JSON.EncodeNameValuePair(writer, "RequireModeration", requireModeration);
			writer.Write(",");
			JSON.EncodeNameValuePair(writer, "AllowVoting", allowVoting);
			writer.Write(",");
			JSON.EncodeNameValuePair(writer, "TopicDisplayOrder", topicDisplayOrder);
			writer.Write(",");
			JSON.EncodeNameValuePair(writer, "Locked", locked);
			writer.Write("}");
		}

		public void LoadJSON(object json)
		{
			Dictionary<string, object> values = json as Dictionary<string, object>;
			if (values == null) return;
			forumID = (long)values["ForumID"];
			forumCategoryID = (long)values["ForumCategoryID"];
			forumCode = (string)values["ForumCode"];
			name = (string)values["Name"];
			description = (string)values["Description"];
			uRLToken = (string)values["URLToken"];
			dateCreated = (DateTime)values["DateCreated"];
			rank = (int?)values["Rank"];
			postWriteAccess = (short)values["PostWriteAccess"];
			replyWriteAccess = (short)values["ReplyWriteAccess"];
			readAccess = (short)values["ReadAccess"];
			postWriteAccessRoleID = (long?)values["PostWriteAccessRoleID"];
			replyWriteAccessRoleID = (long?)values["ReplyWriteAccessRoleID"];
			readAccessRoleID = (long?)values["ReadAccessRoleID"];
			moderatorRoleID = (long?)values["ModeratorRoleID"];
			markupLevel = (short)values["MarkupLevel"];
			showSignatures = (bool?)values["ShowSignatures"];
			allowImagesInMessages = (bool)values["AllowImagesInMessages"];
			allowImagesInSignatures = (bool)values["AllowImagesInSignatures"];
			requireModeration = (bool)values["RequireModeration"];
			allowVoting = (bool)values["AllowVoting"];
			topicDisplayOrder = (short)values["TopicDisplayOrder"];
			locked = (bool)values["Locked"];
		}

		#endregion
		#endregion

		#region Custom
		public Forum(long forumID, long forumCategoryID, string forumCode, string name, string description, string uRLToken, DateTime dateCreated, int? rank, AccessType postWriteAccess, AccessType replyWriteAccess, AccessType readAccess, long? postWriteAccessRoleID, long? replyWriteAccessRoleID, long? readAccessRoleID, long? moderatorRoleID, MarkupType markupLevel, bool? showSignatures, bool allowImagesInMessages, bool allowImagesInSignatures, bool requireModeration, bool allowVoting, DisplayOrderType topicDisplayOrder, bool locked)
		{
			this.forumID = forumID;
			this.forumCategoryID = forumCategoryID;
			this.forumCode = forumCode;
			this.name = name;
			this.description = description;
			this.uRLToken = uRLToken;
			this.dateCreated = dateCreated;
			this.rank = rank;
			this.postWriteAccess = (short)postWriteAccess;
			this.replyWriteAccess = (short)replyWriteAccess;
			this.readAccess = (short)readAccess;
			this.postWriteAccessRoleID = postWriteAccessRoleID;
			this.replyWriteAccessRoleID = replyWriteAccessRoleID;
			this.readAccessRoleID = readAccessRoleID;
			this.moderatorRoleID = moderatorRoleID;
			this.markupLevel = (short)markupLevel;
			this.showSignatures = showSignatures;
			this.allowImagesInMessages = allowImagesInMessages;
			this.allowImagesInSignatures = allowImagesInSignatures;
			this.requireModeration = requireModeration;
			this.allowVoting = allowVoting;
			this.topicDisplayOrder = (short)topicDisplayOrder;
			this.locked = locked;
		}

		public enum AccessType
		{
			Administrators = 0,
			AllMembers = 1,
			ActivatedMembers = 2,
			RoleMembers = 3,
			AllowAnonymous = 4
		}

		public enum MarkupType
		{
			None = 0,
			BBCode = 1,
			Textile = 2,
			LimitedHTML = 3,
			ExtendedHTML = 4
		}

		public enum DisplayOrderType
		{
			TopicDate = 0,
			TopicLastMessageDate = 1,
			VoteWeight = 2
		}

		public MarkupType Markup
		{
			get { return (MarkupType)markupLevel; }
			set { markupLevel = (short)value; }
		}

		public DisplayOrderType DisplayOrder
		{
			get { return (DisplayOrderType)topicDisplayOrder; }
			set { topicDisplayOrder = (short)value; }
		}

		public AccessType Read
		{
			get { return (AccessType)readAccess; }
			set { readAccess = (short)value; }
		}

		public AccessType PostNewTopics
		{
			get { return (AccessType)postWriteAccess; }
			set { postWriteAccess = (short)value; }
		}

		public AccessType WriteReplies
		{
			get { return (AccessType)replyWriteAccess; }
			set { replyWriteAccess = (short)value; }
		}
		#endregion

		#region SprocketScript Extensions
		public bool IsValidPropertyName(string propertyName)
		{
			switch (propertyName)
			{
				case "forumid":
				case "forumcategoryid":
				case "forumcode":
				case "name":
				case "description":
				case "urltoken":
				case "datecreated":
				case "rank":
				case "postwriteaccess":
				case "replywriteaccess":
				case "readaccess":
				case "markuplevel":
				case "showsignatures":
				case "allowimagesinmessages":
				case "allowimagesinsignatures":
				case "requiremoderation":
				case "allowvoting":
				case "topicdisplayorder":
				case "postwriteaccessroleid":
				case "replywriteaccessroleid":
				case "readaccessroleid":
				case "moderatorroleid":
				case "locked":
					return true;
				default:
					return false;
			}
		}

		public object EvaluateProperty(string propertyName, Token token, ExecutionState state)
		{
			switch (propertyName)
			{
				case "forumid": return ForumID;
				case "forumcategoryid": return ForumCategoryID;
				case "forumcode": return ForumCode;
				case "name": return Name;
				case "description": return Description;
				case "urltoken": return URLToken;
				case "datecreated": return DateCreated;
				case "rank": return Rank;
				case "postwriteaccess": return PostWriteAccess;
				case "replywriteaccess": return ReplyWriteAccess;
				case "readaccess": return ReadAccess;
				case "markuplevel": return MarkupLevel;
				case "showsignatures": return ShowSignatures;
				case "allowimagesinmessages": return AllowImagesInMessages;
				case "allowimagesinsignatures": return AllowImagesInSignatures;
				case "requiremoderation": return RequireModeration;
				case "allowvoting": return AllowVoting;
				case "topicdisplayorder": return TopicDisplayOrder;
				case "locked": return Locked;
				case "postwriteaccessroleid": return PostWriteAccessRoleID;
				case "replywriteaccessroleid": return ReplyWriteAccessRoleID;
				case "readaccessroleid": return ReadAccessRoleID;
				case "moderatorroleid": return ModeratorRoleID;
				default:
					throw new InstructionExecutionException("\"" + propertyName + "\" is not a property of the forum object.", token);
			}
		}

		public object Evaluate(ExecutionState state, Token contextToken)
		{
			return this;
		}

		public override string ToString()
		{
			return "[Forum: " + name + "]";
		}
		#endregion
	}

	public class ForumTopic : IJSONEncoder, IJSONReader
	{
		#region Constructor, Fields, Properties, JSON Methods
		#region Fields

		protected long forumTopicID = 0;
		protected long forumID = 0;
		protected long? authorUserID = null;
		protected string authorName = null;
		protected string subject = "";
		protected DateTime dateCreated = DateTime.MinValue;
		protected bool sticky = false;
		protected short moderationState = 0;
		protected bool locked = false;
		protected string uRLToken = null;

		#endregion

		#region Properties

		///<summary>
		///Gets or sets the value for ForumTopicID
		///</summary>
		public long ForumTopicID
		{
			get { return forumTopicID; }
			set { forumTopicID = value; }
		}

		///<summary>
		///Gets or sets the value for ForumID
		///</summary>
		public long ForumID
		{
			get { return forumID; }
			set { forumID = value; }
		}

		///<summary>
		///Gets or sets the value for AuthorUserID
		///</summary>
		public long? AuthorUserID
		{
			get { return authorUserID; }
			set { authorUserID = value; }
		}

		///<summary>
		///Gets or sets the value for AuthorName
		///</summary>
		public string AuthorName
		{
			get { return authorName; }
			set { authorName = value; }
		}

		///<summary>
		///Gets or sets the value for Subject
		///</summary>
		public string Subject
		{
			get { return subject; }
			set { subject = value; }
		}

		///<summary>
		///Gets or sets the value for DateCreated
		///</summary>
		public DateTime DateCreated
		{
			get { return dateCreated; }
			set { dateCreated = value; }
		}

		///<summary>
		///Gets or sets the value for Sticky
		///</summary>
		public bool Sticky
		{
			get { return sticky; }
			set { sticky = value; }
		}

		///<summary>
		///Gets or sets the value for ModerationState
		///</summary>
		public short ModerationState
		{
			get { return moderationState; }
			set { moderationState = value; }
		}

		///<summary>
		///Gets or sets the value for Locked
		///</summary>
		public bool Locked
		{
			get { return locked; }
			set { locked = value; }
		}

		///<summary>
		///Gets or sets the value for URLToken
		///</summary>
		public string URLToken
		{
			get { return uRLToken; }
			set { uRLToken = value; }
		}

		#endregion

		#region Constructors

		public ForumTopic()
		{
		}

		public ForumTopic(long forumTopicID, long forumID, long? authorUserID, string authorName, string subject, DateTime dateCreated, bool sticky, short moderationState, bool locked, string uRLToken)
		{
			this.forumTopicID = forumTopicID;
			this.forumID = forumID;
			this.authorUserID = authorUserID;
			this.authorName = authorName;
			this.subject = subject;
			this.dateCreated = dateCreated;
			this.sticky = sticky;
			this.moderationState = moderationState;
			this.locked = locked;
			this.uRLToken = uRLToken;
		}

		public ForumTopic(IDataReader reader)
		{
			if (reader["ForumTopicID"] != DBNull.Value) forumTopicID = (long)reader["ForumTopicID"];
			if (reader["ForumID"] != DBNull.Value) forumID = (long)reader["ForumID"];
			if (reader["AuthorUserID"] != DBNull.Value) authorUserID = (long?)reader["AuthorUserID"];
			if (reader["AuthorName"] != DBNull.Value) authorName = (string)reader["AuthorName"];
			if (reader["Subject"] != DBNull.Value) subject = (string)reader["Subject"];
			if (reader["DateCreated"] != DBNull.Value) dateCreated = (DateTime)reader["DateCreated"];
			if (reader["Sticky"] != DBNull.Value) sticky = (bool)reader["Sticky"];
			if (reader["ModerationState"] != DBNull.Value) moderationState = (short)reader["ModerationState"];
			if (reader["Locked"] != DBNull.Value) locked = (bool)reader["Locked"];
			if (reader["URLToken"] != DBNull.Value) uRLToken = (string)reader["URLToken"];
		}

		#endregion

		#region Clone
		public ForumTopic Clone()
		{
			ForumTopic copy = new ForumTopic();
			copy.forumTopicID = forumTopicID;
			copy.forumID = forumID;
			copy.authorUserID = authorUserID;
			copy.authorName = authorName;
			copy.subject = subject;
			copy.dateCreated = dateCreated;
			copy.sticky = sticky;
			copy.moderationState = moderationState;
			copy.locked = locked;
			copy.uRLToken = uRLToken;
			return copy;
		}
		#endregion

		#region JSON Methods

		/// <summary>
		/// Writes this entity out as a JSON formatted string
		/// </summary>
		public void WriteJSON(StringWriter writer)
		{
			writer.Write("{");
			JSON.EncodeNameValuePair(writer, "ForumTopicID", forumTopicID);
			writer.Write(",");
			JSON.EncodeNameValuePair(writer, "ForumID", forumID);
			writer.Write(",");
			JSON.EncodeNameValuePair(writer, "AuthorUserID", authorUserID);
			writer.Write(",");
			JSON.EncodeNameValuePair(writer, "AuthorName", authorName);
			writer.Write(",");
			JSON.EncodeNameValuePair(writer, "Subject", subject);
			writer.Write(",");
			JSON.EncodeNameValuePair(writer, "DateCreated", dateCreated);
			writer.Write(",");
			JSON.EncodeNameValuePair(writer, "Sticky", sticky);
			writer.Write(",");
			JSON.EncodeNameValuePair(writer, "ModerationState", moderationState);
			writer.Write(",");
			JSON.EncodeNameValuePair(writer, "Locked", locked);
			writer.Write(",");
			JSON.EncodeNameValuePair(writer, "URLToken", uRLToken);
			writer.Write("}");
		}

		public void LoadJSON(object json)
		{
			Dictionary<string, object> values = json as Dictionary<string, object>;
			if (values == null) return;
			forumTopicID = (long)values["ForumTopicID"];
			forumID = (long)values["ForumID"];
			authorUserID = (long?)values["AuthorUserID"];
			authorName = (string)values["AuthorName"];
			subject = (string)values["Subject"];
			dateCreated = (DateTime)values["DateCreated"];
			sticky = (bool)values["Sticky"];
			moderationState = (short)values["ModerationState"];
			locked = (bool)values["Locked"];
			uRLToken = (string)values["URLToken"];
		}

		#endregion
		#endregion

		#region Custom

		public ForumModerationState Moderation
		{
			get { return (ForumModerationState)moderationState; }
			set { moderationState = (short)value; }
		}

		#endregion
	}

	public class ForumTopicMessage : IJSONEncoder, IJSONReader
	{
		#region Constructor, Fields, Properties, JSON Methods
		#region Fields

		protected long forumTopicMessageID = 0;
		protected long forumTopicID = 0;
		protected long? authorUserID = null;
		protected string authorName = null;
		protected DateTime dateCreated = DateTime.MinValue;
		protected string bodySource = "";
		protected string bodyOutput = "";
		protected short moderationState = 0;
		protected short markupType = 0;

		#endregion

		#region Properties

		///<summary>
		///Gets or sets the value for ForumTopicMessageID
		///</summary>
		public long ForumTopicMessageID
		{
			get { return forumTopicMessageID; }
			set { forumTopicMessageID = value; }
		}

		///<summary>
		///Gets or sets the value for ForumTopicID
		///</summary>
		public long ForumTopicID
		{
			get { return forumTopicID; }
			set { forumTopicID = value; }
		}

		///<summary>
		///Gets or sets the value for AuthorUserID
		///</summary>
		public long? AuthorUserID
		{
			get { return authorUserID; }
			set { authorUserID = value; }
		}

		///<summary>
		///Gets or sets the value for AuthorName
		///</summary>
		public string AuthorName
		{
			get { return authorName; }
			set { authorName = value; }
		}

		///<summary>
		///Gets or sets the value for DateCreated
		///</summary>
		public DateTime DateCreated
		{
			get { return dateCreated; }
			set { dateCreated = value; }
		}

		///<summary>
		///Gets or sets the value for BodySource
		///</summary>
		public string BodySource
		{
			get { return bodySource; }
			set { bodySource = value; }
		}

		///<summary>
		///Gets or sets the value for BodyOutput
		///</summary>
		public string BodyOutput
		{
			get { return bodyOutput; }
			set { bodyOutput = value; }
		}

		///<summary>
		///Gets or sets the value for ModerationState
		///</summary>
		public short ModerationState
		{
			get { return moderationState; }
			set { moderationState = value; }
		}

		///<summary>
		///Gets or sets the value for MarkupType
		///</summary>
		public short MarkupType
		{
			get { return markupType; }
			set { markupType = value; }
		}

		#endregion

		#region Constructors

		public ForumTopicMessage()
		{
		}

		public ForumTopicMessage(long forumTopicMessageID, long forumTopicID, long? authorUserID, string authorName, DateTime dateCreated, string bodySource, string bodyOutput, short moderationState, short markupType)
		{
			this.forumTopicMessageID = forumTopicMessageID;
			this.forumTopicID = forumTopicID;
			this.authorUserID = authorUserID;
			this.authorName = authorName;
			this.dateCreated = dateCreated;
			this.bodySource = bodySource;
			this.bodyOutput = bodyOutput;
			this.moderationState = moderationState;
			this.markupType = markupType;
		}

		public ForumTopicMessage(IDataReader reader)
		{
			if (reader["ForumTopicMessageID"] != DBNull.Value) forumTopicMessageID = (long)reader["ForumTopicMessageID"];
			if (reader["ForumTopicID"] != DBNull.Value) forumTopicID = (long)reader["ForumTopicID"];
			if (reader["AuthorUserID"] != DBNull.Value) authorUserID = (long?)reader["AuthorUserID"];
			if (reader["AuthorName"] != DBNull.Value) authorName = (string)reader["AuthorName"];
			if (reader["DateCreated"] != DBNull.Value) dateCreated = (DateTime)reader["DateCreated"];
			if (reader["BodySource"] != DBNull.Value) bodySource = (string)reader["BodySource"];
			if (reader["BodyOutput"] != DBNull.Value) bodyOutput = (string)reader["BodyOutput"];
			if (reader["ModerationState"] != DBNull.Value) moderationState = (short)reader["ModerationState"];
			if (reader["MarkupType"] != DBNull.Value) markupType = (short)reader["MarkupType"];
		}

		#endregion

		#region Clone
		public ForumTopicMessage Clone()
		{
			ForumTopicMessage copy = new ForumTopicMessage();
			copy.forumTopicMessageID = forumTopicMessageID;
			copy.forumTopicID = forumTopicID;
			copy.authorUserID = authorUserID;
			copy.authorName = authorName;
			copy.dateCreated = dateCreated;
			copy.bodySource = bodySource;
			copy.bodyOutput = bodyOutput;
			copy.moderationState = moderationState;
			copy.markupType = markupType;
			return copy;
		}
		#endregion

		#region JSON Methods

		/// <summary>
		/// Writes this entity out as a JSON formatted string
		/// </summary>
		public void WriteJSON(StringWriter writer)
		{
			writer.Write("{");
			JSON.EncodeNameValuePair(writer, "ForumTopicMessageID", forumTopicMessageID);
			writer.Write(",");
			JSON.EncodeNameValuePair(writer, "ForumTopicID", forumTopicID);
			writer.Write(",");
			JSON.EncodeNameValuePair(writer, "AuthorUserID", authorUserID);
			writer.Write(",");
			JSON.EncodeNameValuePair(writer, "AuthorName", authorName);
			writer.Write(",");
			JSON.EncodeNameValuePair(writer, "DateCreated", dateCreated);
			writer.Write(",");
			JSON.EncodeNameValuePair(writer, "BodySource", bodySource);
			writer.Write(",");
			JSON.EncodeNameValuePair(writer, "BodyOutput", bodyOutput);
			writer.Write(",");
			JSON.EncodeNameValuePair(writer, "ModerationState", moderationState);
			writer.Write(",");
			JSON.EncodeNameValuePair(writer, "MarkupType", markupType);
			writer.Write("}");
		}

		public void LoadJSON(object json)
		{
			Dictionary<string, object> values = json as Dictionary<string, object>;
			if (values == null) return;
			forumTopicMessageID = (long)values["ForumTopicMessageID"];
			forumTopicID = (long)values["ForumTopicID"];
			authorUserID = (long?)values["AuthorUserID"];
			authorName = (string)values["AuthorName"];
			dateCreated = (DateTime)values["DateCreated"];
			bodySource = (string)values["BodySource"];
			bodyOutput = (string)values["BodyOutput"];
			moderationState = (short)values["ModerationState"];
			markupType = (short)values["MarkupType"];
		}

		#endregion
		#endregion

		#region Custom

		public ForumModerationState Moderation
		{
			get { return (ForumModerationState)moderationState; }
			set { moderationState = (short)value; }
		}

		#endregion
	}

	public enum ForumModerationState
	{
		Pending = 0,
		Approved = 1,
		FlaggedForReview = 2,
		Spam = 3
	}

	public class ForumSummary : IPropertyEvaluatorExpression
	{
		private int topicCount = 0, replyCount = 0;
		private string authorUsername = null;
		private DateTime? lastReplyTime = null;
		private Forum forum;

		public Forum Forum
		{
			get { return forum; }
		}

		public DateTime? LastReplyTime
		{
			get { return lastReplyTime; }
		}

		public string AuthorUsername
		{
			get { return authorUsername; }
		}

		public int TopicCount
		{
			get { return topicCount; }
		}

		public int ReplyCount
		{
			get { return replyCount; }
		}

		public ForumSummary(IDataReader reader)
		{
			if (reader["AuthorUsername"] != DBNull.Value) authorUsername = (string)reader["AuthorUsername"];
			if (reader["TopicCount"] != DBNull.Value) topicCount = (int)reader["TopicCount"];
			if (reader["ReplyCount"] != DBNull.Value) replyCount = (int)reader["ReplyCount"];
			if (reader["LastReplyTime"] != DBNull.Value) lastReplyTime = (DateTime)reader["LastReplyTime"];
			forum = new Forum(reader);
		}

		public bool IsValidPropertyName(string propertyName)
		{
			switch (propertyName)
			{
				case "authorusername":
				case "topiccount":
				case "replycount":
				case "lastreplytime":
				case "forum":
					return true;
				default:
					return false;
			}
		}

		public object EvaluateProperty(string propertyName, Token token, ExecutionState state)
		{
			switch (propertyName)
			{
				case "authorusername":
					return authorUsername;
				case "topiccount":
					return topicCount;
				case "replycount":
					return replyCount;
				case "lastreplytime":
					return lastReplyTime;
				case "forum":
					return forum;
				default:
					return VariableExpression.InvalidProperty;
			}
		}

		public object Evaluate(ExecutionState state, Token contextToken)
		{
			return this;
		}

		public override string ToString()
		{
			return "[Forum Summary: " + forum.Name + "]";
		}
	}

	public static class ForumPermissionType
	{
		public const string ForumCreator = "FORUM_CREATOR";
	}
}
