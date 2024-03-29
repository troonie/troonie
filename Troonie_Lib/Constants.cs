﻿using System;
using System.IO;
using System.Net;
using System.Globalization;
using System.Threading;
using System.Collections.Generic;

namespace Troonie_Lib
{
	public delegate void OnUpdateAvailableDelegate(float newVersion);

	public class Constants
	{
		private static Constants instance;
		public static Constants I
		{
			get
			{
				if (instance == null) {
					instance = new Constants ();                   
                }
				return instance;
			}
		}

//		public OnUpdateAvailableDelegate OnUpdateAvailable;

		public const int MAX_NUMBER_OF_IMAGES = 15;
		public const int TIME_DOUBLECLICK = 500; 
		public const int TIMEOUT_INTERVAL = 20;
		public const int TIMEOUT_INTERVAL_FIRST = 500;
		public const uint TIMEOUT_FILTER_PROCESS_PREVIEW = 300;
		public const uint TIMEOUT_STITCH_PROCESS_PREVIEW = 1000;
//		public const string BLACKFILENAME = "black.png";
		public const string AUTHOR = "Troonie Project";
		public const string CJPEGNAME = "cjpeg";
        public const string EXIFTOOLNAME = "exiftool.exe";
        public const string DJPEGNAME = "djpeg";
		public const string EXENAME = "Troonie.exe";
		public const string ICONNAME = "icon.ico";
		public const string WEBSITE = "http://www.troonie.com";
		public const string UPDATESERVERFILE = 
			"https://raw.githubusercontent.com/troonie/troonie/master/Troonie_Lib/Version.cs";
//		public const string ERROR_DELETE_TEMP_FILES = "Could not delete Troonie temp files.";
		public const string TITLE = "Troonie";
		public const string TITLE_LIB = "Troonie_Lib";
		/// <summary>Constant non-changeable text. For changeable value, see Language.I.L[54].</summary>
		public const string DESCRIPTION_FIX_IN_ENGLISH = 
			"A portable tool to convert, trim, stitch, filter photos and work with steganography.";
		public static DateTime PUBLISHDATE =  DateTime.Today; // new DateTime (2016, 03, 18);
		public static string PUBLISHDATE_STRING {
			get {
//				return Troonie_PUBLISHDATE.ToString ("d", new CultureInfo("en-US"));
				 return PUBLISHDATE.ToString ("d", CultureInfo.CurrentUICulture);
			}
		}
		/// <summary>Shortcut for 'Environment.NewLine'.</summary>
		public static string N = Environment.NewLine;

        /// <summary>Shortcut for whitespace (' ').</summary>
        public const char WS = ' ';

        public static string[] Stars = new[]{ string.Empty, "_+", "_++", "_+++", "_++++", "_+++++" };

		// Usage examples
		//		var test = Troonie_Lib.ImageFormatConverter.I.Extensions.Where(x => x.Key == TroonieImageFormat.JPEG24).Select(x => x.Value);
		//		var test2 = Troonie_Lib.ImageFormatConverter.I.Extensions.First(x => x.Key == TroonieImageFormat.JPEG24);
		public static readonly Dictionary<TroonieImageFormat, Tuple<string, string>> Extensions = 
			new Dictionary<TroonieImageFormat, Tuple<string, string>>(15) 
		{
			{ TroonieImageFormat.BMP1, Tuple.Create(".bmp", ".bmp") },
			{ TroonieImageFormat.BMP8, Tuple.Create(".bmp", ".bmp") },
			{ TroonieImageFormat.BMP24, Tuple.Create(".bmp", ".bmp") },
			{ TroonieImageFormat.EMF, Tuple.Create(".emf", ".emf") },
			{ TroonieImageFormat.GIF, Tuple.Create(".gif", ".gif") },
			{ TroonieImageFormat.ICO, Tuple.Create(".ico", ".ico") },
			{ TroonieImageFormat.JPEG8, Tuple.Create(".jpg", ".jpeg") },
			{ TroonieImageFormat.JPEG24, Tuple.Create(".jpg", ".jpeg") },
			{ TroonieImageFormat.JPEGLOSSLESS, Tuple.Create(".jpg", ".jpeg") },
			{ TroonieImageFormat.PNG1, Tuple.Create(".png", ".png") },
			{ TroonieImageFormat.PNG24, Tuple.Create(".png", ".png") },
			{ TroonieImageFormat.PNG32AlphaAsValue, Tuple.Create(".png", ".png") },
			{ TroonieImageFormat.PNG32Transparency, Tuple.Create(".png", ".png") },
			{ TroonieImageFormat.PNG8, Tuple.Create(".png", ".png") },
			{ TroonieImageFormat.TIFF, Tuple.Create(".tif", ".tiff") },
			{ TroonieImageFormat.WMF, Tuple.Create(".wmf", ".wmf") }
		};

