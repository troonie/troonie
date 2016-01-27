using System;
using Gtk;
using Picturez_Lib;

namespace Picturez
{
	public class PicturezAboutDialog
	{
		private static PicturezAboutDialog instance;
		public static PicturezAboutDialog I
		{
			get
			{
				if (instance == null)
					instance = new PicturezAboutDialog ();

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

