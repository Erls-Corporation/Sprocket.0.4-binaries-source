using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Sprocket.Web.CMS.Script
{
	public class Token
	{
		TokenType tokenType;
		string value;
		int position;
		Token previous = null, next = null;

		public TokenType TokenType
		{
			get { return tokenType; }
		}

		public Token Next
		{
			get { return next; }
			internal set { next = value; }
		}

		public Token Previous
		{
			get { return previous; }
			internal set { previous = value; }
		}

		public string Value
		{
			get { return value; }
		}

		public int Position
		{
			get { return position; }
			internal set { position = value; }
		}

		public Token(string value, TokenType tokenType, int position)
		{
			if (tokenType == TokenType.Word)
				this.value = value.ToLower();
			else
				this.value = value;
			this.tokenType = tokenType;
			this.position = position;
		}

		public static bool IsEnd(Token token)
		{
			return token.Value == "end" || token.value == ";";
		}

		public static bool IsElse(Token token)
		{
			return token.value == "else";
		}

		public static bool IsElseIf(Token token)
		{
			return token.value == "elseif";
		}

		public static bool IsLoop(Token token)
		{
			return token.value == "loop";
		}

		public override string ToString()
		{
			return "{" + tokenType + "; " + value + "}";
		}
	}
}
