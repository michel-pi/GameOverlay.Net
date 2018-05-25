using System;
using System.Threading;

using Yato.DirectXOverlay.PInvoke;
using Yato.DirectXOverlay.Renderer;

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
            RendererOptions options = new RendererOptions()
            {
                AntiAliasing = antialiasing,
                Hwnd = IntPtr.Zero,
                MeasureFps = measurefps,
                VSync = vsync
            };
            SetupInstance(parentWindowHandle, options);
        }

        public OverlayManager(IntPtr parentWindowHandle, RendererOptions options)
        {
            SetupInstance(parentWindowHandle, options);
        }

        ~OverlayManager()
        {
            Dispose(false);
        }

        public Direct2DRenderer Graphics { get; private set; }
        public bool IsParentWindowVisible { get; private set; }
        public IntPtr ParentWindowHandle { get; private set; }

        public OverlayWindow Window { get; private set; }

        private void SetupInstance(IntPtr parentWindowHandle, RendererOptions options)
        {
            ParentWindowHandle = parentWindowHandle;

            if (User32.IsWindow(parentWindowHandle) == 0) throw new Exception("The parent window does not exist");

            RECT bounds = new RECT();
            HelperMethods.GetRealWindowRect(parentWindowHandle, out bounds);

            int x = bounds.Left;
            int y = bounds.Top;

            int width = bounds.Right - x;
            int height = bounds.Bottom - y;

            Window = new OverlayWindow(x, y, width, height);

            options.Hwnd = Window.WindowHandle;

            Graphics = new Direct2DRenderer(options);

            serviceThread = new Thread(new ThreadStart(WindowServiceMethod))
            {
                IsBackground = true,
                Priority = ThreadPriority.BelowNormal
            };

            serviceThread.Start();
        }

        private void WindowServiceMethod()
        {
            RECT bounds = new RECT();

            while (!exitThread)
            {
                Thread.Sleep(100);

                if (User32.IsWindowVisible(ParentWindowHandle) == 0)
                {
                    if (Window.IsVisible) Window.HideWindow();
                    continue;
                }

                if (!Window.IsVisible) Window.ShowWindow();

                //if (OverlayWindow.BypassTopmost)
                //{
                //    IntPtr windowAboveParentWindow = User32.GetWindow(ParentWindowHandle, 3 /* GW_HWNDPREV */);

                //    if (windowAboveParentWindow != Window.WindowHandle)
                //    {
                //        User32.SetWindowPos(Window.WindowHandle, windowAboveParentWindow, 0, 0, 0, 0, 0x10 | 0x2 | 0x1 | 0x4000); // SWP_NOACTIVATE | SWP_NOMOVE | SWP_NOSIZE | SWP_ASYNCWINDOWPOS
                //    }
                //}

                HelperMethods.GetRealWindowRect(ParentWindowHandle, out bounds);

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