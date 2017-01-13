using System;
using TagLib.Image;
using System.Collections.Generic;
using System.Globalization;

namespace Troonie_Lib
{
	[Flags]
	public enum TagsFlag
	{
		None = 0,
		#region 16 image tags
		Altitude = 				1 << 0, // = 1,
		Creator = 				1 << 1, // = 2,
		DateTime = 				1 << 2, // = 4,
		ExposureTime = 			1 << 3, // = 8,
		FNumber = 				1 << 4, // = 16,
		FocalLength = 			1 << 5, // = 32,
		FocalLengthIn35mmFilm = 1 << 6, // = 64,
		ISOSpeedRatings = 		1 << 7, // = 128,
		Keywords = 				1 << 8, // = 256,
		Latitude = 				1 << 9, // = 512,
		Longitude = 			1 << 10, // = 1024,
		Make = 					1 << 11, // = 2048,
		Model = 				1 << 12, // = 4096,
		Orientation = 			1 << 13, // = 8192,
		Rating = 				1 << 14, // = 16384,
		Software = 				1 << 15, // = 32768,
		#endregion

		#region 8 other tags			
		Comment = 				1 << 16, // = 65536,
		Composers = 			1 << 17, // = 131072,
		Conductor = 			1 << 18, // = 262144,
		Copyright = 			1 << 19, // = 524288,
		Title = 				1 << 20, // = 1048576,
		Track = 				1 << 21, // = 2097152,
		TrackCount =			1 << 22, // = 4194304
		Year = 					1 << 23, 
		#endregion
	}

	public struct TagsData
	{
		#region 16 image tagsData elements
		public double? Altitude;
		public string Creator;
		public DateTime? DateTime;
		public double? ExposureTime;
		public double? FNumber;
		public double? FocalLength;
		public uint? FocalLengthIn35mmFilm;
		public uint? ISOSpeedRatings;
		public List<string> Keywords;
		public double? Latitude;
		public double? Longitude;
		public string Make;
		public string Model;
		public uint Orientation;
		public uint? Rating;
		public string Software;
		#endregion

		#region 8 other tagsData elements
		public string Comment;
		public List<string> Composers;
		public string Conductor;
		public string Copyright;
		public string Title;
		public uint Track;
		public uint TrackCount;
		public uint Year;
		#endregion

		#region No tagsData elements
		public double OrientationDegree;
		#endregion No tagsData elements

