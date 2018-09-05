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
        private static OverlayNotification _notification;
        private static D2DSolidColorBrush _backgroundBrush;
        private static D2DSolidColorBrush _foregroundBrush;
        private static D2DFont _font;

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

            _image = _device.LoadImage(@"C:\Users\Michel\Desktop\NotificationBackOrange.png");

            _backgroundBrush = _device.CreateSolidColorBrush(0xF1, 0x9A, 0x4C, 255);

            _foregroundBrush = _device.CreateSolidColorBrush(0, 0, 0, 255);

            _font = _device.CreateFont(new FontOptions()
            {
                Bold = false,
                FontFamilyName = "Arial",
                FontSize = 14,
                Italic = false,
                WordWrapping = false
            });

            _notification = new OverlayNotification(_device, _backgroundBrush, _foregroundBrush, _font)
            {
                Body = "You earned an achievement!",
                BodySize = 14,
                Header = "Achievement",
                HeaderSize = 18
            };

            _frameTimer = new FrameTimer(_device, 0);

            _window.OnWindowBoundsChanged += _window_OnWindowBoundsChanged;

            _frameTimer.OnFrame += _frameTimer_OnFrame;

            _frameTimer.Start();

            Console.WriteLine("Press enter to exit");

            Console.ReadLine();
        }

        private static void _frameTimer_OnFrame(FrameTimer timer, D2DDevice device)
        {
            _notification.Setup(100, 100, 310, 94);

            device.ClearScene();

            device.DrawShape(_notification);
        }

        private static void _window_OnWindowBoundsChanged(int x, int y, int width, int height)
        {
            if (_device == null) return;
            if (!_device.IsInitialized) return;

            _device.Resize(width, height);
        }
    }
}
