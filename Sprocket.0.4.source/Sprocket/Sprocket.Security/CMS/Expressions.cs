using System;
using System.Collections.Generic;
using System.Text;
using Sprocket.Web;
using Sprocket.Web.CMS.Script;


namespace Sprocket.Security.CMS
{
	class LoggedInExpression : IExpression
	{
		public object Evaluate(ExecutionState state, Token contextToken)
		{
			return WebAuthentication.Instance.IsLoggedIn;
		}
	}
	class LoggedInExpressionCreator : IExpressionCreator
	{
		public string Keyword
		{
			get { return "loggedin"; }
		}

		public IExpression Create()
		{
			return new LoggedInExpression();
		}
	}

	class UsernameExpression : IExpression
	{
		public object Evaluate(ExecutionState state, Token contextToken)
		{
			if (!WebAuthentication.Instance.IsLoggedIn)
				return "[not logged in]";
			return SecurityProvider.CurrentUser.Username;
		}
	}
	class UsernameExpressionCreator : IExpressionCreator
	{
		public string Keyword
		{
			get { return "username"; }
		}

		public IExpression Create()
		{
			return new UsernameExpression();
		}
	}

	class AccountActivatedExpression : IExpression
	{
		public object Evaluate(ExecutionState state, Token contextToken)
		{
			if (!WebAuthentication.Instance.IsLoggedIn)
				return "[not logged in]";
			return SecurityProvider.CurrentUser.Activated;
		}
	}
	class AccountActivatedExpressionCreator : IExpressionCreator
	{
		public string Keyword
		{
			get { return "accountactivated"; }
		}

		public IExpression Create()
		{
			return new AccountActivatedExpression();
		}
	}

	class UserIDExpression : IExpression
	{
		public object Evaluate(ExecutionState state, Token contextToken)
		{
			if (!WebAuthentication.Instance.IsLoggedIn)
				return "[not logged in]";
			return SecurityProvider.CurrentUser.UserID;
		}
	}
	class UserIDExpressionCreator : IExpressionCreator
	{
		public string Keyword
		{
			get { return "userid"; }
		}

		public IExpression Create()
		{
			return new UserIDExpression();
		}
	}

	class EmailChangePendingExpression : IExpression
	{
		public object Evaluate(ExecutionState state, Token contextToken)
		{
			if (!WebAuthentication.Instance.IsLoggedIn)
				return "[not logged in]";
			if (CurrentRequest.Value["EmailChangePendingExpression_Value"] != null)
				return (bool)CurrentRequest.Value["EmailChangePendingExpression_Value"];
			EmailChangeRequest ecr = SecurityProvider.DataLayer.SelectEmailChangeRequest(SecurityProvider.CurrentUser.UserID);
			bool val;
			if (ecr == null)
				val = false;
			else
				val = true;
			CurrentRequest.Value["EmailChangePendingExpression_Value"] = val;
			return val;
		}
	}
	class EmailChangePendingExpressionCreator : IExpressionCreator
	{
		public string Keyword
		{
			get { return "emailchangepending"; }
		}

		public IExpression Create()
		{
			return new EmailChangePendingExpression();
		}
	}

	class EmailAddressExpression : IExpression
	{
		public object Evaluate(ExecutionState state, Token contextToken)
		{
			if (!WebAuthentication.Instance.IsLoggedIn)
				return "[not logged in]";
			return SecurityProvider.CurrentUser.Email;
		}
	}
	class EmailAddressExpressionCreator : IExpressionCreator
	{
		public string Keyword
		{
			get { return "emailaddress"; }
		}

		public IExpression Create()
		{
			return new EmailAddressExpression();
		}
	}
}
