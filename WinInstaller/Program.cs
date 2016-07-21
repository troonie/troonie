using System.Windows.Forms;

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