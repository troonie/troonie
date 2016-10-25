using System;
using System.Drawing;
using System.Drawing.Imaging;
using TagLib;
using TagLib.Image;
using NetColor = System.Drawing.Color;
using NetIOFile = System.IO.File;
using IOPath = System.IO.Path;

namespace Troonie_Lib
{
	public class BitmapWithTag
	{		
		private CombinedImageTag imageTag;

		public TroonieImageFormat OrigFormat { get; private set; }
		public Bitmap Bitmap { get; private set; }
		public string FileName { get; private set; }

		public BitmapWithTag (string filename, bool exists)
		{
			FileName = filename;

			if (exists) {
				Bitmap = new Bitmap (filename);
				OrigFormat = ImageFormatConverter.I.ConvertToPIF(Bitmap.RawFormat, Bitmap.PixelFormat);
				imageTag = ImageTagHelper.ExtractImageTag (filename);
			} 
			else {
				Bitmap = new Bitmap (180, 180, PixelFormat.Format32bppArgb);
				OrigFormat = TroonieImageFormat.PNG32AlphaAsValue;
				imageTag = new CombinedImageTag (TagTypes.Png);
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

			if (imageTag != null) {
				try {
					imageTag.Clear();
				} catch (NotImplementedException) { /* do nothing */ }
			}
		}				

		public bool Save(Config config, string relativeFileName, bool saveTag)
		{			
			bool success = true;
			try 
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
							NetIOFile.Delete(FileName);
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
					if (saveTag) {
						ImageTagHelper.CopyTagToFile (FileName, imageTag);
					}
					return success;
				case TroonieImageFormat.JPEG24:
					JpegEncoder.SaveJpeg (FileName, dest, config.JpgQuality, false);
					if (saveTag) {
						ImageTagHelper.CopyTagToFile (FileName, imageTag);
					}
					return success;
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

				if (saveTag) {
					ImageTagHelper.CopyTagToFile (FileName, imageTag);
				}

			}
			catch (Exception ) {
				success = false;
			}

			return success;
		}


		#region taglib stuff

		public bool ChangeValueOfTag(Tags tag, string newValue)
		{
			bool success = true;

			try{
				ImageTagHelper.ChangeValueOfTag (imageTag, tag, newValue);
			}
			catch (Exception /* UnsupportedFormatException */) {
				success = false;
			}

			return success;
		}

		public bool ChangeValueOfTag(Tags tag, int newValue)
		{
			bool success = true;

			try{
				ImageTagHelper.ChangeValueOfTag (imageTag, tag, newValue);
			}
			catch (Exception /* UnsupportedFormatException */) {
				success = false;
			}

			return success;
		}
					
		#endregion taglib stuff
	}
}

