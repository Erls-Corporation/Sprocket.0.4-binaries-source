using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Web;


namespace Sprocket.Web.CMS.Script
{
	/// <summary>
	/// This class maintains state information during script execution. Often there will be a case where a script
	/// causes another script to be executed. This can happen two ways. (a) The script can pass program flow to
	/// the secondary script by calling that script's Execute method and passing this ExecutionState object so that
	/// the secondary script will continue to write to the same stream and perform correct recursion checking.
	/// (b) The script can execute the secondary script independently in order to resolve the value of an expression.
	/// When this happens, the secondary script will need to use its own internal ExecutionState object as it will be
	/// writing a new temporary stream that will be used as the expression value in the primary script. The problem
	/// involved with such a scenario is that the child script can then cause infinite recursion as it has its own
	/// ScriptNameStack, and thus entries in the primary stack won't be considered. To avoid this, when calling a
	/// secondary script in order to resolve an expression[to do]
	/// </summary>
	public class ExecutionState
	{
		private Stack<ScriptRecursionIdentifier> scriptIdentifierStack = new Stack<ScriptRecursionIdentifier>();
		/// <summary>
		/// Used for recursion checking. Scripts are not passed parameters. They act as independent output generators
		/// and thus if one either directly or indirectly calls itself, an infinite recursion loop is almost guaranteed.
		/// Use this parameter to check whether or not the script is already on the stack and thus likely to cause
		/// infinite recursion. Note: ALWAYS call this in the context of BaseExecutionState, never directly.
		/// </summary>
		public Stack<ScriptRecursionIdentifier> ScriptIdentifierStack
		{
			get { return scriptIdentifierStack; }
		}

		public class ScriptRecursionIdentifier
		{
			private string descriptiveName, identificationString;

			public string DescriptiveName
			{
				get { return descriptiveName; }
			}

			public string IdentificationString
			{
				get { return identificationString; }
			}

			public ScriptRecursionIdentifier(string descriptiveName, string identificationString)
			{
				this.descriptiveName = descriptiveName;
				this.identificationString = identificationString;
			}

			public override bool Equals(object obj)
			{
				return obj.ToString() == identificationString;
			}

			public override string ToString()
			{
				return identificationString;
			}
			public override int GetHashCode()
			{
				return identificationString.GetHashCode();
			}
		}

		private Stack<SprocketScript> executingScript = new Stack<SprocketScript>();
		/// <summary>
		/// Not used for recursion checking. This used simply to store which script is currently executing so that
		/// when an error is thrown, the exception handler can get the erroneous source code from the correct object.
		/// Note: ALWAYS call this in the context of BaseExecutionState, never directly.
		/// </summary>
		public Stack<SprocketScript> ExecutingScript
		{
			get { return executingScript; }
		}

		private StreamWriter output;
		public StreamWriter Output
		{
			get { return output; }
		}

		public ExecutionState(Stream stream)
		{
			output = new StreamWriter(stream);
			output.AutoFlush = true;
			baseExecutionState = this;
		}

		public ExecutionState(Stream stream, ExecutionState state)
		{
			output = new StreamWriter(stream);
			output.AutoFlush = true;
			baseExecutionState = state;
		}

		private Dictionary<string, SprocketScript> sectionOverrides = new Dictionary<string, SprocketScript>();
		public Dictionary<string, SprocketScript> SectionOverrides
		{
			get { return sectionOverrides; }
			set { sectionOverrides = value; }
		}

		private ExecutionState baseExecutionState;
		public ExecutionState BaseExecutionState
		{
			get { return baseExecutionState; }
		}

		private Token sourceToken = null;
		public Token SourceToken
		{
			get { return sourceToken; }
			set { sourceToken = value; }
		}

		private Dictionary<string, object> variables = new Dictionary<string, object>();
		public Dictionary<string, object> Variables
		{
			get { return variables; }
		}

		public bool HasVariable(string variableName)
		{
			ExecutionState state = this;
			while (state != null)
			{
				if (state.Variables.ContainsKey(variableName))
					return true;
				if (object.ReferenceEquals(state.baseExecutionState, state))
					return false;
				state = state.baseExecutionState;
			}
			return false;
		}

		public object GetVariable(string variableName)
		{
			ExecutionState state = this;
			while (state != null)
			{
				if (state.Variables.ContainsKey(variableName))
					return state.Variables[variableName];
				if (object.ReferenceEquals(state.baseExecutionState, state))
					return null;
				state = state.baseExecutionState;
			}
			return null;
		}
	}
}
