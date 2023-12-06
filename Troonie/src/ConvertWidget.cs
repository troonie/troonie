using System;
using Troonie_Lib;
using Gtk;
using System.Collections.Generic;
using IOPath = System.IO.Path;
using System.IO;
using System.Linq;

namespace Troonie
{
	[System.ComponentModel.ToolboxItem (true)]
	public partial class ConvertWidget : Gtk.Window
	{		
		private bool isSettingGuiToCurrentConfiguration;
		private bool leftControlPressed;
		private Troonie.ColorConverter colorConverter;
		private string format;
//		private float newVersion;
//		private Config config;

		public ConvertWidget (string[] pFilenames = null) : base (Gtk.WindowType.Toplevel)
		{
			this.Build ();
			this.SetIconFromFile(Constants.I.EXEPATH + Constants.ICONNAME);

			GuiHelper.I.CreateToolbarIconButton (hboxToolbarButtons, 0, "folder-new-3.png", Language.I.L[39], OnToolbarBtn_OpenPressed);
			GuiHelper.I.CreateToolbarIconButton (hboxToolbarButtons, 1, "edit-select-all.png", Language.I.L[40], OnToolbarBtn_SelectAllPressed);
			GuiHelper.I.CreateToolbarIconButton (hboxToolbarButtons, 2, "edit-clear-3.png", Language.I.L[41], OnToolbarBtn_ClearPressed);
			GuiHelper.I.CreateToolbarIconButton (hboxToolbarButtons, 3, "window-close-2.png", Language.I.L[42], OnToolbarBtn_RemovePressed);
			GuiHelper.I.CreateToolbarSeparator (hboxToolbarButtons, 4);
			GuiHelper.I.CreateDesktopcontextmenuLanguageAndInfoToolbarButtons (hboxToolbarButtons, 5, OnToolbarBtn_LanguagePressed);

			format = ".jpg";
			colorConverter = Troonie.ColorConverter.Instance;
			SetGuiColors ();
			SetLanguageToGui();

			htlbOutputDirectory.InitDefaultValues ();

			if (Constants.I.CONFIG.StretchImage == ConvertMode.Editor)
				Constants.I.CONFIG.StretchImage = ConvertMode.StretchForge;

			SetGuiToCurrentConfiguration();
			htlbOutputDirectory.OnHyperTextLabelTextChanged += OnHyperTextLabelTextChanged;

			if (Constants.I.WINDOWS) {
				rdPng1bit.Sensitive = true;
				rdBmp1bit.Sensitive = true;
				Gtk.Drag.DestSet (this, 0, null, 0);
			} else {
				// Original is ShadowType.EtchedIn, but linux cannot draw it correctly.
				// Otherwise ShadowType.In looks terrible at Win10.
				frameImageFormat.ShadowType = ShadowType.In;
				frameImageResize.ShadowType = ShadowType.In;
				frameImageList.ShadowType = ShadowType.In;
				frameImageFormat.ShadowType = ShadowType.In;
				frameOutputDirectory.ShadowType = ShadowType.In;

				Gtk.Drag.DestSet (this, DestDefaults.All, MainClass.Target_table, Gdk.DragAction.Copy);
			}

            if (!GuiHelper.I.CheckForJpegAndExiftool())
                pFilenames = null;

            if (pFilenames != null)
				FillImageList (new List<string>(pFilenames));

			SetCorrectWindowSize ();

			if (Constants.I.CONFIG.AskForDesktopContextMenu) {
				new AskForDesktopContextMenuWindow (true, Constants.I.CONFIG).Show ();
			}
		}

		private void SetCorrectWindowSize()
		{
			// best ubuntu 16.04 values // linux mint 17.3
			const int originalWidth = 900; // 765;
			const int originalHeight = 900; // 833;
			const int taskbarHeight = 90;

			// work around by GLib timeout to fix the GTK#-resizing bug
			GLib.TimeoutHandler timeoutHandler = () => {
				Gdk.Rectangle r = Screen.GetMonitorGeometry(Screen.GetMonitorAtWindow(this.GdkWindow));
				int maxW = /* Screen.Width */ r.Width;
				int maxH = /* Screen.Height */ r.Height - taskbarHeight;
				this.WidthRequest = Math.Min(originalWidth, maxW);
				this.HeightRequest = Math.Min(originalHeight, maxH);
				this.Move(0 + r.X, 0 + r.Y);
				// false, because usage only one time
				return false;
			};
			GLib.Timeout.Add(200, timeoutHandler);
		}

