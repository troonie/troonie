using System;
using Gtk;
using Troonie_Lib;

namespace Troonie
{	
	public partial class ConvertWidget
	{
		#region RadioButton image format toggle events

		private void SetToggledProperties(object sender, TroonieImageFormat f, string s)
		{
			if (((RadioButton)sender).Active) {
				config.Format = f;
				format = s;
			}				
		}

		protected void OnRdJpegToggled (object sender, EventArgs e)
		{
			hscaleQuality.Sensitive = rdJpeg.Active;
			lbQuality.Sensitive = rdJpeg.Active;

			SetToggledProperties (sender, TroonieImageFormat.JPEG24, ".jpg");
		}

		protected void OnRdJpegGrayToggled (object sender, EventArgs e)
		{
			hscaleQuality.Sensitive = rdJpegGray.Active;
			lbQuality.Sensitive = rdJpegGray.Active;

			SetToggledProperties (sender, TroonieImageFormat.JPEG8, ".jpg");
		}

		protected void OnRdPng1bitToggled (object sender, EventArgs e)
		{
			SetToggledProperties (sender, TroonieImageFormat.PNG1, ".png");
		}

		protected void OnRdPng8BitToggled (object sender, EventArgs e)
		{			
			SetToggledProperties (sender, TroonieImageFormat.PNG8, ".png");
		}						

		protected void OnRdPng24BitToggled (object sender, EventArgs e)
		{
			SetToggledProperties (sender, TroonieImageFormat.PNG24, ".png");
		}

		protected void OnRdPNG32bitToggled (object sender, EventArgs e)
		{
			lbTransparencyColor.Sensitive = rdPNG32bit.Active;
			btnColor.Sensitive = rdPNG32bit.Active;

			SetToggledProperties (sender, TroonieImageFormat.PNG32Transparency, ".png");
		}

		protected void OnRdPng32BitAlphaAsValueToggled (object sender, EventArgs e)
		{
			SetToggledProperties (sender, TroonieImageFormat.PNG32AlphaAsValue, ".png");
		}

		protected void OnRdBmp1bitToggled (object sender, EventArgs e)
		{
			SetToggledProperties (sender, TroonieImageFormat.BMP1, ".bmp");
		}

		protected void OnRdBmp8bitToggled (object sender, EventArgs e)
		{
			SetToggledProperties (sender, TroonieImageFormat.BMP8, ".bmp");
		}

		protected void OnRdBmp24bitToggled (object sender, EventArgs e)
		{
			SetToggledProperties (sender, TroonieImageFormat.BMP24, ".bmp");
		}	

		protected void OnRdTiffToggled (object sender, EventArgs e)
		{
			SetToggledProperties (sender, TroonieImageFormat.TIFF, ".tif");
		}

		protected void OnRdGifToggled (object sender, EventArgs e)
		{
			SetToggledProperties (sender, TroonieImageFormat.GIF, ".gif");
		}

		protected void OnRdWmfToggled (object sender, EventArgs e)
		{
			SetToggledProperties (sender, TroonieImageFormat.WMF, ".wmf");
		}

		protected void OnRdEmfToggled (object sender, EventArgs e)
		{
			SetToggledProperties (sender, TroonieImageFormat.EMF, ".emf");
		}

		protected void OnRdIconToggled (object sender, EventArgs e)
		{
			SetToggledProperties (sender, TroonieImageFormat.ICO, ".ico");
		}			

		#endregion RadioButton image format toggle events

		#region RadioButton image resize and output directory toggle events
		protected void OnRdOriginalSizeToggled (object sender, EventArgs e)
		{
			if (rdOriginalSize.Active) {
				config.ResizeVersion = ResizeVersion.No;
			}
		}

		protected void OnRdBiggerLengthToggled (object sender, EventArgs e)
		{
			entryBiggerLength.Sensitive = rdBiggerLength.Active;
			lbPixel_BiggerLength.Sensitive = rdBiggerLength.Active;

			if (rdBiggerLength.Active) {
				config.ResizeVersion = ResizeVersion.BiggestLength;
			}
		}

		protected void OnRdFixSizeToggled (object sender, EventArgs e)
		{
			entryFixSizeWidth.Sensitive = rdFixSize.Active;
			entryFixSizeHeight.Sensitive = rdFixSize.Active;
			checkBtnStretch.Sensitive = rdFixSize.Active;
			lbPixel_FixSize.Sensitive = rdFixSize.Active;
			lbX.Sensitive = rdFixSize.Active;

			if (rdFixSize.Active) {
				config.ResizeVersion = ResizeVersion.FixedSize;
			}
		}

		#endregion RadioButton image resize and output directory toggle events

		#region Checkbox AND convert button toogle events
		protected void OnCheckButtonsToggled (object sender, EventArgs e)
		{
			if (isSettingGuiToCurrentConfiguration)
				return;
			config.StretchImage = checkBtnStretch.Active ? ConvertMode.StretchForge : ConvertMode.NoStretchForge;
			config.FileOverwriting = checkBtnOverwriteOriginalImage.Active;
			config.UseOriginalPath = checkBtnUseOriginalDirectory.Active;
			htlbOutputDirectory.Sensitive = !config.UseOriginalPath;
		}			

		#endregion Checkbox AND convert button toogle events

		#region Entry changed events

		protected void OnEntryBiggerLengthTextInserted (object o, TextInsertedArgs args)
		{
			int number;
			if (int.TryParse (entryBiggerLength.Text, out number)) {
				config.BiggestLength = number;
			} else {
				entryBiggerLength.DeleteText (entryBiggerLength.CursorPosition, entryBiggerLength.CursorPosition + 1);
			}
		}

		protected void OnEntryFixSizeHeightTextInserted (object o, TextInsertedArgs args)
		{
			int number;
			if (int.TryParse (entryFixSizeHeight.Text, out number)) {
				config.Height = number;
			} else {
				entryFixSizeHeight.DeleteText (entryFixSizeHeight.CursorPosition, entryFixSizeHeight.CursorPosition + 1);
			}
		}

		protected void OnEntryFixSizeWidthTextInserted (object o, TextInsertedArgs args)
		{
			int number;
			if (int.TryParse (entryFixSizeWidth.Text, out number)) {
				config.Width = number;
			} else {
				entryFixSizeWidth.DeleteText (entryFixSizeWidth.CursorPosition, entryFixSizeWidth.CursorPosition + 1);
			}
		}
		#endregion Entry changed events
	}
}

