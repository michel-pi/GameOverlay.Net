using System;

namespace GameOverlay.Graphics.Primitives
{
    /// <summary>
    /// </summary>
    public struct Line
    {
        /// <summary>
        ///     The start
        /// </summary>
        public Point Start;

        /// <summary>
        ///     The end
        /// </summary>
        public Point End;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Line" /> struct.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        public Line(Point start, Point end)
        {
            Start = start;
            End = end;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Line" /> struct.
        /// </summary>
        /// <param name="startX">The start x.</param>
        /// <param name="startY">The start y.</param>
        /// <param name="endX">The end x.</param>
        /// <param name="endY">The end y.</param>
        public Line(float startX, float startY, float endX, float endY)
        {
            Start = new Point(startX, startY);
            End = new Point(endX, endY);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Line" /> struct.
        /// </summary>
        /// <param name="startX">The start x.</param>
        /// <param name="startY">The start y.</param>
        /// <param name="endX">The end x.</param>
        /// <param name="endY">The end y.</param>
        public Line(int startX, int startY, int endX, int endY)
        {
            Start = new Point(startX, startY);
            End = new Point(endX, endY);
        }

        /// <summary>
        ///     Draws the specified device.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <exception cref="NotImplementedException"></exception>
        public void Draw(D2DDevice device)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        ///     A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return "{Start=" + Start + ", End=" + End + "}";
        }
    }
}