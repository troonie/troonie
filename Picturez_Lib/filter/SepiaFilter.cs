using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Picturez_Lib
{
    /// <summary>
    /// Sepia filter - old brown photo.
    /// </summary>
    public class SepiaFilter : AbstractFilter
    {
        /// <summary>Q coefficient of YIQ color space. Default: 0.0</summary> 
        public float Q { get; set; }

        /// <summary>I coefficient of YIQ color space. Default: 51.</summary>
        public float I { get; set; }
		
        public SepiaFilter()
        {
            SupportedSrcPixelFormat = PixelFormatFlags.All;
			SupportedDstPixelFormat = PixelFormatFlags.SameLikeSource;
            Q = 0.0f;
            I = 51.0f;
        }

        #region protected methods

		protected override void SetProperties (double[] filterProperties)
		{
			Q = (float)filterProperties [3];
			I = (float)filterProperties [4];
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
					float Y = src[RGBA.B];

					// color, 24 and 32 bit
					if (ps >= 3) {
						Y =  0.299f * src[RGBA.R] + 
							 0.587f * src[RGBA.G] +                         
							 0.114f * src[RGBA.B];
					}
                 
                    // original, no sepia   
                    // I = 0.596f * src[RGBA.R] - 0.274f * src[RGBA.G] - 
                    // 0.322f * src[RGBA.B];
                    // Q = 0.212f * src[RGBA.R] - 0.523f * src[RGBA.G] + 
                    // 0.311f * src[RGBA.B]; 

                    float r = (Y + 0.956f * I + 0.621f * Q + 0.5f);
                    float g = (Y - 0.272f * I - 0.647f * Q + 0.5f);
                    float b = (Y - 1.105f * I + 1.702f * Q + 0.5f);

					// 8 bit grayscale
					dst[RGBA.B] = (byte)((r + g + b) / 3.0f + 0.5f);

					// rgb, 24 and 32 bit
					if (ps >= 3) {
						dst [RGBA.R] = (byte)Math.Min (r, 255);
						dst [RGBA.G] = (byte)Math.Max (g, 0);
						dst [RGBA.B] = (byte)Math.Max (b, 0); 
					}

					// alpha, 32 bit
					if (ps == 4) {
						dst [RGBA.A] = Use255ForAlpha ? (byte)255 : src [RGBA.A];
					}
                }
                src += offset;
                dst += offset;
            }
        }

        #endregion protected methods             
    }
}
