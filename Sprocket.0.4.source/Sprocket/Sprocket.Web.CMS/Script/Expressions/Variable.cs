using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Sprocket.Web.CMS.Script
{
	public class VariableExpression : IPropertyEvaluatorExpression, IArgumentListEvaluatorExpression, IFlexibleSyntaxExpression
	{
		public const string InvalidProperty = "___VariableExpression.InvalidProperty___(const|expr)";
		private Token variableToken = null;

		public Token VariableToken
		{
			get { return variableToken; }
		}

		public void PrepareExpression(TokenList tokens, Stack<int?> precedenceStack)
		{
			// get the token representing the variable name
			variableToken = tokens.Current;
			tokens.Advance();
		}

		public object Evaluate(ExecutionState state, Token contextToken)
		{
			// ensure that the variable has been set to some value first
			if(!state.HasVariable(variableToken.Value))
				throw new InstructionExecutionException("I can't evaluate the word \"" + variableToken.Value + "\". Either it doesn't mean anything or you forgot to assign it a value.", variableToken);

			return state.GetVariable(variableToken.Value);
		}

		public bool IsValidPropertyName(string propertyName)
		{
			return true;
		}

		public object EvaluateProperty(string propertyName, Token token, ExecutionState state)
		{
			object o = state.HasVariable(variableToken.Value) ? state.GetVariable(variableToken.Value) : null;
			if (o is IPropertyEvaluatorExpression)
			{
				object val = ((IPropertyEvaluatorExpression)o).EvaluateProperty(propertyName, token, state);
				if(val != null)
					if (val.ToString() == VariableExpression.InvalidProperty)
						throw new InstructionExecutionException("\"" + propertyName + "\" is not a valid property of this variable.", token);
				return val;
			}
			else
				return SystemTypeEvaluator.EvaluateProperty(o, propertyName, token);
		}

		public object Evaluate(Token contextToken, List<ExpressionArgument> args, ExecutionState state)
		{
			object o = state.HasVariable(variableToken.Value) ? state.GetVariable(variableToken.Value) : null;
			if (o is IArgumentListEvaluatorExpression)
				return ((IArgumentListEvaluatorExpression)o).Evaluate(contextToken, args, state);
			else
				return SystemTypeEvaluator.EvaluateArguments(state, o, args, contextToken);
		}
	}
}
