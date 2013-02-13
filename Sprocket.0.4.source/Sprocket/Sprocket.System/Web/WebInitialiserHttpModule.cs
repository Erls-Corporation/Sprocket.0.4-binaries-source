using System;
using System.Web;
using Sprocket;
using System.Diagnostics;

namespace Sprocket.Web
{
	/// <summary>
	/// This class implements the IHttpModule that is required to connect the Sprocket framework
	/// to the ASP.Net pipeline. It is interdependent with the WebEvents class.
	/// </summary>
	public class WebInitialiserHttpModule : IHttpModule
	{
		/// <summary>
		/// This is called just once when the HttpApplication starts up.
		/// </summary>
		/// <param name="app">The HttpApplication object that is to persist between requests.</param>
		public void Init(HttpApplication app)
		{
			app.BeginRequest += new EventHandler(app_BeginRequest);
			app.AcquireRequestState += new EventHandler(app_AcquireRequestState);
			app.EndRequest += new EventHandler(app_EndRequest);
			app.Error += new EventHandler(app_Error);
		}

		void app_Error(object sender, EventArgs e)
		{
			WebEvents.Instance.FireError(sender, e);
		}

		void app_EndRequest(object sender, EventArgs e)
		{
			WebEvents.Instance.FireEndRequest(sender, e);
		}

		void app_AcquireRequestState(object sender, EventArgs e)
		{
			WebEvents.Instance.FireAcquireRequestState(sender, e);
		}

		void app_BeginRequest(object sender, EventArgs e)
		{
			WebEvents.Instance.FireBeginRequest(sender, e);
		}

		public void Dispose()
		{
		}
	}
}
