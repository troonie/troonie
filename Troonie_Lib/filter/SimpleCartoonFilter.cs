using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Troonie_Lib
{
    /// <summary>
	/// Simple Cartoon filter.
    /// </summary>
	public class SimpleCartoonFilter : AbstractFilter
    {
        /// <summary>Range of hue of HSL colors in cartoon image. Default: 60.</summary> 
        public int HueRange { get; set; }
		/// <summary>Range of Saturation of HSL colors in cartoon image. Default: 20.</summary> 
		public int SaturationRange { get; set; }
		/// <summary>Range of lightness of HSL colors in cartoon image. Default: 20.</summary> 
		public int LightnessRange { get; set; }
				
		public SimpleCartoonFilter()
        {
            SupportedSrcPixelFormat = PixelFormatFlags.All;
			SupportedDstPixelFormat = PixelFormatFlags.SameLikeSource;
			HueRange = 36;
			SaturationRange = 10;
			LightnessRange = 10;
        }

        #region protected methods

		protected override void SetProperties (double[] filterProperties)
		{
			HueRange = (int)filterProperties [3];
			SaturationRange = (int)filterProperties [4];
			LightnessRange = (int)filterProperties [5];
		}

        protected internal override unsafe void Process(
            BitmapData srcData, BitmapData dstData)
        {
			SobelEdgeMarkerFilter sobel = new SobelEdgeMarkerFilter ();
			sobel.Process (srcData, dstData);

//			double rangeSize = 255.0 / HueRange;

			int ps = Image.GetPixelFormatSize(dstData.PixelFormat) / 8;
			int w = dstData.Width;
			int h = dstData.Height;
			int offset = dstData.Stride - w * ps;

			byte* src = (byte*)dstData.Scan0.ToPointer();
            byte* dst = (byte*)dstData.Scan0.ToPointer();

            // for each line
            for (int y = 0; y < h; y++)
            {
                // for each pixel
                for (int x = 0; x < w; x++, src += ps, dst += ps)
                {       					
					byte r,g,b;
					// 8 bit grayscale
					if (ps == 8) {
						r = src [RGBA.B]; 
						g = src [RGBA.B];
						b = src [RGBA.B];
						// when black, break it here
						if (r == 0) {
							continue;
						}
						CalcCartoonColorOnlyHue (ref r, ref g, ref b);
						int avg = (r + g + b) / 3;
						dst [RGBA.B] = (byte) avg;
					}

					// rgb, 24 and 32 bit
					if (ps >= 3) {
						r = src [RGBA.R]; 
						g = src [RGBA.G];
						b = src [RGBA.B];

						// when black, break it here
						if (r == 0 && g == 0 && b == 0) {
							continue;
						}
							
						CalcCartoonColorOnlyHue (ref r, ref g, ref b);
						dst [RGBA.R] = r;
						dst [RGBA.G] = g;
						dst [RGBA.B] = b;
					}

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

		[Obsolete]
		private void CalcCartoonColor(ref byte r, ref byte g, ref byte b)
		{
			double d_h,d_s,d_l;
			ColorRgbHsl.I.RGB2HSL(r, g, b, out d_h, out d_s, out d_l);

			int i_h = (int)Math.Round(d_h * 360);
			i_h = i_h + HueRange - i_h % HueRange;
			int i_s = (int)Math.Round(d_s * 100);
			i_s = i_s + SaturationRange - i_s % SaturationRange;
			int i_l = (int)Math.Round(d_l * 100);
			i_l = i_l + LightnessRange - i_l % LightnessRange;

			d_h = i_h / 360.0;
			d_s = i_s / 100.0;
			d_l = i_l / 100.0;
			ColorRgbHsl.I.HSL2RGB (d_h, d_s, d_l, out r, out g, out b);
		}

		private void CalcCartoonColorOnlyHue(ref byte r, ref byte g, ref byte b)
		{
			const int Modulo_S_L = 25;
			double d_h,d_s,d_l;
			ColorRgbHsl.I.RGB2HSL(r, g, b, out d_h, out d_s, out d_l);

//			int i_h = (int)Math.Round(d_h * 360);
//			i_h = i_h + HueRange - i_h % HueRange;

			int i_s = (int)Math.Round(d_s * 100);
			i_s = i_s + Modulo_S_L - i_s % Modulo_S_L;

			int i_l = (int)Math.Round(d_l * 100);
			i_l = i_l + Modulo_S_L - i_l % Modulo_S_L;

//			d_h = i_h / 360.0;
			d_s = i_s / 100.0;
			d_l = i_l / 100.0;
			ColorRgbHsl.I.HSL2RGB (d_h, d_s, d_l, out r, out g, out b);
		}
    }
}
