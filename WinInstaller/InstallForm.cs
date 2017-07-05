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
		int errorcode;
		string path;

		public InstallForm()
		{
			InitializeComponent();
			Assembly thisExe = Assembly.GetExecutingAssembly();
			Stream str = thisExe.GetManifestResourceStream("icon.ico");
			this.Icon = new System.Drawing.Icon(str);
			t = new string[13];
			bool l = IsGermanLanguage();

			t[0] = l ? "Abbrechen" : "Cancel";
			t[1] = "Ok";
			t[2] = l ? "FEHLER: GTK-Sharp oder Visual-C++-Redist-2013 wurden nicht " :
					   "ERROR: GTK-Sharp or Visual-C++-Redist-2013 were not ";
			t[3] = l ? "korrekt installiert. Troonie benötigt diese Komponenten." :
						"installed correctly. Troonie needs these packages.";
			t[4] = l ? "GTK-Sharp und Visual-C++-Redist-2013 wurden erfolgreich installiert." :
					   "GTK-Sharp and Visual-C++-Redist-2013 were installed successfully.";
			t[5] = l ? "Troonie wurde auf dem Desktop entpackt." : "Troonie was extracted at your Desktop.";
			t[6] = l ? "Installiere... Bitte warten. " : "Installing... Please wait.";
			t[7] = l ? "Verzeichnis von Troonie öffnen? " : "Open directory of Troonie now? ";
			t[8] = l ? "Bitte installiere Troonie noch einmal." : "Please re-install Troonie.";
			t[9] = l ? "Der Installer entpackt Troonie auf den Desktop und " : "This installer extracts Troonie at your Desktop and ";
			t[10] = l ? "installiert GTK-Sharp-2 sowie Visual-C++-Redist-2013." : "installs GTK-Sharp-2 as well as Visual-C++-Redist-2013.";
			t[11] = l ? "Ja, Verzeichnis öffnen" : "Yes, open directory";
			t[12] = l ? "Nein, nur Installer beenden" : "No, just close installer";

			btnOk.Text = t[1];
			btnCancel.Text = t[0];
			lbInfo.Text = t[9] + Environment.NewLine + t[10] + Environment.NewLine;
		}

		private void btnOk_Click(object sender, EventArgs e)
		{
			if (errorcode == 0)
			{
				InstallGtksharpAndExtractTroonie();
			}
			else if (errorcode == 1)
			{
				//Process.Start ("Shutdown", "-r -t 0");

				// start troonie
				try
				{					
					Process p = Process.Start(path);

					Close();
					Application.Exit();
				}
				catch (Exception)
				{					
				}
			}
		}

		private void buttonCancel_Click(object sender, EventArgs e)
		{
			Close();
			Application.Exit();
		}

		private void InstallGtksharpAndExtractTroonie()
		{
			btnOk.Visible = false;
			btnCancel.Visible = false;
			progressBar1.Visible = true;
			lbInfo.Text = t[6];
			this.Refresh();

			path = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
			path += Path.DirectorySeparatorChar + "Troonie" + Path.DirectorySeparatorChar;
			Directory.CreateDirectory(path);
			string gtk_sharp_Installer = path + "gtk-sharp-2.12.msi";
			string vcppMsiInstaller = path + "vc_runtimeMinimum_x86.msi";
			string vcppCabInstaller = path + "cab1.cab";
			string gtkInstaller = path + "TroonieInstaller.exe";

			Assembly thisExe = Assembly.GetExecutingAssembly();
			string[] resources = thisExe.GetManifestResourceNames();

			foreach (var item in resources)
			{
				using (Stream str = thisExe.GetManifestResourceStream(item),
					destStream = new FileStream(path + item, FileMode.Create, FileAccess.Write))
				{
					str.CopyTo(destStream);
				}
			}

			Refresh();

			// install Visual-C++-Redist-2013_x86
			ProcessStartInfo info = new ProcessStartInfo();
			info.FileName = gtkInstaller;
			info.Arguments = "\"" + vcppMsiInstaller + "\" \"" + gtk_sharp_Installer + "\"";
			Process p = Process.Start(info);
			// wait max 3 minutes
			if (p != null && p.WaitForExit(3 * 60 * 1000) && p.ExitCode == 0 )
			{
				lbInfo.Text = t[4] + Environment.NewLine;
				errorcode = 1;
			}
			else {
				lbInfo.Text = t[2] + Environment.NewLine + t[3] + Environment.NewLine + Environment.NewLine + t[8];
				errorcode = 0;
			}
			//Console.WriteLine("ExitCode: " + p2.ExitCode);

			Refresh();

			Thread.Sleep(50);
			File.Delete(gtk_sharp_Installer);
			File.Delete(vcppMsiInstaller);
			File.Delete(vcppCabInstaller);
			File.Delete(gtkInstaller);

			btnCancel.Visible = true;
			progressBar1.Visible = false;

			if (errorcode == 0)
			{
				return;
			}
			else {
				btnOk.Visible = true;
				btnOk.Text = t[11];
				btnCancel.Text = t[12];
			}

			string tt = t[5] + // Environment.NewLine + " (" + path + ") " + 
			            Environment.NewLine + Environment.NewLine + t[7] + Environment.NewLine;
			lbInfo.Text = lbInfo.Text + tt;
		}

		private static bool IsGermanLanguage()
		{
			switch (System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName)
			{
				case "de":
					return true;
				case "en":
				default:
					return false;
			}
		}
	}
}
