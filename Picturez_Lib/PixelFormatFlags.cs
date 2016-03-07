namespace Picturez_Lib
{
    using System;

    /// <summary>Defines pixel formats and combinations.</summary>
    [Flags]
    public enum PixelFormatFlags
    {
        /// <summary>No supported pixel format.</summary>
        None = 0,

		/// <summary>1 bpp monochrome black-white format.</summary>
		Format1bppIndexed = 1,

        /// <summary>8 bpp grayscale or indexed color table format.</summary>
        Format8BppIndexed = 2,

        /// <summary>24 bpp rgb color format (8 bit per channel).</summary>
        Format24BppRgb = 4,

        /// <summary>32 bpp argb color format (8 bit per channel).</summary>
        Format32BppArgb = 8,

        /// <summary>
        /// 32 bpp rgb color format (8 bit per channel, alpha will be ignored).
        /// </summary>
        Format32BppRgb = 16,

		/// <summary>[Usage only for destination images.] Defines same pixel format like source image.</summary>
		SameLikeSource = 32,

        /// <summary>Combination of 24 bpp rgb color and 8 bpp 
        /// grayscale/indexed color table  format.</summary>
        Format24And8Bpp = Format24BppRgb | Format8BppIndexed,

        /// <summary>Combination of 32 bpp argb color and 8 bpp 
        /// grayscale/indexed color table  format.</summary>
        Format32And8Bpp = Format32BppArgb | Format8BppIndexed,

        /// <summary>Combination of 24 bpp and 32 bpp argb color format.</summary>
        Color = Format24BppRgb | Format32BppArgb | Format32BppRgb,

		/// <summary>Combination of 32 bpp rgb and 32 bpp argb color format.</summary>
		Format32All = Format32BppArgb | Format32BppRgb,

        /// <summary>Combination of all pixel formats.</summary>
        All = Format8BppIndexed | Format24BppRgb | Format32BppArgb | Format32BppRgb,
    }
}
