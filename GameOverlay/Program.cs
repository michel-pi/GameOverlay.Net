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
        private static D2DSolidColorBrush red;
        private static D2DSolidColorBrush green;
        private static D2DSolidColorBrush black;
        private static D2DSolidColorBrush backgroundBrush;

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

            DeviceOptions rendererOptions = new DeviceOptions()
            {
                AntiAliasing = true,
                Hwnd = overlay.WindowHandle,
                MeasureFps = true,
                VSync = false
            };

            gfx = new D2DDevice(rendererOptions);

            font = gfx.CreateFont("Arial", 22);
            red = gfx.CreateSolidColorBrush(255, 0, 0, 255);
            black = gfx.CreateSolidColorBrush(0, 0, 0, 255);
            green = gfx.CreateSolidColorBrush(0, 255, 0, 255);
            backgroundBrush = gfx.CreateSolidColorBrush(0xCC, 0xCC, 0xCC, 80);

            overlay.OnWindowBoundsChanged += Overlay_OnWindowBoundsChanged;

            FrameTimer timer = new FrameTimer(0, gfx);

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

        private static void Timer_OnFrameStart(FrameTimer timer, D2DDevice device)
        {
            device.BeginScene();
            device.ClearScene();

            device.DrawTextWithBackground(device.FPS.ToString(), new Graphics.Primitives.Point(10, 20), font, red, backgroundBrush);

            //device.OutlineFillCircle(new Graphics.Primitives.Circle(200, 200, 100), green, red, 2.0f);
            //device.OutlineLine(new Graphics.Primitives.Line(200, 200, 300, 200), green, red, 16.0f);
            device.DrawDashedRoundedRectangle(new Graphics.Primitives.RoundedRectangle(200, 200, 300, 300, 10), green, 2.0f);

            device.EndScene();
        }
    }
}

#endif