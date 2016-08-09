using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using Cairo;
using Gtk;
using Image = System.Drawing.Image;
using ImageFormat = System.Drawing.Imaging.ImageFormat;
using IOPath = System.IO.Path;
using PixelFormat = System.Drawing.Imaging.PixelFormat;
using ImageConverter = Troonie_Lib.ImageConverter;
using Troonie;
using Troonie_Lib;
using System.Diagnostics;

namespace Troonie
{
	public partial class EditWidget : Gtk.Window
	{
		private struct shortcutFormatStruct
		{
			public int Width;
			public int Height;
			public string Name;
		}

		private const string blackFileName = "black.png";

		private shortcutFormatStruct[] shortcutFormats;
		private Troonie.ColorConverter colorConverter = Troonie.ColorConverter.Instance;
		private Constants constants = Constants.I;
		private int imageW; 
		private int imageH;
		private string tempScaledImageFileName;

		private bool repeatTimeout;
		private Slider timeoutSlider;
		private Gdk.Key timeoutKey;
		private int timeoutRotateValue;
		private Stopwatch timeoutSw;

		public string FileName { get; set; }
		public BitmapWithTag bt;
		private List<string> filterNames;

		public EditWidget (string pFilename) : base (Gtk.WindowType.Toplevel)
		{
			FileName = pFilename;

			Build ();
			this.SetIconFromFile(Constants.I.EXEPATH + Constants.ICONNAME);
			filterNames = new List<string> { "Filter", 
												Language.I.L[90], 
												Language.I.L[91], 
												Language.I.L[92], 
												Language.I.L[104],
												Language.I.L[108],
												Language.I.L[120],
												Language.I.L[123],
												Language.I.L[150]};

			GuiHelper.I.CreateToolbarIconButton (hboxToolbarButtons, 0, "folder-new-3.png", Language.I.L[2], OnToolbarBtn_OpenPressed);
			GuiHelper.I.CreateToolbarIconButton (hboxToolbarButtons, 1, "document-save-5.png", Language.I.L[3], OnToolbarBtn_SaveAsPressed);
			GuiHelper.I.CreateToolbarSeparator (hboxToolbarButtons, 2);
			GuiHelper.I.CreateToolbarIconButton (hboxToolbarButtons, 3, "view-split-left-right-2.png", Language.I.L[131], OnToolbarBtn_StitchPressed, "Stitch");
			GuiHelper.I.CreateMenubarInToolbar (hboxToolbarButtons, 4, "filter.png", Language.I.L[84],
			                                    OnToolbarBtn_ShaderFilterPressed, filterNames.ToArray());		
			GuiHelper.I.CreateToolbarSeparator (hboxToolbarButtons, 5);
			GuiHelper.I.CreateDesktopcontextmenuLanguageAndInfoToolbarButtons (hboxToolbarButtons, 6, OnToolbarBtn_LanguagePressed);

			timeoutSw = new Stopwatch();
			SetGuiColors ();
			SetLanguageToGui ();
			Initialize(true);

		if (constants.WINDOWS) {
			Gtk.Drag.DestSet (this, 0, null, 0);
		} else {
			// Original is ShadowType.EtchedIn, but linux cannot draw it correctly.
			// Otherwise ShadowType.In looks terrible at Win10.
			frameCutPoints.ShadowType = ShadowType.In;
			frameRotation.ShadowType = ShadowType.In;
			frameImageDimensions.ShadowType = ShadowType.In;
			frameCursorPos.ShadowType = ShadowType.In;
			frameShortcuts.ShadowType = ShadowType.In;

			Gtk.Drag.DestSet (this, DestDefaults.All, MainClass.Target_table, Gdk.DragAction.Copy);

			if (!Constants.I.CJPEG) {
					OkCancelDialog pseudo = new OkCancelDialog (true);
					pseudo.Title = Language.I.L [161];
					pseudo.Label1 = Language.I.L [162];
					pseudo.Label2 = Language.I.L [163] + Constants.N + Language.I.L [164];
					pseudo.OkButtontext = Language.I.L [16];
					pseudo.Show ();
			}
		}

			imagepanel1.OnCursorPosChanged += OnCursorPosChanged;

			if (Constants.I.CONFIG.AskForDesktopContextMenu) {
				new AskForDesktopContextMenuWindow (true, Constants.I.CONFIG).Show ();
			}
		}

