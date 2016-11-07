using System;
using Troonie_Lib;
using System.Collections.Generic;
using Gtk;
using System.Drawing;
using ImageConverter = Troonie_Lib.ImageConverter;
using IOPath = System.IO.Path;
using System.IO;
using System.Linq;

namespace Troonie
{
	public struct TableTagsViewerRowElement
	{
		public Label TagName;
		public Label TagData;
		public TroonieButton ChangeBtn;
	}

	public partial class ViewerWidget : Gtk.Window
	{
		private const string blackFileName = "black.png";

		private Troonie.ColorConverter colorConverter = Troonie.ColorConverter.Instance;
		private Constants constants = Constants.I;
//		private int imageW; 
//		private int imageH;
//		private string tempScaledImageFileName;
		private int startWidth, imagePerRow;
		private uint rowNr, colNr;
		private bool leftControlPressed;
		private List<ViewerImagePanel> pressedVips;

//		public string FileName { get; set; }
		public BitmapWithTag bt;
		public List<string>ImageFullPaths { get; private set; }
		public List<TableTagsViewerRowElement> TableTagsViewerRowElements { get; private set; }


		public ViewerWidget (List<string> newImages) :	base (Gtk.WindowType.Toplevel)
		{
			Build ();
			this.SetIconFromFile(Constants.I.EXEPATH + Constants.ICONNAME);
			ImageFullPaths = new List<string> ();
			pressedVips = new List<ViewerImagePanel> ();
			TableTagsViewerRowElements = new List<TableTagsViewerRowElement> ();

//			int monitor = Screen.GetMonitorAtWindow (this.GdkWindow); 
//			Gdk.Rectangle bounds = Screen.GetMonitorGeometry (monitor);
			int wx = 20;
			int wy = 20;
			startWidth = Screen.Width - 2 * wx;
			Move (wx, wy);

			Resize (startWidth, Screen.Width - 2 * wy - 100 /*taskbarHeight*/);

//			scrolledwindowViewer.WidthRequest = 1300;

			imagePerRow = (int)((startWidth - frame1.WidthRequest - 10) / (ViewerImagePanel.BiggestLengthSmall + tableViewer.ColumnSpacing));
			rowNr = 0; 
			colNr = 0;

			InitTableTagsViewer ();

			if (newImages != null)
				FillImageList (newImages);



			GuiHelper.I.CreateToolbarIconButton (hboxToolbarButtons, 0, "folder-new-3.png", Language.I.L[39], OnToolbarBtn_OpenPressed);
			GuiHelper.I.CreateToolbarIconButton (hboxToolbarButtons, 1, "edit-select-all.png", Language.I.L[40], OnToolbarBtn_SelectAllPressed);
			GuiHelper.I.CreateToolbarIconButton (hboxToolbarButtons, 2, "edit-clear-3.png", Language.I.L[41], OnToolbarBtn_ClearPressed);
			GuiHelper.I.CreateToolbarIconButton (hboxToolbarButtons, 3, "window-close-2.png", Language.I.L[42], OnToolbarBtn_RemovePressed);
			GuiHelper.I.CreateToolbarSeparator (hboxToolbarButtons, 4);
			GuiHelper.I.CreateDesktopcontextmenuLanguageAndInfoToolbarButtons (hboxToolbarButtons, 5, OnToolbarBtn_LanguagePressed);

			SetGuiColors ();
			SetLanguageToGui ();

			if (constants.WINDOWS) {
				Gtk.Drag.DestSet (this, 0, null, 0);
			} else {
				// Original is ShadowType.EtchedIn, but linux cannot draw it correctly.
				// Otherwise ShadowType.In looks terrible at Win10.

//				frameCursorPos.ShadowType = ShadowType.In;
//				frameSteganography.ShadowType = ShadowType.In;
//				frameModus.ShadowType = ShadowType.In;
//				frameKey.ShadowType = ShadowType.In;
//				frameContent.ShadowType = ShadowType.In;
				Gtk.Drag.DestSet (this, DestDefaults.All, MainClass.Target_table, Gdk.DragAction.Copy);
			}				

			if (Constants.I.CONFIG.AskForDesktopContextMenu) {
				new AskForDesktopContextMenuWindow (true, Constants.I.CONFIG).Show ();
			}					
		}

