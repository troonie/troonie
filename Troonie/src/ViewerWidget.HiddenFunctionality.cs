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
			try {	
				int nr = 0;

				List<ViewerImagePanel>pressedInVIPs = GetPressedInVIPs();

				foreach (ViewerImagePanel pib in pressedInVIPs) {
					tmp = pib.RelativeImageName;
					string creatorText = "";

					// check whether file is image or video
					FileInfo info = new FileInfo (pib.OriginalImageFullName);
					string ext = info.Extension.ToLower ();
					long origLength = info.Length; 
					uint rating = 0;
					bool isVideo = false;
					//				DateTime? dt = null;
					TagsData td;
					string f = pib.RelativeImageName, fullf = pib.OriginalImageFullName;

					if (ext.Length != 0 && Constants.Extensions.Any (x => x.Value.Item1 == ext || x.Value.Item2 == ext)) {						
						td = ImageTagHelper.GetTagsData (pib.OriginalImageFullName);
						rating = td.Rating == null ? 0 : td.Rating.Value;

					} else {
//						td = VideoTagHelper.GetTagsData(pib.OriginalImageFullName);
//						rating = td.TrackCount;
						isVideo = true;

//						ConvertWidget.InsertIdentifierAtBegin(ref f, ref fullf, "V-", td.Title);
//						SetTextAndFulltextAndRedrawVip(pib, f, fullf);
					}

					long limitInBytes = Math.Max (rating * 1050000, 350000);
					int biggestLength;

					switch (rating) 
					{
					case 0:			
						break;
					case 1:
						AppendIdentifier (ref f, ref fullf, "_+");
						SetTextAndFulltextAndRedrawVip(pib, f, fullf);
						break;
					case 2: 
						AppendIdentifier (ref f, ref fullf, "_++");
						SetTextAndFulltextAndRedrawVip(pib, f, fullf);
						break;
					case 3: 
						AppendIdentifier (ref f, ref fullf, "_+++");
						SetTextAndFulltextAndRedrawVip(pib, f, fullf);
						break;
					case 4: 
						AppendIdentifier (ref f, ref fullf, "_++++");
						SetTextAndFulltextAndRedrawVip(pib, f, fullf);
						break;
					case 5: 
						AppendIdentifier (ref f, ref fullf, "_+++++");
						SetTextAndFulltextAndRedrawVip(pib, f, fullf);
						limitInBytes = long.MaxValue; // avoid any jpg compression
						break;
					}

					if (!isVideo && (Constants.Extensions[TroonieImageFormat.JPEG24].Item1 == ext || 
						Constants.Extensions[TroonieImageFormat.JPEG24].Item2 == ext)) {
						byte jpqQuality = 95;
						biggestLength = 1800 + 1200 * (int)rating;
						if (origLength > limitInBytes &&
							TroonieBitmap.GetBiggestLength (pib.OriginalImageFullName) > biggestLength) 
						{
							ReduceImageSize (pib.RelativeImageName, pib.OriginalImageFullName, ref creatorText, biggestLength, jpqQuality);
						}

						ConvertByRating (pib.RelativeImageName, pib.OriginalImageFullName, ref creatorText, limitInBytes, jpqQuality);
					}

					nr++;
				}
			}
			catch (Exception e)
			{
				OkCancelDialog pseudo = new OkCancelDialog (true);
				pseudo.Title = Language.I.L [153];
				pseudo.Label1 = "Something went wrong by 'AppendIdAndCompressionByRating'.";
				pseudo.Label2 = "Stoppage at image '" + tmp + "'. Exception message: " + Constants.N + e.Message;
				pseudo.OkButtontext = Language.I.L [16];
				pseudo.Show ();
			}
		}

		private static void SetTextAndFulltextAndRedrawVip(ViewerImagePanel vip, string filename, string fullfilename)
		{
			vip.RelativeImageName = filename; 
			vip.OriginalImageFullName = fullfilename;
			vip.QueueDraw ();
		}

		private void RenameByCreationDate()
		{
			string tmp = "";
			try{

				List<ViewerImagePanel>pressedInVIPs = GetPressedInVIPs();

				foreach (ViewerImagePanel vip in pressedInVIPs) {
					tmp = vip.RelativeImageName;
					string f = vip.RelativeImageName, fullf = vip.OriginalImageFullName;
					DateTime? dt = null;
					ImageTagHelper.GetDateTime (fullf, out dt);
					if (dt.HasValue) {
						RenameFileByDate (ref f, ref fullf, dt.Value);
						SetTextAndFulltextAndRedrawVip(vip, f, fullf);
					}
					else {
						dt = GetDatetimeFromFilename(f);
						if (dt.HasValue) {
							vip.TagsData.DateTime = dt;
							vip.IsPressedIn = vip.IsPressedIn;
//							SetTextAndFulltextAndRedrawVip(vip, f, fullf);
						}
					}
				}					
			}
			catch (Exception e)
			{
				OkCancelDialog pseudo = new OkCancelDialog (true);
				pseudo.Title = Language.I.L [153];
				pseudo.Label1 = "Something went wrong by 'RenameByCreationDate'.";
				pseudo.Label2 = "Stoppage at image '" + tmp + "'. Exception message: " + Constants.N + e.Message;
				pseudo.OkButtontext = Language.I.L [16];
				pseudo.Show ();
			}
		}

		private void RenameVideoByTitleAndInsertIdentifier()
		{
			string tmp = "";
			try{
				List<ViewerImagePanel>pressedInVIPs = GetPressedInVIPs();

				foreach (ViewerImagePanel vip in pressedInVIPs) {
					if (!vip.IsVideo) {
						continue;
					}						

					TagsData td = VideoTagHelper.GetTagsData(vip.OriginalImageFullName);
					string f = vip.RelativeImageName, fullf = vip.OriginalImageFullName;												
//					InsertIdentifierAtBegin(ref f, ref fullf, "V-", td.Title);

					string s = f;
					string title = td.Title;
					string identifier = "V-";

					if (title.Length != 0) {
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
					//END InsertIdentifierAtBegin(ref f, ref fullf, "V-", td.Title);

					string oldFullPicName = vip.OriginalImageFullName + ".png";
					string newFullPicName = fullf + ".png";
					string newRelativePicName = f + ".png";
					SetTextAndFulltextAndRedrawVip(vip, f, fullf);

					// check, if picture video exists nad rename it
					if (File.Exists (oldFullPicName)) {
						foreach (ViewerImagePanel picVip in tableViewer.Children) {
							if (picVip.OriginalImageFullName != oldFullPicName) {
								continue;
							}
							else {
								TroonieBitmap.CreateTextBitmap (newFullPicName, f);
								File.Delete(oldFullPicName);
								ImageTagHelper.SetTag(newFullPicName, (TagsFlag)0xFFFFFF, vip.TagsData);
								picVip.TagsData = vip.TagsData;
								SetTextAndFulltextAndRedrawVip(picVip, newRelativePicName, newFullPicName);
								// dirty workaround to refresh thumbnail image
								picVip.IsDoubleClicked = false; 

								break;
							}
						}													
					}


				}					
			}
			catch (Exception e)
			{
				OkCancelDialog pseudo = new OkCancelDialog (true);
				pseudo.Title = Language.I.L [153];
				pseudo.Label1 = "Something went wrong by 'RenameVideoByTitleAndInsertIdentifier'.";
				pseudo.Label2 = "Stoppage at video '" + tmp + "'. Exception message: " + Constants.N + e.Message;
				pseudo.OkButtontext = Language.I.L [16];
				pseudo.Show ();
			}
		}


		#region Image changing and adapting

		public static void ReduceImageSize(string filename, string fullfilename, ref string creatorText, int biggestLength, byte jpgQuality)
		{
			BitmapWithTag bt_final = new BitmapWithTag (fullfilename, true);
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
				creatorText += "BLength=" + biggestLength + separator;
			}
		}

		public static bool ConvertByRating(string filename, string fullfilename, ref string creatorText, long limitInBytes, byte jpgQuality)
		{
			bool success = true;
			FileInfo info = new FileInfo (fullfilename);
			long l = info.Length;
			jpgQuality++;

			string relativeImageName = filename.Substring(0, filename.LastIndexOf('.')) + "_tmp.jpg";


			while (l > limitInBytes && jpgQuality > 5)
			{
				jpgQuality--;

				BitmapWithTag bt = new BitmapWithTag (fullfilename, true);
				Config c = new Config();				
				c.UseOriginalPath = false;
				c.Path = Constants.I.EXEPATH;
				c.HighQuality = true;
				c.ResizeVersion = ResizeVersion.No;
				c.JpgQuality = jpgQuality;
				c.FileOverwriting = false;
				bool success_inner = bt.Save (c, relativeImageName, true);
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
				success = ImageTagHelper.SetTag(fullfilename, TagsFlag.Creator, new TagsData { Creator = creatorText });
			} else {
				BitmapWithTag bt_final = new BitmapWithTag (fullfilename, true);
				Config c_final = new Config ();				
				c_final.UseOriginalPath = true;
				c_final.HighQuality = true;
				c_final.ResizeVersion = ResizeVersion.No;
				c_final.JpgQuality = jpgQuality;
				c_final.FileOverwriting = true;
				creatorText += "Jpg-Q=" + jpgQuality.ToString() + separator;
				//				success = bt_final.ChangeValueOfTag (Tags.Creator, creatorText);
				success = bt_final.ChangeValueOfTag (TagsFlag.Creator, new TagsData { Creator = creatorText });
				if (success) {
					success = bt_final.Save (c_final, filename, true);
				}
				bt_final.Dispose ();
			}
			return success;
		}

		public static void RenameFileByDate(ref string filename, ref string fullfilename, DateTime dt)
		{			
			string format = "yyyyMMdd-HHmmss";
			string s = dt.ToString (format) + filename.Substring(filename.LastIndexOf(".")).ToLower();
			string newFullText = fullfilename.Replace (filename, s);
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

			FileHelper.I.Rename (fullfilename, newFullText);
			fullfilename = newFullText;
			filename = newFullText.Substring (newFullText.LastIndexOf (IOPath.DirectorySeparatorChar) + 1);

			//			pib.FullText = newFullText;
			//			pib.Text = newFullText.Substring (newFullText.LastIndexOf (IOPath.DirectorySeparatorChar) + 1);
			//			pib.Redraw ();
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
					return dt;
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
					return dt;
				}
			}

			// TODO: GO ON
			// third pattern check
			pattern = @"(\d+)[-_.\/](\d+)";
			formats = new string[] {"yyyyMMdd_hhmmss", "yyyyMMdd-hhmmss"};
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

