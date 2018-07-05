#if DEBUG

using System;
using System.Diagnostics;

using GameOverlay.Windows;
using GameOverlay.Graphics;
using GameOverlay.Utilities;

namespace GameOverlay
{
    internal class Program
    {
        private static D2DDevice gfx;
        private static D2DFont font;
        private static D2DBrush brush;
        private static D2DBrush backgroundBrush;

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

            DeviceOptions rendererOptions = new DeviceOptions()
            {
                AntiAliasing = true,
                Hwnd = overlay.WindowHandle,
                MeasureFps = true,
                VSync = false
            };

            gfx = new D2DDevice(rendererOptions);

            font = gfx.CreateFont("Arial", 22);
            brush = gfx.CreateBrush(255, 0, 0, 255);
            backgroundBrush = gfx.CreateBrush(0xCC, 0xCC, 0xCC, 80);

            FrameTimer timer = new FrameTimer(60);

            timer.FrameStarting += Timer_OnFrameStart;

            timer.Start();

            Console.ReadLine();

            timer.Stop();

            gfx.Dispose();
            overlay.Dispose();
        }

        private static void Overlay_OnWindowBoundsChanged(int x, int y, int width, int height)
        {
            gfx.Resize(width, height);
        }

        private static void Timer_OnFrameStart(D2DDevice device)
        {
            device.BeginScene();
            device.ClearScene();

            device.DrawTextWithBackground(gfx.FPS.ToString(), 100, 100, font, brush, backgroundBrush);

            device.EndScene();
        }
    }
}

#endif