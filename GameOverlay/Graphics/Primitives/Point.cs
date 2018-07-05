using System;

namespace GameOverlay.Graphics.Primitives
{
    /// <summary>
    /// 
    /// </summary>
    public struct Point
    {
        /// <summary>
        /// The x
        /// </summary>
        public float X;
        /// <summary>
        /// The y
        /// </summary>
        public float Y;

        /// <summary>
        /// Initializes a new instance of the <see cref="Point"/> struct.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        public Point(float x, float y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Point"/> struct.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return "{X= " + X + ", Y= " + Y + "}";
        }
    }
}
