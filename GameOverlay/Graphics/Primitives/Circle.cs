namespace GameOverlay.Graphics.Primitives
{
    /// <summary>
    /// </summary>
    public struct Circle
    {
        /// <summary>
        ///     The location
        /// </summary>
        public Point Location;

        /// <summary>
        ///     The radius
        /// </summary>
        public float Radius;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Circle" /> struct.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="radius">The radius.</param>
        public Circle(Point location, float radius)
        {
            Location = location;
            Radius = radius;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Circle" /> struct.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="radius">The radius.</param>
        public Circle(float x, float y, float radius)
        {
            Location = new Point(x, y);
            Radius = radius;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Circle" /> struct.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="radius">The radius.</param>
        public Circle(Point location, int radius)
        {
            Location = location;
            Radius = radius;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Circle" /> struct.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="radius">The radius.</param>
        public Circle(int x, int y, int radius)
        {
            Location = new Point(x, y);
            Radius = radius;
        }

        /// <summary>
        ///     Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        ///     A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return Location + "(" + Radius + ")";
        }

        /// <summary>
        ///     Performs an implicit conversion from <see cref="Circle" /> to <see cref="SharpDX.Direct2D1.Ellipse" />.
        /// </summary>
        /// <param name="circle">The circle.</param>
        /// <returns>
        ///     The result of the conversion.
        /// </returns>
        public static implicit operator SharpDX.Direct2D1.Ellipse(Circle circle)
        {
            return new SharpDX.Direct2D1.Ellipse(circle.Location, circle.Radius, circle.Radius);
        }
    }
}