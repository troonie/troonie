using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.IO;
using NetColorPalette = System.Drawing.Imaging.ColorPalette;
using PicColorPalette = Troonie_Lib.ColorPalette;

namespace Troonie_Lib
{
	public enum ConvertMode
	{
		/// <summary>
		/// Editor mode. Image will be cut directly by passed x, y, w, h values.
		/// </summary>
		Editor,
		/// <summary>
		/// Default convert mode. Image will be stretch or forge by pushing 
		/// complete (NO CUTTING) image in passed x, y, w, h values.
		/// </summary>
		StretchForge,
		/// <summary>
		/// Image will be cut by passed x, y, w, h values, NO stretching or forging
		/// will be done.
		/// </summary>
		NoStretchForge
	}

	/// <summary>
	/// Provides several static functions to convert one or multiple image file(s) in a 
	/// special image format.
	/// </summary>
	/// <remarks>
	/// <para>Supported formats:</para>
	/// <list type="enumeration">BMP</list>
	/// <list type="enumeration">EMF</list>
	/// <list type="enumeration">GIF</list>
	/// <list type="enumeration">JPG</list>
	/// <list type="enumeration">PNG</list>
	/// <list type="enumeration">TIFF</list>
	/// <list type="enumeration">WMF</list>
	/// </remarks>
	public static class ImageConverter
	{
		#region enums, statics and constants

		#pragma warning disable 649
		// ReSharper disable InconsistentNaming
		private static uint BI_RGB;
		private static uint DIB_RGB_COLORS;
		// ReSharper restore InconsistentNaming
		#pragma warning restore 649

		// ReSharper disable InconsistentNaming
		private const int SRCCOPY = 0x00CC0020;
		// ReSharper restore InconsistentNaming

		[System.Runtime.InteropServices.StructLayout(
			System.Runtime.InteropServices.LayoutKind.Sequential)]
		private struct Bitmapinfo
		{
			public uint biSize;
			public int biWidth, biHeight;
			public short biPlanes, biBitCount;
			public uint biCompression, biSizeImage;
			public int biXPelsPerMeter, biYPelsPerMeter;
			public uint biClrUsed, biClrImportant;
			[System.Runtime.InteropServices.MarshalAs(
				System.Runtime.InteropServices.UnmanagedType.ByValArray, 
				SizeConst = 256)]
			public uint[] cols;
		}

		#endregion enums, statics and constants

		#region Public functions

		/// <summary>
		/// Fast reading-out of image dimension.
		/// </summary>
		public static void GetImageDimension(string imagePath, out int width, out int height, out PixelFormat pixelFormat)
		{
			// fast reading-out of image sizes
			using (FileStream fs = new FileStream (imagePath, FileMode.Open)) 
			{
				Image image = Image.FromStream (fs, true, false);
				width = image.Width;
				height = image.Height;
				pixelFormat = image.PixelFormat;
			}
		}

		public static bool IsColorImage(Bitmap b)
		{
			int bitNumber = Image.GetPixelFormatSize(b.PixelFormat);
			return (bitNumber >= 24);
		}

		public static RectangleF GetRectangle( 
		                                      int sourceWidth, 
		                                      int sourceHeight, 
		                                      float xStart, 
		                                      float yStart,
		                                      int width, 
		                                      int height, 
		                                      ConvertMode convertMode)
		{
			RectangleF rec;

			switch (convertMode) {
				case ConvertMode.Editor:
				rec = new RectangleF(xStart, yStart, width, height);
				break;
				case ConvertMode.NoStretchForge:
				float ratio = width / (float)height;
				float h = sourceHeight;
				float w = sourceWidth;

				xStart = 0f;
				yStart = 0f;
				float wCorrect = w;
				float hCorrect = h;

				if (w / h > ratio) {
					wCorrect = h * ratio;
					xStart /*+*/= (w - wCorrect) / 2.0f;
				} else if (w / h <= ratio) {
					hCorrect = w / ratio;
					yStart /*+*/= (h - hCorrect) / 2.0f;
				}

				rec = new RectangleF(xStart, yStart, wCorrect, hCorrect);				
				break;
				case ConvertMode.StretchForge:
				default:
				rec = new RectangleF(0, 0, sourceWidth, sourceHeight);
				break;
			}	

			return rec;
		}

