using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using Sprocket;
using Sprocket.Web;

namespace Sprocket.Web
{
	/// <summary>
	/// This class exists because of a need for static variables that last for the life of the current
	/// request only. Static variables in the current system are unfortunately lasting for the life of
	/// the application, which is both annoying and useful. :P
	/// </summary>
	[ModuleDescription("Provides a mechanism for storing global data confined to the scope of the current request.")]
	[ModuleTitle("Current Request State Handler")]
	public class CurrentRequest : ISprocketModule
	{
		private static CurrentRequest cr = null;
		public static CurrentRequest Value
		{
			get
			{
				if (cr == null)
					cr = new CurrentRequest();
				return cr;
			}
		}

		private Dictionary<string, object> dict;
		private Dictionary<string, object> Values
		{
			get
			{
				if (HttpContext.Current == null)
				{
					if(dict == null)
						dict = new Dictionary<string,object>();
					return dict;
				}
				else
				{
					if(HttpContext.Current.Items["Sprocket_CurrentRequest"] == null)
						HttpContext.Current.Items["Sprocket_CurrentRequest"] = new Dictionary<string, object>();
					return (Dictionary<string, object>)HttpContext.Current.Items["Sprocket_CurrentRequest"];
				}
			}
		}

		public CurrentRequest()
		{
		}

		public object this[string val]
		{
			set { Values[val] = value; }
			get { return Values.ContainsKey(val) ? Values[val] : null; }
		}

		public void PostRequestHandlerExecute(object sender, EventArgs e)
		{
			HttpContext.Current.Items["Sprocket_CurrentRequest"] = null;
		}

		#region ISprocketModule Members

		public void AttachEventHandlers(ModuleRegistry registry)
		{
			if(HttpContext.Current != null)
				HttpContext.Current.ApplicationInstance.PostRequestHandlerExecute += new EventHandler(PostRequestHandlerExecute);
		}

		#endregion
	}
}
