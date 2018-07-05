using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOverlay.Graphics.Primitives
{
    public struct Triangle : IShape
    {
        public Point A;
        public Point B;
        public Point C;

        public Triangle(Point a, Point b, Point c)
        {
            A = a;
            B = b;
            C = c;
        }

        public Triangle(float a_x, float a_y, float b_x, float b_y, float c_x, float c_y)
        {
            A = new Point(a_x, a_y);
            B = new Point(b_x, b_y);
            C = new Point(c_x, c_y);
        }

        public Triangle(int a_x, int a_y, int b_X, int b_y, int c_x, int c_y)
        {
            A = new Point(a_x, a_y);
            B = new Point(b_X, b_y);
            C = new Point(c_x, c_y);
        }

        public void Draw(D2DDevice device)
        {
            throw new NotImplementedException();
        }
    }
}
