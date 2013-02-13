using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Sprocket.Web.CMS.Script
{
	public class TokenList
	{
		private List<Token> list;
		private int index = 0;

		public TokenList(List<Token> tokens)
		{
			list = tokens;
			list.Insert(0, new Token(InstructionList.Keyword, TokenType.Word, 0));
			list.Add(new Token("end", TokenType.Word, 0));
		}

		public Token Peek(int relativeIndex)
		{
			int n = index + relativeIndex;
			if (n < 0 || n >= list.Count)
				throw new TokenParserException("A relative index of " + relativeIndex + " equates to an absolute index of " + n + ", which is out of bounds for the current token list.", list[list.Count - 1]);
			return list[n];
		}

		public Token Current
		{
			get
			{
				if (index >= list.Count)
					return null;
				return list[index];
			}
		}

		public void Advance()
		{
			if (index == list.Count)
				throw new TokenParserException("Can't advance to the next token. The end of the token list has already been reached.", list[list.Count-1]);
			index++;
		}

		public void Advance(int numberOfTokensToAdvancePast)
		{
			index += numberOfTokensToAdvancePast;
			if (index > list.Count)
				throw new TokenParserException("Can't advance. The end of the token list has been left behind.", list[list.Count - 1]);
		}

		public Token Next
		{
			get { return index + 1 >= list.Count ? null : list[index + 1]; }
		}

		public Token Previous
		{
			get { return index == 0 ? null : list[index - 1]; }
		}

		public int TokensFollowingCurrent
		{
			get { return Math.Max(0, list.Count - 1 - index); }
		}
	}
}
