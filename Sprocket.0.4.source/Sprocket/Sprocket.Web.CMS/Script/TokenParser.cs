using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Sprocket.Web.CMS.Script
{
	public static class TokenParser
	{
		private static Dictionary<string, IInstructionCreator> instructionCreators;
		private static Dictionary<string, IBinaryExpressionCreator> binaryExpressionCreators;
		private static Dictionary<string, IExpressionCreator> expressionCreators;

		/// <summary>
		/// Determines if the word has been reserved for use as the keyword for an expression or instruction.
		/// </summary>
		/// <param name="word">A word to check</param>
		/// <returns>True if the word is reserved by an existing instruction or expression, otherwise False</returns>
		public static bool IsReservedWord(string word)
		{
			if (instructionCreators.ContainsKey(word)) return true;
			if (binaryExpressionCreators.ContainsKey(word)) return true;
			if (expressionCreators.ContainsKey(word)) return true;
			return false;
		}

		static TokenParser()
		{
			instructionCreators = new Dictionary<string, IInstructionCreator>();
			binaryExpressionCreators = new Dictionary<string, IBinaryExpressionCreator>();
			expressionCreators = new Dictionary<string, IExpressionCreator>();

			foreach (Type t in Core.Modules.GetInterfaceImplementations(typeof(IInstructionCreator)))
			{
				IInstructionCreator ic = (IInstructionCreator)Activator.CreateInstance(t);
				instructionCreators.Add(ic.Keyword.ToLower(), ic);
			}

			foreach (Type t in Core.Modules.GetInterfaceImplementations(typeof(IBinaryExpressionCreator)))
			{
				IBinaryExpressionCreator bxc = (IBinaryExpressionCreator)Activator.CreateInstance(t);
				binaryExpressionCreators.Add(bxc.Keyword.ToLower(), bxc);
			}

			foreach (Type t in Core.Modules.GetInterfaceImplementations(typeof(IExpressionCreator)))
			{
				IExpressionCreator xc = (IExpressionCreator)Activator.CreateInstance(t);
				expressionCreators.Add(xc.Keyword.ToLower(), xc);
			}
		}

		public static IInstruction BuildInstruction(TokenList tokens)
		{
			Token token = tokens.Current;

			// special case: if free text is found, create a "show" instruction to process it
			if (token.TokenType == TokenType.FreeText)
				return new ShowInstruction(BuildExpression(tokens));

			// find the relevant creator/processor for the instruction
			if (token.TokenType == TokenType.Word || token.TokenType == TokenType.OtherSymbolic)
			{
				if (instructionCreators.ContainsKey(token.Value))
				{
					IInstruction instruction = instructionCreators[token.Value].Create();
					instruction.Build(tokens);
					return instruction;
				}
			}

			if (token.TokenType == TokenType.GroupStart || token.TokenType == TokenType.GroupEnd)
				throw new TokenParserException("Not sure why there is a bracket here.", token);

			if (token.Value == "end" && token.TokenType == TokenType.Word && tokens.Next == null)
				throw new TokenParserException("The end of the script has been reached prematurely.", tokens.Peek(-1));

			if (expressionCreators.ContainsKey(token.Value))
				throw new TokenParserException("\"" + token.Value + "\" can't stand by itself, as it is only designed to equate to a value. If you want to display the value, precede the keyword with a \"show\" instruction.", token);

			throw new TokenParserException("I have no idea what \"" + token.Value + "\" means.", token);
		}

		public static IExpression BuildExpression(TokenList tokens)
		{
			Stack<int?> stack = new Stack<int?>();
			stack.Push(null);
			return BuildExpression(tokens, stack);
		}

		public static IExpression BuildExpression(TokenList tokens, Stack<int?> precedenceStack)
		{
			if (tokens.Current == null)
				throw new TokenParserException("The script seems to have ended prematurely. Shouldn't there be something here?", tokens.Peek(-2));

			Token token = tokens.Current;
			IExpression expr = null;
			bool endGroupedExpression = false;
			switch (token.TokenType)
			{
				case TokenType.Number:
					expr = new NumericExpression(tokens, precedenceStack);
					break;

				case TokenType.QuotedString:
					expr = new StringExpression(tokens, precedenceStack);
					break;

				case TokenType.FreeText:
					expr = new StringExpression(tokens, precedenceStack);
					return expr;

				case TokenType.Word:
					if (expressionCreators.ContainsKey(token.Value))
					{
						// find the appropriate handler for this keyword
						expr = expressionCreators[token.Value].Create();
					}
					else // we don't recognise the word, so we can assume it's a variable and validate it at run-time
					{
						// don't allow instruction keywords to be used as variable names
						if (instructionCreators.ContainsKey(token.Value))
							throw new TokenParserException("This word is the name of an instruction. You can't use it here in this context.", token);
						else
							expr = new VariableExpression();
					}

					// if applicable, have the expression prepare itself according to its own rules
					if (expr is IFlexibleSyntaxExpression)
						((IFlexibleSyntaxExpression)expr).PrepareExpression(tokens, precedenceStack);
					else // otherwise just advance to the next token
						tokens.Advance();

					// chain together any properties and argument lists
					expr = BuildDeepExpression(expr, tokens);

					//// if the expression can take an argument list, supply it with the trailing argument list if it has been supplied
					//if (expr is IArgumentListEvaluatorExpression)
					//{
					//    List<ExpressionArgument> args = BuildArgumentList(tokens);
					//    if (args.Count > 0)
					//        expr = new ArgumentsOfExpression((IArgumentListEvaluatorExpression)expr, args, tokens, token);
					//}

					//// if the expression can have properties, check for a property designator and if it exists, build a property chain
					//if (expr is IPropertyEvaluatorExpression)
					//{
					//    ExpressionProperty ep = BuildExpressionProperty((IPropertyEvaluatorExpression)expr, tokens);
					//    while (ep != null)
					//    {
					//        Token t = tokens.TokensFollowingCurrent >= 2 ? tokens.Peek(2) : null;
					//        expr = new PropertyOfExpression((IPropertyEvaluatorExpression)expr, ep, t);
					//        if (!(expr is IPropertyEvaluatorExpression))
					//            break;

					//    }
					//}
					//else if (tokens.Current.TokenType == TokenType.PropertyDesignator)
					//    goto case TokenType.PropertyDesignator;
					break;

				case TokenType.PropertyDesignator:
					// property designators are handled elsewhere. if we found one here, it's a parsing error.
					if (tokens.Previous.TokenType == TokenType.Word)
						throw new TokenParserException("This type of expression doesn't allow you to specify a property.", tokens.Next);
					throw new TokenParserException("You've got a property designator in a spot where it doesn't belong.", tokens.Next);

				case TokenType.GroupStart:
					expr = BuildGroupedExpression(tokens);
					break;

				case TokenType.GroupEnd:
					endGroupedExpression = true;
					break;

				default:
					throw new TokenParserException("This part of the script should equate to a value but instead I got \"" + token.Value + "\", which doesn't really mean anything in this context.", token);
			}

			if (!endGroupedExpression)
			{
				int? precedence = precedenceStack.Peek();
				while (NextHasGreaterPrecedence(precedence, tokens))
					expr = BuildBinaryExpression(tokens, expr, precedenceStack);
			}
			return expr;
		}

		public static IExpression BuildGroupedExpression(TokenList tokens)
		{
			// advance past the opening bracket
			tokens.Advance();

			//build the expression
			IExpression expr = BuildExpression(tokens);
			if (tokens.Current.TokenType != TokenType.GroupEnd)
			{
				string tokenval = tokens.Current.Value.Trim();
				if (tokenval == "")
					tokenval = "#";
				throw new TokenParserException("I think a closing bracket should be here. Did you forget to put it in?", new Token(tokenval, TokenType.Word, tokens.Current.Position));
			}

			//advance past the closing bracket
			tokens.Advance();
			return expr;
		}

		public static IExpression BuildDeepExpression(IExpression expr, TokenList tokens)
		{
			if (tokens.Current.TokenType == TokenType.GroupStart)
			{
				Token t = tokens.Current;
				ArgumentsOfExpression ax = new ArgumentsOfExpression(expr, t, BuildArgumentList(tokens));
				return BuildDeepExpression(ax, tokens);
			}
			else if (tokens.Current.TokenType == TokenType.PropertyDesignator)
			{
				tokens.Advance(); // past the property designator
				if (tokens.Current.TokenType != TokenType.Word)
					throw new TokenParserException("This bit should be a word naming which property of thing thing before it that you are trying to evaluate.", tokens.Current);
				PropertyOfExpression px = new PropertyOfExpression(expr, tokens.Current);
				tokens.Advance(); // past the property name
				return BuildDeepExpression(px, tokens);
			}
			else
				return expr;
			// if args exist
				// make a new argexpr
				// set argexpr.args to the arg list
				// set argexpr.expr to expr
				// return BuildDeepExpression(argexpr, tokens)
			// else if property designator exists
				// make a new propexpr
				// set propexpr.name to the name
				// set propexpr.expr to expr
				// return BuildDeepExpression(propexpr, tokens)
			// else
				// return expr
		}

		public static List<ExpressionArgument> BuildArgumentList(TokenList tokens)
		{
			List<ExpressionArgument> args = new List<ExpressionArgument>();
			if (tokens.Current.TokenType != TokenType.GroupStart)
				return args;

			tokens.Advance(); // past the opening bracket

			while (tokens.Current.TokenType != TokenType.GroupEnd)
			{
				Token token = tokens.Current;
				IExpression expr = BuildExpression(tokens);
				if (tokens.Next == null)
					throw new TokenParserException("Oops, looks like someone didn't finish writing the script. It ended while I was putting together a list of arguments for a function call.", tokens.Peek(-2));
				args.Add(new ExpressionArgument(expr, token));
				token = tokens.Current;
				if (token.TokenType == TokenType.GroupEnd)
					continue;
				if (token.TokenType == TokenType.OtherSymbolic && token.Value == ",")
				{
					tokens.Advance();
					if (tokens.Current.TokenType == TokenType.GroupEnd)
						throw new TokenParserException("There is a comma here to indicate that I should expect an argument following it, but instead there is a closing bracket.", tokens.Current);
				}
				else
					throw new TokenParserException("The list of function arguments needs to either end with a closing bracket, or have a comma to indicate that another argument is next.", token);
			}

			tokens.Advance();
			return args;
		}

		public static IExpression BuildBinaryExpression(TokenList tokens, IExpression leftExpr, Stack<int?> precedenceStack)
		{
			IBinaryExpressionCreator bxc = binaryExpressionCreators[tokens.Current.Value];
			IBinaryExpression bx = bxc.Create();
			bx.Left = leftExpr;
			precedenceStack.Push(bx.Precedence);
			bx.PrepareExpression(tokens, precedenceStack);
			precedenceStack.Pop();
			return bx;
		}

		public static bool NextHasGreaterPrecedence(int? thanPrecedence, TokenList tokens)
		{
			if (tokens.Current == null)
				return false;
			Token token = tokens.Current;
			if (token.TokenType != TokenType.OtherSymbolic && token.TokenType != TokenType.Word)
				return false;
			if (!binaryExpressionCreators.ContainsKey(token.Value))
				return false;
			if (thanPrecedence == null) // previous check must come before this one or we'll get a default true even if the operator (e.g. "@") isn't defined as a standard binary expression
				return true;
			return binaryExpressionCreators[token.Value].Precedence < thanPrecedence.Value;
		}

		public static object VerifyUnderlyingType(object o)
		{
			if (o == null)
				return new BooleanExpression.SoftBoolean(false);
			if (o is int || o is short || o is long || o is float || o is double || o is ushort || o is ulong || o is uint)
				return Convert.ToDecimal(o);
			return o;
		}
	}
}
