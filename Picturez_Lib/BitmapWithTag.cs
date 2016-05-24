using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using TagLib;
using TagLib.Gif;
using TagLib.IFD;
using TagLib.Image;
using TagLib.Jpeg;
using TagLib.Png;
using NetColor = System.Drawing.Color;
using IOFile = System.IO.File;
using IOPath = System.IO.Path;

namespace Picturez_Lib
{
	public class BitmapWithTag
	{		
		private PicturezImageFormat newFormat;

		public PicturezImageFormat OrigFormat { get; private set; }
		public Bitmap Bitmap { get; private set; }
		public string FileName { get; private set; }
		public CombinedImageTag ImageTag { get; private set; }
		// public Configuration Config { get; private set; }

		public BitmapWithTag (string filename, bool exists)
		{
//			Config = new Configuration ();
//			Config.Path = filename;
			FileName = filename;

			if (exists) {
				Bitmap = new Bitmap (filename);
				OrigFormat = newFormat = ImageFormatConverter.I.ConvertToPIF(Bitmap.RawFormat, Bitmap.PixelFormat);
				// Config.Format = ImageFormatConverter.I.ConvertToPIF(Bitmap.RawFormat);
				ImageTag = ExtractTags (filename);

				if (ImageTag != null && ImageTag.AllTags.Count == 0) 
				{
					switch (OrigFormat) 
					{
					case PicturezImageFormat.PNG1:
					case PicturezImageFormat.PNG8:
					case PicturezImageFormat.PNG24:
					case PicturezImageFormat.PNG32Transparency:
					case PicturezImageFormat.PNG32AlphaAsValue:
							ImageTag.PIC_ENHANCE_AddTag (new PngTag ());
							break;
					case PicturezImageFormat.JPEG8:
					case PicturezImageFormat.JPEG24:
							ImageTag.PIC_ENHANCE_AddTag (new IFDTag ());
							ImageTag.PIC_ENHANCE_AddTag (new JpegCommentTag ());
							break;
					case PicturezImageFormat.TIFF:
							ImageTag.PIC_ENHANCE_AddTag (new IFDTag ());
							break;
					case PicturezImageFormat.GIF:
							ImageTag.PIC_ENHANCE_AddTag (new GifCommentTag ());
							break;
					}
				}
			} 
			else {
				Bitmap = new Bitmap (180, 180, PixelFormat.Format32bppArgb);
				OrigFormat = newFormat = PicturezImageFormat.PNG32AlphaAsValue;
				ImageTag = new CombinedImageTag (TagTypes.Png);
			}				
		}			

		public void ChangeBitmapButNotTags(Bitmap newBitmap)
		{
			Bitmap = newBitmap;
		}

		public void Dispose()
		{
			if (Bitmap != null) {
				Bitmap.Dispose ();
				Bitmap = null;
			}

			if (ImageTag != null) {
				ImageTag.Clear();
			}
		}
			
//		public void SaveAsJpeg(string newFileName, byte quality, bool grayscale)
//		{
//			FileName = newFileName;
//			newFormat = grayscale ? PicturezImageFormat.JPEG8 : PicturezImageFormat.JPEG24;
//			//TODO: Fix quality bug for saving
//			JpegEncoder.SaveJpeg(FileName, Bitmap, quality, grayscale);
//			SaveTag ();
//		}
			
//		public void Save(string newFileName, ImageFormat format)
//		{			
//			FileName = newFileName;
//			newFormat = ImageFormatConverter.I.ConvertToPIF(format, Bitmap.PixelFormat);
//			Bitmap.Save(FileName, format);
//			SaveTag ();
//		}			

