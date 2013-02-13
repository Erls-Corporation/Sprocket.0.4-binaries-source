using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Sprocket.Web.CMS.Script
{
	public class ListEachInstruction : IInstruction
	{
		private InstructionList instructions = null;
		private IExpression expr = null;
		private Token token = null, iteratorToken = null, listToken = null;

		public void Build(TokenList tokens)
		{
			// store the "list each [variable] in [list_expression]" tokens
			token = tokens.Current;
			Token eachToken = tokens.Peek(1);
			iteratorToken = tokens.Peek(2);
			Token inToken = tokens.Peek(3);
			listToken = tokens.Peek(4); // the first token of the list expression
			tokens.Advance(4);

			// make sure the "each" token is actually the word "each"
			if (eachToken.Value != "each" || eachToken.TokenType != TokenType.Word)
				throw new TokenParserException("\"list\" must be followed by the word \"each\".", eachToken);

			// validate the various tokens
			if (iteratorToken.TokenType != TokenType.Word)
				throw new TokenParserException("You must specify a word here that can be used as a variable, e.g. \"list each item in whatever\"", iteratorToken);
			if (TokenParser.IsReservedWord(iteratorToken.Value))
				throw new TokenParserException("You can't use \"" + iteratorToken.Value + "\" as a variable name. It is the keyword for either an expression or an instruction.", iteratorToken);
			if (inToken.Value != "in" || inToken.TokenType != TokenType.Word)
				throw new TokenParserException("\"list each [something] must be followed by the word \"in\".", inToken);

			// build the list expression and the subsequent instruction list to loop through
			expr = TokenParser.BuildExpression(tokens);
			instructions = new InstructionList();
			instructions.IsLoopBlock = true;
			instructions.Build(tokens);
		}

		public void Execute(ExecutionState state)
		{
			IList list;
			if (expr is IListExpression)
				list = ((IListExpression)expr).GetList(state);
			else
			{
				list = expr.Evaluate(state, listToken) as IList;
				if (list == null)
					throw new InstructionExecutionException("\"" + listToken.Value + "\" doesn't contain a list of anything.", listToken);
			}

			if (state.HasVariable(iteratorToken.Value))
				throw new InstructionExecutionException("\"" + iteratorToken.Value + "\" already equates to something else. You should use a different word.", iteratorToken);
			foreach (object item in list)
			{
				state.Variables[iteratorToken.Value] = item;
				instructions.Execute(state);
			}
			state.Variables.Remove(iteratorToken.Value);
		}
	}

	public class ListEachInstructionCreator : IInstructionCreator
	{
		public string Keyword { get { return "list"; } }
		public IInstruction Create() { return new ListEachInstruction(); }
	}
}
