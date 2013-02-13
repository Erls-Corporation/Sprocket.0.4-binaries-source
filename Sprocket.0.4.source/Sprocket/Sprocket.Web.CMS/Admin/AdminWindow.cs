using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

using Sprocket.Utility;
using Sprocket.Web.CMS.Script;

namespace Sprocket.Web.CMS.Admin
{
	[ModuleTitle("AdminWindow")]
	[ModuleDescription("Places a floating collapsible admin window on every screen if logged in with correct permissions")]
	[ModuleDependency(typeof(WebEvents))]
	public class AdminWindow //: ISprocketModule
	{
		public static AdminWindow Instance
		{
			get { return (AdminWindow)Core.Instance[typeof(AdminWindow)].Module; }
		}

		public event InterruptableEventHandler OnBeforeDisplayAdminWindowOverlay;
		public void AttachEventHandlers(ModuleRegistry registry)
		{
			WebEvents.Instance.OnLoadRequestedPath += new WebEvents.RequestedPathEventHandler(WebEvents_OnLoadRequestedPath);
			WebEvents.Instance.OnRequestedPathProcessed += new WebEvents.HttpApplicationEventHandler(WebEvents_OnRequestedPathProcessed);
		}

		void WebEvents_OnLoadRequestedPath(HandleFlag handled)
		{
			if (handled.Handled) return;
			if (SprocketPath.Sections[0] == "admin")
			{
				if (!WebAuthentication.Instance.IsLoggedIn)
				{
					HttpContext.Current.Response.Write("Access Denied.");
					handled.Set();
					return;
				}
				switch (SprocketPath.Value)
				{
					case "admin":
						{
							string html = WebUtility.CacheTextFile("resources/admin/frames/admin-iframes.htm");
							//string html = ResourceLoader.LoadTextResource("Sprocket.Web.CMS.Admin.admin-iframes.htm");
							SprocketScript scr = new SprocketScript(html, "Admin Frames", "Admin Frames");
							HttpContext.Current.Response.Write(scr.Execute());
						}
						break;

					case "admin/overlay":
						RenderOverlayPage();
						break;

					case "admin/frames":
						{
							string html = WebUtility.CacheTextFile("resources/admin/frames/admin-frames.htm");
							//string html = ResourceLoader.LoadTextResource("Sprocket.Web.CMS.Admin.admin-frames.htm");
							SprocketScript scr = new SprocketScript(html, "Admin Overlay Frame", "Admin Overlay Frame");
							HttpContext.Current.Response.Write(scr.Execute());
						}
						break;

					case "admin/addressbar":
						{
							string html = WebUtility.CacheTextFile("resources/admin/frames/admin-address-bar.htm");
							//string html = ResourceLoader.LoadTextResource("Sprocket.Web.CMS.Admin.admin-address-bar.htm");
							SprocketScript scr = new SprocketScript(html, "Admin Overlay Frame", "Admin Overlay Frame");
							HttpContext.Current.Response.Write(scr.Execute());
						}
						break;

					default:
						return;
				}
				handled.Set();
			}
		}

		void RenderOverlayPage()
		{
			//string html = ResourceLoader.LoadTextResource("Sprocket.Web.CMS.Admin.admin-iframe-overlay.htm");
			List<IAdminMenuItem> items = new List<IAdminMenuItem>();
			foreach (Type t in Core.Modules.GetInterfaceImplementations(typeof(IAdminMenuItem)))
				items.Add((IAdminMenuItem)Activator.CreateInstance(t));
			items.Sort(delegate(IAdminMenuItem a, IAdminMenuItem b)
			{
				int f = a.Priority.CompareTo(b.Priority);
				if (f != 0) return f;
				return a.MenuLinkText.CompareTo(b.MenuLinkText);
			});
			StringBuilder links = new StringBuilder();
			StringBuilder head = new StringBuilder();
			foreach (IAdminMenuItem item in items)
			{
				links.AppendFormat("<span class=\"divider\"> | </span><a class=\"tab\" href=\"javascript:void(0)\" onclick=\"{0}\">{1}</a>",
					item.MenuLinkOnClick, item.MenuLinkText);
				head.Append(item.HeadContent);
			}

			string html = WebUtility.CacheTextFile("resources/admin/frames/admin-iframe-overlay.htm");
			SprocketScript scr = new SprocketScript(html, "Admin Overlay Frame", "Admin Overlay Frame");
			HttpContext.Current.Response.Write(
				scr.Execute()
				.Replace("[head-content]", head.ToString())
				.Replace("[menu-links]",links.ToString())
			);
		}

		void WebEvents_OnRequestedPathProcessed()
		{
			if (SprocketPath.Value == "$dbsetup")
				return;
			if (!HttpContext.Current.Response.ContentType.Contains("html"))
				return;
			if (OnBeforeDisplayAdminWindowOverlay != null)
			{
				Result result = new Result();
				OnBeforeDisplayAdminWindowOverlay(result);
				if (!result.Succeeded)
					return;
			}
			//string html = WebUtility.CacheTextFile("resources/admin/frames/admin-mode-button.htm");
			//HttpContext.Current.Response.Write(string.Format(html, WebUtility.BasePath));
		}
	}
}
