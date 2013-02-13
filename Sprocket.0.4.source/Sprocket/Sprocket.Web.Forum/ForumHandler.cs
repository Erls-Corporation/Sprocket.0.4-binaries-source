using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Transactions;

using Sprocket;
using Sprocket.Data;
using Sprocket.Web;
using Sprocket.Web.CMS;
using Sprocket.Web.CMS.Content;
using Sprocket.Security;

namespace Sprocket.Web.Forums
{
	[ModuleTitle("Forums Module")]
	[ModuleDescription("Provides discussion forum functionality")]
	//[AjaxMethodHandler("ForumHandler")]
	[ModuleDependency(typeof(WebEvents))]
	[ModuleDependency(typeof(DatabaseManager))]
	public class ForumHandler : ISprocketModule
	{
		HttpRequest Request { get { return HttpContext.Current.Request; } }
		HttpResponse Response { get { return HttpContext.Current.Response; } }

		public void AttachEventHandlers(ModuleRegistry registry)
		{
			DatabaseManager.Instance.OnDatabaseHandlerLoaded += new NotificationEventHandler<IDatabaseHandler>(DatabaseManager_OnDatabaseHandlerLoaded);
			ContentManager.Instance.OnBeforeRenderPage += new ContentManager.BeforeRenderPage(ContentManager_OnBeforeRenderPage);
		}

		void ContentManager_OnBeforeRenderPage(PageEntry page)
		{
			if (SprocketPath.Sections[SprocketPath.Sections.Length - 1] == "$forum_submit")
			{
				switch (Request.Form["action"])
				{
					case "save_forum_settings": SaveForumSettings(); break;
					case "post_topic": PostTopic(); break;
					default:
						Response.Write("<p>Nope, sorry.</p><p>Click <a href=\"" + HttpUtility.HtmlEncode(Request.UrlReferrer.ToString()) + "\">here</a> to go back to where you were.</p>");
						Response.End();
						return;
				}
				Response.Redirect(Request.UrlReferrer.ToString());
			}
		}

		private IForumDataHandler dataLayer = null;
		public static IForumDataHandler DataLayer
		{
			get { return Instance.dataLayer; }
		}

		public static ForumHandler Instance
		{
			get { return (ForumHandler)Core.Instance[typeof(ForumHandler)].Module; }
		}

		void DatabaseManager_OnDatabaseHandlerLoaded(IDatabaseHandler source)
		{
			source.OnInitialise += new InterruptableEventHandler(DatabaseHandler_OnInitialise);
			foreach (Type t in Core.Modules.GetInterfaceImplementations(typeof(IForumDataHandler)))
			{
				IForumDataHandler dal = (IForumDataHandler)Activator.CreateInstance(t);
				if (dal.DatabaseHandlerType == source.GetType())
				{
					dataLayer = dal;
					break;
				}
			}
		}

		void DatabaseHandler_OnInitialise(Result result)
		{
			if (!result.Succeeded)
				return;
			if (dataLayer == null)
				result.SetFailed("ForumHandler has no implementation for " + DatabaseManager.DatabaseEngine.Title);
			else
				dataLayer.InitialiseDatabase(result);
		}

