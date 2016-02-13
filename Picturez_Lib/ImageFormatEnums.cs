using System.Drawing.Imaging;
using System;

namespace Picturez_Lib
{
	// PicturezImageFormatEnum
	public enum PicturezImageFormat
	{
		JPEG24,
		JPEG8,
		PNG1,
		PNG8,
		PNG24,
		/// <summary> Usage for transparency for a color, only passed color is transparent, 
		/// all other pixel have max value (alpha = 255).  </summary>
		PNG32Transparency,
		/// <summary>  Usage for alpha as a channel value, every pixel has own alpha value, 
		/// e.g. steganography filter. </summary>
		PNG32AlphaAsValue,
		BMP1,
		BMP8,
		BMP24,
		TIFF,
		GIF,
		WMF,
		EMF,
		ICO,
		Undefined
	} 

//	public enum DotNetImageFormat
//	{
//		Bmp,
//		Emf,
//		Exif,
//		Gif,
//		Icon,
//		Jpeg,
//		MemoryBmp,
//		Png,
//		Tiff,
//		Wmf,
//		Undefined
//	}

	public class ImageFormatConverter
	{
		private static ImageFormatConverter instance;
		public static ImageFormatConverter I
		{
			get
			{
				if (instance == null)
					instance = new ImageFormatConverter ();

				return instance;
			}
		}

//		// TODO: Complete it.
//		public DotNetImageFormat ConvertTo(ImageFormat f)
//		{
//			if (f.Guid == ImageFormat.Bmp.Guid)
//				return DotNetImageFormat.Bmp;
//			else if (f.Guid == ImageFormat.Jpeg.Guid)
//				return DotNetImageFormat.Jpeg;
//			else if (f.Guid == ImageFormat.Png.Guid)
//				return DotNetImageFormat.Png;
//			else if (f.Guid == ImageFormat.Tiff.Guid)
//				return DotNetImageFormat.Tiff;		
//			else if (f.Guid == ImageFormat.Gif.Guid)
//				return DotNetImageFormat.Gif;	
//
//			return DotNetImageFormat.Undefined;
//			//throw new NotSupportedException ("Image format is not supported.");
//		}

		public ImageFormat ConvertFromPIF(PicturezImageFormat f)
		{
			switch (f) {
			case PicturezImageFormat.BMP1:
			case PicturezImageFormat.BMP24:
			case PicturezImageFormat.BMP8:
				return ImageFormat.Bmp;
			case PicturezImageFormat.EMF:
				return ImageFormat.Emf;
			case PicturezImageFormat.GIF:
				return ImageFormat.Gif;
			case PicturezImageFormat.ICO:
				return ImageFormat.Icon;
			case PicturezImageFormat.JPEG24:
			case PicturezImageFormat.JPEG8:
				return ImageFormat.Jpeg;
			case PicturezImageFormat.PNG1:
			case PicturezImageFormat.PNG24:
			case PicturezImageFormat.PNG32Transparency:
			case PicturezImageFormat.PNG32AlphaAsValue:
			case PicturezImageFormat.PNG8:
				return ImageFormat.Png;
			case PicturezImageFormat.TIFF:
				return ImageFormat.Tiff;
			case PicturezImageFormat.WMF:
				return ImageFormat.Wmf;
			default:
				return null;
			}
		}

		public PicturezImageFormat ConvertToPIF(ImageFormat f, PixelFormat pf)
		{
			if (f.Guid == ImageFormat.Bmp.Guid) {
				switch (pf) {
				case PixelFormat.Format1bppIndexed:
					return PicturezImageFormat.BMP1;
				case PixelFormat.Format8bppIndexed:
					return PicturezImageFormat.BMP8;
				case PixelFormat.Format24bppRgb:
					return PicturezImageFormat.BMP24; 
				default:
					return PicturezImageFormat.Undefined;
				}						
			}
			else if (f.Guid == ImageFormat.Emf.Guid)
				return PicturezImageFormat.EMF;
			else if (f.Guid == ImageFormat.Gif.Guid)
				return PicturezImageFormat.GIF;
			else if (f.Guid == ImageFormat.Icon.Guid)
				return PicturezImageFormat.ICO;
			else if (f.Guid == ImageFormat.Jpeg.Guid) {
				switch (pf) {
				case PixelFormat.Format8bppIndexed:
					return PicturezImageFormat.JPEG8;
				case PixelFormat.Format24bppRgb:
					return PicturezImageFormat.JPEG24; 
				default:
					return PicturezImageFormat.Undefined;
				}						
			}
			else if (f.Guid == ImageFormat.Png.Guid) {
				switch (pf) {
				case PixelFormat.Format1bppIndexed:
					return PicturezImageFormat.PNG1;
				case PixelFormat.Format8bppIndexed:
					return PicturezImageFormat.PNG8;
				case PixelFormat.Format24bppRgb:
					return PicturezImageFormat.PNG24; 
				case PixelFormat.Format32bppArgb:
				case PixelFormat.Format32bppPArgb:
				case PixelFormat.Format32bppRgb:
					return PicturezImageFormat.PNG32AlphaAsValue; 
				default:
					return PicturezImageFormat.Undefined;
				}						
			}
			else if (f.Guid == ImageFormat.Tiff.Guid)
				return PicturezImageFormat.TIFF;		
			else if (f.Guid == ImageFormat.Wmf.Guid)
				return PicturezImageFormat.WMF;
			
			return PicturezImageFormat.Undefined;
			//throw new NotSupportedException ("Image format is not supported.");
		}

		public bool IsSameDotNetFormat(PicturezImageFormat format1, PicturezImageFormat format2)
		{
			return GetGuid (format1) == GetGuid (format2);
		}
			
		private Guid GetGuid(PicturezImageFormat format)
		{
			return ConvertFromPIF (format).Guid;	
		}

//		private Guid GetGuid(PicturezImageFormat format)
//		{
//			switch (format) {
//			case PicturezImageFormat.BMP1:
//			case PicturezImageFormat.BMP24:
//			case PicturezImageFormat.BMP8:
//				return ImageFormat.Bmp.Guid;
//			case PicturezImageFormat.EMF:
//				return ImageFormat.Emf.Guid;
//			case PicturezImageFormat.GIF:
//				return ImageFormat.Gif.Guid;
//			case PicturezImageFormat.ICO:
//				return ImageFormat.Icon.Guid;
//			case PicturezImageFormat.JPEG24:
//			case PicturezImageFormat.JPEG8:
//				return ImageFormat.Jpeg.Guid;
//			case PicturezImageFormat.PNG1:
//			case PicturezImageFormat.PNG24:
//			case PicturezImageFormat.PNG32Alpha:
//			case PicturezImageFormat.PNG8:
//				return ImageFormat.Png.Guid;
//			case PicturezImageFormat.TIFF:
//				return ImageFormat.Tiff.Guid;
//			case PicturezImageFormat.WMF:
//				return ImageFormat.Wmf.Guid;
//			default:
//				return new Guid(new byte[]{0});
//			}							
//		}
	}
}

