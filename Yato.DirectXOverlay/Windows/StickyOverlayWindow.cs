using System;
using System.Threading;

using Yato.DirectXOverlay.PInvoke;

namespace Yato.DirectXOverlay.Windows
{
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

        private bool ExitServiceThread;
        private Thread ServiceThread;

        public delegate void WindowBoundsChanged(int x, int y, int width, int height);

        public event WindowBoundsChanged OnWindowBoundsChanged;

        public IntPtr ParentWindowHandle { get; private set; }

        public StickyOverlayWindow(IntPtr parentWindowHandle) : base(DefaultOptions)
        {
            ParentWindowHandle = parentWindowHandle;

            Install(DefaultOptions);
        }

        public StickyOverlayWindow(IntPtr parentWindowHandle, OverlayCreationOptions options) : base(options)
        {
            ParentWindowHandle = parentWindowHandle;

            Install(options);
        }

        ~StickyOverlayWindow()
        {
            Dispose(false);
        }

        public void Install(OverlayCreationOptions options)
        {
            if (ParentWindowHandle == IntPtr.Zero || ExitServiceThread || ServiceThread != null) return;

            base.ShowWindow();

            ExitServiceThread = false;

            ServiceThread = new Thread(WindowService)
            {
                IsBackground = true,
                Priority = ThreadPriority.BelowNormal
            };

            ServiceThread.Start();
        }

        public void UnInstall()
        {
            if (ParentWindowHandle == IntPtr.Zero) return;
            if (ExitServiceThread) return;
            if (ServiceThread == null) return;

            ExitThread();

            base.HideWindow();
        }

        private void ExitThread()
        {
            if (ExitServiceThread) return;
            if (ServiceThread == null) return;

            ExitServiceThread = true;

            try
            {
                ServiceThread.Join();
            }
            catch
            {
            }

            ServiceThread = null;
            ExitServiceThread = false;
        }

        private void WindowService()
        {
            RECT bounds = new RECT();

            while (!ExitServiceThread)
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

                HelperMethods.GetRealWindowRect(ParentWindowHandle, out bounds);

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

        protected override void Dispose(bool disposing)
        {
            UnInstall();

            base.Dispose(disposing);
        }
    }
}