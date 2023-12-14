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
using System.Linq;

namespace Troonie
{
	public partial class EditWidget : Gtk.Window
	{
		private struct filterN
		{
			public static string OVERVIEW = "Filter"; 
			public static string Invert = Language.I.L [90]; 
			public static string Grayscale = Language.I.L[91]; 
			public static string RGB_Channels = Language.I.L[92]; 
			public static string Gaussian_blur = Language.I.L[104];
			public static string Canny_edge_detector = Language.I.L[108];
			public static string Sepia = Language.I.L[120];
			public static string Oil_painting = Language.I.L[123];
			public static string Difference = Language.I.L[150];
			public static string Posterization = Language.I.L[168];
			public static string Cartoon = Language.I.L[268];
			public static string Sobel_edge_detector = Language.I.L[272];
			public static string Unsharp_masking = Language.I.L[275];
			public static string Sobel_edge_marker = Language.I.L[277];
			public static string Binarization = Language.I.L[285];
			public static string Meanshift = Language.I.L[290];
			public static string Edge_Point_dilatation = Language.I.L[292];
			public static string Exponentiate_channels = Language.I.L[293];
			public static string Convolution5x5 = Language.I.L [295];	
			public static string Blend = Language.I.L [305];
			public static string Mosaic = Language.I.L [307];
			public static string Mirror = Language.I.L [308];
			public static string Chessboard = Language.I.L [313];
            public static string Contrast = Language.I.L[330];
            public static string Hsl = Language.I.L[331];
            public static string RotateQuarterTurns = Language.I.L[348];
        }

		private struct shortcutFormatStruct
		{
			public int Width;
			public int Height;
			public string Name;
		}
			
		private shortcutFormatStruct[] shortcutFormats;
		private Troonie.ColorConverter colorConverter = Troonie.ColorConverter.Instance;
		private Constants constants = Constants.I;
		private int imageW; 
		private int imageH;
//		private string tempScaledImageFileName;
		private bool leftControlPressed;
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
			try {
				
				FileName = pFilename;

				Build ();
                Title = Language.I.L[340];
                this.SetIconFromFile(Constants.I.EXEPATH + Constants.ICONNAME);
				filterNames = new List<string> { filterN.OVERVIEW,
					filterN.RotateQuarterTurns,
					filterN.Binarization,
					filterN.Blend,
					filterN.Canny_edge_detector,
					filterN.Cartoon,
					filterN.Difference,
					filterN.Edge_Point_dilatation,
					filterN.Exponentiate_channels,
					filterN.Convolution5x5,
					filterN.Invert, 
					filterN.Gaussian_blur,
					filterN.Grayscale,
                    filterN.Hsl,
                    filterN.Contrast, 
					filterN.Meanshift,
					filterN.Mosaic,
					filterN.Oil_painting,
					filterN.Posterization,
					filterN.RGB_Channels,
					filterN.Chessboard,
					filterN.Sepia,
					filterN.Sobel_edge_detector,
					filterN.Sobel_edge_marker,
					filterN.Mirror,
					filterN.Unsharp_masking,
						};

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
                if (!GuiHelper.I.CheckForJpegAndExiftool()) { 
                    FileName = null;
					return;
                }
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
				}

				imagepanel1.OnCursorPosChanged += OnCursorPosChanged;

