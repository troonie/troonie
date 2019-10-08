using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Troonie_Lib
{
	/// <summary>
	/// Simple contrast filter by exponentiation of channel value. 
	/// </summary>
	public class ContrastFilter : AbstractFilter
    {
		/// <summary>Exponent for manipulating channel result. Lightness in- or decreasing by exponentiation.</summary>
		public float Exp { get; set; }

		/// <summary>Threshold to determine whether increasing or decreasing channel value.</summary>
		public byte Threshold { get; set; }

		public ContrastFilter ()
        {
            SupportedSrcPixelFormat = PixelFormatFlags.All;
			SupportedDstPixelFormat = PixelFormatFlags.SameLikeSource;
			Exp = 1;
			Threshold = 127;
		}
	    
        #region protected methods

		protected override void SetProperties (double[] filterProperties)
		{
			Exp = (float)filterProperties [3];
			Threshold = (byte)filterProperties [4];
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
					// 8 bit grayscale
					if (ps == 1) {
						float tmp_exp = src [RGBA.B] >= Threshold ? Exp : 2 - Exp;
						double tmp = Math.Pow (src [RGBA.B], tmp_exp) + 0.5f;
						*dst = (byte)Math.Min (tmp, 255);
					}

					// rgb, 24 and 32 bit
					if (ps >= 3) {
						// red
						float tmp_exp = src [RGBA.R] >= Threshold ? Exp : 2 - Exp;
						double tmp = Math.Pow (src [RGBA.R], tmp_exp) + 0.5f;
						dst [RGBA.R] = (byte)Math.Min (tmp, 255);

						// green
						tmp_exp = src [RGBA.G] >= Threshold ? Exp : 2 - Exp;
						tmp = Math.Pow (src [RGBA.G], tmp_exp) + 0.5f;
						dst [RGBA.G] = (byte)Math.Min (tmp, 255);

						// blue
						tmp_exp = src [RGBA.B] >= Threshold ? Exp : 2 - Exp;
						tmp = Math.Pow (src [RGBA.B], tmp_exp) + 0.5f;
						dst [RGBA.B] = (byte)Math.Min (tmp, 255);
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
    }
}
