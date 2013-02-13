using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

using Sprocket.Utility;

namespace Sprocket.Web
{
	public static class WebUtility
	{
		public static string MakeFullPath(string sprocketPath)
		{
			int p = sprocketPath.LastIndexOf(".");
			int s = sprocketPath.LastIndexOf("/");
			return BasePath + sprocketPath + (sprocketPath.Length == 0 || p > s ? "" : "/");
		}

		public static string MapPath(string sprocketPath)
		{
			return HttpContext.Current.Server.MapPath(MakeFullPath(sprocketPath));
		}

		private static string basePath = null;
		public static string BasePath
		{
			get
			{
				if (basePath == null)
				{
					basePath = HttpContext.Current.Request.ApplicationPath.Trim('/');
					if (basePath.Length > 0)
						basePath = "/" + basePath + "/";
					else
						basePath = "/";
				}
				return basePath;
			}
		}

		/// <summary>
		/// Gets the web address and virtual directory (if it exists).
		/// e.g. www.mydomain.com/my-virtual-dir
		/// or www.mydomain.com
		/// </summary>
		public static string AbsoluteBasePath
		{
			get
			{
				return (HttpContext.Current.Request.Url.Host.ToString() + BasePath).Trim('/');
			}
		}

		/// <summary>
		/// Gets the URL serving the current request, e.g. http://localhost
		/// </summary>
		public static string BaseURL
		{
			get { return HttpContext.Current.Request.Url.Host.ToString(); }
		}

		private class CachedFile
		{
			public DateTime FileDate;
			public string FileText;
		}

		private static Dictionary<string, CachedFile> txtFileCache = new Dictionary<string, CachedFile>();
		private static Dictionary<string, CachedFile> htmlFileCache = new Dictionary<string, CachedFile>();

		public static string CacheTextFile(string sprocketPath)
		{
			return CacheTextFile(sprocketPath, false);
		}

		public static string CacheTextFile(string sprocketPath, bool condenseJavaScript)
		{
			string path = HttpContext.Current.Request.PhysicalApplicationPath + '\\' + sprocketPath.Replace('/', '\\').Trim('\\');
			DateTime modified = new FileInfo(path).LastWriteTime;
			HttpContext.Current.Application.Lock();
			if (txtFileCache.ContainsKey(path))
			{
				if (txtFileCache[path].FileDate == modified)
				{
					HttpContext.Current.Application.UnLock();
					return txtFileCache[path].FileText;
				}
				txtFileCache.Remove(path);
			}
			StreamReader sr = new StreamReader(path, Encoding.Default);
			string txt = sr.ReadToEnd();
			sr.Close();
			CachedFile file = new CachedFile();
			file.FileDate = modified;
			file.FileText = condenseJavaScript ? JavaScriptCondenser.Condense(txt) : txt;
			txtFileCache.Add(path, file);
			HttpContext.Current.Application.UnLock();
			return txt;
		}

		public static string CacheWebFile(string sprocketRequestPath, string sprocketHtmlFilePath)
		{
			string path = HttpContext.Current.Request.PhysicalApplicationPath + '\\' + sprocketHtmlFilePath.Replace('/', '\\').Trim('\\');
			string refpath = MakeReferencePath(sprocketRequestPath, sprocketHtmlFilePath);

			DateTime modified = new FileInfo(path).LastWriteTime;
			if (htmlFileCache.ContainsKey(path))
			{
				if (htmlFileCache[path].FileDate == modified)
					return htmlFileCache[path].FileText.Replace("[~]", refpath);
				htmlFileCache.Remove(path);
			}
			StreamReader sr = new StreamReader(path);
			string txt = sr.ReadToEnd();
			sr.Close();
			string replace = "${1}[~]";
			string pattern1 = "(?<1>(?:(?:src=)|(?:href=)|(?:background=))(?:'|\"))(?!#|/|([a-zA-Z]+://))";
			string pattern2 = "(?<1>@(?:(?:import)|(?:url)) *(?:'|\"|\\())(?!#|/|([a-zA-Z]+://))";
			RegexOptions options = RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture;
			txt = Regex.Replace(txt, pattern1, replace, options);
			txt = Regex.Replace(txt, pattern2, replace, options);
								   
			CachedFile file = new CachedFile();
			file.FileDate = modified;
			file.FileText = txt;
			htmlFileCache.Add(path, file);
			return txt.Replace("[~]", refpath);
		}

