using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Troonie_Lib
{
	/// <summary>
	/// HSL filter by manipulating H, S, L values. 
	/// </summary>
	/// <remarks>
	/// Some code fragments (Hsl converting) from © Rod Stephens, 2016
	/// http://csharphelper.com/blog/2016/08/convert-between-rgb-and-hls-color-models-in-c/

	/// </remarks>
	public class HslFilter : AbstractFilter
    {
		/// <summary>Summand for hue. Value: [0, 360]. </summary>
		public int AddHue { get; set; }

		/// <summary>Summand for saturation. Value: [-1.0, 1.0]. </summary>
		public double AddSaturation { get; set; }

		/// <summary>Summand for lightness. Value: [-1.0, 1.0]. </summary>
		public double AddLightness { get; set; }

		public HslFilter ()
        {
            SupportedSrcPixelFormat = PixelFormatFlags.Color;
			SupportedDstPixelFormat = PixelFormatFlags.SameLikeSource;
			AddHue = 0;
			AddSaturation = 0;
			AddLightness = 0;
		}
	    
        #region protected methods

		protected override void SetProperties (double[] filterProperties)
		{
			AddHue = (int)filterProperties [3];
			AddSaturation = filterProperties [4];
			AddLightness = filterProperties [5];
		}

        protected internal override unsafe void Process(
            BitmapData srcData, BitmapData dstData)
        {
			int ps = Image.GetPixelFormatSize(srcData.PixelFormat) / 8;
			int w = srcData.Width;
			int h = srcData.Height;
			int offset = srcData.Stride - w * ps;

			byte* src = (byte*)srcData.Scan0.ToPointer();
            byte* dst = (byte*)dstData.Scan0.ToPointer();

            // for each line
            for (int y = 0; y < h; y++)
            {
                // for each pixel
                for (int x = 0; x < w; x++, src += ps, dst += ps)
                {       					
					int r, g, b;
					double hue, sat, lig;
					RgbToHsl (src [RGBA.R], src [RGBA.G], src [RGBA.B], out hue, out sat, out lig);

					sat += AddSaturation;
					sat = Math.Max (0, sat);
					sat = Math.Min (1, sat);

					lig += AddLightness;
					lig = Math.Max (0, lig);
					lig = Math.Min (1, lig);

					HslToRgb (hue + AddHue, sat, lig, out r, out g, out b);
					dst [RGBA.R] = (byte)r;
					dst [RGBA.G] = (byte)g;
					dst [RGBA.B] = (byte)b;

					// alpha, 32 bit
					if (ps == 4) {
						dst[RGBA.A] = Use255ForAlpha ? (byte)255 : src[RGBA.A];
					}
                }
                src += offset;
                dst += offset;
            }
        }

		#endregion protected methods 

		// © Rod Stephens, 2016
		// source: http://csharphelper.com/blog/2016/08/convert-between-rgb-and-hls-color-models-in-c/
		#region HSl converting code

		// Convert an RGB value into an HSL value.
		private static void RgbToHsl (int r, int g, int b, out double h, out double s, out double l)
		{
			// Convert RGB to a 0.0 to 1.0 range.
			double double_r = r / 255.0;
			double double_g = g / 255.0;
			double double_b = b / 255.0;

			// Get the maximum and minimum RGB components.
			double max = double_r;
			if (max < double_g) max = double_g;
			if (max < double_b) max = double_b;

			double min = double_r;
			if (min > double_g) min = double_g;
			if (min > double_b) min = double_b;

			double diff = max - min;
			l = (max + min) / 2;
			if (Math.Abs (diff) < 0.00001) {
				s = 0;
				h = 0;  // H is really undefined.
			} else {
				if (l <= 0.5) s = diff / (max + min);
				else s = diff / (2 - max - min);

				double r_dist = (max - double_r) / diff;
				double g_dist = (max - double_g) / diff;
				double b_dist = (max - double_b) / diff;

				if (double_r == max) h = b_dist - g_dist;
				else if (double_g == max) h = 2 + r_dist - b_dist;
				else h = 4 + g_dist - r_dist;

				h = h * 60;
				if (h < 0) h += 360;
			}
		}

		// Convert an HSL value into an RGB value.
		private static void HslToRgb (double h, double s, double l, out int r, out int g, out int b)
		{
			double p2;
			if (l <= 0.5) p2 = l * (1 + s);
			else p2 = l + s - l * s;

			double p1 = 2 * l - p2;
			double double_r, double_g, double_b;
			if (s == 0) {
				double_r = l;
				double_g = l;
				double_b = l;
			} else {
				double_r = QqhToRgb (p1, p2, h + 120);
				double_g = QqhToRgb (p1, p2, h);
				double_b = QqhToRgb (p1, p2, h - 120);
			}

			// Convert RGB to the 0 to 255 range.
			r = (int)(double_r * 255.0);
			g = (int)(double_g * 255.0);
			b = (int)(double_b * 255.0);
		}

		private static double QqhToRgb (double q1, double q2, double hue)
		{
			if (hue > 360) hue -= 360;
			else if (hue < 0) hue += 360;

			if (hue < 60) return q1 + (q2 - q1) * hue / 60;
			if (hue < 180) return q2;
			if (hue < 240) return q1 + (q2 - q1) * (240 - hue) / 60;
			return q1;
		}
		#endregion
	}
}
