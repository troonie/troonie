using System;
using Gtk;
using System.Collections.Generic;

namespace TroonieSqlite
{
	public class MainClass : Troonie_Lib.IPlugin
	{
		private enum TargetType {
			String,
			RootWindow
		};

		public static TargetEntry [] Target_table = new TargetEntry [] {
			new TargetEntry ("STRING", 0, (uint) TargetType.String ),
			new TargetEntry ("text/plain", 0, (uint) TargetType.String),
			new TargetEntry ("application/x-rootwindow-drop", 0, (uint) TargetType.RootWindow)
		};

		public static void Main (string[] args)
		{
			Application.Init ();
			MainClass mc = new MainClass ();

			mc.Start (new List<string>(args));
			Application.Run ();
		}

		public void Start (List<string> filenames)
		{
			MainWindow win = new MainWindow (filenames);
			win.Show ();
		}

		public void Start (string filename)
		{
			List<string> list = new List<string> ();
			list.Add (filename);
			MainWindow win = new MainWindow (list);
			win.Show ();
		}
	}
}
