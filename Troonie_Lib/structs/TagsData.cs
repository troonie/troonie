using System;
using System.Collections.Generic;
using System.Globalization;

namespace Troonie_Lib
{
    public struct TagsData
	{
		#region 16 image tagsData elements
		public double? Altitude;
		public string Creator;
		public DateTimeOffset? CreateDate;
        //public DateTime? TrackCreateDate;
        //public DateTime? MediaCreateDate;
        public OffsetTime OffsetTime;
		public double? ExposureTime;
		public ushort? Flash;
		public double? FNumber;
		public double? FocalLength;
		public uint? FocalLengthIn35mmFilm;
		public uint? ISOSpeedRatings;
		public List<string> Keywords;
		public double? Latitude;
		public double? Longitude;
		public string Make;
		public ushort? MeteringMode;
		public string Model;
		public uint Orientation;
		public uint? Rating;
		public string Software;
		#endregion

		#region other tagsData elements
		public string Comment;
		public string Copyright;
		public string Title;
		#endregion

		#region No TagsFlag elements
		public double OrientationDegree;
		public int Width;
		public int Height;
		public int Pixelformat;
		public long FileSize;
        #endregion No TagsFlag elements

		public string KeywordsString { get; private set; }

        public bool SetValues (TagsFlag flag, object o, bool IsVideo = false, bool UseDateTimeOriginalFlag = false)
		{
			bool tSuccess = false;
            uint flagValue = int.MaxValue;
            flagValue += 1;

			while (flagValue != 0)
			{
				switch (flag & (TagsFlag)flagValue)
				{

					case TagsFlag.CreateDate: tSuccess = ExtractDateTime(o, ref CreateDate, ref OffsetTime, IsVideo, UseDateTimeOriginalFlag); break;
					case TagsFlag.OffsetTime: tSuccess = ExtractOffsetTime(o, ref OffsetTime); break;
					case TagsFlag.Altitude: tSuccess = ExtractNullableDouble(o, ref Altitude); break;
					case TagsFlag.Creator: tSuccess = ExtractString(o, ref Creator); break;
					case TagsFlag.ExposureTime: tSuccess = ExtractNullableDouble(o, ref ExposureTime); break;
					case TagsFlag.Flash: tSuccess = ExtractNullableUshort(o, ref Flash); break;
					case TagsFlag.FNumber: tSuccess = ExtractNullableDouble(o, ref FNumber); break;
					case TagsFlag.FocalLength: tSuccess = ExtractNullableDouble(o, ref FocalLength); break;
					case TagsFlag.FocalLengthIn35mmFilm: tSuccess = ExtractNullableUint(o, ref FocalLengthIn35mmFilm); break;
					case TagsFlag.ISOSpeedRatings: tSuccess = ExtractNullableUint(o, ref ISOSpeedRatings); break;
					case TagsFlag.Keywords:
						Keywords = o as List<string>;
						//List<string> keywordList = o as List<string>;
						//if (keywordList == null /* || keywordList.Count == 0 */ ) {
						//	string[] keywordArray = o as string[];
						//	if (keywordArray == null /* || keywordArray.Length == 0 */ ) {
						//		tSuccess = false;
						//	} else {
						//		keywordList = new List<string> (keywordArray); 							
						//	}
						//}

						KeywordsString = Keywords.Count != 0 ? StringHelper.ReplaceGermanUmlauts(Keywords[0]) : string.Empty;
						for (int i = 1; i < Keywords.Count; i++)
						{
							Keywords[i] = StringHelper.ReplaceGermanUmlauts(Keywords[i]); 
							KeywordsString += ", " + Keywords[i];
						}
						//Keywords = keywordList;
						tSuccess = true;
                        break;
                    case TagsFlag.Latitude: tSuccess = ExtractNullableDouble(o, ref Latitude); break;
					case TagsFlag.Longitude: tSuccess = ExtractNullableDouble(o, ref Longitude); break;
					case TagsFlag.Make: tSuccess = ExtractString(o, ref Make); break;
					case TagsFlag.MeteringMode: tSuccess = ExtractNullableUshort(o, ref MeteringMode); break;
					case TagsFlag.Model: tSuccess = ExtractString(o, ref Model); break;
					case TagsFlag.Orientation:
						bool result = ExtractUint(o, ref Orientation); 
						CalcOrientationDegree(); 
						tSuccess = result;
                        break;
                    case TagsFlag.Rating: tSuccess = ExtractNullableUint(o, ref Rating); break;
					case TagsFlag.Software: tSuccess = ExtractString(o, ref Software); break;
					//				// other tags
					case TagsFlag.Comment: tSuccess = ExtractString(o, ref Comment); break;
					case TagsFlag.Copyright: tSuccess = ExtractString(o, ref Copyright); break;
					case TagsFlag.Title: tSuccess = ExtractString(o, ref Title); break;
                    // exiftool --> getting date time objects in videos
                    //case TagsFlag.CreateDate: tSuccess = ExtractDateTime(o, ref CreateDate); break;
                    //         case TagsFlag.TrackCreateDate: tSuccess = ExtractDateTime(o, ref TrackCreateDate); break;
                    //         case TagsFlag.MediaCreateDate: tSuccess = ExtractDateTime(o, ref MediaCreateDate); break;
                    //case TagsFlag.AllCreateDates:
                    //		DateTime? dt = null;
                    //                 bool b = ExtractDateTime(o, ref dt); 
                    //		SetAllCreateDates(dt); 
                    //		tSuccess = b;			
                    //		break;
                    // elements With, Height and PixelFormat will not get a 'SETTER'
				} // switch

                flagValue >>= 1;

            }// while

			return tSuccess;
		}
			
