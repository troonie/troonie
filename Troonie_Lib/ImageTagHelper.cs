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
            //bool success = true;
            string tArgs = " -overwrite_original -S -n " +
                            "-GPSAltitude " +
							"-Creator " +
                            "-CreateDate " +
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

            Constants.I.ET.Process(tArgs, true);

			//* Read the output
			// string output = proc.StandardOutput.ReadToEnd();
			foreach (string item in Constants.I.ET.Lines)
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
                    case "GPSAltitude": td.Altitude = allValues; break;
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
					case "Rating": td.Rating = result[1]; break;
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

		public static string GetRating(string fileName)
		{
            string tArgs = " -S -Rating# " + fileName;
			string rating = string.Empty;
            Constants.I.ET.Process(tArgs, true);

			//* Read the output
			// string output = proc.StandardOutput.ReadToEnd();
			if (Constants.I.ET.Lines.Count != 0)
			{
                string[] result = Constants.I.ET.Lines[0].Split(new string[] { "\r\n", ": ", ", " }, StringSplitOptions.RemoveEmptyEntries);
				if (result.Length == 2)
				{
                    //string key = result[0];
                    rating = result[1];
                }                
            }

			return rating;
        }


        public static bool SetRating(string fileName, string rating)
		{
            string tArgs = " -overwrite_original -S -\"" + "Rating#=" + rating + "\" " + fileName;
            return Constants.I.ET.Process(tArgs, false);
        }

        public static bool SetTagET(string fileName, TagsFlag flag, TagsData newData/*, bool append = false*/)
        {
            bool success = true;
			string tArgs = " -overwrite_original -S ";

            try
            {
                uint flagValue = int.MaxValue;
                flagValue += 1;

                while (flagValue != 0)
                {
                    switch (flag & (TagsFlag)flagValue)
                    {
                        // image tags
                        case TagsFlag.Altitude: tArgs += "-\"GPSAltitude#=" + newData.Altitude + "\" "; break;
                        case TagsFlag.Creator:
							//string tCreator = Enum.GetName(typeof(TagsFlag), flag);
							//tCreator = append ? tCreator + "<${" + tCreator + "}" : tCreator + "="; 
							tArgs += "-\"Creator=" + newData.Creator + "\" ";							
							break;
						case TagsFlag.Keywords:
                            tArgs += "-\"Keywords=" + newData.KeywordsForET + "\" -sep " + "\", \" ";
                            tArgs += "-\"Subject=" + newData.KeywordsForET + "\" -sep " + "\", \" ";
                            break;
                        case TagsFlag.DateTime:
							tArgs += "-\"CreateDate=" + ExifTool.DateTimeToString(newData.DateTime) + "\" ";
							break;
						//case TagsFlag.ExposureTime: imageTag.ExposureTime = newData.ExposureTime; break;
						//case TagsFlag.Flash:
						//    if (imageTag.Exif != null/* && newData.Flash.HasValue */)
						//    {
						//        foreach (IFDDirectory dir in imageTag.Exif.ExifIFD.Directories)
						//        {
						//            dir.Remove((ushort)ExifEntryTag.Flash);
						//            if (newData.Flash.HasValue)
						//            {
						//                dir.Add((ushort)ExifEntryTag.Flash, new ShortIFDEntry((ushort)ExifEntryTag.Flash, newData.Flash.Value));
						//                break; // TODO: check, if correct to add just in first directory
						//            }
						//        }
						//    }
						//    break;
						//case TagsFlag.FNumber: imageTag.FNumber = newData.FNumber; break;
						//case TagsFlag.FocalLength: imageTag.FocalLength = newData.FocalLength; break;
						//case TagsFlag.FocalLengthIn35mmFilm:
						//    imageTag.FocalLengthIn35mmFilm = newData.FocalLengthIn35mmFilm; break;
						//case TagsFlag.ISOSpeedRatings:
						//    imageTag.ISOSpeedRatings = newData.ISOSpeedRatings; break;
						//case TagsFlag.Keywords:
						//    imageTag.Keywords = (newData.Keywords != null && newData.Keywords.Count != 0) ? newData.Keywords.ToArray() : null;
						//    break;
						//case TagsFlag.Latitude: imageTag.Latitude = newData.Latitude; break;
						//case TagsFlag.Longitude: imageTag.Longitude = newData.Longitude; break;
						//case TagsFlag.Make: imageTag.Make = newData.Make; break;
						//case TagsFlag.MeteringMode:
						//    if (imageTag.Exif != null)
						//    {
						//        foreach (IFDDirectory dir in imageTag.Exif.ExifIFD.Directories)
						//        {
						//            dir.Remove((ushort)ExifEntryTag.MeteringMode);
						//            if (newData.MeteringMode.HasValue)
						//            {
						//                dir.Add((ushort)ExifEntryTag.MeteringMode, new ShortIFDEntry((ushort)ExifEntryTag.MeteringMode, newData.MeteringMode.Value));
						//                break; // TODO: check, if correct to add just in first directory
						//            }
						//        }
						//    }
						//    break;
						//case TagsFlag.Model: imageTag.Model = newData.Model; break;
						case TagsFlag.Orientation: tArgs += "-\"" + "Orientation#=" + newData.Orientation + "\" "; break;
						case TagsFlag.Rating: tArgs += "-\"" + "Rating#=" + newData.Rating + "\" "; break;
                            //case TagsFlag.Software: imageTag.Software = newData.Software; break;
                            //// other tags
                            //case TagsFlag.Comment: imageTag.Comment = newData.Comment; break;
                            //case TagsFlag.Copyright: imageTag.Copyright = newData.Copyright; break;
                            //case TagsFlag.Title: imageTag.Title = newData.Title; break;
                            //    //			default:
                            //    //				throw new NotImplementedException ();
                    }

                    flagValue >>= 1;
                }

				tArgs += fileName;
				
				if (!Constants.I.ET.Process(tArgs, false))
				{
                    Console.WriteLine("TODO: Do something when Exiftool does not work correctly.");
                }
            }
            catch (Exception e/* NotImplementedException e /* UnsupportedFormatException */ )
            {
                Console.WriteLine(e.Message);
                success = false;
            }

            return success;
        }

  //      public static bool SetTag(string fileName, TagsFlag flag, TagsData newData)
		//{
		//	bool success = true;
		//	TagLib.Image.File imageTagFile = LoadTagFile (fileName);
		//	if (imageTagFile == null) {
		//		return false;
		//	}

		//	CombinedImageTag imageTag = imageTagFile.ImageTag;

		//	try{
		//		ChangeValueOfTag (imageTag, flag, newData);
		//		imageTagFile.Save();
		//		imageTagFile.Dispose ();
		//	}
		//	catch (Exception e/* NotImplementedException e /* UnsupportedFormatException */ ) {
		//		Console.WriteLine (e.Message);
		//		success = false;
		//	}

		//	return success;
		//}
			
		public static void GetDateTime(string fileName, out DateTime? dateTime)
		{
			TagsData td = GetTagsDataET(fileName);
            dateTime = td.DateTime;

   //         CombinedImageTag tag = GetTag (fileName);
			//if (tag == null || tag.DateTime == null) {
			//	dateTime = null;
			//	return;
			//} else {
			//	dateTime = tag.DateTime;
			//}
		}

	}
}

