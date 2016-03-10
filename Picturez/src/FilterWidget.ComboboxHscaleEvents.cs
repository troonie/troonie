using System;
using Gtk;
using Picturez_Lib;

namespace Picturez
{
	public partial class FilterWidget
	{
		#region GaussianBlur
	
		private bool GaussianBlur_HscaleTimeoutHandler()
		{
			gaussianBlur.Sigma = hscale1.Value;
			gaussianBlur.Size = (int) hscale2.Value;
			ProcessPreview ();

			repeatTimeout = false;
			return false;
		}

		protected void GaussianBlur_Hscale1ChangeValue (object sender, ChangeValueArgs args)
		{
			if (repeatTimeout)
				return;

			repeatTimeout = true;
			GLib.Timeout.Add(timeoutInterval, new GLib.TimeoutHandler(GaussianBlur_HscaleTimeoutHandler));
		}

		protected void GaussianBlur_Hscale2ChangeValue (object sender, ChangeValueArgs args)
		{
			if (repeatTimeout)
				return;

			repeatTimeout = true;
			GLib.Timeout.Add(timeoutInterval, new GLib.TimeoutHandler(GaussianBlur_HscaleTimeoutHandler));
		}

		#endregion GaussianBlur

		#region ExtractOrRotateChannels, Grayscale

		protected void ExtractOrRotateChannels_Combobox1Changed (object sender, EventArgs e)
		{
			//			Console.WriteLine ("Changed index: " + combobox1.Active);
			extractOrRotateChannels.Order = (ExtractOrRotateChannelsFilter.RGBOrder) combobox1.Active;
			ProcessPreview ();
		}

		protected void Grayscale_Combobox1Changed (object sender, EventArgs e)
		{
			grayscale.Algorithm = (GrayscaleFilter.CommonAlgorithms) combobox1.Active;
			ProcessPreview ();
		}

		#endregion ExtractOrRotateChannels, Grayscale
	}
}

