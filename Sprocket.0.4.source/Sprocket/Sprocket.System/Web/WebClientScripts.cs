using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using Sprocket;
using Sprocket.Utility;

namespace Sprocket.Web
{
	/// <summary>
	/// Handles loading and rendering of JavaScript on the client, including initialisation
	/// of the AJAX subsystem and mapping of server-side ajax methods to client-side methods
	/// and objects of the same names. JavaScripts are compressed using Atif Aziz's
	/// JavaScriptCondenser class, which strips out comments, whitespace, etc. Data formatting
	/// for transfer between the client and server is provided by the open-source JSON format.
	/// </summary>
	[ModuleDescription( "Handles javascript aggregation and rendering to the page.")]
	[ModuleTitle("Web Client Script Renderer")]
	public class WebClientScripts : ISprocketModule
	{
		/// <summary>
		/// Provides the facility to add extra javascript blocks to the current page.
		/// </summary>
		/// <param name="scripts">Dictionary mapping a unique key name to a block of javascript code</param>
		public delegate void LoadingJavaScriptsHandler(Dictionary<string, string> scripts);

		/// <summary>
		/// Provides the facility for other modules to make modifications to the loaded javascripts
		/// before they are rendered.
		/// </summary>
		/// <param name="scripts">A reference to the javascript collection</param>
		public delegate void BeforeRenderJavaScriptHandler(JavaScriptCollection scripts);

		/// <summary>
		/// Fires after all javascript is loaded and before it is rendered.
		/// </summary>
		public event BeforeRenderJavaScriptHandler OnBeforeRenderJavaScript;

		public static WebClientScripts Instance
		{
			get { return (WebClientScripts)Core.Instance[typeof(WebClientScripts)].Module; }
		}

		private static Dictionary<Type, AjaxModuleRef> ajaxScripts = null;
		/// <summary>
		/// Builds a set of javascript methods which are mapped to corresponding ModuleBase-derived
		/// class methods which are marked with the AjaxMethod attribute.
		/// </summary>
		/// <param name="restrictToAjaxModules">An optional list of ajax module names that should be used instead of the full list</param>
		/// <returns>A block of javascript defining objects and methods that encapsulate ajax calls to
		/// matching server-side methods</returns>
		public string GetAjaxMethodsScript(params string[] restrictToAjaxModules)
		{
			if (ajaxScripts == null)
			{
				ajaxScripts = new Dictionary<Type, AjaxModuleRef>();
				foreach (RegisteredModule module in Core.Modules.ModuleRegistry)
				{
					Type t = module.Module.GetType();
					// don't include modules that don't have the correct attribute
					AjaxMethodHandlerAttribute[] amh = (AjaxMethodHandlerAttribute[])t.GetCustomAttributes(typeof(AjaxMethodHandlerAttribute), false);
					if (amh.Length != 1)
						continue;

					AjaxModuleRef modref = new AjaxModuleRef(t, amh[0].AjaxTypeName);

					// get all the methods for the module
					MethodInfo[] infos = module.Module.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance);
					foreach (MethodInfo info in infos)
					{
						// make sure the method has the appropriate attribute
						object[] attrs = info.GetCustomAttributes(typeof(AjaxMethodAttribute), false);
						if (attrs.Length == 1)
							modref.AddMethod(info.Name);
					}
					ajaxScripts.Add(t, modref);
				}
			}

			StringBuilder sb = new StringBuilder();
			sb.Append("Ajax = {};");
			sb.Append(Environment.NewLine);
			foreach (AjaxModuleRef m in ajaxScripts.Values)
			{
				// if restrictions have been specified, make sure this module is included in the accepted module types
				if (restrictToAjaxModules.Length > 0)
					if (Array.IndexOf<string>(restrictToAjaxModules, m.AjaxTypeName) == -1)
						continue;

				sb.Append(m.ToString());
			}

			return sb.ToString();
		}

