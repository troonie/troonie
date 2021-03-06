using System;
using System.IO;
using System.Drawing.Imaging;
using System.Xml.Serialization;

namespace Troonie_Lib
{
    /// <summary> Data container for configure Troonie. </summary>
	public class Config
    {
		private static string xmlFile = Constants.I.EXEPATH + "config.xml"; 

		#region global properties

		public int LanguageID{ get; set; }

		#endregion global properties

        #region converter properties 

		public bool AskForDesktopContextMenu { get; set; }
		public bool ConfirmDeleteImages { get; set; }

        /// <summary>The length of the bigger side of the image(s) to convert.
        /// </summary>
        public int BiggestLength; // { get; set; }

        /// <summary>Determines, if the original image has to be overwritten 
        /// or deleted after converting.
        /// </summary>
        public bool FileOverwriting { get; set; }

        /// <summary>The format to convert in.</summary>
		public TroonieImageFormat Format { get; set; }

        /// <summary>The new height of the image(s) to convert.</summary>
        public int Height;

        /// <summary>The quality of the jpeg codec.</summary>
        public byte JpgQuality;

		/// <summary>File path for xml serialization for stored keywords.</summary>
		public string KeywordsXmlFilePath { get; set; }

		/// <summary>The maximum image length for image filtering.</summary>
		//		[XmlAttribute("MaxImageLengthForFiltering", DataType = "int")]
		public int MaxImageLengthForFiltering { get; set; }

		public int ViewerImagePanelSize { get; set; }

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

		public bool ReplacingTransparencyWithColor;

		public ConvertMode StretchImage { get; set; }

		public byte TransparencyColorRed { get; set; }
		public byte TransparencyColorGreen { get; set; }
		public byte TransparencyColorBlue { get; set; }

		public byte ReplaceTransparencyColorRed { get; set; }
		public byte ReplaceTransparencyColorGreen { get; set; }
		public byte ReplaceTransparencyColorBlue { get; set; }

        /// <summary>
        /// Determines, if the path of initial image(s) will be used as 
        /// <see cref="Path"/> for converted image(s).
        /// </summary>
        public bool UseOriginalPath { get; set; }

		/// <summary>File path for external videoplayer.</summary>
		public string VideoplayerPath { get; set; }
		public bool VideoplayerWorks { get; set; }

        /// <summary>The new width of the image(s) to convert.</summary>
        public int Width;

		#endregion converter properties

		#region editor properties 

		/// <summary>The bottom value. </summary>
		public int eBottom { get; set; }
		/// <summary>The left value. </summary>
		public int eLeft { get; set; }
		//        /// <summary>The name of the configuration.</summary>
		//        [XmlAttribute("Name", DataType = "string")]
		//        public string Name { get; set; }
		/// <summary>The right value. </summary>
		public int eRight { get; set; }
		/// <summary>The rotation value. </summary>
		public int eRotation { get; set; }
		/// <summary>The top value. </summary>
		public int eTop { get; set; }

		#endregion editor properties 


		public Config() {

			LanguageID = SetLanguageID ();

			AskForDesktopContextMenu = true;
			ConfirmDeleteImages = true;
			BiggestLength = 1280;
			FileOverwriting = false;
			Format = Troonie_Lib.TroonieImageFormat.JPEG24;
			Height = 800;
			JpgQuality = 100;
			MaxImageLengthForFiltering = 1024;
			ViewerImagePanelSize = 300;
			Name = "Name";
			Path = string.Format("{0}"+ System.IO.Path.DirectorySeparatorChar, 
			                     Environment.GetFolderPath(Environment.SpecialFolder.MyPictures));			
			HighQuality = true;
			ResizeVersion = ResizeVersion.No;
			StretchImage = ConvertMode.StretchForge;
			UseOriginalPath = false;
			Width = 1280;

			TransparencyColorRed = 255;
			TransparencyColorGreen = 255;
			TransparencyColorBlue = 255;

			/* Replacing tranparency will not be saved, 
			 *  just one-time usage in SaveAsDialog form */ 
			ReplacingTransparencyWithColor = false;
			ReplaceTransparencyColorRed = 255;
			ReplaceTransparencyColorGreen = 255;
			ReplaceTransparencyColorBlue = 255;

			VideoplayerPath = "   Click here to set a videoplayer.";
			KeywordsXmlFilePath = Constants.I.EXEPATH + "keywords.xml";

			switch (Environment.OSVersion.Platform)
			{
			case PlatformID.Win32NT:
				if (VideoplayerWorks = File.Exists (@"%ProgramFiles(x86)%\Windows Media Player\wmplayer.exe")) {
					VideoplayerPath = @"%ProgramFiles(x86)%\Windows Media Player\wmplayer.exe";
					break;
				} else if (VideoplayerWorks = File.Exists (@"C:\Program Files (x86)\Windows Media Player\wmplayer.exe")) {
					VideoplayerPath = @"C:\Program Files (x86)\Windows Media Player\wmplayer.exe";
					break;
				}
				break;
			case PlatformID.Unix:
				if (VideoplayerWorks = File.Exists (@"/usr/bin/xplayer")) {
					VideoplayerPath = @"/usr/bin/xplayer";
					break;
				} else if (VideoplayerWorks = File.Exists (@"/usr/bin/vlc")) {
					VideoplayerPath = @"/usr/bin/vlc";
					break;
				}
				break;
//			case PlatformID.MacOSX:
//			default:
//				VideoplayerPath = "   Click here to set a videoplayer.";
//				break;
			}

		}

		private static int SetLanguageID()
		{
			switch (System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName) {
			case "de":
				return 1;
			case "en":
			default:
				return 0;
			}
		}

		public static Config Load()
		{
			if (!File.Exists (xmlFile)) {
				Save(new Config());
			}				

			XmlSerializer serializer = new XmlSerializer(typeof(Config));
			StreamReader sr = new StreamReader(xmlFile);
			Config c = (Config)serializer.Deserialize(sr);
			sr.Close();

			return c;
		}

		public static void Save(Config c)
		{
			// Just to be cautious
			c.FileOverwriting = false;

			XmlSerializer serializer = new XmlSerializer(typeof(Config));
			FileStream fs = new FileStream(xmlFile, FileMode.Create); 
			serializer.Serialize(fs, c);
			fs.Close();
		}
	}
}
