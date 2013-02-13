using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Sprocket.Web
{
	public static class SprocketPath
	{
		public static string Value
		{
			get { return (string)CurrentRequest.Value["Sprocket.Web.SprocketPath.Value"]; }
			internal set { CurrentRequest.Value["Sprocket.Web.SprocketPath.Value"] = value; }
		}

		public static string[] Sections
		{
			get { return (string[])CurrentRequest.Value["Sprocket.Web.SprocketPath.PathSections"]; }
			internal set { CurrentRequest.Value["Sprocket.Web.SprocketPath.PathSections"] = value; }
		}

		public static string Physical
		{
			get
			{
				string s = CurrentRequest.Value["SprocketPath.Physical"] as string;
				if (s == null)
				{
					s = WebUtility.MapPath(Value);
					CurrentRequest.Value["SprocketPath.Physical"] = s;
				}
				return s;
			}
		}

		public static bool IsPathDescendentOf(string descendentPath, string parentPath)
		{
			if (parentPath.Length + 1 >= descendentPath.Length)
				return false;
			if (descendentPath.Substring(parentPath.Length, 1) != "/")
				return false;
			return descendentPath.StartsWith(parentPath);
		}

		public static bool IsCurrentPathDescendentOf(string parentPath)
		{
			return IsPathDescendentOf(Value, parentPath);
		}

		public static string GetDescendentPath(string descendentPath, string parentPath)
		{
			if (parentPath.Length + 1 >= descendentPath.Length) // don't allow for just a trailing slash
				return "";
			if (descendentPath.Substring(parentPath.Length, 1) != "/")
				return "";
			return descendentPath.Substring(0, parentPath.Length + 1);
		}

		public static string GetDescendentPath(string parentPath)
		{
			string descendentPath = Value;
			if (parentPath.Length + 1 >= descendentPath.Length) // don't allow for just a trailing slash
				return "";
			if (descendentPath.Substring(parentPath.Length, 1) != "/")
				return "";
			return descendentPath.Substring(parentPath.Length + 1);
		}

		public static string ExtractSprocketPath(string fullURL)
		{
			return HttpContext.Current.Request.Path.ToLower().Remove(0, HttpContext.Current.Request.ApplicationPath.Length).Trim('/');
		}
	}
}
