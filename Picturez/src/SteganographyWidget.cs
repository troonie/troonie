using System;
using System.Collections.Generic;
using System.Drawing;
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

namespace Picturez
{
	public partial class SteganographyWidget : Gtk.Window
	{
		private const string blackFileName = "black.png";

		private Picturez.ColorConverter colorConverter = Picturez.ColorConverter.Instance;
		private Constants constants = Constants.I;
		private int imageW; 
		private int imageH;
		private string tempScaledImageFileName;

		public string FileName { get; set; }
		public BitmapWithTag bt;

		public SteganographyWidget (string pFilename = null) : base (Gtk.WindowType.Toplevel)
		{
			FileName = pFilename;

			Build ();
			this.SetIconFromFile(Constants.I.EXEPATH + Constants.ICONNAME);

			GuiHelper.I.CreateToolbarIconButton (hboxToolbarButtons, 0, "folder-new-3.png", OnToolbarBtn_OpenPressed);
			GuiHelper.I.CreateToolbarIconButton (hboxToolbarButtons, 1, "document-save-5.png", OnToolbarBtn_SaveAsPressed);
			GuiHelper.I.CreateToolbarIconButton (hboxToolbarButtons, 2, "help-about-3.png", OnToolbarBtn_AboutPressed);
			GuiHelper.I.CreateToolbarSeparator (hboxToolbarButtons, 3);
			GuiHelper.I.CreateToolbarIconButton (hboxToolbarButtons, 4, "tools-check-spelling-5.png", OnToolbarBtn_LanguagePressed);

			SetGuiColors ();
			SetLanguageToGui ();
			Initialize(true);

		if (constants.WINDOWS) {
			Gtk.Drag.DestSet (this, 0, null, 0);
		} else {
			// Original is ShadowType.EtchedIn, but linux cannot draw it correctly.
			// Otherwise ShadowType.In looks terrible at Win10.
			frameCursorPos.ShadowType = ShadowType.In;
			frameSteganography.ShadowType = ShadowType.In;
			frameModus.ShadowType = ShadowType.In;
			frameKey.ShadowType = ShadowType.In;
			frameContent.ShadowType = ShadowType.In;
			frameMinAlphaValue.ShadowType = ShadowType.In;
			Gtk.Drag.DestSet (this, DestDefaults.All, MainClass.Target_table, Gdk.DragAction.Copy);
		}

			simpleimagepanel1.OnCursorPosChanged += OnCursorPosChanged;
		}