		private void PostTopic()
		{
			string forumStr = Request.Form["forum"];
			string path = Request.Form["path"];

			string notLoggedInURL = Request.Form["notLoggedInURL"];

			Forum forum = DataLayer.SelectForumByURLToken(forumStr);
			if (forum == null)
				forum = DataLayer.SelectForumByCode(forumStr);
			if (forum == null)
			{
				WriteErrorMessage("Bad forum code");
				return;
			}

			#region Check to see if the current user is allowed to post a new topic
			switch (forum.PostNewTopics)
			{
				case Forum.AccessType.AllowAnonymous:
					throw new NotImplementedException("need to put in anonymous author name.");

				case Forum.AccessType.ActivatedMembers:
					CheckAuthentication(notLoggedInURL);
					if (!SecurityProvider.CurrentUser.Activated)
					{
						WriteErrorMessage("You're not authenticated yet.");
						return;
					}
					break;

				case Forum.AccessType.AllMembers:
					CheckAuthentication(notLoggedInURL);
					break;

				case Forum.AccessType.Administrators:
					CheckAuthentication(notLoggedInURL);
					if (!SecurityProvider.CurrentUser.HasPermission(PermissionType.AdministrativeAccess))
					{
						WriteErrorMessage("Only administrators may post new topics.");
						return;
					}
					break;

				case Forum.AccessType.RoleMembers:
					CheckAuthentication(notLoggedInURL);
					if (forum.PostWriteAccessRoleID.HasValue)
					{
						Role role = SecurityProvider.DataLayer.SelectRole(forum.PostWriteAccessRoleID.Value);
						if (role != null)
						{
							if (SecurityProvider.CurrentUser.HasRole(role.RoleCode))
								break;
						}
					}
					WriteErrorMessage("You don't have the required permissions to post new topics.");
					return;
			}
			#endregion

			ForumTopic topic = new ForumTopic();
			ForumTopicMessage msg = new ForumTopicMessage();

			if (WebAuthentication.Instance.IsLoggedIn)
			{
				topic.AuthorUserID = SecurityProvider.CurrentUser.UserID;
				msg.AuthorUserID = SecurityProvider.CurrentUser.UserID;
			}
			else
			{
				throw new NotImplementedException("need to put in anonymous author name.");
				//topic.AuthorName =
				//msg.AuthorName =
			}

			topic.DateCreated = SprocketDate.Now;
			topic.ForumID = forum.ForumID;
			topic.ForumTopicID = 0;

#warning to do: let administrators put in a "locked" checkbox to lock the topic by default when posting it
			topic.Locked = false;

#warning to do: check for spam
			if (forum.RequireModeration)
				topic.Moderation = ForumModerationState.Pending;
			else
				topic.Moderation = ForumModerationState.Approved;

#warning to do: should be able to make the topic sticky when posting it
			topic.Sticky = false;

#warning to do: validate the subject. if invalid, store values in fast-expiring cookie and redirect to standalone posting page
			topic.Subject = Request.Form["subject"];

#warning to do: administrators should be able to specify a URL Token
			//topic.URLToken

			msg.BodySource = Request.Form["body"];
			switch (forum.Markup)
			{
				case Forum.MarkupType.BBCode:
#warning to do: check for images in source
					throw new NotImplementedException("BBCode not implemented yet.");

				case Forum.MarkupType.None:
					msg.BodyOutput = HttpUtility.HtmlEncode(msg.BodySource).Replace(Environment.NewLine, "<br />");
					break;

				case Forum.MarkupType.Textile:
#warning to do: check for images in source
					msg.BodyOutput = Textile.TextileFormatter.FormatString(msg.BodySource);
					break;

				case Forum.MarkupType.LimitedHTML:
#warning to do: check for images in source
					throw new NotImplementedException("Limited HTML not implemented yet.");

				case Forum.MarkupType.ExtendedHTML:
#warning to do: check for images in source
					msg.BodyOutput = WebUtility.SafeHtmlString(msg.BodySource, true);
					break;

				default:
					throw new NotImplementedException();
			}
#warning to do: signatures need to be appended to the output

			msg.ForumTopicMessageID = 0;
			msg.DateCreated = SprocketDate.Now;

			if (forum.RequireModeration)
				msg.Moderation = ForumModerationState.Pending;
			else
			{
				if (MightBeSpam(msg.BodySource))
				{
					msg.Moderation = ForumModerationState.Pending;
					topic.Moderation = ForumModerationState.Pending;
				}
				else
				{
					msg.Moderation = ForumModerationState.Approved;
				}
			}

			try
			{
				using (TransactionScope scope = new TransactionScope())
				{
					DatabaseManager.DatabaseEngine.PersistConnection();
					DataLayer.Store(topic);
					msg.ForumTopicID = topic.ForumTopicID;
					DataLayer.Store(msg);
					scope.Complete();
				}
			}
			finally
			{
				DatabaseManager.DatabaseEngine.ReleaseConnection();
			}

#warning to do: redirect to message rather than the forum itself.
		}

		void WriteErrorMessage(string msg)
		{
			Response.Write("<p>" + msg + "</p><p>Click <a href=\"" + HttpUtility.HtmlEncode(Request.UrlReferrer.ToString()) + "\">here</a> to go back to where you were. Maybe refresh the page when you get there.</p>");
			Response.End();
		}

