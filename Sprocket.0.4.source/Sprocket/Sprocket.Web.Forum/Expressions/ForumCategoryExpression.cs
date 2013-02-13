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
using Sprocket.Web.CMS.Script.Parser;

namespace Sprocket.Web.Forums
{
	public class ForumCategoryExpression : IPropertyExpression, IVariableExpression
	{
		private PropertyType propertyType = PropertyType.None;
		private List<ExpressionArgument> args = null;
		private ForumCategory forumCategory = null;

		public bool PresetPropertyName(Token propertyToken, List<Token> tokens, ref int nextIndex)
		{
			switch (propertyToken.Value)
			{
				case "forumcategoryid": propertyType = PropertyType.ForumCategoryID; return true;
				case "clientspaceid": propertyType = PropertyType.ClientSpaceID; return true;
				case "categorycode": propertyType = PropertyType.CategoryCode; return true;
				case "name": propertyType = PropertyType.Name; return true;
				case "urltoken": propertyType = PropertyType.URLToken; return true;
				case "datecreated": propertyType = PropertyType.DateCreated; return true;
				case "rank": propertyType = PropertyType.Rank; return true;
				case "internaluseonly": propertyType = PropertyType.InternalUseOnly; return true;
				case "forumlist": propertyType = PropertyType.ForumList; return true;
				default: return false;
			}
		}

		public void SetFunctionArguments(List<ExpressionArgument> arguments, Token functionCallToken)
		{
			args = arguments;
			if (args.Count != 1)
				throw new TokenParserException("An argument was expected specifying which \"forumcategory\" to load.", functionCallToken);
		}

		public object Evaluate(ExecutionState state)
		{
			// attempt to load the specified ForumCategory
			object value = TokenParser.VerifyUnderlyingType(args[0].Expression.Evaluate(state));

			if (value is decimal)
				forumCategory = ForumHandler.DataLayer.SelectForumCategory((long)value);
			else if (value is string)
				forumCategory = ForumHandler.DataLayer.SelectForumCategoryByCode((string)value);
			else
				throw new InstructionExecutionException("Could not load an instance of \"forumcategory\" because the argument did not equate to the right kind of value.", args[0].Token);

			// return the relevant value
			switch (propertyType)
			{
				case PropertyType.ForumCategoryID: return forumCategory.ForumCategoryID;
				case PropertyType.ClientSpaceID: return forumCategory.ClientSpaceID;
				case PropertyType.CategoryCode: return forumCategory.CategoryCode;
				case PropertyType.Name: return forumCategory.Name;
				case PropertyType.URLToken: return forumCategory.URLToken;
				case PropertyType.DateCreated: return forumCategory.DateCreated;
				case PropertyType.Rank: return forumCategory.Rank;
				case PropertyType.InternalUseOnly: return forumCategory.InternalUseOnly;
				case PropertyType.ForumList: return GetForums();
				case PropertyType.None:
					if (forumCategory == null)
						return null;
					return this;
				default: return "[ForumCategory]";
			}
		}

		public void PrepareExpression(Token expressionToken, List<Token> tokens, ref int nextIndex, Stack<int?> precedenceStack)
		{
		}

		public object EvaluateVariableProperty(string propertyName, Token propertyToken, ExecutionState state)
		{
			switch (propertyName)
			{
				case "forumcategoryid": return forumCategory.ForumCategoryID;
				case "clientspaceid": return forumCategory.ClientSpaceID;
				case "categorycode": return forumCategory.CategoryCode;
				case "name": return forumCategory.Name;
				case "urltoken": return forumCategory.URLToken;
				case "datecreated": return forumCategory.DateCreated;
				case "rank": return forumCategory.Rank;
				case "internaluseonly": return forumCategory.InternalUseOnly;
				case "forumlist": return GetForums();
				default:
					throw new InstructionExecutionException("\"" + propertyName + "\" is not a property of this variable.", propertyToken);
			}
		}

		private List<Forum> GetForums()
		{
			List<Forum> forums = ForumHandler.DataLayer.ListForums(forumCategory.ForumCategoryID);
			return forums;
		}

		enum PropertyType
		{
			None,
			ForumCategoryID,
			ClientSpaceID,
			CategoryCode,
			Name,
			URLToken,
			DateCreated,
			Rank,
			InternalUseOnly,
			ForumList
		}
	}
	public class ForumCategoryExpressionCreator : IExpressionCreator
	{
		public string Keyword { get { return "forumcategory"; } }
		public IExpression Create() { return new ForumCategoryExpression(); }
	}
}
