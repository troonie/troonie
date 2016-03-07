using System;
using System.Drawing.Imaging;

namespace Picturez_Lib
{
	/// <summary> Invert image. </summary>
	/// <remarks><para>The filter inverts images.</para> </remarks>    
	public class InvertFilter : AbstractFilter
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Invert"/> class.
		/// </summary>
		public InvertFilter()
		{
			SupportedSrcPixelFormat = PixelFormatFlags.Format24BppRgb;
			SupportedDstPixelFormat = PixelFormatFlags.SameLikeSource;
		}

		#region protected methods

		/// <summary>
		/// Processes the filter on the passed <paramref name="srcData"/>
		/// resulting into <paramref name="dstData"/>.
		/// </summary>
		/// <param name="srcData">The source bitmap data.</param>
		/// <param name="dstData">The destination bitmap data.</param>
		protected override unsafe void Process(BitmapData srcData, BitmapData dstData)
		{
			// int ps = Image.GetPixelFormatSize(srcData.PixelFormat) / 8;
			const int ps = 3;
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
					dst[RGBA.R] = (byte)(255 - src[RGBA.R]);
					dst[RGBA.G] = (byte)(255 - src[RGBA.G]);
					dst[RGBA.B] = (byte)(255 - src[RGBA.B]);
				}
				src += offset;
				dst += offset;
			}
		}

		#endregion protected methods             
	}
}