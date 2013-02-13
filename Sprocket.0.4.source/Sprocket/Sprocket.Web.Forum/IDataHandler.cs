using System;
using System.Collections.Generic;
using System.Text;
using Sprocket.Data;

namespace Sprocket.Web.Forums
{
	public interface IForumDataHandler
	{
		Type DatabaseHandlerType { get; }
		void InitialiseDatabase(Result result);

		#region Definitions for Forum

		event InterruptableEventHandler<Forum> OnBeforeDeleteForum;
		event NotificationEventHandler<Forum> OnForumDeleted;
		Result Store(Forum forum);
		Result Delete(Forum forum);
		Forum SelectForum(long id);

		#endregion

		Forum SelectForumByCode(string forumCode);
		Forum SelectForumByURLToken(string urlToken);

		#region Definitions for ForumCategory

		event InterruptableEventHandler<ForumCategory> OnBeforeDeleteForumCategory;
		event NotificationEventHandler<ForumCategory> OnForumCategoryDeleted;
		Result Store(ForumCategory forumCategory);
		Result Delete(ForumCategory forumCategory);
		ForumCategory SelectForumCategory(long id);

		#endregion

		ForumCategory SelectForumCategoryByCode(string categoryCode);
		ForumCategory SelectForumCategoryByURLToken(string urlToken);

		#region Definitions for ForumTopic

		event InterruptableEventHandler<ForumTopic> OnBeforeDeleteForumTopic;
		event NotificationEventHandler<ForumTopic> OnForumTopicDeleted;
		Result Store(ForumTopic forumTopic);
		Result Delete(ForumTopic forumTopic);
		ForumTopic SelectForumTopic(long id);

		#endregion
		#region Definitions for ForumTopicMessage

		event InterruptableEventHandler<ForumTopicMessage> OnBeforeDeleteForumTopicMessage;
		event NotificationEventHandler<ForumTopicMessage> OnForumTopicMessageDeleted;
		Result Store(ForumTopicMessage forumTopicMessage);
		Result Delete(ForumTopicMessage forumTopicMessage);
		ForumTopicMessage SelectForumTopicMessage(long id);

		#endregion

		List<Forum> ListForums(long forumCategoryID);
		List<ForumSummary> ListForumSummary(string categoryCode);
	}
}
