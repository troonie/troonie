using System;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace WinInstaller
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			string path = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
			path += Path.DirectorySeparatorChar + "Troonie" + Path.DirectorySeparatorChar;
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
				                   "Troonie needs GTK-Sharp. Please re-install it.\n"); 
			else {
				Console.WriteLine ("GTK-Sharp was installed successfully.\n");
			}

			// Console.WriteLine ("Troonie was extracted to Desktop (" + path + ").");
			// Console.WriteLine ("You can start it with Troonie.exe.\n");
			Thread.Sleep (50);
			File.Delete (gtkInstaller);		

			string text = "Troonie was extracted to Desktop (" + path + "). You can start it with Troonie.exe. \n" + 
					      "Windows needs to be restarted before Troonie can be used. Restart now? \n\n" + 
						  "Troonie wurde auf dem Desktop (" + path + ") entpackt. Es kann mit Troonie.exe gestartet werden. \n" + 
						  "Zuvor muss jedoch Windows neu gestartet werden, bevor Troonie genutzt werden kann. Jetzt neu starten? ";
			if (MessageBox.Show (text, "Restart Windows",
				    MessageBoxButtons.YesNo) == DialogResult.Yes) {
				Process.Start ("Shutdown", "-r -t 5");
				return;
			} else {
			}

			// Console.WriteLine ("NOTE: Your system will reboot now. Please press any key to reboot.\n");
			// Console.ReadKey();


		}
	}
}