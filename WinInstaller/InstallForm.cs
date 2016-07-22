using System;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace WinInstaller
{
    public partial class InstallForm : Form
    {
		string[] t; 
		int count;

		public InstallForm()
        {
            InitializeComponent();
			Assembly thisExe = Assembly.GetExecutingAssembly();
			Stream str = thisExe.GetManifestResourceStream("icon.ico");
			this.Icon = new System.Drawing.Icon (str);
			t = new string[13];
			bool l = IsGermanLanguage ();

			t[0] = l ? "Abbrechen" : "Cancel";
			t[1] = l ? "Ok" : "Ok";
			t[2] = l ? "INFO: GTK-Sharp oder Visual-C++-Redist-2013 wurden nicht korrekt installiert." : 
					   "INFO: GTK-Sharp or Visual-C++-Redist-2013 were not installed correctly.";
			t[3] = l ? "Troonie benötigt diese Komponenten. Bitte installiere Troonie noch einmal." : 
						"Troonie needs these packages. Please re-install Troonie.";
			t[4] = l ? "GTK-Sharp und Visual-C++-Redist-2013 wurden erfolgreich installiert." : 
					   "GTK-Sharp and Visual-C++-Redist-2013 were installed successfully.";
			t[5] = l ? "Troonie wurde auf dem Desktop entpackt." : "Troonie was extracted at your Desktop.";
			t[6] = l ? "Installiere... Bitte warten. " : "Installing... Please wait."; 
			t[7] = l ? "Zuvor muss Windows neu gestartet werden, bevor ": 
					   "Windows needs to be restarted before Troonie ";
			t[8] = l ? "Troonie genutzt werden kann. Jetzt neu starten?" : "can be used. Restart now?";
			t[9] = l ? "Der Installer entpackt Troonie auf den Desktop und " : "This installer extracts Troonie at your Desktop and ";
			t[10] = l ? "installiert GTK-Sharp-2 sowie Visual-C++-Redist-2013." : "installs GTK-Sharp-2 as well as Visual-C++-Redist-2013.";
			t[11] = l ? "Ja, neu starten" : "Yes, restart";
			t[12] = l ? "Nein, nur Installer beenden" : "No, just close installer";

			btnOk.Text = t[1];
			btnCancel.Text = t[0];
			lbInfo.Text = t[9]  + Environment.NewLine + t[10] + Environment.NewLine;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
			if (count == 0) {
				InstallGtksharpAndExtractTroonie ();
			} else if (count == 1) {
				Process.Start ("Shutdown", "-r -t 0");
			}
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
			Close ();
			Application.Exit ();
        }

		private void InstallGtksharpAndExtractTroonie()
		{
			btnOk.Visible = false;
			btnCancel.Visible = false;
			progressBar1.Visible = true;
			lbInfo.Text = t [6];
			this.Refresh ();

			string path = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
			path += Path.DirectorySeparatorChar + "Troonie" + Path.DirectorySeparatorChar;
			Directory.CreateDirectory (path);
			string gtkInstaller = path + "gtk-sharp-2.12.38.msi";
			string vcppMsiInstaller = path + "vc_runtimeMinimum_x86.msi";
			string vcppCabInstaller = path + "cab1.cab";

			Assembly thisExe = Assembly.GetExecutingAssembly();
			string [] resources = thisExe.GetManifestResourceNames();

			foreach (var item in resources) {
				using (Stream str = thisExe.GetManifestResourceStream(item), 
					destStream = new FileStream(path + item, FileMode.Create, FileAccess.Write))
				{
					str.CopyTo (destStream);
				}
			}

			// install gtk-sharp-2
			ProcessStartInfo info1 = new ProcessStartInfo();
			info1.FileName = gtkInstaller;
			info1.Arguments = "/Quiet /Passive /qn";
			Process p1 = Process.Start(info1);
			// wait max 3 minutes
			if (p1 != null && p1.WaitForExit (3 * 60 * 1000)) {
				count = 1;
			}
				
			// install Visual-C++-Redist-2013_x86
			ProcessStartInfo info2 = new ProcessStartInfo();
			info2.FileName = vcppMsiInstaller;
			info2.Arguments = "/Quiet /Passive /qn";
			Process p2 = Process.Start(info2);
			// wait max 3 minutes
			if (count == 1 && p2 != null && p2.WaitForExit (3 * 60 * 1000)) {
				lbInfo.Text = t [4] + Environment.NewLine;
			} else {
				lbInfo.Text = t [2] + Environment.NewLine + t [3] + Environment.NewLine;
				count = 0;
			}				
				
			Thread.Sleep (50);
			File.Delete (gtkInstaller);
			File.Delete (vcppMsiInstaller);
			File.Delete (vcppCabInstaller);

			btnCancel.Visible = true;
			progressBar1.Visible = false;

			if (count == 0) {
				return;
			} else {
				btnOk.Visible = true;
				btnOk.Text = t[11];
				btnCancel.Text = t[12];
			}

			string tt = t [5] + Environment.NewLine + 
						" (" + path + ") " + Environment.NewLine + Environment.NewLine + 
						t [7] + Environment.NewLine +
						t [8] + Environment.NewLine;
			lbInfo.Text = lbInfo.Text + tt;
		}			

		private static bool IsGermanLanguage()
		{
			switch (System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName) {
			case "de":
				return true;
			case "en":
			default:
				return false;
			}
		}			
    }
}