		public static string MakeReferencePath(string sprocketRequestPath, string sprocketHtmlFilePath)
		{
			string[] pathTo = sprocketHtmlFilePath.ToLower().Replace("\\", "/").Trim('/').Split('/');
			string[] pathFrom = sprocketRequestPath.ToLower().Replace("\\", "/").Trim('/').Split('/');

			int index = 0;
			while (index < pathTo.Length && index < pathFrom.Length)
				if (pathTo[index] == pathFrom[index])
					index++;
				else
					break;
			string refpath = HttpContext.Current.Request.ApplicationPath + "/"; // "";
			int lenPathFrom = pathFrom.Length - 1; // pathFrom[pathFrom.Length - 1].Contains(".") ? pathFrom.Length : pathFrom.Length - 1;
			int lenPathTo = pathTo[pathTo.Length - 1].Contains(".") ? pathTo.Length - 1 : pathTo.Length;
			//for (int i = index; i < lenPathFrom; i++)
			//	refpath += "../";
			for (int i = index; i < lenPathTo; i++)
				refpath += pathTo[i] + "/";
			return refpath;
		}

		public static void Redirect(string sprocketPath)
		{
			HttpContext.Current.Response.Redirect(BasePath + sprocketPath);
			HttpContext.Current.Response.End();
		}

		public static string RemoveHTMLTags(string input, params string[] tagNameExceptions)
		{
			Regex rx = new Regex(@"</?([a-zA-Z\:])+[^>]*>", RegexOptions.IgnoreCase);
			foreach (Match match in rx.Matches(input))
			{
				if (StringUtilities.MatchesAny(match.Groups[1].Value, tagNameExceptions))
					continue;
				input = input.Replace(match.Value, "");
			}
			return input;
		}

		public static string RawQueryString
		{
			get
			{
				string url = HttpContext.Current.Request.RawUrl;
				string[] arr = url.Split('?');
				if (arr.Length == 1)
					return "";
				// doing it this way allows extra ?s to appear in the querystring
				return url.Remove(0, arr[0].Length + 1);
			}
		}

		public static object GetSyncObject(string name)
		{
			HttpApplicationState app = HttpContext.Current.Application;
			string n = "_LOCKSYNC_" + name;
			app.Lock();
			object sync = app[n];
			if (sync == null)
			{
				sync = new object();
				app[n] = sync;
			}
			app.UnLock();
			return sync;
		}

		public static string UrlEncode(string s)
		{
			return HttpUtility.UrlEncode(s).Replace("+", "%20");
		}

		public static string SafeHtmlString(string s, bool convertLineBreaks)
		{
			StringBuilder sb = new StringBuilder();
			string[] arr = Regex.Split(Regex.Replace(s,
					@"\</?(\<\!|script|style|iframe|link|head|body|html|frameset|frame|noframes|embed|object|applet|form)[^>]*\>?",
					"", RegexOptions.IgnoreCase), @"(\<[^\S]*[^>]*\>)", RegexOptions.IgnoreCase);
			Regex removeJavaScriptEvents = new Regex(@"(?<!\S)on\S+\s*\=", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
			Regex removeNastyStyles = new Regex(@"(expression\s*:\s*)|(position\s*:\s*(absolute|fixed)\s*;?)", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
			for (int i = 0; i < arr.Length; i++)
			{
				if (arr[i].StartsWith("<"))
					sb.Append(removeNastyStyles.Replace(removeJavaScriptEvents.Replace(arr[i], ""), ""));
				else
					sb.Append(arr[i]);
			}
			if (convertLineBreaks)
				return sb.ToString()
					.Replace(Environment.NewLine, "<br />")
					.Replace("\n", "<br />");
			else
				return sb.ToString();
		}
	}
}