		private void SetGuiColors()
		{
			this.ModifyBg(StateType.Normal, colorConverter.GRID);
			eventboxRoot.ModifyBg(StateType.Normal, colorConverter.GRID);
			eventboxToolbar.ModifyBg(StateType.Normal, colorConverter.GRID);

			entryBiggerLength.ModifyBase(StateType.Normal, colorConverter.White);
			entryFixSizeWidth.ModifyBase(StateType.Normal, colorConverter.White);
			entryFixSizeHeight.ModifyBase(StateType.Normal, colorConverter.White);
			eventboxImageList.ModifyBg(StateType.Normal, colorConverter.White);

			lbFrameImageFormat.ModifyFg(StateType.Normal, colorConverter.FONT);
			lbFrameImageResize.ModifyFg(StateType.Normal, colorConverter.FONT);
			lbFrameOutputDirectory.ModifyFg(StateType.Normal, colorConverter.FONT);

			GtkLabel11.ModifyFg(StateType.Normal, colorConverter.FONT);
			GtkLabel15.ModifyFg(StateType.Normal, colorConverter.FONT);
			GtkLabel21.ModifyFg(StateType.Normal, colorConverter.FONT);
			GtkLabel29.ModifyFg(StateType.Normal, colorConverter.FONT);
			GtkLabel6.ModifyFg(StateType.Normal, colorConverter.FONT);
		}

		private void SetLanguageToGui()
		{			
			// hboxToolbarButtons.Children[0].TooltipText = Language.I.L[2];
			GtkLabel29.LabelProp = "<b>" + Language.I.L[19] + "</b>";
			lbFrameImageFormat.LabelProp = "<b>" + Language.I.L[20] + "</b>";
			rdJpeg.Label = "JPEG (24 Bit " + Language.I.L[22] + ")";
			rdJpegGray.Label = "JPEG (8 Bit " + Language.I.L[21] + ")";
			lbQuality.Text = Language.I.L[23];

			rdPng1bit.Label = "PNG (1 Bit " + Language.I.L[24] + ")";
			rdPng8Bit.Label = "PNG (8 Bit " + Language.I.L[21] + ")";
			rdPng24Bit.Label = "PNG (24 Bit " + Language.I.L[22] + ")";
			rdPNG32bit.Label = "PNG (32 Bit " + Language.I.L[25] + ")";
			rdPng32BitAlphaAsValue.Label = "PNG (32 Bit " + Language.I.L[79] + ")";
			lbTransparencyColor.Text = Language.I.L[26];

			rdBmp1bit.Label = "BMP (1 Bit " + Language.I.L[24] + ")";
			rdBmp8bit.Label = "BMP (8 Bit " + Language.I.L[21] + ")";
			rdBmp24bit.Label = "BMP (24 Bit " + Language.I.L[22] + ")";
			GtkLabel21.LabelProp = "<b>" + Language.I.L[27] + "</b>";

			lbFrameImageResize.LabelProp = "<b>" + Language.I.L[28] + "</b>";
//			GtkLabel24.LabelProp = "<b>" + Language.I.L[29] + "</b>";
			rdOriginalSize.Label = Language.I.L[30];
//			GtkLabel26.LabelProp = "<b>" + Language.I.L[31] + "</b>";
			rdBiggerLength.Label = Language.I.L[32];
//			GtkLabel30.LabelProp = "<b>" + Language.I.L[33] + "</b>";
			rdFixSize.Label = Language.I.L[34];
			checkBtnStretch.Label = Language.I.L[35];

			lbFrameOutputDirectory.LabelProp = "<b>" + Language.I.L[36] + "</b>";
			checkBtnUseOriginalDirectory.Label = Language.I.L[37];
			checkBtnOverwriteOriginalImage.Label = Language.I.L[38];

			btnConvert.Text = Language.I.L[45];
			btnConvert.Redraw ();
		}

