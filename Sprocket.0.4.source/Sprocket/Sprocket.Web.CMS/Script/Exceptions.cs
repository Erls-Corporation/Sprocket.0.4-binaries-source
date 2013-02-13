using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Sprocket.Web.CMS.Script
{
	public class InstructionExecutionException : Exception
	{
		private Token token;
		public Token Token
		{
			get { return token; }
		}

		public InstructionExecutionException(string message, Token token)
			: base(message)
		{
			if (token == null)
				throw new NullReferenceException("Can't throw an InstructionExecutionException without a non-null token reference.");
			this.token = token;
		}

		public InstructionExecutionException(string message, Exception ex, Token token)
			: base(message, ex)
		{
			this.token = token;
		}
	}

	public class InstructionExecutionParseErrorException : Exception
	{
		string html;
		public InstructionExecutionParseErrorException(string errorHTML)
		{
			html = errorHTML;
		}

		public override string ToString()
		{
			return html;
		}
	}

	public class TokenParserException : Exception
	{
		private Token token;

		public Token Token
		{
			get { return token; }
		}

		public TokenParserException(string message, Token token)
			: base(message)
		{
			if (token == null)
				throw new NullReferenceException("Can't throw a TokenParserException without a non-null token reference.");
			this.token = token;
		}
	}

	internal class TokeniserException : Exception
	{
		private int position;
		private string scriptSource;

		public string ScriptSource
		{
			get { return scriptSource; }
		}

		public int Position
		{
			get { return position; }
			internal set { position = value; }
		}

		public TokeniserException(string message, int position, string source)
			: base(message)
		{
			this.position = position;
			this.scriptSource = source;
		}
	}
}
