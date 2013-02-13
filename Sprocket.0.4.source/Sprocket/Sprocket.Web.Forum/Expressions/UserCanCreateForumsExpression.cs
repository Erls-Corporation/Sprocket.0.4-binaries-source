using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

using Sprocket;
using Sprocket.Web;
using Sprocket.Web.CMS;
using Sprocket.Security;
using Sprocket.Web.CMS.Content;
using Sprocket.Web.CMS.Script;

namespace Sprocket.Web.Forums
{
	public class UserCanCreateForumsExpression : IExpression
	{
		public object Evaluate(ExecutionState state, Token contextToken)
		{
			if (!WebAuthentication.Instance.IsLoggedIn)
				return false;
			return SecurityProvider.CurrentUser.HasPermission(ForumPermissionType.ForumCreator);
		}
	}
	public class UserCanCreateForumsExpressionCreator : IExpressionCreator
	{
		public string Keyword { get { return "user_can_create_forums"; } }
		public IExpression Create() { return new UserCanCreateForumsExpression(); }
	}
}
