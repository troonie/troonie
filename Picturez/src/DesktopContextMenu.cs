using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Picturez_Lib;
using System.Diagnostics;
using Microsoft.Win32;
using Gtk;

namespace Picturez
{
	public class DesktopContextMenu
	{
		private static DesktopContextMenu instance;
		public static DesktopContextMenu I
		{
			get
			{
				if (instance == null)
					instance = new DesktopContextMenu ();

				return instance;
			}
		}

		/// <summary>
		/// Deletes or adds Picturez context menu in Linux (Ubuntu derivates) desktops.
		/// If 'add==true', Picturez context menu will be added / overwritten.
		/// Otherwise, Picturez context menu will be deleted.
		/// </summary>
		public void LinuxDesktopContextMenu(bool add)
		{
			Linux_CreateDesktopFile (add);
			Linux_OverwriteOrDeleteInMinmappsFile (add);
		}

		/// <summary>
		/// Deletes or adds Picturez.desktop file in Linux (Ubuntu derivates) desktops.
		/// If 'add==true', Picturez.desktop file will be added / overwritten.
		/// Otherwise, Picturez.desktop file will be deleted.
		/// </summary>
		private void Linux_CreateDesktopFile(bool add)
		{
			#region desktop file

			string desktopPath = Constants.I.HOMEPATH + 
				".local/share/applications/";
			string deskopFile = desktopPath + "Picturez.desktop";

			string[] lines = { 
				"[Desktop Entry]", 
				"Name=" + Constants.TITLE, // + " " + Language.I.L[67],
				"Comment=" + Language.I.L[54], 
				"Exec=mono '" + Constants.I.EXEPATH + Constants.EXENAME + "' %F",
				"Type=Application",
				"Terminal=false", 
				"Icon=" + Constants.I.EXEPATH + Constants.ICONNAME,
				// Constants.I.PICTUREZ_COMMENT, 
				"Categories=GTK;Graphics;Viewer;RasterGraphics;2DGraphics;Photography;", 
				"MimeType=image/bmp;image/gif;image/jpeg;image/jpg;image/pjpeg;image/png;" + 
				"image/tiff;image/x-bmp;image/x-gray;image/x-icb;image/x-ico;image/x-png;" + 
				"image/x-portable-anymap;image/x-portable-bitmap;image/x-portable-graymap;" + 
				"image/x-portable-pixmap;image/x-xbitmap;image/x-xpixmap;image/x-pcx;",
				// "image/svg+xml;image/svg+xml-compressed;image/vnd.wap.wbmp;",
				"StartupNotify=false"};

			// WriteAllLines creates a file, writes a collection of strings to the file,
			// and then closes the file. No need to call Flush() or Close().
			if (add) {
				Directory.CreateDirectory (desktopPath);
				File.WriteAllLines (deskopFile, lines);
			}
			else if (File.Exists(deskopFile))
				File.Delete(deskopFile);

			#endregion desktop file

			#region desktop directory
			string deskopFile_directory = desktopPath + "Picturez_directory.desktop";

			string[] lines_directory = { 
				"[Desktop Entry]", 
				"Name=" + Constants.TITLE, // + " " + Language.I.L[68],
				"Comment=" + Language.I.L[54], 
				"Exec=mono '" + Constants.I.EXEPATH + Constants.EXENAME + "' -d %f",
				"Type=Application",
				"Terminal=false", 
				"Icon=" + Constants.I.EXEPATH + Constants.ICONNAME,
				// Constants.I.PICTUREZ_COMMENT, 
				"Categories=GTK;Graphics;Viewer;RasterGraphics;2DGraphics;Photography;", 
				"MimeType=inode/directory;",
				"StartupNotify=false"};

			// WriteAllLines creates a file, writes a collection of strings to the file,
			// and then closes the file. No need to call Flush() or Close().
			if (add) {
				Directory.CreateDirectory (desktopPath);
				File.WriteAllLines (deskopFile_directory, lines_directory);
			}
			else if (File.Exists(deskopFile_directory))
				File.Delete(deskopFile_directory);
			#endregion desktop directory
		}


