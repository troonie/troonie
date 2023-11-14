using System;
using System.IO;
using Troonie_Lib;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using IOPath = System.IO.Path;

namespace Troonie
{	
	public partial class ViewerWidget
	{
		private const string separator = "; ";

		private void AppendIdAndCompressionByRating()
		{		
			string tmp = "";
			string errorImages = string.Empty;
			try {	
				List<ViewerImagePanel>pressedInVIPs = GetPressedInVIPs();

				foreach (ViewerImagePanel pib in pressedInVIPs) {
					tmp = pib.RelativeImageName;
					string creatorText = "";

					// check whether file is image or video
					FileInfo info = new FileInfo (pib.OriginalImageFullName);
					string ext = info.Extension.ToLower ();
					long fileSize = info.Length; 
					uint rating = 0;
					bool isVideo = false;
					//				DateTime? dt = null;
					TagsData td;
					string f = pib.RelativeImageName, fullf = pib.OriginalImageFullName;

					if (ext.Length != 0 && Constants.Extensions.Any (x => x.Value.Item1 == ext || x.Value.Item2 == ext)) {						
						td = ImageTagHelper.GetTagsDataET (pib.OriginalImageFullName);
						rating = td.Rating == null ? 0 : td.Rating.Value;						
						if (td.Creator != null && td.Creator.Length != 0)
                            creatorText = td.Creator + separator;  

                    } else {
//						td = VideoTagHelper.GetTagsData(pib.OriginalImageFullName);
//						rating = td.TrackCount;
						isVideo = true;

						string fullPicName = info.FullName + ".png";
						if (File.Exists (fullPicName)) {
                            rating = ImageTagHelper.GetRating (fullPicName);
                        }
//						ConvertWidget.InsertIdentifierAtBegin(ref f, ref fullf, "V-", td.Title);
//						SetTextAndFulltextAndRedrawVip(pib, f, fullf);
					}

					long limitInBytes = Math.Max (rating * 1050000, 350000);
					int biggestLength;

//					switch (rating) 
//					{
//					case 0:			
//						break;
//					case 1:
//						AppendIdentifier (ref f, ref fullf, Constants.Stars[rating]);
//						SetTextAndFulltextAndRedrawVip(pib, f, fullf);
//						break;
//					case 2: 
//						AppendIdentifier (ref f, ref fullf, "_++");
//						SetTextAndFulltextAndRedrawVip(pib, f, fullf);
//						break;
//					case 3: 
//						AppendIdentifier (ref f, ref fullf, "_+++");
//						SetTextAndFulltextAndRedrawVip(pib, f, fullf);
//						break;
//					case 4: 
//						AppendIdentifier (ref f, ref fullf, "_++++");
//						SetTextAndFulltextAndRedrawVip(pib, f, fullf);
//						break;
//					case 5: 
//						AppendIdentifier (ref f, ref fullf, "_+++++");
//						SetTextAndFulltextAndRedrawVip(pib, f, fullf);
//						limitInBytes = long.MaxValue; // avoid any jpg compression
//						break;
//					}

					AppendIdentifier (ref f, ref fullf, Constants.Stars[rating]);
					SetTextAndFulltextAndRedrawVip(pib, f, fullf);
					if (rating == 5) {
						limitInBytes = long.MaxValue; // avoid any jpg compression
					}

					if (!isVideo && (Constants.Extensions[TroonieImageFormat.JPEG24].Item1 == ext || 
						Constants.Extensions[TroonieImageFormat.JPEG24].Item2 == ext)) {
						byte jpqQuality = 95;
						biggestLength = 1800 + 1200 * (int)rating;
						if (fileSize > limitInBytes &&
							TroonieBitmap.GetBiggestLength (pib.OriginalImageFullName) > biggestLength) 
						{
							ReduceImageSize (pib.RelativeImageName, pib.OriginalImageFullName, ref creatorText, biggestLength, jpqQuality);
                            ImageConverter.CalcBiggerSideLength(biggestLength, ref pib.TagsData.Width, ref pib.TagsData.Height);
                        }
							
						bool success = ConvertByRating (pib.RelativeImageName, pib.OriginalImageFullName, ref creatorText, limitInBytes, jpqQuality, out fileSize);
						if (success) {
							pib.TagsData.FileSize = fileSize;
							pib.TagsData.Creator = creatorText;
							// dirty workaround to refresh label strings of ViewerWidget.tableTagsViewer
							pib.IsPressedIn = pib.IsPressedIn;
						}
						else {
							errorImages += tmp + Constants.N;
						}
					}						
				}

				if (errorImages.Length != 0) {
					ShowErrorDialog(Language.I.L[251], errorImages + Constants.N);
				}
			}
			catch (Exception e)
			{
				ShowErrorDialog (Language.I.L[252], 
					Language.I.L[253] + " '" + tmp + "'. Exception message: " + Constants.N + e.Message);
			}
		}

