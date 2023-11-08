using System;
using System.Collections.Generic;
using System.Globalization;

namespace Troonie_Lib
{
    public class ImageTagHelper
	{
//		private const ushort KEY_FLASH = 37385;

//		private static string[] tagNames = 
//		{
//			"Keywords", "Rating", "DateTime", "Orientation", "Software", "Latitude", 
//			"Longitude", "Altitude", "ExposureTime", "FNumber", "ISOSpeedRatings", "FocalLength", 
//			"FocalLengthIn35mmFilm", "Make", "Model", "Creator"
//		}; 

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
			
		public static TagsData GetTagsDataET(string fileName) 
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

            tArgs += " " + fileName;

            Constants.I.ET.Process(tArgs, lines);

			//* Read the output
			// string output = proc.StandardOutput.ReadToEnd();
			foreach (string item in lines)
			{
                string[] result = item.Split(new string[] { "\r\n", ": ", ", " }, StringSplitOptions.RemoveEmptyEntries);
				if (result.Length == 0)
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
                        DateTime t;
                        DateTime? tt = null;
                        bool b = DateTime.TryParseExact(result[1], "yyyy:MM:dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out t);
                        if (b)
                        {
                            tt = t; // t.ToLocalTime();
                        }                            
						td.DateTime = tt; 
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

                        //case ET1_Rating_XMP_xmp:
                        //    uint u;
                        //    if (uint.TryParse(result[i + 1], out u))
                        //        td.Rating = u;
                        //    break;
                        //case ET1_Title_Quicktime:
                        //    td.Title = result[i + 1]; break;
                        //case ET1_Comment_Quicktime:
                        //    td.Comment = result[i + 1]; break;
                        //case ET1_Copyright_Quicktime:
                        //    td.Copyright = result[i + 1]; break;
                        //case ET1_Subject_XMP_dc:
                        //    List<string> list = new List<string>();

                        //    while (i + 1 < result.Length)
                        //    {
                        //        list.Add(result[i + 1]);
                        //        i++;
                        //    }
                        //    td.Keywords = list; break;
                        // }
                }
            }

			//string[] result = lines.Split(new string[] { "\r\n", ": ", ", " }, StringSplitOptions.RemoveEmptyEntries);

			return td;
        }

        public static DateTime? GetDateTime(string fileName)
        {
            DateTime? tt = null;
            string tArgs = " -S -CreateDate " + fileName;
            List<string> lines = new List<string>();
            Constants.I.ET.Process(tArgs, lines);

            if (lines.Count != 0)
            {
                string[] result = lines[0].Split(new string[] { "\r\n", ": ", ", " }, StringSplitOptions.RemoveEmptyEntries);
                if (result.Length == 2)
                {
                    DateTime t;

                    bool b = DateTime.TryParseExact(result[1], "yyyy:MM:dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out t);
                    if (b)
                    {
                        tt = t; // t.ToLocalTime();
                    }
                }
            }

            return tt;
        }

        public static uint GetRating(string fileName)
		{
            string tArgs = " -S -Rating# " + fileName;
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
            string tArgs = " -overwrite_original -S -Rating#=" + rating + " " + fileName;
            Constants.I.ET.Process(tArgs);
			return Constants.I.ET.Success;
        }

        public static bool SetTagET(string fileName, TagsFlag flag, TagsData newData/*, bool append = false*/)
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
                        tArgs += "-Keywords=\"" + newData.KeywordsForET + "\" -sep " + "\", \" ";
                        tArgs += "-Subject=\"" + newData.KeywordsForET + "\" -sep " + "\", \" ";
                        break;
                    case TagsFlag.DateTime:
						tArgs += "-CreateDate=\"" + ExifTool.DateTimeToString(newData.DateTime) + "\" ";
						break;
                    case TagsFlag.OffsetTime: tArgs += "-OffsetTime=\"" + newData.OffsetTime.Value + "\" "; break;
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
					case TagsFlag.Rating: tArgs += "-Rating#=" + newData.Rating + " "; break;
                    case TagsFlag.Software: tArgs += "-Software=\"" + newData.Software + "\" "; break;
                    // other tags
                    case TagsFlag.Comment: tArgs += "-Comment=\"" + newData.Comment + "\" "; break;
                    case TagsFlag.Copyright: tArgs += "-Copyright=\"" + newData.Copyright + "\" "; break;
                    case TagsFlag.Title: tArgs += "-Title=\"" + newData.Title + "\" "; break;
                        //			default:
                        //				throw new NotImplementedException ();
                }

                flagValue >>= 1;
            }

			tArgs += fileName;
			Constants.I.ET.Process(tArgs);

            return Constants.I.ET.Success;
        }			       
    }
}

