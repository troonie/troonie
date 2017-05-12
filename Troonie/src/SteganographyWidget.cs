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
using ImageConverter = Troonie_Lib.ImageConverter;
using Troonie;
using Troonie_Lib;
using System.Linq;

namespace Troonie
{
	public partial class SteganographyWidget : Gtk.Window
	{
		private static char sep = System.IO.Path.DirectorySeparatorChar;
		private Troonie.ColorConverter colorConverter = Troonie.ColorConverter.Instance;
		private Constants constants = Constants.I;
		private bool leftControlPressed;
		private int imageW; 
		private int imageH;
		private string tempScaledImageFileName;

		public string FileName { get; set; }
		public BitmapWithTag bt;

		public SteganographyWidget (string pFilename = null) : base (Gtk.WindowType.Toplevel)
		{

			try {
				
				FileName = pFilename;

				Build ();

				textviewContent.Buffer.Changed += (sender, e) => { ChangeLbPayloadspace (); };

				hypertextlabelFileChooser.InitDefaultValues();
				hypertextlabelFileChooser.OnHyperTextLabelTextChanged += ChangeLbPayloadspace;
				hypertextlabelFileChooser.HeightRequest = entryKey.Allocation.Height;
				hypertextlabelFileChooser.Text = Constants.I.WINDOWS ? Environment.ExpandEnvironmentVariables ("%HOMEDRIVE%%HOMEPATH%")
					: Environment.GetEnvironmentVariable ("HOME");
//				hypertextlabelFileChooser.FileChooserAction = FileChooserAction.Open;
//				// default values
//				hypertextlabelFileChooser.Sensitive = true;
//				hypertextlabelFileChooser.ShownTextLength = 45;
//				// TextColor = colorConverter.Blue;
//				hypertextlabelFileChooser.Alignment = Pango.Alignment.Left;
//				hypertextlabelFileChooser.TextSize = 11;
//				hypertextlabelFileChooser.Bold = false;
//				hypertextlabelFileChooser.Italic = false;
//				hypertextlabelFileChooser.HeightRequest = 5;
//				hypertextlabelFileChooser.Font = "Serif";						

				this.SetIconFromFile(Constants.I.EXEPATH + Constants.ICONNAME);

				GuiHelper.I.CreateToolbarIconButton (hboxToolbarButtons, 0, "folder-new-3.png", Language.I.L[2], OnToolbarBtn_OpenPressed);
				GuiHelper.I.CreateToolbarIconButton (hboxToolbarButtons, 1, "document-save-5.png", Language.I.L[3], OnToolbarBtn_SaveAsPressed);
				GuiHelper.I.CreateToolbarSeparator (hboxToolbarButtons, 2);
				GuiHelper.I.CreateDesktopcontextmenuLanguageAndInfoToolbarButtons (hboxToolbarButtons, 3, OnToolbarBtn_LanguagePressed);

				SetGuiColors ();
				SetLanguageToGui ();

				Initialize(true);
//				rdBtn01_StegHash.Active = !BitSteg.SHOW_IN_GUI;
//				rdBtn02_BitSteg_Text.Active = BitSteg.SHOW_IN_GUI;
				if (!BitSteg.SHOW_IN_GUI) {
					comboboxAlgorithm.RemoveText(2);
					comboboxAlgorithm.RemoveText(1);
					comboboxAlgorithm.Active = 0;
				}

				if (constants.WINDOWS) {
					Gtk.Drag.DestSet (this, 0, null, 0);
				} else {
					// Original is ShadowType.EtchedIn, but linux cannot draw it correctly.
					// Otherwise ShadowType.In looks terrible at Win10.
					framePayloadspace.ShadowType = ShadowType.In;
					frameSteganography.ShadowType = ShadowType.In;
					frameProperties.ShadowType = ShadowType.In;
					frameKey.ShadowType = ShadowType.In;
					frameContent.ShadowType = ShadowType.In;
					frameFileChooser.ShadowType = ShadowType.In;
					Gtk.Drag.DestSet (this, DestDefaults.All, MainClass.Target_table, Gdk.DragAction.Copy);
				}					

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
				FileName = constants.EXEPATH + Constants.BLACKFILENAME;
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

						if (ext.Length != 0 && Constants.Extensions.Any(x => x.Value.Item1 == ext || x.Value.Item2 == ext)) {
							Title = FileName;
							bt = new BitmapWithTag(FileName, true);
							imageW = bt.Bitmap.Width;
							imageH = bt.Bitmap.Height;
						}
						else{
							LoadException ();
						}
					} // try end
					catch (ArgumentException) {
						LoadException ();
						return;
					}
				} // else end
			}			