		public override void Destroy ()
		{
			if (bt != null) {
				bt.Dispose ();
			}

//			imagepanel1.Dispose ();

			base.Destroy ();
		}

		private void LoadException()
		{
			FileName = null;
			Initialize (true);

			MessageDialog md = new MessageDialog (
				null, DialogFlags.Modal, 
				MessageType.Info, 
				ButtonsType.Ok, Language.I.L[51]);
			md.Run ();
			md.Destroy ();		
		}

		private void Initialize(bool newFileName)
		{
			if (FileName == null)
			{
				FileName = constants.EXEPATH + blackFileName;
				Title = FileName;
				bt = new BitmapWithTag (FileName, false);
				imageW = bt.Bitmap.Width;
				imageH = bt.Bitmap.Height;
				bt.Bitmap.Save(FileName, ImageFormat.Png);
			}
			else
			{          
				if (!newFileName) {
					imageW = bt.Bitmap.Width;
					imageH = bt.Bitmap.Height;
				} 
				else 
				{
					try 
					{
						FileInfo info = new FileInfo (FileName);
						string ext = info.Extension.ToLower ();

						switch (ext) {
						case ".wmf":
						case ".tiff":
						case ".tif":
						case ".gif":
						case ".emf":
						case ".png":
						case ".bmp":
						case ".jpeg":
						case ".jpg":
						case ".ico":
							Title = FileName;
							bt = new BitmapWithTag(FileName, true);
							imageW = bt.Bitmap.Width;
							imageH = bt.Bitmap.Height;
							break;
						default:
							LoadException ();
							return;
						} // switch end
					} // try end
					catch (ArgumentException) {
						LoadException ();
						return;
					}
				} // else end
			}			
			
			// Gdk.Pixbuf.GetFileInfo(FileName, out imageW, out imageH);

			SetPanelSize();	

			tempScaledImageFileName = constants.EXEPATH + "tempScaledImageFileName.png";

			imagepanel1.SurfaceFileName = tempScaledImageFileName;

			if (newFileName) 
			{
				Bitmap pic = new Bitmap(FileName);                              
				Bitmap croppedPic;

				ImageConverter.ScaleAndCut (
					pic, 
					out croppedPic, 
					0,
					0,
					imagepanel1.WidthRequest,
					imagepanel1.HeightRequest,
					ConvertMode.StretchForge,
					false);

				pic.Dispose ();
				croppedPic.Save(tempScaledImageFileName, ImageFormat.Png);
				croppedPic.Dispose();

			} 
			else 
			{
				Bitmap b2;

				ImageConverter.ScaleAndCut (
					bt.Bitmap, 
					out b2,
					0 /*xStart*/, 
					0 /*yStart*/,
					imagepanel1.WidthRequest,
					imagepanel1.HeightRequest,
					ConvertMode.StretchForge,
					false);

				b2.Save (tempScaledImageFileName, ImageFormat.Png);
				b2.Dispose ();
			}

			imagepanel1.Initialize();
			imagepanel1.LeftSlider.OnSliderChangedValue += OnSliderChangedValue;
			imagepanel1.RightSlider.OnSliderChangedValue += OnSliderChangedValue;
			imagepanel1.TopSlider.OnSliderChangedValue += OnSliderChangedValue;
			imagepanel1.BottomSlider.OnSliderChangedValue += OnSliderChangedValue;

//			if (bt.Bitmap.PixelFormat == PixelFormat.Format24bppRgb ||
//				bt.Bitmap.PixelFormat == PixelFormat.Format8bppIndexed) {
//				// entryRotate
//				frameRotation.Sensitive = true;
//			} else {
//				frameRotation.Sensitive = false;
//				frameRotation.TooltipText = Language.I.L[72];
//			}

			ShowAll();
		}		

