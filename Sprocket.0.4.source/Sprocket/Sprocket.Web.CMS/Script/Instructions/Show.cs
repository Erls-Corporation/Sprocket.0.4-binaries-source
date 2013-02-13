using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Sprocket.Web.CMS.Script
{
	internal class ShowInstruction : IInstruction
	{
		private IExpression expression = null;
		private Token token;

		public ShowInstruction() { }
		public ShowInstruction(IExpression expr)
		{
			expression = expr;
		}

		public void Build(TokenList tokens)
		{
			token = tokens.Current;
			// advance past the instruction token/symbol
			tokens.Advance();

			// build the expression to evaluate and display at execution time
			expression = TokenParser.BuildExpression(tokens);
		}

		public void Execute(ExecutionState state)
		{
			object text = expression.Evaluate(state, token);
			while (text is IExpression)
			{
				object eval = ((IExpression)text).Evaluate(state, token);
				if (object.ReferenceEquals(eval, text))
					text = eval.ToString();
				else
					//throw new InstructionExecutionException("Can't evaluate expression. Infinite recursion detected in type " + eval.GetType().FullName, token);
					text = eval;
			}
			if (text is IList)
				state.Output.Write(RenderList((IList)text, state));
			else if (text == null)
				state.Output.Write("");
			else
				state.Output.Write(text.ToString());
		}

		private string RenderList(IList list, ExecutionState state)
		{
			StringBuilder sb = new StringBuilder("[");
			foreach (object o in list)
			{
				if (sb.Length > 1)
					sb.Append(",");
				object val = o;
				while (val is IExpression)
				{
					val = ((IExpression)o).Evaluate(state, token);
					if (object.ReferenceEquals(o, val))
						break;
				}
				if (val is IList)
					sb.Append(RenderList((IList)val, state));
				else if (val == null)
					sb.Append("");
				else
					sb.Append(val.ToString());
			}
			sb.Append("]");
			return sb.ToString();
		}
	}

	internal class ShowInstructionCreator : IInstructionCreator
	{
		public string Keyword { get { return "show"; } }
		public IInstruction Create() { return new ShowInstruction(); }
	}

	internal class ShowInstructionCreator2 : IInstructionCreator
	{
		public string Keyword { get { return "?"; } }
		public IInstruction Create() { return new ShowInstruction(); }
	}
}