		public object GetSingleValue (TagsFlag flag)
		{
			switch (flag) {
			// image tags
			case TagsFlag.Altitude:		return Altitude;		
			case TagsFlag.Creator:		return Creator;			
			case TagsFlag.CreateDate:		return CreateDate;
            case TagsFlag.OffsetTime:	return OffsetTime.Value;
            case TagsFlag.ExposureTime:	return ExposureTime;	
			case TagsFlag.Flash:		return Flash;		
			case TagsFlag.FNumber:		return FNumber;			
			case TagsFlag.FocalLength:	return FocalLength;	
			case TagsFlag.FocalLengthIn35mmFilm:return FocalLengthIn35mmFilm;				
			case TagsFlag.ISOSpeedRatings:		return ISOSpeedRatings;						
			case TagsFlag.Keywords:		return Keywords;		
			case TagsFlag.Latitude:		return Latitude;		
			case TagsFlag.Longitude:	return Longitude;		
			case TagsFlag.Make:			return Make;		
			case TagsFlag.MeteringMode:		return MeteringMode;		
			case TagsFlag.Model:		return Model;			
			case TagsFlag.Orientation:	return Orientation;		
			case TagsFlag.Rating:		return Rating;			
			case TagsFlag.Software:		return Software;		
				// other tags
			case TagsFlag.Comment:		return Comment;				
			case TagsFlag.Copyright:	return Copyright;		
			case TagsFlag.Title:		return Title;	
			// No TagsFlag elements
			case TagsFlag.Width:		return Width;	
			case TagsFlag.Height:		return Height;
			case TagsFlag.Pixelformat:	return Pixelformat;
			case TagsFlag.FileSize:		return FileSize;
			// exiftool --> getting date time objects in videos
			//case TagsFlag.CreateDate:	return CreateDate;
			//case TagsFlag.TrackCreateDate:	return TrackCreateDate;
   //         case TagsFlag.MediaCreateDate: return MediaCreateDate;			
            // default:
            //	throw new NotImplementedException ();
            }

			return null;
		}

