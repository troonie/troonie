using System;
using System.Drawing.Imaging;
using System.Drawing;

namespace Troonie_Lib
{
	/// <summary>
	/// Generates from a non-black pixel a 3x3 thick pixel area.
	/// </summary>
	public class ThickPixelFilter : AbstractFilter
	{
		public ThickPixelFilter()
		{
			SupportedSrcPixelFormat = PixelFormatFlags.All;
			SupportedDstPixelFormat = PixelFormatFlags.SameLikeSource;
		}

		#region protected methods

		protected override void SetProperties (double[] filterProperties)
		{

		}

		/// <summary>
		/// Processes the filter on the passed <paramref name="srcData"/>
		/// resulting into <paramref name="dstData"/>.
		/// </summary>
		/// <param name="srcData">The source bitmap data.</param>
		/// <param name="dstData">The destination bitmap data.</param>
		protected internal override unsafe void Process(BitmapData srcData, BitmapData dstData)
		{
			int ps = Image.GetPixelFormatSize(srcData.PixelFormat) / 8;
//			Rectangle rectCompare = new Rectangle(0, 0, CompareBitmap.Width, CompareBitmap.Height);

			int w = srcData.Width;
			int h = srcData.Height;
			int stride = srcData.Stride;
			int offset = stride - w * ps;

			byte* src = (byte*)srcData.Scan0.ToPointer();
			byte* dst = (byte*)dstData.Scan0.ToPointer();
			// align pointers
			src += stride + ps;
			dst += stride + ps;

			// for each line
			for (int y = 1; y < h - 1; y++)
			{
				// for each pixel
				for (int x = 1; x < w - 1; x++, src += ps, dst += ps)
				{
					// 8 bit grayscale
					if (src [RGBA.B] != 0) {

						dst [RGBA.B] = src [RGBA.B];
						dst [RGBA.B - stride - ps] = src [RGBA.B]; //up left
						dst [RGBA.B - stride] = src [RGBA.B]; //up
						dst [RGBA.B - stride + ps] = src [RGBA.B]; //up right

						dst [RGBA.B - ps] = src [RGBA.B]; //left
						dst [RGBA.B + ps] = src [RGBA.B]; //right

						dst [RGBA.B + stride - ps] = src [RGBA.B]; //down left
						dst [RGBA.B + stride] = src [RGBA.B]; //down
						dst [RGBA.B + stride + ps] = src [RGBA.B]; //down right
					}

					// rgb, 24 and 32 bit
					if (ps >= 3) {
						if (src [RGBA.G] != 0) {

							dst [RGBA.G] = src [RGBA.G];
							dst [RGBA.G - stride - ps] = src [RGBA.G]; //up left
							dst [RGBA.G - stride] = src [RGBA.G]; //up
							dst [RGBA.G - stride + ps] = src [RGBA.G]; //up right

							dst [RGBA.G - ps] = src [RGBA.G]; //left
							dst [RGBA.G + ps] = src [RGBA.G]; //right

							dst [RGBA.G + stride - ps] = src [RGBA.G]; //down left
							dst [RGBA.G + stride] = src [RGBA.G]; //down
							dst [RGBA.G + stride + ps] = src [RGBA.G]; //down right
						}

						if (src [RGBA.R] != 0) {

							dst [RGBA.R] = src [RGBA.R];
							dst [RGBA.R - stride - ps] = src [RGBA.R]; //up left
							dst [RGBA.R - stride] = src [RGBA.R]; //up
							dst [RGBA.R - stride + ps] = src [RGBA.R]; //up right

							dst [RGBA.R - ps] = src [RGBA.R]; //left
							dst [RGBA.R + ps] = src [RGBA.R]; //right

							dst [RGBA.R + stride - ps] = src [RGBA.R]; //down left
							dst [RGBA.R + stride] = src [RGBA.R]; //down
							dst [RGBA.R + stride + ps] = src [RGBA.R]; //down right
						}
					}

					// alpha, 32 bit
					if (ps == 4) {
						if (Use255ForAlpha) {

							dst [RGBA.A] = 255;
							dst [RGBA.A - stride - ps] = 255; //up left
							dst [RGBA.A - stride] = 255; //up
							dst [RGBA.A - stride + ps] = 255; //up right

							dst [RGBA.A - ps] = 255; //left
							dst [RGBA.A + ps] = 255; //right

							dst [RGBA.A + stride - ps] = 255; //down left
							dst [RGBA.A + stride] = 255; //down
							dst [RGBA.A + stride + ps] = 255; //down right

						} else {

							dst [RGBA.A] = src [RGBA.A];
							dst [RGBA.A - stride - ps] = src [RGBA.A]; //up left
							dst [RGBA.A - stride] = src [RGBA.A]; //up
							dst [RGBA.A - stride + ps] = src [RGBA.A]; //up right

							dst [RGBA.A - ps] = src [RGBA.A]; //left
							dst [RGBA.A + ps] = src [RGBA.A]; //right

							dst [RGBA.A + stride - ps] = src [RGBA.A]; //down left
							dst [RGBA.A + stride] = src [RGBA.A]; //down
							dst [RGBA.A + stride + ps] = src [RGBA.A]; //down right
						}

					}
				}
				src += 2 * ps + offset;
				dst += 2 * ps + offset;
			}
		}

		#endregion protected methods             
	}
}