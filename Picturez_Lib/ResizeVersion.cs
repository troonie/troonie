namespace Picturez_Lib
{
    /// <summary> Image resize possibilities of Picturez.</summary>
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
}
