using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Troonie_Lib
{
    /// <summary>
	/// Binarization filter.
    /// </summary>
	public class BinarizationFilter : AbstractFilter
    {		
		/// <summary> The threshold value, used as border for binarization. Default: 100 </summary>
		public byte Threshold { get; set; }

		public bool ColorBinarization { get; set; }

		public BinarizationFilter()
        {
            SupportedSrcPixelFormat = PixelFormatFlags.All;
			SupportedDstPixelFormat = PixelFormatFlags.SameLikeSource;

			Threshold = 100;
			ColorBinarization = false;
	    }

        #region protected methods

		protected override void SetProperties (double[] filterProperties)
		{
			ColorBinarization = filterProperties[0] == 1;
			Threshold = (byte)filterProperties [3];
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
					dst[RGBA.B] = (byte)(src[RGBA.B] > Threshold ? 255 : 0);

					// rgb, 24 and 32 bit
					if (ps >= 3) {
						if (ColorBinarization) {
							dst [RGBA.R] = (byte)(src [RGBA.R] > Threshold ? 255 : 0);
							dst [RGBA.G] = (byte)(src [RGBA.G] > Threshold ? 255 : 0);
						} else {
							if (src[RGBA.R] > Threshold ||
								src[RGBA.G] > Threshold ||
								src[RGBA.B] > Threshold)
							{
								dst[RGBA.R] = 255;
								dst[RGBA.G] = 255;
								dst[RGBA.B] = 255;
							}
							else
							{
								dst[RGBA.R] = 0;
								dst[RGBA.G] = 0;
								dst[RGBA.B] = 0;                                
							}
						}							
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
