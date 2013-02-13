using System;
using System.Collections.Generic;
using System.Text;

using Sprocket.Web.CMS.Script;


namespace Sprocket.Web.CMS.Content
{
	public class RequestSpeedExpression : IExpression
	{
		public static void Set()
		{
			CurrentRequest.Value["RequestSpeedExpression.Start"] = SprocketDate.Now;
		}

		public object Evaluate(ExecutionState state, Token contextToken)
		{
			TimeSpan ts = SprocketDate.Now.Subtract((DateTime)CurrentRequest.Value["RequestSpeedExpression.Start"]);
			string s = ts.TotalSeconds.ToString("0.####") + "s";
			if (s == "0s")
				return "instant";
			return s;
		}
	}

	public class RequestSpeedExpressionCreator : IExpressionCreator
	{
		public string Keyword { get { return "requestspeed"; } }
		public IExpression Create() { return new RequestSpeedExpression(); }
	}
}
