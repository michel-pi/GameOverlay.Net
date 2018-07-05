using System;

namespace GameOverlay.Graphics.Primitives
{
    public struct Point : IShape
    {
        public float X;
        public float Y;

        public Point(float x, float y)
        {
            X = x;
            Y = y;
        }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