		public bool SetValue (TagsFlag flag, object o)
		{
			switch (flag) {
			case TagsFlag.DateTime:
				DateTime dt;
				string dt_string = string.Empty;
				bool b = ExtractString (o, ref dt_string);	
				if (b) {
					b = System.DateTime.TryParse (dt_string, out dt);
					DateTime = dt;
				}
				return b;
			case TagsFlag.Altitude:		return ExtractNullableDouble (o, ref Altitude);		
			case TagsFlag.Creator:		return ExtractString(o, ref Creator);					
			case TagsFlag.ExposureTime:	return ExtractNullableDouble (o, ref ExposureTime);		
			case TagsFlag.FNumber:		return ExtractNullableDouble (o, ref FNumber);
			case TagsFlag.FocalLength:	return ExtractNullableDouble (o, ref FocalLength);
			case TagsFlag.FocalLengthIn35mmFilm:	return ExtractNullableUint (o, ref FocalLengthIn35mmFilm);				
			case TagsFlag.ISOSpeedRatings:			return ExtractNullableUint (o, ref ISOSpeedRatings);	
			case TagsFlag.Keywords:	
			case TagsFlag.Composers:	
				List<string> keywordList = o as List<string>;
				if (keywordList == null || keywordList.Count == 0) {
					string[] keywordArray = o as string[];
					if (keywordArray == null || keywordArray.Length == 0) {
						return false;
					} else {
						keywordList = new List<string> (keywordArray);							
					}
				}

				if (flag == TagsFlag.Keywords) {
					Keywords = keywordList;
				} else {
					Composers = keywordList;
				}
				return true;		
			case TagsFlag.Latitude:		return ExtractNullableDouble (o, ref Latitude);			
			case TagsFlag.Longitude:	return ExtractNullableDouble (o, ref Longitude);			
			case TagsFlag.Make:			return ExtractString(o, ref Make);			
			case TagsFlag.Model:		return ExtractString(o, ref Model);			
			case TagsFlag.Orientation:	
				bool result = ExtractUint (o, ref Orientation);
				CalcOrientationDegree ();
				return result;		
			case TagsFlag.Rating:		return ExtractNullableUint (o, ref Rating);		
			case TagsFlag.Software:		return ExtractString(o, ref Software);		
//				// other tags
			case TagsFlag.Comment:		return ExtractString(o, ref Comment);					
			case TagsFlag.Conductor:	return ExtractString(o, ref Conductor);		
			case TagsFlag.Copyright:	return ExtractString(o, ref Copyright);		
			case TagsFlag.Title:		return ExtractString(o, ref Title);			
			case TagsFlag.Track:		return ExtractUint (o, ref Track);		
			case TagsFlag.TrackCount:	return ExtractUint (o, ref TrackCount);
			case TagsFlag.Year:			return ExtractUint (o, ref Year);


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
			case TagsFlag.ExposureTime:	return ExposureTime;	
			case TagsFlag.FNumber:		return FNumber;			
			case TagsFlag.FocalLength:	return FocalLength;	
			case TagsFlag.FocalLengthIn35mmFilm:return FocalLengthIn35mmFilm;				
			case TagsFlag.ISOSpeedRatings:		return ISOSpeedRatings;						
			case TagsFlag.Keywords:		return Keywords;		
			case TagsFlag.Latitude:		return Latitude;		
			case TagsFlag.Longitude:	return Longitude;		
			case TagsFlag.Make:			return Make;			
			case TagsFlag.Model:		return Model;			
			case TagsFlag.Orientation:	return Orientation;		
			case TagsFlag.Rating:		return Rating;			
			case TagsFlag.Software:		return Software;		
				// other tags
			case TagsFlag.Comment:		return Comment;			
			case TagsFlag.Composers:	return Composers;		
			case TagsFlag.Conductor:	return Conductor;		
			case TagsFlag.Copyright:	return Copyright;		
			case TagsFlag.Title:		return Title;			
			case TagsFlag.Track:		return Track;			
			case TagsFlag.TrackCount:	return TrackCount;		
			case TagsFlag.Year:			return Year;			
//			default:
//				throw new NotImplementedException ();
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

	public class ImageTagHelper
	{
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
			
		private static TagLib.Image.File LoadTagFile(string fileName)
		{
			TagLib.Image.File imageTagFile;
			imageTagFile = TagLib.File.Create(fileName) as TagLib.Image.File;
			if (imageTagFile == null){
				return null;
			}

			imageTagFile.EnsureAvailableTags ();
	
			return imageTagFile;
		}

		// Does not work. When PNG corrupt, then also not possible to create new one...
//		private static TagLib.Image.File TryCreatePndFile(string fileName)
//		{
//			TagLib.Image.File imageTagFile = null;
//
//			// try getting new TagLib.Image.File if extension is PNG
//			FileInfo info = new FileInfo (fileName);
//			string ext = info.Extension.ToLower ();
//			if (ext.Length != 0 && Constants.Extensions [TroonieImageFormat.PNG24].Item1 == ext) { 
//				// (x => x.Value.Item1 == ext || x.Value.Item2 == ext)) {
//				imageTagFile = new TagLib.Png.File (fileName);
//				imageTagFile.EnsureAvailableTags ();
//			}
//			return imageTagFile;
//		}

		public static void CopyTagToFile(string fileName, CombinedImageTag tag)
		{
			//			TagLib.Image.File imageTagFile;
			//			try{
			//				imageTagFile = TagLib.File.Create(fileName) as TagLib.Image.File;
			//			}
			//			catch (Exception /* UnsupportedFormatException */) {
			//				return;
			//			}
			//
			//			imageTagFile.EnsureAvailableTags();

			TagLib.Image.File imageTagFile = LoadTagFile(fileName);

			if (tag == null || imageTagFile == null)
				return;

			// all general tags
			tag.CopyTo(imageTagFile.ImageTag, true);

			// all image tags
			if (tag.Keywords != null) imageTagFile.ImageTag.Keywords = tag.Keywords;
			if (tag.Rating != null) imageTagFile.ImageTag.Rating = tag.Rating;
			if (tag.DateTime != null) imageTagFile.ImageTag.DateTime = tag.DateTime;
			imageTagFile.ImageTag.Orientation = tag.Orientation;
			if (tag.Software != null) imageTagFile.ImageTag.Software = tag.Software;
			if (tag.Latitude != null) imageTagFile.ImageTag.Latitude = tag.Latitude;
			if (tag.Longitude != null) imageTagFile.ImageTag.Longitude = tag.Longitude;
			if (tag.Altitude != null) imageTagFile.ImageTag.Altitude = tag.Altitude;
			if (tag.ExposureTime != null) imageTagFile.ImageTag.ExposureTime = tag.ExposureTime;
			if (tag.FNumber != null) imageTagFile.ImageTag.FNumber = tag.FNumber;
			if (tag.ISOSpeedRatings != null) imageTagFile.ImageTag.ISOSpeedRatings = tag.ISOSpeedRatings;
			if (tag.FocalLength != null) imageTagFile.ImageTag.FocalLength = tag.FocalLength;
			if (tag.FocalLengthIn35mmFilm != null) imageTagFile.ImageTag.FocalLengthIn35mmFilm = tag.FocalLengthIn35mmFilm;
			if (tag.Make != null) imageTagFile.ImageTag.Make = tag.Make;
			if (tag.Model != null) imageTagFile.ImageTag.Model = tag.Model;
			if (tag.Creator != null) imageTagFile.ImageTag.Creator = tag.Creator;

			// examples for setting properties manually
			//			imageTagFile.ImageTag.Creator = "MARKI";
			//			imageTagFile.ImageTag.FocalLength = 33.0;
			//			imageTagFile.ImageTag.Rating = 5;

			try{
				imageTagFile.Save();
			}
			catch (Exception /* UnsupportedFormatException */) {
				// do nothing
			}

			imageTagFile.Dispose ();
		}

		public static TagsData GetTagsData(string fileName)
		{
			TagsData td = new TagsData ();
			CombinedImageTag cit = GetTag (fileName);
			if (cit != null) {				
				// image tags
				td.Altitude = cit.Altitude;
				td.Creator = cit.Creator;
				td.DateTime = cit.DateTime;
				td.ExposureTime = cit.ExposureTime;
				td.FNumber = cit.FNumber;
				td.FocalLength = cit.FocalLength;
				td.FocalLengthIn35mmFilm = cit.FocalLengthIn35mmFilm;
				td.ISOSpeedRatings = cit.ISOSpeedRatings;
				td.Keywords = new List<string>(cit.Keywords);
				td.Latitude = cit.Latitude;
				td.Longitude = cit.Longitude;
				td.Make = cit.Make;
				td.Model = cit.Model;
				td.Orientation = (uint)cit.Orientation;
				td.CalcOrientationDegree();
				td.Rating = cit.Rating;
				td.Software = cit.Software;
				// other tags
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

		public static CombinedImageTag GetTag(string fileName)
		{
			//			TagLib.Image.File imageTagFile;
			//			try{
			//				imageTagFile = TagLib.File.Create(fileName) as TagLib.Image.File;
			//				if (imageTagFile == null){
			//					return null;
			//				}
			//			}
			//			catch (Exception /* UnsupportedFormatException */) {
			//				return null;
			//			}				
			//
			////			if (imageTagFile.ImageTag != null && imageTagFile.ImageTag.AllTags.Count == 0) {
			//				imageTagFile.EnsureAvailableTags ();
			////			}

			TagLib.Image.File imageTagFile = LoadTagFile(fileName);

			if (imageTagFile == null)
				return null;

			CombinedImageTag tag = imageTagFile.ImageTag;
			imageTagFile.Dispose ();
			return tag;
		}

		public static void ChangeValueOfTag(CombinedImageTag imageTag, TagsFlag flag, TagsData newData)
		{
			uint flagValue = int.MaxValue;
			flagValue += 1;

			while(flagValue != 0)
			{
				switch (flag & (TagsFlag)flagValue) {
				// image tags
	//			case tag.HasFlag(Tags.Copyright): break;
				case TagsFlag.Altitude:		imageTag.Altitude = newData.Altitude; 			break;
				case TagsFlag.Creator:		imageTag.Creator = newData.Creator;				break;
				case TagsFlag.DateTime:		imageTag.DateTime = newData.DateTime; 			break;
				case TagsFlag.ExposureTime:	imageTag.ExposureTime = newData.ExposureTime;	break;
				case TagsFlag.FNumber:		imageTag.FNumber = newData.FNumber;				break;
				case TagsFlag.FocalLength:	imageTag.FocalLength = newData.FocalLength;		break;
				case TagsFlag.FocalLengthIn35mmFilm: 
					imageTag.FocalLengthIn35mmFilm = newData.FocalLengthIn35mmFilm;		break;
				case TagsFlag.ISOSpeedRatings:	
					imageTag.ISOSpeedRatings = newData.ISOSpeedRatings;					break;
				case TagsFlag.Keywords:		imageTag.Keywords = newData.Keywords.ToArray();	break;
				case TagsFlag.Latitude:		imageTag.Latitude = newData.Latitude;			break;
				case TagsFlag.Longitude:	imageTag.Longitude = newData.Longitude;			break;
				case TagsFlag.Make:			imageTag.Make = newData.Make;					break;
				case TagsFlag.Model:		imageTag.Model = newData.Model;					break;
				case TagsFlag.Orientation:	imageTag.Orientation = 
					(ImageOrientation) newData.Orientation;								break;
				case TagsFlag.Rating:		imageTag.Rating = newData.Rating;				break;
				case TagsFlag.Software:		imageTag.Software = newData.Software;			break;
				// other tags
				case TagsFlag.Comment:		imageTag.Comment = newData.Comment;				break;
				case TagsFlag.Composers:	imageTag.Composers= newData.Composers.ToArray();break;
				case TagsFlag.Conductor:	imageTag.Conductor = newData.Conductor;			break;
				case TagsFlag.Copyright:	imageTag.Copyright = newData.Copyright;			break;
				case TagsFlag.Title:		imageTag.Title = newData.Title;					break;
				case TagsFlag.Track:		imageTag.Track = newData.Track;					break;
				case TagsFlag.TrackCount:	imageTag.TrackCount = newData.TrackCount;		break;
				case TagsFlag.Year:			imageTag.Year = newData.Year;					break;
	//			default:
	//				throw new NotImplementedException ();
				}

				flagValue >>= 1;
			}
		}

		public static bool SetTag(string fileName, TagsFlag flag, TagsData newData)
		{
			bool success = true;
			TagLib.Image.File imageTagFile = LoadTagFile (fileName);
			if (imageTagFile == null) {
				return false;
			}

			CombinedImageTag imageTag = imageTagFile.ImageTag;

			try{
				ChangeValueOfTag (imageTag, flag, newData);
				imageTagFile.Save();
				imageTagFile.Dispose ();
			}
			catch (Exception e /* UnsupportedFormatException */ ) {
				Console.WriteLine (e.Message);
				success = false;
			}

			return success;
		}

		public static void GetDateTime(string fileName, out DateTime? dateTime)
		{
			CombinedImageTag tag = GetTag (fileName);
			if (tag == null || tag.DateTime == null) {
				dateTime = null;
				return;
			} else {
				dateTime = tag.DateTime;
			}
		}

//		public static void ChangeValueOfTag(CombinedImageTag imageTag, Tags tag, string newValue)
//		{
////			string tagName = tagNames [(int)tag];
//			
//			switch (tag) {
//			case Tags.Rating:
//				throw new MethodAccessException ("Please use ChangeValueOfTag(CombinedImageTag imageTag, Tags tag, int newValue).");
//			case Tags.Creator:
//				imageTag.Creator = newValue;
//				break;
////			case Tags.Conductor:
////				imageTag.Conductor = newValue;
////				break;
////			case "Copyright":
////				imageTag.Copyright = newValue;
////				break;
//			default:
//				throw new NotImplementedException ();
//			}	
//		}				

//		public static void GetImageRating(string fileName, out int rating)
//		{
//			CombinedImageTag tag = ExtractImageTag (fileName);
//			if (tag == null || tag.Rating == null) {
//				rating = -1;
//				//				dateTime = null;
//				return;
//			} 
//			else {
//				rating = (int)tag.Rating;
//				//				dateTime = tag.DateTime;
//			}
//		}

//		public static bool SetAndSave_Rating(string fileName, uint? newValue)
//		{
//			bool success = true;
//			TagLib.Image.File imageTagFile = LoadTagFile (fileName);
//			CombinedImageTag imageTag = imageTagFile.ImageTag;
//
//			try{
//				imageTag.Rating = newValue;
//				imageTagFile.Save();
//				imageTagFile.Dispose ();
//			}
//			catch (Exception /* UnsupportedFormatException */ ) {
//				success = false;
//			}
//
//			return success;
//		}

//		public static bool SetAndSaveTag(string fileName, Tags tag, string newValue)
//		{
//			bool success = true;
//			TagLib.Image.File imageTagFile = LoadTagFile (fileName);
//			CombinedImageTag imageTag = imageTagFile.ImageTag;
//
//			try{
//				ChangeValueOfTag (imageTag, tag, newValue);
//				imageTagFile.Save();
//				imageTagFile.Dispose ();
//			}
//			catch (Exception /* UnsupportedFormatException */ ) {
//				success = false;
//			}
//
//			return success;
//		}
	}
}