				if (Constants.I.CONFIG.AskForDesktopContextMenu) {
					new AskForDesktopContextMenuWindow (true, Constants.I.CONFIG).Show ();
				}

			}
			catch (Exception) {

				OkCancelDialog win = new OkCancelDialog (true);
				win.WindowPosition = WindowPosition.CenterAlways;
				win.Title = Language.I.L [153];
				win.Label1 = Language.I.L [194];
				win.Label2 = Language.I.L [195];
				win.OkButtontext = Language.I.L [16];
				DeleteEventArgs args = new DeleteEventArgs ();
				win.OnReleasedOkButton += () => { OnDeleteEvent(win, args); };
				win.Show ();

				this.DestroyAll ();
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
			lbFileSize.Text = string.Empty;
			if (FileName == null)
			{
				hboxToolbarButtons.Children[1].Sensitive = false;
				hboxToolbarButtons.Children[3].Sensitive = false;
				hboxToolbarButtons.Children[4].Sensitive = false;
				newFileName = false;
//				FileName = constants.EXEPATH + Constants.BLACKFILENAME;
				Title = FileName;
				bt = new BitmapWithTag (FileName);
				imageW = bt.Bitmap.Width;
				imageH = bt.Bitmap.Height;
//				bt.Bitmap.Save(FileName, ImageFormat.Png);
			}
			else
			{    		
				hboxToolbarButtons.Children[1].Sensitive = true;
				hboxToolbarButtons.Children[3].Sensitive = true;
				hboxToolbarButtons.Children[4].Sensitive = true;

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
						lbFileSize.Text = GuiHelper.I.GetFileSizeString(info.Length, 2);

						if (ext.Length != 0 && Constants.Extensions.Any(x => x.Value.Item1 == ext || x.Value.Item2 == ext)) {
							Title = FileName;
                            // catch, whether image loading (e.g. cjepg/djepg) does not work
                            try
                            {
                                bt = new BitmapWithTag(FileName);
                            }
                            catch (Exception)
                            {
                                LoadException();
                                return;
                            }
                            imageW = bt.Bitmap.Width;
							imageH = bt.Bitmap.Height;
						}
						else{
							LoadException ();
                            return;
						}
					} // try end
					catch (Exception) {
						LoadException ();
						return;
					}
				} // else end
			}			
			
			// Gdk.Pixbuf.GetFileInfo(FileName, out imageW, out imageH);
			SetPanelSize();	

//			tempScaledImageFileName = constants.EXEPATH + "tempScaledImageFileName.png";
//			imagepanel1.SurfaceFileName = tempScaledImageFileName;


			if (newFileName) 
			{
				Bitmap pic = TroonieBitmap.FromFile (FileName);                            
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
//				croppedPic.Save(tempScaledImageFileName, ImageFormat.Png);
				croppedPic.Save(imagepanel1.MemoryStream, ImageFormat.Png);
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

//				b2.Save (tempScaledImageFileName, ImageFormat.Png);
				b2.Save(imagepanel1.MemoryStream, ImageFormat.Png);
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
	
			Gdk.Rectangle r = Screen.GetMonitorGeometry(Screen.GetMonitorAtWindow(this.GdkWindow));
			int winW = r.Width;
			int winH = r.Height - taskbarHeight;

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
				Move (0 + r.X, 0 + r.Y);
				Resize (winW, winH);
				// false, because usage only one time
				return false;
			};
			GLib.Timeout.Add(200, timeoutHandler);

			lbOriginal.Text = imageW + " x " + imageH;
			lbNew.Text = lbOriginal.Text;
			lbFormat.Text = Image.GetPixelFormatSize(bt.Bitmap.PixelFormat) + " " + Language.I.L[267];

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
			entryLeft.ModifyBg(StateType.Normal, colorConverter.White);
			entryRight.ModifyBg(StateType.Normal, colorConverter.White);
			entryTop.ModifyBg(StateType.Normal, colorConverter.White);
			entryBottom.ModifyBg(StateType.Normal, colorConverter.White);
			entryRotate.ModifyBg(StateType.Normal, colorConverter.White);

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
			lbFormat.ModifyFg (StateType.Normal, colorConverter.FONT);
			lbFormatText.ModifyFg (StateType.Normal, colorConverter.FONT);
			lbFileSize.ModifyFg (StateType.Normal, colorConverter.FONT);
			lbFileSizeText.ModifyFg (StateType.Normal, colorConverter.FONT);

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
			lbFormatText.Text = "\t" + Language.I.L[266] + ":";
			lbFileSizeText.Text = "\t" + Language.I.L[303] + ":";

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
				new shortcutFormatStruct { Width = 0, Height = 0, Name = " - " },

				new shortcutFormatStruct { Width = 13, Height = 9, Name = 13 + " x " + 9 },
				new shortcutFormatStruct { Width = 9, Height = 13, Name = 9 + " x " + 13  },

				new shortcutFormatStruct { Width = 15, Height = 10, Name = 15 + " x " + 10  },
				new shortcutFormatStruct { Width = 10, Height = 15, Name = 10 + " x " + 15 },

				new shortcutFormatStruct { Width = 18, Height = 13, Name = 18 + " x " + 13  },
				new shortcutFormatStruct { Width = 13, Height = 18, Name = 13 + " x " + 18  },

				new shortcutFormatStruct { Width = 30, Height = 20, Name = 30 + " x " + 20  },
				new shortcutFormatStruct { Width = 20, Height = 30, Name = 20 + " x " + 30  },

				new shortcutFormatStruct { Width = 4, Height = 3, Name = 4 + " x " + 3  },
				new shortcutFormatStruct { Width = 3, Height = 4, Name = 3 + " x " + 4  },

				new shortcutFormatStruct { Width = 16, Height = 10, Name = 16 + " x " + 10  },
				new shortcutFormatStruct { Width = 10, Height = 16, Name = 10 + " x " + 16  },

				new shortcutFormatStruct { Width = 297, Height = 210, Name = "A4 " + Language.I.L[129] /*landscape*/ + " ( " + 297 + " x " + 210 + " )" },
				new shortcutFormatStruct { Width = 210, Height = 297, Name = "A4 " + Language.I.L[130] /*portrait*/ + " ( " + 210 + " x " + 297 + " )" }
            };

			List<string> entries = new List<string>();
            for (int i = 0; i < shortcutFormats.Length; i++)
            {
				entries.Add(shortcutFormats[i].Name);
                //string s = shortcutFormats[i].Width + " x " + shortcutFormats[i].Height;
                //if (shortcutFormats[i].Name != null)
                //{
                //    s = shortcutFormats[i].Name + " ( " + s + " )";
                //}

                //comboboxShortcuts.Children[i].Name = s; // RemoveText (i);
                //                                        //comboboxShortcuts.InsertText (i, s);

            }

            comboboxShortcuts = new ComboBox(entries.ToArray());
			comboboxShortcuts.Active = 0;
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

