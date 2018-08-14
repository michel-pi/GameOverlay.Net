using SharpDX.Mathematics.Interop;

namespace GameOverlay.Graphics.Primitives
{
    /// <summary>
    /// </summary>
    public struct Point
    {
        /// <summary>
        ///     The x
        /// </summary>
        public float X;

        /// <summary>
        ///     The y
        /// </summary>
        public float Y;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Point" /> struct.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        public Point(float x, float y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Point" /> struct.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        ///     Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        ///     A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return "{X=" + X + ", Y=" + Y + "}";
        }

        /// <summary>
        ///     Performs an implicit conversion from <see cref="Point" /> to <see cref="SharpDX.Mathematics.Interop.RawVector2" />.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>
        ///     The result of the conversion.
        /// </returns>
        public static implicit operator RawVector2(Point point)
        {
            return new RawVector2(point.X, point.Y);
        }

        public static implicit operator Point(RawVector2 vector)
        {
            return new Point(vector.X, vector.Y);
        }
    }
}