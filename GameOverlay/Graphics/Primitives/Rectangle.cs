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
    /// <seealso cref="GameOverlay.Graphics.IShape" />
    public class Rectangle : IShape
    {
        /// <summary>
        /// The left
        /// </summary>
        public float Left;
        /// <summary>
        /// The top
        /// </summary>
        public float Top;
        /// <summary>
        /// The right
        /// </summary>
        public float Right;
        /// <summary>
        /// The bottom
        /// </summary>
        public float Bottom;

        /// <summary>
        /// Initializes a new instance of the <see cref="Rectangle"/> class.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="top">The top.</param>
        /// <param name="right">The right.</param>
        /// <param name="bottom">The bottom.</param>
        public Rectangle(float left, float top, float right, float bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Rectangle"/> class.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="top">The top.</param>
        /// <param name="right">The right.</param>
        /// <param name="bottom">The bottom.</param>
        public Rectangle(int left, int top, int right, int bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
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
        /// Creates the specified x.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns></returns>
        public static Rectangle Create(float x, float y, float width, float height)
        {
            return new Rectangle(x, y, x + width, y + height);
        }

        /// <summary>
        /// Creates the specified x.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns></returns>
        public static Rectangle Create(int x, int y, int width, int height)
        {
            return new Rectangle(x, y, x + width, y + height);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return "{Left=" + Left + ", Top=" + Top + ", Right=" + Right + ", Bottom=" + Bottom + "}";
        }
    }
}
