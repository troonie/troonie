using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Troonie_Lib
{
    /// <summary>
	/// Posterization filter.
    /// </summary>
	public class PosterizationFilter : AbstractFilter
    {
        /// <summary>Divisor of posterization. Default: 20.</summary> 
        public double Divisor { get; set; }
				
		public PosterizationFilter()
        {
            SupportedSrcPixelFormat = PixelFormatFlags.All;
			SupportedDstPixelFormat = PixelFormatFlags.SameLikeSource;
			Divisor = 20;
        }

        #region protected methods

		protected override void SetProperties (double[] filterProperties)
		{
			Divisor = filterProperties [3];
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
//					dst[RGBA.B] = (byte)(Math.Round(src[RGBA.B] / Divisor) * src[RGBA.B]);
					dst[RGBA.B] = (byte)(src[RGBA.B] - Math.Round(src[RGBA.B] % Divisor));

					// rgb, 24 and 32 bit
					if (ps >= 3) {
						dst[RGBA.G] = (byte)(src[RGBA.G] - Math.Round(src[RGBA.G] % Divisor));
						dst[RGBA.R] = (byte)(src[RGBA.R] - Math.Round(src[RGBA.R] % Divisor));
					}

					// alpha, 32 bit
					if (ps == 4) {
						dst[RGBA.A] = Use255ForAlpha ? (byte)255 : (byte)(src[RGBA.A] - Math.Round(src[RGBA.A] % Divisor));

					}
                }
                src += offset;
                dst += offset;
            }
        }

        #endregion protected methods             
    }
}
