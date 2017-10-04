namespace Troonie_Lib
{
    using System;
    using System.Drawing;

    /// <summary>
    /// Struct for two float values (like a <see cref="Point"/> with float) 
    /// including overloaded operators.
    /// </summary>
    public struct Float2
    {
        /// <summary>X-component.</summary>
        public float X { get; set; }
        /// <summary>Y-component.</summary>
        public float Y { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Float2"/> struct.
        /// </summary>
        /// <param name="x">The x component.</param>
        /// <param name="y">The y component.</param>
        public Float2(float x, float y)
            : this()
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Implements the operator +.
        /// </summary>
        /// <param name="p1">The first <see cref="Float2"/>.</param>
        /// <param name="p2">The second <see cref="Float2"/>.</param>
        /// <returns> The Float2 result of the operator. </returns>
        public static Float2 operator +(Float2 p1, Float2 p2)
        {
            return new Float2(p1.X + p2.X, p1.Y + p2.Y);
        }

        /// <summary>
        /// Implements the operator +.
        /// </summary>
        /// <param name="p1">The first <see cref="Float2"/>.</param>
        /// <param name="p2">The second <see cref="float"/>.</param>
        /// <returns> The Float2 result of the operator. </returns>
        public static Float2 operator +(Float2 p1, float p2)
        {
            return new Float2(p1.X + p2, p1.Y + p2);
        }

        /// <summary>
        /// Implements the operator -.
        /// </summary>
        /// <param name="p1">The first <see cref="Float2"/>.</param>
        /// <param name="p2">The second <see cref="Float2"/>.</param>
        /// <returns> The Float2 result of the operator. </returns>
        public static Float2 operator -(Float2 p1, Float2 p2)
        {
            return new Float2(p1.X - p2.X, p1.Y - p2.Y);
        }

        /// <summary>
        /// Implements the operator -.
        /// </summary>
        /// <param name="p1">The first <see cref="Float2"/>.</param>
        /// <param name="p2">The second <see cref="float"/>.</param>
        /// <returns> The Float2 result of the operator. </returns>
        public static Float2 operator -(Float2 p1, float p2)
        {
            return new Float2(p1.X - p2, p1.Y - p2);
        }

        /// <summary> Multiplication operator. </summary>
        public static Float2 operator *(Float2 p1, float p2)
        {
            return new Float2(p1.X * p2, p1.Y * p2);
        }

        /// <summary> Multiplication operator. </summary>
        public static Float2 operator *(float p2, Float2 p1)
        {
            return new Float2(p1.X * p2, p1.Y * p2);
        }

        /// <summary> Multiplication operator. </summary>
        public static Float2 operator *(Float2 p1, Float2 p2)
        {
            return new Float2(p1.X * p2.X, p1.Y * p2.Y);
        }

        /// <summary> Division operator. </summary>
        public static Float2 operator /(Float2 p1, float p2)
        {
            float x = p1.X / p2;
            float y = p1.Y / p2;
            return new Float2(x, y);
        }

        /// <summary> Division operator. </summary>
        public static Float2 operator /(Float2 p1, Float2 p2)
        {
            float x = p1.X / p2.X;
            float y = p1.Y / p2.Y;
            return new Float2(x, y);
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="p1">The first <see cref="Float2"/>.</param>
        /// <param name="p2">The second <see cref="Float2"/>.</param>
        /// <returns> The bool result of the operator. </returns>
        public static bool operator ==(Float2 p1, Float2 p2)
        {
            return p1.Equals(p2);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="p1">The first <see cref="Float2"/>.</param>
        /// <param name="p2">The second <see cref="Float2"/>.</param>
        /// <returns> The bool result of the operator. </returns>
        public static bool operator !=(Float2 p1, Float2 p2)
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
        public bool Equals(Float2 other)
        {
            return Math.Abs(other.X - X) <= 0.00001 &&
                   Math.Abs(other.Y - Y) <= 0.00001;
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
            return obj is Float2 && Equals((Float2)obj);
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
                return result;
            }
        }

        /// <summary>
        /// Returns the square root of a specified <see cref="Float2"/> number.
        /// </summary>
        /// <param name="p">The specified <see cref="Float2"/> number.</param>
        /// <returns>The square root.</returns>
        public static Float2 Sqrt(Float2 p)
        {
            return new Float2((float)Math.Sqrt(p.X), (float)Math.Sqrt(p.Y));
        }

        /// <summary>
        /// Returns the square root of a specified <see cref="Point"/> number.
        /// </summary>
        /// <param name="p">The specified <see cref="Point"/> number.</param>
        /// <returns>The square root.</returns>
        public static Float2 Sqrt(Point p)
        {
            return new Float2((float)Math.Sqrt(p.X), (float)Math.Sqrt(p.Y));
        }
    }
}