		private void SetGuiToCurrentConfiguration()
		{
			isSettingGuiToCurrentConfiguration = true;

			// image format radio buttons
			switch (Constants.I.CONFIG.Format)
			{
			case TroonieImageFormat.BMP1:
				rdBmp1bit.Active = true;
				break;
			case TroonieImageFormat.BMP8:
				rdBmp8bit.Active = true;
				break;
			case TroonieImageFormat.BMP24:
				rdBmp24bit.Active = true;
				break;
			case TroonieImageFormat.EMF:
				rdEmf.Active = true;
				break;
			case TroonieImageFormat.GIF:
				rdGif.Active = true;
				break;
			case TroonieImageFormat.ICO:
				rdIcon.Active = true;
				break;
			case TroonieImageFormat.JPEG8:
				rdJpegGray.Active = true;
				break;
			case TroonieImageFormat.JPEG24:
				rdJpeg.Active = true;
				break;
			case TroonieImageFormat.PNG1:
				rdPng1bit.Active = true;
				break;
			case TroonieImageFormat.PNG8:
				rdPng8Bit.Active = true;
				break;
			case TroonieImageFormat.PNG24:
				rdPng24Bit.Active = true;
				break;
			case TroonieImageFormat.PNG32Transparency:
				rdPNG32bit.Active = true;
				break;
			case TroonieImageFormat.PNG32AlphaAsValue:
				rdPng32BitAlphaAsValue.Active = true;
				break;
			case TroonieImageFormat.TIFF:
				rdTiff.Active = true;
				break;
			case TroonieImageFormat.WMF:
				rdWmf.Active = true;
				break;
			}

			// jpg quality track bar
			hscaleQuality.Value = Constants.I.CONFIG.JpgQuality;

			// resize version radio button
			switch (Constants.I.CONFIG.ResizeVersion)
			{
			case ResizeVersion.No:
				rdOriginalSize.Active = true;
				break;
			case ResizeVersion.BiggestLength:
				rdBiggerLength.Active = true;
				break;
			case ResizeVersion.FixedSize:
				rdFixSize.Active = true;
				break;
			}

			// BiggestLength text box
			entryBiggerLength.Text = Constants.I.CONFIG.BiggestLength.ToString();

			// stretch image check box
			checkBtnStretch.Active = Constants.I.CONFIG.StretchImage == ConvertMode.StretchForge;

			// Fixed width and height text boxes
			entryFixSizeHeight.Text = Constants.I.CONFIG.Height.ToString();
			entryFixSizeWidth.Text = Constants.I.CONFIG.Width.ToString();

			// path
			htlbOutputDirectory.Text = Constants.I.CONFIG.Path;
			// htlbOutputDirectory.QueueDraw ();

			// using original path and overwrite image check boxes
			checkBtnUseOriginalDirectory.Active = Constants.I.CONFIG.UseOriginalPath;
			htlbOutputDirectory.Sensitive = !Constants.I.CONFIG.UseOriginalPath;
			checkBtnOverwriteOriginalImage.Active = Constants.I.CONFIG.FileOverwriting;

			// NEW: transparency color button
			btnColor.Color = new Gdk.Color(Constants.I.CONFIG.TransparencyColorRed,
				Constants.I.CONFIG.TransparencyColorGreen, Constants.I.CONFIG.TransparencyColorBlue);

			isSettingGuiToCurrentConfiguration = false;
		}

		private void OnHyperTextLabelTextChanged()
		{
			Constants.I.CONFIG.Path = htlbOutputDirectory.Text;
		}			

		protected void OnHscaleQualityValueChanged (object sender, EventArgs e)
		{
			Constants.I.CONFIG.JpgQuality = (byte)hscaleQuality.Value;
		}

