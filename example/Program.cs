using System;

using GameOverlayExample.Examples;

namespace GameOverlayExample
{
    class Program
    {
        // make sure to have the nuget packag or reference installed for GameOverlay.dll

        static void Main(string[] args)
        {
            RunBasicsExample();

            //RunAdvancedExample();
        }

        private static void RunBasicsExample()
        {
            var example = new Basics();

            example.Initialize();

            example.Run();
        }

        public static void RunAdvancedExample()
        {
            var example = new StickyOverlayWindow();

            example.Initialize();

            example.Run();
        }
    }
}
