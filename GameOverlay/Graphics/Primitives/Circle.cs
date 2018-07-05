using System;

namespace GameOverlay.Graphics.Primitives
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="GameOverlay.Graphics.IShape" />
    public struct Circle : IShape
    {
        /// <summary>
        /// The location
        /// </summary>
        public Point Location;

        /// <summary>
        /// The radius
        /// </summary>
        public float Radius;

        /// <summary>
        /// Initializes a new instance of the <see cref="Circle"/> struct.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="radius">The radius.</param>
        public Circle(Point location, float radius)
        {
            Location = location;
            Radius = radius;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Circle"/> struct.
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
        /// Initializes a new instance of the <see cref="Circle"/> struct.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="radius">The radius.</param>
        public Circle(Point location, int radius)
        {
            Location = location;
            Radius = radius;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Circle"/> struct.
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
            return Location.ToString() + "(" + Radius + ")";
        }
    }
}
