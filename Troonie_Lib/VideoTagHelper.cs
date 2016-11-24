using System;
using TagLib;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Collections.Generic;

namespace Troonie_Lib
{
	public class VideoTagHelper
	{
		private static VideoTagHelper instance;
		public static VideoTagHelper I
		{
			get
			{
				if (instance == null) {
					instance = new VideoTagHelper ();
				}
				return instance;
			}
		}			
			
		public static void SetDateAndRatingInVideoTag(string fileName, uint rating)
		{
			TagsFlag flag = TagsFlag.Track;
			uint dateAsUint;
			string dateAsString;
			GetDateFromFilenameAsUint (fileName, out dateAsUint, out dateAsString);
			TagsData td = new TagsData { Track = rating };

			if (dateAsUint != 0) {
				flag = TagsFlag.Track | TagsFlag.Year | TagsFlag.Composers;
				td.Year = dateAsUint;
				td.Composers = new List<string>{ "Creation date (Troonie): " + dateAsString, "Rating (Troonie): " + rating };
			} 

			SetTag(fileName, flag, td);
		}

		public static TagsData GetTagsData(string fileName)
		{
			TagsData td = new TagsData ();
			Tag cit = GetTag (fileName);
			if (cit != null) {
				td.Comment = cit.Comment;
				td.Composers = new List<string>(cit.Composers);
				td.Conductor = cit.Conductor;
				td.Copyright = cit.Copyright;
				td.Title = cit.Title;
				td.Track = cit.Track;
				td.TrackCount = cit.TrackCount;
				td.Year = cit.Year;
			}

			return td;
		}

		public static Tag GetTag(string fileName)
		{
			TagLib.File tagFile = LoadTagFile(fileName);			

			if (tagFile == null)
				return null;
			
			Tag tag = tagFile.Tag;
			tagFile.Dispose ();
			return tag;
		}

		public static void ChangeValueOfTag(Tag tag, TagsFlag flag, TagsData newData)
		{
			uint flagValue = int.MaxValue;
			flagValue += 1;

			while(flagValue != 0)
			{
				switch (flag & (TagsFlag)flagValue) {
				case TagsFlag.Comment:		tag.Comment = newData.Comment;				break;
				case TagsFlag.Composers:	tag.Composers= newData.Composers.ToArray();	break;
				case TagsFlag.Conductor:	tag.Conductor = newData.Conductor;			break;
				case TagsFlag.Copyright:	tag.Copyright = newData.Copyright;			break;
				case TagsFlag.Title:		tag.Title = newData.Title;					break;
				case TagsFlag.Track:		tag.Track = newData.Track;					break;
				case TagsFlag.TrackCount:	tag.TrackCount = newData.TrackCount;		break;
				case TagsFlag.Year:			tag.Year = newData.Year;					break;
				}

				flagValue >>= 1;
			}
		}

		public static bool SetTag(string fileName, TagsFlag flag, TagsData newData)
		{
			bool success = true;
			TagLib.File tagFile = LoadTagFile (fileName);
			Tag tag = tagFile.Tag;

			try{
				ChangeValueOfTag (tag, flag, newData);
				tagFile.Save();
				tagFile.Dispose ();
			}
			catch (Exception /* UnsupportedFormatException */ ) {
				success = false;
			}

			return success;
		}

		private static TagLib.File LoadTagFile(string fileName)
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

			return tagFile;
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