		private static void SetTextAndFulltextAndRedrawVip(ViewerImagePanel vip, string filename, string fullfilename)
		{
			vip.RelativeImageName = filename; 
			vip.OriginalImageFullName = fullfilename;
			vip.QueueDraw ();
		}

		private void SetDatetimeFromFilename()
		{
			string tmp = "";
			string errorImages = string.Empty;
			try{

				List<ViewerImagePanel>pressedInVIPs = GetPressedInVIPs();

				foreach (ViewerImagePanel vip in pressedInVIPs) {
					tmp = vip.RelativeImageName;
					string f = vip.RelativeImageName, fullf = vip.OriginalImageFullName;
					DateTime? dt = GetDatetimeFromFilename(f);
					bool success = false; 
					if (dt.HasValue) {
                        //if (vip.IsVideo)
                        //{
                        //    vip.TagsData.SetAllCreateDates(dt);
                        //    success = VideoTagHelper.SetTag(vip.OriginalImageFullName, TagsFlag.AllCreateDates2, vip.TagsData);
                        //}
                        //else
                        {
                            vip.TagsData.CreateDate = dt;
                            success = ImageTagHelper.SetTagET(vip.OriginalImageFullName, TagsFlag.AllCreateDates2, vip.TagsData);
                        }

                        // dirty workaround to refresh label strings of ViewerWidget.tableTagsViewer
                        vip.IsPressedIn = vip.IsPressedIn;						
					}

					if (!success) {
						errorImages += tmp + Constants.N;
					}
				}

				if (errorImages.Length != 0) {
					ShowErrorDialog(Language.I.L[255], errorImages + Constants.N);
				}
			}
			catch (Exception e)
			{
				ShowErrorDialog (Language.I.L[256], 
					Language.I.L[253] + " '" + tmp + "'. Exception message: " + Constants.N + e.Message);
			}			
		}

		private void RenameFileByDateTime()
		{
			string tmp = "";
			string errorImages = string.Empty;

			try{

				List<ViewerImagePanel>pressedInVIPs = GetPressedInVIPs();

				foreach (ViewerImagePanel vip in pressedInVIPs) {
					tmp = vip.RelativeImageName;
					string f = vip.RelativeImageName, fullf = vip.OriginalImageFullName;
					DateTime? dt = ImageTagHelper.GetDateTime (fullf);
					bool success = false; 

					if (dt.HasValue) {
						string format = "yyyyMMdd-HHmmss";
						string s = dt.Value.ToString (format) + f.Substring(f.LastIndexOf(".")).ToLower();
						string newFullText = fullf.Replace (f, s);
						string tmpNewFullText = newFullText;

						// Check, if already file exists
						int fileCounter = 1;
						while (File.Exists (tmpNewFullText)) {
							fileCounter++;
							tmpNewFullText = newFullText;
							tmpNewFullText = tmpNewFullText.Insert(tmpNewFullText.LastIndexOf("."), "_" + fileCounter);
						}

						if (fileCounter != 1) {
							newFullText = newFullText.Insert(newFullText.LastIndexOf("."), "_" + fileCounter);
						}

						success = FileHelper.I.Rename (fullf, newFullText);
						fullf = newFullText;
						f = newFullText.Substring (newFullText.LastIndexOf (IOPath.DirectorySeparatorChar) + 1);
					}

					if (success) {
						SetTextAndFulltextAndRedrawVip(vip, f, fullf);
					}
					else {						
						errorImages += tmp + Constants.N;
					}
				}

				if (errorImages.Length != 0) {
					ShowErrorDialog(Language.I.L[257], errorImages + Constants.N);
				}
			}
			catch (Exception e)
			{
				ShowErrorDialog (Language.I.L[258], 
					Language.I.L[253] + " '" + tmp + "'. Exception message: " + Constants.N + e.Message);
			}
		}

