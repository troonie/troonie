using System;
using Gtk;
using System.Collections.Generic;

namespace TroonieSqlite
{
	public class MainClass
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

		public void Start (List<string> pFilenames)
		{
			MainWindow win = new MainWindow (pFilenames);
			win.Show ();
		}
	}
}
