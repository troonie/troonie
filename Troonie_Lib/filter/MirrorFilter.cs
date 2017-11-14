using System;
using System.Drawing.Imaging;
using System.Drawing;

namespace Troonie_Lib
{
	/// <summary>
	/// The filter mirrors colored and images.
	/// </summary>
	public class MirrorFilter : AbstractFilter
	{
		/// <summary>
		/// Sets the axis for mirroring.
		/// </summary>
		public enum MirrorAxis
		{
			/// <summary>X-axis.</summary>
			X,
			/// <summary>Y-axis. </summary>
			Y,
			/// <summary>z-axis.</summary>
			Z
		}

		/// <summary>
		/// The axis for mirroring. Default value: Y.
		/// </summary>
		public MirrorAxis Axis;

		public MirrorFilter()
		{
			SupportedSrcPixelFormat = PixelFormatFlags.All;
			SupportedDstPixelFormat = PixelFormatFlags.SameLikeSource; 
			Axis = MirrorAxis.Y;
		}

		#region protected methods

		protected override void SetProperties (double[] filterProperties)
		{
			Axis = (MirrorAxis)filterProperties[0];
		}
			
		protected override internal unsafe void Process(BitmapData srcData, BitmapData dstData)
		{
			int ps = Image.GetPixelFormatSize(srcData.PixelFormat) / 8;
			int w = srcData.Width;
			int h = srcData.Height;
			int offset = srcData.Stride - w * ps;
			int stride = srcData.Stride;

//			byte* src = (byte*)srcData.Scan0.ToPointer();
			byte* dst = (byte*)dstData.Scan0.ToPointer();

			switch (Axis) {
			case MirrorAxis.Y:
				// for each line
				for (int y = 0; y < h; y++) {
					// align pointer to end of the line
					byte* src = (byte*)srcData.Scan0.ToPointer ();
					src += stride * y - offset;

					// for each pixel
					for (int x = 0; x < w; x++, src -= ps, dst += ps) {
						// 8 bit grayscale
						dst [RGBA.B] = src [RGBA.B];

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
					//				src += offset;
					dst += offset;
				}
				break;

			case MirrorAxis.X:
				// for each line
				for (int y = h - 1; y >= 0; y--) {
					// align pointer to end of the line
					byte* src = (byte*)srcData.Scan0.ToPointer ();
					src += stride * y;

					// for each pixel
					for (int x = 0; x < w; x++, src += ps, dst += ps) {
						// 8 bit grayscale
						dst [RGBA.B] = src [RGBA.B];

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
					dst += offset;
				}

				break;

			case MirrorAxis.Z:
				// for each line
				for (int y = h - 1; y >= 0; y--) {
					// align pointer to end of the line
					byte* src = (byte*)srcData.Scan0.ToPointer ();
					src += stride * y + stride - offset;
//					src += stride * y - offset;
					// for each pixel
					for (int x = 0; x < w; x++, src -= ps, dst += ps) {
						// 8 bit grayscale
						dst [RGBA.B] = src [RGBA.B];

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
					dst += offset;
				}

				break;
			}
		}

		#endregion protected methods             
	}
}