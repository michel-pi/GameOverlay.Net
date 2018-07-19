using System;

namespace GameOverlay.Graphics.Primitives
{
    /// <summary>
    /// 
    /// </summary>
    public struct Ellipse
    {
        /// <summary>
        /// The location
        /// </summary>
        public Point Location;

        /// <summary>
        /// The radius x
        /// </summary>
        public float RadiusX;
        /// <summary>
        /// The radius y
        /// </summary>
        public float RadiusY;

        /// <summary>
        /// Initializes a new instance of the <see cref="Ellipse"/> struct.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="radiusX">The radius x.</param>
        /// <param name="radiusY">The radius y.</param>
        public Ellipse(Point location, float radiusX, float radiusY)
        {
            Location = location;
            RadiusX = radiusX;
            RadiusY = radiusY;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Ellipse"/> struct.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="radiusX">The radius x.</param>
        /// <param name="radiusY">The radius y.</param>
        public Ellipse(Point location, int radiusX, int radiusY)
        {
            Location = location;
            RadiusX = radiusX;
            RadiusY = radiusY;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Ellipse"/> struct.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="radiusX">The radius x.</param>
        /// <param name="radiusY">The radius y.</param>
        public Ellipse(float x, float y, float radiusX, float radiusY)
        {
            Location = new Point(x, y);
            RadiusX = radiusX;
            RadiusY = radiusY;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Ellipse"/> struct.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="radiusX">The radius x.</param>
        /// <param name="radiusY">The radius y.</param>
        public Ellipse(int x, int y, int radiusX, int radiusY)
        {
            Location = new Point(x, y);
            RadiusX = radiusX;
            RadiusY = radiusY;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return Location.ToString() + "(" + RadiusX + ", " + RadiusY + ")";
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Ellipse"/> to <see cref="SharpDX.Direct2D1.Ellipse"/>.
        /// </summary>
        /// <param name="ellipse">The ellipse.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator SharpDX.Direct2D1.Ellipse(Ellipse ellipse)
        {
            return new SharpDX.Direct2D1.Ellipse(ellipse.Location, ellipse.RadiusX, ellipse.RadiusY);
        }
    }
}
