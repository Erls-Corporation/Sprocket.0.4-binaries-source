using System;
using System.Collections.Generic;
using System.Text;

namespace Sprocket.Web
{
	/// <summary>
	/// Represents a block of JavaScript that will be supplied to the page via an Ajax
	/// call after the page has been loaded. The script has an ID string to uniquely
	/// identify it. Any script sent to the browser using this class will be executed
	/// once only. If the ID string is blank or set to null, the script will be executed
	/// every time it is sent to the browser.
	/// </summary>
	public class AjaxJavaScript
	{
		private string id, script;

		public string Script
		{
			get { return script; }
			set { script = value; }
		}

		public string ID
		{
			get { return id; }
			set { id = value; }
		}

		public AjaxJavaScript(string id, string script)
		{
			this.id = id;
			this.script = script;
		}

		public override string ToString()
		{
			if(id == "" || id == null) return script;
			string idx = id.Replace("'","\\'");
			string arr = "__ajaxscr['" + idx + "']";
			return "var __ajaxscr;\r\nif(!__ajaxscr)\r\n\t__ajaxscr = [];\r\n" +
				"\tif(!" + arr + ") {\r\n\t" + arr + " = true;\r\n" + script + "\r\n}\r\n";
		}
	}
}
