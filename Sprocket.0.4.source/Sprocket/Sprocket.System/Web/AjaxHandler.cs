using System;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Reflection;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using Sprocket;
using Sprocket.Utility;

namespace Sprocket.Web
{
	/// <summary>
	/// An ISprocketModule implementation that plugs into the WebEvents module to handle
	/// requests where the URL ends with .ajax (which signifies an Ajax request) from the
	/// browser via the browser's XmlHttpRequest object. Ajax requests utilise the classes
	/// AjaxMethodHandlerAttribute and AjaxMethodAttribute for determining which classes
	/// and methods can be used in Ajax requests.
	/// </summary>
	[ModuleDependency(typeof(WebEvents))]
	[ModuleDependency(typeof(WebAuthentication))]
	[ModuleDescription("Processes requests from the XmlHttpRequest javascript object (Ajax calls).")]
	[ModuleTitle("Ajax Request Handler")]
	public class AjaxRequestHandler : ISprocketModule
	{
		public static AjaxRequestHandler Instance
		{
			get { return (AjaxRequestHandler)Core.Instance[typeof(AjaxRequestHandler)].Module; }
		}

		#region ISprocketModule Members

		public void AttachEventHandlers(ModuleRegistry registry)
		{
			WebEvents.Instance.OnBeginHttpRequest += new WebEvents.HttpApplicationCancellableEventHandler(OnBeginHttpRequest);
			WebEvents.Instance.OnEndHttpRequest += new WebEvents.HttpApplicationEventHandler(OnEndHttpRequest);
		}

		void OnBeginHttpRequest(HandleFlag handled)
		{
			if (handled.Handled)
				return;

			if (IsAjaxRequest)
			{
				handled.Set();
				ProcessRequest(HttpContext.Current);
			}
		}

		void OnEndHttpRequest()
		{
		}

		#endregion

		public static bool IsAjaxRequest
		{
			get { return HttpContext.Current.Request.Path.EndsWith(".ajax"); }
		}

		private static Guid authKey = Guid.Empty;
		public static Guid AuthKey
		{
			get { return authKey; }
		}

		private DateTime pageTimeStamp = DateTime.MinValue;
		/// <summary>
		/// This should be used to set a base date for the page in order to control when an ajax request
		/// is no longer valid. The value should be set at the start of a page request so that any ajax
		/// scripts are written out with the appropriate time stamp. Each ajax request supplies the
		/// timestamp that was written to the page at load time. If data pertaining to the page is
		/// changed externally, i.e. from a later page request in a different browser, the new timestamp
		/// can be recorded in a database. Ajax requests can have their timestamp compared to the stored
		/// timestamp to ensure that requests on old data are not executed.
		/// </summary>
		public DateTime PageTimeStamp
		{
			get { return pageTimeStamp; }
			set { pageTimeStamp = value; }
		}

		/// <summary>
		/// Called when an AjaxMethod that has a RequiresAuthentication value of true
		/// passes the authentication check. This event can be used to perform further
		/// attribute checks on the method to further authenticate the calling user,
		/// such as for role and permission requirement checking.
		/// </summary>
		public event InterruptableEventHandler<MethodInfo> OnAjaxRequestAuthenticationCheck;

		/// <summary>
		/// This is fired for each Ajax request to ensure that the page data has not expired and would
		/// thus undermine the data integrity related to the ajax request.
		/// </summary>
		public event InterruptableEventHandler<DateTime> OnAjaxRequestTimeStampCheck;