		private void RenameVideoByTitleAndInsertIdentifier()
		{
			string tmp = "";
			string errorImages = string.Empty;

			try{
				List<ViewerImagePanel>pressedInVIPs = GetPressedInVIPs();

				foreach (ViewerImagePanel vip in pressedInVIPs) {
					if (!vip.IsVideo) {
						continue;
					}						

					bool success = true;
					tmp = vip.RelativeImageName;
					string f = vip.RelativeImageName, fullf = vip.OriginalImageFullName;
					vip.TagsData.CreateDate = GetDatetimeFromFilename(fullf);

//					ImageTagHelper.SetTag (vip.OriginalImageFullName, TagsFlag.DateTime, vip.TagsData);
					// dirty workaround to refresh label strings of ViewerWidget.tableTagsViewer
					vip.IsPressedIn = vip.IsPressedIn;

					string s = f;
					string title = vip.TagsData.Title; // td.Title;
					string identifier = "V-";

					if (title != null && title.Length != 0) {
						title = title.Replace(' ', '-');
						title = System.Text.RegularExpressions.Regex.Replace(title, @"[\\/:*?""<>|]", string.Empty);
						title = title.Replace("[", ""); // will not replaced by 'Regex.Replace(..)'
						title = title.Replace("]", ""); // will not replaced by 'Regex.Replace(..)'
						s = s.Insert(s.LastIndexOf('.'), "_" + title);
					}

					// remove old identifier
					s = s.Replace("-vid", "");

					int firstIdentifier = s.IndexOf (identifier);
					if (firstIdentifier == -1 || firstIdentifier != 0) {
						s = s.Insert (0, identifier);
						FileHelper.I.Rename (fullf, s);

						fullf = fullf.Replace(f, s);
						f = s;
					}

					string oldFullPicName = vip.OriginalImageFullName + ".png";
					string newFullPicName = fullf + ".png";
					string newRelativePicName = f + ".png";
					SetTextAndFulltextAndRedrawVip(vip, f, fullf);

					// check, if picture video exists and rename it
					if (File.Exists (oldFullPicName)) {
						foreach (ViewerImagePanel picVip in tableViewer.Children) {
							if (picVip.OriginalImageFullName != oldFullPicName) {
								continue;
							}
							else {
								TroonieBitmap.CreateTextBitmap (newFullPicName, f);
								File.Delete(oldFullPicName);
								success = ImageTagHelper.SetTagET(newFullPicName, (TagsFlag)0xFFFFFF, vip.TagsData);
								picVip.TagsData = vip.TagsData;
								SetTextAndFulltextAndRedrawVip(picVip, newRelativePicName, newFullPicName);
								// dirty workaround to refresh thumbnail image
								picVip.IsDoubleClicked = false; 

								break;
							}
						}													
					}

					if(!success) {						
						errorImages += newRelativePicName + Constants.N;
					}
				}

				if (errorImages.Length != 0) {
					ShowErrorDialog(Language.I.L[261], errorImages + Constants.N);
				}
			}
			catch (Exception e)
			{
				ShowErrorDialog (Language.I.L[259], 
					Language.I.L[260] + " '" + tmp + "'. Exception message: " + Constants.N + e.Message);
			}
		}


		#region Image changing and adapting

		public static void ReduceImageSize(string filename, string fullfilename, ref string creatorText, int biggestLength, byte jpgQuality)
		{
			BitmapWithTag bt_final = new BitmapWithTag (fullfilename);
			string origDims = "OrigWidth=" + bt_final.Bitmap.Width + "px" + separator + "OrigHeight=" + bt_final.Bitmap.Height + "px" + separator;
            Config c_final = new Config ();				
			c_final.UseOriginalPath = true;
			c_final.HighQuality = true;
			c_final.ResizeVersion = ResizeVersion.BiggestLength;
			c_final.BiggestLength = biggestLength;
			c_final.JpgQuality = jpgQuality;
			c_final.FileOverwriting = true;

			bool success = bt_final.Save (c_final, filename, true);
			bt_final.Dispose ();

			if (success) {
				creatorText += origDims + "BLength=" + biggestLength + separator;
			}
		}

