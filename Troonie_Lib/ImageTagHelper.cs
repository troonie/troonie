using System;
using System.Collections.Generic;
using System.Globalization;

namespace Troonie_Lib
{
    public class ImageTagHelper
	{
		private static ImageTagHelper instance;
		public static ImageTagHelper I
		{
			get
			{
				if (instance == null) {
					instance = new ImageTagHelper ();
				}
				return instance;
			}
		}
			
		public static TagsData GetTags(string fileName) 
		{
            TagsData td = new TagsData();
            List<string> lines = new List<string>();
            //bool success = true;
            string tArgs = " -overwrite_original -S -n " +
                            "-GPSAltitude " +
                            "-Creator " +
                            "-CreateDate " +
                            "-OffsetTime " +
                            "-ExposureTime " + 
                            "-Flash " +
                            "-MeteringMode " +
                            "-FNumber " +
                            "-FocalLength " +
                            "-FocalLengthIn35mmFormat " +
                            "-ISO " +
                            "-Keywords " +
                            "-Subject " +
                            "-Microsoft:Category " + 
                            "-GPSLatitude " +
                            "-GPSLongitude " +
                            "-Make " +
                            "-Model " +
                            "-Orientation " +
                            "-Rating " +
                            "-Software " +
                            // other tags
                            "-Comment " +
                            "-Copyright " +
                            "-Title ";                            						

            tArgs += " \"" + fileName + "\" ";

            Constants.I.ET.Process(tArgs, lines);

			//* Read the output
			// string output = proc.StandardOutput.ReadToEnd();
			foreach (string item in lines)
			{
                string[] result = item.Split(new string[] { "\r\n", ": ", ", " }, StringSplitOptions.RemoveEmptyEntries);
				if (result.Length < 2)
					continue;

				string key = result[0];
				string allValues = item.Substring((key + ": ").Length);

                //for (int i = 1; i < result.Length; i++)
                //{
                double d;
                ushort us;
				uint ui;
                switch (key)
                {                        
                    case "GPSAltitude": if (double.TryParse(result[1], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out d)) td.Altitude = d; break;
                    case "Creator":  td.Creator = allValues; break;
                    case "CreateDate":
                        DateTimeOffset t;
                        DateTimeOffset? tt = null;
                        bool b = DateTimeOffset.TryParseExact(result[1], "yyyy:MM:dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out t);
                        if (b)
                        {
                            tt = t; 
                            td.DaylightSavingTime = t.DateTime.IsDaylightSavingTime() ? Language.I.L[261] : Language.I.L[201];
                        }                            
						td.CreateDate = tt; 
						break;
                    case "OffsetTime": td.OffsetTime = new OffsetTime(result[1]); break;
                    case "ExposureTime": if(double.TryParse(result[1], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out d)) td.ExposureTime = d; break;
                    case "Flash": if (ushort.TryParse(result[1], out us)) td.Flash = us; break;
					case "MeteringMode": if (ushort.TryParse(result[1], out us)) td.MeteringMode = us; break;
					case "FNumber": if (double.TryParse(result[1], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out d)) td.FNumber = d; break;
					case "FocalLength": if (double.TryParse(result[1], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out d)) td.FocalLength = d; break;
					case "FocalLengthIn35mmFormat": if (uint.TryParse(result[1], out ui)) td.FocalLengthIn35mmFilm = ui; break;
					case "ISO": if (uint.TryParse(result[1], out ui)) td.ISOSpeedRatings = ui; break;
                    case "Keywords":
                    case "Subject":
                    case "Category":
						if (td.Keywords == null)
							td.Keywords = new List<string>();

                        for (int i = 1; i < result.Length; i++)
                            td.Keywords.Add(result[i]);
                        
                        // remove duplicates
                        HashSet<string> hs = new HashSet<string>();
                        td.Keywords.RemoveAll(x => !hs.Add(x));
                        hs.Clear(); hs = null;
                        break;
					case "GPSLatitude": if (double.TryParse(result[1], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out d)) td.Latitude = d; break;
                    case "GPSLongitude": if (double.TryParse(result[1], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out d)) td.Longitude = d; break;
                    case "Make": td.Make= allValues; break;
                    case "Model": td.Model = allValues; break;
                    case "Orientation":
						if (uint.TryParse(result[1], out ui))
						{
							td.Orientation = ui;
							td.CalcOrientationDegree();
						}
						break;
					case "Rating": if (uint.TryParse(result[1], out ui)) td.Rating = ui; break;
                    case "Software": td.Software = allValues; break;
                    // other tags
                    case "Comment": td.Comment = allValues; break;
                    case "Copyright": td.Copyright = allValues; break;
                    case "Title": td.Title = allValues; break;
                }
            }

			return td;
        }

        public static DateTimeOffset? GetCreateDate(string fileName)
        {
            DateTimeOffset? tt = null;
            string tArgs = " -S -CreateDate " + " \"" + fileName + "\" ";
            List<string> lines = new List<string>();
            Constants.I.ET.Process(tArgs, lines);

            if (lines.Count != 0)
            {
                string[] result = lines[0].Split(new string[] { "\r\n", ": ", ", " }, StringSplitOptions.RemoveEmptyEntries);
                if (result.Length == 2)
                {
                    DateTimeOffset t;

                    bool b = DateTimeOffset.TryParseExact(result[1], "yyyy:MM:dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out t);
                    if (b)
                    {
                        tt = t;
                    }
                }
            }

            return tt;
        }

        public static uint GetRating(string fileName)
		{
            string tArgs = " -S -Rating# " + " \"" + fileName + "\" ";
			uint rating = 0;
            List<string> lines = new List<string>();
            Constants.I.ET.Process(tArgs, lines);

			//* Read the output
			// string output = proc.StandardOutput.ReadToEnd();
			if (lines.Count != 0)
			{
                string[] result = lines[0].Split(new string[] { "\r\n", ": ", ", " }, StringSplitOptions.RemoveEmptyEntries);
				if (result.Length == 2)
				{
					//string key = result[0];
					//rating = result[1];
					uint.TryParse(result[1], out rating);
                }                
            }

			return rating;
        }


        public static bool SetRating(string fileName, uint rating)
		{
            string MS_Rating = " -Microsoft:SharedUserRating#=";
            switch (rating)
            {
                case 0: break;
                case 1: MS_Rating += 1; break;
                case 2: MS_Rating += 25; break;
                case 3: MS_Rating += 50; break;
                case 4: MS_Rating += 75; break;
                case 5: MS_Rating += 99; break;
            }

            string tArgs = " -overwrite_original -S -Rating#=" + rating + MS_Rating + " " + " \"" + fileName + "\" ";
            Constants.I.ET.Process(tArgs);
			return Constants.I.ET.Success;
        }

        public static bool SetTags(string fileName, TagsFlag flag, TagsData newData /*, bool append = false*/)
        {
			string tArgs = " -overwrite_original -S ";
            uint flagValue = int.MaxValue;
            flagValue += 1;

            while (flagValue != 0)
            {
                switch (flag & (TagsFlag)flagValue)
                {
                    // image tags
                    case TagsFlag.Altitude: tArgs += "-GPSAltitude#=" + newData.Altitude + " "; break;
                    case TagsFlag.Creator:
						//string tCreator = Enum.GetName(typeof(TagsFlag), flag);
						//tCreator = append ? tCreator + "<${" + tCreator + "}" : tCreator + "="; 
						tArgs += "-Creator=\"" + newData.Creator + "\" ";							
						break;
					case TagsFlag.Keywords:
                        tArgs += "-Keywords=\"" + newData.KeywordsString + "\" -sep " + "\", \" ";
                        tArgs += "-Subject=\"" + newData.KeywordsString + "\" -sep " + "\", \" ";
                        tArgs += "-Microsoft:Category=\"" + newData.KeywordsString + "\" -sep " + "\", \" ";
                        break;
                    case TagsFlag.CreateDate:
						tArgs += "-CreateDate=" + ExifTool.DateTimeToString(newData.CreateDate);
						break;
                    case TagsFlag.OffsetTime: 
                        tArgs += "-OffsetTime=\"" + newData.OffsetTime.Value + "\" ";
                        break;
                    case TagsFlag.ExposureTime: tArgs += "-ExposureTime#=" + newData.ExposureTime + " "; break;
                    case TagsFlag.Flash: tArgs += "-Flash#=" + newData.Flash + " "; break;
                    case TagsFlag.FNumber: tArgs += "-FNumber#=" + newData.FNumber + " "; break;
                    case TagsFlag.FocalLength: tArgs += "-FocalLength#=" + newData.FocalLength + " "; break;
                    case TagsFlag.FocalLengthIn35mmFilm: tArgs += "-FocalLengthIn35mmFormat#=" + newData.FocalLengthIn35mmFilm + " "; break;
                    case TagsFlag.ISOSpeedRatings: tArgs += "-ISO#=" + newData.ISOSpeedRatings + " "; break;
                    case TagsFlag.Latitude: tArgs += "-GPSLatitude#=" + newData.Latitude + " "; break;
                    case TagsFlag.Longitude: tArgs += "-GPSLongitude#=" + newData.Longitude + " "; break;
                    case TagsFlag.Make: tArgs += "-Make=\"" + newData.Make + "\" "; break;
                    case TagsFlag.MeteringMode: tArgs += "-MeteringMode#=" + newData.MeteringMode + " "; break;
                    case TagsFlag.Model: tArgs += "-Model=\"" + newData.Model + "\" "; break;
                    case TagsFlag.Orientation: tArgs += "-Orientation#=" + newData.Orientation + " "; break;
					case TagsFlag.Rating:
                        string MS_Rating = " -Microsoft:SharedUserRating#=";
                        if (newData.Rating.HasValue) { 
                            switch (newData.Rating)
                            {
                                case 0: break;
                                case 1: MS_Rating += 1; break;
                                case 2: MS_Rating += 25; break;
                                case 3: MS_Rating += 50; break;
                                case 4: MS_Rating += 75; break;
                                case 5: MS_Rating += 99; break;
                            }
                        }
                        tArgs += "-Rating#=" + newData.Rating + MS_Rating + " "; break;
                    case TagsFlag.Software: tArgs += "-Software=\"" + newData.Software + "\" "; break;
                    // other tags
                    case TagsFlag.Comment: tArgs += "-Comment=\"" + newData.Comment + "\" "; break;
                    case TagsFlag.Copyright: tArgs += "-Copyright=\"" + newData.Copyright + "\" "; break;
                    case TagsFlag.Title: 
                        tArgs += "-Title=\"" + newData.Title + "\" ";
                        // tArgs += "-Quicktime:Title=\"" + newData.Title + "\" ";
                        break;                    
                    // also setting hidden tags
                    case TagsFlag.MediaCreateDate:
                        tArgs += "-MediaCreateDate=" + ExifTool.DateTimeToString(newData.CreateDate); // videos
                        break;
                    case TagsFlag.TrackCreateDate:
                        tArgs += "-TrackCreateDate=" + ExifTool.DateTimeToString(newData.CreateDate); // videos
                        break;
                    case TagsFlag.ModifyDate:
                        tArgs += "-ModifyDate=" + ExifTool.DateTimeToString(newData.CreateDate); // images+videos
                        break;
                    case TagsFlag.TrackModifyDate:
                        tArgs += "-TrackModifyDate=" + ExifTool.DateTimeToString(newData.CreateDate); // videos
                        break;
                    case TagsFlag.MediaModifyDate:
                        tArgs += "-MediaModifyDate=" + ExifTool.DateTimeToString(newData.CreateDate);  // videos
                        break;
                    case TagsFlag.DateTimeOriginal:
                        tArgs += "-OriginalCreateDateTime=" + ExifTool.DateTimeToString(newData.CreateDate); // videos
                        tArgs += "-DateTimeOriginal=" + ExifTool.DateTimeToString(newData.CreateDate); // images+videos

                        if (flag.HasFlag(TagsFlag.OffsetTime) /* | flag.HasFlag(TagsFlag.CreateDate) */ )
                            tArgs += "-OffsetTimeOriginal=\"" + newData.OffsetTime.Value + "\" ";
                        break;

                        //			default:
                        //				throw new NotImplementedException ();
                }

                flagValue >>= 1;
            }

			tArgs += " \"" + fileName + "\" ";
			Constants.I.ET.Process(tArgs);

            return Constants.I.ET.Success;
        }			       
    }
}