		public override void Destroy ()
		{
			ImageFullPaths.Clear ();
			if (bt != null) {
				bt.Dispose ();
			}				

			base.Destroy ();
		}

		protected void OnDeleteEvent (object sender, DeleteEventArgs a)
		{
			if (bt != null) {
				bt.Dispose ();
			}
			this.DestroyAll ();

			Application.Quit ();
			a.RetVal = true;

//			File.Delete (tempScaledImageFileName);
//			File.Delete (Constants.I.EXEPATH + blackFileName);
		}

		private void InitTableTagsViewer()
		{
			const int maxL = 10;
			uint nr = 0;
			foreach (string s in Enum.GetNames(typeof(Tags))) {
				Label lbTagName = new Label (s.Length > maxL ? s.Substring(0, maxL) : s);
				lbTagName.TooltipText = s;
				string strTagData = s == "Keywords" ? "data, data, data, data" : "1234";
				Label lbTagData = new Label (strTagData);
				TroonieButton b = new TroonieButton ();
				b.Text = "...";
//				b.TextSize = 10;
				b.ButtonHeight = 20;
				b.ButtonWidth = 30;

				TableTagsViewerRowElements.Add (new TableTagsViewerRowElement { TagName = lbTagName, TagData = lbTagData, ChangeBtn = b });
				tableTagsViewer.Attach (lbTagName, 0, 1, nr, nr + 1, AttachOptions.Shrink, AttachOptions.Shrink, 0, 0);
				tableTagsViewer.Attach (lbTagData, 1, 2, nr, nr + 1, AttachOptions.Shrink, AttachOptions.Shrink, 0, 0);
				tableTagsViewer.Attach (b, 2, 3, nr, nr + 1, AttachOptions.Shrink, AttachOptions.Shrink, 0, 0);

				nr++;
			}
		}

		private void SetGuiColors()
		{
			this.ModifyBg(StateType.Normal, colorConverter.GRID);
			eventboxToolbar.ModifyBg(StateType.Normal, colorConverter.GRID);
			eventboxTagsViewer.ModifyBg(StateType.Normal, colorConverter.GRID);
			eventboxViewer.ModifyBg(StateType.Normal, colorConverter.GRID);
//			this.tableViewer.ModifyFg(StateType.Normal, colorConverter.GRID);
//			this.scrolledwindowViewer.ModifyFg(StateType.Normal, colorConverter.GRID);

//			lbFrameCursorPos.ModifyFg (StateType.Normal, colorConverter.FONT);
//			lbCursorPos.ModifyFg (StateType.Normal, colorConverter.FONT);
//
//			lbFrameSteganography.ModifyFg (StateType.Normal, colorConverter.FONT);
//			lbFrameModus.ModifyFg (StateType.Normal, colorConverter.FONT);
//			lbFrameKey.ModifyFg (StateType.Normal, colorConverter.FONT);
//			lbFrameContent.ModifyFg (StateType.Normal, colorConverter.FONT);
		}

		private void SetLanguageToGui()
		{
//			lbFrameCursorPos.LabelProp = "<b>" + Language.I.L[15] + "</b>";
//			btnOk.Text = Language.I.L[16];
//			btnOk.Redraw ();
//
		}

		private void SetRatingOfSelectedImages(uint rating)
		{
			for (int i = 0; i < tableViewer.Children.Length; i++) {
				ViewerImagePanel vip = tableViewer.Children[i] as ViewerImagePanel;
				if (vip.IsPressedin) {
					uint? old = vip.TagsData.Rating;
					vip.TagsData.Rating = rating;
					bool success = ImageTagHelper.SetTag (vip.OriginalImageFullName, Tags.Rating, vip.TagsData);
					if (success) {
						vip.QueueDraw ();
					} else {
						vip.TagsData.Rating = old;
					}
				}
			}
//			tableViewer.ShowAll ();
		}

		private void zzz(ViewerImagePanel vip)
		{
			if (vip.IsPressedin) {
				pressedVips.Add (vip);
			} else {
				pressedVips.Remove (vip);
			}

			Console.WriteLine ("IsPressedIn-Anzahl: " + pressedVips.Count);
//			foreach (var l_vip in pressedVips) {
//				if (!l_vip.IsPressedin) {
//					
//				}
//			}
		}

		#region drag and drop

