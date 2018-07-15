#if DEBUG

using System;

using GameOverlay.Graphics;
using GameOverlay.Graphics.Primitives;

using GameOverlay.Windows;

namespace GameOverlay
{
    class ColorTest
    {
        public static void Main(string[] args)
        {
            OverlayWindow window = new OverlayWindow(new OverlayCreationOptions()
            {
                BypassTopmost = false,
                Height = 1080,
                Width = 1920,
                WindowTitle = "test",
                X = 0,
                Y = 0
            });

            while(!window.IsInitialized)
            {

            }

            D2DDevice device = new D2DDevice(new DeviceOptions()
            {
                AntiAliasing = true,
                Hwnd = window.WindowHandle,
                MeasureFps = false,
                VSync = false
            });

            var red = device.CreateSolidColorBrush(1.0f, 0.0f, 0.0f);

            device.BeginScene();
            device.ClearScene();

            device.DrawLine(100, 100, 800, 800, 1.0f, red);

            device.EndScene();

            Console.ReadLine();
        }
    }
}

#endif