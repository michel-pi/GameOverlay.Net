using System;
using System.Threading;
using System.Diagnostics;

using GameOverlay.Drawing;
using GameOverlay.PInvoke;

namespace GameOverlay.Windows
{
    /// <summary>
    /// Represents a StickyWindow which uses a GraphicsWindow sticks to a parent window.
    /// </summary>
    public class StickyWindow : GraphicsWindow
    {
        private Stopwatch _watch;

        /// <summary>
        /// Gets or Sets an IntPtr which is used to identify the parent window.
        /// </summary>
        public IntPtr ParentWindowHandle { get; set; }

        /// <summary>
        /// Gets or sets a Boolean which indicates wether to bypass the need of the windows Topmost flag.
        /// </summary>
        public bool BypassTopmost { get; set; }
        /// <summary>
        /// Gets or sets a Boolean which indicates wether to stick to the parents client area.
        /// </summary>
        public bool AttachToClientArea { get; set; }

        /// <summary>
        /// Initializes a new StickyWindow with a default window position and size.
        /// </summary>
        public StickyWindow() : base()
        {
            X = 0;
            Y = 0;
            Width = 800;
            Height = 600;
        }

        /// <summary>
        /// Initializes a new StickyWindow with the given window position and size.
        /// </summary>
        /// <param name="x">The position of the window on the X-Axis of the desktop.</param>
        /// <param name="y">The position of the window on the Y-Axis of the desktop.</param>
        /// <param name="width">The width of the window.</param>
        /// <param name="height">The height of the window.</param>
        public StickyWindow(int x, int y, int width, int height) : base(x, y, width, height)
        {

        }

        /// <summary>
        /// Initializes a new StickyWindow with the given window position and size and the window handle of the parent window.
        /// </summary>
        /// <param name="x">The position of the window on the X-Axis of the desktop.</param>
        /// <param name="y">The position of the window on the Y-Axis of the desktop.</param>
        /// <param name="width">The width of the window.</param>
        /// <param name="height">The height of the window.</param>
        /// <param name="parentWindow">An IntPtr representing the parent windows handle.</param>
        /// <param name="device">Optionally specify a Graphics device to use.</param>
        public StickyWindow(int x, int y, int width, int height, IntPtr parentWindow, Graphics device = null) : base(x, y, width, height, device)
        {
            if (!User32.IsWindow(parentWindow)) throw new ArgumentOutOfRangeException(nameof(parentWindow));

            ParentWindowHandle = parentWindow;
        }

        /// <summary>
        /// Initializes a new StickyWindow with the ability to stick to a parent window.
        /// </summary>
        /// <param name="parentWindow">An IntPtr representing the parent windows handle.</param>
        /// <param name="device">Optionally specify a Graphics device to use.</param>
        public StickyWindow(IntPtr parentWindow, Graphics device = null) : base(device)
        {
            if (!User32.IsWindow(parentWindow)) throw new ArgumentOutOfRangeException(nameof(parentWindow));

            ParentWindowHandle = parentWindow;
        }

        /// <summary>
        /// Gets called when the timer thread needs to render a new Scene / frame.
        /// </summary>
        /// <param name="graphics">A Graphics surface.</param>
        protected override void OnDrawGraphics(Graphics graphics)
        {
            if (_watch == null)
            {
                _watch = Stopwatch.StartNew();
            }

            if (_watch.ElapsedMilliseconds > 32) // executes 30 times per second
            {
                if (BypassTopmost) PlaceAboveWindow(ParentWindowHandle);
                FitToWindow(ParentWindowHandle, AttachToClientArea);

                _watch.Restart();
            }

            base.OnDrawGraphics(graphics);
        }
    }
}
