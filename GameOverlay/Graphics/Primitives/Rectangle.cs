using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOverlay.Graphics.Primitives
{
    public class Rectangle : IShape
    {
        public float Left;
        public float Top;
        public float Right;
        public float Bottom;

        public Rectangle(float left, float top, float right, float bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public Rectangle(int left, int top, int right, int bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public static Rectangle Create(float x, float y, float width, float height)
        {
            return new Rectangle(x, y, x + width, y + height);
        }

        public static Rectangle Create(int x, int y, int width, int height)
        {
            return new Rectangle(x, y, x + width, y + height);
        }
    }
}
