using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Sprocket.Web.CMS.Script
{
	internal static class Tokeniser
	{
		public static TokenList Extract(string source)
		{

			List<Token> tokens = new List<Token>();

			if (source == null)
				return new TokenList(tokens);

			string sourceExpr = @"\{\?\s*(?<expr>(([^\{\}]*)|((?<=\\)[\{\}]))*)\s*\}";
			MatchCollection matches = Regex.Matches(source, sourceExpr, RegexOptions.Multiline | RegexOptions.ExplicitCapture);
			if (matches.Count == 0)
			{
				Token htmlToken = new Token(source, TokenType.FreeText, 0);
				AddToken(tokens, htmlToken);
			}
			else
			{
				int previousMatchEndPosition = 0;
				for (int i = 0; i < matches.Count; i++)
				{
					Match match = matches[i];
					int htmlLength = match.Index - previousMatchEndPosition;
					if (htmlLength > 0)
					{
						Token htmlToken = new Token(source.Substring(previousMatchEndPosition, htmlLength), TokenType.FreeText, previousMatchEndPosition);
						AddToken(tokens, htmlToken);
					}
					try
					{
						previousMatchEndPosition = match.Index + match.Length;
						string str = match.Groups["expr"].Value;
						int index = 0;

						while (index < str.Length)
						{
							Token token = ReadToken(ref str, ref index);
							if (token != null)
							{
								token.Position += match.Groups["expr"].Index;
								AddToken(tokens, token);
							}
						}
					}
					catch (TokeniserException ex)
					{
						ex.Position += match.Groups["expr"].Index;
						throw ex;
						//int start = Math.Max(ex.Position - 100, 0);
						//string err = Environment.NewLine
						//    + "ERROR PARSING SPROCKETSCRIPT\r\n"
						//    + "Position " + ex.Position + ": " + ex.Message + Environment.NewLine
						//    + "Source: " + ex.ScriptSource.Substring(start, ex.Position - start) + "[ERROR CODE...]\r\n\r\n";
						//tokens.Clear();
						//Token token = new Token(err, TokenType.StringLiteral, 0);
						//token.IsNonScriptText = true;
						//tokens.Add(token);
						//return tokens;
					}
				}
				if (previousMatchEndPosition < source.Length)
				{
					Token htmlToken = new Token(source.Substring(previousMatchEndPosition), TokenType.FreeText, previousMatchEndPosition);
					AddToken(tokens, htmlToken);
				}
			}

			return new TokenList(tokens);
		}

		private static void AddToken(List<Token> list, Token token)
		{
			list.Add(token);
			if (list.Count == 1) return;
			Token p = list[list.Count - 2];
			p.Next = token;
			token.Previous = p;
		}

		private static bool IsOtherSymbolic(char ch)
		{
			switch (ch)
			{
				case '+':
				case '-':
				case '*':
				case '/':
				case '=':
				case '>':
				case '<':
				case '!':
				case '@':
				case '#':
				case '$':
				case '%':
				case '^':
				case '&':
				case ';':
				case '?':
				case ',':
					return true;

				default:
					return false;
			}
		}

		private delegate Token TokenReaderMethod(ref string source, ref int index);

		private static Token ReadToken(ref string source, ref int index)
		{
			TokenReaderMethod method;
			char ch = source[index];
			if (ch == '~')
				method = ReadComment;
			else if (char.IsDigit(ch) || ch == '.')
				method = ReadNumber;
			else if (char.IsLetter(ch) || ch == '_')
				method = ReadWord;
			else if (ch == '(')
				method = ReadGroupStart;
			else if (ch == ':')
				method = ReadPropertyDesignator;
			else if (ch == ')')
				method = ReadGroupEnd;
			else if (char.IsWhiteSpace(ch))
				method = ReadWhiteSpace;
			else if (IsOtherSymbolic(ch))
				method = ReadSymbolic;
			else if (ch == '"')
				method = ReadStringLiteral;
			else
				throw new TokeniserException("Unexpected character found", index, source);
			return method(ref source, ref index);
		}

		private static Token ReadComment(ref string source, ref int index)
		{
			index++;
			while (source[index++] != '~')
				if(index == source.Length)
					throw new TokeniserException("Script block ended before comment block ended", index, source);
			return null;
		}

		private static Token ReadNumber(ref string source, ref int index)
		{
			int start = index;
			bool decimalPointAdded = false;
			bool addLeadingZero = false, addTrailingZero = true;
			while (index < source.Length)
			{
				if (source[index] == '.')
				{
					if (decimalPointAdded)
						throw new TokeniserException("Duplicate decimal point found in number", index, source);
					if (start == index)
						addLeadingZero = true;
					decimalPointAdded = true;
					index++;
					continue;
				}
				else if (!char.IsDigit(source[index]))
					break;
				index++;
				addTrailingZero = false;
			}
			return new Token((addLeadingZero ? "0" : "")
				+ source.Substring(start, index - start)
				+ (addTrailingZero ? "0" : ""), TokenType.Number, start);
		}
		private static Token ReadSymbolic(ref string source, ref int index)
		{
			int start = index;
			while (IsOtherSymbolic(source[index]))
			{
				index++;
				if (index == source.Length)
					break;
			}
			return new Token(source.Substring(start, index - start), TokenType.OtherSymbolic, start);
		}
		private static Token ReadWord(ref string source, ref int index)
		{
			int start = index;
			index++;
			while (char.IsLetterOrDigit(source[index]) || source[index] == '_' || source[index] == '\'')
			{
				index++;
				if (index == source.Length)
					break;
			}
			return new Token(source.Substring(start, index - start), TokenType.Word, start);
		}
		private static Token ReadPropertyDesignator(ref string source, ref int index)
		{
			return new Token(":", TokenType.PropertyDesignator, index++);
		}
		private static Token ReadGroupStart(ref string source, ref int index)
		{
			return new Token("(", TokenType.GroupStart, index++);
		}
		private static Token ReadGroupEnd(ref string source, ref int index)
		{
			return new Token(")", TokenType.GroupEnd, index++);
		}
		private static Token ReadWhiteSpace(ref string source, ref int index)
		{
			while (char.IsWhiteSpace(source[index]))
			{
				index++;
				if (index == source.Length)
					break;
			}
			return null;
		}
		private static Token ReadStringLiteral(ref string source, ref int index)
		{
			bool isEscaped = false;
			int start = index;
			index++;
			StringBuilder sb = new StringBuilder();
			while (index < source.Length)
			{
				switch (source[index])
				{
					case '"':
						if (!isEscaped)
						{
							index++;
							return new Token(sb.ToString(), TokenType.QuotedString, start);
						}
						break;

					case '\\':
						if (!isEscaped)
						{
							isEscaped = true;
							index++;
							continue;
						}
						break;
				}
				sb.Append(source[index++]);
				isEscaped = false;
			}
			throw new TokeniserException("String literal missing a closing quote (\") character", index, source);
		}
	}
}
