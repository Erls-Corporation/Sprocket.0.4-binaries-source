using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Sprocket.Web.FileManager
{
	public class SizingOptions
	{
		private int width = 0;
		private int height = 0;
		private int padding = 0;
		private Color backgroundColor = Color.Black;
		private Color borderColor = Color.Transparent;
		private int borderSize = 0;
		private int tileSpacing = 0;
		private Display displayType = Display.Letterbox;
		private CropAnchor anchor = CropAnchor.Center;
		private bool preventEnlargement = false;
		
		private string filename = "";
		private long sprocketFileID = 0;
		private Image image = null;
		private string physicalPath = "";
		private long jpegQuality = 180;

		public enum Display
		{
			/// <summary>
			/// Fits the image inside the area, maintaining aspect ratio and leaving the background showing through, letterbox style
			/// </summary>
			Letterbox = 0,
			/// <summary>
			/// Stretches the image to exactly fit the dimensions
			/// </summary>
			Stretch = 1,
			/// <summary>
			/// Tiles the image inside the area until the background is covered over
			/// </summary>
			Tile = 2,
			/// <summary>
			/// Fits the smaller dimension of the image into the area, cropping off the remaining portion(s) of the image
			/// </summary>
			Crop = 3,
			/// <summary>
			/// Centers the image inside the area without sizing it up or down
			/// </summary>
			Center = 4,
			/// <summary>
			/// Resizes the image and maintains its aspect ratio whilst making sure that neither dimension in the resultant image is larger than
			/// the specified dimensions. This means in most cases, the final image will only match one of the specified dimensions as the other
			/// dimension will be reduced or enlarged to maintain the original aspect ratio.
			/// </summary>
			Constrain = 5
		}

		public enum CropAnchor
		{
			Top = 0,
			Right = 1,
			Bottom = 2,
			Left = 3,
			Center = 4
		}

		private void SetFilename()
		{
			filename = string.Format(
				"{0}_{1}_{2}_{3}_{4}_{5}_{6}_{7}_{8}_{9}_{10}.jpg",
				sprocketFileID,
				width,
				height,
				padding,
				backgroundColor.ToArgb(),
				borderColor.ToArgb(),
				borderSize,
				tileSpacing,
				(int)displayType,
				(int)anchor,
				preventEnlargement ? 1 : 0
			);
			long f1 = sprocketFileID % 200 + 1;
			long f2 = ((sprocketFileID - f1) / 200) % 200 + 1;
			physicalPath = WebUtility.MapPath(string.Format("datastore/filecache/{0}/{1}/{2}", f1, f2, filename));
		}

		private SizingOptions() { }
		public SizingOptions(
			int width, int height, int padding,
			Color backgroundColor,
			Color borderColor, int borderSize,
			Display displayType,
			long sprocketFileID)
		{
			this.padding = padding;
			this.height = height;
			this.width = width;
			this.backgroundColor = backgroundColor;
			this.borderColor = borderColor;
			this.borderSize = borderSize;
			this.displayType = displayType;
			this.sprocketFileID = sprocketFileID;
			SetFilename();
		}

		public SizingOptions(
			int width, int height, int padding,
			Color backgroundColor,
			Color borderColor, int borderSize,
			CropAnchor anchor,
			long sprocketFileID)
		{
			this.padding = padding;
			this.height = height;
			this.width = width;
			this.backgroundColor = backgroundColor;
			this.borderColor = borderColor;
			this.borderSize = borderSize;
			this.displayType = Display.Crop;
			this.anchor = anchor;
			this.sprocketFileID = sprocketFileID;
			SetFilename();
		}

		public SizingOptions(
			int width, int height, int padding,
			Color backgroundColor,
			Display displayType,
			long sprocketFileID)
		{
			this.padding = padding;
			this.height = height;
			this.width = width;
			this.backgroundColor = backgroundColor;
			this.displayType = displayType;
			this.sprocketFileID = sprocketFileID;
			SetFilename();
		}

		public SizingOptions(
			int width, int height, int padding,
			Color backgroundColor,
			CropAnchor anchor,
			long sprocketFileID)
		{
			this.padding = padding;
			this.height = height;
			this.width = width;
			this.backgroundColor = backgroundColor;
			this.displayType = Display.Crop;
			this.anchor = anchor;
			this.sprocketFileID = sprocketFileID;
			SetFilename();
		}

		public SizingOptions(
			int width, int height,
			Display displayType,
			long sprocketFileID)
		{
			this.height = height;
			this.width = width;
			this.displayType = displayType;
			this.sprocketFileID = sprocketFileID;
			SetFilename();
		}

		public SizingOptions(
			int width, int height, int padding,
			Color backgroundColor,
			Color borderColor, int borderSize,
			int tileSpacing,
			long sprocketFileID)
		{
			this.padding = padding;
			this.height = height;
			this.width = width;
			this.backgroundColor = backgroundColor;
			this.borderColor = borderColor;
			this.borderSize = borderSize;
			this.displayType = Display.Tile;
			this.tileSpacing = tileSpacing;
			this.sprocketFileID = sprocketFileID;
			SetFilename();
		}

		public SizingOptions(
			int width, int height, int padding,
			Color backgroundColor,
			int tileSpacing,
			long sprocketFileID)
		{
			this.padding = padding;
			this.height = height;
			this.width = width;
			this.backgroundColor = backgroundColor;
			this.displayType = Display.Tile;
			this.tileSpacing = tileSpacing;
			this.sprocketFileID = sprocketFileID;
			SetFilename();
		}

		public static SizingOptions Parse(string filename)
		{
			SizingOptions o = new SizingOptions();
			string[] arr = filename.Split('.');
			string[] dim = arr[0].Split('_');
			if (arr.Length != 2 || dim.Length != 11)
				return null;
			int[] values = new int[dim.Length];
			for (int i = 0; i < dim.Length; i++)
				if (!int.TryParse(dim[i], out values[i]))
					return null;
			o.sprocketFileID = values[0];
			o.width = values[1];
			o.height = values[2];
			o.padding = values[3];
			o.backgroundColor = Color.FromArgb(values[4]);
			o.borderColor = Color.FromArgb(values[5]);
			o.borderSize = values[6];
			o.tileSpacing = values[7];
			o.displayType = (Display)values[8];
			o.anchor = (CropAnchor)values[9];
			o.preventEnlargement = values[10] == 1;
			o.SetFilename();
			return o;
		}

		#region Class Properties

		public bool PreventEnlargement
		{
			get { return preventEnlargement; }
			set { preventEnlargement = value; SetFilename(); }
		}

		public int TileSpacing
		{
			get { return tileSpacing; }
			set { tileSpacing = value; SetFilename(); }
		}

		public Display DisplayType
		{
			get { return displayType; }
			set { displayType = value; SetFilename(); }
		}

		public CropAnchor Anchor
		{
			get { return anchor; }
			set { anchor = value; SetFilename(); }
		}

		public long JpegQuality
		{
			get { return jpegQuality; }
			set { jpegQuality = value; SetFilename(); }
		}

		public long SprocketFileID
		{
			get { return sprocketFileID; }
		}

		public Image Image
		{
			get { return image; }
			set { image = value; }
		}

		public string PhysicalPath
		{
			get { return physicalPath; }
		}

		public string Filename
		{
			get { return filename; }
		}

		public int Width
		{
			get { return width; }
			set { width = value; SetFilename(); }
		}

		public int Height
		{
			get { return height; }
			set { height = value; SetFilename(); }
		}

		public int Padding
		{
			get { return padding; }
			set { padding = value; SetFilename(); }
		}

		public Color BackgroundColor
		{
			get { return backgroundColor; }
			set { backgroundColor = value; SetFilename(); }
		}

		public Color BorderColor
		{
			get { return borderColor; }
			set { borderColor = value; SetFilename(); }
		}

		public int BorderSize
		{
			get { return borderSize; }
			set { borderSize = value; SetFilename(); }
		}

		#endregion
	}
}
