namespace Troonie_Lib
{
    using System;
    using System.Drawing;

    /// <summary>
    /// Struct for three float values (like a color) including overloaded 
    /// operators.
    /// </summary>
    public struct Float3
    {
       /// <summary>X-component.</summary>
        public float X { get; set; }
        /// <summary>Y-component.</summary>
        public float Y { get; set; }
        /// <summary>Z-component.</summary>
        public float Z { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Float3"/> struct.
        /// </summary>
        /// <param name="x">The x component.</param>
        /// <param name="y">The y component.</param>
        /// <param name="z">The z component.</param>
        public Float3(float x, float y, float z)
            : this()
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// Implements the operator +.
        /// </summary>
        /// <param name="p1">The first <see cref="Float3"/>.</param>
        /// <param name="p2">The second <see cref="Float3"/>.</param>
        /// <returns> The Float3 result of the operator. </returns>
        public static Float3 operator +(Float3 p1, Float3 p2)
        {
            return new Float3(p1.X + p2.X, p1.Y + p2.Y, p1.Z + p2.Z);
        }

        /// <summary>
        /// Implements the operator +.
        /// </summary>
        /// <param name="p1">The first <see cref="Float3"/>.</param>
        /// <param name="p2">The second <see cref="float"/>.</param>
        /// <returns> The Float3 result of the operator. </returns>
        public static Float3 operator +(Float3 p1, float p2)
        {
            return new Float3(p1.X + p2, p1.Y + p2, p1.Z + p2);
        }

        /// <summary>
        /// Implements the operator -.
        /// </summary>
        /// <param name="p1">The first <see cref="Float3"/>.</param>
        /// <param name="p2">The second <see cref="Float3"/>.</param>
        /// <returns> The Float3 result of the operator. </returns>
        public static Float3 operator -(Float3 p1, Float3 p2)
        {
            return new Float3(p1.X - p2.X, p1.Y - p2.Y, p1.Z - p2.Z);
        }

        /// <summary>
        /// Implements the operator -.
        /// </summary>
        /// <param name="p1">The first <see cref="Float3"/>.</param>
        /// <param name="p2">The second <see cref="float"/>.</param>
        /// <returns> The Float3 result of the operator. </returns>
        public static Float3 operator -(Float3 p1, float p2)
        {
            return new Float3(p1.X - p2, p1.Y - p2, p1.Z - p2);
        }

        /// <summary> Mulitplication operator. </summary>
        public static Float3 operator *(Float3 p1, float p2)
        {
            return new Float3(p1.X * p2, p1.Y * p2, p1.Z * p2);
        }

        /// <summary> Multiplication operator. </summary>
        public static Float3 operator *(float p2, Float3 p1)
        {
            return new Float3(p1.X * p2, p1.Y * p2, p1.Z * p2);
        }

        /// <summary> Multiplication operator. </summary>
        public static Float3 operator *(Float3 p1, Float3 p2)
        {
            return new Float3(p1.X * p2.X, p1.Y * p2.Y, p1.Z * p2.Z);
        }

        /// <summary> Division operator. </summary>
        public static Float3 operator /(Float3 p1, float p2)
        {
            float x = p1.X / p2;
            float y = p1.Y / p2;
            float z = p1.Z / p2;
            return new Float3(x, y, z);
        }

        /// <summary> Division operator. </summary>
        public static Float3 operator /(Float3 p1, Float3 p2)
        {
            float x = p1.X / p2.X;
            float y = p1.Y / p2.Y;
            float z = p1.Z / p2.Z;
            return new Float3(x, y, z);
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="p1">The first <see cref="Float3"/>.</param>
        /// <param name="p2">The second <see cref="Float3"/>.</param>
        /// <returns> The bool result of the operator. </returns>
        public static bool operator ==(Float3 p1, Float3 p2)
        {
            return p1.Equals(p2);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="p1">The first <see cref="Float3"/>.</param>
        /// <param name="p2">The second <see cref="Float3"/>.</param>
        /// <returns> The bool result of the operator. </returns>
        public static bool operator !=(Float3 p1, Float3 p2)
        {
            return !p1.Equals(p2);
        }

        /// <summary>
        /// Checks, if the specified <paramref name="other"/> is 
        /// equal to this instance.
        /// </summary>
        /// <param name="other">The specified instanct to check with this 
        /// instance.</param>
        /// <returns>True, if the specified <paramref name="other"/> is 
        /// equal to this instance, otherwise false.</returns>
        public bool Equals(Float3 other)
        {
            return Math.Abs(other.X - X) <= 0.00001 &&
                   Math.Abs(other.Y - Y) <= 0.00001 &&
                   Math.Abs(other.Z - Z) <= 0.00001;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            return obj is Float3 && Equals((Float3)obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int result = Math.Round(X, 5).GetHashCode();
                result = Math.Round(result * Y, 5).GetHashCode();
                result = Math.Round(result * Z, 5).GetHashCode();
                return result;
            }
        }

        /// <summary>
        /// Returns the square root of a specified <see cref="Float3"/> number.
        /// </summary>
        /// <param name="p">The specified <see cref="Float3"/> number.</param>
        /// <returns>The square root.</returns>
        public static Float3 Sqrt(Float3 p)
        {
            return new Float3((float)Math.Sqrt(p.X), 
                              (float)Math.Sqrt(p.Y), 
                              (float)Math.Sqrt(p.Z));
        }

        /// <summary>
        /// Returns the square root of a specified <see cref="Int3"/> number.
        /// </summary>
        /// <param name="p">The specified <see cref="Int3"/> number.</param>
        /// <returns>The square root.</returns>
        public static Float3 Sqrt(Int3 p)
        {
            return new Float3((float)Math.Sqrt(p.X),
                              (float)Math.Sqrt(p.Y),
                              (float)Math.Sqrt(p.Z));
        }

        /// <summary>
        /// Returns the X- and Y-component as <see cref="Point"/>.
        /// </summary>
        /// <returns>The X- and Y-component as <see cref="Point"/>.</returns>
        public PointF ToPointF()
        {
            return new PointF(X, Y);
        }
    }
}
