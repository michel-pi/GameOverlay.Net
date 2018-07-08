using System;
using System.Threading;

using GameOverlay.PInvoke;
using GameOverlay.Utilities;

namespace GameOverlay.Windows
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="GameOverlay.Windows.OverlayWindow" />
    public class StickyOverlayWindow : OverlayWindow
    {
        private static OverlayCreationOptions DefaultOptions = new OverlayCreationOptions()
        {
            BypassTopmost = false,
            Height = 600,
            Width = 800,
            WindowTitle = HelperMethods.GenerateRandomString(5, 11),
            X = 0,
            Y = 0
        };

        private bool _exitServiceThread;
        private Thread _serviceThread;

        /// <summary>
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public delegate void WindowBoundsChanged(int x, int y, int width, int height);

        /// <summary>
        /// Occurs when [on window bounds changed].
        /// </summary>
        public event WindowBoundsChanged OnWindowBoundsChanged;

        /// <summary>
        /// Gets the parent window handle.
        /// </summary>
        /// <value>The parent window handle.</value>
        public IntPtr ParentWindowHandle { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StickyOverlayWindow"/> class.
        /// </summary>
        /// <param name="parentWindowHandle">The parent window handle.</param>
        public StickyOverlayWindow(IntPtr parentWindowHandle) : base(DefaultOptions)
        {
            ParentWindowHandle = parentWindowHandle;
            Install();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StickyOverlayWindow"/> class.
        /// </summary>
        /// <param name="parentWindowHandle">The parent window handle.</param>
        /// <param name="options">The options.</param>
        public StickyOverlayWindow(IntPtr parentWindowHandle, OverlayCreationOptions options) : base(options)
        {
            ParentWindowHandle = parentWindowHandle;
            Install();
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="StickyOverlayWindow"/> class.
        /// </summary>
        ~StickyOverlayWindow()
        {
            Dispose(false);
        }

        /// <summary>
        /// Installs the sticky overlay.
        /// </summary>
        public void Install()
        {
            if (ParentWindowHandle == IntPtr.Zero || _exitServiceThread || _serviceThread != null) return;

            base.ShowWindow();

            _exitServiceThread = false;

            _serviceThread = new Thread(WindowService)
            {
                IsBackground = true,
                Priority = ThreadPriority.BelowNormal
            };

            _serviceThread.Start();
        }

        /// <summary>
        /// Uns the install.
        /// </summary>
        public void UnInstall()
        {
            if (ParentWindowHandle == IntPtr.Zero) return;
            if (_exitServiceThread) return;
            if (_serviceThread == null) return;

            ExitThread();

            base.HideWindow();
        }

        private void ExitThread()
        {
            if (_exitServiceThread) return;
            if (_serviceThread == null) return;

            _exitServiceThread = true;

            try
            {
                _serviceThread.Join();
            }
            catch
            {
            }

            _serviceThread = null;
            _exitServiceThread = false;
        }

        private void WindowService()
        {
            RECT bounds = new RECT();

            while (!_exitServiceThread)
            {
                Thread.Sleep(100);

                if (User32.IsWindowVisible(ParentWindowHandle) == 0)
                {
                    if (base.IsVisible) base.HideWindow();
                    continue;
                }
                else
                {
                    if (!base.IsVisible) base.ShowWindow();
                }

                if (base.BypassTopmost)
                {
                    IntPtr windowAboveParentWindow = User32.GetWindow(ParentWindowHandle, 3 /* GW_HWNDPREV */);

                    if (windowAboveParentWindow != base.WindowHandle)
                    {
                        User32.SetWindowPos(base.WindowHandle, windowAboveParentWindow, 0, 0, 0, 0, 0x10 | 0x2 | 0x1 | 0x4000); // SWP_NOACTIVATE | SWP_NOMOVE | SWP_NOSIZE | SWP_ASYNCWINDOWPOS
                    }
                }

                if (!HelperMethods.GetWindowClientRect(ParentWindowHandle, out bounds)) continue;

                int x = bounds.Left;
                int y = bounds.Top;

                int width = bounds.Right - x;
                int height = bounds.Bottom - y;

                if (base.X == x
                    && base.Y == y
                    && base.Width == width
                    && base.Height == height) continue;

                base.SetWindowBounds(x, y, width, height);

                OnWindowBoundsChanged?.Invoke(x, y, width, height);
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        /// <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
        /// unmanaged resources.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            UnInstall();

            base.Dispose(disposing);
        }
    }
}