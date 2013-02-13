using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Sprocket.Web.CMS.Script
{
	public class WhileInstruction : IInstruction
	{
		private InstructionList list = null;
		private IExpression expr = null;
		private Token token = null;

		public void Build(TokenList tokens)
		{
			//store the "while" token
			token = tokens.Current;
			tokens.Advance();

			// build the expression to evaluate for each iteration through the loop
			expr = TokenParser.BuildExpression(tokens);

			// build the list of instructions to execute inside the loop
			list = new InstructionList();
			list.IsLoopBlock = true;
			list.Build(tokens);
		}

		public void Execute(ExecutionState state)
		{
			DateTime start = DateTime.Now;
			DateTime stop = start.AddSeconds(15);
			while (true)
			{
				if (DateTime.Now > stop)
					throw new InstructionExecutionException("I have stopped the \"while\" loop because more than 15 seconds has passed, which means something has likely gone wrong.", token);
				object val = expr.Evaluate(state, token);
				if (val == null)
					break;
				if (BooleanExpression.True.Equals(val))
					list.Execute(state);
				else
					break;
			}
		}
	}

	public class WhileInstructionCreator : IInstructionCreator
	{
		public string Keyword { get { return "while"; } }
		public IInstruction Create() { return new WhileInstruction(); }
	}
}
