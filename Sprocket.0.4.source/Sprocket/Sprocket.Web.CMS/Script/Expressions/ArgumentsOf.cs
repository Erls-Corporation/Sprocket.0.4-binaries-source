using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Sprocket.Web.CMS.Script
{
	internal class ArgumentsOfExpression : IExpression
	{
		private IExpression expr;
		private List<ExpressionArgument> args;
		private Token token;

		public ArgumentsOfExpression(IExpression expr, Token token, List<ExpressionArgument> args)
		{
			this.expr = expr;
			this.args = args;
			this.token = token;
		}

		public object Evaluate(ExecutionState state, Token contextToken)
		{
			if (expr is IArgumentListEvaluatorExpression)
				return ((IArgumentListEvaluatorExpression)expr).Evaluate(token, args, state);
			object o = expr.Evaluate(state, token);
			return SystemTypeEvaluator.EvaluateArguments(state, o, args, token);
		}
	}
}
