using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GameOverlay.Graphics;
using GameOverlay.Utilities;
using GameOverlay.Windows;

namespace GameOverlayTest
{
    class Program
    {
        private static OverlayWindow _window;
        private static D2DDevice _device;
        private static FrameTimer _frameTimer;
        private static D2DImage _image;

        static void Main(string[] args)
        {
            _window = new OverlayWindow(new OverlayOptions()
            {
                BypassTopmost = false,
                Height = 600,
                Width = 800,
                WindowTitle = "GameOverlayExample",
                X = 0,
                Y = 0
            });

            _device = new D2DDevice(new DeviceOptions()
            {
                AntiAliasing = true,
                Hwnd = _window.WindowHandle,
                MeasureFps = true,
                MultiThreaded = false,
                VSync = false
            });

            _image = _device.LoadImage(@"C:\Users\Michel\Desktop\alpha.png");

            _frameTimer = new FrameTimer(_device, 0);

            _window.OnWindowBoundsChanged += _window_OnWindowBoundsChanged;

            _frameTimer.OnFrame += _frameTimer_OnFrame;

            _frameTimer.Start();

            Console.WriteLine("Press enter to exit");

            Console.ReadLine();
        }

        private static void _frameTimer_OnFrame(FrameTimer timer, D2DDevice device)
        {
            device.ClearScene();

            device.DrawImage(_image, 100, 100, 100, 100);
        }

        private static void _window_OnWindowBoundsChanged(int x, int y, int width, int height)
        {
            if (_device == null) return;
            if (!_device.IsInitialized) return;

            _device.Resize(width, height);
        }
    }
}
