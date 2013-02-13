using System;
using System.Collections.Generic;
using System.Text;
using Sprocket.Web.CMS.Script;

using System.Xml;

namespace Sprocket.Web.CMS.Content.Expressions
{
	public class PageExpression : IPropertyEvaluatorExpression, IArgumentListEvaluatorExpression
	{
		public bool IsValidPropertyName(string propertyName)
		{
			return new PageEntry().IsValidPropertyName(propertyName);
		}

		public object EvaluateProperty(string propertyName, Token token, ExecutionState state)
		{
			if (ContentManager.PageStack.Count == 0)
				throw new InstructionExecutionException("I can't retrieve information for the current page because there is not a specific page entry defined for the current request. (definitions.xml)", token);
			return ContentManager.PageStack.Peek().EvaluateProperty(propertyName, token, state);
		}

		public object Evaluate(Token contextToken, List<ExpressionArgument> args, ExecutionState state)
		{
			if (args.Count != 1)
				throw new InstructionExecutionException("There should be only one argument in the argument list and it should specify the code name of the page to retrieve.", contextToken);
			object argval = args[0].Expression.Evaluate(state, args[0].Token);
			if (argval == null)
				throw new InstructionExecutionException("The argument you specified equates to null. I need a non-null value in order to retrieve the page you want.", args[0].Token);
			PageEntry page = ContentManager.Pages.FromPageCode(argval.ToString());
			if(page == null)
				throw new InstructionExecutionException("The argument you specified equates to \"" + argval.ToString() + "\", which is not the code name for any existing page.", args[0].Token);
			return page;
		}

		public object Evaluate(ExecutionState state, Token contextToken)
		{
			if (ContentManager.PageStack.Count == 0)
				throw new InstructionExecutionException("I can't retrieve information for the current page because there is not a specific page entry defined for the current request. (definitions.xml)", contextToken);
			Token t = state.SourceToken;
			state.SourceToken = contextToken.Next;
			string s = ContentManager.PageStack.Peek().Render(state);
			state.SourceToken = t;
			return s;
		}
	}
	public class PageExpressionCreator : IExpressionCreator
	{
		public string Keyword { get { return "page"; } }
		public IExpression Create() { return new PageExpression(); }
	}
}
