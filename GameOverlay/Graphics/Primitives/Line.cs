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
    public struct Line : IShape
    {
        /// <summary>
        /// The start
        /// </summary>
        public Point Start;
        /// <summary>
        /// The end
        /// </summary>
        public Point End;

        /// <summary>
        /// Initializes a new instance of the <see cref="Line"/> struct.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        public Line(Point start, Point end)
        {
            Start = start;
            End = end;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Line"/> struct.
        /// </summary>
        /// <param name="start_x">The start x.</param>
        /// <param name="start_y">The start y.</param>
        /// <param name="end_x">The end x.</param>
        /// <param name="end_y">The end y.</param>
        public Line(float start_x, float start_y, float end_x, float end_y)
        {
            Start = new Point(start_x, start_y);
            End = new Point(end_x, end_y);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Line"/> struct.
        /// </summary>
        /// <param name="start_x">The start x.</param>
        /// <param name="start_y">The start y.</param>
        /// <param name="end_x">The end x.</param>
        /// <param name="end_y">The end y.</param>
        public Line(int start_x, int start_y, int end_x, int end_y)
        {
            Start = new Point(start_x, start_y);
            End = new Point(end_x, end_y);
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
            return "{Start=" + Start.ToString() + ", End=" + End.ToString() + "}";
        }
    }
}
