using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using IOPath = System.IO.Path;
using NetColor = System.Drawing.Color;
using NetIOFile = System.IO.File;
//using C = Troonie_Lib.Constants;

namespace Troonie_Lib
{
    public class BitmapWithTag : IDisposable
	{
		//private CombinedImageTag imageTag;
		private int _errorCode;
		private string _errorText;

		public readonly ExifTool ET = Constants.I.ET;

        public TroonieImageFormat OrigFormat { get; private set; }
		public Bitmap Bitmap { get; private set; }
		public string FileName { get; private set; }

		// TODO [17-04-2020]: Unnecessary? 
		public int ErrorCode { get => _errorCode; private set => _errorCode = value; }
		public string ErrorText { get => _errorText; private set => _errorText = value; }

		public BitmapWithTag (string filename)
		{
			FileName = filename;

			if (filename != null) {
				Bitmap = TroonieBitmap.FromFile (FileName, out _errorCode, out _errorText);
				OrigFormat = ImageFormatConverter.I.ConvertToPIF (Bitmap.RawFormat, Bitmap.PixelFormat);
				//imageTag = ImageTagHelper.GetTag (filename);
			} else {
				Bitmap = new Bitmap (180, 180, PixelFormat.Format32bppArgb);
				OrigFormat = TroonieImageFormat.PNG32AlphaAsValue;
				//imageTag = new CombinedImageTag (TagTypes.Png);
			}
		}


		public void ChangeBitmapButNotTags (Bitmap newBitmap)
		{
			Bitmap = newBitmap;
		}

		public void Dispose ()
		{
			if (Bitmap != null) {
				Bitmap.Dispose ();
				Bitmap = null;
			}

   //         if (imageTag != null)
			//{
   //             imageTag = null;
			//	// [14.07.2023]
   //             //	DONT: imageTag.Clear(); --> causes NotImplementedException in Clear() of classes
			//	//	IFDTag and XmpTag, no troonie specific code in TagLibSharp anymore.
   //         }

            MemoryReducer.ReduceMemoryUsage (true);
		}

		public bool Save (Config config, string relativeFileName, bool saveTag)
		{
			bool success = true;
            //bool saveTagsByET = false;
			string origExifTagsFile = Constants.I.TEMPPATH + "ET_" + relativeFileName.Trim(Path.DirectorySeparatorChar);

            if (saveTag && Constants.I.EXIFTOOL)
			{
				Bitmap bb = new Bitmap(10, 10);
				bb.Save(origExifTagsFile, ImageFormat.Jpeg);
                string tArg = " -overwrite_original -S -TagsFromFile " + FileName + " \"-all:all>all:all\" " + origExifTagsFile;
                ET.Process(tArg);
				//saveTagsByET = ET.Success;
            }

			try {
				Bitmap dest;
				int w = Bitmap.Width;
				int h = Bitmap.Height;

				if (config.UseOriginalPath) {
					config.Path = System.IO.Directory.GetParent (FileName).FullName;
				}

				#region FileOverwriting
				string tmpNewFileName;
				if (config.Path [config.Path.Length - 1] == IOPath.DirectorySeparatorChar)
					tmpNewFileName = config.Path + relativeFileName;
				else
					tmpNewFileName = config.Path + IOPath.DirectorySeparatorChar + relativeFileName;

				if (config.StretchImage != ConvertMode.Editor) {
					if (config.FileOverwriting) {
						if (FileName != tmpNewFileName) {
							NetIOFile.Delete (FileName);
						}
					} else {
						int countImage = 0;
						while (FileHelper.I.Exists (tmpNewFileName)) {
							countImage++;
							string identifier = "__n";
							int lastindexofIdentifier = tmpNewFileName.LastIndexOf (identifier);
							if (lastindexofIdentifier != -1) {
								string part1 = tmpNewFileName.Remove (lastindexofIdentifier);
								string part2 = tmpNewFileName.Substring (tmpNewFileName.LastIndexOf ('.'));
								tmpNewFileName = part1 + part2;
							}

							tmpNewFileName = tmpNewFileName.Insert (tmpNewFileName.LastIndexOf ('.'), identifier + countImage);
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

				#region ReplacingTransparencyWithColor
				if (config.ReplacingTransparencyWithColor && config.Format != (
						TroonieImageFormat.JPEGLOSSLESS |
						TroonieImageFormat.PNG32AlphaAsValue |
						TroonieImageFormat.PNG32Transparency)) {
					dest = ImageConverter.ARGBToRGB (
						dest,
						config.ReplacingTransparencyWithColor,
						config.ReplaceTransparencyColorRed,
						config.ReplaceTransparencyColorGreen,
						config.ReplaceTransparencyColorBlue);
				}

				#endregion ReplacingTransparencyWithColor

				#region Saving by using TroonieImageFormat
				switch (config.Format) {
				case TroonieImageFormat.JPEG8:
				case TroonieImageFormat.JPEG24:
				case TroonieImageFormat.JPEGLOSSLESS:
					success = JpegEncoder.SaveWithCjpeg (FileName, dest, config.JpgQuality, config.Format);
					if (saveTag) 
					{
						string tArg = " -overwrite_original -S -TagsFromFile " + origExifTagsFile + " \"-all:all>all:all\" " + FileName; // + " -execute 'TODO further commands'";
						ET.Process(tArg);
						//saveTagsByET = ET.Success;                      
					}
					Bitmap = dest;
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
				dest.Save (FileName, ImageFormatConverter.I.ConvertFromPIF (config.Format));
				#endregion Saving by using TroonieImageFormat

				Bitmap = dest;

				if (saveTag) 
				{
                    //ImageTagHelper.CopyTagToFile (FileName, imageTag);
                    string tArg = " -overwrite_original -S -TagsFromFile " + origExifTagsFile + " \"-all:all>all:all\" " + FileName; // + " -execute 'TODO further commands'";
                    ET.Process(tArg);
                    //saveTagsByET = ET.Success;
                }

			} catch (Exception ex) {
				success = false;
				Console.WriteLine (ex.Message);
			}

			return success;
		}


		#region taglib stuff

		//		public bool ChangeValueOfTag(Tags tag, string newValue)
		//		{
		//			bool success = true;
		//
		//			try{
		//				ImageTagHelper.ChangeValueOfTag (imageTag, tag, newValue);
		//			}
		//			catch (Exception /* UnsupportedFormatException */) {
		//				success = false;
		//			}
		//
		//			return success;
		//		}

		//public bool ChangeValueOfTag (TagsFlag flag, TagsData newData)
		//{
		//	bool success = true;

		//	try {
		//		ImageTagHelper.ChangeValueOfTag (imageTag, flag, newData);
		//	} catch (Exception /* UnsupportedFormatException */) {
		//		success = false;
		//	}

		//	return success;
		//}

		//		public bool ChangeValueOfTag(Tags tag, int newValue)
		//		{
		//			bool success = true;
		//
		//			try{
		//				ImageTagHelper.ChangeValueOfTag (imageTag, tag, newValue);
		//			}
		//			catch (Exception /* UnsupportedFormatException */) {
		//				success = false;
		//			}
		//
		//			return success;
		//		}

		#endregion taglib stuff
	}
}

