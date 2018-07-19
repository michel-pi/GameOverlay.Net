using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOverlay.Graphics.Primitives
{
    /// <summary>
    /// 
    /// </summary>
    public struct RoundedRectangle
    {
        /// <summary>
        /// The rectangle
        /// </summary>
        public Rectangle Rectangle;

        /// <summary>
        /// The radius x
        /// </summary>
        public float RadiusX;

        /// <summary>
        /// The radius y
        /// </summary>
        public float RadiusY;

        /// <summary>
        /// Initializes a new instance of the <see cref="RoundedRectangle"/> struct.
        /// </summary>
        /// <param name="rectangle">The rectangle.</param>
        /// <param name="radius">The radius.</param>
        public RoundedRectangle(Rectangle rectangle, float radius)
        {
            Rectangle = rectangle;
            RadiusX = radius;
            RadiusY = radius;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoundedRectangle"/> struct.
        /// </summary>
        /// <param name="rectangle">The rectangle.</param>
        /// <param name="radius_x">The radius x.</param>
        /// <param name="radius_y">The radius y.</param>
        public RoundedRectangle(Rectangle rectangle, float radius_x, float radius_y)
        {
            Rectangle = rectangle;
            RadiusX = radius_x;
            RadiusY = radius_y;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoundedRectangle"/> struct.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="top">The top.</param>
        /// <param name="right">The right.</param>
        /// <param name="bottom">The bottom.</param>
        /// <param name="radius">The radius.</param>
        public RoundedRectangle(float left, float top, float right, float bottom, float radius)
        {
            Rectangle = new Rectangle(left, top, right, bottom);
            RadiusX = radius;
            RadiusY = radius;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoundedRectangle"/> struct.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="top">The top.</param>
        /// <param name="right">The right.</param>
        /// <param name="bottom">The bottom.</param>
        /// <param name="radius_x">The radius x.</param>
        /// <param name="radius_y">The radius y.</param>
        public RoundedRectangle(float left, float top, float right, float bottom, float radius_x, float radius_y)
        {
            Rectangle = new Rectangle(left, top, right, bottom);
            RadiusX = radius_x;
            RadiusY = radius_y;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return "{Rectangle= " + Rectangle.ToString() + ", RadiusX= " + RadiusX + ", RadiusY= " + RadiusY + "}";
        }

        /// <summary>
        /// Creates the specified x.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="radius">The radius.</param>
        /// <returns></returns>
        public static RoundedRectangle Create(float x, float y, float width, float height, float radius)
        {
            return new RoundedRectangle(Rectangle.Create(x, y, width, height), radius);
        }

        /// <summary>
        /// Creates the specified x.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="radius_x">The radius x.</param>
        /// <param name="radius_y">The radius y.</param>
        /// <returns></returns>
        public static RoundedRectangle Create(float x, float y, float width, float height, float radius_x, float radius_y)
        {
            return new RoundedRectangle(Rectangle.Create(x, y, width, height), radius_x, radius_y);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="RoundedRectangle"/> to <see cref="SharpDX.Direct2D1.RoundedRectangle"/>.
        /// </summary>
        /// <param name="rectangle">The rectangle.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator SharpDX.Direct2D1.RoundedRectangle(RoundedRectangle rectangle)
        {
            return new SharpDX.Direct2D1.RoundedRectangle()
            {
                RadiusX = rectangle.RadiusX,
                RadiusY = rectangle.RadiusY,
                Rect = rectangle.Rectangle
            };
        }
    }
}