		/// <summary>
		/// Scales and cuts source bitmap into destination as COLOR (a)rgb bitmap.
		/// </summary>
		public static bool ScaleAndCut(
			Bitmap source, 
			out Bitmap destination,
			float xStart, 
			float yStart,
			int width, 
			int height, 
			ConvertMode convertMode,
			bool highGraphicsQuality)
		{
//			// TOD-O: Remove it?
//			if (width == 0 || height == 0)
//			{
//				throw new Exception ("TODO: Remove it?");
//				width = source.Width;
//				height = source.Height;
//			}		

			RectangleF rec = GetRectangle (
				source.Width, source.Height, xStart, yStart, width, height, convertMode);						

//			destination = source.Clone (rec, source.PixelFormat);
//
//			if (source.PixelFormat == PixelFormat.Format8bppIndexed) {
//				destination = new Bitmap(destination, width, height);
//			} else {
//				destination = CloneBitmapByUsingGraphics (destination, source.PixelFormat, width, height, highGraphicsQuality);
//			}
//
			// NEW: 8 bit grayscale is also converted in 24 bit to use CloneBitmapByUsingGraphics(..)-method
			PixelFormat rgbPixelFormat;
			// Converting (by cloning) in color format, necessary for drawing with graphics in next step
			if (source.PixelFormat == PixelFormat.Format32bppArgb || 
				source.PixelFormat == PixelFormat.Format32bppRgb || 
				source.PixelFormat == PixelFormat.Format24bppRgb) {
				// Here Pixelformat does matter, it will be used the source.PixelFormat
				destination = source.Clone (rec, source.PixelFormat);
				rgbPixelFormat = source.PixelFormat;
			}
			else {
				// Here Pixelformat does NOT matter, Bitmap.Clone does not produce 24 bit, 
				// instead source.PixelFormat will be used. Confusing...
				destination = source.Clone (rec, PixelFormat.Format24bppRgb);
				rgbPixelFormat = PixelFormat.Format24bppRgb;
			}
			destination = CloneBitmapByUsingGraphics(destination, rgbPixelFormat, width, height, highGraphicsQuality);

			return true;
		}

		/// <summary>
		/// Converts a bitmap into a 1 bpp bitmap of the same dimensions, fast.
		/// Original source from: http://www.wischik.com/lu/programmer/1bpp.html
		/// </summary>
		/// <param name="source">The bitmap, which will be converted.</param>
		/// <returns>A 1 or 8 bpp copy of the source bitmap.</returns>
		public static Bitmap To1Bpp(Bitmap source)
		{
			// Plan: built into Windows GDI is the ability to convert
			// bitmaps from one format to another. Most of the time, this
			// job is actually done by the graphics hardware accelerator card
			// and so is extremely fast. The rest of the time, the job is done by
			// very fast native code.
			// We will call into this GDI functionality from C#. Our plan:
			// (1) Convert our Bitmap into a GDI hbitmap (ie. copy unmanaged->managed)
			// (2) Create a GDI monochrome hbitmap
			// (3) Use GDI "BitBlt" function to copy from hbitmap into monochrome (as above)
			// (4) Convert the monochrone hbitmap into a Bitmap (ie. copy unmanaged->managed)

			int w = source.Width, h = source.Height;
			IntPtr hbm = source.GetHbitmap(); // this is step (1)
			//
			// Step (2): create the monochrome bitmap.
			// "BITMAPINFO" is an interop-struct which we define below.
			// In GDI terms, it's a BITMAPHEADERINFO followed by an array of two RGBQUADs
			Bitmapinfo bmi = new Bitmapinfo
			{
				biSize = 40,
				biWidth = w,
				biHeight = h,
				biPlanes = 1,
				biBitCount = 1,
				biCompression = BI_RGB,
				biSizeImage = (uint) (((w + 7) & 0xFFFFFFF8)*h/8),
				biXPelsPerMeter = 1000000,
				biYPelsPerMeter = 1000000
			};
			// Now for the colour table.
			const uint ncols = (uint)1 << 1;
			bmi.biClrUsed = ncols;
			bmi.biClrImportant = ncols;
			bmi.cols = new uint[256]; // The structure always has fixed size 256, even if we end up using fewer colours
			bmi.cols[0] = MakeRgb(0, 0, 0); 
			bmi.cols[1] = MakeRgb(255, 255, 255);

			// Now create the indexed bitmap "hbm0"
			IntPtr bits0; // not used for our purposes. It returns a pointer to the raw bits that make up the bitmap.
			IntPtr hbm0 = CreateDIBSection(IntPtr.Zero, ref bmi, DIB_RGB_COLORS, out bits0, IntPtr.Zero, 0);
			//
			// Step (3): use GDI's BitBlt function to copy from original hbitmap into monocrhome bitmap
			// GDI programming is kind of confusing... nb. The GDI equivalent of "Graphics" is called a "DC".
			IntPtr sdc = GetDC(IntPtr.Zero);       // First we obtain the DC for the screen
			// Next, create a DC for the original hbitmap
			IntPtr hdc = CreateCompatibleDC(sdc); SelectObject(hdc, hbm);
			// and create a DC for the monochrome hbitmap
			IntPtr hdc0 = CreateCompatibleDC(sdc); SelectObject(hdc0, hbm0);
			// Now we can do the BitBlt:
			BitBlt(hdc0, 0, 0, w, h, hdc, 0, 0, SRCCOPY);
			// Step (4): convert this monochrome hbitmap back into a Bitmap:
			Bitmap b0 = Image.FromHbitmap(hbm0);
			//
			// Finally some cleanup.
			DeleteDC(hdc);
			DeleteDC(hdc0);
			ReleaseDC(IntPtr.Zero, sdc);
			DeleteObject(hbm);
			DeleteObject(hbm0);
			//
			return b0;
		}