		public static bool ConvertByRating(string filename, string fullfilename, ref string creatorText, long limitInBytes, byte jpgQuality, out long l)
		{
			bool success = true;
			FileInfo info = new FileInfo (fullfilename);
			l = info.Length;
			jpgQuality++;

			string relativeImageName = IOPath.DirectorySeparatorChar + filename.Substring(0, filename.LastIndexOf('.')) + "_tmp.jpg";


			while (l > limitInBytes && jpgQuality > 5)
			{
				jpgQuality--;

				BitmapWithTag bt = new BitmapWithTag (fullfilename);
				Config c = new Config();				
				c.UseOriginalPath = true;
//				c.Path = Constants.I.EXEPATH;
				c.HighQuality = true;
				c.ResizeVersion = ResizeVersion.No;
				c.JpgQuality = jpgQuality;
				c.FileOverwriting = false;
				bool success_inner = bt.Save (c, relativeImageName, false /* DO NOT SAVE Exiftags here  true */);
				bt.Dispose();

				if (success_inner) {
					FileInfo info_inner = new FileInfo (c.Path + relativeImageName);
					l = info_inner.Length;
					info_inner.Delete ();
				}
				else{
					l = info.Length;
					break;
				}
			}

			if (l == info.Length) {
				// no jpg compression was done
				creatorText += "Jpg-Q=Original" + separator;
				//				success = ImageTagHelper.SetTag(fullfilename, Tags.Creator | Tags.Copyright | Tags.Title, new TagsData { Creator = creatorText, Copyright = "oioioi", Title = "MyTitle" });
				// success = ImageTagHelper.SetTag(fullfilename, TagsFlag.Creator, new TagsData { Creator = creatorText });
                success = ImageTagHelper.SetTagET(fullfilename, TagsFlag.Creator, new TagsData { Creator = creatorText });
            } else {
				BitmapWithTag bt_final = new BitmapWithTag (fullfilename);
				Config c_final = new Config ();				
				c_final.UseOriginalPath = true;
				c_final.HighQuality = true;
				c_final.ResizeVersion = ResizeVersion.No;
				c_final.JpgQuality = jpgQuality;
				c_final.FileOverwriting = true;
				creatorText += "Jpg-Q=" + jpgQuality.ToString() + separator;
				//				success = bt_final.ChangeValueOfTag (Tags.Creator, creatorText);
				//success = bt_final.ChangeValueOfTag (TagsFlag.Creator, new TagsData { Creator = creatorText });
                success = ImageTagHelper.SetTagET(fullfilename, TagsFlag.Creator, new TagsData { Creator = creatorText });
                if (success) {
					success = bt_final.Save (c_final, filename, true);
				}
				bt_final.Dispose ();
			}
			return success;
		}
			
		public static void AppendIdentifier(ref string filename, ref string fullfilename, string identifier)
		{
			string s = fullfilename;
			// remove old identifier
			s = s.Replace("-big", "");
			s = s.Replace("_big", "");
			s = s.Replace("-raw", "");

			int lastIdentifier = s.LastIndexOf (identifier);
			int lastDot = s.LastIndexOf ('.');
			if (lastIdentifier == -1 || lastIdentifier + identifier.Length != lastDot) {
				s = s.Insert (s.LastIndexOf ('.'), identifier);
				FileHelper.I.Rename (fullfilename, s);

				fullfilename = s;
				filename = s.Substring (s.LastIndexOf (IOPath.DirectorySeparatorChar) + 1);	
			}
		}

