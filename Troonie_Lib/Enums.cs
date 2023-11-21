using System;

namespace Troonie_Lib
{
    [Flags]
    public enum TagsFlag : long
    {
        None = 0,
        #region 16+1 image tags
        Altitude = 1 << 0, // = 1,
        Creator = 1 << 1, // = 2,
        CreateDate = 1 << 2, // = 4,
        OffsetTime = 1 << 3,// = 8,
        ExposureTime = 1 << 4, // = 16,
        FNumber = 1 << 5, // = 32,
        FocalLength = 1 << 6, // = 64,
        FocalLengthIn35mmFilm = 1 << 7, // = 128,
        ISOSpeedRatings = 1 << 8, // = 256,
        Keywords = 1 << 9, // = 512,
        Latitude = 1 << 10, // = 1024,
        Longitude = 1 << 11, // = 2048,
        Make = 1 << 12, // = 4096,
        Model = 1 << 13, // = 8192,
        Orientation = 1 << 14, // = 16384,
        Rating = 1 << 15, // = 32768,
        Software = 1 << 16, // = 65536,

        Flash = 1 << 17, // = 131072,
        #endregion

        #region other tags			
        Comment = 1 << 18, // = 262144,
        Copyright = 1 << 19, // = 524288, 
        Title = 1 << 20, 
        MeteringMode = 1 << 21,

        IsDaylightSavingTime = 1 << 22, // summer time
        Width = 1 << 23,
        Height = 1 << 24,
        Pixelformat = 1 << 25,
        FileSize = 1 << 26,
        #endregion

        #region Hidden tags --> DO NOT forget to set HiddenTagsNumber.Count
        MediaCreateDate = 1 << 27,
        TrackCreateDate = 1 << 28,
        ModifyDate = 1 << 29,
        MediaModifyDate = 1 << 30,
        TrackModifyDate = 1 << 31,
        DateTimeOriginal = (long)1 << 32,        

        AllCreateAndModifyDates = CreateDate | MediaCreateDate | TrackCreateDate | 
								  ModifyDate | MediaModifyDate |TrackModifyDate |
								  DateTimeOriginal | OffsetTime
        #endregion
    }

	public class HiddenTagsNumber { public const int Count = 7; }

    /// <summary> Image resize possibilities of Troonie.</summary>
    public enum ResizeVersion
    {
        /// <summary> No Resizing.</summary>
        No,
        /// <summary>
        /// Biggest length will be set, smaller length calculated by ratio.
        /// </summary>
        BiggestLength,
        /// <summary> New width and height will be set. </summary>
        FixedSize
    }

	public enum ExceptionType
	{
		/// <summary> No exception. </summary>
		NoException,
		/// <summary>Corrupted image by using System.Drawing.Image exception. </summary>
		SystemDrawing_ImageIsCorruptedException,
		/// <summary>Corrupted image by using cjpeg/djpeg exception. </summary>
		CJpeg_ImageIsCorruptedException,
		/// <summary>Corrupted image by using XMP exif tagging exception. </summary>
		XMP_ImageIsCorruptedException,
		/// <summary> System.UnauthorizedAccessException. </summary>
		UnauthorizedAccessException,
		/// <summary> System.IO.IOException. </summary>
		IO_IOException,
		/// <summary> System.Exception. </summary>
		Exception
	}


	/// <summary>
	/// Describes the orientation of an image.
	/// Values are viewed in terms of rows and columns.
	/// </summary>
	/// <remarks>Source code getting from 
	/// https://github.com/mono/taglib-sharp/blob/master/src/TagLib/Image/ImageOrientation.cs</remarks>
	public enum Orientation : uint
	{
		/// <summary>
		/// No value is known.
		/// </summary>
		None = 0,

		/// <summary>
		/// No need to do any transformations.
		/// </summary>
		TopLeft = 1,

		/// <summary>
		/// Mirror image vertically.
		/// </summary>
		TopRight = 2,

		/// <summary>
		/// Rotate image 180 degrees.
		/// </summary>
		BottomRight = 3,

		/// <summary>
		/// Mirror image horizontally
		/// </summary>
		BottomLeft = 4,

		/// <summary>
		/// Mirror image horizontally and rotate 90 degrees clockwise.
		/// </summary>
		LeftTop = 5,

		/// <summary>
		/// Rotate image 90 degrees clockwise.
		/// </summary>
		RightTop = 6,

		/// <summary>
		/// Mirror image vertically and rotate 90 degrees clockwise.
		/// </summary>
		RightBottom = 7,

		/// <summary>
		/// Rotate image 270 degrees clockwise.
		/// </summary>
		LeftBottom = 8
	}

//	public enum ImageOrientation : uint
//	{
//		None,
//		TopLeft,
//		TopRight,
//		BottomRight,
//		BottomLeft,
//		LeftTop,
//		RightTop, // portrait
//		RightBottom,
//		LeftBottom
//	}
}
