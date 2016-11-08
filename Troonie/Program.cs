using System;
using Gtk;
using System.IO;
using Troonie_Lib;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Troonie
{
	public class MainClass
	{		
		private enum TargetType {
			String,
			RootWindow
		};

		public static TargetEntry [] Target_table = new TargetEntry [] {
			new TargetEntry ("STRING", 0, (uint) TargetType.String ),
			new TargetEntry ("text/plain", 0, (uint) TargetType.String),
			new TargetEntry ("application/x-rootwindow-drop", 0, (uint) TargetType.RootWindow)
		};

		public static void Main (string[] args)
		{
			Constants.I.Init ();
			#region Set new version number in code
//			Troonie_Lib.Version.SetNewVersionNumberInAllFiles("1.0.4");
//			return;
			#endregion Set new version number in code

			GetProgramIcon ();

			Application.Init ();
			// Gtk.Settings.Default.SetLongProperty ("gtk-button-images", 1, "");		

			string filename = null;
			// START VALUE
//			args = new string[] { "-v"};

			if (args.Length == 0) {
				StarterWidget start = new StarterWidget (args);
//				start.Visible = true;
				start.Show ();			
			} else {
				if (args.Length > 1)
					filename = args [args.Length - 1];

				switch (args [0]) {
				case "-e":
					EditWidget winEdit = new EditWidget (filename);
					winEdit.Show ();
					break;
				case "-s":
					SteganographyWidget winSteg = new SteganographyWidget (filename);
					winSteg.Show ();
					break;
//				case "-i":
//					StitchWidget winStitch = new StitchWidget ("pic.png", "pic.png");
//					winStitch.Show ();
//					break;
				case "-v":
					ViewerWidget winViewer = new ViewerWidget (
						new System.Collections.Generic.List<string>() {
							"/home/james/Schreibtisch/Tesimages/Brandenburger_Tor_Ban2.png", "/home/james/Schreibtisch/Tesimages/Pilz_dat2.png",
							"/home/james/Schreibtisch/Tesimages/Brandenburger_Tor_Ban2.png", "/home/james/Schreibtisch/Tesimages/Pilz_dat2.png",
							"/home/james/Schreibtisch/Tesimages/04.jpg", "/home/james/Schreibtisch/Tesimages/01.jpg",
							"/home/james/Schreibtisch/Tesimages/green.bmp", "/home/james/Schreibtisch/Tesimages/portrait.jpg",
							"/home/james/Schreibtisch/Tesimages/green_portrait.png", "/home/james/Schreibtisch/Tesimages/green.png"});
					winViewer.Show ();
					break;
				case "-d":
					DirectoryInfo di = new DirectoryInfo (args [args.Length - 1]);
					if (di.Exists) {
						FileInfo[] fi = di.GetFiles ();
						int fiLength = fi.Length;
						args = new string[fiLength];
						for (int i = 0; i < fiLength; i++) {
							args[i] = fi [i].FullName;
						}
					};
					ConvertWidget winConvert = new ConvertWidget (args);
					winConvert.Show ();
					break;
				case "-c":
					string[] argsWithoutFirst = new string[args.Length - 1];
					for (int i = 0; i < argsWithoutFirst.Length; i++) {
						argsWithoutFirst[i] = args[i + 1];
					}
					ConvertWidget winConvert2 = new ConvertWidget (argsWithoutFirst);
					winConvert2.Show ();
					break;
				default:
					StarterWidget start = new StarterWidget (args);
					//				start.Visible = true;
					start.Show ();	
					break;
				}
			}

			Application.Run ();
		}	

		private static void GetProgramIcon()
		{
			if (File.Exists (Constants.I.EXEPATH + Constants.ICONNAME))
				return;
			Assembly thisExe = Assembly.GetExecutingAssembly();
//			string [] resources = thisExe.GetManifestResourceNames();

			using (Stream str = thisExe.GetManifestResourceStream(Constants.ICONNAME), 
			       destStream = new FileStream(Constants.I.EXEPATH + Constants.ICONNAME, FileMode.Create, FileAccess.Write))
			{
				str.CopyTo (destStream);
			}
		}
	}
}
