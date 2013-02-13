using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Web;


namespace Sprocket.Web.CMS.Script
{
	public sealed class SprocketScript
	{
		private IInstruction instruction;
		private string source;
		private ExecutionState.ScriptRecursionIdentifier identifier;
		bool hasError = false;
		TokeniserException exception = null;

		internal TokeniserException Exception
		{
			get { return exception; }
		}
		
		public bool HasParseError
		{
			get { return hasError; }
			set { hasError = value; }
		}

		public string Source
		{
			get { return source; }
		}

		public SprocketScript(string source, string descriptiveName, string scriptIdentificationString)
		{
			this.source = source;
			this.identifier = new ExecutionState.ScriptRecursionIdentifier(descriptiveName, scriptIdentificationString);

			TokenList tokens;
			try
			{
				tokens = Tokeniser.Extract(source);
			}
			catch (TokeniserException ex)
			{
				Token falseToken = new Token(source.Substring(ex.Position, 1), TokenType.FreeText, ex.Position);
				Token token = new Token(GetErrorHTML(ex.Message, falseToken, null), TokenType.FreeText, 0);
				tokens = new TokenList(new List<Token>(new Token[] { token }));
				instruction = new ShowInstruction();
				instruction.Build(tokens);
				hasError = true;
				exception = ex;
				return;
			}

			try
			{
				instruction = TokenParser.BuildInstruction(tokens);
			}
			catch (TokenParserException ex)
			{
				Token token = new Token(GetErrorHTML(ex.Message, ex.Token, null), TokenType.FreeText, 0);
				tokens = new TokenList(new List<Token>(new Token[] { token }));
				instruction = new ShowInstruction();
				instruction.Build(tokens);
				hasError = true;
			}
		}

		string GetErrorHTML(string message, Token token, ExecutionState state)
		{
			int start = Math.Max(token.Position - 150, 0);
			string source = state == null ? Source : state.ExecutingScript.Peek().Source;
			string code = source.Substring(start, token.Position - start);
			string prefix = "", suffix = "";
			string tokenValue = token.Value;
			if (token.TokenType == TokenType.QuotedString)
				tokenValue = "\"" + tokenValue + "\"";
			int start2 = start + code.Length + tokenValue.Length;
			string code2 = source.Substring(start2, Math.Min(150, source.Length - start2));

			if (start > 0) prefix = "...";
			if (source.Length - start2 > 150) suffix = "...";

			string names = "";
			if (state == null)
				names = identifier.DescriptiveName;
			else
			{
				while (state.ScriptIdentifierStack.Count > 0)
				{
					if (names.Length > 0)
						names = " &gt; " + names;
					names = state.ScriptIdentifierStack.Pop().DescriptiveName + names;
				}
			}
			return "<style>body{font-family:verdana;font-size:8pt;}</style>"
					+ "<body><h2>Couldn't understand part of the script :(</h2>"
					+ "<strong style=\"color:red\">" + HttpUtility.HtmlEncode(message) + "</strong><br/>"
					+ "The script I was processing was: <strong>" + names + "</strong><br />"
					+ "The error occurred at position " + token.Position + " in the script, at a section that looks like this:<br/><br/>"
					+ "<div style=\"padding:10px;background-color:#ffd;border:1px dotted #cca;\">"
					+ "<pre style=\"margin:0\">" + prefix + HttpUtility.HtmlEncode(code)
					+ "<span style=\"color:yellow;background-color:red;font-weight:bold;\">" + HttpUtility.HtmlEncode(tokenValue) + "</span>"
					+ HttpUtility.HtmlEncode(code2) + suffix + "</pre></div></body>";
		}

		private Dictionary<string, SprocketScript> sectionOverrides = new Dictionary<string, SprocketScript>();
		public void OverrideSection(string sectionName, SprocketScript script)
		{
			sectionOverrides[sectionName] = script;
		}

		public void Execute(Stream stream)
		{
			using (StreamWriter writer = new StreamWriter(stream))
				writer.Write(Execute());
		}

		public string Execute()
		{
			MemoryStream stream = new MemoryStream();
			ExecutionState state = new ExecutionState(stream);
			state.ScriptIdentifierStack.Push(identifier);
			state.ExecutingScript.Push(this);
			state.SectionOverrides = sectionOverrides;
			try
			{
				instruction.Execute(state);
				stream.Seek(0, SeekOrigin.Begin);
				using (StreamReader reader = new StreamReader(stream))
					return reader.ReadToEnd();
			}
			catch (InstructionExecutionException ex)
			{
				return GetErrorHTML(ex.Message, ex.Token, state);
			}
			catch (InstructionExecutionParseErrorException ex)
			{
				return ex.ToString();
			}
		}

		public string ExecuteToResolveExpression(ExecutionState baseState)
		{
			if (baseState.BaseExecutionState.ScriptIdentifierStack.Contains(identifier))
				throw new InstructionExecutionException("Script \"" + identifier.DescriptiveName + "\" aborted to prevent infinite recursion", baseState.SourceToken);
			if (hasError)
				throw new InstructionExecutionParseErrorException(Execute());

			MemoryStream stream = new MemoryStream();
			ExecutionState state = new ExecutionState(stream, baseState);
			baseState.ScriptIdentifierStack.Push(identifier);
			baseState.ExecutingScript.Push(this);
			state.SectionOverrides = sectionOverrides;

			instruction.Execute(state);

			baseState.ScriptIdentifierStack.Pop();
			baseState.ExecutingScript.Pop();

			stream.Seek(0, SeekOrigin.Begin);
			using (StreamReader reader = new StreamReader(stream))
				return reader.ReadToEnd();
		}

		/// <summary>
		/// Executes the script as a branch of execution of another script, maintaining the
		/// existing execution state. This is used exclusively for allowing certain named
		/// script sections (InstructionList objects) to execute a secondary script instead
		/// of their standard child instructions.
		/// </summary>
		/// <param name="state">The state in which to continue execution</param>
		internal void ExecuteInParentContext(ExecutionState state)
		{
			if (state.BaseExecutionState.ScriptIdentifierStack.Contains(identifier))
				throw new InstructionExecutionException("Script \"" + identifier.DescriptiveName + "\" aborted to prevent infinite recursion", state.SourceToken);
			if (hasError)
				throw new InstructionExecutionParseErrorException(Execute());

			state.BaseExecutionState.ScriptIdentifierStack.Push(identifier);
			state.BaseExecutionState.ExecutingScript.Push(this);
			
			Dictionary<string, SprocketScript> preservedOverrides = state.SectionOverrides;
			state.SectionOverrides = sectionOverrides;
			instruction.Execute(state);
			state.SectionOverrides = preservedOverrides;

			state.BaseExecutionState.ScriptIdentifierStack.Pop();
			state.BaseExecutionState.ExecutingScript.Pop();
		}
	}
}
