using System;
using System.IO;
using Microsoft.Win32;

namespace WinContextMenu
{   
    public class Program
    {        
        static void Main(string[] args)
        {
            string programExe =
				AppDomain.CurrentDomain.BaseDirectory + Path.DirectorySeparatorChar + "Troonie.exe";

            if (args.Length != 0 && args[0] == "Remove")
            {
                RemoveTroonieEntries();
                return;
            }

            AddTroonieEntries(programExe);         
        }
			
		private static void AddTroonieEntries(string programExe)
		{
			RegistryKey regmenu;
			try
			{
				// CONVERT WITH Troonie
				regmenu = Registry.ClassesRoot.CreateSubKey(
					"*\\shell\\Troonie\\Command");
				if (regmenu != null)
				{
					regmenu.SetValue("", "\"" + programExe + "\" \"%1\"");
					regmenu.Close();
				}

//				// EDIT WITH Troonie
//				regmenu = Registry.ClassesRoot.CreateSubKey(
//					"*\\shell\\Edit with Troonie\\Command");
//				if (regmenu != null)
//				{
//					regmenu.SetValue("", "\"" + programExe + "\" \"-e\" \"%1\"");
//					regmenu.Close();
//				}

				// CONVERT FOLDER WITH Troonie
				regmenu = Registry.ClassesRoot.CreateSubKey(
					"Folder" + "\\shell\\Troonie\\Command");
				if (regmenu != null)
				{
					regmenu.SetValue("", "\"" + programExe + "\" \"-d\" \"%1\"");
					regmenu.Close();
				}
			}
			catch (Exception ex)
			{
				// MessageBox.Show(ex.ToString());
				Console.WriteLine(ex.ToString());
			}
		}

		private static void RemoveTroonieEntries()
		{
			try
			{
				Registry.ClassesRoot.DeleteSubKey(
					"*\\shell\\Troonie\\Command", false);
				Registry.ClassesRoot.DeleteSubKey(
					"*\\shell\\Troonie", false);
//				Registry.ClassesRoot.DeleteSubKey(
//					"*\\shell\\Edit with Troonie\\Command", false);
//				Registry.ClassesRoot.DeleteSubKey(
//					"*\\shell\\Edit with Troonie", false);
				Registry.ClassesRoot.DeleteSubKey(
					"Folder\\shell\\Troonie\\Command", false);
				Registry.ClassesRoot.DeleteSubKey(
					"Folder\\shell\\Troonie", false);
			}
			catch (Exception ex)
			{
				// MessageBox.Show(ex.ToString());
				Console.WriteLine(ex.ToString());
			}
		}
    }
}