		public override void Destroy ()
		{
			if (bt != null) {
				bt.Dispose ();
			}
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

			simpleimagepanel1.SurfaceFileName = tempScaledImageFileName;

			if (newFileName) 
			{
				Bitmap pic = new Bitmap(FileName);                              
				Bitmap croppedPic;

				ImageConverter.ScaleAndCut (
					pic, 
					out croppedPic, 
					0,
					0,
					simpleimagepanel1.WidthRequest,
					simpleimagepanel1.HeightRequest,
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
					simpleimagepanel1.WidthRequest,
					simpleimagepanel1.HeightRequest,
					ConvertMode.StretchForge,
					false);

				b2.Save (tempScaledImageFileName, ImageFormat.Png);
				b2.Dispose ();
			}

			simpleimagepanel1.Initialize();

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
			// DIFFERENCE 1 to EditWidget
//			int winH = bounds.Height - taskbarHeight - 300;
//			int winW = 700;
			int winH = 600;

			// DIFFERENCE 2 to EditWidget
//			int panelW = winW - optionsWidth - paddingOffset;
//			int panelH = winH - (int)(paddingOffset * multiplicatorHeight);
			int panelW = 300;
			int panelH = 200;

			// setting padding for left and right side
			global::Gtk.Box.BoxChild w4 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.simpleimagepanel1]));
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

			simpleimagepanel1.WidthRequest = panelW;
			simpleimagepanel1.HeightRequest = panelH;

			simpleimagepanel1.ScaleCursorX = imageW / (float)panelW;
			simpleimagepanel1.ScaleCursorY = imageH / (float)panelH;

			this.Resize (winW, winH);
			this.Move (0, 0);
		}

		private void SetGuiColors()
		{
			this.ModifyBg(StateType.Normal, colorConverter.GRID);
			eventboxToolbar.ModifyBg(StateType.Normal, colorConverter.GRID);

			lbFrameCursorPos.ModifyFg (StateType.Normal, colorConverter.FONT);
			lbCursorPos.ModifyFg (StateType.Normal, colorConverter.FONT);

			lbFrameSteganography.ModifyFg (StateType.Normal, colorConverter.FONT);
			lbFrameModus.ModifyFg (StateType.Normal, colorConverter.FONT);
			lbFrameKey.ModifyFg (StateType.Normal, colorConverter.FONT);
			lbFrameContent.ModifyFg (StateType.Normal, colorConverter.FONT);
			lbFrameMinAlphaValue.ModifyFg (StateType.Normal, colorConverter.FONT);
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

			lbFrameCursorPos.LabelProp = "<b>" + Language.I.L[15] + "</b>";
			btnOk.Text = Language.I.L[16];
			btnOk.Redraw ();

			lbFrameSteganography.LabelProp = "<b>" + Language.I.L[73] + "</b>";
			lbFrameModus.LabelProp = "<b>" + Language.I.L[74] + "</b>";
			rdBtnRead.Label = Language.I.L[75];
			rdBtnWrite.Label = Language.I.L[76];
			lbFrameKey.LabelProp = "<b>" + Language.I.L[77] + "</b>";
			lbFrameContent.LabelProp = "<b>" + Language.I.L[78] + "</b>";
			lbFrameMinAlphaValue.LabelProp = "<b>" + Language.I.L[29] + "</b>";

			frameMinAlphaValue.TooltipMarkup = "<b>" + Language.I.L[31] + "</b>" + Language.I.L[33];
		}

		private void DoSteganography()
		{
			Bitmap b1 = null;
			SteganographyFilter filter = new SteganographyFilter ();
			filter.MinAlphaValue = int.Parse(comboboxMinAlphaValue.ActiveText);
			filter.Key = entryKey.Text;
			entryKey.Text = string.Empty;
			filter.WritingMode = rdBtnWrite.Active;

			PseudoPicturezContextMenu pseudo = new PseudoPicturezContextMenu (true);
			pseudo.Title = Language.I.L [80];
			pseudo.Label1 = Language.I.L [81];
			pseudo.OkButtontext = Language.I.L [16];
			pseudo.CancelButtontext = Language.I.L [17];

			if (filter.WritingMode) {
				string[] content = textviewContent.Buffer.Text.Split ('\n');
				filter.FillLines (content);
				b1 = ImageConverter.To32Bpp(bt.Bitmap);
				b1 = filter.Apply (b1, null);

				if (filter.Success) {
					pseudo.Label2 = Language.I.L [83];
				} else {
					pseudo.Label1 =  Language.I.L [53];
					pseudo.Label2 =  Language.I.L [52];
				}
			} 
			else {
				if (bt.Bitmap.PixelFormat != PixelFormat.Format32bppArgb) {
					pseudo.DestroyAll ();
					PseudoPicturezContextMenu wrongImageContextMenu = new PseudoPicturezContextMenu (true);
					wrongImageContextMenu.Title = Language.I.L [53];
					wrongImageContextMenu.Label1 = Language.I.L [55];
					wrongImageContextMenu.Label2 = Language.I.L [56];
					wrongImageContextMenu.OkButtontext = Language.I.L [16];
					//					wrongImageContextMenu.CancelButtontext = Language.I.L [17];
					wrongImageContextMenu.Show ();
					return;
				}

				b1 = filter.Apply (bt.Bitmap, null);
				textviewContent.Buffer.Text = string.Empty;
				foreach (var item in filter.GetLines()) {
					textviewContent.Buffer.Text += item + "\n";
				}
				pseudo.Label2 = Language.I.L [82];
			}				

			bt.Bitmap.Dispose ();
			bt.ChangeBitmapButNotTags(b1);

			Initialize (false);

			pseudo.Show ();
		}

		private void OnCursorPosChanged(int x, int y)
		{
			lbCursorPos.Text = 	x.ToString() + " x " +	y.ToString();
		}

		protected void OnDeleteEvent (object sender, DeleteEventArgs a)
		{
			if (bt != null) {
				bt.Dispose ();
			}
			this.DestroyAll ();

			Application.Quit ();
			a.RetVal = true;

			File.Delete (tempScaledImageFileName);
			File.Delete (Constants.I.EXEPATH + blackFileName);
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

		protected void OnBtnOkButtonReleaseEvent (object o, ButtonReleaseEventArgs args)
		{
			if (entryKey.Text.Length == 0) {
				PseudoPicturezContextMenu warn = new PseudoPicturezContextMenu (true);
				warn.Title = Language.I.L [118];
				warn.Label1 = string.Empty;
				warn.Label2 = Language.I.L [119];
				warn.OkButtontext = Language.I.L [16];
//				warn.CancelButtontext = Language.I.L [17];	
				warn.Show ();

				return;
			}

			if (rdBtnWrite.Active && entryKey.Text.Length < 10) {
				PseudoPicturezContextMenu warn = new PseudoPicturezContextMenu (false);
				warn.Title = Language.I.L [109];
				warn.Label1 = Language.I.L [110] + entryKey.Text.Length + Language.I.L [111];
				warn.Label2 = Language.I.L [112];
				warn.OkButtontext = Language.I.L [16];
				warn.CancelButtontext = Language.I.L [17];	
				warn.Show ();

				warn.OnReleasedOkButton += DoSteganography;
			} else {
				DoSteganography ();
			}
		}

		protected void OnEntryKeyKeyReleaseEvent (object o, KeyReleaseEventArgs args)
		{
			if (entryKey.Text.Length == 0)
				return;

			char c = entryKey.Text [entryKey.Text.Length - 1];
			if (c == ' ') {
				entryKey.DeleteText (entryKey.CursorPosition - 1, entryKey.CursorPosition);
			}
		}

		protected void OnRdBtnWriteToggled (object sender, EventArgs e)
		{
			frameMinAlphaValue.Sensitive = rdBtnWrite.Active;
		}
	}
}