		/// <summary>
		/// This method is called in response to any request where the URL ends with a .ajax
		/// extension, which Sprocket uses to designate an Ajax request from an XmlHttpRequest
		/// call in the browser. The posted data contains information that this method uses to
		/// find the correct ISprocketModule implementation. It then uses reflection to get
		/// the requested method from the module, converts the javascript arguments into native
		/// CLR types and passes them to the method, after which it takes the returned data and
		/// writes it to the output stream ready for the XmlHttpRequest object to complete its
		/// call. Note that data transport uses JSON encoding. See the JSON class in the
		/// Sprocket.System namespace for information.
		/// </summary>
		/// <param name="context">The current HttpContext object.</param>
		internal void ProcessRequest(HttpContext context)
		{
			System.Diagnostics.Debug.WriteLine("Start of AJAX page request...");
			Dictionary<string, object> responseData = new Dictionary<string, object>();
			try
			{
				// load the post data from the http stream
				byte[] posted = new byte[context.Request.InputStream.Length];
				context.Request.InputStream.Read(posted, 0, posted.Length);

				// interpret the stream as a JSON string and parse it into a dictionary
				string strData = System.Text.Encoding.ASCII.GetString(posted);
				IDictionary<string, object> data = (IDictionary<string, object>)JSON.Parse(strData);
				
				// extract the base page time stamp
				pageTimeStamp = new DateTime(long.Parse(data["LoadTimeStamp"].ToString()));
				System.Diagnostics.Debug.WriteLine("Extracted page time stamp of " + pageTimeStamp.Ticks.ToString());
				if (OnAjaxRequestTimeStampCheck != null)
				{
					Result result = new Result();
					OnAjaxRequestTimeStampCheck(pageTimeStamp, result);
					if (!result.Succeeded)
						throw new AjaxSessionExpiredException(result.Message);
				}

				// extract the module and method name
				string fullname = data["ModuleName"].ToString();
				int n = fullname.LastIndexOf(".");
				if(n == -1)
					throw new AjaxException("Method name specified incorrectly. Expected format ModuleNamespace.MethodName.\nThe following incorrect format was supplied: " + data["ModuleName"]);
				string moduleNamespace = fullname.Substring(0, n);
				string methodName = fullname.Substring(n+1, fullname.Length - (n+1));

				// extract the authentication key
				if (data["AuthKey"].ToString() != WebAuthentication.AuthKeyPlaceholder)
					authKey = new Guid(data["AuthKey"].ToString());

				// extract the arguments
				List<object> parsedArguments = (List<object>)data["MethodArgs"];

				// find and verify the module/method that should handle this request
				ISprocketModule module = Core.Instance[moduleNamespace].Module;
				if(module == null)
					throw new AjaxException("The specified module \"" + moduleNamespace + "\" was not found.");
				if(Attribute.GetCustomAttribute(module.GetType(), typeof(AjaxMethodHandlerAttribute), false) == null)
					throw new SprocketException("The specified module is not marked with AjaxMethodHandlerAttribute. (" + data["ModuleName"] + ")");
				MethodInfo info = module.GetType().GetMethod(methodName);
				if(info == null)
					throw new AjaxException("Failed to find an instance of the specified method. (" + data["ModuleName"] + ")");
				Attribute ajaxMethodAttr = Attribute.GetCustomAttribute(info, typeof(AjaxMethodAttribute));
				if(ajaxMethodAttr == null)
					throw new AjaxException("Specified method is not marked with AjaxMethodAttribute. (" + data["ModuleName"] + ")");
				AjaxMethodAttribute attr = (AjaxMethodAttribute)ajaxMethodAttr;
				if (attr.RequiresAuthentication)
				{
					WebAuthentication auth = WebAuthentication.Instance;
					if (!auth.IsLoggedIn)
						throw new AjaxException("You're not currently logged in. Please refresh the page.");

					if (OnAjaxRequestAuthenticationCheck != null)
					{
						Result result = new Result();
						OnAjaxRequestAuthenticationCheck(info, result);
						if (!result.Succeeded)
							throw new AjaxException(result.Message);
					}
				}
				
				// get all of the parameters that the method requires
				ParameterInfo[] methodParamInfos = info.GetParameters();

				// funcinfo is a string representation of the method format and is used for displaying meaningful errors
				string funcinfo = data["ModuleName"] + "(";
				if(methodParamInfos.Length > 0)
					funcinfo += methodParamInfos[0].ParameterType.Name + " " + methodParamInfos[0].Name;
				for(int j=1; j<methodParamInfos.Length; j++)
					funcinfo += ", " + methodParamInfos[j].ParameterType.Name + " " + methodParamInfos[j].Name;
				funcinfo += ")";
				if(methodParamInfos.Length != parsedArguments.Count)
					throw new AjaxException("Method expects " + methodParamInfos.Length + " argument(s) but instead received " + (parsedArguments.Count) + ". Expected format is:\n" + funcinfo);

				// create the parameter array and convert each supplied value to its native type
				object[] prmValuesForMethod = new object[methodParamInfos.Length];
				for(int i=0; i<prmValuesForMethod.Length; i++)
				{
					Type t = methodParamInfos[i].ParameterType;
					try
					{
						if (parsedArguments[i] == null)
							prmValuesForMethod[i] = null;
						else if (t.Name == "Object")
							prmValuesForMethod[i] = parsedArguments[i];
						else if (t.Name == "DateTime")
							prmValuesForMethod[i] = DateTime.Parse(parsedArguments[i].ToString());
						else if (t.IsArray || t.IsSubclassOf(typeof(IList)))
						{
							int elementCount = ((List<object>)parsedArguments[i]).Count;
							Type arrType = t.GetElementType().MakeArrayType(elementCount);
							object arr = Array.CreateInstance(t.GetElementType(), ((List<object>)parsedArguments[i]).Count);
							for (int k = 0; k < ((IList<object>)parsedArguments[i]).Count; k++)
								((IList)arr)[k] = ((IList<object>)parsedArguments[i])[k];
							prmValuesForMethod[i] = arr;
						}
						else if (t.GetInterface("IJSONReader") != null)
						{
							object obj = Activator.CreateInstance(t);
							((IJSONReader)obj).LoadJSON(parsedArguments[i]);
							prmValuesForMethod[i] = obj;
						}
						else if (t.IsAssignableFrom(typeof(Guid)) || t.IsAssignableFrom(typeof(Guid?)))
							prmValuesForMethod[i] = parsedArguments[i] == null ? (Guid?)null : new Guid(parsedArguments[i].ToString());
						else if (t.IsAssignableFrom(typeof(long)) || t.IsAssignableFrom(typeof(long?)))
							prmValuesForMethod[i] = parsedArguments[i] == null ? (long?)null : long.Parse(parsedArguments[i].ToString());
						else
							prmValuesForMethod[i] = Convert.ChangeType(parsedArguments[i], t);
					}
					catch(Exception ex)
					{
						string err = "Error converting parameter {0} to type {1}. Expected format is:\n{2}\nReceived Value:\n{3}\nException message:\n{4}";
						throw new AjaxException(string.Format(err, i, methodParamInfos[i].ParameterType.Name, funcinfo, parsedArguments[i], ex));
					}
				}
				
				// invoke the method
				if(info.ReturnType == typeof(void))
				{
					info.Invoke(module, prmValuesForMethod);
				}
				else
				{
					object returnVal = info.Invoke(module, prmValuesForMethod);
					responseData["Data"] = returnVal;
				}
				//context.Response.Write(JSON.Encode(responseData));
			}
			catch(Exception e)
			{
				responseData["__error"] = e;
				responseData["__exceptionType"] = e.GetType().FullName;
			}
			if (!responseData.ContainsKey("Data"))
				responseData["Data"] = null;
			//responseData["__timeStamp"] = pageTimeStamp.Ticks;
			context.Response.Write(JSON.Encode(responseData));
		}
	}
}