		/// <summary>
		/// Converts a bitmap with unspecific pixel format into 24 bpp bitmap.
		/// </summary>
		/// <param name="source">The bitmap to convert.</param>
		/// <returns>Result (24 bpp) bitmap.</returns>
		/// <exception cref="System.ArgumentException">
		/// PixelFormat of bitmap is not supported.;source</exception>
		public static Bitmap To24Bpp(Bitmap source)
		{
			switch (source.PixelFormat)
			{
			case PixelFormat.Format1bppIndexed:
				// TODO: Check, if it works.
				Rectangle rec = new Rectangle (0, 0, source.Width, source.Height);
				return source.Clone (rec, PixelFormat.Format24bppRgb);
//				return CloneBitmapByUsingGraphics(source, 
//					source.Width, 
//					source.Height, 
//					true);
			case PixelFormat.Format8bppIndexed:
				return GrayscaleToRGB(source);
			case PixelFormat.Format24bppRgb:
				return source;
			case PixelFormat.Format32bppArgb:
			case PixelFormat.Format32bppRgb:
				return ARGBToRGB(source);
			default:
				throw new ArgumentException(
					"PixelFormat of bitmap is not supported.", "source");
			}
		}

		/// <summary>
		/// Converts a bitmap with unspecific pixel format into 8 bpp bitmap.
		/// </summary>
		/// <param name="source">The bitmap to convert.</param>
		/// <returns>Result (8 bpp) bitmap.</returns>
		/// <exception cref="System.ArgumentException">
		/// PixelFormat of bitmap is not supported.;source</exception>
		public static Bitmap To8Bpp(Bitmap source)
		{
			switch (source.PixelFormat)
			{
			case PixelFormat.Format1bppIndexed:
				// TODO: Check, if it works.
				Rectangle rec = new Rectangle (0, 0, source.Width, source.Height);
				return source.Clone (rec, PixelFormat.Format8bppIndexed);
//				return RGBTo8Bpp(CloneBitmapByUsingGraphics(source,
//					source.Width,
//					source.Height,
//					true));                  			
			case PixelFormat.Format8bppIndexed:
				return source;
			case PixelFormat.Format24bppRgb:
				return RGBTo8Bpp(source);
			case PixelFormat.Format32bppArgb:
			case PixelFormat.Format32bppRgb:
				return ARGBTo8Bpp(source);
			default:
				throw new ArgumentException(
					"PixelFormat of bitmap is not supported.", "source");
			}
		}

