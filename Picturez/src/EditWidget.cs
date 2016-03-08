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
using ImageConverter = Picturez_Lib.ImageConverter;
using Picturez;
using Picturez_Lib;
using System.Diagnostics;

namespace Picturez
{
	public partial class EditWidget : Gtk.Window
	{
		private const string blackFileName = "black.png";
		private const int timeoutInterval = 20;
		private const int timeoutIntervalFirst = 500;

		private Picturez.ColorConverter colorConverter = Picturez.ColorConverter.Instance;
		private Constants constants = Constants.I;
		private int imageW; 
		private int imageH;
		private string tempScaledImageFileName;

		private ConfigEdit config;
		private bool repeatTimeout;
		private Slider timeoutSlider;
		private Gdk.Key timeoutKey;
		private int timeoutRotateValue;
		private Stopwatch timeoutSw;

		public string FileName { get; set; }
		public BitmapWithTag bt;
		private List<string> filterNames;

		public EditWidget (string pFilename = null) : base (Gtk.WindowType.Toplevel)
		{
			FileName = pFilename;

			Build ();
			this.SetIconFromFile(Constants.I.EXEPATH + Constants.ICONNAME);
			filterNames = new List<string> { "Shader-based Filter", Language.I.L[90], Language.I.L[91], Language.I.L[92]};

			GuiHelper.I.CreateToolbarIconButton (hboxToolbarButtons, 0, "folder-new-3.png", OnToolbarBtn_OpenPressed);
			GuiHelper.I.CreateToolbarIconButton (hboxToolbarButtons, 1, "document-save-5.png", OnToolbarBtn_SaveAsPressed);
			GuiHelper.I.CreateToolbarIconButton (hboxToolbarButtons, 2, "help-about-3.png", OnToolbarBtn_AboutPressed);
			GuiHelper.I.CreateToolbarSeparator (hboxToolbarButtons, 3);
			GuiHelper.I.CreateToolbarIconButton (hboxToolbarButtons, 4, "tools-check-spelling-5.png", OnToolbarBtn_LanguagePressed);
			GuiHelper.I.CreateToolbarSeparator (hboxToolbarButtons, 5);
			GuiHelper.I.CreateMenubarInToolbar (hboxToolbarButtons, 6, "help-about-3.png", 
			                                    OnToolbarBtn_ShaderFilterPressed, filterNames.ToArray());

			timeoutSw = new Stopwatch();
			config = ConfigEdit.Load ();
			SetGuiColors ();
			SetLanguageToGui ();
			Initialize(true);

			if (constants.WINDOWS)
			 	Gtk.Drag.DestSet (this, 0, null, 0);
			else
				Gtk.Drag.DestSet (this, DestDefaults.All, MainClass.Target_table, Gdk.DragAction.Copy);

			imagepanel1.OnCursorPosChanged += OnCursorPosChanged;

			// TODO: Image filter: Remove, when implemented
//			hboxToolbarButtons.Children [6].Visible = false;
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

			if (bt.Bitmap.PixelFormat == PixelFormat.Format24bppRgb ||
				bt.Bitmap.PixelFormat == PixelFormat.Format8bppIndexed) {
				// entryRotate
				frameRotation.Sensitive = true;
			} else {
				frameRotation.Sensitive = false;
				frameRotation.TooltipText = Language.I.L[72];
			}

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

			this.Resize (winW, winH);
			this.Move (0, 0);

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
			hboxToolbarButtons.Children[0].TooltipText = Language.I.L[2];
			hboxToolbarButtons.Children[1].TooltipText = Language.I.L[3];
			hboxToolbarButtons.Children[2].TooltipText = Language.I.L[4];
			hboxToolbarButtons.Children[4].TooltipText = 
				Language.I.L[43] +	": " + 
				Language.I.L[0] + "\n\n" + 
				Language.I.L[44] +	": \n" +
				Language.AllLanguagesAsString;
			hboxToolbarButtons.Children[6].TooltipText = Language.I.L[84];

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
			if (timeoutSw.ElapsedMilliseconds < timeoutIntervalFirst) {
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
			if (timeoutSw.ElapsedMilliseconds < timeoutIntervalFirst) {
				return repeatTimeout;
			}

			MoveTimeoutSlider();
			return repeatTimeout;
		}

		private void OnSliderChangedValue(Picturez.Slider.Types t, float v)
		{
			int vScaledX = (int)(v * imagepanel1.ScaleCursorX + 0.5f);
			int vScaledY = (int)(v * imagepanel1.ScaleCursorY + 0.5f);
			switch (t) {
			case Picturez.Slider.Types.Left:
				entryLeft.Text = vScaledX.ToString ();
				break;
			case Picturez.Slider.Types.Right:
				entryRight.Text = vScaledX.ToString ();
				break;
			case Picturez.Slider.Types.Top:
				entryTop.Text = vScaledY.ToString ();
				break;
			case Picturez.Slider.Types.Bottom:
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
			Application.Quit ();
			a.RetVal = true;

			if (bt != null) {
				bt.Dispose ();
			}

			imagepanel1.Dispose ();
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


		[GLib.ConnectBefore ()] 
		protected void OnKeyPressEvent (object o, KeyPressEventArgs args)
		{
//			System.Console.WriteLine("Keypress: {0}  -->  State: {1}", args.Event.Key, args.Event.State); 

			if (args.Event.State == (Gdk.ModifierType.ControlMask | Gdk.ModifierType.Mod2Mask)) {
				switch (args.Event.Key) {
					case Gdk.Key.l:
						entryLeft.Text = config.Left.ToString ();
						OnEntryLeftKeyReleaseEvent (entryLeft, null);

						entryRight.Text = config.Right.ToString ();
						OnEntryRightKeyReleaseEvent (entryRight, null);

						entryTop.Text = config.Top.ToString ();
						OnEntryTopKeyReleaseEvent (entryTop, null);

						entryBottom.Text = config.Bottom.ToString ();
						OnEntryBottomKeyReleaseEvent (entryBottom, null);
						break;
					case Gdk.Key.r:
						if (!frameRotation.Sensitive)
							break;
						entryRotate.Text = config.Rotation.ToString ();
						OnEntryRotateKeyReleaseEvent (entryRotate, null);
						break;
					default:
						break;
				}
			} 

			imagepanel1.MoveSliderByKey (args.Event.Key, 1);
		}	

		private void FilterEvent(Bitmap filterBitmap)
		{
			Console.WriteLine ("FilterEvent");

			bt.Bitmap.Dispose ();
			bt.ChangeBitmapButNotTags(filterBitmap);

			Initialize (false);
		}
	}
}

