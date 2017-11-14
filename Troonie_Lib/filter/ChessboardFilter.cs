using System;
using System.Drawing.Imaging;
using System.Drawing;

namespace Troonie_Lib
{
	/// <summary> Chessboard filter. </summary>  
	public class ChessboardFilter : AbstractFilter
	{
		/// <summary>
		/// Sets the axis for mirroring.
		/// </summary>
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
				double v = y / (double)h;
				// for each pixel
				for (int x = 0; x < w; x++, src += ps, dst += ps)
				{
					double u = x / (double)w;

					int mod = (int)((Math.Floor(Number * u) + Math.Floor(Number * v)) % 2.0);
					bool bmod = mod == 1;
					if (Variants == ChessboardVariants.InvertedBlack ||
						Variants == ChessboardVariants.InvertedWhite ||
						Variants == ChessboardVariants.InvertedBlackWhite) {
						bmod = !bmod;
					}
						
					bool isBW = false;
					if (Variants == ChessboardVariants.BackWhite ||
						Variants == ChessboardVariants.InvertedBlackWhite) {
						isBW = true;
					}

					byte fill, r, g, b;
					if (Variants == ChessboardVariants.White ||
					    Variants == ChessboardVariants.InvertedWhite) {
						fill = 255;
						r = g = b = 0;
					} else {
						fill = 0;
						r = g = b = 255;
					}
						
					// 8 bit grayscale
					b = isBW ? b : src [RGBA.B];
					dst[RGBA.B] = bmod ? b : fill;

					// rgb, 24 and 32 bit
					if (ps >= 3) {
						g = isBW ? g : src [RGBA.G];
						r = isBW ? r : src [RGBA.R];
						dst [RGBA.G] = bmod ? g : fill;
						dst [RGBA.R] = bmod ? r : fill;
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