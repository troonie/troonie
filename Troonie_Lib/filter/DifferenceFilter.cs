using System;
using System.Drawing.Imaging;
using System.Drawing;

namespace Troonie_Lib
{
	public class DifferenceFilter : AbstractFilter
	{
		/// <summary>
		/// Determines whether pixels will be drawn as 3x3 thick pixels. Default: false.
		/// </summary>
		public bool DrawThick3x3Pixels { get; set; }

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
			DrawThick3x3Pixels = filterProperties[0] == 1;
			Smallest = (byte)filterProperties[3];
			Highest = (byte)filterProperties[4];
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

			float mapper = 255.0f / (Highest - Smallest);
			int w = srcData.Width;
			int h = srcData.Height;
			int stride = srcData.Stride;
			PixelFormat pf = srcData.PixelFormat;
			int ps = Image.GetPixelFormatSize(pf) / 8;
			int psCompare = Image.GetPixelFormatSize(CompareBitmap.PixelFormat) / 8;
			Rectangle rect = new Rectangle(0, 0, w, h);
			BitmapData compareData = CompareBitmap.LockBits(rect, ImageLockMode.ReadOnly, CompareBitmap.PixelFormat);		

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
					dst[RGBA.B] = (byte)(Math.Round(Math.Abs(src[RGBA.B] - comp[RGBA.B]) * mapper));

					// rgb, 24 and 32 bit
					if (ps >= 3) {
						dst[RGBA.G] = (byte)(Math.Round(Math.Abs(src[RGBA.G] - comp[RGBA.G]) * mapper));
						dst[RGBA.R] = (byte)(Math.Round(Math.Abs(src[RGBA.R] - comp[RGBA.R]) * mapper));
					}

					// alpha, 32 bit
					if (ps == 4) {
						dst [RGBA.A] = 255;
						if (psCompare == 4 && !Use255ForAlpha) {
							dst [RGBA.A] = (byte)(Math.Round(Math.Abs(src[RGBA.A] - comp[RGBA.A]) * mapper));
						}
					}

				}
				src += offset;
				dst += offset;
				comp += compareOffset;
			}				

			CompareBitmap.UnlockBits(compareData);

			#region thick pixel drawing
			if (DrawThick3x3Pixels) {
				
				Bitmap bd = new Bitmap(w, h, pf);
				BitmapData bdData = bd.LockBits(rect, ImageLockMode.WriteOnly, pf);
				dst = (byte*)dstData.Scan0.ToPointer();
				byte* bdPtr = (byte*)bdData.Scan0.ToPointer();
					
				for (int i = 0; i < stride * h; i++)
				{
					bdPtr[i] = dst[i];
				}

				DilatationFilter dilatationFilter = new DilatationFilter();
				dilatationFilter.Process(bdData, dstData);
				bd.UnlockBits(bdData);
				bd.Dispose();
			}
			#endregion thick pixel drawing
		}

		#endregion protected methods             
	}
}