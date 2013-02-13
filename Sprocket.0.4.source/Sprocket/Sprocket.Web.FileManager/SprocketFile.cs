using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Xml;

using Sprocket;
using Sprocket.Web;
using Sprocket.Data;

namespace Sprocket.Web.FileManager
{
	public class SprocketFile
	{
		#region Fields

		protected long sprocketFileID = 0;
		protected long clientSpaceID = 0;
		protected byte[] fileData = null;
		protected string fileTypeExtension = "";
		protected string originalFileName = "";
		protected string contentType = "";
		protected string title = "";
		protected string description = "";
		private long dataLength = 0;
		protected DateTime uploadDate = DateTime.MinValue;

		#endregion

		#region Properties

		public long DataLength
		{
			get { return dataLength; }
		}

		///<summary>
		///Gets or sets the value for SprocketFileID
		///</summary>
		public long SprocketFileID
		{
			get { return sprocketFileID; }
			set { sprocketFileID = value; }
		}

		///<summary>
		///Gets or sets the value for ClientSpaceID
		///</summary>
		public long ClientSpaceID
		{
			get { return clientSpaceID; }
			set { clientSpaceID = value; }
		}

		///<summary>
		///Gets or sets the value for FileData
		///</summary>
		public byte[] FileData
		{
			get { return fileData; }
			set
			{
				fileData = value;
				dataLength = fileData.Length;
			}
		}

		///<summary>
		///Gets or sets the value for FileTypeExtension
		///</summary>
		public string FileTypeExtension
		{
			get { return fileTypeExtension; }
			set { fileTypeExtension = value; }
		}

		///<summary>
		///Gets or sets the value for OriginalFileName
		///</summary>
		public string OriginalFileName
		{
			get { return originalFileName; }
			set { originalFileName = value; }
		}

		///<summary>
		///Gets or sets the value for ContentType
		///</summary>
		public string ContentType
		{
			get { return contentType; }
			set { contentType = value; }
		}

		///<summary>
		///Gets or sets the value for Title
		///</summary>
		public string Title
		{
			get { return title; }
			set { title = value; }
		}

		///<summary>
		///Gets or sets the value for Description
		///</summary>
		public string Description
		{
			get { return description; }
			set { description = value; }
		}

		///<summary>
		///Gets or sets the value for UploadDate
		///</summary>
		public DateTime UploadDate
		{
			get { return uploadDate; }
			set { uploadDate = value; }
		}

		#endregion

		#region Constructors

		public SprocketFile()
		{
		}

		public SprocketFile(long sprocketFileID, long clientSpaceID, byte[] fileData, string fileTypeExtension, string originalFileName, string contentType, string title, string description, DateTime uploadDate)
		{
			this.sprocketFileID = sprocketFileID;
			this.clientSpaceID = clientSpaceID;
			this.fileData = fileData;
			this.fileTypeExtension = fileTypeExtension;
			this.originalFileName = originalFileName;
			this.contentType = contentType;
			this.title = title;
			this.description = description;
			this.uploadDate = uploadDate;
			dataLength = fileData.Length;
		}

		public SprocketFile(long clientSpaceID, HttpPostedFile fileData, string title, string description)
		{
			this.clientSpaceID = clientSpaceID;
			this.fileData = new byte[fileData.ContentLength];
			fileData.InputStream.Read(this.fileData, 0, fileData.ContentLength);
			this.originalFileName = fileData.FileName;
			string[] arr = fileData.FileName.Split('.');
			if (arr.Length > 1)
				fileTypeExtension = arr[1];
			else
				fileTypeExtension = "";
			this.contentType = fileData.ContentType;
			this.title = title;
			this.description = description;
			this.uploadDate = SprocketDate.Now;
			dataLength = fileData.ContentLength;
		}