			// Gdk.Pixbuf.GetFileInfo(FileName, out imageW, out imageH);

			GuiHelper.I.SetPanelSize(this, simpleimagepanel1, hbox1, 500, 600, imageW, imageH, 1270, 650);

			tempScaledImageFileName = constants.EXEPATH + "tempScaledImageFileName.png";

			simpleimagepanel1.SurfaceFileName = tempScaledImageFileName;

			if (newFileName) 
			{
				Bitmap pic = TroonieBitmap.FromFile (FileName); // new Bitmap(FileName);                              
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
			ChangeLbPayloadspace ();

			Show (); // ShowAll();
		}		

		private void SetGuiColors()
		{
			this.ModifyBg(StateType.Normal, colorConverter.GRID);
			eventboxToolbar.ModifyBg(StateType.Normal, colorConverter.GRID);

			lbFramePayloadspace.ModifyFg (StateType.Normal, colorConverter.FONT);
			lbPayloadspace.ModifyFg (StateType.Normal, colorConverter.FONT);

			lbFrameSteganography.ModifyFg (StateType.Normal, colorConverter.FONT);
			lbFrameProperties.ModifyFg (StateType.Normal, colorConverter.FONT);
			lbFrameKey.ModifyFg (StateType.Normal, colorConverter.FONT);
			lbFrameContent.ModifyFg (StateType.Normal, colorConverter.FONT);
			lbFrameFileChooser.ModifyFg (StateType.Normal, colorConverter.FONT);
		}

		private void SetLanguageToGui()
		{
			string dummySpaces = "  ";
			lbFramePayloadspace.LabelProp = "<b>" + Language.I.L[246] + "</b>";
			btnOk.Text = Language.I.L[16];
			btnOk.Redraw ();

			lbFrameSteganography.LabelProp = "<b>" + Language.I.L[73] + "</b>";
			lbFrameProperties.LabelProp = "<b>" + Language.I.L[241] + "</b>";
			lbModus.LabelProp = "<b>" + dummySpaces + Language.I.L[74] + "</b>";
			rdBtnDecrypt.Label = Language.I.L[75];
			rdBtnEncrypt.Label = Language.I.L[76];
			lbPayload.LabelProp = "<b>" + dummySpaces + dummySpaces + Language.I.L[242] + "</b>";
			rdBtnPayloadText.Label = Language.I.L[243];
			rdBtnPayloadFile.Label = Language.I.L[244];
			lbAlgorithm.LabelProp = "<b>" + dummySpaces + dummySpaces + Language.I.L[245] + "</b>";
			checkBtnStrongObfuscation.Label = Language.I.L[160];
			lbFrameKey.LabelProp = "<b>" + Language.I.L[77] + "</b>";
			lbFrameContent.LabelProp = "<b>" + Language.I.L[78] + "</b>";
			lbFrameFileChooser.LabelProp = "<b>" + Language.I.L[231] + "</b>";
		}

