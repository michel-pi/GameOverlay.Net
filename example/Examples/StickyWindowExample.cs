using GameOverlay.Drawing;
using GameOverlay.Windows;
using GameOverlayExample.Properties;
using System;
using System.Diagnostics;

namespace GameOverlayExample.Examples
{
    public class StickyWindowExample : IExample
    {
        private readonly GraphicsWindow _window;

        private Font _font;

        private SolidBrush _black;
        private SolidBrush _gray;
        private SolidBrush _red;
        private SolidBrush _green;
        private SolidBrush _blue;

        private Image _image;

        public StickyWindowExample()
        {
            // initialize a new Graphics object
            // GraphicsWindow will do the remaining initialization
            var graphics = new Graphics
            {
                MeasureFPS = true,
                PerPrimitiveAntiAliasing = true,
                TextAntiAliasing = true,
                UseMultiThreadedFactories = false,
                VSync = true,
                WindowHandle = IntPtr.Zero
            };

            // it is important to set the window to visible (and topmost) if you want to see it!
            _window = new StickyWindow(GetConsoleWindowHandle(), graphics)
            {
                IsTopmost = true,
                IsVisible = true,
                FPS = 60,
                X = 0,
                Y = 0,
                Width = 800,
                Height = 600
            };

            _window.SetupGraphics += _window_SetupGraphics;
            _window.DestroyGraphics += _window_DestroyGraphics;
            _window.DrawGraphics += _window_DrawGraphics;
        }

        ~StickyWindowExample()
        {
            // you do not need to dispose the Graphics surface
            _window.Dispose();
        }

        public void Initialize()
        {
            Console.WindowWidth = 110;
            Console.WindowHeight = 35;
        }

        public void Run()
        {
            // creates the window and setups the graphics
            _window.StartThread();
        }

        private void _window_SetupGraphics(object sender, SetupGraphicsEventArgs e)
        {
            var gfx = e.Graphics;

            // creates a simple font with no additional style
            _font = gfx.CreateFont("Arial", 16);

            // colors for brushes will be automatically normalized. 0.0f - 1.0f and 0.0f - 255.0f is accepted!
            _black = gfx.CreateSolidBrush(0, 0, 0);
            _gray = gfx.CreateSolidBrush(0x24, 0x29, 0x2E);

            _red = gfx.CreateSolidBrush(Color.Red); // those are the only pre defined Colors
            _green = gfx.CreateSolidBrush(Color.Green);
            _blue = gfx.CreateSolidBrush(Color.Blue);

            _image = gfx.CreateImage(Resources.image); // loads the image using our image.bytes file in our resources
        }

        private void _window_DrawGraphics(object sender, DrawGraphicsEventArgs e)
        {
            // you do not need to call BeginScene() or EndScene()
            var gfx = e.Graphics;

            gfx.ClearScene(_gray); // set the background of the scene (can be transparent)

            gfx.DrawTextWithBackground(_font, _red, _black, 10, 10, "FPS: " + gfx.FPS);

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
        }

        private void _window_DestroyGraphics(object sender, DestroyGraphicsEventArgs e)
        {
            // you may want to dispose any brushes, fonts or images
        }

        private static IntPtr GetConsoleWindowHandle()
        {
            return Process.GetCurrentProcess().MainWindowHandle;
        }
    }
}
