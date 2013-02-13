using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Sprocket.Web.CMS.Script
{
	public class ExpressionArgument
	{
		private IExpression expression;
		private Token token;

		public Token Token
		{
			get { return token; }
		}

		public IExpression Expression
		{
			get { return expression; }
		}

		public ExpressionArgument(IExpression expr, Token token)
		{
			this.token = token;
			expression = expr;
		}
	}
}