		private void FillImageList(List<string> newImages)
		{
//			foreach (string s in newImages) {
			for (int i = 0; i < newImages.Count; ++i) {

				string waste = Constants.I.WINDOWS ? "file:///" : "file://";
				newImages [i] = newImages [i].Replace (@waste, "");
				// Also change possible wrong directory separator
				newImages [i] = newImages [i].Replace (IOPath.AltDirectorySeparatorChar, IOPath.DirectorySeparatorChar);

				// check whether file is image or video
				FileInfo info = new FileInfo (newImages [i]);
				string ext = info.Extension.ToLower ();

				if (ext.Length != 0 && (Constants.Extensions.Any (x => x.Value.Item1 == ext || x.Value.Item2 == ext)  /* ||
				    Constants.VideoExtensions.Any (x => x.Value.Item1 == ext || x.Value.Item2 == ext || x.Value.Item3 == ext) */ ) && 
					!ImageFullPaths.Contains(newImages[i])) {

					ImageFullPaths.Add (newImages [i]);
					ViewerImagePanel vip2 = new ViewerImagePanel (newImages [i], true /* path  + relativeSmall, path + relativeBig */);
					vip2.OnIsPressedInChanged += zzz;
					tableViewer.Attach (vip2, rowNr, rowNr + 1, colNr, colNr + 1, 
						AttachOptions.Shrink, AttachOptions.Shrink, 0, 0);

					if (rowNr + 1 == imagePerRow) {
						rowNr = 0;
						colNr++;
					} else {
						rowNr++;
					}
				}
			}

			ShowAll ();
		}

		protected void OnDragDrop (object sender, Gtk.DragDropArgs args)
		{
			Gtk.Drag.GetData
			((Gtk.Widget)sender, args.Context,
				args.Context.Targets[0], args.Time);
		}

		protected void OnDragDataReceived (object sender, Gtk.DragDataReceivedArgs args)
		{
			if (args.SelectionData.Length > 0
				&& args.SelectionData.Format == 8) {

				byte[] data = args.SelectionData.Data;
				string encoded = System.Text.Encoding.UTF8.GetString (data);

				// drag n drop at linux wont accept spaces, so it has to be replaced
				encoded = encoded.Replace ("%20", " ");

				List<string> newImages = new List<string> (encoded.Split ('\r', '\n'));
				newImages.RemoveAll (string.IsNullOrEmpty);

				// I don't know what last object (when Windows) is,
				//  but I tested and noticed that it is not a path
				if (Constants.I.WINDOWS)
					newImages.RemoveAt (newImages.Count-1);

				FillImageList (newImages);
				newImages.Clear ();
			}
		}			

		#endregion drag and drop

		#region key events

		[GLib.ConnectBefore ()] 
		protected void OnKeyPressEvent (object o, KeyPressEventArgs args)
		{
			// System.Console.WriteLine("Keypress: {0}", args.Event.Key);

			switch (args.Event.Key) {
			case Gdk.Key.Control_L:
				leftControlPressed = true;
				break;
			case Gdk.Key.a:
				if (leftControlPressed) {
					OnToolbarBtn_SelectAllPressed (null, null);
				}
				break;
			case Gdk.Key.Escape:
				OnToolbarBtn_ClearPressed (null, null);
				break;
			case Gdk.Key.Delete:
				OnToolbarBtn_RemovePressed (null, null);
				break;

			case Gdk.Key.Key_0:
				SetRatingOfSelectedImages (0);
				break;
			case Gdk.Key.Key_1:
//				OnToolbarBtn_RemovePressed (null, null);
				Console.WriteLine ("ONE!");
				SetRatingOfSelectedImages (1);
				break;
			case Gdk.Key.Key_2:
				SetRatingOfSelectedImages (2);
				break;
			case Gdk.Key.Key_3:
				SetRatingOfSelectedImages (3);
				break;
			case Gdk.Key.Key_4:
				SetRatingOfSelectedImages (4);
				break;
			case Gdk.Key.Key_5:
				SetRatingOfSelectedImages (5);
				break;

			}

			// args.RetVal = true;
		}

		[GLib.ConnectBefore ()] 
		protected void OnKeyReleaseEvent (object o, KeyReleaseEventArgs args)
		{
			// System.Console.WriteLine("Keyrelease: {0}", args.Event.Key);
			if (args.Event.Key == Gdk.Key.Control_L) {
				leftControlPressed = false;
			}

			// args.RetVal = true;
		}

		#endregion key events
	}
}

