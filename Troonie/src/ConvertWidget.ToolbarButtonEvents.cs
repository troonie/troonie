using Gtk;
using System;
using System.Collections.Generic;
using Troonie_Lib;
using System.Diagnostics;
using System.Globalization;

namespace Troonie
{	
	public partial class ConvertWidget
	{
		#region toolbar button events

		protected void OnToolbarBtn_OpenPressed (object sender, EventArgs e)
		{			
			FileChooserDialog fc = GuiHelper.I.GetImageFileChooserDialog (true);

			if (fc.Run() == (int)ResponseType.Ok) 
			{
				FillImageList (new List<string>(fc.Filenames));
			}

			fc.Destroy();
		}

		protected void OnToolbarBtn_SelectAllPressed (object sender, EventArgs e)
		{
			foreach (PressedInButton pib in vboxImageList.Children) {
				pib.SetPressedIn (true);
			}
		}

		protected void OnToolbarBtn_ClearPressed (object sender, EventArgs e)
		{
			foreach (PressedInButton pib in vboxImageList.Children) {
				pib.SetPressedIn (false);
			}
		}

		protected void OnToolbarBtn_RemovePressed (object sender, EventArgs e)
		{
			for (int i = 0; i < vboxImageList.Children.Length; i++) {
				PressedInButton pib = vboxImageList.Children[i] as PressedInButton;
				if (pib.IsPressedin) {
					vboxImageList.Remove (pib);
					i--;
				}
			}
		}

		protected void OnToolbarBtn_InfoPressed (object sender, EventArgs e)
		{
			 TroonieAboutDialog.I.Run ();
		}

		protected void OnToolbarBtn_LanguagePressed (object sender, EventArgs e)
		{
			Language.I.LanguageID++;
			SetLanguageToGui ();
		}

		protected void OnToolbarBtn_DesktopContextMenuPressed (object sender, EventArgs e)
		{
			new AskForDesktopContextMenuWindow (false, config).Show();
		}

		protected void OnToolbarBtn_UpdatePressed (object sender, EventArgs e)
		{
			PseudoTroonieContextMenu pseudo = new PseudoTroonieContextMenu (false);
			pseudo.Title = Language.I.L [69];
			pseudo.Label1 = Language.I.L [70] + newVersion.ToString(CultureInfo.InvariantCulture);
			pseudo.Label2 = Language.I.L [71];
			pseudo.OkButtontext = Language.I.L [16];
			pseudo.CancelButtontext = Language.I.L [17];
			// pseudo.OnReleasedOkButton += delegate{Process.Start (Constants.WEBSITE);};
			pseudo.OnReleasedOkButton += () => Process.Start (Constants.WEBSITE);

			pseudo.Show ();
		}


		#endregion toolbar button events
	}
}

