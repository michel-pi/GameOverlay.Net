using System;

namespace GameOverlay.Graphics.Primitives
{
    public struct Circle : IShape
    {
        public Point Location;

        public float Radius;

        public Circle(Point location, float radius)
        {
            Location = location;
            Radius = radius;
        }

        public Circle(float x, float y, float radius)
        {
            Location = new Point(x, y);
            Radius = radius;
        }

        public Circle(Point location, int radius)
        {
            Location = location;
            Radius = radius;
        }

        public Circle(int x, int y, int radius)
        {
            Location = new Point(x, y);
            Radius = radius;
        }
    }
}
