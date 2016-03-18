using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Picturez_Lib
{    
    public abstract class AbstractFilter
    {
		private string errorMsg = "No supported source pixel format.";
		private PixelFormatFlags supportedSrcPixelFormat;
		private PixelFormatFlags supportedDstPixelFormat;
		private PixelFormat dstPixelFormat;

		/// <summary>
		/// If <c>true</c> alpha channel of filtered image is always 255 (only in ARGB 32 bit images). 
		/// Otherwise channel will be processed like the other RGB channels.
		/// Default: <c>true</c>.
		/// </summary>
		public bool Use255ForAlpha { get; set; }

		/// <summary>Defines supported pixel format for result image of the 
		/// filter.</summary>
		public PixelFormatFlags SupportedDstPixelFormat
		{
			get { return supportedDstPixelFormat; }
			protected set
			{
				supportedDstPixelFormat = value;
				switch (value)
				{
					case PixelFormatFlags.Format1bppIndexed:
					dstPixelFormat = PixelFormat.Format1bppIndexed;
					break;					
					case PixelFormatFlags.Format8BppIndexed:
					dstPixelFormat = PixelFormat.Format8bppIndexed;
					break;
					case PixelFormatFlags.Format24BppRgb:
					dstPixelFormat = PixelFormat.Format24bppRgb;
					break;
					case (PixelFormatFlags.Format32BppArgb):
					dstPixelFormat = PixelFormat.Format32bppArgb;
					break;
					case PixelFormatFlags.Format32BppRgb:
					dstPixelFormat = PixelFormat.Format32bppRgb;
					break;
					case PixelFormatFlags.SameLikeSource:
					// dstPixelFormat will be set in CheckPixelFormat()
					break;
					default:
					throw new ArgumentException ("Value of SupportedDstPixelFormat needs to be unique.", 
					                             "SupportedDstPixelFormat");
				}
			}
		}

		/// <summary>Defines supported pixel formats of the source image for the 
		/// filter.</summary>
		public PixelFormatFlags SupportedSrcPixelFormat
		{
			get { return supportedSrcPixelFormat; }
			protected set
			{
				supportedSrcPixelFormat = value;
				switch (value)
				{
					case PixelFormatFlags.None:
					errorMsg = "No supported pixel format of source image.";
					break;
					case PixelFormatFlags.Format1bppIndexed:
					errorMsg = "Source image can be monochrome black-white (1 bpp) image only.";
					break;
					case PixelFormatFlags.Format8BppIndexed:
					errorMsg = "Source image can be grayscale (8 bpp) image only.";
					break;
					case PixelFormatFlags.Format24BppRgb:
					errorMsg = "Source image can be color (24 bpp) image only.";
					break;
					case (PixelFormatFlags.Format32BppArgb | PixelFormatFlags.Format32BppRgb):
					errorMsg = "Source image can be color (32 bpp) image only.";
					break;
					case PixelFormatFlags.Format24And8Bpp:
					errorMsg = "Source image can be color (24 bpp) image" + 
						" or grayscale (8 bpp) image only.";
					break;
					case PixelFormatFlags.Format32And8Bpp:
					errorMsg = "Source image can be color (32 bpp) image" +
						" or grayscale (8 bpp) image only.";
					break;
					case PixelFormatFlags.Color:
					errorMsg = "Source image can be color (24 or 32 bpp) image only.";
					break;
					case PixelFormatFlags.All:
					errorMsg = "Source image can be color (24 or 32 bpp) image" +
						" or grayscale (8 bpp) image only.";
					break;
				}
			} 
		}

		protected AbstractFilter()
		{
			Use255ForAlpha = true;
		}

		/// <summary>
		/// Processes the filter on the passed <paramref name="srcData"/> 
		/// resulting into <paramref name="dstData"/>.
		/// </summary>
		/// <param name="srcData">The source bitmap data.</param>
		/// <param name="dstData">The destination bitmap data.</param>
		protected abstract internal void Process(BitmapData srcData, BitmapData dstData);

		/// <summary>
		/// Checks, if the <paramref name="format"/> is supported by the filter. 
		/// <seealso cref="SupportedSrcPixelFormats"/>
		/// </summary>
		/// <param name="format">The format.</param>
		/// <exception cref="ArgumentException">Pixelformat of source or dstination image 
		/// cannot be processed.</exception>
		protected void CheckPixelFormat(PixelFormat format)
		{
			PixelFormatFlags flags;
			switch (format)
			{
				case PixelFormat.Format1bppIndexed:
				flags = PixelFormatFlags.Format1bppIndexed;
				break;
				case PixelFormat.Format8bppIndexed:
				flags = PixelFormatFlags.Format8BppIndexed;
				break;
				case PixelFormat.Format24bppRgb:
				flags = PixelFormatFlags.Format24BppRgb;
				break;
				case PixelFormat.Format32bppArgb:
				flags = PixelFormatFlags.Format32BppArgb;
				break;
				case PixelFormat.Format32bppRgb:
				flags = PixelFormatFlags.Format32BppRgb;
				break;
				default:
				flags = PixelFormatFlags.None;
				break;
			}

			if ((flags & supportedSrcPixelFormat) == 0)
			{
				throw new ArgumentException(errorMsg);
			}        

			if (supportedDstPixelFormat == PixelFormatFlags.None) {
				errorMsg = "No supported pixel format of destination image.";
				throw new ArgumentException(errorMsg);
			}
			else if (supportedDstPixelFormat == PixelFormatFlags.SameLikeSource) {
				dstPixelFormat = format;
			}
		}

		protected virtual void SetColorPalette(Bitmap b)
		{
			ColorPalette.I.SetColorPaletteToGray (b);
		}

		/// <summary>
		/// Applies the filter on the passed <paramref name="source"/> bitmap.
		/// </summary>
		/// <param name="source">The source image to process.</param>
		/// <returns>The filter result as a new bitmap.</returns>
		public Bitmap Apply(Bitmap source)
		{
			CheckPixelFormat(source.PixelFormat);
			Bitmap destination = new Bitmap(source.Width, source.Height, dstPixelFormat);
			Rectangle rect = new Rectangle(0, 0, source.Width, source.Height);
			BitmapData srcData = source.LockBits(rect, ImageLockMode.ReadWrite, source.PixelFormat);
			BitmapData dstData = destination.LockBits(rect, ImageLockMode.ReadWrite, destination.PixelFormat);
			Process(srcData, dstData);
			destination.UnlockBits(dstData);
			source.UnlockBits(srcData);

			if (destination.PixelFormat == PixelFormat.Format8bppIndexed)
			{
				SetColorPalette(destination);
			}

			return destination;
		}
    }
}
