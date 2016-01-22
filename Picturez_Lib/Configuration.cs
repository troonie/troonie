using System;
using System.Drawing.Imaging;
using System.Xml.Serialization;

namespace Picturez_Lib
{
    /// <summary> Data container for configure Picturez. </summary>
    public class Configuration : ICloneable
    {
        #region public properties 

		public bool AskForDesktopContextMenu { get; set; }

        /// <summary>The length of the bigger side of the image(s) to convert.
        /// </summary>
        public int BiggestLength; // { get; set; }

        /// <summary>Determines, if the original image has to be overwritten 
        /// or deleted after converting.
        /// </summary>
        public bool FileOverwriting { get; set; }

        /// <summary>The format to convert in.</summary>
		public PicturezImageFormat Format { get; set; }

        /// <summary>The new height of the image(s) to convert.</summary>
        public int Height;

        /// <summary>The quality of the jpeg codec.</summary>
        public byte JpgQuality;

        /// <summary>The name of the configuration.</summary>
        [XmlAttribute("Name", DataType = "string")]
        public string Name { get; set; }

        /// <summary>File path for saving converted image(s).</summary>
        public string Path { get; set; }	

        /// <summary> Image resize version used for converting. </summary>
        public ResizeVersion ResizeVersion { get; set; }

        /// <summary> Determines whether higher quality and slower rendering, 
        /// or lower quality and faster rendering will be used. </summary>
        public bool HighQuality { get; set; }

		public ConvertMode StretchImage { get; set; }

		public byte TransparencyColorRed { get; set; }
		public byte TransparencyColorGreen { get; set; }
		public byte TransparencyColorBlue { get; set; }

        /// <summary>
        /// Determines, if the path of initial image(s) will be used as 
        /// <see cref="Path"/> for converted image(s).
        /// </summary>
        public bool UseOriginalPath { get; set; }

        /// <summary>The new width of the image(s) to convert.</summary>
        public int Width;

        #endregion public properties

		public Configuration() {
		}

		public void SetDefaultValues()
		{
			AskForDesktopContextMenu = true;
            BiggestLength = 1280;
            FileOverwriting = false;
			Format = Picturez_Lib.PicturezImageFormat.JPEG24;
            Height = 800;
            JpgQuality = 100;
            Name = "Name";
			Path = string.Format("{0}"+ System.IO.Path.DirectorySeparatorChar, 
				Environment.GetFolderPath(Environment.SpecialFolder.MyPictures));			
            HighQuality = true;
			ResizeVersion = ResizeVersion.No;
			StretchImage = Constants.I.EDITMODE ? ConvertMode.Editor : ConvertMode.StretchForge;
            UseOriginalPath = false;
            Width = 1280;				

			TransparencyColorRed = 255;
			TransparencyColorGreen = 255;
			TransparencyColorBlue = 255;
		}


        /// <summary> Makes a deep copy of this instance. </summary>
        /// <returns>A cloned <see cref="Configuration"/> of this instance.
        /// </returns>
        public object Clone()
        {
            Configuration c = new Configuration();
			c.AskForDesktopContextMenu = AskForDesktopContextMenu;
            c.BiggestLength = BiggestLength;
            c.FileOverwriting = FileOverwriting;
            c.Format = Format;
            c.Height = Height;
            c.JpgQuality = JpgQuality;
            c.Name = Name;
            c.Path = Path;
            c.HighQuality = HighQuality;
            c.ResizeVersion = ResizeVersion;
            c.StretchImage = StretchImage;
            c.UseOriginalPath = UseOriginalPath;
            c.Width = Width;

			c.TransparencyColorRed = TransparencyColorRed;
			c.TransparencyColorGreen = TransparencyColorGreen;
			c.TransparencyColorBlue = TransparencyColorBlue;
            return c;
        }
}
}
