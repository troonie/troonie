using System;
using TagLib;
using System.Text.RegularExpressions;
using System.Globalization;

namespace Troonie_Lib
{
	public class VideoTag
	{
		public VideoTag ()
		{
		}			

		public static int GetVideoRating(string fileName)
		{
			Tag tag = ExtractGeneralTag (fileName);
			if (tag == null || tag.Track == 0) {
				return -1;
			} else {
				return (int)tag.Track;
			}
		}
			
		public static void SetDateAndRatingInVideoTag(string fileName, uint rating)
		{
			TagLib.File tagFile;
			try{
				tagFile = TagLib.File.Create(fileName);
				if (tagFile == null){
					return;
				}
			}
			catch (Exception ex /* UnsupportedFormatException */) {
				return;
			}
				
			//			tagFile.Tag.Comment = "Rating 1";
			tagFile.Tag.Track = rating;
			uint dateAsUint;
			string dateAsString;
			GetDateFromFilenameAsUint (fileName, out dateAsUint, out dateAsString);
			if (dateAsUint != 0) {
				tagFile.Tag.Year = dateAsUint;
//				tagFile.Tag.Conductor = "Conductor: " + date.ToString ();
//				tagFile.Tag.Copyright = "Copyright: " + date.ToString ();
				tagFile.Tag.Composers = new[]{"Creation date (Troonie): " + dateAsString, "Rating (Troonie): " + rating };
			}

			try{
				tagFile.Save();
			}
			catch (Exception /* UnsupportedFormatException */) {
				// do nothing
				//				Console.WriteLine(e2.Message);
			}

			tagFile.Dispose ();
			return;
		}

		private static Tag ExtractGeneralTag(string fileName)
		{
			TagLib.File tagFile;
			try{
				tagFile = TagLib.File.Create(fileName);
				if (tagFile == null){
					return null;
				}
			}
			catch (Exception /* UnsupportedFormatException */) {
				return null;
			}				

			Tag tag = tagFile.Tag;
			tagFile.Dispose ();
			return tag;
		}


		private static void GetDateFromFilenameAsUint(string filename, out uint dateAsUint, out string date)
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
	}
}

