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
				}
			}

			if (needRepopulate) {
				rowNr = 0;
				colNr = 0;
				Widget[] widgetList = tableViewer.Children;

				foreach (Widget item in widgetList) {
					tableViewer.Remove (item);
				}

				for (int i = widgetList.Length - 1; i >= 0;  i--) {
//				foreach (ViewerImagePanel vip in widgetList) {
//					ViewerImagePanel vip2 = new ViewerImagePanel (IncrementImageID(), newImages [i], smallVipWidthAndHeight, maxVipWidth, maxVipHeight);
//					ImageFullPaths.Add (newImages [i]);
//					vip2.OnIsPressedInChanged += OnIsPressedIn;
//					vip2.OnDoubleClicked += OnDoubleClicked;
					tableViewer.Attach (widgetList[i], rowNr, rowNr + 1, colNr, colNr + 1, 
						AttachOptions.Shrink, AttachOptions.Shrink, 0, 0);					

					if (rowNr + 1 == imagePerRow) {
						rowNr = 0;
						colNr++;
					} else {
						rowNr++;
					}
				}

				MoveVIP (Gdk.Key.Right);
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

