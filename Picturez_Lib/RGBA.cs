namespace Picturez_Lib
{
    /// <summary>Represents the possible channel order of RGB pixel.</summary>
    public enum RGBOrder
    {
        /// <summary>Red channel.</summary>
        R,
        /// <summary>Green channel.</summary>
        G,
        /// <summary>Blue order.</summary>
        B,
        /// <summary>RGB channel order.</summary>
        RGB,
        /// <summary>RBG channel order.</summary>
        RBG,
        /// <summary>BRG channel order.</summary>
        BRG,
        /// <summary>BGR channel order.</summary>
        BGR,
        /// <summary>GBR channel order.</summary>
        GBR,
        /// <summary>GRB channel order.</summary>
        GRB
    }

    /// <summary>Represents the channel indices of ARGB structure.</summary>
    public struct RGBA
    {
        /// <summary>Color channel <c>blue</c></summary>
        public const byte B = 0;
        /// <summary>Color channel <c>green</c></summary>
        public const byte G = 1;
        /// <summary>Color channel <c>red</c></summary>
        public const byte R = 2;
        /// <summary>Transparence channel <c>alpha</c></summary>
        public const byte A = 3;
    }
}
