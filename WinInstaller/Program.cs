using System;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Threading;

namespace WinInstaller
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			string path = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
			path += Path.DirectorySeparatorChar + "Picturez" + Path.DirectorySeparatorChar;
			Directory.CreateDirectory (path);
			string gtkInstaller = path + "gtk-sharp-2.12.30.msi";

			Assembly thisExe = Assembly.GetExecutingAssembly();
			string [] resources = thisExe.GetManifestResourceNames();

			foreach (var item in resources) {
				using (Stream str = thisExe.GetManifestResourceStream(item), 
				       destStream = new FileStream(path + item, FileMode.Create, FileAccess.Write))
				{
					str.CopyTo (destStream);
				}
			}

			ProcessStartInfo startInfo = new ProcessStartInfo();
			//			startInfo.UseShellExecute = true;
			//			startInfo.WorkingDirectory = Environment.CurrentDirectory;
			startInfo.FileName = gtkInstaller;
			// run as admin
			//			startInfo.Verb = "runas";
			//			if (remove)
			//				startInfo.Arguments = "Remove";

			Process process = Process.Start(startInfo);

			// wait max 5 minutes
			if (process == null || !process.WaitForExit (5 * 60 * 1000))
				Console.WriteLine ("INFO: GTK-Sharp was not installed correctly.\n" + 
				                   "Picturez needs GTK-Sharp. Please re-install it.\n"); 
			else {
				Console.WriteLine ("GTK-Sharp was installed successfully.\n");
			}

			Console.WriteLine ("Picturez was extracted to Desktop (" + path + ").");
			Console.WriteLine ("You can start it with Picturez.exe.\n");
			Thread.Sleep (50);
			File.Delete (gtkInstaller);		

			Console.WriteLine ("NOTE: Your system will reboot now. Please press any key to reboot.\n");
			Console.ReadKey();

			Process.Start ("Shutdown", "-r -t 5");
		}
	}
}