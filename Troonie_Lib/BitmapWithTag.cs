using System;
using System.Drawing;
using System.Drawing.Imaging;
using TagLib;
using TagLib.Image;
using NetColor = System.Drawing.Color;
using IOFile = System.IO.File;
using IOPath = System.IO.Path;

namespace Troonie_Lib
{
	public class BitmapWithTag
	{		
		public TroonieImageFormat OrigFormat { get; private set; }
		public Bitmap Bitmap { get; private set; }
		public string FileName { get; private set; }
		public CombinedImageTag ImageTag { get; private set; }

		public BitmapWithTag (string filename, bool exists)
		{
			FileName = filename;

			if (exists) {
				Bitmap = new Bitmap (filename);
				OrigFormat = ImageFormatConverter.I.ConvertToPIF(Bitmap.RawFormat, Bitmap.PixelFormat);
				ImageTag = ExtractImageTag (filename);
			} 
			else {
				Bitmap = new Bitmap (180, 180, PixelFormat.Format32bppArgb);
				OrigFormat = TroonieImageFormat.PNG32AlphaAsValue;
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
				try {
					ImageTag.Clear();
				} catch (NotImplementedException) { /* do nothing */ }
			}
		}	

		public void ChangeImageTag(string tagName, string newValue)
		{
			switch (tagName) {
			case "Creator":
				ImageTag.Creator = newValue;
				break;
			case "Conductor":
				ImageTag.Conductor = newValue;
				break;
			case "Copyright":
				ImageTag.Copyright = newValue;
				break;
			default:
				throw new NotImplementedException ();
			}				
		}

		public void Save(Config config, string relativeFileName)
		{			
			Bitmap dest;
			int w = Bitmap.Width;
			int h = Bitmap.Height;

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

			ImageConverter.ScaleAndCut (Bitmap, 
				out dest,
				0,
				0,
				w,
				h,
				config.StretchImage,
				config.HighQuality);
			#endregion Resizing

			#region Saving by using TroonieImageFormat
			switch (config.Format) {
			case TroonieImageFormat.JPEG8:
				JpegEncoder.SaveJpeg (FileName, dest, config.JpgQuality, true);
				CopyTagToFile (FileName, ImageTag);
				return;
			case TroonieImageFormat.JPEG24:
				JpegEncoder.SaveJpeg (FileName, dest, config.JpgQuality, false);
				CopyTagToFile (FileName, ImageTag);
				return;
			case TroonieImageFormat.BMP1:
			case TroonieImageFormat.PNG1:
				if (Constants.I.WINDOWS) {
					dest = ImageConverter.To1Bpp (dest);
				} else {
					// throw new NotImplementedException ("Not implemented for linux yet.");
				}
				break;
			case TroonieImageFormat.BMP8:
			case TroonieImageFormat.PNG8:
				dest = ImageConverter.To8Bpp (dest);
				break;
			case TroonieImageFormat.BMP24:
			case TroonieImageFormat.EMF:
			case TroonieImageFormat.PNG24:
			case TroonieImageFormat.TIFF:
			case TroonieImageFormat.WMF:
				// TODO: Correct for EMF, TIFF, WMF converting to 24 bit? Alpha?				
				dest = ImageConverter.To24Bpp (dest);
				break;
			case TroonieImageFormat.GIF:
			case TroonieImageFormat.ICO:
			case TroonieImageFormat.PNG32Transparency:
				// TODO: Correct for GIF, ICON using to 32 bit?
				// MakeTransparent() makes EVERY (also 1bpp) pixel format to 32bit ARGB
				dest.MakeTransparent (NetColor.FromArgb (config.TransparencyColorRed,
					config.TransparencyColorGreen, config.TransparencyColorBlue));
				break;
			case TroonieImageFormat.PNG32AlphaAsValue:
				dest = ImageConverter.To32Bpp (dest);
				break;
			}								
			dest.Save(FileName, ImageFormatConverter.I.ConvertFromPIF(config.Format));
			#endregion Saving by using TroonieImageFormat

			Bitmap = dest;
			CopyTagToFile (FileName, ImageTag);
		}

		#region static taglib stuff

		private static TagLib.Image.File LoadTagFile(string fileName)
		{
			TagLib.Image.File imageTagFile;
			try{
				imageTagFile = TagLib.File.Create(fileName) as TagLib.Image.File;
				if (imageTagFile == null){
					return null;
				}
			}
			catch (Exception /* UnsupportedFormatException */) {
				return null;
			}				

			// comment out comes from earlier version of method ExtractTags(..)
			//			if (imageTagFile.ImageTag != null && imageTagFile.ImageTag.AllTags.Count == 0) {
			imageTagFile.EnsureAvailableTags ();
			//			}			

			return imageTagFile;
		}