		private class AjaxModuleRef
		{
			public string AjaxTypeName;

			private Type type;
			public Type Type
			{
				get { return type; }
			}

			private StringBuilder sb = new StringBuilder();
			bool isFirstMethod = true;
			public AjaxModuleRef(Type t, string ajaxTypeName)
			{
				type = t;
				AjaxTypeName = ajaxTypeName;
				sb.Append(Environment.NewLine);
				sb.Append("Ajax.");
				sb.Append(ajaxTypeName);
				sb.Append(" = {");
				sb.Append(Environment.NewLine);
			}

			public void AddMethod(string methodName)
			{
				if (!isFirstMethod)
				{
					sb.Append(",");
					sb.Append(Environment.NewLine);
				}
				else
					isFirstMethod = false;
				sb.Append("\t");
				sb.Append(methodName);
				sb.Append(" : function() { SprocketAjax.Request('");
				sb.Append(type.FullName);
				sb.Append(".");
				sb.Append(methodName);
				sb.Append("', arguments); }");
			}

			public override string ToString()
			{
				return sb.ToString() + Environment.NewLine + "};" + Environment.NewLine;
			}
		}

		private string standardScripts = null;

		public string StandardScripts
		{
			get
			{
				if (standardScripts == null)
				{
					standardScripts = string.Concat(
						ResourceLoader.LoadTextResource("Sprocket.Web.javascript.generic.js"),
						ResourceLoader.LoadTextResource("Sprocket.Web.javascript.browser-tools.js"),
						ResourceLoader.LoadTextResource("Sprocket.Web.javascript.json.js"),
						ResourceLoader.LoadTextResource("Sprocket.Web.javascript.ajax.js"),
						GetAjaxMethodsScript()
						);
				}
				return standardScripts;
			}
		}

		/// <summary>
		/// Writes all registered javascripts into a string surrounded by html script tags,
		/// ready to be written to an html page.
		/// </summary>
		/// <returns>HTML script tags with containing javascript</returns>
		public string BuildStandardScriptsBlock()
		{
			Dictionary<string, string> scripts = new Dictionary<string, string>();
			JavaScriptCollection jsc = new JavaScriptCollection();
			jsc.SetKey("$APPLICATIONROOT$", WebUtility.BasePath);
			jsc.SetKey("$LOADTIMESTAMP$", AjaxRequestHandler.Instance.PageTimeStamp.Ticks.ToString());
			jsc.Add("standard", StandardScripts);

			if (OnBeforeRenderJavaScript != null) OnBeforeRenderJavaScript(jsc);

			return jsc.CreateScriptTags();
		}

		public void AttachEventHandlers(ModuleRegistry registry)
		{
			WebEvents.Instance.OnBeforeLoadExistingFile += new WebEvents.RequestedPathEventHandler(WebEvents_OnBeforeLoadExistingFile);
		}

		private Dictionary<string, DateTime> compressedJSFiles = new Dictionary<string, DateTime>();
		void WebEvents_OnBeforeLoadExistingFile(HandleFlag handled)
		{
			if (!SprocketPath.Value.EndsWith(".js")) return;
			FileInfo file = new FileInfo(SprocketPath.Physical);
			HttpContext.Current.Response.Cache.SetLastModified(file.LastWriteTime);
			HttpContext.Current.Response.Cache.SetMaxAge(new TimeSpan(24, 0, 0));
			if (!CompressJavaScript) return;
			bool rewrite = false;
			if (!ContentCache.IsContentCached(SprocketPath.Value))
				rewrite = true;
			else if (!compressedJSFiles.ContainsKey(file.FullName))
				rewrite = true;
			else if (compressedJSFiles[file.FullName] != file.LastWriteTime)
				rewrite = true;
			HttpContext.Current.Response.ContentType = "text/javascript";
			if (rewrite)
			{
				try
				{
					using (StreamReader reader = file.OpenText())
					{
						string s = JavaScriptCondenser.Condense(reader.ReadToEnd());
						HttpContext.Current.Response.Write(s);
						ContentCache.CacheContent(SprocketPath.Value, s);
						reader.Close();
						compressedJSFiles[file.FullName] = file.LastWriteTime;
					}
				}
				catch
				{
					return; // if an error occurs, let the system serve up the file normally
				}
			}
			else
				HttpContext.Current.Response.Write(ContentCache.ReadCache(SprocketPath.Value));
			handled.Set();
		}