		private bool FileCheckForBitSteg()
		{
			entryFile.Text = FileHelper.I.TransformStringToValidFilename (entryFile.Text, true);
			if (rdBtnDecrypt.Active &&  entryFile.Text == string.Empty) {
				OkCancelDialog warn = new OkCancelDialog (true);
				warn.WindowPosition = WindowPosition.CenterAlways;
				warn.Title = Language.I.L [233];
				warn.Label1 = string.Empty;
				warn.Label2 = Language.I.L [234];
				warn.OkButtontext = Language.I.L [16];
				warn.Show ();
				entryFile.GrabFocus ();
				return false;
			}

			if (rdBtnEncrypt.Active && !File.Exists(hypertextlabelFileChooser.Text)) {
				OkCancelDialog warn = new OkCancelDialog (true);
				warn.WindowPosition = WindowPosition.CenterAlways;
				warn.Title = Language.I.L [235];
				warn.Label1 = string.Empty;
				warn.Label2 = Language.I.L [232];
				warn.OkButtontext = Language.I.L [16];
				warn.Show ();
				hypertextlabelFileChooser.GrabFocus ();
				return false;
			}
				
			return true;
		}

		private void DoSelectedSteganography()
		{
			switch (comboboxAlgorithm.Active) {
			case 0:
				DoStegHash ();
				break;
			case 1:
			case 2:
				if (rdBtnPayloadText.Active || FileCheckForBitSteg ()) {
					DoBitSteg ();
				}
				break;
			}
		}

