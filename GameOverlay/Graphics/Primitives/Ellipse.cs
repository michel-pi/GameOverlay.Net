using System;

namespace GameOverlay.Graphics.Primitives
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="GameOverlay.Graphics.IShape" />
    public struct Ellipse : IShape
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
        /// Draws the specified device.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <exception cref="NotImplementedException"></exception>
        public void Draw(D2DDevice device)
        {
            throw new NotImplementedException();
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
    }
}
