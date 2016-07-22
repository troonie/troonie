using System.Windows.Forms;
using System.Diagnostics;
using System.Security.Principal;
using System;

namespace WinInstaller
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new InstallForm());					
		}					
	}
}