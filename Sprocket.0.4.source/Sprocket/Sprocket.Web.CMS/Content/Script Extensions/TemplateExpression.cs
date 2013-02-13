using System;
using System.Collections.Generic;
using System.Text;
using Sprocket.Web.CMS.Script;


namespace Sprocket.Web.CMS.Content.Expressions
{
	class TemplateExpression : IArgumentListEvaluatorExpression
	{
		public object Evaluate(Token contextToken, List<ExpressionArgument> args, ExecutionState state)
		{
			if (args.Count != 1)
				throw new TokenParserException("The \"template\" expression takes a single argument specifying which template to load", contextToken);
			object o = args[0].Expression.Evaluate(state, args[0].Token);
			if(o == null)
				throw new TokenParserException("You can't request a template using a null value as the template name.", args[0].Token);

			Template t = ContentManager.Templates[o.ToString()];
			if(t == null)
				throw new TokenParserException("The template \"" + o.ToString() + "\" does not exist.", args[0].Token);

			return t;
		}

		public object Evaluate(ExecutionState state, Token contextToken)
		{
			throw new InstructionExecutionException("You can't use the \"template\" expression by itself as the default behaviour is to render the content of the template which would result in infinite recursion.", contextToken.Next);
		}
	}

	class TemplateExpressionCreator : IExpressionCreator
	{
		public string Keyword
		{
			get { return "template"; }
		}

		public IExpression Create()
		{
			return new TemplateExpression();
		}
	}
}