		protected void OnBtnColorColorSet (object sender, EventArgs e)
		{			
			byte r, g, b;
			colorConverter.ToDotNetColor (btnColor.Color, out r, out g,out b);

			Constants.I.CONFIG.TransparencyColorRed = r;
			Constants.I.CONFIG.TransparencyColorGreen = g;
			Constants.I.CONFIG.TransparencyColorBlue = b;
		}

//		private void OnUpdateAvailable(float newVersion)
//		{
//			this.newVersion = newVersion;
////			GuiHelper.I.CreateToolbarIconButton (hboxToolbarButtons, 10, 
////				"security-medium-2.png", Language.I.L[70] + newVersion, OnToolbarBtn_UpdatePressed, Language.I.L[69]);
//
//			GuiHelper.I.CreateToolbarUpdateButton (hboxToolbarButtons, 8, 
//				"security-medium-2.png", Language.I.L[70], newVersion);			
//
//		}

		#region drag and drop

		private void FillImageList(List<string> newImages)
		{
			PressedInButton l_pressedInButton;

			for (int i=0; i<newImages.Count; ++i)
			{
                /// todo [2020-02-12] duplicated in GuiHelper.CorrectUmlautsOfDragData(..), plz test removing region
    //            #region [2020-02-12] duplicated in GuiHelper.CorrectUmlautsOfDragData(..), plz test removing region
    //            string waste = Constants.I.WINDOWS ? "file:///" : "file://";
				//newImages [i] = newImages [i].Replace (@waste, "");
				//// Also change possible wrong directory separator
				//newImages [i] = newImages [i].Replace (IOPath.AltDirectorySeparatorChar, IOPath.DirectorySeparatorChar);
                //#endregion deprectaed, plz testing

                // check whether file is image or video
                FileInfo info = new FileInfo (newImages [i]);
				string ext = info.Extension.ToLower ();
				bool isVideo = Constants.VideoExtensions.Any (x => x.Value.Item1 == ext || x.Value.Item2 == ext || x.Value.Item3 == ext);

				if (ext.Length != 0 && (Constants.Extensions.Any (x => x.Value.Item1 == ext || x.Value.Item2 == ext) || isVideo)) {
					l_pressedInButton = new PressedInButton ();
					l_pressedInButton.IsVideo = isVideo;
					l_pressedInButton.FullText = newImages [i];
					l_pressedInButton.Text = newImages [i].Substring(newImages[i].LastIndexOf(
						IOPath.DirectorySeparatorChar) + 1);	
					l_pressedInButton.CanFocus = true;
					l_pressedInButton.Sensitive = true;
					l_pressedInButton.TextSize = 10;
					l_pressedInButton.ShowAll ();

					vboxImageList.PackStart(l_pressedInButton, false, false, 0);
				}					
			}	
		}

		protected void OnDragDrop (object sender, Gtk.DragDropArgs args)
		{
			Gtk.Drag.GetData
			((Gtk.Widget)sender, args.Context,
				args.Context.Targets[0], args.Time);
		}

		protected void OnDragDataReceived (object sender, Gtk.DragDataReceivedArgs args)
		{
            List<string> newImages = GuiHelper.I.CorrectUmlautsOfDragData(Constants.I.WINDOWS, sender, args);
            if (newImages == null || newImages.Count == 0)
                return;

            FillImageList(newImages);
            newImages.Clear();
        }

		#endregion drag and drop

		protected void OnDeleteEvent (object sender, DeleteEventArgs a)
		{
			Config.Save (Constants.I.CONFIG);
			this.DestroyAll ();

			Application.Quit ();
			a.RetVal = true;
		}

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

//			case Gdk.Key.r:
//				OkCancelDialog warn = new OkCancelDialog (false);
//				warn.Title = Language.I.L [29];
//				warn.Label1 = Language.I.L [175];
//				warn.Label2 = Language.I.L [171];
//				warn.OkButtontext = Language.I.L [16];
//				warn.CancelButtontext = Language.I.L [17];	
//				warn.Show ();
//
//				warn.OnReleasedOkButton += RenameByCreationDate;
//				break;
//			case Gdk.Key.c:
//
//				OkCancelDialog warn2 = new OkCancelDialog (false);
//				warn2.Title = Language.I.L [29];
//				warn2.Label1 = Language.I.L [170];
//				warn2.Label2 = Language.I.L [171];
//				warn2.OkButtontext = Language.I.L [16];
//				warn2.CancelButtontext = Language.I.L [17];	
//				warn2.Show ();
//
//				warn2.OnReleasedOkButton += AppendIdAndCompressionByRating;						
//				break;

