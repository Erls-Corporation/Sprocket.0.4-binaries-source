using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Sprocket.Web.CMS.Script
{
	public class SetInstruction : IInstruction
	{
		private IExpression expr = null;
		Token varNameToken = null;
		public void Build(TokenList tokens)
		{
			// read "set varname to"
			tokens.Advance();
			varNameToken = tokens.Current;
			tokens.Advance();
			Token toToken = tokens.Current;
			tokens.Advance();

			// make sure the variable name token is a valid word/name for a variable
			if (varNameToken.TokenType != TokenType.Word)
				throw new TokenParserException("The \"set\" instruction must be followed by a word of your choice that will be subsequently used to hold some value.", tokens.Previous);
			if (TokenParser.IsReservedWord(varNameToken.Value))
				throw new TokenParserException("You can't use \"" + varNameToken.Value + "\" as a variable name. It is the keyword for either an expression or an instruction.", varNameToken);

			// make sure the "to" token is actually the word "to"
			if (toToken.TokenType != TokenType.Word || toToken.Value != "to")
				throw new TokenParserException("The \"set\" instruction must be formatted like this: \"set something to some_expression\"", toToken);

			// build the expression that will be evaluated and assigned to the variable at run time
			expr = TokenParser.BuildExpression(tokens);
		}

		public void Execute(ExecutionState state)
		{
			state.Variables[varNameToken.Value] = expr.Evaluate(state, varNameToken);
		}
	}

	public class SetInstructionCreator : IInstructionCreator
	{
		public string Keyword { get { return "set"; } }
		public IInstruction Create() { return new SetInstruction(); }
	}
}
