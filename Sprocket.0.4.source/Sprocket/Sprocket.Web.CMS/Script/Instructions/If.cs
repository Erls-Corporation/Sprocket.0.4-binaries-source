using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Sprocket.Web.CMS.Script
{
	public class IfInstruction : IInstruction
	{
		private List<ExecutionPath> executionPaths = new List<ExecutionPath>();
		public void Build(TokenList tokens)
		{
			Token token = tokens.Current;
			// advance past the instruction token/symbol
			tokens.Advance();

			// build condition expression
			IExpression expr = TokenParser.BuildExpression(tokens);

			// build the instruction list that will execute if the expression evaluates to true
			InstructionList list = new InstructionList();
			list.AcceptELSEInPlaceOfEND = true;
			list.Build(tokens);
			executionPaths.Add(new ExecutionPath(expr, list, token));

			// if any elseif statements were used, build the execution paths for each one
			while (list.Terminator == InstructionList.TerminatorType.ElseIf)
			{
				token = tokens.Previous;
				expr = TokenParser.BuildExpression(tokens);
				list = new InstructionList();
				list.AcceptELSEInPlaceOfEND = true;
				list.Build(tokens);
				executionPaths.Add(new ExecutionPath(expr, list, token));
			}

			// if an else statement was used, build the instruction list that will execute if the expression evaluates to false
			if (list.Terminator == InstructionList.TerminatorType.Else)
			{
				token = tokens.Previous;
				list = new InstructionList();
				list.AcceptELSEInPlaceOfEND = true;
				list.Build(tokens);
				executionPaths.Add(new ExecutionPath(null, list, token));
			}
		}

		public void Execute(ExecutionState state)
		{
			foreach (ExecutionPath ep in executionPaths)
			{
				object o = true;
				if (ep.Condition != null)
					o = ep.Condition.Evaluate(state, ep.Token);
				if (BooleanExpression.True.Equals(o))
				{
					ep.Instructions.Execute(state);
					return;
				}
			}
		}

		private class ExecutionPath
		{
			public IExpression Condition;
			public InstructionList Instructions;
			public Token Token;

			public ExecutionPath(IExpression condition, InstructionList instructions, Token token)
			{
				Condition = condition;
				Instructions = instructions;
				Token = token;
			}
		}
	}

	public class IfInstructionCreator : IInstructionCreator
	{
		public string Keyword { get { return "if"; } }
		public IInstruction Create() { return new IfInstruction(); }
	}
}
