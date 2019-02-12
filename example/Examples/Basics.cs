using System;

using GameOverlay.Drawing;
using GameOverlay.Windows;

namespace GameOverlayExample.Examples
{
    public class Basics : IExample
    {
        private OverlayWindow _window;
        private Graphics _graphics;

        private Font _font;

        private SolidBrush _black;
        private SolidBrush _gray;
        private SolidBrush _red;
        private SolidBrush _green;
        private SolidBrush _blue;

        private Image _image;

        public Basics()
        {
            // it is important to set the window to visible (and topmost) if you want to see it!
            _window = new OverlayWindow(0, 0, 800, 600)
            {
                IsTopmost = true,
                IsVisible = true
            };

            // handle this event to resize your Graphics surface
            _window.SizeChanged += _window_SizeChanged;

            // initialize a new Graphics object
            // set everything before you call _graphics.Setup()
            _graphics = new Graphics()
            {
                MeasureFPS = true,
                Height = _window.Height,
                PerPrimitiveAntiAliasing = true,
                TextAntiAliasing = true,
                UseMultiThreadedFactories = false,
                VSync = true,
                Width = _window.Width,
                WindowHandle = IntPtr.Zero
            };
        }
        
        ~Basics()
        {
            // dont forget to free resources
            _graphics.Dispose();
            _window.Dispose();
        }

        public void Initialize()
        {
            // creates the window using the settings we applied to it in the constructor
            _window.CreateWindow();

            _graphics.WindowHandle = _window.Handle; // set the target handle before calling Setup()
            _graphics.Setup();

            // creates a simple font with no additional style
            _font = _graphics.CreateFont("Arial", 16);

            // colors for brushes will be automatically normalized. 0.0f - 1.0f and 0.0f - 255.0f is accepted!
            _black = _graphics.CreateSolidBrush(0, 0, 0);
            _gray = _graphics.CreateSolidBrush(0x24, 0x29, 0x2E);

            _red = _graphics.CreateSolidBrush(Color.Red); // those are the only pre defined Colors
            _green = _graphics.CreateSolidBrush(Color.Green);
            _blue = _graphics.CreateSolidBrush(Color.Blue);

            _image = _graphics.CreateImage(Properties.Resources.image); // loads the image using our image.bytes file in our resources
        }

        public void Run()
        {
            var gfx = _graphics; // little shortcut

            while (true)
            {
                gfx.BeginScene(); // call before you start any drawing
                gfx.ClearScene(_gray); // set the background of the scene (can be transparent)

                gfx.DrawTextWithBackground(_font, _red, _black, 10, 10, "FPS: " + gfx.FPS.ToString());

                gfx.DrawCircle(_red, 100, 100, 50, 2);
                gfx.DashedCircle(_green, 250, 100, 50, 2);

                // Rectangle.Create takes x, y, width, height instead of left top, right, bottom
                gfx.DrawRectangle(_blue, Rectangle.Create(350, 50, 100, 100), 2);
                gfx.DrawRoundedRectangle(_red, RoundedRectangle.Create(500, 50, 100, 100, 6), 2);

                gfx.DrawTriangle(_green, 650, 150, 750, 150, 700, 50, 2);

                gfx.DrawLine(_blue, 50, 175, 750, 175, 2);
                gfx.DashedLine(_red, 50, 200, 750, 200, 2);

                gfx.OutlineCircle(_black, _red, 100, 275, 50, 4);
                gfx.FillCircle(_green, 250, 275, 50);

                // parameters will always stay in this order: outline color, inner color, position, stroke
                gfx.OutlineRectangle(_black, _blue, Rectangle.Create(350, 225, 100, 100), 4);
                gfx.FillRoundedRectangle(_red, RoundedRectangle.Create(500, 225, 100, 100, 6));

                gfx.FillTriangle(_green, 650, 325, 750, 325, 700, 225);

                // you could also scale the image on the fly
                gfx.DrawImage(_image, 310, 375);

                gfx.EndScene();
            }
        }

        private void _window_SizeChanged(object sender, OverlaySizeEventArgs e)
        {
            if (_graphics == null) return;

            if (_graphics.IsInitialized)
            {
                // after the Graphics surface is initialized you can only use the Resize method in order to enqueue a size change
                _graphics.Resize(e.Width, e.Height);
            }
            else
            {
                // otherwise just set its members
                _graphics.Width = e.Width;
                _graphics.Height = e.Height;
            }
        }
    }
}
