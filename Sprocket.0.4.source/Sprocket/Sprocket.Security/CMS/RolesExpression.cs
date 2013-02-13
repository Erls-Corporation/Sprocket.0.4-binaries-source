using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Sprocket.Web.CMS.Script;

namespace Sprocket.Security.CMS
{
	class RolesExpression : IListExpression
	{
		public IList GetList(ExecutionState state)
		{
			return SecurityProvider.DataLayer.ListAccessibleRoles(SecurityProvider.CurrentUser.UserID);
		}

		public object Evaluate(ExecutionState state, Token contextToken)
		{
			return GetList(state);
		}
	}

	class RolesExpressionCreator : IExpressionCreator
	{
		public string Keyword
		{
			get { return "roles"; }
		}

		public IExpression Create()
		{
			return new RolesExpression();
		}
	}
}