		/// <summary>
		/// Converts a bitmap with unspecific pixel format into 32 bpp bitmap.
		/// If pixel format provides no alpha, all pixels will get full alpha value (a = 255).
		/// </summary>
		/// <param name="source">The bitmap to convert.</param>
		/// <returns>Result (32 bpp) bitmap.</returns>
		/// <exception cref="System.ArgumentException">
		/// PixelFormat of bitmap is not supported.;source</exception>
		public static Bitmap To32Bpp(Bitmap source)
		{
			switch (source.PixelFormat)
			{
			case PixelFormat.Format1bppIndexed:
				// TODO: Check, if it works.
				Rectangle rec = new Rectangle (0, 0, source.Width, source.Height);
				return source.Clone (rec, PixelFormat.Format32bppArgb);
//				return RGBToARGB(CloneBitmapByUsingGraphics(source, 
//					source.Width, 
//					source.Height,
//					true), 255);
			case PixelFormat.Format8bppIndexed:
				//TODO refactoring this step
				return  RGBToARGB(GrayscaleToRGB(source), 255);
			case PixelFormat.Format24bppRgb:
				return RGBToARGB (source, 255);
			case PixelFormat.Format32bppArgb:
			case PixelFormat.Format32bppRgb:
				return source;
			default:
				throw new ArgumentException(
					"PixelFormat of bitmap is not supported.", "source");
			}
		}

//		/// <summary>
//		/// Converts a bitmap with unspecific pixel format into 32 bpp bitmap.
//		/// </summary>
//		/// <param name="source">The bitmap to convert.</param>
//		/// <returns>Result (32 bpp) bitmap.</returns>
//		/// <exception cref="System.ArgumentException">
//		/// PixelFormat of bitmap is not supported.;source</exception>
//		[Obsolete("Use ImageConverter.To32BppWithTransparencyColor(Bitmap source, Color transparencyColor) instead.")]
//		public static Bitmap To32Bpp(Bitmap source, byte alpha)
//		{
//			switch (source.PixelFormat)
//			{
//				case PixelFormat.Format1bppIndexed:
//				// TODO: Check, if it works.
//				Rectangle rec = new Rectangle (0, 0, source.Width, source.Height);
//				return source.Clone (rec, PixelFormat.Format32bppArgb);
////				return RGBToARGB(CloneBitmapByUsingGraphics(source, 
////				                                            source.Width, 
////				                                            source.Height,
////				                                            true), alpha);
//				case PixelFormat.Format8bppIndexed:
//				//TODO refactoring this step
//				return  RGBToARGB(GrayscaleToRGB(source), alpha);
//				case PixelFormat.Format24bppRgb:
//				return RGBToARGB (source, alpha);
//				case PixelFormat.Format32bppArgb:
//				case PixelFormat.Format32bppRgb:
//				//TODO refactoring this step
//				return RGBToARGB (ARGBToRGB(source), alpha);
//				default:
//				throw new ArgumentException(
//					"PixelFormat of bitmap is not supported.", "source");
//			}
//		}

		/// <summary>
		/// Converts a bitmap with unspecific pixel format into 32 bpp bitmap with a specified transparency color.
		/// Note: This function makes a real (deep) copy ot the image. 
		/// </summary>
		public static Bitmap To32BppWithTransparencyColor(Bitmap source, Color transparencyColor)
		{
//			// correct pixel format
//			Bitmap b2 = new Bitmap (10, 10, PixelFormat.Format8bppIndexed);
//			// WRONG: Not pixel format of source, always 32bit ARGB
//			Bitmap b3 = new Bitmap(source, 10, 20);
//			Bitmap b4 = CloneBitmap (b2, b2.Width, b2.Height, PixelFormat.Format24bppRgb, true);

			Bitmap b = source.Clone() as Bitmap;
			// make every pixel format to 32 bit ARGB and set transparency color
			b.MakeTransparent(transparencyColor);
			return b;
		}

		public static void CalcBiggerSideLength( 
			int biggerSideLength,
			ref int origWidth,
			ref int origHeight)
		{
			float ratio = (float)origWidth / origHeight;
			if (origWidth > origHeight)
			{
				origWidth = biggerSideLength;
				origHeight = (int)Math.Round(origWidth / ratio);
			}
			else
			{
				origHeight = biggerSideLength;
				origWidth = (int)Math.Round(origHeight * ratio);
			}		
		}