		public void Save(ConfigConvert config, string relativeFileName)
		{			
			Bitmap dest;
			int w = Bitmap.Width;
			int h = Bitmap.Height;
			newFormat = config.Format;

			if (config.UseOriginalPath) {
				config.Path = System.IO.Directory.GetParent(FileName).FullName;
			}

			#region FileOverwriting
			string tmpNewFileName;
			if (config.Path[config.Path.Length -1] == IOPath.DirectorySeparatorChar)
				tmpNewFileName = config.Path + relativeFileName;
			else 
				tmpNewFileName = config.Path + IOPath.DirectorySeparatorChar + relativeFileName;

			if (config.StretchImage != ConvertMode.Editor)
			{
				if (config.FileOverwriting){
					if (FileName != tmpNewFileName){
						IOFile.Delete(FileName);
					}
				}else {
					int countImage = 0;
					while(FileHelper.I.Exists(tmpNewFileName)){
						countImage++;
						string identifier = "__n";
						int lastindexofIdentifier = tmpNewFileName.LastIndexOf(identifier);
						if (lastindexofIdentifier != -1){
							string part1 = tmpNewFileName.Remove(lastindexofIdentifier);
							string part2 = tmpNewFileName.Substring(tmpNewFileName.LastIndexOf('.'));
							tmpNewFileName = part1 + part2;
						}

						tmpNewFileName = tmpNewFileName.Insert(tmpNewFileName.LastIndexOf('.'), identifier + countImage);				
					}
				}
			}
			FileName = tmpNewFileName;
			#endregion FileOverwriting

			#region Resizing
			switch (config.ResizeVersion) {
//			case ResizeVersion.No:
//				break;
			case ResizeVersion.BiggestLength:
				ImageConverter.CalcBiggerSideLength (config.BiggestLength, ref w, ref h);
				break;
			case ResizeVersion.FixedSize:
				w = config.Width;
				h = config.Height;
				break;
			}

			// TODO: Was already merged with ScaleAndCut(..)-method, experimaental, take care.
//			if (config.Format == PicturezImageFormat.PNG32AlphaAsValue)	{
//				RectangleF rec = ImageConverter.GetRectangle(Bitmap.Width, Bitmap.Height, 0,0 , w, h, config.StretchImage);
//				dest = null;
//				dest = Bitmap.Clone(rec, PixelFormat.Format32bppArgb);
//			}else{
				ImageConverter.ScaleAndCut (Bitmap, 
					out dest,
					0,
					0,
					w,
					h,
					config.StretchImage,
					config.HighQuality);
//			}
			#endregion Resizing

			#region Saving by using PicturezImageFormat
			switch (config.Format) {
			case PicturezImageFormat.JPEG8:
				JpegEncoder.SaveJpeg (FileName, dest, config.JpgQuality, true);
				SaveTag ();
				return;
			case PicturezImageFormat.JPEG24:
				JpegEncoder.SaveJpeg (FileName, dest, config.JpgQuality, false);
				SaveTag ();
				return;
			case PicturezImageFormat.BMP1:
			case PicturezImageFormat.PNG1:
				if (Constants.I.WINDOWS) {
					dest = ImageConverter.To1Bpp (dest);
				} else {
					// throw new NotImplementedException ("Not implemented for linux yet.");
				}
				break;
			case PicturezImageFormat.BMP8:
			case PicturezImageFormat.PNG8:
				dest = ImageConverter.To8Bpp (dest);
				break;
			case PicturezImageFormat.BMP24:
			case PicturezImageFormat.EMF:
			case PicturezImageFormat.PNG24:
			case PicturezImageFormat.TIFF:
			case PicturezImageFormat.WMF:
				// TODO: Correct for EMF, TIFF, WMF converting to 24 bit? Alpha?				
				dest = ImageConverter.To24Bpp (dest);
				break;
			case PicturezImageFormat.GIF:
			case PicturezImageFormat.ICO:
			case PicturezImageFormat.PNG32Transparency:
				// TODO: Correct for GIF, ICON using to 32 bit?
				// MakeTransparent() makes EVERY (also 1bpp) pixel format to 32bit ARGB
				dest.MakeTransparent (NetColor.FromArgb (config.TransparencyColorRed,
					config.TransparencyColorGreen, config.TransparencyColorBlue));
				break;
			case PicturezImageFormat.PNG32AlphaAsValue:
				dest = ImageConverter.To32Bpp (dest);
				break;
			}								
			dest.Save(FileName, ImageFormatConverter.I.ConvertFromPIF(config.Format));
			#endregion Saving by using PicturezImageFormat

			Bitmap = dest;
			SaveTag ();
		}

