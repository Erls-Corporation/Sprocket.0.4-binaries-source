using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Sprocket.Web.CMS.Script
{
	#region Expression Interfaces

	/// <summary>
	/// This is the most base interface for all types of expressions. Its sole purpose to is to evaluate to a value
	/// when a script is executed.
	/// </summary>
	public interface IExpression
	{
		/// <summary>
		/// Requests that the expression express itself as some kind of value
		/// </summary>
		/// <param name="state">The state container for the currently-executing script</param>
		/// <returns>A value or object reference calculated by the expression</returns>
		object Evaluate(ExecutionState state, Token contextToken);
	}

	/// <summary>
	/// Required if the expression should be able to be invoked by a keyword in a script
	/// </summary>
	public interface IExpressionCreator
	{
		/// <summary>
		/// The keyword that should invoke the expression in a script
		/// </summary>
		string Keyword { get; }
		/// <summary>
		/// Create an instance of the relevant expression type
		/// </summary>
		/// <returns>An instance of the type of expression that is referenced by the associated keyword</returns>
		IExpression Create();
	}

	/// <summary>
	/// Used for expression types that have flexible syntax. e.g. operators are IFlexibleSyntaxExpressions that read an expression
	/// on the left and an expression on the right and evaluate both to produce output. Note that expressions that implement
	/// IFlexibleSyntaxExpression are expected to take care of advancing the token list past the expression keyword.
	/// </summary>
	public interface IFlexibleSyntaxExpression : IExpression
	{
		/// <summary>
		/// Allows the expression to build itself and iterate as required through the token list in
		/// order to build itself into a meaningful construct.
		/// </summary>
		/// <param name="tokens">The token list being parsed</param>
		/// <param name="precedenceStack">A stack used for building complex expressions, mostly commonly for calculating operator
		/// precedence in binary expressions.</param>
		void PrepareExpression(TokenList tokens, Stack<int?> precedenceStack);
	}

	/// <summary>
	/// Used for objects that should be able to evaluate themselves according to the supplied arguments
	/// </summary>
	public interface IArgumentListEvaluatorExpression : IExpression
	{
		/// <summary>
		/// Requests that the object evaluate itself in the context of the supplied arguments
		/// </summary>
		/// <param name="args">A list of arguments to evaluate</param>
		/// <param name="state">The current execution state</param>
		/// <returns>An object containing a value calculated in accordance with the supplied list of arguments</returns>
		object Evaluate(Token contextToken, List<ExpressionArgument> args, ExecutionState state);
	}

	/// <summary>
	/// Specifies that this expression type can evaluate itself in the context of a given property
	/// </summary>
	public interface IPropertyEvaluatorExpression : IExpression
	{
		/// <summary>
		/// Allows the expression to validate whether or not the named property is valid as a member of this object
		/// </summary>
		/// <param name="propertyName">The name of the property to validate</param>
		/// <returns>True if the property name can be evaluated, otherwise false</returns>
		bool IsValidPropertyName(string propertyName);
		/// <summary>
		/// Requests that the expression evaluate the given property in the context of the expression
		/// </summary>
		/// <param name="prop">A property to evaluate</param>
		/// <param name="state">The current execution state</param>
		/// <returns>An object containing the evaluation result</returns>
		object EvaluateProperty(string propertyName, Token token, ExecutionState state);
	}

	/// <summary>
	/// Indicates that the expression can be used in a "list each" statement.
	/// </summary>
	public interface IListExpression : IExpression
	{
		/// <summary>
		/// Retrieve an IList object that can be iterated through
		/// </summary>
		/// <param name="state">The current execution state</param>
		/// <returns>an IList object</returns>
		IList GetList(ExecutionState state);
	}

	/// <summary>
	/// Used for expressions that return a value calculated from two expressions (one either side of the expression keyword).
	/// Standard mathematical operators are good examples of binary expressions.
	/// </summary>
	public interface IBinaryExpression : IFlexibleSyntaxExpression
	{
		/// <summary>
		/// Indicates order of precedence. A higher value will cause the current expression stack to resolve
		/// </summary>
		int Precedence { get; }
		IExpression Left { get; set; }
		IExpression Right { get; }
	}
	public interface IBinaryExpressionCreator
	{
		string Keyword { get; }
		int Precedence { get; }
		IBinaryExpression Create();
	}

	#endregion

	#region Instruction Interfaces
	
	public interface IInstruction
	{
		/// <summary>
		/// Builds an instruction tree
		/// </summary>
		/// <param name="tokens">The full token list created by the Tokeniser</param>
		/// <param name="index">The index of the first token after the token that represents this instruction.
		/// For example, if this is a "show" instruction which exists at token list index 10, then index should be 11.
		/// After processing, index should be left at the first position not processed by either directly or indirectly
		/// (recursively) by this instruction</param>
		void Build(TokenList tokens);
		void Execute(ExecutionState state);
	}

	public interface IInstructionCreator
	{
		string Keyword { get; }
		IInstruction Create();
	}

	#endregion
}