		public static DateTime? GetDatetimeFromFilename(string filename)
		{
			DateTime dt;
			bool success;

//			// simple check
//			success = DateTime.TryParse(filename, out dt);
//			if (success){
//				return dt;
//			}

			// example IMG_20170118_185925088

			string pattern = @"(\d+)[-_ :.\/](\d+)[-_ :.\/](\d+)[-_ :.\/](\d+)[-_ :.\/](\d+)[-_ :.\/](\d+)";
			string []formats = {
				"yyyy-MM-dd HH:mm:ss", "yyyy/MM/dd HH:mm:ss", "yyyy.MM.dd HH:mm:ss", "yyyy_MM_dd HH:mm:ss",
				"dd-MM-yyyy HH:mm:ss", "dd/MM/yyyy HH:mm:ss", "dd.MM.yyyy HH:mm:ss", "dd_MM_yyyy HH:mm:ss",
				"dd-MM-yy HH:mm:ss",   "yy/MM/dd HH:mm:ss",   "dd.MM.yy HH:mm:ss",   "yy_MM_dd HH:mm:ss",
				 

				"yyyy-MM-dd-HH-mm-ss", "yyyy/MM/dd/HH/mm/ss", "yyyy.MM.dd.HH.mm.ss", "yyyy_MM_dd_HH_mm_ss", 
				"dd-MM-yyyy-HH-mm-ss", "dd/MM/yyyy/HH/mm/ss", "dd.MM.yyyy.HH.mm.ss", "dd_MM_yyyy_HH_mm_ss",
				"yy/MM/dd/HH/mm/ss",   "dd-MM-yy-HH-mm-ss",   "dd_MM.yy.HH.mm.ss",   "yy_MM_dd_HH_mm_ss"  };

			Regex r = new Regex(pattern);
			Match m = r.Match(filename);
			if(m.Success)
			{
				success = DateTime.TryParseExact(m.Value, formats, CultureInfo.InvariantCulture, 
					DateTimeStyles.None, out dt);

				if (success){
					return dt;
				}
			}

			pattern = @"(\d+)[-_ :.\/](\d{6})";
			formats =  new string[] {"yyyyMMdd_HHmmss", "yyyyMMdd-HHmmss"};

			r = new Regex(pattern);
			m = r.Match(filename);
			if(m.Success)
			{
				success = DateTime.TryParseExact(m.Value, formats, CultureInfo.InvariantCulture, 
					DateTimeStyles.None, out dt);
				
				if (success){
					return dt;
				}
			}
				
			pattern = @"(\d+)[-.\/](\d+)[-.\/](\d+)";
			formats = new string[] {"yyyy-MM-dd", "yyyy/MM/dd", "yyyy.MM.dd", "dd-MM-yyyy", 
				"dd/MM/yyyy", "dd.MM.yyyy", "yy/MM/dd", "dd-MM-yy", "dd.MM.yy"};
			r = new Regex(pattern);
			m = r.Match(filename);
			if(m.Success)
			{
				success = DateTime.TryParseExact(m.Value, formats, CultureInfo.InvariantCulture, 
					DateTimeStyles.None, out dt);
				if (success){
					return dt;
				}
			}
				
			pattern =  @"(\d+)";
			formats = new string []{"yyyyMMdd", "ddMMyyyy", "yyMMdd", "ddMMyy"};
			r = new Regex(pattern);
			m = r.Match(filename);
			if(m.Success)
			{
				success = DateTime.TryParseExact(m.Value, formats, CultureInfo.InvariantCulture, 
					DateTimeStyles.None, out dt);
				if (success){
					return dt;
				}
			}

			return null;
		}

		[Obsolete("No useful usage anymore. Try 'GetDatetimeFromFilename(string filename)' instead.")]
		public static void GetDateFromFilenameAsUint(string filename, out uint dateAsUint, out string date)
		{
			dateAsUint = 0;
			date = string.Empty;
			DateTime dt;
			bool success;

			// first pattern check
			string pattern = @"(\d+)";
			string[] formats = {"yyyyMMdd", "ddMMyyyy", "yyMMdd", "ddMMyy"};

			Regex r = new Regex(pattern);
			Match m = r.Match(filename);
			if(m.Success)
			{
				success = DateTime.TryParseExact(m.Value, formats, CultureInfo.InvariantCulture, 
					DateTimeStyles.None, out dt);
				if (success){
					date = dt.ToShortDateString ();
					string s = dt.ToString("yyyyMMdd");  
					dateAsUint = Convert.ToUInt32(s);
					return;
				}
			}

			// second pattern check
			pattern = @"(\d+)[-.\/](\d+)[-.\/](\d+)";
			formats = new string[] {"yyyy-MM-dd", "yyyy/MM/dd", "yyyy.MM.dd", "dd-MM-yyyy", 
				"dd/MM/yyyy", "dd.MM.yyyy", "yy/MM/dd", "dd-MM-yy", "dd.MM.yy"};
			r = new Regex(pattern);
			m = r.Match(filename);
			if(m.Success)
			{
				success = DateTime.TryParseExact(m.Value, formats, CultureInfo.InvariantCulture, 
					DateTimeStyles.None, out dt);
				if (success){
					date = dt.ToShortDateString ();
					string s = dt.ToString("yyyyMMdd");  
					dateAsUint = Convert.ToUInt32(s);
					return;
				}
			}							
		}

		#endregion Image changing and adapting
	}
}

