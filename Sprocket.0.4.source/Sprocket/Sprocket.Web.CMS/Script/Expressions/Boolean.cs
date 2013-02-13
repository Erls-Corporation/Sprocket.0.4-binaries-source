using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Sprocket.Web.CMS.Script
{
	public class BooleanExpression : IExpression
	{
		IExpression expr = null;
		object o = null;
		public object Evaluate(ExecutionState state, Token contextToken)
		{
			if (expr != null)
				o = expr.Evaluate(state, contextToken);
			if (o == null)
				return new SoftBoolean(false);
			return new SoftBoolean(!(o.Equals(0m) || o.Equals("") || o.Equals(false)));
		}

		public class SoftBoolean
		{
			bool value;
			public SoftBoolean(bool value)
			{
				this.value = value;
			}

			public override bool Equals(object o)
			{
				if (o == null) return !value;
				bool oValue = !(o.Equals(0m) || o.Equals(null) || o.Equals("") || o.Equals(false));
				return value == oValue;
			}

			public override int GetHashCode()
			{
				return base.GetHashCode();
			}

			public override string ToString()
			{
				return "{" + value.ToString() + "}";
			}
		}

		private static SoftBoolean _true = new SoftBoolean(true);
		private static SoftBoolean _false = new SoftBoolean(false);
		public static SoftBoolean True
		{
			get { return _true; }
		}
		public static SoftBoolean False
		{
			get { return _false; }
		}

		public void PrepareExpression(Token expressionToken, List<Token> tokens, ref int nextIndex, Stack<int?> precedenceStack)
		{
			Token token = expressionToken;
			if (token.Value == "true")
				o = true;
			else
				o = false;
		}

		public BooleanExpression() { }
		public BooleanExpression(IExpression expr)
		{
			this.expr = expr;
		}
	}

	public class BooleanExpressionCreator : IExpressionCreator
	{
		public string Keyword { get { return "true"; } }
		public IExpression Create() { return new BooleanExpression(); }
	}

	public class BooleanExpressionCreator2 : IExpressionCreator
	{
		public string Keyword { get { return "false"; } }
		public IExpression Create() { return new BooleanExpression(); }
	}

	public class BooleanExpressionCreator3 : IExpressionCreator
	{
		public string Keyword { get { return "empty"; } }
		public IExpression Create() { return new BooleanExpression(); }
	}

	public class BooleanExpressionCreator4 : IExpressionCreator
	{
		public string Keyword { get { return "nothing"; } }
		public IExpression Create() { return new BooleanExpression(); }
	}

	public class BooleanExpressionCreator5 : IExpressionCreator
	{
		public string Keyword { get { return "null"; } }
		public IExpression Create() { return new BooleanExpression(); }
	}
}
