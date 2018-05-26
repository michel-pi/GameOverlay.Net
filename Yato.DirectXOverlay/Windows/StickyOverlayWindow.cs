using System;
using System.Threading;

using Yato.DirectXOverlay.PInvoke;

namespace Yato.DirectXOverlay.Windows
{
    public class StickyOverlayWindow : IDisposable
    {
        private bool ExitServiceThread;
        private Thread ServiceThread;

        private StickyOverlayWindow()
        {
        }

        public StickyOverlayWindow(IntPtr parentWindowHandle)
        {
            ParentWindowHandle = parentWindowHandle;

            var options = new OverlayCreationOptions()
            {
                BypassTopmost = false,
                Height = 600,
                Width = 800,
                WindowTitle = null,
                X = 0,
                Y = 0
            };

            Install(options);
        }

        public StickyOverlayWindow(IntPtr parentWindowHandle, OverlayCreationOptions options)
        {
            ParentWindowHandle = parentWindowHandle;

            Install(options);
        }

        ~StickyOverlayWindow()
        {
            Dispose(false);
        }

        public delegate void WindowBoundsChanged(int x, int y, int width, int height);

        public event WindowBoundsChanged OnWindowBoundsChanged;

        public OverlayWindow OverlayWindow { get; private set; }

        public IntPtr OverlayWindowHandle => OverlayWindow.WindowHandle == IntPtr.Zero ? IntPtr.Zero : OverlayWindow.WindowHandle;

        public IntPtr ParentWindowHandle { get; private set; }

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
                    if (OverlayWindow.IsVisible) OverlayWindow.HideWindow();
                    continue;
                }
                else
                {
                    if (!OverlayWindow.IsVisible) OverlayWindow.ShowWindow();
                }

                if (OverlayWindow.BypassTopmost)
                {
                    IntPtr windowAboveParentWindow = User32.GetWindow(ParentWindowHandle, 3 /* GW_HWNDPREV */);

                    if (windowAboveParentWindow != OverlayWindow.WindowHandle)
                    {
                        User32.SetWindowPos(OverlayWindow.WindowHandle, windowAboveParentWindow, 0, 0, 0, 0, 0x10 | 0x2 | 0x1 | 0x4000); // SWP_NOACTIVATE | SWP_NOMOVE | SWP_NOSIZE | SWP_ASYNCWINDOWPOS
                    }
                }

                HelperMethods.GetRealWindowRect(ParentWindowHandle, out bounds);

                int x = bounds.Left;
                int y = bounds.Top;

                int width = bounds.Right - x;
                int height = bounds.Bottom - y;

                if (OverlayWindow.X == x
                    && OverlayWindow.Y == y
                    && OverlayWindow.Width == width
                    && OverlayWindow.Height == height) continue;

                OverlayWindow.SetWindowBounds(x, y, width, height);

                OnWindowBoundsChanged?.Invoke(x, y, width, height);
            }
        }

        public void Install(OverlayCreationOptions options)
        {
            if (OverlayWindow == null)
            {
                OverlayWindow = new OverlayWindow(options);
            }
            else
            {
                OverlayWindow.Dispose();
                OverlayWindow = new OverlayWindow(options);
            }

            if (ParentWindowHandle == IntPtr.Zero || ExitServiceThread || ServiceThread != null) return;

            OverlayWindow.ShowWindow();

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
            if (OverlayWindow == null) return;
            if (ParentWindowHandle == IntPtr.Zero) return;
            if (ExitServiceThread) return;
            if (ServiceThread == null) return;

            ExitThread();

            OverlayWindow.HideWindow();
        }

        #region IDisposable Support

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }

                UnInstall();

                OverlayWindow.Dispose();

                OverlayWindow = null;

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable Support
    }
}