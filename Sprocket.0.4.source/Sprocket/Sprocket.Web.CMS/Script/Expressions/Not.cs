using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Sprocket.Web.CMS.Script
{
	public class NotExpression : IFlexibleSyntaxExpression
	{
		private IExpression expression = null;

		public object Evaluate(ExecutionState state, Token contextToken)
		{
			return new NotValue(expression.Evaluate(state, contextToken));
		}

		public void PrepareExpression(TokenList tokens, Stack<int?> precedenceStack)
		{
			tokens.Advance();
			expression = TokenParser.BuildExpression(tokens, precedenceStack);
		}

		public class NotValue
		{
			object value;
			public NotValue(object value)
			{
				this.value = value;
			}
			public override bool Equals(object obj)
			{
				if (value is bool)
					if ((bool)value)
						return BooleanExpression.False.Equals(obj);
					else
						return BooleanExpression.True.Equals(obj);
				return !object.Equals(obj, value);
			}
			public override string ToString()
			{
				return "{not " + value + "}";
			}
			public override int GetHashCode()
			{
				return base.GetHashCode();
			}
		}
	}
	public class NotExpressionCreator : IExpressionCreator
	{
		public string Keyword { get { return "not"; } }
		public IExpression Create() { return new NotExpression(); }
	}
}
