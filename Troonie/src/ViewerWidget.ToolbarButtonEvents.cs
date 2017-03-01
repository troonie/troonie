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
			FileChooserDialog fc = GuiHelper.I.GetImageFileChooserDialog (true, true);

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

		void RemoveAndDeleteSelectedImages()
		{
			List<ViewerImagePanel>pressedInVIPs = GetPressedInVIPs();

			foreach (ViewerImagePanel vip in pressedInVIPs) {
				try {
					System.IO.File.Delete(vip.OriginalImageFullName);
				}
				catch (Exception)
				{ /* do nothing */ }
			}

			OnToolbarBtn_RemovePressed (null, null);
		}

		protected void OnToolbarBtn_RemoveAndDeleteFilePressed (object sender, EventArgs e)
		{
			if (Constants.I.CONFIG.ConfirmDeleteImages) {
				ConfirmingDeleteImagesWindow conf = new ConfirmingDeleteImagesWindow (Constants.I.CONFIG);
				conf.OnReleasedOkButton	+= RemoveAndDeleteSelectedImages;

				conf.Show ();
			} else {
				RemoveAndDeleteSelectedImages ();
			}
		}

		protected void OnToolbarBtn_RemovePressed (object sender, EventArgs e)
		{
			bool needRepopulate = false;
			int lastRemovedId = 0;
			for (int i = 0; i < tableViewer.Children.Length; i++) {
				ViewerImagePanel vip = tableViewer.Children[i] as ViewerImagePanel;
				if (vip.IsPressedIn) {
					ImageFullPaths.Remove(vip.OriginalImageFullName);
					vip.IsPressedIn = false;
					vip.OnIsPressedInChanged -= OnIsPressedIn;
					vip.OnDoubleClicked -= OnDoubleClicked;
					tableViewer.Remove (vip);
					vip.DestroyAll ();
					i--;
					needRepopulate = true;
					lastRemovedId = vip.ID;
				}
			}

			if (needRepopulate) {				
				rowNr = 0;
				colNr = 0;
				Widget[] widgetList = tableViewer.Children;

				foreach (Widget item in widgetList) {
					tableViewer.Remove (item);
				}

				if (widgetList.Length == 0)
					return;
				
				int nextId = (widgetList [widgetList.Length - 1] as ViewerImagePanel).ID;
				int diff = int.MaxValue;

				for (int i = widgetList.Length - 1; i >= 0;  i--) {
					tableViewer.Attach (widgetList[i], rowNr, rowNr + 1, colNr, colNr + 1, 
						AttachOptions.Shrink, AttachOptions.Shrink, 0, 0);					

					if (rowNr + 1 == imagePerRow) {
						rowNr = 0;
						colNr++;
					} else {
						rowNr++;
					}

					// calc next right neighbour to make it pressedIn
					int tmpId = (widgetList [i] as ViewerImagePanel).ID;
					if (tmpId > lastRemovedId && tmpId - lastRemovedId < diff) {
						nextId = tmpId;
						diff = Math.Abs (tmpId - lastRemovedId);
					}
				}
					
				// set next right neighbour to pressedIn
				foreach (ViewerImagePanel vip_new in tableViewer.Children) {
					if (vip_new.ID == nextId) {
						vip_new.IsPressedIn = true;
						vip_new.IsDoubleClicked = doubleClickedMode;
						vip_new.Show ();
						break;
					}
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

