using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Web;
using Sprocket.Web.CMS.Script;
using Sprocket.Utility;

namespace Sprocket.Web.CMS.Content.Expressions
{
	class QueryStringExpression : IListExpression, IArgumentListEvaluatorExpression
	{
		public object Evaluate(ExecutionState state, Token contextToken)
		{
			return WebUtility.RawQueryString;
		}

		public IList GetList(ExecutionState state)
		{
			List<QSComponent> list = new List<QSComponent>();
			foreach (string s in HttpContext.Current.Request.QueryString.Keys)
				list.Add(new QSComponent(s, HttpContext.Current.Request.QueryString[s]));
			return list;
		}

		private class QSComponent : IPropertyEvaluatorExpression
		{
			public object Evaluate(ExecutionState state, Token contextToken)
			{
				return value;
			}

			string value, name;
			public QSComponent(string name, string value)
			{
				this.name = name;
				this.value = value;
			}

			public bool IsValidPropertyName(string propertyName)
			{
				switch (propertyName)
				{
					case "name":
					case "value":
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
					case "length": return value.Length;
					case "name": return name;
					case "value": return value;
					default: throw new InstructionExecutionException("\"" + propertyName + "\" is not a property of this variable", token);
				}
			}

			public override string ToString()
			{
				return name + "=" + value;
			}
		}

		public object Evaluate(Token contextToken, List<ExpressionArgument> args, ExecutionState state)
		{
			if (args.Count > 1)
				throw new TokenParserException("the querystring expression must contain no more than one argument, and it should be a number specifying which querystring element you want, or a string (word) specifying the name of the querystring parameter you want.", contextToken);
			object o = TokenParser.VerifyUnderlyingType(args[0].Expression.Evaluate(state, args[0].Token));
			if (o is decimal)
			{
				int n = Convert.ToInt32(o);
				if (n >= HttpContext.Current.Request.QueryString.Count)
					throw new TokenParserException("The index you specified is greater than the top index in the querystring", args[0].Token);
				return HttpContext.Current.Request.QueryString[n];
			}
			else if (o == null)
				throw new TokenParserException("The index you specified equates to null, which I can't use here.", args[0].Token);
			else
				return HttpContext.Current.Request.QueryString[o.ToString()];
		}
	}

	class QueryStringExpressionCreator : IExpressionCreator
	{
		public string Keyword
		{
			get { return "querystring"; }
		}

		public IExpression Create()
		{
			return new QueryStringExpression();
		}
	}
}
