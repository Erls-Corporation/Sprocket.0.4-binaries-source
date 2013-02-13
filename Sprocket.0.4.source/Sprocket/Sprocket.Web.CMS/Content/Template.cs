using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

using Sprocket.Web.CMS.Script;

namespace Sprocket.Web.CMS.Content
{
	public class Template : IPropertyEvaluatorExpression
	{
		private Dictionary<string, DateTime> fileTimes;
		public Dictionary<string, DateTime> FileTimes
		{
			get { return fileTimes; }
		}

		SprocketScript script;
		public SprocketScript Script
		{
			get { return script; }
		}

		private string name;
		public Template(string name, SprocketScript script, Dictionary<string, DateTime> fileTimes)
		{
			this.fileTimes = fileTimes;
			this.script = script;
			this.name = name;
		}

		public bool IsOutOfDate
		{
			get
			{
				foreach (KeyValuePair<string, DateTime> kvp in fileTimes)
				{
					FileInfo file = new FileInfo(kvp.Key);
					if (!file.Exists)
						return true;
					if (file.LastWriteTime != kvp.Value)
						return true;
				}
				return false;
			}
		}

		public bool IsValidPropertyName(string propertyName)
		{
			return HasProperty(propertyName);
		}

		public static bool HasProperty(string propertyName)
		{
			switch (propertyName)
			{
				case "name":
					return true;
				default:
					return false;
			}
		}

		public object EvaluateProperty(string propertyName, Token token, ExecutionState state)
		{
			switch (propertyName)
			{
				case "name":
					return name;
				default:
					return VariableExpression.InvalidProperty;
			}
		}

		public object Evaluate(ExecutionState state, Token contextToken)
		{
			return Script.ExecuteToResolveExpression(state);
		}
	}
}
