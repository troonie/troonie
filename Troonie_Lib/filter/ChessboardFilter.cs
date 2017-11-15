using System;
using System.Drawing.Imaging;
using System.Drawing;

namespace Troonie_Lib
{
	/// <summary> Chessboard filter. </summary>  
	public class ChessboardFilter : AbstractFilter
	{
		public enum ChessboardVariants
		{
			BackWhite,
			Black,
			White,
			InvertedBlackWhite,
			InvertedBlack,
			InvertedWhite
		}
			
		public ChessboardVariants Variants { get; set; }
		public int Number { get; set; }

		public ChessboardFilter()
		{
			SupportedSrcPixelFormat = PixelFormatFlags.All;
			SupportedDstPixelFormat = PixelFormatFlags.SameLikeSource;
			Number = 4;
			Variants = ChessboardVariants.Black;
		}

		#region protected methods

		protected override void SetProperties (double[] filterProperties)
		{
			Variants = (ChessboardVariants)filterProperties[0];
			Number = (int)filterProperties[3];
		}

		protected internal override unsafe void Process(BitmapData srcData, BitmapData dstData)
		{
			int isInverted =  Variants == ChessboardVariants.InvertedBlack ||
			                  Variants == ChessboardVariants.InvertedWhite ||
			                  Variants == ChessboardVariants.InvertedBlackWhite ? 0 : 1;

			bool isBW = Variants == ChessboardVariants.BackWhite ||
			            Variants == ChessboardVariants.InvertedBlackWhite;

			byte fill1, fill2;
			if (Variants == ChessboardVariants.White ||
				Variants == ChessboardVariants.InvertedWhite) {
				fill1 = 255;
				fill2 = 0;
			} else {
				fill1 = 0;
				fill2 = 255;
			}

			int ps = Image.GetPixelFormatSize(srcData.PixelFormat) / 8;
			int w = srcData.Width;
			int h = srcData.Height;
			int offset = srcData.Stride - w * ps;

			byte* src = (byte*)srcData.Scan0.ToPointer();
			byte* dst = (byte*)dstData.Scan0.ToPointer();

			/* black-white */
			if (isBW) {
				// for each line
				for (int y = 0; y < h; y++)
				{
					double v = y / (double)h;
					// for each pixel
					for (int x = 0; x < w; x++, src += ps, dst += ps)
					{
						double u = x / (double)w;

						int mod = (int)((Math.Floor(Number * u) + Math.Floor(Number * v)) % 2.0);
						bool bmod = mod == isInverted;

						// 8 bit grayscale
						dst[RGBA.B] = bmod ? fill2 : fill1;

						// rgb, 24 and 32 bit
						if (ps >= 3) {
							dst [RGBA.G] = bmod ? fill2 : fill1;
							dst [RGBA.R] = bmod ? fill2 : fill1;
						}

						// alpha, 32 bit
						if (ps == 4) {
							dst [RGBA.A] = Use255ForAlpha ? (byte)255 : src [RGBA.A];
						}

					}
					src += offset;
					dst += offset;
				}
			} // end of /* black-white */
			/* NOT black-white */
			else {
				// for each line
				for (int y = 0; y < h; y++)
				{
					double v = y / (double)h;
					// for each pixel
					for (int x = 0; x < w; x++, src += ps, dst += ps)
					{
						double u = x / (double)w;

						int mod = (int)((Math.Floor(Number * u) + Math.Floor(Number * v)) % 2.0);
						bool bmod = mod == isInverted;

						// 8 bit grayscale
						dst[RGBA.B] = bmod ? src [RGBA.B] : fill1;

						// rgb, 24 and 32 bit
						if (ps >= 3) {
							dst [RGBA.G] = bmod ? src [RGBA.G] : fill1;
							dst [RGBA.R] = bmod ? src [RGBA.R] : fill1;
						}

						// alpha, 32 bit
						if (ps == 4) {
							dst [RGBA.A] = Use255ForAlpha ? (byte)255 : src [RGBA.A];
						}

					}
					src += offset;
					dst += offset;
				}
			} // end of /* NOT black-white */				
		}

		#endregion protected methods             
	}
}