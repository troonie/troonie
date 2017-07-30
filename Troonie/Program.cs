using System;
using Gtk;
using System.IO;
using Troonie_Lib;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Diagnostics;

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
			try {
				Constants.I.Init ();
				#region Set new version number in code
//				Troonie_Lib.Version.SetNewVersionNumberInAllFiles("1.1.0");
//				return;
				#endregion Set new version number in code

				GetProgramIcon ();
//				GetCjpegExecutable();
			}
			catch (Exception ex) {
//				Console.WriteLine (ex.Message);
				Console.WriteLine ("Error.");
				Console.WriteLine ("Troonie cannot work correctly and was closing.");
				Console.WriteLine ("Troonie (as well as its directory) requires read and write permission.");
				return;
//				OkCancelDialog pseudo = new OkCancelDialog (true);
//				pseudo.Title = Language.I.L [153];
//				pseudo.Label1 = Language.I.L [194];
//				pseudo.Label2 = Language.I.L [195];
//				pseudo.OkButtontext = Language.I.L [16];
//				pseudo.Show ();
			}

			Application.Init ();
			// Gtk.Settings.Default.SetLongProperty ("gtk-button-images", 1, "");		

			string filename = null;
			// START VALUE
//			args = new string[] { "-v"};
//			args = new string[] { "-e", "/home/jazz/Schreibtisch/Tesimages/portrait.jpg"};

			if (args.Length == 0) {
				StarterWidget start = new StarterWidget (args, true);
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
						new string[] {
//							"/home/jazz/Schreibtisch/Tesimages/Brandenburger_Tor_Ban03.png", 
//							"/home/jazz/Schreibtisch/Tesimages/Pilz_dat2.png",//							
//							"/home/jazz/Schreibtisch/Tesimages/portrait.jpg", 
//							"/home/jazz/Schreibtisch/Tesimages/01.jpg",
//							"/home/jazz/Schreibtisch/Tesimages/02.jpg",
//							"/home/jazz/Schreibtisch/Tesimages/testviteo-1sec.mp4",
							"/home/jazz/Schreibtisch/Tesimages/Brandenburger_Tor.jpg"});
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
//					ConvertWidget winConvert = new ConvertWidget (args);
//					winConvert.Show ();

					StarterWidget start_new = new StarterWidget (args, false);
					start_new.Show ();
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
					StarterWidget start = new StarterWidget (args, true);
					start.Show ();	
					break;
				}
			}
				

//			try{
			Application.Run ();
//			}
//			catch (Exception) {
//				Console.WriteLine ("Error 2.");
//				Console.WriteLine ("Troonie cannot work correctly and was closing.");
//				Console.WriteLine ("Troonie (as well as its directory) requires read and write permission.");
//				return;
//				//				OkCancelDialog pseudo = new OkCancelDialog (true);
//				//				pseudo.Title = Language.I.L [153];
//				//				pseudo.Label1 = Language.I.L [194];
//				//				pseudo.Label2 = Language.I.L [195];
//				//				pseudo.OkButtontext = Language.I.L [16];
//				//				pseudo.Show ();
//			}
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

//		private static void GetCjpegExecutable()
//		{
//			string name = Constants.I.WINDOWS ? Constants.CJPEGNAME + @".exe" : Constants.CJPEGNAME;
//
//			if (File.Exists (Constants.I.EXEPATH + name)) {
////				if (!Constants.I.WINDOWS) {
////					Constants.I.CJPEG = SetChmodX (Constants.I.EXEPATH + name);
////				} else {
////					Constants.I.CJPEG = true;
////				}
//				Constants.I.CJPEG = Constants.I.WINDOWS ? true : SetChmodX (Constants.I.EXEPATH + name);
//				return;
//			}
//
//			Assembly thisExe = Assembly.GetExecutingAssembly();
//			//			string [] resources = thisExe.GetManifestResourceNames();
//
//			using (Stream str = thisExe.GetManifestResourceStream(name), 
//				destStream = new FileStream(Constants.I.EXEPATH + name, FileMode.Create, FileAccess.Write))
//			{
//				str.CopyTo (destStream);
//			}
//
//			Constants.I.CJPEG = Constants.I.WINDOWS ? true : SetChmodX (Constants.I.EXEPATH + name);
//		}
//
//		private static bool SetChmodX(string file)
//		{
//			bool success;
//			using (Process proc = new Process ()) {
//				try {
//					proc.StartInfo.FileName = "chmod";  
//					proc.StartInfo.Arguments = "ugo+x " + file; 
//					proc.StartInfo.UseShellExecute = false; 
//					proc.StartInfo.RedirectStandardOutput = true;
//					proc.StartInfo.RedirectStandardError = true;
//					proc.Start ();
//					proc.WaitForExit ();
//					proc.Close ();
//	//				proc.Dispose ();
//					success = true;
//				}
//				catch(Exception){
//					success = false;
//				}
//			}
//
//			return success;
//		}
	}
}
