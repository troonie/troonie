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
			ad.Version = Troonie_Lib.Version.VERSION;
			ad.Website = Constants.WEBSITE;
			ad.Comments = Language.I.L [54]; // Constants.I.DESCRIPTION;
			ad.Title = Language.I.L[149] + " " + Constants.TITLE;
//			ad.Authors = new string[1] { Constants.AUTHOR};
//			ad.Artists = new string[1] { Constants.AUTHOR };
//			ad.Documenters = new string[1] { Constants.AUTHOR };

			ad.Copyright = 
				"© " + Constants.AUTHOR + Constants.N + Constants.N +
			"############ " + Language.I.L [138] + " ############" + Constants.N + Constants.N +
			Language.I.L [139] + Constants.N +
			Language.I.L [140] + Constants.N +
			Language.I.L [141] + Constants.N +
			Language.I.L [142] + Constants.N + Constants.N +
			Language.I.L [143] + Constants.N +
			Language.I.L [144] + Constants.N +
			Language.I.L [145] + Constants.N +
			Language.I.L [146] + Constants.N + Constants.N +
			Language.I.L [147] + Constants.N +
			Language.I.L [148] + Constants.N + Constants.N +
			Language.I.L [209] + Constants.N +
			Language.I.L [210] + Constants.N +
			Language.I.L [211] + Constants.N +
			Language.I.L [212] + Constants.N +
				Language.I.L [213] + Constants.N +
				Language.I.L [220] + Constants.N +
			Language.I.L [214] + Constants.N +
			Language.I.L [215] + Constants.N +
			Language.I.L [216] + Constants.N +
				Language.I.L [217] + Constants.N +
				Language.I.L [219] + Constants.N +
			Language.I.L [220] + Constants.N +
			Language.I.L [221] + Constants.N +
			Language.I.L [222] + Constants.N +
			Language.I.L [223] + Constants.N +
			Language.I.L [224] + Constants.N +
			Language.I.L [225] + Constants.N +
			Language.I.L [226] + Constants.N;
			ad.Icon = Gdk.Pixbuf.LoadFromResource (Constants.ICONNAME); 
			ad.Logo = Gdk.Pixbuf.LoadFromResource (Constants.ICONNAME); 
//			ad.HasSeparator = true;
			ad.Run ();
			ad.Destroy ();
		}
	}
}

