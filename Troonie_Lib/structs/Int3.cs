namespace Troonie_Lib
{
    using System.Drawing;

    /// <summary>
    /// Struct for three integer values (like a color) including overloaded 
    /// operators.
    /// </summary>
    public struct Int3
    {
       /// <summary>X-component.</summary>
        public int X { get; set; }
        /// <summary>Y-component.</summary>
        public int Y { get; set; }
        /// <summary>Z-component.</summary>
        public int Z { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Int3"/> struct.
        /// </summary>
        /// <param name="x">The x component.</param>
        /// <param name="y">The y component.</param>
        /// <param name="z">The z component.</param>
        public Int3(int x, int y, int z)
            : this()
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// Implements the operator +.
        /// </summary>
        /// <param name="p1">The first <see cref="Int3"/>.</param>
        /// <param name="p2">The second <see cref="Int3"/>.</param>
        /// <returns> The Int3 result of the operator. </returns>
        public static Int3 operator +(Int3 p1, Int3 p2)
        {
            return new Int3(p1.X + p2.X, p1.Y + p2.Y, p1.Z + p2.Z);
        }

        /// <summary>
        /// Implements the operator -.
        /// </summary>
        /// <param name="p1">The first <see cref="Int3"/>.</param>
        /// <param name="p2">The second <see cref="Int3"/>.</param>
        /// <returns> The Int3 result of the operator. </returns>
        public static Int3 operator -(Int3 p1, Int3 p2)
        {
            return new Int3(p1.X - p2.X, p1.Y - p2.Y, p1.Z - p2.Z);
        }

        /// <summary> Mulitplication operator. </summary>
        public static Int3 operator *(Int3 p1, int p2)
        {
            return new Int3(p1.X * p2, p1.Y * p2, p1.Z * p2);
        }

        /// <summary> Mulitplication operator. </summary>
        public static Int3 operator *(int p2, Int3 p1)
        {
            return new Int3(p1.X * p2, p1.Y * p2, p1.Z * p2);
        }

        /// <summary> Mulitplication operator. </summary>
        public static Int3 operator *(Int3 p1, Int3 p2)
        {
            return new Int3(p1.X * p2.X, p1.Y * p2.Y, p1.Z * p2.Z);
        }

        /// <summary> Division operator. </summary>
        public static Int3 operator /(Int3 p1, int p2)
        {
            int x = (int)(p1.X / (p2 + 0.0f) + 0.5f);
            int y = (int)(p1.Y / (p2 + 0.0f) + 0.5f);
            int z = (int)(p1.Z / (p2 + 0.0f) + 0.5f);
            return new Int3(x, y, z);
        }

        /// <summary> Division operator. </summary>
        public static Int3 operator /(Int3 p1, Int3 p2)
        {
            int x = (int)(p1.X / (p2.X + 0.0f) + 0.5f);
            int y = (int)(p1.Y / (p2.Y + 0.0f) + 0.5f);
            int z = (int)(p1.Z / (p2.Z + 0.0f) + 0.5f);
            return new Int3(x, y, z);
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="p1">The first <see cref="Int3"/>.</param>
        /// <param name="p2">The second <see cref="Int3"/>.</param>
        /// <returns> The bool result of the operator. </returns>
        public static bool operator ==(Int3 p1, Int3 p2)
        {
            return p1.Equals(p2);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="p1">The first <see cref="Int3"/>.</param>
        /// <param name="p2">The second <see cref="Int3"/>.</param>
        /// <returns> The bool result of the operator. </returns>
        public static bool operator !=(Int3 p1, Int3 p2)
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
        public bool Equals(Int3 other)
        {
            return other.X == X &&
                   other.Y == Y &&
                   other.Z == Z;
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
            return obj is Int3 && Equals((Int3)obj);
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
                int result = X.GetHashCode();
                result = (result * Y).GetHashCode();
                result = (result * Z).GetHashCode();
                return result;
            }
        }

        /// <summary>
        /// Returns the X- and Y-component as <see cref="Point"/>.
        /// </summary>
        /// <returns>The X- and Y-component as <see cref="Point"/>.</returns>
        public Point ToPoint()
        {
            return new Point(X, Y);
        }
    }
}
