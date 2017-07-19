using System;
using TagLib.Image;
using System.Collections.Generic;
using System.Globalization;
using TagLib.IFD;
using TagLib.IFD.Entries;
using TagLib.Xmp;
using TagLib.IFD.Tags;

namespace Troonie_Lib
{
	[Flags]
	public enum TagsFlag
	{
		None = 0,
		#region 16+1 image tags
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

		Flash = 				1 << 16, // = 65536,
		#endregion

		#region other tags			
		Comment = 				1 << 17, // = 131072,
		Copyright = 			1 << 18, // = 262144,
		Title = 				1 << 19, // = 524288, 
		MeteringMode =			1 << 20,

		Width =					1 << 21,
		Height =				1 << 22,
		Pixelformat =			1 << 23
		#endregion
	}

	public struct TagsData
	{
		#region 16 image tagsData elements
		public double? Altitude;
		public string Creator;
		public DateTime? DateTime;
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
		#endregion No TagsFlag elements

		public bool SetValue (TagsFlag flag, object o)
		{
			switch (flag) {
			case TagsFlag.DateTime:
				DateTime dt;
				string dt_string = string.Empty;
				bool b = ExtractString (o, ref dt_string);	
				if (b) {
					if (dt_string == string.Empty) {
						DateTime = null;
					} else {
						b = System.DateTime.TryParse (dt_string, out dt);
						DateTime = dt;
					}
				}
				return b;
			case TagsFlag.Altitude:		return ExtractNullableDouble (o, ref Altitude);		
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
			case TagsFlag.Rating:		return ExtractNullableUint (o, ref Rating);		
			case TagsFlag.Software:		return ExtractString(o, ref Software);		
//				// other tags
			case TagsFlag.Comment:		return ExtractString(o, ref Comment);							
			case TagsFlag.Copyright:	return ExtractString(o, ref Copyright);		
			case TagsFlag.Title:		return ExtractString(o, ref Title);			
//			// elements With, Height and PixelFormat will not get a 'SETTER'
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
			case TagsFlag.Width:			return Width;	
			case TagsFlag.Height:		return Height;
			case TagsFlag.Pixelformat:	return Pixelformat;
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

//		private static bool ExtractInt(object o, ref int d)
//		{
//			string s = o.ToString();
//			int tmp;
//			bool b = int.TryParse (s, NumberStyles.AllowDecimalPoint, 
//				CultureInfo.CreateSpecificCulture("en-us"), out tmp);
//			if (b) {
//				d = tmp;
//				return true;
//			} else { 
//				return false;
//			}
//		}

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
			TagLib.Image.File imageTagFile = LoadTagFile(fileName);

			if (tag == null || imageTagFile == null)
				return;

			foreach (var prop in typeof (TagLib.Image.ImageTag).GetProperties ()) {
				if (!prop.CanWrite || prop.Name == "TagTypes")
					continue;

				var value = prop.GetValue (tag, null);
				prop.SetValue (imageTagFile.ImageTag, value, null);
			}
				
			// OLD version: all general tags
//				tag.CopyTo(imageTagFile.ImageTag, true);

			// all jpeg exifIFD tags, if exists
			if (tag.Exif != null && imageTagFile.ImageTag.Exif != null) {
				for (int i = 0; i < tag.Exif.ExifIFD.Directories.Length; i++) {
					IFDDirectory dir_old = tag.Exif.ExifIFD.Directories [i];
					IFDDirectory dir_new = imageTagFile.ImageTag.Exif.ExifIFD.Directories [i];

					foreach (KeyValuePair<ushort, IFDEntry> entry in dir_old) {
						if (!dir_new.ContainsKey (entry.Key)) {
							dir_new.Add (entry.Key, entry.Value);
						}
					}
				}
			}	
							
//			// OLD version: all image tags without exif(jpeg) specifics
//			if (tag.Keywords != null) imageTagFile.ImageTag.Keywords = tag.Keywords;
//			if (tag.Rating != null) imageTagFile.ImageTag.Rating = tag.Rating;
//			if (tag.DateTime != null) imageTagFile.ImageTag.DateTime = tag.DateTime;
//			imageTagFile.ImageTag.Orientation = tag.Orientation;
//			if (tag.Software != null) imageTagFile.ImageTag.Software = tag.Software;
//			if (tag.Latitude != null) imageTagFile.ImageTag.Latitude = tag.Latitude;
//			if (tag.Longitude != null) imageTagFile.ImageTag.Longitude = tag.Longitude;
//			if (tag.Altitude != null) imageTagFile.ImageTag.Altitude = tag.Altitude;
//			if (tag.ExposureTime != null) imageTagFile.ImageTag.ExposureTime = tag.ExposureTime;
//			if (tag.ISOSpeedRatings != null) imageTagFile.ImageTag.ISOSpeedRatings = tag.ISOSpeedRatings;
//			if (tag.FNumber != null) imageTagFile.ImageTag.FNumber = tag.FNumber;
//			if (tag.FocalLength != null) imageTagFile.ImageTag.FocalLength = tag.FocalLength;
//			if (tag.FocalLengthIn35mmFilm != null) imageTagFile.ImageTag.FocalLengthIn35mmFilm = tag.FocalLengthIn35mmFilm;
//			if (tag.Make != null) imageTagFile.ImageTag.Make = tag.Make;
//			if (tag.Model != null) imageTagFile.ImageTag.Model = tag.Model;
//			if (tag.Creator != null) imageTagFile.ImageTag.Creator = tag.Creator;


			try{
				imageTagFile.Save();
			}
			catch (Exception /* UnsupportedFormatException */) {
				// do nothing, will be caused by formats without tags, e.g. .bmp
//				Console.WriteLine (ex2.Message);
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

				if (cit.Exif != null) {		
					#region FLASH
					IFDDirectory dir = null;
					IFDEntry entry1;
					ShortIFDEntry shortIFDEntry;
					bool existEntry = false;
					for (int i = 0; i < cit.Exif.ExifIFD.Directories.Length; i++ ) {
						if (existEntry = cit.Exif.ExifIFD.ContainsTag (i, (ushort)ExifEntryTag.Flash)) {							
							dir = cit.Exif.ExifIFD.Directories[i];
							break;
						}
					}
						
					if (existEntry && dir != null) {						
						dir.TryGetValue((ushort)ExifEntryTag.Flash, out entry1);
						shortIFDEntry = entry1 as ShortIFDEntry;
						if (shortIFDEntry != null) {
							td.Flash = shortIFDEntry.Value;
						}
					}
					#endregion FLASH

					#region METERINGMODE
					dir = null;
					existEntry = false;
					for (int i = 0; i < cit.Exif.ExifIFD.Directories.Length; i++ ) {
						if (existEntry = cit.Exif.ExifIFD.ContainsTag (i, (ushort)ExifEntryTag.MeteringMode)) {							
							dir = cit.Exif.ExifIFD.Directories[i];
							break;
						}
					}

					if (existEntry && dir != null) {						
						dir.TryGetValue((ushort)ExifEntryTag.MeteringMode, out entry1);
						shortIFDEntry = entry1 as ShortIFDEntry;
						if (shortIFDEntry != null) {
							td.MeteringMode = shortIFDEntry.Value;
						}
					}
					#endregion METERINGMODE
				}
					
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
				td.Copyright = cit.Copyright;
				td.Title = cit.Title;
			}

			return td;
		}

		public static CombinedImageTag GetTag(string fileName)
		{
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
				case TagsFlag.Altitude:		imageTag.Altitude = newData.Altitude; 			break;
				case TagsFlag.Creator:		imageTag.Creator = newData.Creator;				break;
				case TagsFlag.DateTime:		imageTag.DateTime = newData.DateTime; 			break;
				case TagsFlag.ExposureTime:	imageTag.ExposureTime = newData.ExposureTime;	break;
				case TagsFlag.Flash:					
					if (imageTag.Exif != null/* && newData.Flash.HasValue */) {
						foreach (IFDDirectory dir in imageTag.Exif.ExifIFD.Directories) {
							dir.Remove((ushort)ExifEntryTag.Flash);
							if (newData.Flash.HasValue) {
								dir.Add ((ushort)ExifEntryTag.Flash, new ShortIFDEntry ((ushort)ExifEntryTag.Flash, newData.Flash.Value));
								break; // TODO: check, if correct to add just in first directory
							}
						}
					}
					break;
				case TagsFlag.FNumber:		imageTag.FNumber = newData.FNumber;				break;
				case TagsFlag.FocalLength:	imageTag.FocalLength = newData.FocalLength;		break;
				case TagsFlag.FocalLengthIn35mmFilm: 
						imageTag.FocalLengthIn35mmFilm = newData.FocalLengthIn35mmFilm;		break;
				case TagsFlag.ISOSpeedRatings:	
						imageTag.ISOSpeedRatings = newData.ISOSpeedRatings;					break;
				case TagsFlag.Keywords:		
					imageTag.Keywords = (newData.Keywords != null && newData.Keywords.Count != 0) ? newData.Keywords.ToArray() : null;	
					break;
				case TagsFlag.Latitude:		imageTag.Latitude = newData.Latitude;			break;
				case TagsFlag.Longitude:	imageTag.Longitude = newData.Longitude;			break;
				case TagsFlag.Make:			imageTag.Make = newData.Make;					break;
				case TagsFlag.MeteringMode:
					if (imageTag.Exif != null) {
						foreach (IFDDirectory dir in imageTag.Exif.ExifIFD.Directories) {
							dir.Remove((ushort)ExifEntryTag.MeteringMode);
							if (newData.MeteringMode.HasValue) {
								dir.Add ((ushort)ExifEntryTag.MeteringMode, new ShortIFDEntry ((ushort)ExifEntryTag.MeteringMode, newData.MeteringMode.Value));
								break; // TODO: check, if correct to add just in first directory
							}
						}
					}
					break;					
				case TagsFlag.Model:		imageTag.Model = newData.Model;					break;
				case TagsFlag.Orientation:	imageTag.Orientation = 
						(ImageOrientation) newData.Orientation;								break;
				case TagsFlag.Rating:		imageTag.Rating = newData.Rating;				break;
				case TagsFlag.Software:		imageTag.Software = newData.Software;			break;
				// other tags
				case TagsFlag.Comment:		imageTag.Comment = newData.Comment;				break;
				case TagsFlag.Copyright:	imageTag.Copyright = newData.Copyright;			break;
				case TagsFlag.Title:		imageTag.Title = newData.Title;					break;
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
			catch (Exception /* NotImplementedException e /* UnsupportedFormatException */ ) {
//				Console.WriteLine (e.Message);
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

	}
}