		public static readonly Dictionary<TroonieVideoFormat, Tuple<string, string, string>> VideoExtensions = 
			new Dictionary<TroonieVideoFormat, Tuple<string, string, string>>(3) 
		{
			{ TroonieVideoFormat.AVI, Tuple.Create(".avi", ".divx", ".xvid") },
			{ TroonieVideoFormat.MP4, Tuple.Create(".mp4", ".mp4", ".mp4") },
			{ TroonieVideoFormat.MPEG, Tuple.Create(".mpg", ".mpeg", ".m2v") }
		};

		public string[] VideoExtensionArray{
			get { 
					List<string> l = new List<string> ();
					foreach (var item in VideoExtensions.Values) {
						if (item.Item1 != string.Empty) {
							l.Add (item.Item1);
						}						
						if (item.Item2 != string.Empty) {
							l.Add (item.Item2);
						}						
						if (item.Item3 != string.Empty) {
							l.Add (item.Item3);
						}
					}

				string[] result = l.ToArray ();
				l.Clear ();
				l = null;
				return result;
			}
		}

		private float versionFloat;
		public float VERSION_FLOAT { get {	return versionFloat; } }

		private float serverVersionFloat;
		public float SERVER_VERSION_FLOAT { get {	return serverVersionFloat; } }

//		private string description;
//		public string DESCRIPTION { get {	return description; } }

		private bool windows;
		public bool WINDOWS { get { return windows; }}

		private bool cjpeg;
		public bool CJPEG { get { return cjpeg;} }

        private bool exiftool;
        public bool EXIFTOOL { get { return exiftool; } }

        private string exepath;
		public string EXEPATH { get { return exepath; }}

        public string TEMPPATH { get { return exepath + "temp" + Path.DirectorySeparatorChar; } }

        private string homepath;
		public string HOMEPATH { get { return homepath; }}

		private Config config;
		public Config CONFIG { get { return config; }}

		public ExifTool ET { get; private set; }

		public void Init()
		{
			windows = IsWindows ();
			cjpeg = JpegEncoder.ExistsCjpeg ();            
            exepath = AppDomain.CurrentDomain.BaseDirectory; // + Path.DirectorySeparatorChar;
            exiftool = File.Exists(exepath + Path.DirectorySeparatorChar + EXIFTOOLNAME);
            homepath = windows ? Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%")
				: Environment.GetEnvironmentVariable("HOME");
			homepath += Path.DirectorySeparatorChar;
			config = Config.Load ();
			Language.I.LanguageID = config.LanguageID;
//			description = Language.I.L[54];
			versionFloat = GetFloatVersionNumber (Version.VERSION);
            Directory.CreateDirectory(TEMPPATH);
            ET = new ExifTool();

            CheckUpdateAsThread ();
		}	

		private bool IsWindows()
		{
			switch (Environment.OSVersion.Platform)
			{
			case PlatformID.Unix:
				return false;
			case PlatformID.MacOSX:
				return false;
			default:
				return true;
			}
		}

		public void CheckUpdateAsThread()
		{
			Thread thread = new Thread(CheckUpdate);
			thread.IsBackground = true;
			thread.Start();
		}

		private void CheckUpdate()
		{			
			try
			{
				WebRequest request = WebRequest.Create(UPDATESERVERFILE);
				// this step is the problem for sometimes delaying
				WebResponse response = request.GetResponse();
				StreamReader r = new StreamReader(response.GetResponseStream());
				string serverVersion = r.ReadLine();
				int s = serverVersion.IndexOf('"');
				int l = serverVersion.LastIndexOf('"');
				serverVersion = serverVersion.Substring(s + 1, l - s - 1);
				r.Close();
				serverVersionFloat = GetFloatVersionNumber(serverVersion);
//				bool updateAvailable = versionFloat < serverVersionFloat;
//				//fire the event now
//				if (updateAvailable && this.OnUpdateAvailable != null) //is there a EventHandler?
//				{
//					this.OnUpdateAvailable.Invoke(serverVersionFloat); //calls its EventHandler                
//				} //if not, ignore

//				Console.WriteLine("serverVersionFloat: " + serverVersionFloat);
			}
			catch (/*System.Net.WebException*/Exception)
			{
				// when catching here, no internet is available or
				// server is not available.
//				Console.WriteLine ("Exception by update check.");
//				Console.WriteLine(e.Message);
//				Console.WriteLine(e.StackTrace);
			}
		}
			
		private static float GetFloatVersionNumber(string version)
		{
			int pos = version.IndexOf ('.');
			string subs = version.Substring(0, pos + 1) + version.Substring (pos).Replace (".", "");
			float f = float.Parse (subs, CultureInfo.CreateSpecificCulture("en-us"));		
			return f;			
		}
	}
}

