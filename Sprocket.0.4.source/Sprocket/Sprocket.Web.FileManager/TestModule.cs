using System;
using System.Drawing;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Data.SqlClient;
using System.IO;

using Sprocket;
using Sprocket.Data;
using Sprocket.Utility;
using Sprocket.Web;

namespace Sprocket.Web.FileManager
{
	[AjaxMethodHandler("TestModule")]
	[ModuleDependency(typeof(WebEvents))]
	[ModuleDependency(typeof(FileManager))]
	[ModuleDescription("A module for writing test code.")]
	[ModuleTitle("FileManager Testing Module")]
	public class TestModule //: ISprocketModule
	{
		HttpRequest Request { get { return HttpContext.Current.Request; } }
		HttpResponse Response { get { return HttpContext.Current.Response; } }

		public void AttachEventHandlers(ModuleRegistry registry)
		{
			WebEvents.Instance.OnLoadRequestedPath += new WebEvents.RequestedPathEventHandler(OnLoadRequestedPath);
		}

		void OnLoadRequestedPath(HandleFlag handled)
		{
			switch (SprocketPath.Value)
			{
				case "test":
					Response.Write("<form method=\"post\" action=\""
						+ WebUtility.BasePath + "test/upload/\" enctype=\"multipart/form-data\">"
						+ "<input type=\"file\" size=\"40\" name=\"thefile\" /> <input type=\"submit\" value=\"upload\" />"
						+ "</form>"
						);
					break;

				case "test/upload":
					HttpPostedFile posted = HttpContext.Current.Request.Files[0];
					SprocketFile file = new SprocketFile(Security.SecurityProvider.ClientSpaceID, posted, "Test Image", "A test image.");
					FileManager.DataLayer.Store(file);
					WebUtility.Redirect("test/show/?" + file.SprocketFileID);
					break;

				case "test/show":
					long id = long.Parse(WebUtility.RawQueryString);
					SizingOptions options = new SizingOptions(320, 180, 10, Color.Black, Color.CadetBlue, 2, SizingOptions.Display.Letterbox, id);
					Response.Write("<img src=\"" + WebUtility.BasePath + "test/image/" + options.Filename + "?nocache\" hspace=\"5\" vspace=\"5\" /> ");

					options = new SizingOptions(200, 200, 0, Color.Black, Color.CadetBlue, 0, SizingOptions.Display.Letterbox, id);
					Response.Write("<img src=\"" + WebUtility.BasePath + "test/image/" + options.Filename + "?nocache\" hspace=\"5\" vspace=\"5\" /> ");
					
					options = new SizingOptions(200, 200, 0, Color.Black, Color.CadetBlue, 0, SizingOptions.Display.Stretch, id);
					Response.Write("<img src=\"" + WebUtility.BasePath + "test/image/" + options.Filename + "?nocache\" hspace=\"5\" vspace=\"5\" /> ");
					
					options = new SizingOptions(100, 200, 0, Color.Black, Color.CadetBlue, 0, SizingOptions.Display.Letterbox, id);
					Response.Write("<img src=\"" + WebUtility.BasePath + "test/image/" + options.Filename + "?nocache\" hspace=\"5\" vspace=\"5\" /> ");
					
					options = new SizingOptions(100, 100, 10, Color.White, Color.FromArgb(240, 240, 240), 1, SizingOptions.Display.Letterbox, id);
					Response.Write("<img src=\"" + WebUtility.BasePath + "test/image/" + options.Filename + "?nocache\" hspace=\"5\" vspace=\"5\" /> ");

					options = new SizingOptions(100, 100, 0, Color.Black, Color.CadetBlue, 0, SizingOptions.CropAnchor.Top, id);
					Response.Write("<img src=\"" + WebUtility.BasePath + "test/image/" + options.Filename + "?nocache\" hspace=\"5\" vspace=\"5\" /> ");

					options = new SizingOptions(100, 100, 15, Color.Black, Color.Red, 5, SizingOptions.CropAnchor.Top, id);
					Response.Write("<img src=\"" + WebUtility.BasePath + "test/image/" + options.Filename + "?nocache\" hspace=\"5\" vspace=\"5\" /> ");

					options = new SizingOptions(100, 100, 0, Color.Black, Color.CadetBlue, 0, SizingOptions.CropAnchor.Bottom, id);
					Response.Write("<img src=\"" + WebUtility.BasePath + "test/image/" + options.Filename + "?nocache\" hspace=\"5\" vspace=\"5\" /> ");

					options = new SizingOptions(100, 100, 15, Color.Black, Color.Red, 5, SizingOptions.CropAnchor.Bottom, id);
					Response.Write("<img src=\"" + WebUtility.BasePath + "test/image/" + options.Filename + "?nocache\" hspace=\"5\" vspace=\"5\" /> ");

					options = new SizingOptions(100, 100, 0, Color.Black, Color.CadetBlue, 0, SizingOptions.CropAnchor.Center, id);
					Response.Write("<img src=\"" + WebUtility.BasePath + "test/image/" + options.Filename + "?nocache\" hspace=\"5\" vspace=\"5\" /> ");

					options = new SizingOptions(100, 100, 15, Color.Black, Color.Red, 5, SizingOptions.CropAnchor.Center, id);
					Response.Write("<img src=\"" + WebUtility.BasePath + "test/image/" + options.Filename + "?nocache\" hspace=\"5\" vspace=\"5\" /> ");

					options = new SizingOptions(100, 100, 0, Color.Black, Color.CadetBlue, 0, SizingOptions.CropAnchor.Left, id);
					Response.Write("<img src=\"" + WebUtility.BasePath + "test/image/" + options.Filename + "?nocache\" hspace=\"5\" vspace=\"5\" /> ");

					options = new SizingOptions(100, 100, 15, Color.Black, Color.Red, 5, SizingOptions.CropAnchor.Left, id);
					Response.Write("<img src=\"" + WebUtility.BasePath + "test/image/" + options.Filename + "?nocache\" hspace=\"5\" vspace=\"5\" /> ");

					options = new SizingOptions(100, 100, 0, Color.Black, Color.CadetBlue, 0, SizingOptions.CropAnchor.Right, id);
					Response.Write("<img src=\"" + WebUtility.BasePath + "test/image/" + options.Filename + "?nocache\" hspace=\"5\" vspace=\"5\" /> ");

					options = new SizingOptions(100, 100, 15, Color.Black, Color.Red, 5, SizingOptions.CropAnchor.Right, id);
					Response.Write("<img src=\"" + WebUtility.BasePath + "test/image/" + options.Filename + "?nocache\" hspace=\"5\" vspace=\"5\" /> ");

					options = new SizingOptions(100, 100, 0, Color.Black, Color.CadetBlue, 0, SizingOptions.Display.Center, id);
					Response.Write("<img src=\"" + WebUtility.BasePath + "test/image/" + options.Filename + "?nocache\" hspace=\"5\" vspace=\"5\" /> ");

					options = new SizingOptions(100, 100, 15, Color.Black, Color.Red, 5, SizingOptions.Display.Center, id);
					Response.Write("<img src=\"" + WebUtility.BasePath + "test/image/" + options.Filename + "?nocache\" hspace=\"5\" vspace=\"5\" /> ");

					options = new SizingOptions(400, 300, 10, Color.Black, Color.CadetBlue, 0, 10, id);
					Response.Write("<img src=\"" + WebUtility.BasePath + "test/image/" + options.Filename + "?nocache\" hspace=\"5\" vspace=\"5\" align=\"top\" /> ");
					break;

				default:
					if (SprocketPath.Value.EndsWith(".jpg") && SprocketPath.Value.StartsWith("test/image/"))
					{
						FileManager.Instance.TransmitImage(SprocketPath.Sections[SprocketPath.Sections.Length - 1]);
						break;
					}
					return;
			}
			handled.Set();
		}
	}
}
