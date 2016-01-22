namespace Picturez_Lib
{
    using System;

    /// <summary>Defines pixel formats and combinations.</summary>
    [Flags]
    public enum PixelFormatFlags
    {
        /// <summary>No supported pixel format.</summary>
        None = 0,

        /// <summary>8 bpp grayscale or indexed color table format.</summary>
        Format8BppIndexed = 1,

        /// <summary>24 bpp rgb color format (8 bit per channel).</summary>
        Format24BppRgb = 2,

        /// <summary>32 bpp argb color format (8 bit per channel).</summary>
        Format32BppArgb = 4,

        /// <summary>
        /// 32 bpp rgb color format (8 bit per channel, alpha will be ignored).
        /// </summary>
        Format32BppRgb = 8,

        /// <summary>Combination of 24 bpp rgb color and 8 bpp 
        /// grayscale/indexed color table  format.</summary>
        Format24And8Bpp = Format24BppRgb | Format8BppIndexed,

        /// <summary>Combination of 32 bpp argb color and 8 bpp 
        /// grayscale/indexed color table  format.</summary>
        Format32And8Bpp = Format32BppArgb | Format8BppIndexed,

        /// <summary>Combination of 24 bpp and 32 bpp argb color format.</summary>
        Color = Format24BppRgb | Format32BppArgb | Format32BppRgb,

        /// <summary>Combination of all pixel formats.</summary>
        All = Format8BppIndexed | Format24BppRgb | Format32BppArgb | Format32BppRgb,
    }
}
