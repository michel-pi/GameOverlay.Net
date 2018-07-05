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
    public struct Triangle : IShape
    {
        /// <summary>
        /// a
        /// </summary>
        public Point A;
        /// <summary>
        /// The b
        /// </summary>
        public Point B;
        /// <summary>
        /// The c
        /// </summary>
        public Point C;

        /// <summary>
        /// Initializes a new instance of the <see cref="Triangle"/> struct.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <param name="c">The c.</param>
        public Triangle(Point a, Point b, Point c)
        {
            A = a;
            B = b;
            C = c;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Triangle"/> struct.
        /// </summary>
        /// <param name="a_x">a x.</param>
        /// <param name="a_y">a y.</param>
        /// <param name="b_x">The b x.</param>
        /// <param name="b_y">The b y.</param>
        /// <param name="c_x">The c x.</param>
        /// <param name="c_y">The c y.</param>
        public Triangle(float a_x, float a_y, float b_x, float b_y, float c_x, float c_y)
        {
            A = new Point(a_x, a_y);
            B = new Point(b_x, b_y);
            C = new Point(c_x, c_y);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Triangle"/> struct.
        /// </summary>
        /// <param name="a_x">a x.</param>
        /// <param name="a_y">a y.</param>
        /// <param name="b_X">The b x.</param>
        /// <param name="b_y">The b y.</param>
        /// <param name="c_x">The c x.</param>
        /// <param name="c_y">The c y.</param>
        public Triangle(int a_x, int a_y, int b_X, int b_y, int c_x, int c_y)
        {
            A = new Point(a_x, a_y);
            B = new Point(b_X, b_y);
            C = new Point(c_x, c_y);
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
    }
}
