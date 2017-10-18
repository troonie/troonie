using System;
using System.Linq;
using System.Text.RegularExpressions;
using Gtk;
using Troonie_Lib;
using IOPath = System.IO.Path;

namespace Troonie
{
	[System.ComponentModel.ToolboxItem (true)]
	public partial class SaveAsDialog : Gtk.Dialog
	{
		private bool onlyColorLosslessSaving;
		private BitmapWithTag bitmap;
		//		private TroonieImageFormat format;
		//		private byte jpegQuality;
		//		private Gdk.Color transparencyColor;
		private Config config;
		private Troonie.ColorConverter colorConverter = Troonie.ColorConverter.Instance;

		public string SavedFileName { get { return htLabelDirectory.Text + IOPath.DirectorySeparatorChar + entryFilename.Text +
				lbFormat.Text; } }

		public SaveAsDialog (BitmapWithTag b, ConvertMode cm)
		{			
			this.Build ();

			bitmap = b;
			config = new Config ();
			config.StretchImage = cm;
			onlyColorLosslessSaving = false;

			btnColor.Color = colorConverter.White;
			btnReplaceAlphaColor.Color = colorConverter.White;
			htLabelDirectory.InitDefaultValues ();
			htLabelDirectory.OnHyperTextLabelTextChanged += OnHyperTextLabelTextChanged;

			this.ModifyBg(StateType.Normal, colorConverter.GRID);
			GtkLabel3.ModifyFg(StateType.Normal, colorConverter.FONT);
			GtkLabel8.ModifyFg(StateType.Normal, colorConverter.FONT);
			GtkLabel12.ModifyFg(StateType.Normal, colorConverter.FONT);
			GtkLabel18.ModifyFg(StateType.Normal, colorConverter.FONT);
			lbFile.ModifyFg(StateType.Normal, colorConverter.FONT);

			SetLanguageToGui ();

			// set path and filename
			int lastDirectorySeparator = bitmap.FileName.LastIndexOf(IOPath.DirectorySeparatorChar);
			if (lastDirectorySeparator == -1) {
				lastDirectorySeparator = bitmap.FileName.LastIndexOf(IOPath.AltDirectorySeparatorChar);
			}
			int lastDot = bitmap.FileName.LastIndexOf('.');
			int fileNameLength = lastDot - (lastDirectorySeparator + 1);
			entryFilename.Text = bitmap.FileName.Substring (lastDirectorySeparator + 1, fileNameLength);
			// will also change config.Path, caused of delegate 'OnHyperTextLabelTextChanged'
			htLabelDirectory.Text = bitmap.FileName.Substring(0, lastDirectorySeparator);
			// config.Path = htLabelDirectory.Text;



			switch (b.OrigFormat) 
			{
			case TroonieImageFormat.PNG1:
				rdPng1bit.Active = true;
				OnRdPng1bitToggled (rdPng1bit, null);
				break;
			case TroonieImageFormat.PNG8:
				rdPng8Bit.Active = true;
				OnRdPng8BitToggled (rdPng8Bit, null);
				break;
			case TroonieImageFormat.PNG24:
				rdPng24Bit.Active = true;
				OnRdPng24BitToggled (rdPng24Bit, null);
				break;
			case TroonieImageFormat.PNG32Transparency:
				hbox6.Visible = true;  // replacing alpha with color
				btnReplaceAlphaColor.Sensitive = !rdPng32BitAlphaAsValue.Active;
				rdPNG32bit.Active = true;
				OnRdPNG32bitToggled (rdPNG32bit, null);
				break;
			case TroonieImageFormat.PNG32AlphaAsValue:
				hbox6.Visible = true;  // replacing alpha with color
				rdPng32BitAlphaAsValue.Active = true;
				OnRdPng32BitAlphaAsValueToggled (rdPng32BitAlphaAsValue, null);
				break;
			case TroonieImageFormat.JPEG8:
				rdJpegGray.Active = true;
				OnRdJpegGrayToggled (rdJpegGray, null);				
				break;
			case TroonieImageFormat.JPEG24:
				rdJpeg.Active = true;
				OnRdJpegToggled (rdJpeg, null);				
				break;
			case TroonieImageFormat.BMP1:
				rdBmp1bit.Active = true;
				OnRdBmp1bitToggled (rdBmp1bit, null);
				break;
			case TroonieImageFormat.BMP8:
				rdBmp8bit.Active = true;
				OnRdBmp8bitToggled (rdBmp8bit, null);
				break;
			case TroonieImageFormat.BMP24:
				rdBmp24bit.Active = true;
				OnRdBmp24bitToggled (rdBmp24bit, null);		
				break;
			case TroonieImageFormat.TIFF:
				rdTiff.Active = true;
				OnRdTiffToggled (rdTiff, null);				
				break;
			case TroonieImageFormat.GIF:
				rdGif.Active = true;
				OnRdGifToggled (rdGif, null);				
				break;
			case TroonieImageFormat.WMF:
				rdWmf.Active = true;
				OnRdWmfToggled (rdWmf, null);				
				break;
			case TroonieImageFormat.EMF:
				rdEmf.Active = true;
				OnRdEmfToggled (rdEmf, null);				
				break;
			case TroonieImageFormat.ICO:
				rdIcon.Active = true;
				OnRdIconToggled (rdIcon, null);				
				break;
			default:
				throw new NotSupportedException (Language.I.L[49]);
			}

			if (Constants.I.WINDOWS) {
				rdPng1bit.Sensitive = true;
				rdBmp1bit.Sensitive = true;
			} else {
				// Original is ShadowType.EtchedIn, but linux cannot draw it correctly.
				// Otherwise ShadowType.In looks terrible at Win10.
				frame3.ShadowType = ShadowType.In;
				frame4.ShadowType = ShadowType.In;
				frame5.ShadowType = ShadowType.In;
				frame6.ShadowType = ShadowType.In;
				frame7.ShadowType = ShadowType.In;

				//rdJpegGray.Sensitive = true;
				frame3.Sensitive = Constants.I.CJPEG;

				if (!Constants.I.CJPEG && (rdJpeg.Active || rdJpegGray.Active)) {
					rdPng24Bit.Active = true;
				}
			}		
		}

