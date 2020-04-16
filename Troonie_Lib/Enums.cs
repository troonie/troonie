namespace Troonie_Lib
{
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
		/// <summary>Corrupted image exception. </summary>
		ImageIsCorruptedException,
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
