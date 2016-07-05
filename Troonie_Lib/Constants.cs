﻿using System;
using System.IO;
using System.Net;
using System.Globalization;
using System.Threading;

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

		public OnUpdateAvailableDelegate OnUpdateAvailable;

		public const int TIME_DOUBLECLICK = 500; 
		public const int TIMEOUT_INTERVAL = 20;
		public const int TIMEOUT_INTERVAL_FIRST = 500;
		public const uint TIMEOUT_FILTER_PROCESS_PREVIEW = 300;
		public const uint TIMEOUT_STITCH_PROCESS_PREVIEW = 1000;
		public const string AUTHOR = "Troonie Project";
		public const string EXENAME = "Troonie.exe";
		public const string ICONNAME = "icon.ico";
		public const string WEBSITE = "http://www.troonie.com";
		public const string UPDATESERVERFILE = 
			"http://www.troonie.com/TroonieVersion";
		public const string TITLE = "Troonie";
		public const string TITLE_LIB = "Troonie_Lib";
		public const string DESCRIPTION_FIX_IN_ENGLISH = 
			"A slender tool to convert, trim, stitch, filter photos and work with steganography.";
		public const string VERSION = "1.0.0";
		public static DateTime PUBLISHDATE =  DateTime.Today; // new DateTime (2016, 03, 18);
		public static string PUBLISHDATE_STRING {
			get {
//				return Troonie_PUBLISHDATE.ToString ("d", new CultureInfo("en-US"));
				 return PUBLISHDATE.ToString ("d", CultureInfo.CurrentUICulture);
			}
		}
		/// <summary>Shortcut for 'Environment.NewLine'.</summary>
		public static string N = Environment.NewLine;

		private float versionFloat;
		public float VERSION_FLOAT { get {	return versionFloat; } }

		private string description;
		public string DESCRIPTION { get {	return description; } }

		private bool windows;
		public bool WINDOWS { get { return windows; }}
				
		private string exepath;
		public string EXEPATH { get { return exepath; }}

		private string homepath;
		public string HOMEPATH { get { return homepath; }}

		public void Init()
		{
			windows = IsWindows ();
			exepath = AppDomain.CurrentDomain.BaseDirectory + Path.DirectorySeparatorChar;
			homepath = windows ? Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%")
				: Environment.GetEnvironmentVariable("HOME");
			homepath += Path.DirectorySeparatorChar;
			description = Language.I.L[54];
			versionFloat = GetFloatVersionNumber (VERSION);
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

		private void CheckUpdateAsThread()
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
				r.Close();
				float serverVersionFloat = GetFloatVersionNumber(serverVersion);
				bool updateAvailable = versionFloat < serverVersionFloat;
				//fire the event now
				if (updateAvailable && this.OnUpdateAvailable != null) //is there a EventHandler?
				{
					this.OnUpdateAvailable.Invoke(serverVersionFloat); //calls its EventHandler                
				} //if not, ignore

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
