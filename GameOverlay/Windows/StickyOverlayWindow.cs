using System;
using System.Threading;
using GameOverlay.PInvoke;
using GameOverlay.PInvoke.Libraries;
using GameOverlay.PInvoke.Types;
using GameOverlay.Utilities;

namespace GameOverlay.Windows
{
    /// <inheritdoc />
    /// <summary>
    /// </summary>
    /// <seealso cref="T:GameOverlay.Windows.OverlayWindow" />
    public class StickyOverlayWindow : OverlayWindow
    {
        private static readonly OverlayCreationOptions DefaultOptions = new OverlayCreationOptions
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

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:GameOverlay.Windows.StickyOverlayWindow" /> class.
        /// </summary>
        /// <param name="parentWindowHandle">The parent window handle.</param>
        public StickyOverlayWindow(IntPtr parentWindowHandle) : base(DefaultOptions)
        {
            if (parentWindowHandle == IntPtr.Zero) throw new ArgumentNullException(nameof(parentWindowHandle));

            while (!IsInitialized) Thread.Sleep(10);

            ParentWindowHandle = parentWindowHandle;
            Install();
        }

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:GameOverlay.Windows.StickyOverlayWindow" /> class.
        /// </summary>
        /// <param name="parentWindowHandle">The parent window handle.</param>
        /// <param name="options">The options.</param>
        public StickyOverlayWindow(IntPtr parentWindowHandle, OverlayCreationOptions options) : base(options)
        {
            if (parentWindowHandle == IntPtr.Zero) throw new ArgumentNullException(nameof(parentWindowHandle));

            while (!IsInitialized) Thread.Sleep(10);

            ParentWindowHandle = parentWindowHandle;
            Install();
        }

        /// <summary>
        ///     Gets the parent window handle.
        /// </summary>
        /// <value>The parent window handle.</value>
        public IntPtr ParentWindowHandle { get; }

        /// <inheritdoc />
        /// <summary>
        ///     Finalizes an instance of the <see cref="T:GameOverlay.Windows.StickyOverlayWindow" /> class.
        /// </summary>
        ~StickyOverlayWindow()
        {
            Dispose(false);
        }

        /// <summary>
        ///     Installs the sticky overlay.
        /// </summary>
        public void Install()
        {
            if (_exitServiceThread || _serviceThread != null) return;

            ShowWindow();

            _exitServiceThread = false;

            _serviceThread = new Thread(WindowService)
            {
                IsBackground = true,
                Priority = ThreadPriority.BelowNormal
            };

            _serviceThread.Start();
        }

        /// <summary>
        ///     Uns the install.
        /// </summary>
        public void UnInstall()
        {
            if (ParentWindowHandle == IntPtr.Zero) return;
            if (_exitServiceThread) return;
            if (_serviceThread == null) return;

            ExitThread();

            HideWindow();
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
                // ignored
            }

            _serviceThread = null;
            _exitServiceThread = false;
        }

        private void WindowService()
        {
            while (!_exitServiceThread)
            {
                Thread.Sleep(100);

                if (User32.IsWindowVisible(ParentWindowHandle) == 0)
                {
                    if (IsVisible) HideWindow();
                    continue;
                }
                if (!IsVisible) ShowWindow();

                if (BypassTopmost)
                {
                    var windowAboveParentWindow = User32.GetWindow(ParentWindowHandle, 3 /* GW_HWNDPREV */);

                    if (windowAboveParentWindow != WindowHandle)
                        User32.SetWindowPos(WindowHandle, windowAboveParentWindow, 0, 0, 0, 0,
                            0x10 | 0x2 | 0x1 | 0x4000); // SWP_NOACTIVATE | SWP_NOMOVE | SWP_NOSIZE | SWP_ASYNCWINDOWPOS
                }

                if (!HelperMethods.GetWindowClientRect(ParentWindowHandle, out var bounds)) continue;

                int x = bounds.Left;
                int y = bounds.Top;

                int width = bounds.Right - x;
                int height = bounds.Bottom - y;

                if (X == x
                    && Y == y
                    && Width == width
                    && Height == height) continue;

                SetWindowBounds(x, y, width, height);
            }
        }

        /// <inheritdoc />
        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
        ///     unmanaged resources.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            UnInstall();

            base.Dispose(disposing);
        }
    }
}