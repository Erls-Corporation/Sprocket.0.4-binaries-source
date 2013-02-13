using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Web;

using Sprocket;
using Sprocket.Web;

namespace Sprocket.Web
{
	/// <summary>
	/// The ContentCache class provides a method where generated content can be cached to disk
	/// for future requests to speed up further calls to specific URLs. The ASP.Net framework
	/// provides a caching mechanism also, but it caches directly to memory, which is very fast,
	/// but could possibly create a problem if we're trying to cache hundreds or thousands of
	/// pages for an unknown period of time.
	/// </summary>
	[ModuleDependency(typeof(WebEvents))]
	[ModuleDescription("Simple replacement for the ASP.Net cache controls. Handles caching of requested content to disk instead of memory")]
	[ModuleTitle("Content Cache")]
	public class ContentCache : ISprocketModule
	{
		#region private methods
		private static void VerifyCacheExists()
		{
			Directory.CreateDirectory(WebUtility.MapPath(CacheDirectorySprocketPath));
		}

		private static string CacheDirectorySprocketPath
		{
			get { return "datastore/content-cache"; }
		}

		private static string GetCacheFilePath(string sprocketPath)
		{
			return WebUtility.MapPath(CacheDirectorySprocketPath + "/" + sprocketPath.Replace("/", "~") + ".cache");
		}
		#endregion

		public static bool IsContentCacheDisabled
		{
			get { return SprocketSettings.GetBooleanValue("CacheDisabled"); }
		}

		public static bool IsContentCached(string sprocketPath)
		{
			if (IsContentCacheDisabled)
				return false;

			VerifyCacheExists();

			return File.Exists(GetCacheFilePath(sprocketPath));
		}

		public static bool CacheContent(string sprocketPath, string content)
		{
			VerifyCacheExists();
			if (IsContentCacheDisabled)
				return false;

			try
			{
				StreamWriter sw = new StreamWriter(GetCacheFilePath(sprocketPath));
				sw.Write(content);
				sw.Flush();
				sw.Close();
				return true;
			}
			catch
			{
				return false;
			}
		}

		public static void ClearCache(params string[] sprocketPaths)
		{
			VerifyCacheExists();

			if (sprocketPaths.Length == 0)
			{
				foreach (string file in Directory.GetFiles(WebUtility.MapPath(CacheDirectorySprocketPath), "*.*", SearchOption.AllDirectories))
					File.Delete(file);
				return;
			}

			foreach (string path in sprocketPaths)
			{
				string p = GetCacheFilePath(path);
				if (File.Exists(p))
					File.Delete(p);
			}
		}

		public static string ReadCache(string sprocketPath)
		{
			VerifyCacheExists();

			StreamReader sr;
			sr = new StreamReader(GetCacheFilePath(sprocketPath));
			string str = sr.ReadToEnd();
			sr.Close();
			return str;
		}

		#region ISprocketModule Members

		public void AttachEventHandlers(ModuleRegistry registry)
		{
			WebEvents.Instance.OnLoadRequestedPath += new WebEvents.RequestedPathEventHandler(ContentCache_OnLoadRequestedPath);
		}

		public event InterruptableEventHandler OnCacheClearanceRequested;
		void ContentCache_OnLoadRequestedPath(HandleFlag handled)
		{
			if (handled.Handled)
				return;
			else if (SprocketPath.Value == "$clear-cache")
			{
				if (OnCacheClearanceRequested != null)
				{
					Result r = new Result();
					OnCacheClearanceRequested(r);
					if (!r.Succeeded)
					{
						HttpContext.Current.Response.Write(r.Message);
						handled.Set();
						return;
					}
				}
				ClearCache();
				HttpContext.Current.Response.Write("The cache has been cleared.");
				handled.Set();
			}
			else if (SprocketPath.Value == "datastore\\content-cache" || SprocketPath.Value.StartsWith("datastore\\content-cache\\"))
			{
				handled.Set();
				HttpContext.Current.Response.Write("Access denied.");
			}
		}

		#endregion
	}
}
