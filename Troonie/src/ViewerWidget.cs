using System;
using Troonie_Lib;
using System.Collections.Generic;
using Gtk;
using System.Drawing;
using ImageConverter = Troonie_Lib.ImageConverter;
using IOPath = System.IO.Path;
using System.IO;
using System.Linq;
using System.Diagnostics;

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
		private bool leftControlPressed, leftShiftPressed, leftAltPressed, doubleClickedMode;
		private Dictionary<int, ViewerImagePanel> pressedVipsDict;

//		public string FileName { get; set; }
		public BitmapWithTag bt;
		public List<string>ImageFullPaths { get; private set; }
//		public List<TableTagsViewerRowElement> TableTagsViewerRowElements { get; private set; }
		public Table TableViewer { get { return tableViewer; }  }

		public ViewerWidget (string[] newImages) :	base (Gtk.WindowType.Toplevel)
		{
			try {
				Build ();
				tableViewer.RowSpacing = tableViewerSpacing;
				tableViewer.ColumnSpacing = tableViewerSpacing;
				this.SetIconFromFile(Constants.I.EXEPATH + Constants.ICONNAME);
				ImageFullPaths = new List<string> ();
				pressedVipsDict = new Dictionary<int, ViewerImagePanel>();
	//			TableTagsViewerRowElements = new List<TableTagsViewerRowElement> ();
				imageId = -1;

	//			int monitor = Screen.GetMonitorAtWindow (this.GdkWindow); 
	//			Gdk.Rectangle bounds = Screen.GetMonitorGeometry (monitor);
				int wx = 10;
				int wy = 10;
				startW = Screen.Width - 2 * wx;
				startH = Screen.Height - 2 * wy - 68 /*- 70*/ /*taskbarHeight*/;
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
				GuiHelper.I.CreateToolbarIconButton (hboxToolbarButtons, 4, "trash-empty-3.png", Language.I.L[191], OnToolbarBtn_RemoveAndDeleteFilePressed);
				GuiHelper.I.CreateToolbarSeparator (hboxToolbarButtons, 5);
				GuiHelper.I.CreateDesktopcontextmenuLanguageAndInfoToolbarButtons (hboxToolbarButtons, 6, OnToolbarBtn_LanguagePressed);

				SetGuiColors ();
				SetLanguageToGui ();

				if (constants.WINDOWS) {
					Gtk.Drag.DestSet (this, 0, null, 0);
				} else {
					// Original is ShadowType.EtchedIn, but linux cannot draw it correctly.
					// Otherwise ShadowType.In looks terrible at Win10.
					frame1.ShadowType = ShadowType.In;
					scrolledwindowViewer.ShadowType = ShadowType.In;

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
			catch (Exception ex) {

				OkCancelDialog win = new OkCancelDialog (true);
				win.WindowPosition = WindowPosition.CenterAlways;
				win.Title = Language.I.L [153];
				UnauthorizedAccessException uaaex = ex as UnauthorizedAccessException;
				if (uaaex != null) {					
					win.Label1 = Language.I.L [194];
					win.Label2 = Language.I.L [195];
				} else {
					win.Label1 = Language.I.L [194];
					win.Label2 = Language.I.L [195];
				}
				win.OkButtontext = Language.I.L [16];
				DeleteEventArgs args = new DeleteEventArgs ();
				win.OnReleasedOkButton += () => { OnDeleteEvent(win, args); };
				win.Show ();

				this.DestroyAll ();
			}
		}

		private List<ViewerImagePanel> GetPressedInVIPs()
		{
			List<ViewerImagePanel>pressedInVIPs = new List<ViewerImagePanel>();
			foreach (ViewerImagePanel vip in tableViewer.Children) {
				//				vip.IsPressedIn = true;
				if (vip.IsPressedIn) {
					pressedInVIPs.Add (vip);
				}
			}

			return pressedInVIPs;
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
				if (s == Enum.GetName (typeof(TagsFlag), TagsFlag.Width) || 
					s == Enum.GetName (typeof(TagsFlag), TagsFlag.Height) ||
					s == Enum.GetName (typeof(TagsFlag), TagsFlag.Pixelformat)) {
					b.Sensitive = false;
					b.ButtonHeight = 0;
					b.ButtonWidth = 0;
				}
				else {
					b.Text = "...";
					//				b.TextSize = 10;
					b.ButtonHeight = 20;
					b.ButtonWidth = 30;
					b.Name = (i - 1).ToString();
					b.ButtonReleaseEvent += OnTroonieBtnReleaseEvent; // (o, args) => {};
				}

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
			GtkLabel.LabelProp = "<b>" + Language.I.L[179] + "</b>";
//			lbFrameCursorPos.LabelProp = "<b>" + Language.I.L[15] + "</b>";
//			btnOk.Text = Language.I.L[16];
//			btnOk.Redraw ();
//
		}

		public static void ShowErrorDialog(string label1, string label2)
		{
			OkCancelDialog win = new OkCancelDialog (true);
			win.WindowPosition = WindowPosition.CenterAlways;
			win.Title = Language.I.L [153];
			win.Label1 = label1;
			win.Label2 = label2;
			win.OkButtontext = Language.I.L [16];
			//				DeleteEventArgs args = new DeleteEventArgs ();
			//				win.OnReleasedOkButton += () => { OnDeleteEvent(win, args); };
			win.Show ();
		}

		private void SetRatingOfSelectedImages(uint rating)
		{
			string errorImages = string.Empty;
			List<ViewerImagePanel>pressedInVIPs = GetPressedInVIPs();
			foreach (ViewerImagePanel vip in pressedInVIPs) {	
				uint? old = vip.TagsData.Rating;
				vip.TagsData.Rating = rating;
				bool success = true;

				if (!vip.IsVideo) { 
					success = ImageTagHelper.SetTag (vip.OriginalImageFullName, TagsFlag.Rating, vip.TagsData);	
				}

				if (success) {
					vip.QueueDraw ();
					// dirty workaround to refresh label strings of ViewerWidget.tableTagsViewer
					vip.IsPressedIn = vip.IsPressedIn;
				} else {
					vip.TagsData.Rating = old;
					errorImages += vip.RelativeImageName + Constants.N;
				}
			}
//			tableViewer.ShowAll ();

			if (errorImages.Length != 0) {		
				ShowErrorDialog(Language.I.L[254], errorImages + Constants.N);
			}
		}

		private void MoveVIP(Gdk.Key key)
		{
			if (!doubleClickedMode || tableViewer.Children.Length == 0)
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

			if (vip_old != null) {
				vip_old.IsDoubleClicked = false;
			}

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
					if (vip.IsVideo && Constants.I.CONFIG.VideoplayerWorks) {
						Process proc = new Process();
						proc.StartInfo.FileName = Constants.I.CONFIG.VideoplayerPath;   //xplayer ODER vlc ODER cvlc
						proc.StartInfo.Arguments = "\"" + vip.OriginalImageFullName + "\"";

						proc.StartInfo.UseShellExecute = false; 
						proc.StartInfo.RedirectStandardOutput = true;
						proc.StartInfo.RedirectStandardError = true;
						try {
							proc.Start();
	//						proc.WaitForExit();
							proc.Close();
						}
						catch (Exception ex) {
							// player could not be found
							Console.WriteLine (ex.Message);
						}
					}
//					l_vip.IsPressedIn = true;
				} else {
					l_vip.IsPressedIn = false;
					l_vip.Hide ();
				}
			}				
		}

		private void OnIsPressedIn(ViewerImagePanel vip)
		{
			if (vip.IsPressedIn && !pressedVipsDict.ContainsKey(vip.ID)) {
				pressedVipsDict.Add (vip.ID, vip);
			} else if (!vip.IsPressedIn && pressedVipsDict.ContainsKey(vip.ID)){
				pressedVipsDict.Remove(vip.ID);
			}			

			SaveTagMode saveTagMode;
			SaveTagModeKeywords saveTagMode2;

			int l = tableTagsViewer.Children.Length;

			for (int k = 0, i = l - 2; k < l / 3; i-=3, k++) {
				TagsFlag flag = (TagsFlag)(1 << k);
				string s = flag == TagsFlag.Keywords ? 
					EnterKeywordsWindow.SetStartText(new List<ViewerImagePanel>(pressedVipsDict.Values), out saveTagMode2) : 
					EnterMetaDataWindow.SetStartText(new List<ViewerImagePanel>(pressedVipsDict.Values), flag, out saveTagMode);
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

		private void FillImageList(List<string> newImages, bool check = true)
		{
			if (check && newImages.Count > Constants.MAX_NUMBER_OF_IMAGES)
			{
				OkCancelDialog warn_d = new OkCancelDialog(false);
				warn_d.Title = Language.I.L[29];
				warn_d.Label1 = Language.I.L[262] + newImages.Count + Language.I.L[263] + Constants.N + Language.I.L[264];
				warn_d.Label2 = Language.I.L[265];
				warn_d.OkButtontext = Language.I.L[16];
				warn_d.CancelButtontext = Language.I.L[17];
				warn_d.Show();

				warn_d.OnReleasedOkButton += () => FillImageList(newImages, false); //(newImages, true) => FillImageList;
				return;
			}

			const int length = 45;
			List<Tuple<ExceptionType, string>> errors = new List<Tuple<ExceptionType, string>> ();
			bool addingVideoPicture = false;
			bool isFirstQuestion = true;

//			foreach (string s in newImages) {
			for (int i = 0; i < newImages.Count; ++i) {

				string waste = Constants.I.WINDOWS ? "file:///" : "file://";
				newImages [i] = newImages [i].Replace (@waste, "");
				// Also change possible wrong directory separator
				newImages [i] = newImages [i].Replace (IOPath.AltDirectorySeparatorChar, IOPath.DirectorySeparatorChar);

				// check whether file is image or video
				FileInfo info = new FileInfo (newImages [i]);
				string ext = info.Extension.ToLower ();
				bool isImage = Constants.Extensions.Any (x => x.Value.Item1 == ext || x.Value.Item2 == ext);
				bool isVideo = Constants.VideoExtensions.Any (x => x.Value.Item1 == ext || x.Value.Item2 == ext || x.Value.Item3 == ext);

				// ask (and do) for adding video picture
				if (isVideo) {					

					if (isFirstQuestion) {
						isFirstQuestion = false;
						MessageDialog md = new MessageDialog (this, 
							DialogFlags.DestroyWithParent, MessageType.Question, 
							ButtonsType.OkCancel, Language.I.L [201]);
						//					md.Run ();
						md.KeepAbove = true;
						md.WindowPosition = WindowPosition.CenterAlways;

						ResponseType tp = (Gtk.ResponseType)md.Run();
//						if (tp == ResponseType.Ok) {
//							addingVideoPicture = true;
//
//						} else {
//							addingVideoPicture = false;
//						}
						addingVideoPicture = tp == ResponseType.Ok;
						md.Destroy ();
					} 

					if (addingVideoPicture) {


						string fullPicName = null; // = info.FullName + ".png";

						for (int k = 5; k >= 1; k--) {
//							string ss = info.FullName.Replace (Constants.Stars [k], string.Empty);
							fullPicName = info.FullName.Replace(Constants.Stars[k], string.Empty) + Constants.Stars[k] + ".png";
							if (File.Exists (fullPicName)) {
								break;
							}

							// If not found, setting default video pic filename
							fullPicName = info.FullName + ".png";
						}

						// if video pic does not exist, create it
						if (!File.Exists(fullPicName)) {
							TroonieBitmap.CreateTextBitmap (fullPicName, 
								info.FullName.Substring(info.FullName.LastIndexOf(IOPath.DirectorySeparatorChar) + 1));
						}

						// if video pic is not added yet, add it
						if (!newImages.Contains (fullPicName)) {
							newImages.Insert (i, fullPicName);
							info = new FileInfo (newImages [i]);
							ext = info.Extension.ToLower ();
							isImage = true;
							isVideo = false;
//							i++;
						}
					}
				}

				if (ext.Length != 0 && (isImage || isVideo) && !ImageFullPaths.Contains(newImages[i])) {

					try {
						ViewerImagePanel vip = new ViewerImagePanel (IncrementImageID(), isVideo, newImages [i], smallVipWidthAndHeight, maxVipWidth, maxVipHeight);
						if (info.IsReadOnly) {
							throw new UnauthorizedAccessException();
						}
						ImageFullPaths.Add (newImages [i]);
						vip.OnIsPressedInChanged += OnIsPressedIn;
						vip.OnDoubleClicked += OnDoubleClicked;
						tableViewer.Attach (vip, rowNr, rowNr + 1, colNr, colNr + 1, 
							AttachOptions.Shrink, AttachOptions.Shrink, 0, 0);					

						if (rowNr + 1 == imagePerRow) {
							rowNr = 0;
							colNr++;
						} else {
							rowNr++;
						}
					}
						catch (Exception ex) {
						ExceptionType errorType;
						if (ex as UnauthorizedAccessException != null) {
							errorType = ExceptionType.UnauthorizedAccessException;
						} else if (ex as System.IO.IOException != null) {
							errorType = ExceptionType.IO_IOException;
						} else {
							errorType = ExceptionType.Exception;
						}
						errors.Add (Tuple.Create(errorType, newImages [i]));
						}
				}
			}

			string mssg = string.Empty;
			if (errors.Count != 0) {
				var error0 = errors.Where(x => x.Item1 == ExceptionType.UnauthorizedAccessException);
				if (error0.Count() != 0) {
					mssg += Language.I.L [196] + Constants.N + Constants.N;
					foreach (var t in error0) {
						string errorimage = t.Item2;
						int l = errorimage.Length;
						if (l < length) {
							mssg += "  *  " + errorimage + Constants.N;
						} else {
							mssg += "  *  ..." + errorimage.Substring (l - length) + Constants.N;
						}
					}						
				}

				error0 = errors.Where(x => x.Item1 == ExceptionType.IO_IOException);
				if (error0.Count() != 0) {
					mssg += Constants.N + Constants.N + Language.I.L [197] + Constants.N + Constants.N;
					foreach (var t in error0) {
						string errorimage = t.Item2;
						int l = errorimage.Length;
						if (l < length) {
							mssg += "  *  " + errorimage + Constants.N;
						} else {
							mssg += "  *  ..." + errorimage.Substring (l - length) + Constants.N;
						}
					}						
				}

				error0 = errors.Where(x => x.Item1 == ExceptionType.Exception);
				if (error0.Count() != 0) {
					mssg += Constants.N + Constants.N + Language.I.L [198] + Constants.N + Constants.N;
					foreach (var t in error0) {
						string errorimage = t.Item2;
						int l = errorimage.Length;
						if (l < length) {
							mssg += "  *  " + errorimage + Constants.N;
						} else {
							mssg += "  *  ..." + errorimage.Substring (l - length) + Constants.N;
						}
					}						
				}

				ShowErrorDialog (mssg, string.Empty);
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
				List<ViewerImagePanel>pressedInVIPs = GetPressedInVIPs();

				TagsFlag t;
				switch (args.Event.Key) {
				// image tags
				case Gdk.Key.d:
					t = TagsFlag.DateTime;
					break;
				case Gdk.Key.f:
					t = TagsFlag.FocalLength;
					break;
				case Gdk.Key.k:
					t = TagsFlag.Keywords;
					break;
				// other tags
				case Gdk.Key.c:
					t = TagsFlag.Comment;
					break;
				case Gdk.Key.t:
					t = TagsFlag.Title;
					break;				
				case Gdk.Key.x:
					t = TagsFlag.Copyright;
					break;
				default:
					leftAltPressed = false;
					return;
				}

				if (pressedInVIPs.Count != 0) {
					if (t == TagsFlag.Keywords) {
						EnterKeywordsWindow ekw = new EnterKeywordsWindow (pressedInVIPs);
						//	pw.WindowPosition = WindowPosition.CenterAlways;
						ekw.Show ();	
					}
					else {
						EnterMetaDataWindow emw = new EnterMetaDataWindow (pressedInVIPs, t);
						// pw.WindowPosition = WindowPosition.CenterAlways;
						emw.Show ();
					}
				}

				leftAltPressed = false;

				return;
			}
			#endregion 'alt + ...'


			#region 'shift + ...'
			if (leftShiftPressed) {
				switch (args.Event.Key) {
				case Gdk.Key.Delete:
					OnToolbarBtn_RemoveAndDeleteFilePressed (null, null);
					break;
				}

				leftShiftPressed = false;

				return;
			}
			#endregion 'shift + ...'


			#region 'ctrl + ...'
			if (leftControlPressed) {
				switch (args.Event.Key) {
				case Gdk.Key.a:
					OnToolbarBtn_SelectAllPressed (null, null);
					break;
				case Gdk.Key.c:

					OkCancelDialog warn_c = new OkCancelDialog (false);
					warn_c.Title = Language.I.L [29];
					warn_c.Label1 = Language.I.L [170];
					warn_c.Label2 = Language.I.L [171];
					warn_c.OkButtontext = Language.I.L [16];
					warn_c.CancelButtontext = Language.I.L [17];	
					warn_c.Show ();

					warn_c.OnReleasedOkButton += AppendIdAndCompressionByRating;						
					break;
				case Gdk.Key.d:
					OkCancelDialog warn_d = new OkCancelDialog (false);
					warn_d.Title = Language.I.L [29];
					warn_d.Label1 = Language.I.L [200];
					warn_d.Label2 = Language.I.L [171];
					warn_d.OkButtontext = Language.I.L [16];
					warn_d.CancelButtontext = Language.I.L [17];	
					warn_d.Show ();

					warn_d.OnReleasedOkButton += SetDatetimeFromFilename;
					break;				
				case Gdk.Key.r:
					OkCancelDialog warn_r = new OkCancelDialog (false);
					warn_r.Title = Language.I.L [29];
					warn_r.Label1 = Language.I.L [175];
					warn_r.Label2 = Language.I.L [171];
					warn_r.OkButtontext = Language.I.L [16];
					warn_r.CancelButtontext = Language.I.L [17];	
					warn_r.Show ();

					warn_r.OnReleasedOkButton += RenameFileByDateTime;
					break;
				case Gdk.Key.v:

					OkCancelDialog warn_v = new OkCancelDialog (false);
					warn_v.Title = Language.I.L [29];
					warn_v.Label1 = Language.I.L [199];
					warn_v.Label2 = Language.I.L [171];
					warn_v.OkButtontext = Language.I.L [16];
					warn_v.CancelButtontext = Language.I.L [17];	
					warn_v.Show ();

					warn_v.OnReleasedOkButton += RenameVideoByTitleAndInsertIdentifier;						
					break;
//				default:
//					leftControlPressed = false;
//					return;
				}

				leftControlPressed = false;

				return;
			}
			#endregion 'ctrl + ...'

			switch (args.Event.Key) {
			case Gdk.Key.Control_L:
				leftControlPressed = true;
				break;
			case Gdk.Key.Shift_L:
				leftShiftPressed = true;
				break;
			case Gdk.Key.Alt_L:
				leftAltPressed = true;
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
			case Gdk.Key.Shift_L:
				leftShiftPressed = false;
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
			int shift;
			if (tb != null && int.TryParse (tb.Name, out shift)) {
//				Console.WriteLine ("Trooniebutton-Name: " + tb.Name);

				List<ViewerImagePanel>pressedInVIPs = GetPressedInVIPs();

				if (pressedInVIPs.Count != 0) {
//					EnterMetaDataWindow pw = new EnterMetaDataWindow (pressedInVIPs, (TagsFlag)(1 << shift));
//					pw.Show ();

					if ((TagsFlag)(1 << shift) == TagsFlag.Keywords) {
						EnterKeywordsWindow ekw = new EnterKeywordsWindow (pressedInVIPs);
						//	pw.WindowPosition = WindowPosition.CenterAlways;
						ekw.Show ();	
					}
					else {
						EnterMetaDataWindow emw = new EnterMetaDataWindow (pressedInVIPs, (TagsFlag)(1 << shift));
						// pw.WindowPosition = WindowPosition.CenterAlways;
						emw.Show ();
					}
				}
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