		/// <summary>
		/// Deletes or adds / overwrites Picturez entries in linux mimeapps.list.
		/// If 'add==true', the Picturez entries will be added / overwritten.
		/// Otherwise, the Picturez entries will be deleted.
		/// </summary>
		private void Linux_OverwriteOrDeleteInMinmappsFile(bool add)
		{
			string mimeappsFile = Constants.I.HOMEPATH + 
				".local/share/applications/mimeapps.list";

			if (!File.Exists (mimeappsFile)) {	
				if (add)
					File.CreateText (mimeappsFile).Close();
				else
					return;
			} 

			// read file
			List<string> lines = new List<string>();
			string line;
			bool existsAddedAssociations = false;

			StreamReader readFile = 
				new StreamReader(mimeappsFile);
			while((line = readFile.ReadLine()) != null)
			{
				if (line.Contains("[Added Associations]"))
					existsAddedAssociations = true;

				if (!line.Contains("Picturez"))
				lines.Add(line);
			}

			readFile.Close();

			if (add) {
				if (!existsAddedAssociations) {
					lines.Add("[Added Associations]");
					lines.Add("");
				}

				lines.Add("inode/directory=Picturez_directory.desktop");
				lines.Add("image/bmp=Picturez.desktop");
				lines.Add("image/emf=Picturez.desktop");
				lines.Add("image/gif=Picturez.desktop");
				lines.Add("image/ico=Picturez.desktop");
				lines.Add("image/x-ico=Picturez.desktop");
				lines.Add("image/jpeg=Picturez.desktop");
				lines.Add("image/jpg=Picturez.desktop");
				lines.Add("image/pjpeg=Picturez.desktop");
				lines.Add("image/png=Picturez.desktop");
				lines.Add("image/tiff=Picturez.desktop");
				lines.Add("image/wmf=Picturez.desktop");	
			}

			if (lines.Count != 0)
				File.WriteAllLines(mimeappsFile, lines);
		}


		/// <summary>
		/// Deletes or adds Picturez context menu in Windows desktops.
		/// If 'add==true', Picturez context menu will be added / overwritten.
		/// Otherwise, Picturez context menu will be deleted.
		/// </summary>
		public void WindowsDesktopContextMenu(bool add)
		{
			bool entriesExists = Win_AreCorrectContextMenuEntries ();

			if (add && entriesExists || !add && !entriesExists)
			{
				// Entries correct, nothing to do
				return;
			}

			if (!add && entriesExists) {
				StartContextMenuExe (true);
				return;
			}

			// else if (add && !entriesExists)
			StartContextMenuExe (false);				
		}

		private static void StartContextMenuExe(bool remove)
		{		
			
			// VistaSecurity.RestartElevatedForUpdate();
			ProcessStartInfo startInfo = new ProcessStartInfo();
			startInfo.UseShellExecute = true;
			startInfo.WorkingDirectory = Environment.CurrentDirectory;
			startInfo.FileName = Constants.I.EXEPATH + "WinContextMenu.exe";
			// run as admin
			startInfo.Verb = "runas";
			if (remove)
				startInfo.Arguments = "Remove";

			Process.Start(startInfo);

			//				try
			//				{
			//					Process.Start(startInfo);
			//				}
			//				catch(System.ComponentModel.Win32Exception)
			//				{
			//					//If cancelled, do nothing
			//				}			
		}

		/// <summary>
		/// Determines whether the windows explorer context menu entries for 
		/// Picturez are correct in the win registry.
		/// </summary>
		/// <returns>True, if contextmenu are correct.</returns>
		private static bool Win_AreCorrectContextMenuEntries()
		{
			RegistryKey regmenu;
			try
			{
				// 1.) check CONVERT WITH PICTUREZ
				regmenu = Registry.ClassesRoot.OpenSubKey(
					"*\\shell\\Picturez\\Command", false);
				//If format does not already exist, return true
				if (regmenu == null)
				{
					return false;
				}

//				// 2.) check EDIT WITH PICTUREZ
//				regmenu = Registry.ClassesRoot.OpenSubKey(
//					"*\\shell\\Edit with Picturez\\Command", false);
//				//If format does not already exist, return true
//				if (regmenu == null)
//				{
//					return false;
//				}

				// 3.) check FOLDER WITH PICTUREZ
				regmenu = Registry.ClassesRoot.OpenSubKey(
					"Folder" + "\\shell\\Picturez\\Command", false);
				//If format does not already exist, return true
				if (regmenu == null)
				{
					return false;
				}

				regmenu.Close();
				return true;
			}
			catch (Exception ex)
			{
				MessageDialog md = new MessageDialog(null, DialogFlags.DestroyWithParent, 
					MessageType.Error, ButtonsType.Ok, "Error: " + ex.Message);
				md.Run();
				md.Destroy ();
				return false;
			}
		}
	}
}

