using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Sprocket.Web.CMS.Script
{
	public class NumericExpression : IFlexibleSyntaxExpression
	{
		private object value;

		public object Evaluate(ExecutionState state, Token contextToken)
		{
			return value;
		}

		public void PrepareExpression(TokenList tokens, Stack<int?> precedenceStack)
		{
			decimal n;
			if (decimal.TryParse(tokens.Current.Value, out n))
				value = n;
			else
				value = null;
			tokens.Advance();
		}

		public NumericExpression() { }
		public NumericExpression(TokenList tokens, Stack<int?> precedenceStack)
		{
			PrepareExpression(tokens, precedenceStack);
		}

		public override string ToString()
		{
			return "{NumericExpression: " + value + "}";
		}
	}
}