		public void CalcOrientationDegree()
		{
			switch ((Orientation)Orientation) {
			case Troonie_Lib.Orientation.None:
				OrientationDegree = 0;
				break;
				/// <summary>
				/// No need to do any transformations.
				/// </summary>
			case Troonie_Lib.Orientation.TopLeft:
				OrientationDegree = 0;
				break;
				/// <summary>
				/// TODO: Mirror image vertically.
				/// </summary>
			case Troonie_Lib.Orientation.TopRight:
				OrientationDegree = 360;
				break;
				/// <summary>
				/// Rotate image 180 degrees.
				/// </summary>
			case Troonie_Lib.Orientation.BottomRight:
				OrientationDegree = 180;
				break;
				/// <summary>
				///  TODO: Mirror image horizontally
				/// </summary>
			case Troonie_Lib.Orientation.BottomLeft:
				OrientationDegree = 360;
				break;
				/// <summary>
				///  TODO: Mirror image horizontally and rotate 90 degrees clockwise.
				/// </summary>
			case Troonie_Lib.Orientation.LeftTop:
				OrientationDegree = 360;
				break;
				/// <summary>
				/// Rotate image 90 degrees clockwise. Portrait value.
				/// </summary>
			case Troonie_Lib.Orientation.RightTop:
				OrientationDegree = 90;
				break;
				/// <summary>
				///  TODO: Mirror image vertically and rotate 90 degrees clockwise.
				/// </summary>
			case Troonie_Lib.Orientation.RightBottom:
				OrientationDegree = 360;
				break;
				/// <summary>
				/// Rotate image 270 degrees clockwise.
				/// </summary>
			case Troonie_Lib.Orientation.LeftBottom:
				OrientationDegree = 270;
				break;
			}

			//			orientationDegree = Math.PI * orientationDegree / 180.0;						
		}

		#region Private static helper functions

		private static bool ExtractNullableDouble(object o, ref double? d)
		{
			string s = o.ToString();
			if (s == null || s.Length == 0) {
				d = null;
				// true, because nullable double values
				return true;
			}

			s = s.Replace (',', '.');
			double tmp;
			bool b = double.TryParse (s, NumberStyles.AllowDecimalPoint, 
				CultureInfo.CreateSpecificCulture("en-us"), out tmp);
			if (b) {
				d = tmp;
				return true;
			} else { 
				return false;
			}
		}

		private static bool ExtractNullableUshort(object o, ref ushort? d)
		{
			string s = o.ToString();
			if (s == null || s.Length == 0) {
				d = null;
				// true, because nullable int values
				return true;
			}

			ushort tmp;
			bool b = ushort.TryParse (s, NumberStyles.AllowDecimalPoint, 
				CultureInfo.CreateSpecificCulture("en-us"), out tmp);
			if (b) {
				d = tmp;
				return true;
			} else { 
				return false;
			}
		}

		private static bool ExtractNullableUint(object o, ref uint? d)
		{
			string s = o.ToString();
			if (s == null || s.Length == 0) {
				d = null;
				// true, because nullable int values
				return true;
			}

			uint tmp;
			bool b = uint.TryParse (s, NumberStyles.AllowDecimalPoint, 
				CultureInfo.CreateSpecificCulture("en-us"), out tmp);
			if (b) {
				d = tmp;
				return true;
			} else { 
				return false;
			}
		}

		private static bool ExtractUint(object o, ref uint d)
		{
			string s = o.ToString();
			uint tmp;
			bool b = uint.TryParse (s, NumberStyles.AllowDecimalPoint, 
				CultureInfo.CreateSpecificCulture("en-us"), out tmp);
			if (b) {
				d = tmp;
				return true;
			} else { 
				return false;
			}
		}

		private static bool ExtractDateTime(object o, ref DateTimeOffset? dt, ref OffsetTime offset, bool IsVideo, bool UseDateTimeOriginalFlag)
		{ 
            string dt_string = string.Empty;
            bool b = ExtractString(o, ref dt_string);
                if (b)
                {
                    if (dt_string == string.Empty)
                    {
                        dt = null;
                    }
                    else
                    {
						DateTimeOffset tmp;
                        b = DateTimeOffset.TryParseExact(dt_string, "yyyy:MM:dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out tmp);

						if (b) 
						{
							// TODO Setting correct UTC time when video file
							#region checkforoffset
							if (IsVideo)
							{
								tmp = tmp.UtcDateTime;
                        }
							else
							{
								//offset.Value
							}
							#endregion


                        dt = tmp; 
						}
							
                    }
                }
                return b;
        }

        private static bool ExtractOffsetTime(object o, ref OffsetTime ot)
        {
            OffsetTime tmp = new OffsetTime(o.ToString());
            if (tmp.HasValidValue)
                ot = tmp;

			return tmp.HasValidValue;
        }

        private static bool ExtractString(object o, ref string s)
		{
			s = o.ToString();
			if (s == null) {
				return false;
			}

			s = StringHelper.ReplaceGermanUmlauts(s);

            return true;
		}

		#endregion Private static helper functions
	}
}

