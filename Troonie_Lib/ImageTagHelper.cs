using System;
using TagLib.Image;
using System.Collections.Generic;

namespace Troonie_Lib
{
	public enum Tags
	{
		Keywords,
		Rating,
		DateTime,
		Orientation,
		Software,
		Latitude,
		Longitude,
		Altitude,
		ExposureTime,
		FNumber,
		ISOSpeedRatings,
		FocalLength,
		FocalLengthIn35mmFilm,
		Make,
		Model,
		Creator

		// all image tags
//		if (tag.Keywords != null) imageTagFile.ImageTag.Keywords = tag.Keywords;
//		if (tag.Rating != null) imageTagFile.ImageTag.Rating = tag.Rating;
//		if (tag.DateTime != null) imageTagFile.ImageTag.DateTime = tag.DateTime;
//		imageTagFile.ImageTag.Orientation = tag.Orientation;
//		if (tag.Software != null) imageTagFile.ImageTag.Software = tag.Software;
//		if (tag.Latitude != null) imageTagFile.ImageTag.Latitude = tag.Latitude;
//		if (tag.Longitude != null) imageTagFile.ImageTag.Longitude = tag.Longitude;
//		if (tag.Altitude != null) imageTagFile.ImageTag.Altitude = tag.Altitude;
//		if (tag.ExposureTime != null) imageTagFile.ImageTag.ExposureTime = tag.ExposureTime;
//		if (tag.FNumber != null) imageTagFile.ImageTag.FNumber = tag.FNumber;
//		if (tag.ISOSpeedRatings != null) imageTagFile.ImageTag.ISOSpeedRatings = tag.ISOSpeedRatings;
//		if (tag.FocalLength != null) imageTagFile.ImageTag.FocalLength = tag.FocalLength;
//		if (tag.FocalLengthIn35mmFilm != null) imageTagFile.ImageTag.FocalLengthIn35mmFilm = tag.FocalLengthIn35mmFilm;
//		if (tag.Make != null) imageTagFile.ImageTag.Make = tag.Make;
//		if (tag.Model != null) imageTagFile.ImageTag.Model = tag.Model;
//		if (tag.Creator != null) imageTagFile.ImageTag.Creator = tag.Creator;
	}

	public struct TagsData
	{
		public List<string> Keywords;
		public uint? Rating;
		public DateTime? DateTime;
		public uint Orientation;
		public string Software;
		public double? Latitude;
		public double? Longitude;
		public double? Altitude;
		public double? ExposureTime;
		public double? FNumber;
		public uint? ISOSpeedRatings;
		public double? FocalLength;
		public uint? FocalLengthIn35mmFilm;
		public string Make;
		public string Model;
		public string Creator;
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
			try{
				imageTagFile = TagLib.File.Create(fileName) as TagLib.Image.File;
				if (imageTagFile == null){
					return null;
				}
			}
			catch (Exception /* UnsupportedFormatException */) {

				return null;
			}				

			// comment out comes from earlier version of method ExtractTags(..)
			//			if (imageTagFile.ImageTag != null && imageTagFile.ImageTag.AllTags.Count == 0) {
			imageTagFile.EnsureAvailableTags ();
			//			}			

			return imageTagFile;
		}

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

		public static void ExtractImageTag(string fileName, out TagsData td)
		{
			CombinedImageTag cit = ExtractImageTag (fileName);
			if (cit != null) {
				td = new TagsData ();
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
				td.Rating = cit.Rating;
				td.Software = cit.Software;
			}
		}

		public static CombinedImageTag ExtractImageTag(string fileName)
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

		public static void ChangeValueOfTag(CombinedImageTag imageTag, Tags tag, int newValue)
		{
//			string tagName = tagNames [(int)tag];

			switch (tag) {
			case Tags.Rating:
				imageTag.Rating = (uint)newValue;
				break;
			case Tags.Creator:
				throw new MethodAccessException ("Please use ChangeValueOfTag(CombinedImageTag imageTag, Tags tag, string newValue).");
//				break;
				//			case Tags.Conductor:
				//				imageTag.Conductor = newValue;
				//				break;
				//			case "Copyright":
				//				imageTag.Copyright = newValue;
				//				break;
			default:
				throw new NotImplementedException ();
			}	
		}


		public static void ChangeValueOfTag(CombinedImageTag imageTag, Tags tag, string newValue)
		{
//			string tagName = tagNames [(int)tag];
			
			switch (tag) {
			case Tags.Rating:
				throw new MethodAccessException ("Please use ChangeValueOfTag(CombinedImageTag imageTag, Tags tag, int newValue).");
			case Tags.Creator:
				imageTag.Creator = newValue;
				break;
//			case Tags.Conductor:
//				imageTag.Conductor = newValue;
//				break;
//			case "Copyright":
//				imageTag.Copyright = newValue;
//				break;
			default:
				throw new NotImplementedException ();
			}	
		}

		public static void GetDateTime(string fileName, out DateTime? dateTime)
		{
			CombinedImageTag tag = ExtractImageTag (fileName);
			if (tag == null || tag.DateTime == null) {
				dateTime = null;
				return;
			} else {
				dateTime = tag.DateTime;
			}
		}

		public static void GetImageRating(string fileName, out int rating)
		{
			CombinedImageTag tag = ExtractImageTag (fileName);
			if (tag == null || tag.Rating == null) {
				rating = -1;
				//				dateTime = null;
				return;
			} 
			else {
				rating = (int)tag.Rating;
				//				dateTime = tag.DateTime;
			}
		}

		public static bool SetAndSave_Rating(string fileName, uint? newValue)
		{
			bool success = true;
			TagLib.Image.File imageTagFile = LoadTagFile (fileName);
			CombinedImageTag imageTag = imageTagFile.ImageTag;

			try{
				imageTag.Rating = newValue;
				imageTagFile.Save();
				imageTagFile.Dispose ();
			}
			catch (Exception /* UnsupportedFormatException */ ) {
				success = false;
			}

			return success;
		}

		public static bool SetAndSaveTag(string fileName, Tags tag, string newValue)
		{
			bool success = true;
			TagLib.Image.File imageTagFile = LoadTagFile (fileName);
			CombinedImageTag imageTag = imageTagFile.ImageTag;

			try{
				ChangeValueOfTag (imageTag, tag, newValue);
				imageTagFile.Save();
				imageTagFile.Dispose ();
			}
			catch (Exception /* UnsupportedFormatException */ ) {
				success = false;
			}

			return success;
		}
	}
}

