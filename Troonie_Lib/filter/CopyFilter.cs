using System;
using System.Drawing.Imaging;
using System.Drawing;

namespace Troonie_Lib
{
	/// <summary> No filter. just a deep copy of image. </summary>
	public class CopyFilter : AbstractFilter
	{
		public CopyFilter()
		{
			SupportedSrcPixelFormat = PixelFormatFlags.All;
			SupportedDstPixelFormat = PixelFormatFlags.SameLikeSource;
		}

		#region protected methods

		protected override void SetProperties (double[] filterProperties)
		{

		}
			
		protected internal override unsafe void Process(BitmapData srcData, BitmapData dstData)
		{
			int ps = Image.GetPixelFormatSize(srcData.PixelFormat) / 8;
//			const int ps = 3;
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
					dst[RGBA.B] = src[RGBA.B];

					// rgb, 24 and 32 bit
					if (ps >= 3) {
						dst [RGBA.G] = src [RGBA.G];
						dst [RGBA.R] = src [RGBA.R];
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