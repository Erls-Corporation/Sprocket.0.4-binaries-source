using System;
using System.Collections.Generic;
using System.Text;
using Sprocket.Web.CMS.Script;


namespace Sprocket.Web.CMS.Content.Expressions
{
	public class BasePathExpression : IPropertyEvaluatorExpression
	{
		public object Evaluate(ExecutionState state, Token contextToken) { return WebUtility.BasePath; }

		public bool IsValidPropertyName(string propertyName)
		{
			return propertyName == "length";
		}

		public object EvaluateProperty(string propertyName, Token token, ExecutionState state)
		{
			return WebUtility.BasePath.Length;
		}
	}
	public class BasePathExpressionCreator : IExpressionCreator
	{
		public string Keyword { get { return "basepath"; } }
		public IExpression Create() { return new BasePathExpression(); }
	}

	public class SprocketPathExpression : IArgumentListEvaluatorExpression, IPropertyEvaluatorExpression
	{
		public object Evaluate(ExecutionState state, Token contextToken)
		{
			return SprocketPath.Value;
		}

		public object Evaluate(Token contextToken, List<ExpressionArgument> args, ExecutionState state)
		{
			switch (args.Count)
			{
				case 0:
					return SprocketPath.Value;

				case 1:
					{
						object o = TokenParser.VerifyUnderlyingType(args[0].Expression.Evaluate(state, args[0].Token));
						if (o is decimal)
						{
							int n = Convert.ToInt32(o);
							if (n >= SprocketPath.Sections.Length)
								throw new InstructionExecutionException("The argument you specified for the \"path\" expression equates to the number " + n + ", which is too high. Remember, the first component in the path is index zero (0). The second is one (1), and so forth.", args[0].Token);
							return SprocketPath.Sections[n];
						}
						throw new InstructionExecutionException("You specified an argument for the \"path\" expression, but it isn't equating to a number. I need a number to know which bit of the path you are referring to.", args[0].Token);
					}

				default:
					throw new TokenParserException("the \"path\" expression must contain no more than one argument, and it should be a number specifying which querystring element you want, or a string (word) specifying the name of the querystring parameter you want.", args[1].Token);
			}			
		}

		public bool IsValidPropertyName(string propertyName)
		{
			switch (propertyName)
			{
				case "sectioncount":
				case "length":
					return true;
				default:
					return false;
			}
		}

		public object EvaluateProperty(string propertyName, Token token, ExecutionState state)
		{
			switch (propertyName)
			{
				case "sectioncount":
					return SprocketPath.Sections.Length;

				case "length":
					return SprocketPath.Value.Length;

				default:
					return VariableExpression.InvalidProperty;
			}
		}
	}
	public class SprocketPathExpressionCreator : IExpressionCreator
	{
		public string Keyword { get { return "path"; } }
		public IExpression Create() { return new SprocketPathExpression(); }
	}

	public class DescendentPathExpression : IArgumentListEvaluatorExpression, IPropertyEvaluatorExpression
	{
		public object Evaluate(ExecutionState state, Token contextToken)
		{
			string descendentPath;
			string[] sections;
			GetDescendentPath(out descendentPath, out sections);
				sections = new string[0];
			return descendentPath;
		}

		private void GetDescendentPath(out string descendentPath, out string[] sections)
		{
			if (CurrentRequest.Value["DescendentPathExpression.Evaluate.Sections"] == null)
			{
				if (ContentManager.RequestedPage == null)
				{
					descendentPath = SprocketPath.Value;
					sections = SprocketPath.Sections;
				}
				else
				{
					descendentPath = SprocketPath.GetDescendentPath(ContentManager.RequestedPage.Path);
					sections = descendentPath.Split('/');
				}
				if (descendentPath == "")
					sections = new string[0];
				CurrentRequest.Value["DescendentPathExpression.Evaluate.Sections"] = sections;
				CurrentRequest.Value["DescendentPathExpression.Evaluate.Path"] = descendentPath;
			}
			else
			{
				sections = (string[])CurrentRequest.Value["DescendentPathExpression.Evaluate.Sections"];
				descendentPath = (string)CurrentRequest.Value["DescendentPathExpression.Evaluate.Path"];
			}
		}

		public object Evaluate(Token contextToken, List<ExpressionArgument> args, ExecutionState state)
		{
			string descendentPath;
			string[] sections;
			GetDescendentPath(out descendentPath, out sections);
			switch (args.Count)
			{
				case 0:
					return descendentPath;

				case 1:
					{
						object o = TokenParser.VerifyUnderlyingType(args[0].Expression.Evaluate(state, args[0].Token));
						if (o is decimal)
						{
							int n = Convert.ToInt32(o);
							if (n >= sections.Length)
								throw new InstructionExecutionException("The argument you specified for the \"descendentpath\" expression equates to the number " + n + ", which is too high. Remember, the first component in the path is index zero (0). The second is one (1), and so forth.", args[0].Token);
							return sections[n];
						}
						throw new InstructionExecutionException("You specified an argument for the \"descendentpath\" expression, but it isn't equating to a number. I need a number to know which bit of the path you are referring to.", args[0].Token);
					}

				default:
					throw new TokenParserException("the \"descendentpath\" expression must contain no more than one argument, and it should be a number specifying which querystring element you want, or a string (word) specifying the name of the querystring parameter you want.", args[1].Token);
			}
		}

		public bool IsValidPropertyName(string propertyName)
		{
			switch (propertyName)
			{
				case "sectioncount":
				case "length":
					return true;
				default:
					return false;
			}
		}

		public object EvaluateProperty(string propertyName, Token token, ExecutionState state)
		{
			string descendentPath;
			string[] sections;
			GetDescendentPath(out descendentPath, out sections);
			switch (propertyName)
			{
				case "sectioncount":
					return sections.Length;

				case "length":
					return descendentPath.Length;

				default:
					return VariableExpression.InvalidProperty;
			}
		}
	}

	public class DescendentPathExpressionCreator : IExpressionCreator
	{
		public string Keyword { get { return "descendentpath"; } }
		public IExpression Create() { return new DescendentPathExpression(); }
	}
}