		bool MightBeSpam(string str)
		{
			string[] spamWords = new string[]{
				"your loan request", "immediate cash", "your monthly payments", "this offer", "bad credit", "this deal", "refinance",
				"huge savings", "your application", "cialis", "unbeatable", "online pharmacy", "penis enlarge", "viagra", "v1agra", "vi4gra",
				"online casino", "earn money", "earn cash", "make money"
			};
			str = str.ToLower();
			foreach (string word in spamWords)
			{
				if (str.Contains(word))
					return true;
			}
			return false;
		}

		void CheckAuthentication(string notLoggedInURL)
		{
			if (!WebAuthentication.Instance.IsLoggedIn)
			{
				if (notLoggedInURL == "" || notLoggedInURL == null)
					Response.Redirect(notLoggedInURL);
				else
				{
					WriteErrorMessage("You're not logged in.");
					return;
				}
			}
		}

		void SaveForumSettings()
		{
			if (!SecurityProvider.CurrentUser.HasPermission(ForumPermissionType.ForumCreator))
				throw new Exception("You don't have permission to create, edit or delete forums.");

			Forum forum;
			ForumCategory cat;

			long forumID = long.Parse(Request.Form["forumid"]);
			string categoryCode = Request.Form["categorycode"];

			cat = DataLayer.SelectForumCategoryByCode(categoryCode);
			if (cat == null)
			{
				cat = new ForumCategory(DatabaseManager.GetUniqueID(), SecurityProvider.ClientSpaceID,
					categoryCode, categoryCode, null, SprocketDate.Now, 0, false);
				DataLayer.Store(cat);
			}

			if (forumID == 0)
			{
				forum = new Forum();
				forum.ForumID = DatabaseManager.GetUniqueID();
			}
			else
				forum = dataLayer.SelectForum(forumID);
			if (forum == null)
				return;

			forum.Name = Request.Form["forum-name"].Trim();
			if (forum.Name == "") forum.Name = "Untitled Forum";
			forum.URLToken = Request.Form["url-token"].Trim();
			if (forum.URLToken == "") forum.URLToken = forum.ForumID.ToString();
			forum.ForumCode = Request.Form["forum-code"].Trim();
			forum.ForumCategoryID = cat.ForumCategoryID;
			forum.TopicDisplayOrder = short.Parse(Request.Form["topic-display-order"]);
			forum.MarkupLevel = short.Parse(Request.Form["markuplevel"]);

			string postAccess = Request.Form["post-access"];
			string replyAccess = Request.Form["reply-access"];
			string readAccess = Request.Form["read-access"];

			if (postAccess.StartsWith("_"))
			{
				forum.PostWriteAccess = short.Parse(postAccess.Substring(1));
				forum.PostWriteAccessRoleID = null;
			}
			else
			{
				forum.PostNewTopics = Forum.AccessType.RoleMembers;
				forum.PostWriteAccessRoleID = long.Parse(postAccess);
			}

			if (replyAccess.StartsWith("_"))
			{
				forum.ReplyWriteAccess = short.Parse(replyAccess.Substring(1));
				forum.ReplyWriteAccessRoleID = null;
			}
			else
			{
				forum.WriteReplies = Forum.AccessType.RoleMembers;
				forum.ReplyWriteAccessRoleID = long.Parse(replyAccess);
			}

			if (readAccess.StartsWith("_"))
			{
				forum.ReadAccess = short.Parse(readAccess.Substring(1));
				forum.ReadAccessRoleID = null;
			}
			else
			{
				forum.Read = Forum.AccessType.RoleMembers;
				forum.ReadAccessRoleID = long.Parse(readAccess);
			}

			forum.ModeratorRoleID = long.Parse(Request.Form["moderator-role"]);
			forum.RequireModeration = Request.Form["requires-moderation"] == "1";
			forum.AllowVoting = Request.Form["allow-voting"] == "1";
			forum.AllowImagesInMessages = Request.Form["message-images"] == "1";
			forum.ShowSignatures = Request.Form["show-signatures"] == "1";
			forum.AllowImagesInSignatures = Request.Form["signature-images"] == "1";
			forum.Locked = Request.Form["lock-forum"] == "1";
			forum.DateCreated = SprocketDate.Now;

			Result r = dataLayer.Store(forum);
			if (!r.Succeeded)
				throw new Exception(r.Message);
		}
	}
}
