using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Sprocket.Web.CMS.Script
{
	internal class InstructionList : IInstruction
	{
		public const string Keyword = "section";
		List<IInstruction> list = new List<IInstruction>();
		private bool acceptELSEInPlaceOfEND = false, isLoopBlock = false, isStandardSection = false;
		private string name = null;
		private Token instructionListToken = null;
		private TerminatorType terminator = TerminatorType.End;

		internal TerminatorType Terminator
		{
			get { return terminator; }
		}

		public string Name
		{
			get { return name; }
		}

		public bool AcceptELSEInPlaceOfEND
		{
			get { return acceptELSEInPlaceOfEND; }
			set { acceptELSEInPlaceOfEND = value; }
		}

		public bool IsLoopBlock
		{
			get { return isLoopBlock; }
			set { isLoopBlock = value; }
		}

		public void Build(TokenList tokens)
		{
			instructionListToken = tokens.Current;
			if(isStandardSection)
				tokens.Advance(); // only advance past the section token if 

			// if the current token is the name of the instruction list, store it
			if (tokens.Current != null)
				if (tokens.Current.TokenType == TokenType.QuotedString)
				{
					name = tokens.Current.Value;
					tokens.Advance();
				}

			// read each instruction, one by one until the end of the instruction list is reached
			while (tokens.Current != null)
			{
				// if we've reached a token indicating the end of this instruction block, advance past the ending token and return
				if ((!isLoopBlock && Token.IsEnd(tokens.Current)) ||
					(isLoopBlock && Token.IsLoop(tokens.Current)) ||
					(acceptELSEInPlaceOfEND && (Token.IsElse(tokens.Current) || Token.IsElseIf(tokens.Current))))
				{
					// record the type of terminating token then advance past it
					switch (tokens.Current.Value)
					{
						case "else": terminator = TerminatorType.Else; break;
						case "elseif": terminator = TerminatorType.ElseIf; break;
						case "loop": terminator = TerminatorType.Loop; break;
					}
					tokens.Advance();
					return;
				}
				list.Add(TokenParser.BuildInstruction(tokens));
			}
		}

		public void Execute(ExecutionState state)
		{
			if (name != null)
				if (state.SectionOverrides.ContainsKey(name))
				{
					Token prevToken = state.SourceToken;
					state.SourceToken = instructionListToken;
					state.SectionOverrides[name].ExecuteInParentContext(state);
					state.SourceToken = prevToken;
					return;
				}
			foreach (IInstruction instruction in list)
				instruction.Execute(state);
		}

		public enum TerminatorType
		{
			End,
			Else,
			ElseIf,
			Loop
		}

		public InstructionList() { }
		public InstructionList(bool isStandardSection)
		{
			this.isStandardSection = isStandardSection;
		}
	}

	internal class InstructionListCreator : IInstructionCreator
	{
		public string Keyword { get { return InstructionList.Keyword; } }
		public IInstruction Create() { return new InstructionList(true); }
	}

	internal class InstructionListCreator2 : IInstructionCreator
	{
		public string Keyword { get { return "@"; } }
		public IInstruction Create() { return new InstructionList(true); }
	}
}
