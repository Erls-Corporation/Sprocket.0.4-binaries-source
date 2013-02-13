using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Sprocket.Web.CMS.Script
{
	public class StringExpression : IFlexibleSyntaxExpression, IPropertyEvaluatorExpression
	{
		string text;

		public void PrepareExpression(TokenList tokens, Stack<int?> precedenceStack)
		{
			text = tokens.Current.Value;
			tokens.Advance();
		}

		public object Evaluate(ExecutionState state, Token contextToken)
		{
			return text;
		}

		public StringExpression(string text)
		{
			this.text = text;
		}

		public StringExpression(TokenList tokens, Stack<int?> precedenceStack)
		{
			PrepareExpression(tokens, precedenceStack);
		}

		public override string ToString()
		{
			return "{StringExpression}";
		}

		public bool IsValidPropertyName(string propertyName)
		{
			switch (propertyName)
			{
				case "length":
					return true;
				default:
					return false;
			}
		}

		public object EvaluateProperty(string propertyName, Token token, ExecutionState state)
		{
			switch (propertyName)
			{
				case "length": return text.Length;
				default: return VariableExpression.InvalidProperty;
			}
		}
	}
}
