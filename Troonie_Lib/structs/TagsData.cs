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
		public DateTime? DateTime;
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

        #region exiftool --> getting date time objects in videos
        public DateTime? TrackCreateDate;
		public DateTime? MediaCreateDate;

		//public DateTime? AllCreateDates	{ set {	CreateDate = TrackCreateDate = MediaCreateDate = value; } }

		public string KeywordsForET 
		{ 
			get 
			{
                string sKeywords = string.Empty;
                if (Keywords != null && Keywords.Count > 0)
                {
                    sKeywords = Keywords[0];

                    for (int i = 1; i < Keywords.Count; i++)
                    {
                        sKeywords += ", " + StringHelper.ReplaceGermanUmlauts(Keywords[i]);
                    }
					// sKeywords += "\" -sep ";

                }
                return sKeywords;
			} 
		}

		public void SetAllCreateDates(DateTime? dt)
		{
            DateTime = dt;
            TrackCreateDate = dt;
            MediaCreateDate = dt;
        }

        #endregion

        public bool SetValue (TagsFlag flag, object o)
		{
			switch (flag) {
			case TagsFlag.DateTime: return ExtractDateTime(o, ref DateTime);
            case TagsFlag.OffsetTime: return ExtractOffsetTime(o, ref OffsetTime);
            case TagsFlag.Altitude:		return ExtractNullableDouble(o, ref Altitude);		
			case TagsFlag.Creator:		return ExtractString(o, ref Creator);					
			case TagsFlag.ExposureTime:	return ExtractNullableDouble (o, ref ExposureTime);	
			case TagsFlag.Flash:		return ExtractNullableUshort (o, ref Flash);	
			case TagsFlag.FNumber:		return ExtractNullableDouble (o, ref FNumber);
			case TagsFlag.FocalLength:	return ExtractNullableDouble (o, ref FocalLength);
			case TagsFlag.FocalLengthIn35mmFilm:	return ExtractNullableUint (o, ref FocalLengthIn35mmFilm);				
			case TagsFlag.ISOSpeedRatings:			return ExtractNullableUint (o, ref ISOSpeedRatings);	
			case TagsFlag.Keywords:	
				List<string> keywordList = o as List<string>;
				if (keywordList == null /* || keywordList.Count == 0 */ ) {
					string[] keywordArray = o as string[];
					if (keywordArray == null /* || keywordArray.Length == 0 */ ) {
						return false;
					} else {
						keywordList = new List<string> (keywordArray);							
					}
				}
				Keywords = keywordList;
				return true;		
			case TagsFlag.Latitude:		return ExtractNullableDouble (o, ref Latitude);			
			case TagsFlag.Longitude:	return ExtractNullableDouble (o, ref Longitude);			
			case TagsFlag.Make:			return ExtractString(o, ref Make);			
			case TagsFlag.MeteringMode:		return ExtractNullableUshort (o, ref MeteringMode);					
			case TagsFlag.Model:		return ExtractString(o, ref Model);			
			case TagsFlag.Orientation:	
				bool result = ExtractUint (o, ref Orientation);
				CalcOrientationDegree ();
				return result;		
			case TagsFlag.Rating:		return ExtractNullableUint(o, ref Rating);		
			case TagsFlag.Software:		return ExtractString(o, ref Software);		
//				// other tags
			case TagsFlag.Comment:		return ExtractString(o, ref Comment);							
			case TagsFlag.Copyright:	return ExtractString(o, ref Copyright);		
			case TagsFlag.Title:		return ExtractString(o, ref Title);
            // exiftool --> getting date time objects in videos
            //case TagsFlag.CreateDate: return ExtractDateTime(o, ref CreateDate);
            case TagsFlag.TrackCreateDate: return ExtractDateTime(o, ref TrackCreateDate);
            case TagsFlag.MediaCreateDate: return ExtractDateTime(o, ref MediaCreateDate);
			case TagsFlag.AllCreateDates:
					DateTime? dt = null;
                    bool b = ExtractDateTime(o, ref dt);
					SetAllCreateDates(dt);
					return b;			
            // elements With, Height and PixelFormat will not get a 'SETTER'
            default:
			return false;
			}				
		}
			
		public object GetValue (TagsFlag flag)
		{
			switch (flag) {
			// image tags
			case TagsFlag.Altitude:		return Altitude;		
			case TagsFlag.Creator:		return Creator;			
			case TagsFlag.DateTime:		return DateTime;
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
			case TagsFlag.TrackCreateDate:	return TrackCreateDate;
            case TagsFlag.MediaCreateDate: return MediaCreateDate;			
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

		private static bool ExtractDateTime(object o, ref DateTime? dt)
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
						DateTime tmp;
                        b = System.DateTime.TryParse(dt_string, out tmp);
						if (b) 
						{ 
							dt = tmp; 
						}
							
                    }
                }
                return b;
        }

        private static bool ExtractOffsetTime(object o, ref OffsetTime ot)
        {
            ot = new OffsetTime(o.ToString());            
            return ot.HasValidValue;
        }

        private static bool ExtractString(object o, ref string s)
		{
			s = o.ToString();
			if (s == null) {
				return false;
			}

			return true;
		}

		#endregion Private static helper functions
	}
}

