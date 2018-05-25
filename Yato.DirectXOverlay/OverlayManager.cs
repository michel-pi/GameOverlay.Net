using System;
using System.Threading;
using System.Runtime.InteropServices;

namespace Yato.DirectXOverlay
{
    public class OverlayManager : IDisposable
    {
        private bool exitThread;
        private Thread serviceThread;

        private OverlayManager()
        {
        }

        public OverlayManager(IntPtr parentWindowHandle, bool vsync = false, bool measurefps = false, bool antialiasing = true)
        {
            Direct2DRendererOptions options = new Direct2DRendererOptions()
            {
                AntiAliasing = antialiasing,
                Hwnd = IntPtr.Zero,
                MeasureFps = measurefps,
                VSync = vsync
            };
            setupInstance(parentWindowHandle, options);
        }

        public OverlayManager(IntPtr parentWindowHandle, Direct2DRendererOptions options)
        {
            setupInstance(parentWindowHandle, options);
        }

        ~OverlayManager()
        {
            Dispose(false);
        }

        public Direct2DRenderer Graphics { get; private set; }
        public bool IsParentWindowVisible { get; private set; }
        public IntPtr ParentWindowHandle { get; private set; }

        public OverlayWindow Window { get; private set; }

        private void setupInstance(IntPtr parentWindowHandle, Direct2DRendererOptions options)
        {
            ParentWindowHandle = parentWindowHandle;

            if (PInvoke.IsWindow(parentWindowHandle) == 0) throw new Exception("The parent window does not exist");

            PInvoke.RECT bounds = new PInvoke.RECT();
            PInvoke.GetRealWindowRect(parentWindowHandle, out bounds);

            int x = bounds.Left;
            int y = bounds.Top;

            int width = bounds.Right - x;
            int height = bounds.Bottom - y;

            Window = new OverlayWindow(x, y, width, height);

            options.Hwnd = Window.WindowHandle;

            Graphics = new Direct2DRenderer(options);

            serviceThread = new Thread(new ThreadStart(windowServiceThread))
            {
                IsBackground = true,
                Priority = ThreadPriority.BelowNormal
            };

            serviceThread.Start();
        }

        private void windowServiceThread()
        {
            PInvoke.RECT bounds = new PInvoke.RECT();

            while (!exitThread)
            {
                Thread.Sleep(100);

                if (PInvoke.IsWindowVisible(ParentWindowHandle) == 0)
                {
                    if (Window.IsVisible) Window.HideWindow();
                    continue;
                }

                if (!Window.IsVisible) Window.ShowWindow();

                if (OverlayWindow.BypassTopmost)
                {
                    IntPtr windowAboveParentWindow = PInvoke.GetWindow(ParentWindowHandle, 3 /* GW_HWNDPREV */);

                    if (windowAboveParentWindow != Window.WindowHandle)
                    {
                        PInvoke.SetWindowPos(Window.WindowHandle, windowAboveParentWindow, 0, 0, 0, 0, 0x10 | 0x2 | 0x1 | 0x4000); // SWP_NOACTIVATE | SWP_NOMOVE | SWP_NOSIZE | SWP_ASYNCWINDOWPOS
                    }
                }

                PInvoke.GetRealWindowRect(ParentWindowHandle, out bounds);

                int x = bounds.Left;
                int y = bounds.Top;

                int width = bounds.Right - x;
                int height = bounds.Bottom - y;

                if (Window.X == x
                    && Window.Y == y
                    && Window.Width == width
                    && Window.Height == height) continue;

                Window.SetWindowBounds(x, y, width, height);
                Graphics.Resize(width, height);
            }
        }

        #region IDisposable Support

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // managed
                }

                // unmanaged

                if (serviceThread != null)
                {
                    exitThread = true;

                    try
                    {
                        serviceThread.Join();
                    }
                    catch
                    {
                    }
                }

                Graphics.Dispose();
                Window.Dispose();

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