		private void OnHyperTextLabelTextChanged()
		{
			// Current.Path = htlbOutputDirectory.Text;
			config.Path = htLabelDirectory.Text;
		}	

		private void SetLanguageToGui()
		{
			this.Title = Language.I.L [3];

			lbFile.LabelProp = "<b>" + Language.I.L[46] + "</b>";
			lbDirectoryText.Text = Language.I.L[47];
			label3.Text = Language.I.L[48];

			rdJpeg.Label = "JPEG (24 Bit " + Language.I.L[22] + ")";
			rdJpegGray.Label = "JPEG (8 Bit " + Language.I.L[21] + ")";
			lbQuality.Text = Language.I.L[23];

			rdPng1bit.Label = "PNG (1 Bit " + Language.I.L[24] + ")";
			rdPng8Bit.Label = "PNG (8 Bit " + Language.I.L[21] + ")";
			rdPng24Bit.Label = "PNG (24 Bit " + Language.I.L[22] + ")";
			rdPNG32bit.Label = "PNG (32 Bit " + Language.I.L[25] + ")";
			rdPng32BitAlphaAsValue.Label = "PNG (32 Bit " + Language.I.L[79] + ")";
			lbTransparencyColor.Text = Language.I.L[26];
			checkBtnReplaceAlpha.Label = Language.I.L[301];

			rdBmp1bit.Label = "BMP (1 Bit " + Language.I.L[24] + ")";
			rdBmp8bit.Label = "BMP (8 Bit " + Language.I.L[21] + ")";
			rdBmp24bit.Label = "BMP (24 Bit " + Language.I.L[22] + ")";
			GtkLabel18.LabelProp = "<b>" + Language.I.L[27] + "</b>";
			buttonOk.Label = Language.I.L[16];
			buttonCancel.Label = Language.I.L[17];
		}

		public bool Process()
		{
			entryFilename.Text = Regex.Replace(entryFilename.Text, @"[\\/:?*^""<>|]", "_");

			if (FileHelper.I.Exists (SavedFileName)) 
			{
				MessageDialog md = new MessageDialog (this, 
					DialogFlags.DestroyWithParent, MessageType.Question, 
					ButtonsType.OkCancel, Language.I.L[50]);
				if (md.Run () == (int)ResponseType.Cancel) {
					md.Destroy ();
					return false;
				}
				md.Destroy ();
				config.FileOverwriting = true;
			}		

			bool success;
			success = bitmap.Save (config, entryFilename.Text + lbFormat.Text, true);
			return success;
		}

		public void AllowOnlyColorLoselessSaving()
		{
			onlyColorLosslessSaving = true;

			hbox6.Visible = false; // replacing alpha with color
			rdPng1bit.Sensitive = false;
			rdPng8Bit.Sensitive = false;
			rdPNG32bit.Sensitive = false;
			rdBmp1bit.Sensitive = false;
			rdBmp8bit.Sensitive = false;
			// frame3.Sensitive = false; // jpg frame
			rdJpeg.Label = Language.I.L[250];
			rdJpegGray.Sensitive = false;
			lbQuality.Visible = false;
			hscaleQuality.Visible = false;

			//			frame6.Sensitive = false; // other
			rdGif.Sensitive = false;
			rdIcon.Sensitive = false;

			rdPng24Bit.Active = true;
		}

		#region RadioButton toggle events

		private void SetToggledProperties(object sender, TroonieImageFormat f)
		{
			var pair = Constants.Extensions.First(x => x.Key == f);

			if (((RadioButton)sender).Active) {
				config.Format = f;
				lbFormat.Text = pair.Value.Item1; 
			}				
		}

		protected void OnRdJpegToggled (object sender, EventArgs e)
		{
			hscaleQuality.Sensitive = rdJpeg.Active;
			lbQuality.Sensitive = rdJpeg.Active;

			SetToggledProperties (sender, onlyColorLosslessSaving ? TroonieImageFormat.JPEGLOSSLESS : TroonieImageFormat.JPEG24);
		}