		private void DoStegHash()
		{
			Bitmap b1 = null;
			StegHashFilter filter = new StegHashFilter ();
			filter.Key = entryKey.Text;
			entryKey.Text = string.Empty;
			filter.WritingMode = rdBtnEncrypt.Active;
			filter.UseStrongObfuscation = checkBtnStrongObfuscation.Active;

			OkCancelDialog pseudo = new OkCancelDialog (true);
			pseudo.WindowPosition = WindowPosition.CenterAlways;
			pseudo.Title = Language.I.L [80];
			pseudo.Label1 = Language.I.L [81];
			pseudo.OkButtontext = Language.I.L [16];
			pseudo.CancelButtontext = Language.I.L [17];

			if (filter.WritingMode) {
				string[] content = textviewContent.Buffer.Text.Split ('\n');
				filter.FillLines (content);
				// only necessary by Steganography1
//				b1 = ImageConverter.To32Bpp(bt.Bitmap);
//				b1 = filter.Apply (b1, null);

				// check and convert, if 8-bit source image
				if (bt.Bitmap.PixelFormat == PixelFormat.Format8bppIndexed) {
					bt.ChangeBitmapButNotTags(ImageConverter.To24Bpp (bt.Bitmap));
				}
				b1 = filter.Apply (bt.Bitmap, null);

				if (filter.Success) {
					pseudo.Label2 = Language.I.L [83];
				} else {
					pseudo.Label1 =  Language.I.L [53];
					pseudo.Label2 =  Language.I.L [52];
				}
			} 
			else {
				if (!ImageConverter.IsColorImage(bt.Bitmap)) {
					pseudo.DestroyAll ();
					OkCancelDialog wrongImageContextMenu = new OkCancelDialog (true);
					wrongImageContextMenu.WindowPosition = WindowPosition.CenterAlways;
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
			
		private void DoBitSteg()
		{
			/* 2 == BitStegRGB */
			BitSteg bs = comboboxAlgorithm.Active == 2 ? new BitStegRGB() : new BitSteg ();

			OkCancelDialog pseudo = new OkCancelDialog (true);
			pseudo.WindowPosition = WindowPosition.CenterAlways;
			pseudo.Title = Language.I.L [80];
			pseudo.Label1 = Language.I.L [81];
			pseudo.OkButtontext = Language.I.L [16];
			pseudo.CancelButtontext = Language.I.L [17];

			if (rdBtnEncrypt.Active) {
				int success = 0;

				if (rdBtnPayloadFile.Active) {
					byte[] bytes = Troonie_Lib.IOFile.BytesFromFile(hypertextlabelFileChooser.Text);
					success = bs.Write(bt.Bitmap, entryKey.Text, bytes);
				} else {
					success = bs.Write (bt.Bitmap, entryKey.Text, textviewContent.Buffer.Text);
				}

				switch (success) {
				case 0:
					pseudo.Label2 = Language.I.L [83];
					break;
				case 1:
					pseudo.Label1 =  Language.I.L [53];
					pseudo.Label2 =  Language.I.L [52];
					break;
				case 2:
					pseudo.Label1 = Language.I.L [53];
					pseudo.Label2 = Language.I.L [236];
					break;
				case 3:
					pseudo.Label1 = Language.I.L [53];
					pseudo.Label2 = Language.I.L [237];
					break;
				case 4:
					pseudo.Label1 = Language.I.L [53];
					pseudo.Label2 = Language.I.L [238];
					break;
				case 5:
					pseudo.Label1 = Language.I.L [53];
					pseudo.Label2 = Language.I.L [240];
					break;
				}
			}
			else { /* READING */
				pseudo.Label2 = Language.I.L [82];

				if (rdBtnPayloadFile.Active) {
					byte[] bytes;
					bs.Read (bt.Bitmap, entryKey.Text, out bytes);
					try {
						IOFile.BytesToFile(bytes, hypertextlabelFileChooser.Text + sep + entryFile.Text);
					}
					catch(Exception) {
						pseudo.Label1 = Language.I.L [53];
						pseudo.Label2 = Language.I.L [239];
					}
				} else {
					string content;
					bs.Read (bt.Bitmap, entryKey.Text, out content);
					textviewContent.Buffer.Text = content;
				}					
			}

			entryKey.Text = string.Empty;
			Initialize (false);

			pseudo.Show ();
		}			

		private void OpenSaveAsDialog()
		{
			SaveAsDialog dialog = new SaveAsDialog(bt, ConvertMode.Editor);
			bool runDialog = true;
			dialog.AllowOnlyColorLoselessSaving ();

			do
			{
				if (dialog.Run () == (int)ResponseType.Ok) {
					bool success = dialog.Process ();
					if (success) {
						FileName = dialog.SavedFileName;
						bt.Dispose ();
						Initialize (true);
						runDialog = false;
					}
					else {
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

		protected void OnDeleteEvent (object sender, DeleteEventArgs a)
		{
			if (bt != null) {
				bt.Dispose ();
			}
			this.DestroyAll ();

			try {
				File.Delete (tempScaledImageFileName);
				File.Delete (Constants.I.EXEPATH + Constants.BLACKFILENAME);
			}
			catch (Exception) {				
				Console.WriteLine(Constants.ERROR_DELETE_TEMP_FILES);;
			}

			Application.Quit ();
			a.RetVal = true;
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
				OkCancelDialog warn = new OkCancelDialog (true);
				warn.WindowPosition = WindowPosition.CenterAlways;
				warn.Title = Language.I.L [118];
				warn.Label1 = string.Empty;
				warn.Label2 = Language.I.L [119];
				warn.OkButtontext = Language.I.L [16];
				//				warn.CancelButtontext = Language.I.L [17];	
				warn.Show ();

				return;
			}

			if (rdBtnEncrypt.Active) {
				if (entryKey.Text.Length < 10) {
					OkCancelDialog warn = new OkCancelDialog (false);
					warn.WindowPosition = WindowPosition.CenterAlways;
					warn.Title = Language.I.L [109];
					warn.Label1 = Language.I.L [110] + entryKey.Text.Length + Language.I.L [111];
					warn.Label2 = Language.I.L [112];
					warn.OkButtontext = Language.I.L [16];
					warn.CancelButtontext = Language.I.L [17];	
					warn.Show ();

					warn.OnReleasedOkButton += StartPasswordDialog;
				} else {
					StartPasswordDialog ();
				}				
			}
			else /* if (rdBtnRead.Active) */ {
				DoSelectedSteganography ();
			}				
		}

		private void StartPasswordDialog()
		{
			PasswordDialog pw = new PasswordDialog ();
			pw.WindowPosition = WindowPosition.CenterAlways;
			pw.Title = Language.I.L [167];
			pw.OkButtontext = Language.I.L [16];
			pw.OnReleasedOkButton += ConfirmKey;
			pw.Show ();
		}

		private void ConfirmKey(string password)
		{
			if (password == entryKey.Text) {
				DoSelectedSteganography ();
			} else {
				OkCancelDialog warn = new OkCancelDialog (true);
				warn.WindowPosition = WindowPosition.CenterAlways;
				warn.Title = Language.I.L [166];
				warn.Label1 = Language.I.L [166];
				warn.Label2 = Language.I.L [167];
				warn.OkButtontext = Language.I.L [16];	
				warn.Show ();
				warn.OnReleasedOkButton += StartPasswordDialog;
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

			if (args.Event.Key == Gdk.Key.Return) {
				OnBtnOkButtonReleaseEvent (o, null);
			}
		}

		[GLib.ConnectBefore ()] 
		protected void OnKeyPressEvent (object o, KeyPressEventArgs args)
		{
			#region 'ctrl + ...'
			if (leftControlPressed) {
				switch (args.Event.Key) {
				case Gdk.Key.s:
					OpenSaveAsDialog ();
					break;
				case Gdk.Key.o:
					// shows/hides common hidden checkbox for StegHash-StrongObfuscation
					checkBtnStrongObfuscation.Visible = !checkBtnStrongObfuscation.Visible;
					break;
				}

//				leftControlPressed = false;

				return;
			}
			#endregion 'ctrl + ...


			switch (args.Event.Key) {
			case Gdk.Key.Control_L:
				leftControlPressed = true;
				break;
			}
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

		protected void OnRdBtnEncryptToggled (object sender, EventArgs e)
		{
			checkBtnStrongObfuscation.Sensitive = rdBtnEncrypt.Active;
			entryFile.Visible = rdBtnDecrypt.Active;
				
			if (rdBtnDecrypt.Active) {
				hypertextlabelFileChooser.Text = Constants.I.WINDOWS ? Environment.ExpandEnvironmentVariables ("%HOMEDRIVE%%HOMEPATH%")
				: Environment.GetEnvironmentVariable ("HOME");
				hypertextlabelFileChooser.FileChooserAction = FileChooserAction.SelectFolder;
			} else {
				hypertextlabelFileChooser.Text = Language.I.L [232];
				hypertextlabelFileChooser.FileChooserAction = FileChooserAction.Open;
			}

			ChangeLbPayloadspace ();
		}	

		private void ChangeLbPayloadspace()
		{
			long l;
			/* 3 == BitStegRGB,  1 == BitSteg */
			int multiplicator = comboboxAlgorithm.Active == 2 ? 3 : 1;
			int dim = multiplicator * (imageW * imageH) / 8 - BitSteg.LengthFinalBytes;

			if (rdBtnPayloadText.Active && comboboxAlgorithm.Active != 0) {
				l = textviewContent.Buffer.Text.Length;
			} else if (rdBtnEncrypt.Active && File.Exists (hypertextlabelFileChooser.Text)) {
				FileInfo info = new FileInfo (hypertextlabelFileChooser.Text);
				l = info.Length;
			} else {
				lbPayloadspace.ModifyFg (StateType.Normal, colorConverter.FONT);
				lbPayloadspace.Text = "-"; 
				return;
			}

			if (l > dim) {	
				lbPayloadspace.UseMarkup = true;
				lbPayloadspace.ModifyFg (StateType.Normal, colorConverter.Red);
				lbPayloadspace.LabelProp = "<b>" + Language.I.L[29] + "  " + l + " / " + dim + " " + Language.I.L[247] + " </b>";
			} else {
				lbPayloadspace.ModifyFg (StateType.Normal, colorConverter.FONT);
				lbPayloadspace.Text = 	l + " / " + dim + " " + Language.I.L[247];					
			}
		}

		protected void OnComboboxAlgorithmChanged (object sender, EventArgs e)
		{
			if (comboboxAlgorithm.Active == 0) {
				rdBtnPayloadText.Active = true;
			}

			if (rdBtnPayloadText.Active) {
				frameFileChooser.Visible = false;
				frameContent.Visible = true;
			} else {
				frameFileChooser.Visible = true;
				frameContent.Visible = false;
			}

			rdBtnPayloadFile.Sensitive = comboboxAlgorithm.Active != 0; 

			ChangeLbPayloadspace ();
		}			
	}
}