//			try {
//				File.Delete (tempScaledImageFileName);
//				File.Delete (Constants.I.EXEPATH + Constants.BLACKFILENAME);
//			}
//			catch (Exception) {				
//				Console.WriteLine(Constants.ERROR_DELETE_TEMP_FILES);;
//			}

			Application.Quit ();
			a.RetVal = true;


		}


		protected void OnExit (object sender, EventArgs e)
		{	
			OnDeleteEvent (sender, e as DeleteEventArgs);
		}

		protected void OnDragDrop (object sender, Gtk.DragDropArgs args)
		{
			Gdk.Atom a = args.Context.ListTargets()[0];

            Gtk.Drag.GetData
			((Gtk.Widget)sender, args.Context,
				a, args.Time);
		}

		void OnDragDataReceived (object sender, Gtk.DragDataReceivedArgs args)
		{
            List<string> paths = GuiHelper.I.CorrectUmlautsOfDragData(constants.WINDOWS, sender, args);
            if (paths == null || paths.Count == 0)
                return;

            FileName = paths[paths.Count - 1];
            Initialize(true);						
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
//			if (args.Event.State == (Gdk.ModifierType.ControlMask /* | Gdk.ModifierType.Mod2Mask*/ )) {
			#region 'ctrl + ...'
			if (leftControlPressed) {
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
					// need to do here, because second GUI is opened and suppressed 'OnKeyReleaseEvent'
					leftControlPressed = false;
					break;
				case Gdk.Key.s:
					if (FileName != null) {
						OpenSaveAsDialog ();
						// need to do here, because second GUI is opened and suppressed 'OnKeyReleaseEvent'
						leftControlPressed = false;
					}
					break;
				case Gdk.Key.n:
					OnToolbarBtn_OpenPressed(null, null);
						// need to do here, because second GUI is opened and suppressed 'OnKeyReleaseEvent'
						leftControlPressed = false;
					break;
				default:
					break;
				}
			}
			#endregion 'ctrl + ...

			switch (args.Event.Key) {
			case Gdk.Key.Control_L:
				leftControlPressed = true;
				break;
			}

			imagepanel1.MoveSliderByKey (args.Event.Key, 1);
		}	

		[GLib.ConnectBefore ()] 
		protected void OnKeyReleaseEvent (object o, KeyReleaseEventArgs args)
		{
			switch (args.Event.Key) {
			case Gdk.Key.Control_L:
				leftControlPressed = false;
				break;
			}

			// args.RetVal = true;
		}

		private void OpenSaveAsDialog()
		{
			SaveAsDialog dialog = new SaveAsDialog(bt, ConvertMode.Editor);
			bool runDialog = true;

			do
			{
				if (dialog.Run () == (int)ResponseType.Ok) {
					int success = dialog.Process ();
					if (success == 1) {
						FileName = dialog.SavedFileName;
						bt.Dispose ();
						Initialize (true);
						runDialog = false;
					}
					else if (success == 0) {
						MessageDialog md = new MessageDialog (dialog, 
							DialogFlags.DestroyWithParent, MessageType.Error, 
							ButtonsType.Ok, Language.I.L[176] + Constants.N + Language.I.L[177]);
						md.Run();
						md.Destroy ();

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

