using System;
using Gtk;
using System.IO;
using Picturez_Lib;
using System.Reflection;

namespace Picturez
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
//			 Console.WriteLine("a.txt: " + System.IO.File.Exists ("/home/jessica/Schreibtisch/a/a.txt"));
//			Console.WriteLine("A.TXT: " + FileHelper.I.Exists ("/home/jessica/Schreibtisch/a/A.TXT"));

//			#region TESTUS
//			System.Drawing.Bitmap b = new System.Drawing.Bitmap("/home/jose/Schreibtisch/Testbilder/a/_1bit.png");
//			ImageConverter.To32BppWithTransparencyColor(b, System.Drawing.Color.FromArgb(255, 255, 255));
//			#endregion TESTUS

			// bool editMode = false;


			// Directory.CreateDirectory (Constants.I.EXEPATH);

			Constants.I.Init ();
			GetProgramIcon ();

			Application.Init ();
			// Gtk.Settings.Default.SetLongProperty ("gtk-button-images", 1, "");

			// START VALUE
			bool edit = false;
			bool steg = false;

			if (args.Length != 0)
			{
				if (args [0] == "-e")
					edit = true;
				else if (args [0] == "-s")
					steg = true;
				else if (args [0] == "-d") {
					DirectoryInfo di = new DirectoryInfo (args [args.Length - 1]);
					if (di.Exists) {
						FileInfo[] fi = di.GetFiles ();
						int fiLength = fi.Length;
						args = new string[fiLength];
						for (int i = 0; i < fiLength; i++) {
							args[i] = fi [i].FullName;
						}
					};
				}
			}

			string filename = null;
			if (args.Length > 1)
				filename = args [args.Length - 1];

			if (edit) {
				EditWidget win = new EditWidget (filename);
				win.Show ();
			} else if (steg){
				SteganographyWidget win = new SteganographyWidget ("test.jpg");
				win.Show ();
			} else {
				ConvertWidget convWidget = new ConvertWidget (args);
				convWidget.Show ();
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
