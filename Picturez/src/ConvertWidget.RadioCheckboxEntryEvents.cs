using System;
using Gtk;
using Picturez_Lib;

namespace Picturez
{	
	public partial class ConvertWidget
	{
		#region RadioButton image format toggle events

		private void SetToggledProperties(object sender, PicturezImageFormat f, string s)
		{
			if (((RadioButton)sender).Active) {
				Current.Format = f;
				format = s;
			}				
		}

		protected void OnRdJpegToggled (object sender, EventArgs e)
		{
			hscaleQuality.Sensitive = rdJpeg.Active;
			lbQuality.Sensitive = rdJpeg.Active;

			SetToggledProperties (sender, PicturezImageFormat.JPEG24, ".jpg");
		}

		protected void OnRdJpegGrayToggled (object sender, EventArgs e)
		{
			hscaleQuality.Sensitive = rdJpegGray.Active;
			lbQuality.Sensitive = rdJpegGray.Active;

			SetToggledProperties (sender, PicturezImageFormat.JPEG8, ".jpg");
		}

		protected void OnRdPng1bitToggled (object sender, EventArgs e)
		{
			SetToggledProperties (sender, PicturezImageFormat.PNG1, ".png");
		}

		protected void OnRdPng8BitToggled (object sender, EventArgs e)
		{			
			SetToggledProperties (sender, PicturezImageFormat.PNG8, ".png");
		}						

		protected void OnRdPng24BitToggled (object sender, EventArgs e)
		{
			SetToggledProperties (sender, PicturezImageFormat.PNG24, ".png");
		}

		protected void OnRdPNG32bitToggled (object sender, EventArgs e)
		{
			lbTransparencyColor.Sensitive = rdPNG32bit.Active;
			btnColor.Sensitive = rdPNG32bit.Active;

			SetToggledProperties (sender, PicturezImageFormat.PNG32Transparency, ".png");
		}

		protected void OnRdPng32BitAlphaAsValueToggled (object sender, EventArgs e)
		{
			SetToggledProperties (sender, PicturezImageFormat.PNG32AlphaAsValue, ".png");
		}

		protected void OnRdBmp1bitToggled (object sender, EventArgs e)
		{
			SetToggledProperties (sender, PicturezImageFormat.BMP1, ".bmp");
		}

		protected void OnRdBmp8bitToggled (object sender, EventArgs e)
		{
			SetToggledProperties (sender, PicturezImageFormat.BMP8, ".bmp");
		}

		protected void OnRdBmp24bitToggled (object sender, EventArgs e)
		{
			SetToggledProperties (sender, PicturezImageFormat.BMP24, ".bmp");
		}	

		protected void OnRdTiffToggled (object sender, EventArgs e)
		{
			SetToggledProperties (sender, PicturezImageFormat.TIFF, ".tif");
		}

		protected void OnRdGifToggled (object sender, EventArgs e)
		{
			SetToggledProperties (sender, PicturezImageFormat.GIF, ".gif");
		}

		protected void OnRdWmfToggled (object sender, EventArgs e)
		{
			SetToggledProperties (sender, PicturezImageFormat.WMF, ".wmf");
		}

		protected void OnRdEmfToggled (object sender, EventArgs e)
		{
			SetToggledProperties (sender, PicturezImageFormat.EMF, ".emf");
		}

		protected void OnRdIconToggled (object sender, EventArgs e)
		{
			SetToggledProperties (sender, PicturezImageFormat.ICO, ".ico");
		}			

		#endregion RadioButton image format toggle events

		#region RadioButton image resize and output directory toggle events
		protected void OnRdOriginalSizeToggled (object sender, EventArgs e)
		{
			if (rdOriginalSize.Active) {
				Current.ResizeVersion = ResizeVersion.No;
			}
		}

		protected void OnRdBiggerLengthToggled (object sender, EventArgs e)
		{
			entryBiggerLength.Sensitive = rdBiggerLength.Active;
			lbPixel_BiggerLength.Sensitive = rdBiggerLength.Active;

			if (rdBiggerLength.Active) {
				Current.ResizeVersion = ResizeVersion.BiggestLength;
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
				Current.ResizeVersion = ResizeVersion.FixedSize;
			}
		}

		#endregion RadioButton image resize and output directory toggle events

		#region Checkbox AND convert button toogle events
		protected void OnCheckButtonsToggled (object sender, EventArgs e)
		{
			Current.StretchImage = checkBtnStretch.Active ? ConvertMode.StretchForge : ConvertMode.NoStretchForge;
			Current.FileOverwriting = checkBtnOverwriteOriginalImage.Active;
			Current.UseOriginalPath = checkBtnUseOriginalDirectory.Active;
			htlbOutputDirectory.Sensitive = !Current.UseOriginalPath;
		}			

		#endregion Checkbox AND convert button toogle events

		#region Entry changed events

		protected void OnEntryBiggerLengthTextInserted (object o, TextInsertedArgs args)
		{
			int number;
			if (int.TryParse (entryBiggerLength.Text, out number)) {
				Current.BiggestLength = number;
			} else {
				entryBiggerLength.DeleteText (entryBiggerLength.CursorPosition, entryBiggerLength.CursorPosition + 1);
			}
		}

		protected void OnEntryFixSizeHeightTextInserted (object o, TextInsertedArgs args)
		{
			int number;
			if (int.TryParse (entryFixSizeHeight.Text, out number)) {
				Current.Height = number;
			} else {
				entryFixSizeHeight.DeleteText (entryFixSizeHeight.CursorPosition, entryFixSizeHeight.CursorPosition + 1);
			}
		}

		protected void OnEntryFixSizeWidthTextInserted (object o, TextInsertedArgs args)
		{
			int number;
			if (int.TryParse (entryFixSizeWidth.Text, out number)) {
				Current.Width = number;
			} else {
				entryFixSizeWidth.DeleteText (entryFixSizeWidth.CursorPosition, entryFixSizeWidth.CursorPosition + 1);
			}
		}
		#endregion Entry changed events
	}
}

