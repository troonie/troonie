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
				FillImageList(fc.Filenames);
			}

			fc.Destroy();
		}

		protected void OnToolbarBtn_SelectAllPressed (object sender, EventArgs e)
		{
			foreach (ViewerImagePanel vip in tableViewer.Children) {
				vip.IsPressedIn = true;
			}
		}

		protected void OnToolbarBtn_ClearPressed (object sender, EventArgs e)
		{
			foreach (ViewerImagePanel vip in tableViewer.Children) {
				if (vip.IsDoubleClicked) {
					doubleClickedMode = false;
					tableViewer.RowSpacing = tableViewerSpacing; 
					tableViewer.ColumnSpacing = tableViewerSpacing;

					vip.IsDoubleClicked = false;
				} else if (vip.IsPressedIn){
					vip.IsPressedIn = false;
				}					
				vip.Show ();
			}
		}

		protected void OnToolbarBtn_RemovePressed (object sender, EventArgs e)
		{
			for (int i = 0; i < tableViewer.Children.Length; i++) {
				ViewerImagePanel vip = tableViewer.Children[i] as ViewerImagePanel;
				if (vip.IsPressedIn) {
					vip.IsPressedIn = false;
					vip.OnIsPressedInChanged -= OnIsPressedIn;
					vip.OnDoubleClicked -= OnDoubleClicked;
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