		private void SetPanelSize()
		{		
			const int optionsWidth = 390;
			// general taskbar size in win_8.1
			const int taskbarHeight = 90;
			const int paddingOffset = 44;
			// necessary to correct to small height 
			const float multiplicatorHeight = 1.2f;
	
			Gdk.Screen screen = this.Screen;
			int monitor = screen.GetMonitorAtWindow (this.GdkWindow); 
			Gdk.Rectangle bounds = screen.GetMonitorGeometry (monitor);
			int winW = bounds.Width;
			int winH = bounds.Height - taskbarHeight;

			int panelW = winW - optionsWidth - paddingOffset;
			int panelH = winH - (int)(paddingOffset * multiplicatorHeight);
			// setting padding for left and right side
			global::Gtk.Box.BoxChild w4 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.imagepanel1]));
			w4.Padding = ((uint)(paddingOffset / 4.0f + 0.5f));

			if (panelW < imageW || panelH < imageH)
			{
				bool wLonger = (imageW / (float)imageH) > (panelW / (float)panelH);
				if (wLonger)
				{
					panelH = (int)(imageH * panelW / (float)imageW  + 0.5f);
					winH = panelH + (int)(paddingOffset * multiplicatorHeight);
				}
				else
				{
					panelW = (int)(imageW * panelH / (float)imageH  + 0.5f);
					winW = panelW + optionsWidth + paddingOffset;
				}
			}
			else
			{
				panelW = imageW;
				panelH = imageH;
				winW = panelW + optionsWidth + paddingOffset;
				winH = panelH + (int)(paddingOffset * multiplicatorHeight);
			}						
			
			imagepanel1.WidthRequest = panelW;
			imagepanel1.HeightRequest = panelH;

			imagepanel1.ScaleCursorX = imageW / (float)panelW;
			imagepanel1.ScaleCursorY = imageH / (float)panelH;

//			this.Resize (winW, winH);
//			this.Move (0, 0);

			winH = Math.Max (490, winH);