		protected void OnRdJpegGrayToggled (object sender, EventArgs e)
		{
			hscaleQuality.Sensitive = rdJpegGray.Active;
			lbQuality.Sensitive = rdJpegGray.Active;

			SetToggledProperties (sender, TroonieImageFormat.JPEG8);
		}

		protected void OnRdPng1bitToggled (object sender, EventArgs e)
		{
			SetToggledProperties (sender, TroonieImageFormat.PNG1);
		}

		protected void OnRdPng8BitToggled (object sender, EventArgs e)
		{			
			SetToggledProperties (sender, TroonieImageFormat.PNG8);
		}						

		protected void OnRdPng24BitToggled (object sender, EventArgs e)
		{
			SetToggledProperties (sender, TroonieImageFormat.PNG24);
		}

		protected void OnRdPNG32bitToggled (object sender, EventArgs e)
		{
			lbTransparencyColor.Sensitive = rdPNG32bit.Active;
			btnColor.Sensitive = rdPNG32bit.Active;

			checkBtnReplaceAlpha.Sensitive = !rdPNG32bit.Active;
			btnReplaceAlphaColor.Sensitive = !rdPNG32bit.Active;
			if (rdPNG32bit.Active) {
				checkBtnReplaceAlpha.Active = false;
			}

			SetToggledProperties (sender, TroonieImageFormat.PNG32Transparency);
		}

		protected void OnRdPng32BitAlphaAsValueToggled (object sender, EventArgs e)
		{
			checkBtnReplaceAlpha.Sensitive = !rdPng32BitAlphaAsValue.Active;
			btnReplaceAlphaColor.Sensitive = !rdPng32BitAlphaAsValue.Active;
			if (rdPng32BitAlphaAsValue.Active) {
				checkBtnReplaceAlpha.Active = false;
			}

			SetToggledProperties (sender, TroonieImageFormat.PNG32AlphaAsValue);
		}

		protected void OnRdBmp1bitToggled (object sender, EventArgs e)
		{
			SetToggledProperties (sender, TroonieImageFormat.BMP1);
		}

		protected void OnRdBmp8bitToggled (object sender, EventArgs e)
		{
			SetToggledProperties (sender, TroonieImageFormat.BMP8);
		}

		protected void OnRdBmp24bitToggled (object sender, EventArgs e)
		{
			SetToggledProperties (sender, TroonieImageFormat.BMP24);
		}	

		protected void OnRdTiffToggled (object sender, EventArgs e)
		{
			SetToggledProperties (sender, TroonieImageFormat.TIFF);
		}

		protected void OnRdGifToggled (object sender, EventArgs e)
		{
			SetToggledProperties (sender, TroonieImageFormat.GIF);
		}

		protected void OnRdWmfToggled (object sender, EventArgs e)
		{
			SetToggledProperties (sender, TroonieImageFormat.WMF);
		}

		protected void OnRdEmfToggled (object sender, EventArgs e)
		{
			SetToggledProperties (sender, TroonieImageFormat.EMF);
		}

		protected void OnRdIconToggled (object sender, EventArgs e)
		{
			SetToggledProperties (sender, TroonieImageFormat.ICO);
		}

		#endregion RadioButton Toggle events

		protected void OnHscaleQualityValueChanged (object sender, EventArgs e)
		{
			config.JpgQuality = (byte)hscaleQuality.Value;
		}

		protected void OnBtnColorColorSet (object sender, EventArgs e)
		{			
			// transparencyColor = btnColor.Color;
			byte r, g, b;
			ColorConverter.Instance.ToDotNetColor (btnColor.Color, out r, out g, out b);

			config.TransparencyColorRed = r;
			config.TransparencyColorGreen = g;
			config.TransparencyColorBlue = b;
		}	

		protected void OnBtnReplaceAlphaColorSet (object sender, EventArgs e)
		{			
			// transparencyColor = btnColor.Color;
			byte r, g, b;
			ColorConverter.Instance.ToDotNetColor (btnReplaceAlphaColor.Color, out r, out g, out b);

			config.ReplaceTransparencyColorRed = r;
			config.ReplaceTransparencyColorGreen = g;
			config.ReplaceTransparencyColorBlue = b;
		}

		[GLib.ConnectBefore ()] 
		protected void OnKeyPressEvent (object o, KeyPressEventArgs args)
		{
			//			 System.Console.WriteLine("Keypress: {0}", args.Event.Key);
			switch (args.Event.Key) {
			case Gdk.Key.Return:
				this.Respond (ResponseType.Ok);
				break;
			case Gdk.Key.Escape:
				this.Respond (ResponseType.Cancel);
				break;
			}
		}

		protected void OnCheckBtnReplaceAlphaToggled (object sender, EventArgs e)
		{
			config.ReplacingTransparencyWithColor = checkBtnReplaceAlpha.Active;
		}
	}
}
