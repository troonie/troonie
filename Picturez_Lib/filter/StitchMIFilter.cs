using System;
using System.Drawing.Imaging;
using System.Drawing;

namespace Picturez_Lib
{
	public class StitchMIFilter
	{
		public Bitmap Bitmap1 { get; set; }
		public Bitmap Bitmap2 { get; set; }
		public Bitmap ResultBitmap { get; private set; }

		public bool Landscape { get; set; }
		public bool Use255ForAlpha { get; set; }

		public int Left01 { get; set; }
		public int Right01 { get; set; }
		public int Top01 { get; set; }
		public int Bottom01 { get; set; }

		public int Left02 { get; set; }
		public int Right02 { get; set; }
		public int Top02 { get; set; }
		public int Bottom02 { get; set; }

		public StitchMIFilter(Bitmap bitmap1, Bitmap bitmap2)
		{
			Bitmap1 = bitmap1;
			Bitmap2 = bitmap2;
		}

		private static PixelFormat CalcPixelFormatFromPixelSize(int ps)
		{
			switch (ps) {
			case 1:
				return PixelFormat.Format8bppIndexed;
			case 3:
				return PixelFormat.Format24bppRgb;
			case 4:
			default:
				return PixelFormat.Format32bppArgb;
			}
		}

		public unsafe void Process()
		{
			int w1 = Bitmap1.Width;
			int h1 = Bitmap1.Height;
			int ps1 = Image.GetPixelFormatSize(Bitmap1.PixelFormat) / 8;
			Rectangle rect1 = new Rectangle(0, 0, w1, h1);
			BitmapData data1 = Bitmap1.LockBits(rect1, ImageLockMode.ReadOnly, Bitmap1.PixelFormat);				
			int offset1 = data1.Stride - w1 * ps1;

			int w2 = Bitmap2.Width;
			int h2 = Bitmap2.Height;
			int ps2 = Image.GetPixelFormatSize(Bitmap2.PixelFormat) / 8;
			Rectangle rect2 = new Rectangle(0, 0, w2, h2);
			BitmapData data2 = Bitmap2.LockBits(rect2, ImageLockMode.ReadOnly, Bitmap2.PixelFormat);				
			int offset2 = data2.Stride - w2 * ps2;

			int w3, h3;
			BitmapData data3;
			int ps3 = Math.Max (ps1, ps2);
			PixelFormat pf3 = CalcPixelFormatFromPixelSize (ps3);

			if (Landscape) {

				w3 = w1 + Left01 + Right01 + w2 + Left02 + Right02;
				h3 = Math.Max ((h1 + Top01 + Bottom01), (h2 + Top02 + Bottom02));
				Rectangle rect3 = new Rectangle (0, 0, w3, h3);
				ResultBitmap = new Bitmap (w3, h3, pf3);
				data3 = ResultBitmap.LockBits (rect3, ImageLockMode.WriteOnly, pf3);	
				int offset3 = data3.Stride - w3 * ps3;

				byte* b1 = (byte*)data1.Scan0.ToPointer ();
				byte* b2 = (byte*)data2.Scan0.ToPointer ();
				byte* b3 = (byte*)data3.Scan0.ToPointer ();

				// b3: for each line
				for (int y = 0; y < h3; y++) {
					// b3: for each pixel
					for (int x = 0; x < w3; x++, b3 += ps3) {
						if ((y >= Top01 && y < Top01 + h1) && 
							(x >= Left01 && x < Left01 + w1)) {

							// 8 bit grayscale
							b3 [RGBA.B] = b1 [RGBA.B];

							// rgb, 24 and 32 bit
							if (ps1 == 1 && ps3 >= 3) {
								b3 [RGBA.G] = b1 [RGBA.B];
								b3 [RGBA.R] = b1 [RGBA.B];
							} else if (ps1 >= 3) {
								b3 [RGBA.G] = b1 [RGBA.G];
								b3 [RGBA.R] = b1 [RGBA.R];
							}

							// alpha, 32 bit
							if (ps3 == 4 && ps1 != 4) {
								b3 [RGBA.A] = 255;
							} else if (ps1 == 4) {
								b3 [RGBA.A] = Use255ForAlpha ? (byte)255 : b1 [RGBA.A];
							}

							b1 += ps1;
						}

						int distanceToB2 = Left01 + w1 + Right01 + Left02;
						if ((y >= Top02 && y < Top02 + h2) &&
							(x >= distanceToB2 && x < distanceToB2 + w2)) {

							// 8 bit grayscale
							b3 [RGBA.B] = b2 [RGBA.B];

							// rgb, 24 and 32 bit
							if (ps2 == 2 && ps3 >= 3) {
								b3 [RGBA.G] = b2 [RGBA.B];
								b3 [RGBA.R] = b2 [RGBA.B];
							} else if (ps2 >= 3) {
								b3 [RGBA.G] = b2 [RGBA.G];
								b3 [RGBA.R] = b2 [RGBA.R];
							}

							// alpha, 32 bit
							if (ps3 == 4 && ps2 != 4) {
								b3 [RGBA.A] = 255;
							} else if (ps2 == 4) {
								b3 [RGBA.A] = Use255ForAlpha ? (byte)255 : b2 [RGBA.A];
							}

							b2 += ps2;
						}
					}

					b1 += offset1;
					b2 += offset2;
					b3 += offset3;
				}
			} else { /* Portrait */
				w3 = Math.Max((w1 + Left01 + Right01), (w2 + Left02 + Right02));
				h3 = h1 + Top01 + Bottom01 + h2 + Top02 + Bottom02;
				Rectangle rect3 = new Rectangle(0, 0, w3, h3);
				ResultBitmap = new Bitmap (w3, h3, pf3);
				data3 = ResultBitmap.LockBits(rect3, ImageLockMode.WriteOnly, pf3);	
				int offset3 = data3.Stride - w3 * ps3;

				byte* b1 = (byte*)data1.Scan0.ToPointer();
				byte* b2 = (byte*)data2.Scan0.ToPointer();
				byte* b3 = (byte*)data3.Scan0.ToPointer();

				// b3: for each line
				for (int y = 0; y < h3; y++)
				{
					// b3: for each pixel
					for (int x = 0; x < w3; x++, b3 += ps3)
					{
						if ((y >= Top01 && y < Top01 + h1) && 
						    (x >= Left01 && x < Left01 + w1)) {

							// 8 bit grayscale
							b3[RGBA.B] = b1[RGBA.B];

							// rgb, 24 and 32 bit
							if (ps1 == 1 && ps3 >= 3) {
								b3 [RGBA.G] = b1 [RGBA.B];
								b3 [RGBA.R] = b1 [RGBA.B];
							}
							else if (ps1 >= 3) {
								b3 [RGBA.G] = b1 [RGBA.G];
								b3 [RGBA.R] = b1 [RGBA.R];
							}

							// alpha, 32 bit
							if (ps3 == 4 && ps1 != 4) {
								b3 [RGBA.A] = 255;
							}
							else if (ps1 == 4) {
								b3 [RGBA.A] = Use255ForAlpha ? (byte)255 : b1 [RGBA.A];
							}

							b1 += ps1;
						}


						int verticaldistanceToB2 = Top01 + h1 + Bottom01 + Top02;

						if ((y >= verticaldistanceToB2 && y < verticaldistanceToB2 + h2) &&
						    (x >= Left02 && x < Left02 + w2 )) {

							// 8 bit grayscale
							b3[RGBA.B] = b2[RGBA.B];

							// rgb, 24 and 32 bit
							if (ps2 == 2 && ps3 >= 3) {
								b3 [RGBA.G] = b2 [RGBA.B];
								b3 [RGBA.R] = b2 [RGBA.B];
							}
							else if (ps2 >= 3) {
								b3 [RGBA.G] = b2 [RGBA.G];
								b3 [RGBA.R] = b2 [RGBA.R];
							}

							// alpha, 32 bit
							if (ps3 == 4 && ps2 != 4) {
								b3 [RGBA.A] = 255;
							}
							else if (ps2 == 4) {
								b3 [RGBA.A] = Use255ForAlpha ? (byte)255 : b2 [RGBA.A];
							}

							b2 += ps2;
						}
					}

					b1 += offset1;
					b2 += offset2;
					b3 += offset3;
				}
			}
			// end of processing
			Bitmap1.UnlockBits(data1);
			Bitmap2.UnlockBits(data2);
			ResultBitmap.UnlockBits(data3);

			if (ps3 == 1)
			{
				ColorPalette.I.SetColorPaletteToGray (ResultBitmap);
			}
		}		         
	}
}