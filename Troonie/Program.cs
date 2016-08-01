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
			#region Compare images testing
//			DifferenceFilter cf = new DifferenceFilter ();
//			cf.Smallest = 0;
//			cf.Highest = 9;
//			cf.CompareBitmap = System.Drawing.Bitmap.FromFile ("b.png") as System.Drawing.Bitmap;
//
//			System.Drawing.Bitmap b = System.Drawing.Bitmap.FromFile ("a.png") as System.Drawing.Bitmap;
////			b = cf.Apply (b, null);
////			b.Save ("c.png", System.Drawing.Imaging.ImageFormat.Png);
//			cf.DrawThick3x3Pixels = true;
//			b = cf.Apply (b, null);
//			b.Save ("c2.png", System.Drawing.Imaging.ImageFormat.Png);

//			ThickPixelFilter tpf = new ThickPixelFilter();
//			b = tpf.Apply (b, null);
//			b.Save ("d.png", System.Drawing.Imaging.ImageFormat.Png);

			#endregion Compare images testing

//			#region StitchMIFilter
//			System.Drawing.Bitmap b1 = System.Drawing.Bitmap.FromFile ("test1.jpg") as System.Drawing.Bitmap;
//			System.Drawing.Bitmap b2 = System.Drawing.Bitmap.FromFile ("test2.jpg") as System.Drawing.Bitmap;
//			StitchMIFilter st = new StitchMIFilter(b1, b2);
//			st.Landscape = false;
//			st.Left01 = 10;
//			st.Left02 = 10;
//			st.Right01 = 10;
//			st.Right02 = 10;
//			st.Top01 = 10;
//			st.Top02 = 10;
//			st.Bottom01 = 10;
//			st.Bottom02 = 10;
//
//			st.Process();
//			st.ResultBitmap.Save ("StitchMIFilter.png", System.Drawing.Imaging.ImageFormat.Png);
//			#endregion

			// Does this work?
			// button.ModifyBg(StateType.Prelight, new Gdk.Color(220, 220, 220));
			Constants.I.Init ();
			GetProgramIcon ();

			Application.Init ();
			// Gtk.Settings.Default.SetLongProperty ("gtk-button-images", 1, "");		

			string filename = null;
			// START VALUE
//			args = new string[] { "-s"};

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