		#endregion Public functions


		#region Private helper functions

		/// <summary>
		/// Converts 32 bpp bitmap to 8 bpp bitmap by mean value of all 3 color 
		/// channels.
		/// </summary>
		/// <param name="source">The bitmap, which will be converted.</param>
		/// <returns>Result (8 bpp) bitmap.</returns>
		private static Bitmap ARGBTo8Bpp(Bitmap source)
		{
			// check image format
			if (source.PixelFormat != PixelFormat.Format32bppRgb &&
				source.PixelFormat != PixelFormat.Format32bppArgb &&
				source.PixelFormat != PixelFormat.Format32bppPArgb) {
				throw new ArgumentException (
					"Source image can be color (32 bpp) image only");
			}
			int w = source.Width;
			int h = source.Height;
			Rectangle rect = new Rectangle(0, 0, w, h);

			Bitmap destination = new Bitmap(w, h, PixelFormat.Format8bppIndexed);
			BitmapData dstData = destination.LockBits
				(rect, ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

			// lock source bitmap data
			BitmapData srcData = source.LockBits(
				rect, ImageLockMode.ReadOnly, source.PixelFormat);
			var dstOffset = dstData.Stride - w;

			// process image
			unsafe
			{
				byte* src = (byte*)srcData.Scan0.ToPointer();
				byte* dst = (byte*)dstData.Scan0.ToPointer();
				// for each line
				for (int y = 0; y < h; y++)
				{
					// for each pixel in line
					for (int x = 0; x < w; x++, src += 4, dst++)
					{
						int avg = src[RGBA.R] + src[RGBA.G] + src[RGBA.B];
						avg /= 3;
						*dst = (byte)avg;
					}
					dst += dstOffset;
				}
			}
			// unlock destination image
			source.UnlockBits(srcData);
			destination.UnlockBits(dstData);
			ColorPalette.I.SetColorPaletteToGray(destination);

			return destination;
		}

		/// <summary>
		/// Converts 32 bpp bitmap to 24 bpp bitmap.
		/// </summary>
		/// <param name="source">The bitmap, which will be converted.</param>
		/// <returns>Result (24 bpp) bitmap.</returns>
		public static Bitmap ARGBToRGB(Bitmap source,
			bool replaceTransparencyWithColor = false,
			byte replaceColorRed = 255,
			byte replaceColorGreen = 255,
			byte replaceColorBlue = 255)
		{
			// check image format
			if (source.PixelFormat != PixelFormat.Format32bppRgb &&
				source.PixelFormat != PixelFormat.Format32bppArgb &&
				source.PixelFormat != PixelFormat.Format32bppPArgb) {
				throw new ArgumentException (
					"Source image can be color (32 bpp) image only");
			}				

			int w = source.Width;
			int h = source.Height;
			Rectangle rect = new Rectangle(0, 0, w, h);

			Bitmap destination = new Bitmap(w, h, PixelFormat.Format24bppRgb);
			BitmapData dstData = destination.LockBits
				(rect, ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

			// lock source bitmap data
			BitmapData srcData = source.LockBits(
				rect, ImageLockMode.ReadOnly, source.PixelFormat);

			var dstOffset = dstData.Stride - w * 3;

			// process image
			unsafe
			{
				byte* src = (byte*)srcData.Scan0.ToPointer();
				byte* dst = (byte*)dstData.Scan0.ToPointer();
				// for each line
				for (int y = 0; y < h; y++)
				{
					// for each pixel in line
					for (int x = 0; x < w; x++, src += 4, dst += 3)
					{
						if (replaceTransparencyWithColor && src [RGBA.A] != 255) {
							dst [RGBA.R] = replaceColorRed;
							dst [RGBA.G] = replaceColorGreen;
							dst [RGBA.B] = replaceColorBlue;
						} else {
							dst [RGBA.R] = src [RGBA.R];
							dst [RGBA.G] = src [RGBA.G];
							dst [RGBA.B] = src [RGBA.B];
						}
					}
					dst += dstOffset;
				}
			}
			// unlock destination image
			source.UnlockBits(srcData);
			destination.UnlockBits(dstData);

			return destination;
		}

		/// <summary>
		/// Converts 24 bpp bitmap to 32 bpp bitmap.
		/// </summary>
		/// <param name="source">The bitmap, which will be converted.</param>
		/// <param name="alpha">The alpha value to set.</param>
		/// <returns>
		/// Result (32 bpp) bitmap.
		/// </returns>
		private static Bitmap RGBToARGB(Bitmap source, byte alpha)
		{
			if (source.PixelFormat == PixelFormat.Format32bppArgb)
			{
				throw new ArgumentException ("Image has already ARGB pixel format.", "source");
				// return source;
			}

			int w = source.Width;
			int h = source.Height;
			Rectangle rect = new Rectangle(0, 0, w, h);

			Bitmap destination = new Bitmap(w, h, PixelFormat.Format32bppArgb);
			BitmapData dstData = destination.LockBits
				(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

			// lock source bitmap data
			BitmapData srcData = source.LockBits(
				rect, ImageLockMode.ReadOnly, source.PixelFormat);

			int srcOffset = srcData.Stride - w * 3;

			// process image
			unsafe
			{
				byte* src = (byte*)srcData.Scan0.ToPointer();
				byte* dst = (byte*)dstData.Scan0.ToPointer();
				// for each line
				for (int y = 0; y < h; y++)
				{
					// for each pixel in line
					for (int x = 0; x < w; x++, src += 3, dst += 4)
					{
						dst[RGBA.R] = src[RGBA.R];
						dst[RGBA.G] = src[RGBA.G];
						dst[RGBA.B] = src[RGBA.B];
						dst[RGBA.A] = alpha;
					}
					src += srcOffset;
				}
			}
			// unlock destination image
			source.UnlockBits(srcData);
			destination.UnlockBits(dstData);

			return destination;
		}
			
		private static Bitmap CloneBitmapByUsingGraphics(
			Bitmap source,
			PixelFormat pixelFormat,
			int width,
			int height,
			bool highGraphicsQuality)
		{
			Bitmap bitmap = new Bitmap(width, height, pixelFormat);

			// draw source image on the new one using Graphics
			Graphics g = Graphics.FromImage(bitmap);

			if (highGraphicsQuality)
			{
				g.InterpolationMode = InterpolationMode.HighQualityBicubic;
				g.PixelOffsetMode = PixelOffsetMode.HighQuality;
				g.SmoothingMode = SmoothingMode.HighQuality;
			}
			else
			{
				g.InterpolationMode = InterpolationMode.Bilinear;
				g.PixelOffsetMode = PixelOffsetMode.HighSpeed;
				g.SmoothingMode = SmoothingMode.HighSpeed;
			}

			g.DrawImage(source, 0, 0, width, height);
			g.Dispose();

			if (source.PixelFormat == PixelFormat.Format8bppIndexed) {
				
			}

			return bitmap;
		}
			
		/// <summary>
		/// Converts 8 bpp bitmap to 24 bpp bitmap.
		/// </summary>
		/// <param name="source">The bitmap, which will be converted.</param>
		/// <returns>Result (24 bpp) bitmap.</returns>
		private static Bitmap GrayscaleToRGB(Bitmap source)
		{
			// check image format
			if (source.PixelFormat != PixelFormat.Format8bppIndexed)
				throw new ArgumentException(
					"Source image can be grayscale (8 bpp) image only.");

			int w = source.Width;
			int h = source.Height;
			Rectangle rect = new Rectangle(0, 0, w, h);

			Bitmap destination = new Bitmap(w, h, PixelFormat.Format24bppRgb);
			BitmapData dstData = destination.LockBits
				(rect, ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

			// lock source bitmap data
			BitmapData srcData = source.LockBits(
				rect, ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);

			var srcOffset = srcData.Stride - w;
			var dstOffset = dstData.Stride - w * 3;

			// process image
			unsafe
			{
				byte* src = (byte*)srcData.Scan0.ToPointer();
				byte* dst = (byte*)dstData.Scan0.ToPointer();
				// for each line
				for (int y = 0; y < h; y++)
				{
					// for each pixel in line
					for (int x = 0; x < w; x++, src++, dst += 3)
					{
						dst[RGBA.R] = *src;
						dst[RGBA.G] = *src;
						dst[RGBA.B] = *src;
					}
					src += srcOffset;
					dst += dstOffset;
				}
			}
			// unlock destination image
			source.UnlockBits(srcData);
			destination.UnlockBits(dstData);

			return destination;
		}

//		private static bool Is24Or32BppPixelFormat(PixelFormat format)
//		{
//			// check pixel format
//			if (format != PixelFormat.Format24bppRgb &&
//				format != PixelFormat.Format32bppArgb &&
//				format != PixelFormat.Format32bppRgb &&
//				format != PixelFormat.Format32bppPArgb)
//			{
//				return false;
//			}
//			return true;
//		}

		/// <summary>
		/// Converts 24 bpp bitmap to 8 bpp bitmap by mean value of all 3 channels.
		/// </summary>
		/// <param name="source">The bitmap, which will be converted.</param>
		/// <returns>Result (8 bpp) bitmap.</returns>
		private static Bitmap RGBTo8Bpp(Bitmap source)
		{
			// TODO: use correct factors for grayscale, NOT only buliding mean value

			// check image format
			if (source.PixelFormat != PixelFormat.Format24bppRgb)
				throw new ArgumentException(
					"Source image can be color (24 bpp) image only");

			int w = source.Width;
			int h = source.Height;
			Rectangle rect = new Rectangle(0, 0, w, h);

			Bitmap destination = new Bitmap(w, h, PixelFormat.Format8bppIndexed);
			BitmapData dstData = destination.LockBits
				(rect, ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

			// lock source bitmap data
			BitmapData srcData = source.LockBits(
				rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
			var srcOffset = srcData.Stride - w * 3;
			var dstOffset = dstData.Stride - w;

			// process image
			unsafe
			{
				byte* src = (byte*)srcData.Scan0.ToPointer();
				byte* dst = (byte*)dstData.Scan0.ToPointer();
				// for each line
				for (int y = 0; y < h; y++)
				{
					// for each pixel in line
					for (int x = 0; x < w; x++, src += 3, dst++)
					{
						int avg = src[RGBA.R] + src[RGBA.G] + src[RGBA.B];
						avg /= 3;
						*dst = (byte)avg;
					}
					src += srcOffset;
					dst += dstOffset;
				}
			}
			// unlock destination image
			source.UnlockBits(srcData);
			destination.UnlockBits(dstData);
			ColorPalette.I.SetColorPaletteToGray(destination);

			return destination;
		}

		#endregion Private helper functions


		#region Helper and wrapper for To1Bpp(Bitmap source) methods

		[System.Runtime.InteropServices.DllImport("gdi32.dll")]
		private static extern bool DeleteObject(IntPtr hObject);

		[System.Runtime.InteropServices.DllImport("user32.dll")]
		private static extern IntPtr GetDC(IntPtr hwnd);

		[System.Runtime.InteropServices.DllImport("gdi32.dll")]
		private static extern IntPtr CreateCompatibleDC(IntPtr hdc);

		[System.Runtime.InteropServices.DllImport("user32.dll")]
		private static extern int ReleaseDC(IntPtr hwnd, IntPtr hdc);

		[System.Runtime.InteropServices.DllImport("gdi32.dll")]
		private static extern int DeleteDC(IntPtr hdc);

		[System.Runtime.InteropServices.DllImport("gdi32.dll")]
		private static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

		[System.Runtime.InteropServices.DllImport("gdi32.dll")]
		private static extern int BitBlt(
			IntPtr hdcDst, 
			int xDst, 
			int yDst, 
			int w, 
			int h, 
			IntPtr hdcSrc, 
			int xSrc, 
			int ySrc, 
			int rop);        

		[System.Runtime.InteropServices.DllImport("gdi32.dll")]
		static extern IntPtr CreateDIBSection(
			IntPtr hdc, 
			ref Bitmapinfo bmi, 
			uint usage, 
			out IntPtr bits, 
			IntPtr hSection, 
			uint dwOffset);

		private static uint MakeRgb(int r, int g, int b)
		{
			return ((uint)(b & 255)) | ((uint)((r & 255) << 8)) | ((uint)((g & 255) << 16));
		}

		#endregion Helper and wrapper for To1Bpp(Bitmap source) methods
	}
}