		public SprocketFile(IDataReader reader)
		{
			if (reader["SprocketFileID"] != DBNull.Value) sprocketFileID = (long)reader["SprocketFileID"];
			if (reader["ClientSpaceID"] != DBNull.Value) clientSpaceID = (long)reader["ClientSpaceID"];
			if (reader["FileData"] != DBNull.Value) fileData = (byte[])reader["FileData"];
			if (reader["FileTypeExtension"] != DBNull.Value) fileTypeExtension = (string)reader["FileTypeExtension"];
			if (reader["OriginalFileName"] != DBNull.Value) originalFileName = (string)reader["OriginalFileName"];
			if (reader["ContentType"] != DBNull.Value) contentType = (string)reader["ContentType"];
			if (reader["Title"] != DBNull.Value) title = (string)reader["Title"];
			if (reader["Description"] != DBNull.Value) description = (string)reader["Description"];
			if (reader["UploadDate"] != DBNull.Value) uploadDate = (DateTime)reader["UploadDate"];
			if (reader["DataLength"] != DBNull.Value) dataLength = (long)reader["DataLength"];
		}

		#endregion

		#region JSON Methods

		/// <summary>
		/// Writes this entity out as a JSON formatted string
		/// </summary>
		public void WriteJSON(StringWriter writer)
		{
			writer.Write("{");
			JSON.EncodeNameValuePair(writer, "SprocketFileID", sprocketFileID);
			writer.Write(",");
			JSON.EncodeNameValuePair(writer, "ClientSpaceID", clientSpaceID);
			writer.Write(",");
			JSON.EncodeNameValuePair(writer, "FileTypeExtension", fileTypeExtension);
			writer.Write(",");
			JSON.EncodeNameValuePair(writer, "OriginalFileName", originalFileName);
			writer.Write(",");
			JSON.EncodeNameValuePair(writer, "ContentType", contentType);
			writer.Write(",");
			JSON.EncodeNameValuePair(writer, "Title", title);
			writer.Write(",");
			JSON.EncodeNameValuePair(writer, "Description", description);
			writer.Write(",");
			JSON.EncodeNameValuePair(writer, "UploadDate", uploadDate);
			writer.Write(",");
			JSON.EncodeNameValuePair(writer, "DataLength", dataLength);
			writer.Write("}");
		}

		#endregion

		public static string GetCachePath(long sprocketFileID, string filename)
		{
			long f1 = sprocketFileID % 200;
			long f2 = ((sprocketFileID - f1) / 200) % 200;
			return WebUtility.MapPath(string.Format("datastore/filecache/{0}/{1}/{2}", f1, f2, filename));
		}
	}
}

