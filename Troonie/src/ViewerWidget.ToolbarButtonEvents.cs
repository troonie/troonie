using Gtk;
using System;
using System.Collections.Generic;
using Troonie_Lib;
using System.Diagnostics;
using System.Globalization;

namespace Troonie
{	
	public partial class ViewerWidget
	{
		#region toolbar button events

		protected void OnToolbarBtn_OpenPressed(object sender, EventArgs e)
		{
			FileChooserDialog fc = GuiHelper.I.GetImageFileChooserDialog (true);

			if (fc.Run() == (int)ResponseType.Ok) 
			{
				FillImageList(new List<string>(fc.Filenames));
			}

			fc.Destroy();
		}

		protected void OnToolbarBtn_SelectAllPressed (object sender, EventArgs e)
		{
			foreach (ViewerImagePanel2 vip in tableViewer.Children) {
				vip.SetPressedIn (true);
			}
		}

		protected void OnToolbarBtn_ClearPressed (object sender, EventArgs e)
		{
			foreach (ViewerImagePanel2 vip in tableViewer.Children) {
				vip.SetPressedIn (false);
			}
		}

		protected void OnToolbarBtn_RemovePressed (object sender, EventArgs e)
		{
			for (int i = 0; i < tableViewer.Children.Length; i++) {
				ViewerImagePanel2 vip = tableViewer.Children[i] as ViewerImagePanel2;
				if (vip.IsPressedin) {
					tableViewer.Remove (vip);
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

