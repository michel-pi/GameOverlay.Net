using System;

using GameOverlay.Graphics;
using GameOverlay.Graphics.Containers;
using GameOverlay.Graphics.Primitives;

namespace GameOverlay
{
    public static class ShapeTest
    {
        public static void Main(string[] args)
        {
            ConcurrentShapeContainer shapes = new ConcurrentShapeContainer();

            for(int i = 0; i < 100; i++)
            {
                shapes.Add(new Line(i, i, i + 2, i + 2));
            }

            IShapeContainer container = (IShapeContainer)shapes;

            foreach(var shape in container)
            {
                Line line = (Line)shape;

                Console.WriteLine(line.Start.ToString() + " - " + line.End.ToString());
            }

            Console.ReadLine();
        }
    }
}