//			WidthRequest = winW;
//			HeightRequest = winH;
			// work around by GLib timeout to fix the GTK#-resizing bug
			GLib.TimeoutHandler timeoutHandler = () => {
				WidthRequest = winW;
				HeightRequest = winH;
				Move (0, 0);
				Resize (winW, winH);
				// false, because usage only one time
				return false;
			};
			GLib.Timeout.Add(200, timeoutHandler);

			lbOriginal.Text = imageW + " x " + imageH;
			lbNew.Text = lbOriginal.Text;
			entryLeft.Text = 0.ToString();
			entryRight.Text = imageW.ToString();
			entryTop.Text = 0.ToString();
			entryBottom.Text = imageH.ToString();
			entryRotate.Text = 0.ToString();
		}

		private void SetGuiColors()
		{
			this.ModifyBg(StateType.Normal, colorConverter.GRID);
			eventboxToolbar.ModifyBg(StateType.Normal, colorConverter.GRID);
			entryLeft.ModifyBase(StateType.Normal, colorConverter.White);
			entryRight.ModifyBase(StateType.Normal, colorConverter.White);
			entryTop.ModifyBase(StateType.Normal, colorConverter.White);
			entryBottom.ModifyBase(StateType.Normal, colorConverter.White);
			entryRotate.ModifyBase(StateType.Normal, colorConverter.White);

			lbFrameCutDimensions.ModifyFg (StateType.Normal, colorConverter.FONT);
			lbLeftText.ModifyFg (StateType.Normal, colorConverter.FONT);
			lbRightText.ModifyFg (StateType.Normal, colorConverter.FONT);
			lbTopText.ModifyFg (StateType.Normal, colorConverter.FONT);
			lbBottomText.ModifyFg (StateType.Normal, colorConverter.FONT);

			lbFrameRotation.ModifyFg (StateType.Normal, colorConverter.FONT);
			lbRotateText.ModifyFg (StateType.Normal, colorConverter.FONT);
			lbFrameShortcuts.ModifyFg (StateType.Normal, colorConverter.FONT);
			lbShortcutsText.ModifyFg (StateType.Normal, colorConverter.FONT);

			lbFrameImageDimensions.ModifyFg (StateType.Normal, colorConverter.FONT);
			lbOriginal.ModifyFg (StateType.Normal, colorConverter.FONT);
			lbOriginalText.ModifyFg (StateType.Normal, colorConverter.FONT);
			lbNew.ModifyFg (StateType.Normal, colorConverter.FONT);
			lbNewText.ModifyFg (StateType.Normal, colorConverter.FONT);

			lbFrameCursorPos.ModifyFg (StateType.Normal, colorConverter.FONT);
			lbCursorPos.ModifyFg (StateType.Normal, colorConverter.FONT);
		}

		private void SetLanguageToGui()
		{
//			hboxToolbarButtons.Children[0].TooltipText = Language.I.L[2];
//			hboxToolbarButtons.Children[1].TooltipText = Language.I.L[3];
//			hboxToolbarButtons.Children[2].TooltipText = Language.I.L[4];
//			hboxToolbarButtons.Children[4].TooltipText = 
//				Language.I.L[43] +	": " + 
//				Language.I.L[0] + "\n\n" + 
//				Language.I.L[44] +	": \n" +
//				Language.AllLanguagesAsString;
//			hboxToolbarButtons.Children[5].TooltipText = Language.I.L[131];
////			hboxToolbarButtons.Children[6].TooltipText = Language.I.L[84];
//			hboxToolbarButtons.Children[7].TooltipText = Language.I.L[84];

			lbFrameShortcuts.LabelProp = "<b>" + Language.I.L[127] + "</b>";
			lbShortcutsText.Text = Language.I.L[128];

			lbFrameCutDimensions.LabelProp = "<b>" + Language.I.L[5] + "</b>";
			lbLeftText.Text = Language.I.L[6];
			lbRightText.Text = Language.I.L[7];
			lbTopText.Text = Language.I.L[8];
			lbBottomText.Text = Language.I.L[9];

			lbFrameRotation.LabelProp = "<b>" + Language.I.L[10] + "</b>";
			lbRotateText.Text = Language.I.L[11];

			lbFrameImageDimensions.LabelProp = "<b>" + Language.I.L[12] + "</b>";
			lbOriginalText.Text = Language.I.L[13] + ":";
			lbNewText.Text = Language.I.L[14] + ":";

			lbFrameCursorPos.LabelProp = "<b>" + Language.I.L[15] + "</b>";
			btnOk.Text = Language.I.L[16];
			btnOk.Redraw ();

			//lbLeftText.Text = Language.I.L[0];

			//		private static Size[] shortcutFormats = {
			//			Size.Empty,
			//			new Size(10, 15),
			//			new Size(15, 10),
			//			new Size(3, 4),
			//			new Size(4, 3),
			//			new Size(16, 10),
			//			new Size(10, 16)
			//		};


			shortcutFormats = new shortcutFormatStruct[]{
				new shortcutFormatStruct { Width = 0, Height = 0, Name = "Empty" },

				new shortcutFormatStruct { Width = 13, Height = 9 },
				new shortcutFormatStruct { Width = 9, Height = 13 },

				new shortcutFormatStruct { Width = 15, Height = 10 },
				new shortcutFormatStruct { Width = 10, Height = 15 },

				new shortcutFormatStruct { Width = 18, Height = 13 },
				new shortcutFormatStruct { Width = 13, Height = 18 },

				new shortcutFormatStruct { Width = 30, Height = 20 },
				new shortcutFormatStruct { Width = 20, Height = 30 },

				new shortcutFormatStruct { Width = 4, Height = 3 },
				new shortcutFormatStruct { Width = 3, Height = 4 },

				new shortcutFormatStruct { Width = 16, Height = 10 },
				new shortcutFormatStruct { Width = 10, Height = 16 },

				new shortcutFormatStruct { Width = 297, Height = 210, Name = "A4 " + Language.I.L[129] /*landscape*/ },
				new shortcutFormatStruct { Width = 210, Height = 297, Name = "A4 " + Language.I.L[130] /*portrait*/ }
			};

			this.comboboxShortcuts.Active = 0;

			for (int i = 1; i < shortcutFormats.Length; i++) {
				string s = shortcutFormats [i].Width + " x " + shortcutFormats [i].Height;
				if (shortcutFormats [i].Name != null) {
					s = shortcutFormats [i].Name + " ( " + s + " )"; 
				}

				comboboxShortcuts.RemoveText (i);
				comboboxShortcuts.InsertText (i, s);
//				comboboxShortcuts.AppendText (s);
			}
		}

		private void Rotate()
		{
			int number = 0;
			int.TryParse (entryRotate.Text, out number);
			entryRotate.Text = (number + timeoutRotateValue).ToString();
			OnEntryRotateKeyReleaseEvent (null, null);
		}

		private bool RotateByTimeoutHandler()
		{
			if (timeoutSw.ElapsedMilliseconds < Constants.TIMEOUT_INTERVAL_FIRST) {
				return repeatTimeout;
			}
			Rotate();
			return repeatTimeout;
		}

		private void MoveTimeoutSlider()
		{
			timeoutSlider.IsEntered = true;
			timeoutSlider.Partner.IsEntered = false;
			imagepanel1.MoveSliderByKey (timeoutKey, 1);
		}

		private bool MoveSliderByTimeoutHandler()
		{
			if (timeoutSw.ElapsedMilliseconds < Constants.TIMEOUT_INTERVAL_FIRST) {
				return repeatTimeout;
			}

			MoveTimeoutSlider();
			return repeatTimeout;
		}

		private void OnSliderChangedValue(Troonie.Slider.Types t, float v)
		{
			int vScaledX = (int)(v * imagepanel1.ScaleCursorX + 0.5f);
			int vScaledY = (int)(v * imagepanel1.ScaleCursorY + 0.5f);
			switch (t) {
			case Troonie.Slider.Types.Left:
				entryLeft.Text = vScaledX.ToString ();
				break;
			case Troonie.Slider.Types.Right:
				entryRight.Text = vScaledX.ToString ();
				break;
			case Troonie.Slider.Types.Top:
				entryTop.Text = vScaledY.ToString ();
				break;
			case Troonie.Slider.Types.Bottom:
				entryBottom.Text = vScaledY.ToString ();
				break;
			}

			double DistXToPartnerScaled = Math.Round(imagepanel1.LeftSlider.DistXToPartner * imagepanel1.ScaleCursorX);
			double DistYToPartnerScaled = Math.Round(imagepanel1.TopSlider.DistYToPartner * imagepanel1.ScaleCursorY);
			lbNew.Text = DistXToPartnerScaled + " x " + DistYToPartnerScaled;
		}

		private void OnCursorPosChanged(int x, int y)
		{
			lbCursorPos.Text = 	x.ToString() + " x " +	y.ToString();
		}

		protected void OnDeleteEvent (object sender, DeleteEventArgs a)
		{
//			if (bt != null) {
//				bt.Dispose ();
//			}
			this.DestroyAll ();
//			imagepanel1.Dispose ();

			Application.Quit ();
			a.RetVal = true;

			File.Delete (tempScaledImageFileName);
			File.Delete (Constants.I.EXEPATH + blackFileName);
		}


		protected void OnExit (object sender, EventArgs e)
		{	
			OnDeleteEvent (sender, e as DeleteEventArgs);
		}

		protected void OnDragDrop (object sender, Gtk.DragDropArgs args)
		{
			Gtk.Drag.GetData
			((Gtk.Widget)sender, args.Context,
				args.Context.Targets[0], args.Time);
		}

		void OnDragDataReceived (object sender, Gtk.DragDataReceivedArgs args)
		{
			if (args.SelectionData.Length > 0
				&& args.SelectionData.Format == 8) {

				byte[] data = args.SelectionData.Data;
				string encoded = System.Text.Encoding.UTF8.GetString (data);
				// drag n drop at linux wont accept spaces, so it has to be replaced
				encoded = encoded.Replace ("%20", " ");

				List<string> paths
				= new List<string> (encoded.Split ('\r', '\n'));
				paths.RemoveAll (string.IsNullOrEmpty);

				// I don't know what last object (when Windows) is,
				//  but I tested and noticed that it is not a path
				if (constants.WINDOWS)
					paths.RemoveAt (paths.Count-1);

				for (int i=0; i<paths.Count; ++i)
				{
					string waste = constants.WINDOWS ? "file:///" : "file://";
					paths [i] = paths [i].Replace (@waste, "");
					// Console.WriteLine (paths[i]);
					FileName = paths [i];
				}

				Initialize(true);				
			}
		}

		protected void OnComboboxShortcutsChanged (object sender, EventArgs e)
		{
			if (comboboxShortcuts.Active == 0) {

				entryLeft.Text = 0.ToString();
				OnEntryLeftKeyReleaseEvent (entryLeft, null);

				entryRight.Text = imageW.ToString ();
				OnEntryRightKeyReleaseEvent (entryRight, null);

				entryTop.Text = 0.ToString ();
				OnEntryTopKeyReleaseEvent (entryTop, null);

				entryBottom.Text = imageH.ToString ();
				OnEntryBottomKeyReleaseEvent (entryBottom, null);

				return;
			}

			float ratioShortcut = (float)shortcutFormats [comboboxShortcuts.Active].Width / 
								 shortcutFormats [comboboxShortcuts.Active].Height;

//			Console.WriteLine ("ratioShortcut=" + ratioShortcut);

			float ratioImage = (float)imageW / imageH;

			if (ratioShortcut > ratioImage) {
			
				int newHeight = (int)Math.Round(imageW / ratioShortcut); 
				int diff = imageH - newHeight;

				int diffTop = (int)Math.Round (diff / 2.0);
				int diffBottom = diff - diffTop;

				entryTop.Text = diffTop.ToString ();
				OnEntryTopKeyReleaseEvent (entryTop, null);

				entryBottom.Text = (imageH - diffBottom).ToString ();
				OnEntryBottomKeyReleaseEvent (entryBottom, null);

				entryLeft.Text = 0.ToString();
				OnEntryLeftKeyReleaseEvent (entryLeft, null);

				entryRight.Text = imageW.ToString ();
				OnEntryRightKeyReleaseEvent (entryRight, null);

			} else {
				int newWidth = (int)Math.Round(imageH * ratioShortcut); 
				int diff = imageW - newWidth;

				int diffLeft = (int)Math.Round (diff / 2.0);
				int diffRight = diff - diffLeft;

				entryLeft.Text = diffLeft.ToString();
				OnEntryLeftKeyReleaseEvent (entryLeft, null);

				entryRight.Text = (imageW - diffRight).ToString ();
				OnEntryRightKeyReleaseEvent (entryRight, null);

				entryTop.Text = 0.ToString ();
				OnEntryTopKeyReleaseEvent (entryTop, null);

				entryBottom.Text = imageH.ToString ();
				OnEntryBottomKeyReleaseEvent (entryBottom, null);
			}		
		}


		[GLib.ConnectBefore ()] 
		protected void OnKeyPressEvent (object o, KeyPressEventArgs args)
		{
//			System.Console.WriteLine("Keypress: {0}  -->  State: {1}", args.Event.Key, args.Event.State); 

			if (args.Event.State == (Gdk.ModifierType.ControlMask /* | Gdk.ModifierType.Mod2Mask*/ )) {
				switch (args.Event.Key) {
					case Gdk.Key.l:
					entryLeft.Text = Constants.I.CONFIG.eLeft.ToString ();
						OnEntryLeftKeyReleaseEvent (entryLeft, null);

						entryRight.Text = Constants.I.CONFIG.eRight.ToString ();
						OnEntryRightKeyReleaseEvent (entryRight, null);

						entryTop.Text = Constants.I.CONFIG.eTop.ToString ();
						OnEntryTopKeyReleaseEvent (entryTop, null);

						entryBottom.Text = Constants.I.CONFIG.eBottom.ToString ();
						OnEntryBottomKeyReleaseEvent (entryBottom, null);
						break;
					case Gdk.Key.r:
						entryRotate.Text = Constants.I.CONFIG.eRotation.ToString ();
						OnEntryRotateKeyReleaseEvent (entryRotate, null);
						break;
				case Gdk.Key.k:
					SaveConfigFromGui ();
					OkCancelDialog pseudo = new OkCancelDialog (true);
					pseudo.Title = Language.I.L [135];
					pseudo.Label1 = Language.I.L [136];
					pseudo.Label2 = Language.I.L [137];
					pseudo.OkButtontext = Language.I.L [16];
//					pseudo.CancelButtontext = Language.I.L [17];
					break;
				case Gdk.Key.s:
					OpenSaveAsDialog ();
					break;
					default:
						break;
				}
			} 

			imagepanel1.MoveSliderByKey (args.Event.Key, 1);
		}	

		private void OpenSaveAsDialog()
		{
			SaveAsDialog dialog = new SaveAsDialog(bt, ConvertMode.Editor);
			bool runDialog = true;

			do
			{
				if (dialog.Run () == (int)ResponseType.Ok) {
					if (dialog.Process ()) {
						FileName = dialog.SavedFileName;
						bt.Dispose ();
						Initialize (true);
						runDialog = false;
					}
				}
				else {
					runDialog = false;
				}
			}
			while (runDialog);

			dialog.Destroy();
		}

		private void FilterEvent(Bitmap filterBitmap)
		{
//			Console.WriteLine ("FilterEvent");

			bt.Bitmap.Dispose ();
			bt.ChangeBitmapButNotTags(filterBitmap);

			Initialize (false);
		}

		private void SaveConfigFromGui()
		{
			Constants.I.CONFIG.eLeft = int.Parse (entryLeft.Text);
			Constants.I.CONFIG.eRight = int.Parse (entryRight.Text);
			Constants.I.CONFIG.eTop =  int.Parse (entryTop.Text);
			Constants.I.CONFIG.eBottom =  int.Parse (entryBottom.Text);
			Constants.I.CONFIG.eRotation =  int.Parse (entryRotate.Text);

			Config.Save (Constants.I.CONFIG);			
		}
	}
}

