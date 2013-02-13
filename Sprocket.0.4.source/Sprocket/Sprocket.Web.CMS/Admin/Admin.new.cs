using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.IO;

using Sprocket;
using Sprocket.Web;
using Sprocket.Utility;
using Sprocket.Data;

namespace Sprocket.Web.CMS.Admin
{
	[ModuleDependency(typeof(WebEvents))]
	[ModuleDependency(typeof(SprocketSettings))]
	[ModuleDependency(typeof(WebAuthentication))]
	[ModuleDescription("The base platform upon which the Sprocket CMS web interface is built. Most modules for the CMS plug into this module.")]
	[ModuleTitle("Website Administration Module")]
	public partial class WebsiteAdmin1 //: ISprocketModule
	{
		public delegate void AdminRequestHandler(AdminInterface admin, HandleFlag handled);

		public event AdminRequestHandler OnAdminRequest;
		public event InterruptableEventHandler<string> OnCMSAdminAuthenticationSuccess;

		public static WebsiteAdmin Instance
		{
			get { return (WebsiteAdmin)Core.Instance[typeof(WebsiteAdmin)].Module; }
		}

		public void AttachEventHandlers(ModuleRegistry registry)
		{
			WebEvents wem = WebEvents.Instance;
			wem.OnLoadRequestedPath += new WebEvents.RequestedPathEventHandler(OnLoadRequestedPath);
			OnAdminRequest += new AdminRequestHandler(WebsiteAdmin_OnAdminRequest);
		}

		public void Write(object val)
		{
			HttpContext.Current.Response.Write(val);
		}

		void OnLoadRequestedPath(HandleFlag handled)
		{
			return;
			if (SprocketPath.Sections.Length == 0) return;
			if (SprocketPath.Sections[0] != "admin") return;
			bool processed = false;
			string lastchunk = SprocketPath.Sections[SprocketPath.Sections.Length - 1];

			switch(lastchunk)
			{
				case "admin.css":
					HttpContext.Current.Response.TransmitFile("~/resources/admin/admin.css");
					HttpContext.Current.Response.ContentType = "text/css";
					processed = true;
					break;

				default:
					WebAuthentication auth = WebAuthentication.Instance;
					HttpResponse Response = HttpContext.Current.Response;
					HttpServerUtility Server = HttpContext.Current.Server;
					switch (SprocketPath.Value)
					{
						case "admin/login":
							ShowLoginScreen();
							processed = true;
							break;

						case "admin/logout":
							auth.ClearAuthenticationCookie();
							Response.Redirect(WebUtility.MakeFullPath("admin/login"));
							processed = true;
							break;

						case "admin/login/process":
							if (auth.ProcessLoginForm("SprocketUsername", "SprocketPassword", "SprocketPreserveLogin"))
								Response.Redirect(WebUtility.MakeFullPath("admin"));
							else
								ShowLoginScreen("Invalid Username and/or Password.");
							processed = true;
							break;

						default:
							if (!auth.IsLoggedIn)
							{
								GotoLoginScreen();
								processed = true;
							}
							else if (OnCMSAdminAuthenticationSuccess != null)
							{
								Result result = new Result();
								OnCMSAdminAuthenticationSuccess(auth.CurrentUsername, result);
								if (!result.Succeeded)
								{
									ShowLoginScreen(result.Message);
									processed = true;
								}
							}
							break;
					}
					break;
			}
			if (processed)
			{
				handled.Set();
				return;
			}

			if (OnAdminRequest != null)
			{
				AdminInterface admin = new AdminInterface();
				OnAdminRequest(admin, handled);
				if (handled.Handled)
				{
					WebClientScripts scripts = WebClientScripts.Instance;
					admin.AddMainMenuLink(new AdminMenuLink("Administrative Tasks", WebUtility.MakeFullPath("admin"), -100));
					admin.AddMainMenuLink(new AdminMenuLink("Log Out", WebUtility.MakeFullPath("admin/logout"), 100));
					admin.AddFooterLink(new AdminMenuLink("&copy; 2005-" + SprocketDate.Now.Year + " " + SprocketSettings.GetValue("WebsiteName"), "", 100));
					string powered = SprocketSettings.GetValue("ShowPoweredBySprocket");
					if(powered != null)
						if(StringUtilities.MatchesAny(powered.ToLower(), "true", "yes"))
							admin.AddFooterLink(new AdminMenuLink("Powered by Sprocket", "http://www.sprocketcms.com", 1000));
					admin.AddHeadSection(new RankedString(scripts.BuildStandardScriptsBlock(), 1));
					HttpContext.Current.Response.Write(admin.Render());
				}
			}
		}

		void WebsiteAdmin_OnAdminRequest(AdminInterface admin, HandleFlag handled)
		{
			if (SprocketPath.Sections[0] != "admin") return;

			switch (SprocketPath.Value)
			{
				case "admin/dbsetup":
					Result result = DatabaseManager.DatabaseEngine.Initialise();
					if (result.Succeeded)
						admin.AddContentSection(new RankedString("<p style=\"color:green\" class=\"standalone-message\">Database setup completed.</p>", 1));
					else
						admin.AddContentSection(new RankedString("<strong style=\"color:red\" class=\"standalone-message\">Unable to Initialise Database</strong><p>" + result.Message + "</p>", 1));
					break;

				case "admin/clearcache":
					ContentCache.ClearCache();
					admin.AddContentSection(new RankedString("<p style=\"color:green\" class=\"standalone-message\">The cache has been cleared.</p>", 1));
					break;

				case "admin":
					break;

				default:
					return;
			}

			admin.ContentHeading = "Current Overview";
			admin.AddContentSection(new RankedString("<div class=\"standalone-message\">" +
				"<a href=\"" + WebUtility.BasePath + "admin/dbsetup\">Run database setup</a> | " +
				"<a href=\"" + WebUtility.BasePath + "admin/clearcache\">Clear page cache</a>" +
				"</div>", 0));
			handled.Set();
		}

		void GotoLoginScreen()
		{
			HttpContext.Current.Response.Redirect(WebUtility.MakeFullPath("admin/login"));
		}

		void ShowLoginScreen()
		{
			ShowLoginScreen("");
		}

		void ShowLoginScreen(string loginErrorMessage)
		{
			string html = WebUtility.CacheTextFile("resources/admin/login.htm");
			html = html.Replace("{basepath}", WebUtility.BasePath);
			html = html.Replace("{login-processor}", WebUtility.MakeFullPath("admin/login/process"));
			html = html.Replace("{login-error}", loginErrorMessage);
			html = html.Replace("{website-name}", "Website Administration");
			Write(html);
		}
	}
}
