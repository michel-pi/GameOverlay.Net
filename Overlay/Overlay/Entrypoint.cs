using System;
using System.Threading;
using System.Drawing;

namespace Overlay
{
    public static class Entrypoint
    {
        public static void Main(string[] args)
        {
            OverlayWindow overlay = new OverlayWindow(false);/*, new IntPtr(0x40B3A));*/

            var gfx = overlay.Graphics;

            var color = Color.FromArgb(80, 255, 0, 0);

            while(true)
            {
                gfx.BeginScene();
                gfx.ClearScene();

                gfx.DrawRectangle(0, 0, 100, 100, 2, color);

                gfx.FillRectangle(100, 100, 100, 100, color);

                gfx.EndScene();

                Thread.Sleep(50);
            }

            Console.ReadLine();
        }
    }
}
