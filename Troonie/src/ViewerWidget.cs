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
//	public struct TableTagsViewerRowElement
//	{
//		public Label TagName;
//		public Label TagData;
//		public TroonieButton ChangeBtn;
//	}

	public partial class ViewerWidget : Gtk.Window
	{
		private const string blackFileName = "black.png";
		private const int smallVipWidthAndHeight = 300;
		private const int tableViewerSpacing = 6;
		private static int startW, startH, maxVipWidth, maxVipHeight;

		private Troonie.ColorConverter colorConverter = Troonie.ColorConverter.Instance;
		private Constants constants = Constants.I;
//		private int imageW; 
//		private int imageH;
//		private string tempScaledImageFileName;
		private int imageId, imagePerRow;
		private uint rowNr, colNr;
		private bool leftControlPressed, leftAltPressed, doubleClickedMode;
		private List<ViewerImagePanel> pressedVips;

//		public string FileName { get; set; }
		public BitmapWithTag bt;
		public List<string>ImageFullPaths { get; private set; }
//		public List<TableTagsViewerRowElement> TableTagsViewerRowElements { get; private set; }
		public Table TableViewer { get { return tableViewer; }  }

		public ViewerWidget (string[] newImages) :	base (Gtk.WindowType.Toplevel)
		{
			Build ();
			tableViewer.RowSpacing = tableViewerSpacing;
			tableViewer.ColumnSpacing = tableViewerSpacing;
			this.SetIconFromFile(Constants.I.EXEPATH + Constants.ICONNAME);
			ImageFullPaths = new List<string> ();
			pressedVips = new List<ViewerImagePanel> ();
//			TableTagsViewerRowElements = new List<TableTagsViewerRowElement> ();
			imageId = -1;

//			int monitor = Screen.GetMonitorAtWindow (this.GdkWindow); 
//			Gdk.Rectangle bounds = Screen.GetMonitorGeometry (monitor);
			int wx = 20;
			int wy = 20;
			startW = Screen.Width - 2 * wx;
			startH = Screen.Height - 2 * wy - 70 /*taskbarHeight*/;
			maxVipWidth = startW - frame1.WidthRequest - 60;
			maxVipHeight = startH - 60  /* ToolbarIconButtonHeight */ ;

			Move (wx, wy);

			Resize (startW, startH);

//			scrolledwindowViewer.WidthRequest = 1300;

			imagePerRow = (int)((startW - frame1.WidthRequest - 10) / (smallVipWidthAndHeight + tableViewer.ColumnSpacing));
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

		private int IncrementImageID()
		{
			if (imageId == int.MaxValue)
				imageId = -1;

			imageId++;
			return imageId;
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
			uint nr = 0;
//			foreach (string s in Enum.GetNames(typeof(TagsFlag))) {
			string[] tagFlagNames = Enum.GetNames(typeof(TagsFlag));
			for (int i = 1; i < tagFlagNames.Length; i++) {
				string s = tagFlagNames [i];
				Label lbTagName = new Label (s);
				lbTagName.TooltipText = s;
				lbTagName.WidthRequest = 70;
//				string strTagData = s == "Keywords" ? "data, data, data, data" : "1234";
				Label lbTagData = new Label ();
				lbTagData.WidthRequest = 120;
				TroonieButton b = new TroonieButton ();
				b.Text = "...";
//				b.TextSize = 10;
				b.ButtonHeight = 20;
				b.ButtonWidth = 30;
				b.Name = i.ToString();
				b.ButtonReleaseEvent += OnTroonieBtnReleaseEvent; // (o, args) => {};

//				TableTagsViewerRowElements.Add (new TableTagsViewerRowElement { TagName = lbTagName, TagData = lbTagData, ChangeBtn = b });
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
				if (vip.IsPressedIn) {
					uint? old = vip.TagsData.Rating;
					vip.TagsData.Rating = rating;
					bool success = ImageTagHelper.SetTag (vip.OriginalImageFullName, TagsFlag.Rating, vip.TagsData);
					if (success) {
						vip.QueueDraw ();
					} else {
						vip.TagsData.Rating = old;
					}
				}
			}
//			tableViewer.ShowAll ();
		}

		private void MoveVIP(Gdk.Key key)
		{
			if (!doubleClickedMode || tableViewer.Children.Length <= 1)
				return;

			int l = tableViewer.Children.Length;
			ViewerImagePanel vip_old = null, vip_new;
			int i_old = 0, i_new = 0;

			for (int i = 0; i < l; i++) {
				ViewerImagePanel vip = tableViewer.Children[i] as ViewerImagePanel;
				if (vip.IsDoubleClicked) {
					i_old = i;
					vip_old = vip;
					break;
				}
			}

			// NOTE: tableViewer.Children list is inverted (LIFO, last image will be stored as first element in list)
			switch (key) {
			case Gdk.Key.Right:
					i_new = i_old == 0 ? l - 1 : i_old - 1;
				break;
			case Gdk.Key.Left:
					i_new = i_old == l - 1 ? 0 : i_old + 1;
				break;
			}

			vip_old.IsDoubleClicked = false;

			vip_new = tableViewer.Children[i_new] as ViewerImagePanel;
			vip_new.IsDoubleClicked = true;
			vip_new.IsPressedIn = true;
			vip_new.Show ();
		}

		private void OnDoubleClicked(ViewerImagePanel vip)
		{
			doubleClickedMode = true;
			tableViewer.RowSpacing = 0; 
			tableViewer.ColumnSpacing = 0;

			for (int i = 0; i < tableViewer.Children.Length; i++) {
				ViewerImagePanel l_vip = tableViewer.Children[i] as ViewerImagePanel;
				if (l_vip.ID == vip.ID) {
//					l_vip.IsPressedIn = true;
				} else {
					l_vip.IsPressedIn = false;
					l_vip.Hide ();
				}
			}				
		}

		private void OnIsPressedIn(ViewerImagePanel vip)
		{
			if (vip.IsPressedIn) {
				pressedVips.Add (vip);
			} else {
				pressedVips.Remove (vip);
			}				

			bool addMode = false;
			int l = tableTagsViewer.Children.Length;

			for (int k = 0, i = l - 2; k < l / 3; i-=3, k++) {
				TagsFlag flag = (TagsFlag)(1 << k);
				string s = EnterMetaDataWindow.SetStartText(pressedVips, flag, ref addMode);
				Label lb = tableTagsViewer.Children[i] as Label;
				lb.Text = s; //s.Length > maxLengthLabelTagData ? s.Substring(0, maxLengthLabelTagData) : s;
				lb.TooltipText = s;
			}
		}

		#region drag and drop

		private void FillImageList(string[] newImages)
		{
			FillImageList (new List<string>(newImages));
		}

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

					ViewerImagePanel vip2 = new ViewerImagePanel (IncrementImageID(), newImages [i], smallVipWidthAndHeight, maxVipWidth, maxVipHeight);
					ImageFullPaths.Add (newImages [i]);
					vip2.OnIsPressedInChanged += OnIsPressedIn;
					vip2.OnDoubleClicked += OnDoubleClicked;
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

			#region 'alt + ...'
			if (leftAltPressed) {
				List<ViewerImagePanel>pressedInVIPs = new List<ViewerImagePanel>();
				foreach (ViewerImagePanel vip in tableViewer.Children) {
					//				vip.IsPressedIn = true;
					if (vip.IsPressedIn) {
						pressedInVIPs.Add (vip);
					}
				}

				TagsFlag t;
				switch (args.Event.Key) {
				case Gdk.Key.f:
					t = TagsFlag.FocalLength;
					break;
				case Gdk.Key.t:
					t = TagsFlag.Keywords;
					break;
				case Gdk.Key.c:
					t = TagsFlag.Comment;
					break;
				default:
					return;
				}

				EnterMetaDataWindow pw = new EnterMetaDataWindow (pressedInVIPs, t);
				pw.WindowPosition = WindowPosition.CenterAlways;
				pw.Title = Language.I.L [167];
				pw.OkButtontext = Language.I.L [16];
				//					pw.OnReleasedOkButton += SetTag;
				pw.Show ();

				return;
			}
			#endregion 'alt + ...'

			switch (args.Event.Key) {
			case Gdk.Key.Control_L:
				leftControlPressed = true;
				break;
			case Gdk.Key.Alt_L:
				leftAltPressed = true;
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
			case Gdk.Key.Left:
				MoveVIP (Gdk.Key.Left);
				break;
			case Gdk.Key.Right:
				MoveVIP (Gdk.Key.Right);
				break;
			}

			// args.RetVal = true;
		}

		[GLib.ConnectBefore ()] 
		protected void OnKeyReleaseEvent (object o, KeyReleaseEventArgs args)
		{
			switch (args.Event.Key) {
			case Gdk.Key.Control_L:
				leftControlPressed = false;
				break;
			case Gdk.Key.Alt_L:
				leftAltPressed = false;
				break;
			}

			// args.RetVal = true;
		}

		#endregion key events

		protected void OnTroonieBtnReleaseEvent (object o, ButtonReleaseEventArgs args)
		{
			TroonieButton tb = o as TroonieButton;

			if (tb != null) {
				Console.WriteLine ("Trooniebutton-Name: " + tb.Name);
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
	}
}