#region old
/*
		public static SprocketFile Upload(HttpPostedFile upload, Guid? clientID, Guid? ownerID,
			Guid? parentFileID, string sprocketPath, string categoryCode, string moduleRegCode,
			string description)
		{
			if (upload.ContentLength > int.Parse(SprocketSettings.GetValue("FileManagerMaxUploadSizeBytes")))
				return null;

			SprocketFile file = new SprocketFile();
			file.sprocketFileID = Guid.NewGuid();
			file.clientID = clientID;
			file.ownerID = ownerID;
			file.parentFileID = parentFileID;
			file.sprocketPath = (sprocketPath.Trim('/') + "/" + Path.GetFileName(upload.FileName)).Trim('/');
			file.categoryCode = categoryCode;
			file.moduleRegCode = moduleRegCode;
			file.description = description;
			file.contentType = upload.ContentType;
			file.uploadDate = SprocketDate.Now;
			file.FileTypeExtension = Path.GetExtension(upload.FileName);
			upload.SaveAs(file.PhysicalPath);
			if (Database.Main.IsTransactionActive)
				file.Save();
			else
			{
				Database.Main.BeginTransaction();
				try
				{
					file.Save();
				}
				catch (Exception ex)
				{
					Database.Main.RollbackTransaction();
					file.EnsureFileDeleted();
					throw ex;
				}
				Database.Main.CommitTransaction();
			}
			return file;
		}

		public void EnsureFileDeleted()
		{
			if (File.Exists(PhysicalPath))
			{
				// Wrapping in a try/catch block ensures that if windows has locked the file
				// for some reason, the application doesn't crash. The file can always be
				// deleted later.
				try { File.Delete(PhysicalPath); }
				catch { } 
				// now delete any corresponding thumbnails
				if (!IsImage)
					return;
				foreach(string file in Directory.GetFiles(
					WebUtility.MapPath("datastore/filemanager/thumbnails"), sprocketFileID.ToString() + "*.*"))
					try { File.Delete(file);}
					catch { }
			}
		}

		public string GetThumbnailPhysicalPath(ThumbnailOptions options)
		{
			string thumbPath = WebUtility.MapPath("datastore/filemanager/thumbnails/" + options.GenerateFilename(sprocketFileID.Value));
			if (File.Exists(thumbPath))
			{
				if (new FileInfo(thumbPath).CreationTime > new FileInfo(PhysicalPath).CreationTime)
					return thumbPath;
				else
					File.Delete(thumbPath);
			}
			Image img = Image.FromFile(PhysicalPath);
			Image thumb = new Bitmap(options.OuterWidth, options.OuterHeight);
			Graphics gfx = Graphics.FromImage(thumb);
			gfx.CompositingQuality = CompositingQuality.HighQuality;
			gfx.SmoothingMode = SmoothingMode.HighQuality;
			gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;
			Brush fill = new SolidBrush(options.BackgroundColor);
			Pen pen = new Pen(options.BorderColor, options.BorderWidth);
			pen.Alignment = PenAlignment.Inset;
			gfx.FillRectangle(fill, 0, 0, options.OuterWidth, options.OuterHeight);
			if(options.BorderWidth > 0)
				gfx.DrawRectangle(pen, 0, 0, options.OuterWidth-1, options.OuterHeight-1);
			Rectangle rect = new Rectangle(0, 0, img.Width, img.Height);
			if (img.Width > img.Height)
			{
				rect.Width = options.InnerWidth;
				rect.Height = (int)(((float)img.Height / (float)img.Width) * (float)options.InnerWidth);
			}
			else
			{
				rect.Height = options.InnerHeight;
				rect.Width = (int)(((float)img.Width / (float)img.Height) * (float)options.InnerHeight);
			}
			int xremain = options.OuterWidth - rect.Width;
			int yremain = options.OuterHeight - rect.Height;
			rect.X = xremain / 2;
			rect.Y = yremain / 2;
			if (xremain % 2 == 1)
				rect.Width++;
			if (yremain % 2 == 1)
				rect.Height++;
			gfx.DrawImage(img, rect);
			ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();
			ImageCodecInfo encoder = null;
			for(int i=0; i<encoders.Length; i++)
				if(encoders[i].MimeType == "image/jpeg")
				{
					encoder = encoders[i];
					break;
				}
			if(encoder == null)
				throw new SprocketException("Can't create a thumbnail because no JPEG encoder exists.");
			EncoderParameters prms = new EncoderParameters(1);
			prms.Param[0] = new EncoderParameter(Encoder.Quality, 70L);
			thumb.Save(thumbPath, encoder, prms);
			img.Dispose();
			thumb.Dispose();
			return thumbPath;
		}

		public bool IsImage
		{
			get { return Utility.Utilities.MatchesAny(fileTypeExtension, "jpg", "gif", "png", "bmp"); }
		}

		public void RenderThumbnailToHttpOutput(ThumbnailOptions options)
		{
			HttpContext.Current.Response.ContentType = "image/jpeg";
			HttpContext.Current.Response.WriteFile(GetThumbnailPhysicalPath(options));
		}

		public void RenderFileTypeIconToHttpOutput()
		{
			Dictionary<string, KeyValuePair<string, string>> icons = GetFileTypeDefinitions();
			string ext = FileTypeExtension;
			if (!icons.ContainsKey(FileTypeExtension)) ext = "*";
			HttpContext.Current.Response.ContentType = "image/gif";
			HttpContext.Current.Response.WriteFile(icons[ext].Key);
		}

		public void RenderIconToHttpOutput()
		{
			if (IsImage)
				RenderThumbnailToHttpOutput(ThumbnailOptions.SprocketStandard);
			else
				RenderFileTypeIconToHttpOutput();
		}

		public void GetFileTypeInfo(out string iconPhysicalPath, out string fileTypeName)
		{
			Dictionary<string, KeyValuePair<string, string>> icons = GetFileTypeDefinitions();
			string ext = FileTypeExtension;
			if (!icons.ContainsKey(FileTypeExtension)) ext = "*";
			KeyValuePair<string, string> kvp = icons[ext];
			iconPhysicalPath = kvp.Key;
			fileTypeName = kvp.Value;
		}

		private Dictionary<string, KeyValuePair<string, string>> GetFileTypeDefinitions()
		{
			HttpApplicationState app = HttpContext.Current.Application;
			string xmlPath = WebUtility.MapPath("resources/filemanager/file-type-definitions.xml");
			DateTime timestamp = DateTime.MinValue;
			if (app["FileManager_FileTypeIcons_TimeStamp"] != null)
				timestamp = (DateTime)app["FileManager_FileTypeIcons_TimeStamp"];
			Dictionary<string, KeyValuePair<string, string>> icons;
			if (new FileInfo(xmlPath).LastWriteTime <= timestamp)
				icons = (Dictionary<string, KeyValuePair<string, string>>)app["FileManager_FileTypeIcons_TimeStamp"];
			else
			{
				icons = new Dictionary<string, KeyValuePair<string, string>>();
				XmlDocument doc = new XmlDocument();
				doc.Load(xmlPath);
				foreach (XmlNode node in doc.DocumentElement.ChildNodes)
					if (node.NodeType == XmlNodeType.Element)
					{
						string[] exts = ((XmlElement)node).GetAttribute("extension").Split(',');
						string iconPath = WebUtility.MapPath("resources/filemanager/file-type-icons/" + ((XmlElement)node).GetAttribute("icon"));
						string fileType = node.FirstChild.Value;
						foreach (string ext in exts)
							icons[ext.ToLower()] = new KeyValuePair<string, string>(iconPath, fileType);
					}
			}
			return icons;
		}

		public void Delete()
		{
			Database.Main.RememberOpenState();
			IDbCommand cmd = Database.Main.CreateCommand("DeleteSprocketFile", CommandType.StoredProcedure);
			Database.Main.AddParameter(cmd, "@SprocketFileID", sprocketFileID);
			cmd.ExecuteNonQuery();
			Database.Main.CloseIfWasntOpen();
			foreach (string file in Directory.GetFiles(WebUtility.MapPath("datastore/filemanager"),
				sprocketFileID.ToString() + "*.*", SearchOption.AllDirectories))
				try { File.Delete(file); }
				catch { }
			FileManager.Instance.NotifyFileDeleted(this);
		}
	}

	public class ThumbnailOptions
	{
		private int outerWidth = 60;
		private int outerHeight = 60;
		private int innerWidth = 40;
		private int innerHeight = 40;
		private Color backgroundColor = Color.White;
		private Color borderColor = Color.Transparent;
		private int borderWidth = 1;

		public ThumbnailOptions() { }
		public ThumbnailOptions(int outerWidth, int outerHeight, int innerWidth, int innerHeight,
			Color backgroundColor, Color borderColor, int borderWidth)
		{
			this.innerHeight = innerHeight;
			this.innerWidth = innerWidth;
			this.outerHeight = outerHeight;
			this.outerWidth = outerWidth;
			this.backgroundColor = backgroundColor;
			this.borderColor = borderColor;
			this.borderWidth = borderWidth;
		}

		public string GenerateFilename(Guid fileID)
		{
			return string.Format("{0}_{1}x{2}_{3}x{4}_{5}_{6}_{7}.thumb",
				fileID, outerWidth, outerHeight, innerWidth, innerHeight,
				backgroundColor.ToArgb(), borderColor.ToArgb(), borderWidth);
		}

		public static ThumbnailOptions SprocketStandard
		{
			get { return new ThumbnailOptions(60, 60, 50, 50, Color.White, Color.FromArgb(204, 204, 204), 1); }
		}

		#region Class Properties

		public int OuterWidth
		{
			get { return outerWidth; }
			set { outerWidth = value; }
		}

		public int OuterHeight
		{
			get { return outerHeight; }
			set { outerHeight = value; }
		}

		public int InnerWidth
		{
			get { return innerWidth; }
			set { innerWidth = value; }
		}

		public int InnerHeight
		{
			get { return innerHeight; }
			set { innerHeight = value; }
		}

		public Color BackgroundColor
		{
			get { return backgroundColor; }
			set { backgroundColor = value; }
		}

		public Color BorderColor
		{
			get { return borderColor; }
			set { borderColor = value; }
		}

		public int BorderWidth
		{
			get { return borderWidth; }
			set { borderWidth = value; }
		}

		#endregion
		*/
#endregion
