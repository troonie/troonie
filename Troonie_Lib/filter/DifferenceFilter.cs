using System;
using System.Drawing.Imaging;
using System.Drawing;

namespace Troonie_Lib
{
	public class DifferenceFilter : AbstractFilter
	{
		/// <summary>
		/// Smallest allowed value in the resulting range. Default: 0.
		/// When <see cref="Highest"/> is also default value (255), 
		/// no mapping is done.
		/// </summary>
		public Byte Smallest { get; set; }

		/// <summary>
		/// Highest allowed value in the resulting range. Default: 255.
		/// When <see cref="Smallest"/> is also default value (0), 
		/// no mapping is done.
		/// </summary>
		public Byte Highest { get; set; }

		/// <summary> Image to compare. </summary>
		public Bitmap CompareBitmap { get; set; }


		public DifferenceFilter()
		{
			SupportedSrcPixelFormat = PixelFormatFlags.All;
			SupportedDstPixelFormat = PixelFormatFlags.SameLikeSource;

			Smallest = 0;
			Highest = 255;
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
			float mapper = 255.0f / (Highest - Smallest);

			int psCompare = Image.GetPixelFormatSize(CompareBitmap.PixelFormat) / 8;
			int ps = Image.GetPixelFormatSize(srcData.PixelFormat) / 8;
			if (psCompare != ps) {
				string errorMsg = "Not same pixel format of comparing images.";
				throw new ArgumentException(errorMsg);
			}

			Rectangle rectCompare = new Rectangle(0, 0, CompareBitmap.Width, CompareBitmap.Height);
			BitmapData compareData = CompareBitmap.LockBits(rectCompare, ImageLockMode.ReadOnly, CompareBitmap.PixelFormat);		

			int w = srcData.Width;
			int h = srcData.Height;
			int offset = srcData.Stride - w * ps;

			byte* src = (byte*)srcData.Scan0.ToPointer();
			byte* comp = (byte*)compareData.Scan0.ToPointer();
			byte* dst = (byte*)dstData.Scan0.ToPointer();

			// for each line
			for (int y = 0; y < h; y++)
			{
				// for each pixel
				for (int x = 0; x < w; x++, src += ps, dst += ps, comp += ps)
				{
					// 8 bit grayscale
					dst[RGBA.B] = (byte)(Math.Round(Math.Abs(src[RGBA.B] - comp[RGBA.B]) * mapper));

					// rgb, 24 and 32 bit
					if (ps >= 3) {
						dst[RGBA.G] = (byte)(Math.Round(Math.Abs(src[RGBA.G] - comp[RGBA.G]) * mapper));
						dst[RGBA.R] = (byte)(Math.Round(Math.Abs(src[RGBA.R] - comp[RGBA.R]) * mapper));
					}

					// alpha, 32 bit
					if (ps == 4) {
						dst [RGBA.A] = Use255ForAlpha ? (byte)255 : (byte)(Math.Round(Math.Abs(src[RGBA.A] - comp[RGBA.A]) * mapper));
					}

				}
				src += offset;
				dst += offset;
				comp += offset;
			}

			CompareBitmap.UnlockBits(compareData);
		}

		#endregion protected methods             
	}
}