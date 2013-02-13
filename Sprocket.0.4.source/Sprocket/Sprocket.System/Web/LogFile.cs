using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Sprocket.Web
{
	public class LogFile
	{
		public static void Append(string filename, params string[] values)
		{
			LogFile log = new LogFile(filename);
			log.Write(values);
		}

		private FileInfo info;
		public LogFile(string filename)
		{
			info = new FileInfo(WebUtility.MapPath("datastore/logs/" + filename));
		}

		public void Write(params string[] values)
		{
			try
			{
				if (!info.Directory.Exists)
					info.Directory.Create();
				using (StreamWriter writer = new StreamWriter(info.FullName, true))
				{
					writer.Write(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
					foreach (string str in values)
					{
						writer.Write("\t");
						writer.Write(str);
					}
					writer.Flush();
					writer.Close();
				}
			}
			catch
			{
			}
		}
	}
}