		public static bool CompressJavaScript
		{
			get { return SprocketSettings.GetBooleanValue("CompressJavaScript"); }
		}
	}

	/// <summary>
	/// Handles collection, preparation and rendering of javascript blocks.
	/// </summary>
	public class JavaScriptCollection
	{
		private Dictionary<string, string> scripts = new Dictionary<string,string>();
		private Dictionary<string, object> keys = new Dictionary<string,object>();
		/// <summary>
		/// Adds a new javascript block to the collection.
		/// </summary>
		/// <param name="name">A unique (arbitrary) code to identify this block</param>
		/// <param name="script">The script block</param>
		public void Add(string name, string script)
		{
			if (scripts.ContainsKey(name))
				throw new SprocketException("JavaScriptCollection: Can't add script of name \"" + name + "\" because it already exists in the collection.");
			scripts.Add(name, script);
		}

		/// <summary>
		/// Use this method to replace javascript placeholders with actual values. For
		/// example, the script block could have a value {AuthKey} somewhere in the various
		/// javascript blocks. This would be put in the script blocks to be replaced by a
		/// real authentication key string when the script is rendered to the page. All
		/// script blocks in the collection will have the specified key/string replaced by
		/// the specified value before the scripts are rendered to the page.
		/// </summary>
		/// <param name="keyName">The name of the key/string to replace at run-time</param>
		/// <param name="keyValue">The value to replace the key with</param>
		public void SetKey(string keyName, string keyValue)
		{
			keys[keyName] = keyValue;
		}

		/// <summary>
		/// Writes all registered scripts into a single string surrounded by html script tags
		/// </summary>
		/// <returns>HTML script tags with containing javascript</returns>
		public string CreateScriptTags()
		{
			StringBuilder sb = new StringBuilder();
			foreach (KeyValuePair<string, string> script in scripts)
			{
				string js = script.Value;
				foreach (KeyValuePair<string, object> key in keys)
					js = js.Replace(key.Key, key.Value.ToString());
				sb.Append("<script language=\"JavaScript\">");
				sb.Append(Environment.NewLine);
				if (WebClientScripts.CompressJavaScript)
					js = JavaScriptCondenser.Condense(js);
				sb.Append(js);
				sb.Append(Environment.NewLine);
				sb.Append("</script>");
				sb.Append(Environment.NewLine);
			}
			return sb.ToString();
		}
	}

	/// <summary>
	/// Marks a class method for use by the AJAX subsystem. This is only used
	/// by classes that implement ISprocketModule. JavaScript will be generated to define
	/// a client-side object for the class that will contain matching methods for
	/// each server-side method marked with this attribute.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple=false)]
	public class AjaxMethodAttribute : Attribute
	{
		private bool requiresAuthentication = false;

		public bool RequiresAuthentication
		{
			get { return requiresAuthentication; }
			set { requiresAuthentication = value; }
		}
	}

	/// <summary>
	/// Any class utilising the AjaxMethod attribute must be marked with
	/// the AjaxMethodHandler attribute or it will be ignored when
	/// Ajax javascript code is generated.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
	public class AjaxMethodHandlerAttribute : Attribute
	{
		private string ajaxTypeName = null;
		public AjaxMethodHandlerAttribute(string ajaxTypeName)
		{
			this.ajaxTypeName = ajaxTypeName;
		}

		public string AjaxTypeName
		{
			get { return ajaxTypeName; }
		}
	}
}
