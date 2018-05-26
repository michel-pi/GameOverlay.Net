#if DEBUG

using System;
using System.Diagnostics;

using Yato.DirectXOverlay.Windows;
using Yato.DirectXOverlay.Renderer;

namespace Yato.DirectXOverlay
{
    internal class Program
    {
        private static Direct2DRenderer gfx;
        private static Direct2DFont font;
        private static Direct2DBrush brush;
        private static Direct2DBrush backgroundBrush;

        private static void Main(string[] args)
        {
            Console.WindowHeight = Console.LargestWindowHeight / 2;
            Console.WindowWidth = Console.LargestWindowWidth / 2;

            OverlayCreationOptions overlayOptions = new OverlayCreationOptions()
            {
                BypassTopmost = true,
                Height = 600,
                Width = 600,
                WindowTitle = HelperMethods.GenerateRandomString(5, 11),
                X = Console.WindowLeft,
                Y = Console.WindowTop
            };

            StickyOverlayWindow overlay = new StickyOverlayWindow(Process.GetCurrentProcess().MainWindowHandle, overlayOptions);

            overlay.OnWindowBoundsChanged += Overlay_OnWindowBoundsChanged;

            RendererOptions rendererOptions = new RendererOptions()
            {
                AntiAliasing = true,
                Hwnd = overlay.WindowHandle,
                MeasureFps = true,
                VSync = false
            };

            gfx = new Direct2DRenderer(rendererOptions);

            font = gfx.CreateFont("Arial", 22);
            brush = gfx.CreateBrush(255, 0, 0, 255);
            backgroundBrush = gfx.CreateBrush(0xCC, 0xCC, 0xCC, 80);

            FrameTimer timer = new FrameTimer(0);

            timer.OnFrameStart += Timer_OnFrameStart;

            timer.Start();

            Console.ReadLine();

            timer.Stop();

            gfx.Dispose();
            overlay.Dispose();
        }

        private static void Overlay_OnWindowBoundsChanged(int x, int y, int width, int height)
        {
            Console.WriteLine($"{x}, {y}, {width}, {height}");
        }

        private static void Timer_OnFrameStart()
        {
            gfx.BeginScene();
            gfx.ClearScene();

            gfx.DrawTextWithBackground(gfx.FPS.ToString(), 100, 100, font, brush, backgroundBrush);

            gfx.EndScene();
        }
    }
}

#endif