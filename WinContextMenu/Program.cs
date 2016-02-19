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
				AppDomain.CurrentDomain.BaseDirectory + Path.DirectorySeparatorChar + "Picturez.exe";

            if (args.Length != 0 && args[0] == "Remove")
            {
                RemovePicturezEntries();
                return;
            }

            AddPicturezEntries(programExe);         
        }
			
		private static void AddPicturezEntries(string programExe)
		{
			RegistryKey regmenu;
			try
			{
				// CONVERT WITH PICTUREZ
				regmenu = Registry.ClassesRoot.CreateSubKey(
					"*\\shell\\Picturez\\Command");
				if (regmenu != null)
				{
					regmenu.SetValue("", "\"" + programExe + "\" \"%1\"");
					regmenu.Close();
				}

//				// EDIT WITH PICTUREZ
//				regmenu = Registry.ClassesRoot.CreateSubKey(
//					"*\\shell\\Edit with Picturez\\Command");
//				if (regmenu != null)
//				{
//					regmenu.SetValue("", "\"" + programExe + "\" \"-e\" \"%1\"");
//					regmenu.Close();
//				}

				// CONVERT FOLDER WITH PICTUREZ
				regmenu = Registry.ClassesRoot.CreateSubKey(
					"Folder" + "\\shell\\Picturez\\Command");
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

		private static void RemovePicturezEntries()
		{
			try
			{
				Registry.ClassesRoot.DeleteSubKey(
					"*\\shell\\Picturez\\Command", false);
				Registry.ClassesRoot.DeleteSubKey(
					"*\\shell\\Picturez", false);
//				Registry.ClassesRoot.DeleteSubKey(
//					"*\\shell\\Edit with Picturez\\Command", false);
//				Registry.ClassesRoot.DeleteSubKey(
//					"*\\shell\\Edit with Picturez", false);
				Registry.ClassesRoot.DeleteSubKey(
					"Folder\\shell\\Picturez\\Command", false);
				Registry.ClassesRoot.DeleteSubKey(
					"Folder\\shell\\Picturez", false);
			}
			catch (Exception ex)
			{
				// MessageBox.Show(ex.ToString());
				Console.WriteLine(ex.ToString());
			}
		}
    }
}