			case Gdk.Key.y:

				string dllTroonieSqlite = @"TroonieSqlite.dll";
				if (File.Exists(dllTroonieSqlite)){
					try {
						var DLL = System.Reflection.Assembly.LoadFile(dllTroonieSqlite);

						// instanciate namespace and class
						Type typePlugin = DLL.GetType("TroonieSqlite.MainClass");
						object objPlugin = Activator.CreateInstance(typePlugin);
//						// cast to base class
						IPlugin basePlugin = objPlugin as IPlugin;

						List<string> filenames = new List<string>();
						foreach (Widget w in vboxImageList.Children) {
							PressedInButton pib = w as PressedInButton;
							filenames.Add(pib.FullText);
						}

						// call method
						basePlugin.Start(filenames);
					}
					catch (Exception) {
//						Console.WriteLine (ex.Message);
					}
				}
					
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
				
		protected void OnBtnConvertReleased (object sender, EventArgs e)
		{			
//			Console.WriteLine ("OnBtnConvertReleased ConvertWidget");

			BitmapWithTag bt;
			int nr = 1;
			int count = vboxImageList.Children.Length;
			progressbar1.Adjustment.Lower = nr;
			progressbar1.Adjustment.Upper = count;
			List<string> errors = new List<string> ();

			foreach (PressedInButton pib in vboxImageList.Children) {
				string imageFile = pib.FullText;
				progressbar1.Adjustment.Value = nr;
				progressbar1.Text = (nr + "/" + count + "  ( " + 
									 nr * 100 / count +"% )");
				// Force redrawing progress bar
				Main.IterationDo (false);
				nr++;

				if (pib.IsVideo) {
					errors.Add (imageFile);
					continue;
				}

				try {
					bt = new BitmapWithTag (imageFile);
				}
				catch (Exception) {
					errors.Add (imageFile);
					continue;
				}

				string relativeImageName = imageFile.Substring(
					imageFile.LastIndexOf(IOPath.DirectorySeparatorChar) + 1);
				relativeImageName = relativeImageName.Substring(0, relativeImageName.LastIndexOf('.'));
				relativeImageName = relativeImageName + format;
				bool success = bt.Save (Constants.I.CONFIG, relativeImageName, true);
				bt.Dispose();
				if (!success) {
					errors.Add (imageFile);
				}
			}
				
			string mssg = string.Empty; // Language.I.L [172] + Environment.NewLine;
			if (errors.Count != 0) {
				mssg += Language.I.L [173] + Environment.NewLine + Environment.NewLine;
				foreach (string errorimage in errors) {
//					mssg += "  *  ..." + errorimage.Substring(errorimage.Length - 35) + Environment.NewLine;
					int l = errorimage.Length;
					if (l < 35) {
						mssg += "  *  " + errorimage + Environment.NewLine;
					} else {
						mssg += "  *  ..." + errorimage.Substring (l - 35) + Environment.NewLine;
					}
				}
			} else {
				mssg += Language.I.L [174] + Environment.NewLine;
			}

//			MessageDialog md = new MessageDialog(this, 
//				DialogFlags.DestroyWithParent, MessageType.Info, 
//				ButtonsType.Close, mssg);
//			md.Run();
//			md.Destroy();	

			OkCancelDialog win = new OkCancelDialog (true);
			win.WindowPosition = WindowPosition.CenterAlways;
			win.Title = Language.I.L [4];
			win.Label1 = Language.I.L [172] + Environment.NewLine;
			win.Label2 = mssg;
			win.OkButtontext = Language.I.L [16];
			win.Show ();
		}	
	}
}

