using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOverlay.Graphics.Primitives
{
    public struct Line : IShape
    {
        public Point Start;
        public Point End;

        public Line(Point start, Point end)
        {
            Start = start;
            End = end;
        }

        public Line(float start_x, float start_y, float end_x, float end_y)
        {
            Start = new Point(start_x, start_y);
            End = new Point(end_x, end_y);
        }

        public Line(int start_x, int start_y, int end_x, int end_y)
        {
            Start = new Point(start_x, start_y);
            End = new Point(end_x, end_y);
        }
    }
}
