using System;
using System.Collections.Generic;
using System.Text;
using Sprocket.Web.CMS.Script;

using Sprocket.Utility;

namespace Sprocket.Web.CMS.Content.Expressions
{
	class AjaxScriptsExpression : IArgumentListEvaluatorExpression
	{
		public object Evaluate(Token contextToken, List<ExpressionArgument> arguments, ExecutionState state)
		{
			string[] types = new string[arguments.Count];
			int c = 0;
			foreach (ExpressionArgument arg in arguments)
			{
				object o = arg.Expression.Evaluate(state, arg.Token);
				if (o == null) o = String.Empty;
				types[c++] = o.ToString();
			}

			string scr =
				ResourceLoader.LoadTextResource(typeof(WebClientScripts).Assembly, "Sprocket.Web.javascript.generic.js")
				+ ResourceLoader.LoadTextResource(typeof(WebClientScripts).Assembly, "Sprocket.Web.javascript.json.js")
				+ ResourceLoader.LoadTextResource(typeof(WebClientScripts).Assembly, "Sprocket.Web.javascript.ajax.js")
					.Replace("$APPLICATIONROOT$", WebUtility.BasePath)
					.Replace("$LOADTIMESTAMP$", AjaxRequestHandler.Instance.PageTimeStamp.Ticks.ToString())
				+ WebClientScripts.Instance.GetAjaxMethodsScript(types);
			if (WebClientScripts.CompressJavaScript)
				return JavaScriptCondenser.Condense(scr);
			else
				return scr;
		}

		public object Evaluate(ExecutionState state, Token contextToken)
		{
			return Evaluate(contextToken, new List<ExpressionArgument>(), state);
		}
	}

	class AjaxScriptsExpressionCreator : IExpressionCreator
	{
		public string Keyword
		{
			get { return "ajaxscripts"; }
		}

		public IExpression Create()
		{
			return new AjaxScriptsExpression();
		}
	}
}
