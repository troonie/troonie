using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Troonie_Lib
{
    /// <summary>
	/// Simple Cartoon filter.
    /// </summary>
	public class CartoonFilter : AbstractFilter
    {			
		public CartoonFilter()
        {
            SupportedSrcPixelFormat = PixelFormatFlags.All;
			SupportedDstPixelFormat = PixelFormatFlags.SameLikeSource;
	    }

		public override Bitmap Apply(Bitmap source, double[] filterproperties)
		{			
			CheckPixelFormat(source.PixelFormat);
			SetProperties (filterproperties);

			SobelEdgeMarkerFilter semf = new SobelEdgeMarkerFilter ();
			Bitmap sobelEdgeMarker = semf.Apply (source/*, new []{ 0.0, 0, 0, 127.0 } */);

			Bitmap destination = new Bitmap(sobelEdgeMarker.Width, sobelEdgeMarker.Height, dstPixelFormat);
			Rectangle rect = new Rectangle(0, 0, sobelEdgeMarker.Width, sobelEdgeMarker.Height);
			BitmapData srcData = sobelEdgeMarker.LockBits(rect, ImageLockMode.ReadWrite, sobelEdgeMarker.PixelFormat);
			BitmapData dstData = destination.LockBits(rect, ImageLockMode.ReadWrite, destination.PixelFormat);
			Process(srcData, dstData);
			destination.UnlockBits(dstData);
			sobelEdgeMarker.UnlockBits(srcData);

			if (destination.PixelFormat == PixelFormat.Format8bppIndexed)
			{
				SetColorPalette(destination);
			}

			return destination;
		}

        #region protected methods

		protected override void SetProperties (double[] filterProperties)
		{
	
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
					byte r,g,b;
					// 8 bit grayscale
					if (ps == 1) {
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

//		private void CalcCartoonColor(ref byte r, ref byte g, ref byte b)
//		{
//			double d_h,d_s,d_l;
//			ColorRgbHsl.I.RGB2HSL(r, g, b, out d_h, out d_s, out d_l);
//
//			int i_h = (int)Math.Round(d_h * 360);
//			i_h = i_h + HueRange - i_h % HueRange;
//			int i_s = (int)Math.Round(d_s * 100);
//			i_s = i_s + SaturationRange - i_s % SaturationRange;
//			int i_l = (int)Math.Round(d_l * 100);
//			i_l = i_l + LightnessRange - i_l % LightnessRange;
//
//			d_h = i_h / 360.0;
//			d_s = i_s / 100.0;
//			d_l = i_l / 100.0;
//			ColorRgbHsl.I.HSL2RGB (d_h, d_s, d_l, out r, out g, out b);
//		}

		private void CalcCartoonColorOnlyHue(ref byte r, ref byte g, ref byte b)
		{
			const int Modulo_S_L = 25;
			double d_h,d_s,d_l;
			ColorRgbHsl.I.RGB2HSL(r, g, b, out d_h, out d_s, out d_l);

			int i_s = (int)Math.Round(d_s * 100);
			i_s = i_s + Modulo_S_L - i_s % Modulo_S_L;
			int i_l = (int)Math.Round(d_l * 100);
			i_l = i_l + Modulo_S_L - i_l % Modulo_S_L;

			d_s = i_s / 100.0;
			d_l = i_l / 100.0;
			ColorRgbHsl.I.HSL2RGB (d_h, d_s, d_l, out r, out g, out b);
		}
    }
}