		private static void CopyTagToFile(string fileName, CombinedImageTag tag)
		{
//			TagLib.Image.File imageTagFile;
//			try{
//				imageTagFile = TagLib.File.Create(fileName) as TagLib.Image.File;
//			}
//			catch (Exception /* UnsupportedFormatException */) {
//				return;
//			}
//
//			imageTagFile.EnsureAvailableTags();

			TagLib.Image.File imageTagFile = LoadTagFile(fileName);

			if (tag == null || imageTagFile == null)
				return;
			
			// all general tags
			tag.CopyTo(imageTagFile.ImageTag, true);

			// all image tags
			if (tag.Keywords != null) imageTagFile.ImageTag.Keywords = tag.Keywords;
			if (tag.Rating != null) imageTagFile.ImageTag.Rating = tag.Rating;
			if (tag.DateTime != null) imageTagFile.ImageTag.DateTime = tag.DateTime;
			imageTagFile.ImageTag.Orientation = tag.Orientation;
			if (tag.Software != null) imageTagFile.ImageTag.Software = tag.Software;
			if (tag.Latitude != null) imageTagFile.ImageTag.Latitude = tag.Latitude;
			if (tag.Longitude != null) imageTagFile.ImageTag.Longitude = tag.Longitude;
			if (tag.Altitude != null) imageTagFile.ImageTag.Altitude = tag.Altitude;
			if (tag.ExposureTime != null) imageTagFile.ImageTag.ExposureTime = tag.ExposureTime;
			if (tag.FNumber != null) imageTagFile.ImageTag.FNumber = tag.FNumber;
			if (tag.ISOSpeedRatings != null) imageTagFile.ImageTag.ISOSpeedRatings = tag.ISOSpeedRatings;
			if (tag.FocalLength != null) imageTagFile.ImageTag.FocalLength = tag.FocalLength;
			if (tag.FocalLengthIn35mmFilm != null) imageTagFile.ImageTag.FocalLengthIn35mmFilm = tag.FocalLengthIn35mmFilm;
			if (tag.Make != null) imageTagFile.ImageTag.Make = tag.Make;
			if (tag.Model != null) imageTagFile.ImageTag.Model = tag.Model;
			if (tag.Creator != null) imageTagFile.ImageTag.Creator = tag.Creator;

			// examples for setting properties manually
//			imageTagFile.ImageTag.Creator = "MARKI";
//			imageTagFile.ImageTag.FocalLength = 33.0;
//			imageTagFile.ImageTag.Rating = 5;

			try{
				imageTagFile.Save();
			}
			catch (Exception /* UnsupportedFormatException */) {
				// do nothing
			}
			
			imageTagFile.Dispose ();
		}

		private static CombinedImageTag ExtractImageTag(string fileName)
		{
//			TagLib.Image.File imageTagFile;
//			try{
//				imageTagFile = TagLib.File.Create(fileName) as TagLib.Image.File;
//				if (imageTagFile == null){
//					return null;
//				}
//			}
//			catch (Exception /* UnsupportedFormatException */) {
//				return null;
//			}				
//
////			if (imageTagFile.ImageTag != null && imageTagFile.ImageTag.AllTags.Count == 0) {
//				imageTagFile.EnsureAvailableTags ();
////			}

			TagLib.Image.File imageTagFile = LoadTagFile(fileName);

			if (imageTagFile == null)
				return null;
			
			CombinedImageTag tag = imageTagFile.ImageTag;
			imageTagFile.Dispose ();
			return tag;
		}
			
		public static int GetImageRating(string fileName)
		{
			CombinedImageTag tag = ExtractImageTag (fileName);
			if (tag == null || tag.Rating == null) {
				return -1;
			} else {
				return (int)tag.Rating;
			}
		}

		public static void SetAndSaveTag(string fileName, string tagName, string newValue)
		{
			TagLib.Image.File imageTagFile = LoadTagFile (fileName);
			CombinedImageTag imageTag = imageTagFile.ImageTag;

			switch (tagName) {
			case "Creator":
				imageTag.Creator = newValue;
				break;
			case "Conductor":
				imageTag.Conductor = newValue;
				break;
			case "Copyright":
				imageTag.Copyright = newValue;
				break;
			default:
				throw new NotImplementedException ();
			}				

			try{
				imageTagFile.Save();
			}
			catch (Exception /* UnsupportedFormatException */) {
				// do nothing
			}

			imageTagFile.Dispose ();
		}

		#endregion taglib stuff
	}
}

