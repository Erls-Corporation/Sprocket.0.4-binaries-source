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
	public class GetForumCategoryExpression : IArgumentListEvaluatorExpression, IFlexibleSyntaxExpression
	{
		public object Evaluate(Token contextToken, List<ExpressionArgument> args, ExecutionState state)
		{
			if (args.Count != 1)
				throw new InstructionExecutionException("An argument was expected specifying which forum to load.", contextToken);
			object o = args[0].Expression.Evaluate(state, args[0].Token);
			if(o == null)
				throw new InstructionExecutionException("The specified argument equates to null. It needs to equate to the forum's code name.", args[0].Token);
			Forum forum = ForumHandler.DataLayer.SelectForumByURLToken(o.ToString());
			if (forum == null)
				forum = ForumHandler.DataLayer.SelectForumByCode(o.ToString());
			return forum;
		}

		public object Evaluate(ExecutionState state, Token contextToken)
		{
			throw new InstructionExecutionException("You must specify an argument representing the code name for the forum category you want to load.", contextToken);
		}

		public void PrepareExpression(TokenList tokens, Stack<int?> precedenceStack)
		{
			if(tokens.Next.TokenType != TokenType.GroupStart)
				throw new TokenParserException("You must specify an argument representing the code name for the forum category you want to load.", tokens.Current);
			tokens.Advance();
		}
	}
	public class GetForumCategoryExpressionCreator : IExpressionCreator
	{
		public string Keyword { get { return "getforumcategory"; } }
		public IExpression Create() { return new GetForumCategoryExpression(); }
	}
}
