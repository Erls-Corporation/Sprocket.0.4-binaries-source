using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

using Sprocket.Utility;
using Sprocket.Web.CMS.Script;

namespace Sprocket.Web.CMS.Content.Expressions
{
	abstract class StringModifierBaseExpression : IArgumentListEvaluatorExpression, IFlexibleSyntaxExpression
	{
		public object Evaluate(Token contextToken, List<ExpressionArgument> args, ExecutionState state)
		{
			StringBuilder sb = new StringBuilder();
			foreach (ExpressionArgument arg in args)
			{
				object o = arg.Expression.Evaluate(state, arg.Token);
				if (o == null)
					sb.Append("null");
				else
					sb.Append(Adjust(o.ToString()));
			}
			return sb.ToString();
		}

		public object Evaluate(ExecutionState state, Token contextToken)
		{
			throw new InstructionExecutionException("This expression requires at least one argument.", token);
		}
	
		Token token;
		public void PrepareExpression(TokenList tokens, Stack<int?> precedenceStack)
		{
 			token = tokens.Current;
			tokens.Advance();
		}

		protected abstract string Adjust(string s);
	}

	class HtmlEncodeExpression : StringModifierBaseExpression
	{
		protected override string Adjust(string s)
		{
			return HttpUtility.HtmlEncode(s);
		}
	}
	class HtmlEncodeExpressionCreator : IExpressionCreator
	{
		public string Keyword
		{
			get { return "htmlencode"; }
		}

		public IExpression Create()
		{
			return new HtmlEncodeExpression();
		}
	}

	class SafeHtmlEncodeExpression : StringModifierBaseExpression
	{
		protected override string Adjust(string s)
		{
			return WebUtility.SafeHtmlString(s, false);
		}
	}
	class SafeHtmlEncodeExpressionCreator : IExpressionCreator
	{
		public string Keyword
		{
			get { return "safehtmlencode"; }
		}

		public IExpression Create()
		{
			return new SafeHtmlEncodeExpression();
		}
	}

	class UrlEncodeExpression : StringModifierBaseExpression
	{
		protected override string Adjust(string s)
		{
			return WebUtility.UrlEncode(s);
		}
	}
	class UrlEncodeExpressionCreator : IExpressionCreator
	{
		public string Keyword
		{
			get { return "urlencode"; }
		}

		public IExpression Create()
		{
			return new UrlEncodeExpression();
		}
	}

	class LowerCaseExpression : StringModifierBaseExpression
	{
		protected override string Adjust(string s)
		{
			return s.ToLower();
		}
	}
	class LowerCaseExpressionCreator : IExpressionCreator
	{
		public string Keyword
		{
			get { return "lowercase"; }
		}

		public IExpression Create()
		{
			return new LowerCaseExpression();
		}
	}

	class UpperCaseExpression : StringModifierBaseExpression
	{
		protected override string Adjust(string s)
		{
			return s.ToUpper();
		}
	}
	class UpperCaseExpressionCreator : IExpressionCreator
	{
		public string Keyword
		{
			get { return "uppercase"; }
		}

		public IExpression Create()
		{
			return new UpperCaseExpression();
		}
	}

	class HowLongAgoExpression : IArgumentListEvaluatorExpression
	{
		public object Evaluate(Token contextToken, List<ExpressionArgument> args, ExecutionState state)
		{
			if(args.Count != 1)
				throw new InstructionExecutionException("how_long_ago expects exactly one argument specifying which date to evaluate.", contextToken);
			object o = args[0].Expression.Evaluate(state, args[0].Token);
			if (o is DateTime)
				return StringUtilities.ApproxHowLongAgo(SprocketDate.Now, (DateTime)o);
			throw new InstructionExecutionException("the argument being fed to how_long_ago turned out to evaluate to something other than a date. (Underlying type: "  + (o == null ? "null" : o.GetType().Name) + ")", contextToken);
		}

		public object Evaluate(ExecutionState state, Token contextToken)
		{
			throw new InstructionExecutionException("how_long_ago expects an argument specifying which date to evaluate.", contextToken);
		}
	}
	class HowLongAgoExpressionCreator : IExpressionCreator
	{
		public string Keyword
		{
			get { return "how_long_ago"; }
		}

		public IExpression Create()
		{
			return new HowLongAgoExpression();
		}
	}

	class FormatDateExpression : IArgumentListEvaluatorExpression
	{
		public object Evaluate(Token contextToken, List<ExpressionArgument> args, ExecutionState state)
		{
			if (args.Count != 2)
				throw new InstructionExecutionException("\"formatdate\" expects two arguments; one specifying which date to evaluate, then one specifying the format string.", contextToken);
			object o = args[0].Expression.Evaluate(state, args[0].Token);
			if (o is DateTime)
			{
				object s = args[1].Expression.Evaluate(state, args[1].Token);
				if (s is string)
				{
					try
					{
						return ((DateTime)o).ToString((string)s);
					}
					catch (Exception ex)
					{
						throw new InstructionExecutionException("The date format you specified was not valid. Try Googling for \"custom .Net DateTime formats\".", ex, args[1].Token);
					}
				}
			}
			throw new InstructionExecutionException("the first argument being fed to \"formatdate\" turned out to evaluate to something other than a date. (Underlying type: " + (o == null ? "null" : o.GetType().Name) + ")", contextToken);
		}

		public object Evaluate(ExecutionState state, Token contextToken)
		{
			throw new InstructionExecutionException("\"formatdate\" expects an argument specifying which date to evaluate.", contextToken);
		}
	}
	class FormatDateExpressionCreator : IExpressionCreator
	{
		public string Keyword
		{
			get { return "formatdate"; }
		}

		public IExpression Create()
		{
			return new FormatDateExpression();
		}
	}

	class CurrentDateExpression : IExpression
	{
		public object Evaluate(ExecutionState state, Token contextToken)
		{
			return SprocketDate.Now;
		}
	}
	class CurrentDateExpressionCreator : IExpressionCreator
	{
		public string Keyword
		{
			get { return "currentdate"; }
		}

		public IExpression Create()
		{
			return new CurrentDateExpression();
		}
	}
}
