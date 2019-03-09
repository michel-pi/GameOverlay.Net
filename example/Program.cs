using System;
using GameOverlayExample.Examples;

namespace GameOverlayExample
{
    internal static class Program
    {
        // make sure to have the nuget package or reference installed for GameOverlay.dll

        static void Main(string[] args)
        {
            RunOverlayWindowExample();

            //RunGraphicsWindowExample();

            //RunStickyWindowExample();
        }

        private static void RunOverlayWindowExample()
        {
            var example = new OverlayWindowExample();

            example.Initialize();

            example.Run();
        }

        private static void RunGraphicsWindowExample()
        {
            var example = new GraphicsWindowExample();

            example.Initialize();

            example.Run();
            Console.ReadLine();
        }

        private static void RunStickyWindowExample()
        {
            var example = new StickyWindowExample();

            example.Initialize();

            example.Run();
            Console.ReadLine();
        }
    }
}
