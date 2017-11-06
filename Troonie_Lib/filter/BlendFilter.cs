using System;
using System.Drawing.Imaging;
using System.Drawing;

namespace Troonie_Lib
{
	public class BlendFilter : AbstractFilter
	{		
		/// <summary>
		/// Mix value in percent. Default: 0.5 (=50%), means both images to same parts.
		/// </summary>
		public double MixPercent { get; set; }

		/// <summary> Image to compare. </summary>
		public Bitmap CompareBitmap { get; set; }

		public BlendFilter()
		{
			SupportedSrcPixelFormat = PixelFormatFlags.All;
			SupportedDstPixelFormat = PixelFormatFlags.SameLikeSource;

			MixPercent = 0.5;
		}
			
		#region protected methods

		protected override void SetProperties (double[] filterProperties)
		{			
			MixPercent = filterProperties[3];
		}
			
		protected internal override unsafe void Process(BitmapData srcData, BitmapData dstData)
		{
			// will be checked by application, not by filter
//			if (Math.Abs(psCompare - ps) > 1 || 
//				CompareBitmap.Width != srcData.Width || 
//				CompareBitmap.Height != srcData.Height) {
//				string errorMsg = "Cannot compare grayscale with color image as well as different image sizes.";
//				throw new ArgumentException(errorMsg);
//			}

			int w = srcData.Width;
			int h = srcData.Height;
			int stride = srcData.Stride;
			PixelFormat pf = srcData.PixelFormat;
			int ps = Image.GetPixelFormatSize(pf) / 8;
			int psCompare = Image.GetPixelFormatSize(CompareBitmap.PixelFormat) / 8;
			Rectangle rect = new Rectangle(0, 0, w, h);
			BitmapData compareData = CompareBitmap.LockBits(
				rect, ImageLockMode.ReadOnly, CompareBitmap.PixelFormat);		

			int offset = stride - w * ps;
			int compareOffset = compareData.Stride - w * psCompare;

			byte* src = (byte*)srcData.Scan0.ToPointer();
			byte* comp = (byte*)compareData.Scan0.ToPointer();
			byte* dst = (byte*)dstData.Scan0.ToPointer();

			// for each line
			for (int y = 0; y < h; y++)
			{
				// for each pixel
				for (int x = 0; x < w; x++, src += ps, dst += ps, comp += psCompare)
				{
					// 8 bit grayscale
					dst[RGBA.B] = (byte)(Math.Round(
						MixPercent * src[RGBA.B] + (1 - MixPercent) * comp[RGBA.B]));

					// rgb, 24 and 32 bit
					if (ps >= 3) {
						dst[RGBA.G] = (byte)(Math.Round(
							MixPercent * src[RGBA.G] + (1 - MixPercent) * comp[RGBA.G]));
						dst[RGBA.R] = (byte)(Math.Round(
							MixPercent * src[RGBA.R] + (1 - MixPercent) * comp[RGBA.R]));
					}

					// alpha, 32 bit
					if (ps == 4) {
						dst [RGBA.A] = 255;
						if (psCompare == 4 && !Use255ForAlpha) {
							dst[RGBA.A] = (byte)(Math.Round(
								MixPercent * src[RGBA.A] + (1 - MixPercent) * comp[RGBA.A]));
						}
					}

				}
				src += offset;
				dst += offset;
				comp += compareOffset;
			}				

			CompareBitmap.UnlockBits(compareData);
		}

		#endregion protected methods             
	}
}