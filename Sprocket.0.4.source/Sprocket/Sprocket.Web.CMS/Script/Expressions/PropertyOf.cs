using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Sprocket.Web.CMS.Script
{
	internal class PropertyOfExpression : IExpression
	{
		private IExpression expr;
		private Token token; 

		public PropertyOfExpression(IExpression expr, Token token)
		{
			this.expr = expr;
			this.token = token;
		}

		public object Evaluate(ExecutionState state, Token contextToken)
		{
			if (expr is IPropertyEvaluatorExpression)
			{
				if (!((IPropertyEvaluatorExpression)expr).IsValidPropertyName(token.Value))
					throw new InstructionExecutionException("\"" + token.Value + "\" is not a valid property for this object. (Underlying type: " + expr.GetType().Name + ")", token);
				object x = ((IPropertyEvaluatorExpression)expr).EvaluateProperty(token.Value, token, state);
				if(x != null)
					if(x.ToString() == VariableExpression.InvalidProperty)
						throw new InstructionExecutionException("\"" + token.Value + "\" is not a valid property for this object. (Underlying type: " + expr.GetType().Name + ")", token);
				return x;
			}
			object o = expr.Evaluate(state, token);
			if(o is IPropertyEvaluatorExpression)
				return ((IPropertyEvaluatorExpression)o).EvaluateProperty(token.Value, token, state);
			return SystemTypeEvaluator.EvaluateProperty(o, token.Value, token);
		}
	}
}
