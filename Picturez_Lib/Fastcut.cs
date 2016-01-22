namespace Picturez_Lib
{
    using System;
    using System.Xml.Serialization;

    /// <summary> Data container for configure Picturez. </summary>
    public class Fastcut : ICloneable
    {
        #region public properties 

        /// <summary>The bottom value. </summary>
        public int Bottom { get; set; }
        /// <summary>The left value. </summary>
        public int Left { get; set; }
        /// <summary>The name of the configuration.</summary>
        [XmlAttribute("Name", DataType = "string")]
        public string Name { get; set; }
        /// <summary>The right value. </summary>
        public int Right { get; set; }
        /// <summary>The rotation value. </summary>
        public int Rotation { get; set; }
        /// <summary>The top value. </summary>
        public int Top { get; set; }

        #endregion public properties

        /// <summary>
        /// Initializes a new instance of the <see cref="Fastcut"/> class.
        /// </summary>
        public Fastcut()
        {
            Name = "";
        }

        /// <summary> Makes a deep copy of this instance. </summary>
        /// <returns>A cloned <see cref="Fastcut"/> of this instance.
        /// </returns>
        public object Clone()
        {
            Fastcut c = new Fastcut();
            c.Left = Left;
            c.Right = Right;
            c.Rotation = Rotation;
            c.Bottom = Bottom;
            c.Top = Top;
            c.Name = Name;
            return c;
        }
}
}