		private void SaveTag()
		{
			if (ImageTag == null)
				return;

			TagLib.File tagFile;
			try{
				tagFile = TagLib.File.Create(FileName);
			}
			catch (Exception /* UnsupportedFormatException */) {
				return;
			}

			var image = tagFile as TagLib.Image.File;
//			// TODO examples for setting properties manually
//			ImageTag.Software = "Converted by Picturez 3.0";
//			ImageTag.Comment = "El commentaro";
//			ImageTag.Creator = "Max Mustermann";

			// if (OrigFormat == newFormat) {
			if (ImageFormatConverter.I.IsSameDotNetFormat(OrigFormat, newFormat)) {
				image.PIC_ENHANCE_SetImageTag(ImageTag);
				image.Save();
				tagFile.Dispose ();
				return;
			}

			CombinedImageTag ctag;

			switch (newFormat) {
			case PicturezImageFormat.PNG1:
			case PicturezImageFormat.PNG8:
			case PicturezImageFormat.PNG24:
			case PicturezImageFormat.PNG32Transparency:
			case PicturezImageFormat.PNG32AlphaAsValue:
				//image.GetTag (TagTypes.Png, true);
				ctag = new CombinedImageTag (TagTypes.Png);
				PngTag pngTag = image.GetTag (TagTypes.Png, true) as PngTag;
				ImageTag.CopyTo(pngTag, true);
				ctag.PIC_ENHANCE_AddTag (pngTag);
				image.PIC_ENHANCE_SetImageTag(ctag);
				break;
			case PicturezImageFormat.JPEG8:
			case PicturezImageFormat.JPEG24:
				ctag = new CombinedImageTag (TagTypes.TiffIFD | TagTypes.JpegComment);
				IFDTag jpgIfdTag = image.GetTag (TagTypes.TiffIFD, true) as IFDTag;
				// TODO: Why using JpegCommentTag? Comment is also in other tags (e.g. IFDTag)
				//JpegCommentTag jpgComTag = image.GetTag (TagTypes.JpegComment, true) as JpegCommentTag;
				ImageTag.CopyTo(jpgIfdTag, true);
				//imageTag.CopyTo(jpgComTag, true);
				ctag.PIC_ENHANCE_AddTag (jpgIfdTag);
				//ctag.PIC_ENHANCE_AddTag (jpgComTag);
				image.PIC_ENHANCE_SetImageTag(ctag);
				break;
			case PicturezImageFormat.TIFF:
				ctag = new CombinedImageTag (TagTypes.TiffIFD);
				IFDTag ifdTag = image.GetTag (TagTypes.TiffIFD, true) as IFDTag;
				ImageTag.CopyTo(ifdTag, true);
				ctag.PIC_ENHANCE_AddTag (ifdTag);
				image.PIC_ENHANCE_SetImageTag(ctag);
				break;
			case PicturezImageFormat.GIF:
				ctag = new CombinedImageTag (TagTypes.GifComment);
				GifCommentTag gifTag = new GifCommentTag (ImageTag.Comment);
				ctag.PIC_ENHANCE_AddTag (gifTag);
				image.PIC_ENHANCE_SetImageTag(ctag);
				break;
//			case DotNetImageFormatEnum.Bmp:
//			case DotNetImageFormatEnum.Wmf:
//			case DotNetImageFormatEnum.Emf:
//			case DotNetImageFormatEnum.Icon:
			default:
				// no metadata	
				break;
			}

			image.Save();
			tagFile.Dispose ();
		}

		private static CombinedImageTag ExtractTags(string fileName)
		{
			TagLib.File tagFile;
			try{
				tagFile = TagLib.File.Create(fileName);
			}
			catch (Exception /* UnsupportedFormatException */) {
				return null;
			}

			var image = tagFile as TagLib.Image.File;
			CombinedImageTag tag = image.ImageTag;
			tagFile.Dispose ();
			return tag;
		}
	}
}

