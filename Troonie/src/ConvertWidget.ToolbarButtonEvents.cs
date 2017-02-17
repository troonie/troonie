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
			FileChooserDialog fc = GuiHelper.I.GetImageFileChooserDialog (true, false);

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

		protected void OnToolbarBtn_LanguagePressed (object sender, EventArgs e)
		{
			Language.I.LanguageID++;
			SetLanguageToGui ();
		}						

		#endregion toolbar button events
	}
}

