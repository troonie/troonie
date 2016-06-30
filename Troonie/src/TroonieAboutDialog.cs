using System;
using Gtk;
using Troonie_Lib;

namespace Troonie
{
	public class TroonieAboutDialog
	{
		private static TroonieAboutDialog instance;
		public static TroonieAboutDialog I
		{
			get
			{
				if (instance == null)
					instance = new TroonieAboutDialog ();

				return instance;
			}
		}

		public void Run()
		{
			AboutDialog ad = new AboutDialog ();
			ad.ModifyBg(StateType.Normal, ColorConverter.Instance.GRID);
			ad.ProgramName = Constants.TITLE;
			ad.Version = Constants.VERSION;
			ad.Website = Constants.WEBSITE;
			ad.Comments = Constants.I.DESCRIPTION;
			ad.Authors = new string[1] { Constants.AUTHOR };
			ad.Artists = new string[1] { Constants.AUTHOR };
			ad.Documenters = new string[1] { Constants.AUTHOR };
			ad.Copyright = Constants.AUTHOR;
			ad.Icon = Gdk.Pixbuf.LoadFromResource (Constants.ICONNAME); 
			ad.Logo = Gdk.Pixbuf.LoadFromResource (Constants.ICONNAME); 
			ad.HasSeparator = true;
			ad.Run ();
			ad.Destroy ();
		}
	}
}

