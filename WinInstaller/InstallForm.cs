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
			t[2] = l ? "INFO: GTK-Sharp wurde nicht korrekt installiert." : 
						"INFO: GTK-Sharp was not installed correctly.";
			t[3] = l ? "Troonie benötigt GTK-Sharp. Bitte installiere es noch einmal." : 
						"Troonie needs GTK-Sharp. Please re-install it.";
			t[4] = l ? "GTK-Sharp wurde erfolgreich installiert." : "GTK-Sharp was installed successfully.";
			t[5] = l ? "Troonie wurde auf dem Desktop entpackt." : "Troonie was extracted at your Desktop.";
			t[6] = null; 
			t[7] = l ? "Zuvor muss Windows neu gestartet werden, bevor ": 
					   "Windows needs to be restarted before Troonie ";
			t[8] = l ? "Troonie genutzt werden kann. Jetzt neu starten?" : "can be used. Restart now?";
			t[9] = l ? "Der Installer entpackt Troonie auf den Desktop " : "This installer extracts Troonie at your Desktop ";
			t[10] = l ? "und installiert GTK-Sharp 2." : "and installs GTK-Sharp 2.";
			t[11] = l ? "Ja, neu starten" : "Yes, restart";
			t[12] = l ? "Nein, nur Installer beenden" : "No, just close installer";

			btnOk.Text = t[1];
			btnCancel.Text = t[0];
			lbInfo.Text = t[9]  + Environment.NewLine + t[10] + Environment.NewLine;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
			if (count == 0)
				InstallGtksharpAndExtractTroonie ();
			else if (count == 1)
				Process.Start ("Shutdown", "-r -t 5");
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
			this.Refresh ();

			string path = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
			path += Path.DirectorySeparatorChar + "Troonie" + Path.DirectorySeparatorChar;
			Directory.CreateDirectory (path);
			string gtkInstaller = path + "gtk-sharp-2.12.38.msi";

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
			// startInfo.Verb = "runas";
			startInfo.Arguments = "/Quiet /Passive";
			Process process = Process.Start(startInfo);

			// wait max 5 minutes
			if (process == null || !process.WaitForExit (5 * 60 * 1000)) {
				lbInfo.Text = t [2] + Environment.NewLine + t [3] + Environment.NewLine; 
			}
			else {
				lbInfo.Text = t[4] + Environment.NewLine;
				count++;
			}
				
			Thread.Sleep (50);
			File.Delete (gtkInstaller);		

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
