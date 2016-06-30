using System.Drawing.Imaging;
using System;

namespace Troonie_Lib
{
	// TroonieImageFormatEnum
	public enum TroonieImageFormat
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

		public ImageFormat ConvertFromPIF(TroonieImageFormat f)
		{
			switch (f) {
			case TroonieImageFormat.BMP1:
			case TroonieImageFormat.BMP24:
			case TroonieImageFormat.BMP8:
				return ImageFormat.Bmp;
			case TroonieImageFormat.EMF:
				return ImageFormat.Emf;
			case TroonieImageFormat.GIF:
				return ImageFormat.Gif;
			case TroonieImageFormat.ICO:
				return ImageFormat.Icon;
			case TroonieImageFormat.JPEG24:
			case TroonieImageFormat.JPEG8:
				return ImageFormat.Jpeg;
			case TroonieImageFormat.PNG1:
			case TroonieImageFormat.PNG24:
			case TroonieImageFormat.PNG32Transparency:
			case TroonieImageFormat.PNG32AlphaAsValue:
			case TroonieImageFormat.PNG8:
				return ImageFormat.Png;
			case TroonieImageFormat.TIFF:
				return ImageFormat.Tiff;
			case TroonieImageFormat.WMF:
				return ImageFormat.Wmf;
			default:
				return null;
			}
		}

		public TroonieImageFormat ConvertToPIF(ImageFormat f, PixelFormat pf)
		{
			if (f.Guid == ImageFormat.Bmp.Guid) {
				switch (pf) {
				case PixelFormat.Format1bppIndexed:
					return TroonieImageFormat.BMP1;
				case PixelFormat.Format8bppIndexed:
					return TroonieImageFormat.BMP8;
				case PixelFormat.Format24bppRgb:
					return TroonieImageFormat.BMP24; 
				default:
					return TroonieImageFormat.Undefined;
				}						
			}
			else if (f.Guid == ImageFormat.Emf.Guid)
				return TroonieImageFormat.EMF;
			else if (f.Guid == ImageFormat.Gif.Guid)
				return TroonieImageFormat.GIF;
			else if (f.Guid == ImageFormat.Icon.Guid)
				return TroonieImageFormat.ICO;
			else if (f.Guid == ImageFormat.Jpeg.Guid) {
				switch (pf) {
				case PixelFormat.Format8bppIndexed:
					return TroonieImageFormat.JPEG8;
				case PixelFormat.Format24bppRgb:
					return TroonieImageFormat.JPEG24; 
				default:
					return TroonieImageFormat.Undefined;
				}						
			}
			else if (f.Guid == ImageFormat.Png.Guid) {
				switch (pf) {
				case PixelFormat.Format1bppIndexed:
					return TroonieImageFormat.PNG1;
				case PixelFormat.Format8bppIndexed:
					return TroonieImageFormat.PNG8;
				case PixelFormat.Format24bppRgb:
					return TroonieImageFormat.PNG24; 
				case PixelFormat.Format32bppArgb:
				case PixelFormat.Format32bppPArgb:
				case PixelFormat.Format32bppRgb:
					return TroonieImageFormat.PNG32AlphaAsValue; 
				default:
					return TroonieImageFormat.Undefined;
				}						
			}
			else if (f.Guid == ImageFormat.Tiff.Guid)
				return TroonieImageFormat.TIFF;		
			else if (f.Guid == ImageFormat.Wmf.Guid)
				return TroonieImageFormat.WMF;
			
			return TroonieImageFormat.Undefined;
			//throw new NotSupportedException ("Image format is not supported.");
		}

		public bool IsSameDotNetFormat(TroonieImageFormat format1, TroonieImageFormat format2)
		{
			return GetGuid (format1) == GetGuid (format2);
		}
			
		private Guid GetGuid(TroonieImageFormat format)
		{
			return ConvertFromPIF (format).Guid;	
		}

//		private Guid GetGuid(TroonieImageFormat format)
//		{
//			switch (format) {
//			case TroonieImageFormat.BMP1:
//			case TroonieImageFormat.BMP24:
//			case TroonieImageFormat.BMP8:
//				return ImageFormat.Bmp.Guid;
//			case TroonieImageFormat.EMF:
//				return ImageFormat.Emf.Guid;
//			case TroonieImageFormat.GIF:
//				return ImageFormat.Gif.Guid;
//			case TroonieImageFormat.ICO:
//				return ImageFormat.Icon.Guid;
//			case TroonieImageFormat.JPEG24:
//			case TroonieImageFormat.JPEG8:
//				return ImageFormat.Jpeg.Guid;
//			case TroonieImageFormat.PNG1:
//			case TroonieImageFormat.PNG24:
//			case TroonieImageFormat.PNG32Alpha:
//			case TroonieImageFormat.PNG8:
//				return ImageFormat.Png.Guid;
//			case TroonieImageFormat.TIFF:
//				return ImageFormat.Tiff.Guid;
//			case TroonieImageFormat.WMF:
//				return ImageFormat.Wmf.Guid;
//			default:
//				return new Guid(new byte[]{0});
//			}							
//		}
	}
}

