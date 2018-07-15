#if DEBUG

using System;
using System.Diagnostics;
using System.Threading.Tasks;

using GameOverlay.Windows;
using GameOverlay.Graphics;
using GameOverlay.Graphics.Primitives;
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
        private static Geometry skeleton;
        private static Geometry radar;

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

            skeleton = CreateSkeleton(gfx);
            radar = CreateRadarBackground(gfx, new Rectangle(100, 100, 400, 400), 10.0f);

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

            device.DrawTextWithBackground(device.FPS.ToString(), new Point(10, 20), font, red, backgroundBrush);

            Parallel.For(0, 100, (int i) =>
            {
                device.DrawRectangle(new Rectangle(100, 100, 400, 400), 1.0f, green);
            });

            //for (int i = 0; i < 1000; i++)
            //{
            //    device.DrawRectangle(new Rectangle(100, 100, 400, 400), 1.0f, green);
            //}

            //Parallel.For(0, 100, (int i) =>
            //{
            //    DrawRadarBackground(device, new Rectangle(100, 100, 400, 400), 10.0f);
            //});

            //for (int i = 0; i < 100; i++)
            //{
            //    DrawRadarBackground(device, new Rectangle(100, 100, 400, 400), 10.0f);
            //}

            device.EndScene();
        }

        private static void DrawRadarBackground(D2DDevice device, Rectangle bounds, float size)
        {
            float width = bounds.Right - bounds.Left;

            int steps = (int)(width / size);

            for(int i = 0; i < steps + 1; i++)
            {
                float curHeight = bounds.Top + (i * size);
                float curWidth = bounds.Left + (i * size);
                device.DrawLine(new Line(bounds.Left, curHeight, bounds.Right, curHeight), 1.0f, green);
                device.DrawLine(new Line(curWidth, bounds.Top, curWidth, bounds.Bottom), 1.0f, green);
            }
        }

        private static Geometry CreateRadarBackground(D2DDevice device, Rectangle bounds, float size)
        {
            var geometry = device.CreateGeometry();

            float width = bounds.Right - bounds.Left;

            int steps = (int)(width / size);

            for (int i = 0; i < steps + 1; i++)
            {
                float curHeight = bounds.Top + (i * size);
                float curWidth = bounds.Left + (i * size);

                geometry.BeginFigure(new Point(bounds.Left, curHeight));
                geometry.AddPoint(new Point(bounds.Right, curHeight));
                geometry.EndFigure(false);

                geometry.BeginFigure(new Point(curWidth, bounds.Top));
                geometry.AddPoint(new Point(curWidth, bounds.Bottom));
                geometry.EndFigure(false);
            }

            geometry.Close();

            return geometry;
        }

        private static Geometry CreateSkeleton(D2DDevice device)
        {
            var geometry = device.CreateGeometry();

            geometry.BeginFigure(new Graphics.Primitives.Point(170, 270));

            geometry.AddPoint(new Graphics.Primitives.Point(200, 200));
            geometry.AddPoint(new Graphics.Primitives.Point(200, 300));
            geometry.AddPoint(new Graphics.Primitives.Point(150, 350));

            geometry.EndFigure(false);

            geometry.BeginFigure(new Graphics.Primitives.Point(200, 200));

            geometry.AddPoint(new Graphics.Primitives.Point(230, 270));

            geometry.EndFigure(false);

            geometry.BeginFigure(new Graphics.Primitives.Point(200, 300));

            geometry.AddPoint(new Graphics.Primitives.Point(250, 350));

            geometry.EndFigure(false);

            geometry.Close();

            return geometry;
        }
    }
}

#endif