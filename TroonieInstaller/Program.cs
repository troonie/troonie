using System;
using System.Diagnostics;

namespace GtkInstaller
{
	class MainClass
	{
		public static int Main(string[] args)
		{
			if (args.Length < 2) {
				return 1;
			}
			int errorcode = 0;
			string vcppMsiInstaller = args[0];
			string gtkInstaller = args[1];

			// install Visual-C++-Redist-2013_x86
			Console.WriteLine("Installing Visual-C++-Redist-2013.");
			ProcessStartInfo info1 = new ProcessStartInfo();
			info1.FileName = vcppMsiInstaller;
			info1.Arguments = "/Quiet /Passive /qn";
			Process p1 = Process.Start(info1);
			// wait max 2 minutes
			if (p1 == null || !p1.WaitForExit(2 * 60 * 1000))
			{
				errorcode = 2;
			}

			// install gtk-sharp-2
			Console.WriteLine("Installing GTK-Sharp-2.");
			ProcessStartInfo info2 = new ProcessStartInfo();
			info2.FileName = gtkInstaller;
			info2.Arguments = "/Quiet /Passive /qn";
			Process p2 = Process.Start(info2);
			// wait max 2 minutes
			if (p2 == null || !p2.WaitForExit(2 * 60 * 1000))
			{
				errorcode = 3;
			}

			return errorcode;
		}
	}
}
