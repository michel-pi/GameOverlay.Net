using System;

namespace GameOverlay.Graphics.Primitives
{
    public struct Ellipse : IShape
    {
        public Point Location;

        public float RadiusX;
        public float RadiusY;

        public Ellipse(Point location, float radiusX, float radiusY)
        {
            Location = location;
            RadiusX = radiusX;
            RadiusY = radiusY;
        }

        public Ellipse(Point location, int radiusX, int radiusY)
        {
            Location = location;
            RadiusX = radiusX;
            RadiusY = radiusY;
        }

        public Ellipse(float x, float y, float radiusX, float radiusY)
        {
            Location = new Point(x, y);
            RadiusX = radiusX;
            RadiusY = radiusY;
        }

        public Ellipse(int x, int y, int radiusX, int radiusY)
        {
            Location = new Point(x, y);
            RadiusX = radiusX;
            RadiusY = radiusY;
        }
    }
}
