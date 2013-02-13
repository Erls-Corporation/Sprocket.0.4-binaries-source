using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

using Sprocket;
using Sprocket.Web;
using Sprocket.Web.CMS;
using Sprocket.Security;
using Sprocket.Web.CMS.Content;
using Sprocket.Web.CMS.Script;

namespace Sprocket.Web.Forums
{
	public class NewForumExpression : IArgumentListEvaluatorExpression, IFlexibleSyntaxExpression
	{
		public object Evaluate(Token contextToken, List<ExpressionArgument> args, ExecutionState state)
		{
			if (args.Count != 1)
				throw new InstructionExecutionException("An argument was expected specifying which category code to use to load.", contextToken);
			object o = args[0].Expression.Evaluate(state, args[0].Token);
			if(o == null)
				throw new InstructionExecutionException("The specified argument equates to null. It needs to equate to the forum's category code.", args[0].Token);
			ForumCategory cat = ForumHandler.DataLayer.SelectForumCategoryByCode(o.ToString());
			if(cat == null)
				throw new InstructionExecutionException("The specified argument equates to \"" + o + "\", which is not an existing category code.", args[0].Token);
			Forum forum = new Forum(0, cat.ForumCategoryID, "", "Untitled Forum", "", "", SprocketDate.Now, null,
				Forum.AccessType.ActivatedMembers, Forum.AccessType.ActivatedMembers, Forum.AccessType.AllMembers, null, null, null, null,
				Forum.MarkupType.None, true, true, false, false, false, Forum.DisplayOrderType.TopicLastMessageDate, false);
			return forum;
		}

		public object Evaluate(ExecutionState state, Token contextToken)
		{
			throw new InstructionExecutionException("You must specify an argument representing the category code for the new forum.", contextToken);
		}

		public void PrepareExpression(TokenList tokens, Stack<int?> precedenceStack)
		{
			if(tokens.Next.TokenType != TokenType.GroupStart)
				throw new TokenParserException("You must specify an argument representing the category code for the new forum.", tokens.Current);
			tokens.Advance();
		}
	}
	public class NewForumExpressionCreator : IExpressionCreator
	{
		public string Keyword { get { return "newforum"; } }
		public IExpression Create() { return new NewForumExpression(); }
	}
}
