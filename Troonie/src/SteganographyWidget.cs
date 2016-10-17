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
		private const string blackFileName = "black.png";

		private Troonie.ColorConverter colorConverter = Troonie.ColorConverter.Instance;
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

			GuiHelper.I.CreateToolbarIconButton (hboxToolbarButtons, 0, "folder-new-3.png", Language.I.L[2], OnToolbarBtn_OpenPressed);
			GuiHelper.I.CreateToolbarIconButton (hboxToolbarButtons, 1, "document-save-5.png", Language.I.L[3], OnToolbarBtn_SaveAsPressed);
			GuiHelper.I.CreateToolbarSeparator (hboxToolbarButtons, 2);
			GuiHelper.I.CreateDesktopcontextmenuLanguageAndInfoToolbarButtons (hboxToolbarButtons, 3, OnToolbarBtn_LanguagePressed);

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
			Gtk.Drag.DestSet (this, DestDefaults.All, MainClass.Target_table, Gdk.DragAction.Copy);
		}

			simpleimagepanel1.OnCursorPosChanged += OnCursorPosChanged;

			if (Constants.I.CONFIG.AskForDesktopContextMenu) {
				new AskForDesktopContextMenuWindow (true, Constants.I.CONFIG).Show ();
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

			GuiHelper.I.SetPanelSize(this, simpleimagepanel1, hbox1, 500, 600, imageW, imageH, 1200, 650);

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
		}

		private void SetLanguageToGui()
		{
			lbFrameCursorPos.LabelProp = "<b>" + Language.I.L[15] + "</b>";
			btnOk.Text = Language.I.L[16];
			btnOk.Redraw ();

			lbFrameSteganography.LabelProp = "<b>" + Language.I.L[73] + "</b>";
			lbFrameModus.LabelProp = "<b>" + Language.I.L[74] + "</b>";
			rdBtnRead.Label = Language.I.L[75];
			rdBtnWrite.Label = Language.I.L[76];
			checkBtnStrongObfuscation.Label = Language.I.L[160];
			lbFrameKey.LabelProp = "<b>" + Language.I.L[77] + "</b>";
			lbFrameContent.LabelProp = "<b>" + Language.I.L[78] + "</b>";
		}

		private void DoSteganography()
		{
			Bitmap b1 = null;
			StegHashFilter filter = new StegHashFilter ();
			filter.Key = entryKey.Text;
			entryKey.Text = string.Empty;
			filter.WritingMode = rdBtnWrite.Active;
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

		private void OnCursorPosChanged(int x, int y)
		{
			lbCursorPos.Text = 	x.ToString() + " x " +	y.ToString();
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

			if (rdBtnWrite.Active) {
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
				DoSteganography ();
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
				DoSteganography ();
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
//			System.Console.WriteLine("Keypress: {0}  -->  State: {1}", args.Event.Key, args.Event.State); 

			switch (args.Event.Key) {
			case Gdk.Key.s:
				if (args.Event.State == (Gdk.ModifierType.ControlMask /* | Gdk.ModifierType.Mod2Mask */))
					OpenSaveAsDialog ();
				break;
				default:
				break;
			}
		}	

		protected void OnRdBtnWriteToggled (object sender, EventArgs e)
		{
				checkBtnStrongObfuscation.Sensitive = rdBtnWrite.Active;
		}
	}
}

