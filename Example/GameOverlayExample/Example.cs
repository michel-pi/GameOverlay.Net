using System;

using GameOverlay.Graphics;
using GameOverlay.Graphics.Primitives;
using GameOverlay.Utilities;
using GameOverlay.Windows;

namespace GameOverlayExample
{
    public class Example
    {
        private OverlayWindow _window;
        private D2DDevice _device;
        private FrameTimer _frameTimer;

        private bool _initializeGraphicObjects;

        private D2DColor _backgroundColor;

        private D2DFont _font;

        private D2DSolidColorBrush _blackBrush;

        private D2DSolidColorBrush _redBrush;
        private D2DSolidColorBrush _greenBrush;
        private D2DSolidColorBrush _blueBrush;

        private D2DLinearGradientBrush _gradient;

        private D2DImage _image;

        public Example()
        {
            SetupInstance();
        }

        ~Example()
        {
            //DestroyInstance();
        }

        private void SetupInstance()
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

            _frameTimer = new FrameTimer(_device, 0);

            _window.OnWindowBoundsChanged += _window_OnWindowBoundsChanged;

            _initializeGraphicObjects = true;

            _frameTimer.OnFrameStarting += _frameTimer_OnFrameStarting;
            _frameTimer.OnFrame += _frameTimer_OnFrame;

            _frameTimer.Start();
        }

        public void DestroyInstance()
        {
            _frameTimer.Stop();

            _frameTimer.Dispose();
            _device.Dispose();
            _window.Dispose();

            _window = null;
            _device = null;
            _frameTimer = null;
        }

        private void _window_OnWindowBoundsChanged(int x, int y, int width, int height)
        {
            if (_device == null) return;
            if (!_device.IsInitialized) return;

            _device.Resize(width, height);
        }

        private void _frameTimer_OnFrameStarting(FrameTimer timer, D2DDevice device)
        {
            if (!_initializeGraphicObjects) return;

            if (!device.IsInitialized) return;
            if (device.IsDrawing) return;

            _backgroundColor = new D2DColor(0x24, 0x29, 0x2E, 0xFF);

            _font = _device.CreateFont(new FontOptions()
            {
                Bold = false,
                FontFamilyName = "Arial",
                FontSize = 16,
                Italic = false,
                WordWrapping = true
            });

            // colors automatically normalize values to fit. you can use 1.0f but also 255.0f.
            _blackBrush = device.CreateSolidColorBrush(0x0, 0x0, 0x0, 0xFF);

            _redBrush = device.CreateSolidColorBrush(0xFF, 0x0, 0x0, 0xFF);
            _greenBrush = device.CreateSolidColorBrush(0x0, 0xFF, 0x0, 0xFF);
            _blueBrush = device.CreateSolidColorBrush(0x0, 0x0, 0xFF, 0xFF);

            _gradient = new D2DLinearGradientBrush(device, new D2DColor(0, 0, 80), new D2DColor(0x88, 0, 125), new D2DColor(0, 0, 225));

            // loads an image from resource bytes (.png in this case)
            _image = device.LoadImage(Properties.Resources.placeholder_image_bytes);

            _initializeGraphicObjects = false;
        }

        private void _frameTimer_OnFrame(FrameTimer timer, D2DDevice device)
        {
            // the render loop will call device.BeginScene() and device.EndScene() for us

            if (!device.IsDrawing)
            {
                _initializeGraphicObjects = true;
                return;
            }

            // clear the scene / fill it with our background

            device.ClearScene(_backgroundColor);

            // text

            // the background is dynamically adjusted to the text's size
            device.DrawTextWithBackground("FPS: " + device.FramesPerSecond, 10, 10, _font, _redBrush, _blackBrush);

            // primitives

            device.DrawCircle(100, 100, 50, 2.0f, _redBrush);
            device.DrawDashedCircle(250, 100, 50, 2.0f, _greenBrush);

            // Rectangle.Create offers a method to create rectangles with x, y, width, heigth 
            device.DrawRectangle(Rectangle.Create(350, 50, 100, 100), 2.0f, _blueBrush);
            device.DrawRoundedRectangle(RoundedRectangle.Create(500, 50, 100, 100, 6.0f), 2.0f, _redBrush);

            device.DrawTriangle(650, 150, 750, 150, 700, 50, _greenBrush, 2.0f);

            // lines

            device.DrawLine(50, 175, 750, 175, 2.0f, _blueBrush);
            device.DrawDashedLine(50, 200, 750, 200, 2.0f, _redBrush);

            // outlines & filled

            device.OutlineCircle(100, 275, 50, 4.0f, _redBrush, _blackBrush);
            device.FillCircle(250, 275, 50, _greenBrush);

            device.OutlineRectangle(Rectangle.Create(350, 225, 100, 100), 4.0f, _blueBrush, _blackBrush);

            _gradient.SetRange(500, 225, 600, 325);
            device.FillRoundedRectangle(RoundedRectangle.Create(500, 225, 100, 100, 6.0f), _gradient);
            
            device.FillTriangle(650, 325, 750, 325, 700, 225, _greenBrush);

            // images

            device.DrawImage(_image, 310, 375);
        }
    }
}
