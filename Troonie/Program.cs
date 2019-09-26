using System;
using Gtk;
using System.IO;
using Troonie_Lib;
using System.Reflection;

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
//				Troonie_Lib.Version.SetNewVersionNumberInAllFiles("1.4");
//				return;
				#endregion Set new version number in code

				GetProgramIcon ();
//				GetCjpegExecutable();

				// testing HSL conversion
//				double h,s,l;
//				ColorRgbHsl.I.RGB2HSL(255, 255, 255, out h, out s,out l);
//				byte r,g,b;
//				ColorRgbHsl.I.HSL2RGB(h,s,l, out r, out g, out b);
//				Console.WriteLine("rgb= " +r + " " + g + " " + b);
			}
			catch (Exception) {
				Console.WriteLine ("Error.");
				Console.WriteLine ("Troonie cannot work correctly and was closing.");
				Console.WriteLine ("Troonie (as well as its directory) requires read and write permission.");
				return;
			}

			Application.Init ();
			// Gtk.Settings.Default.SetLongProperty ("gtk-button-images", 1, "");		

			string filename = null;
            // START VALUE
            //	args = new string[] { "-v"};
            //	args = new string[] { "-e", "../image.jpg"};
              //args = new string[] { "-s", "../image.jpg" };
            //  args = new string[] { "-d", "../testdirectory" };

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
//							"../image01.jpg", 
//							"../image02.png",							
//							"../image03.jpg", 
							/* "../image04.jpg" */ });
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
                        Array.Sort(args);
					};

					StarterWidget start_new = new StarterWidget (args, false);
					start_new.Show ();
					break;
				case "-c":
					string[] argsWithoutFirst = new string[args.Length - 1];
					for (int i = 0; i < argsWithoutFirst.Length; i++) {
						argsWithoutFirst[i] = args[i + 1];
					}
					ConvertWidget winConvert = new ConvertWidget (argsWithoutFirst);
					winConvert.Show ();
					break;
				default:
					StarterWidget start = new StarterWidget (args, true);
					start.Show ();	
					break;
				}
			}
				

//			try{
			Application.Run ();

            TidyUp();
//			}
//			catch (Exception) {
//				Console.WriteLine ("Error 2.");
//				Console.WriteLine ("Troonie cannot work correctly and was closing.");
//				Console.WriteLine ("Troonie (as well as its directory) requires read and write permission.");
//				return;
//			}
		}

        private static void TidyUp()
        {
            try
            {
                Directory.Delete(Constants.I.TEMPPATH, true);
            }
            catch (Exception)
            {
                Console.WriteLine("Could not delete directory '" + Constants.I.TEMPPATH + "'.");
            }
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
