using System;
using System.Threading;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

using Yato.DirectXOverlay.PInvoke;

namespace Yato.DirectXOverlay.Windows
{
    public class OverlayWindow : IDisposable
    {
        private WndProc WindowProc;

        private IntPtr WindowProcPtr;

        private Thread WindowThread;

        public OverlayWindow()
        {
            WindowThread = new Thread(() => WindowThreadProcedure())
            {
                IsBackground = true,
                Priority = ThreadPriority.BelowNormal
            };
            WindowThread.Start();

            while (WindowHandle == IntPtr.Zero) Thread.Sleep(10);
        }

        public OverlayWindow(int x, int y, int width, int height)
        {
            WindowThread = new Thread(() => WindowThreadProcedure(x, y, width, height))
            {
                IsBackground = true,
                Priority = ThreadPriority.BelowNormal
            };
            WindowThread.Start();

            while (WindowHandle == IntPtr.Zero) Thread.Sleep(10);
        }

        ~OverlayWindow()
        {
            Dispose(false);
        }

        private delegate IntPtr WndProc(IntPtr hWnd, WindowsMessage msg, IntPtr wParam, IntPtr lParam);

        public int Height { get; private set; }
        public bool IsVisible { get; private set; }
        public bool Topmost { get; private set; }
        public int Width { get; private set; }
        public string WindowClassName { get; private set; }
        public IntPtr WindowHandle { get; private set; }
        public string WindowTitle { get; private set; }
        public int X { get; private set; }
        public int Y { get; private set; }

        private void SetupInstance(int x = 0, int y = 0, int width = 800, int height = 600)
        {
            IsVisible = true;
            Topmost = true;

            X = x;
            Y = y;
            Width = width;
            Height = height;

            WindowClassName = HelperMethods.GenerateRandomString(5, 11);
            string randomMenuName = HelperMethods.GenerateRandomString(5, 11);

            WindowTitle = HelperMethods.GenerateRandomString(5, 11);

            // prepare method
            WindowProc = WindowProcedure;
            RuntimeHelpers.PrepareDelegate(WindowProc);
            WindowProcPtr = Marshal.GetFunctionPointerForDelegate(WindowProc);

            WNDCLASSEX wndClassEx = new WNDCLASSEX()
            {
                cbSize = WNDCLASSEX.Size(),
                style = 0,
                lpfnWndProc = WindowProcPtr,
                cbClsExtra = 0,
                cbWndExtra = 0,
                hInstance = IntPtr.Zero,
                hIcon = IntPtr.Zero,
                hCursor = IntPtr.Zero,
                hbrBackground = IntPtr.Zero,
                lpszMenuName = randomMenuName,
                lpszClassName = WindowClassName,
                hIconSm = IntPtr.Zero
            };

            User32.RegisterClassEx(ref wndClassEx);

            uint exStyle;

            //if (BypassTopmost)
            //{
            //    exStyle = 0x20 | 0x80000 | 0x80 | 0x8000000;
            //}
            //else
            //{
            exStyle = 0x8 | 0x20 | 0x80000 | 0x80 | 0x8000000; // WS_EX_TOPMOST | WS_EX_TRANSPARENT | WS_EX_LAYERED |WS_EX_TOOLWINDOW | WS_EX_NOACTIVATE
            //}

            WindowHandle = User32.CreateWindowEx(
                exStyle, // WS_EX_TOPMOST | WS_EX_TRANSPARENT | WS_EX_LAYERED |WS_EX_TOOLWINDOW | WS_EX_NOACTIVATE
                WindowClassName,
                WindowTitle,
                0x80000000 | 0x10000000, // WS_POPUP | WS_VISIBLE
                X, Y,
                Width, Height,
                IntPtr.Zero,
                IntPtr.Zero,
                IntPtr.Zero,
                IntPtr.Zero
                );

            User32.SetLayeredWindowAttributes(WindowHandle, 0, 255, /*0x1 |*/ 0x2);
            User32.UpdateWindow(WindowHandle);

            // TODO: If window is incompatible on some platforms use SetWindowLong to set the style
            //       again and UpdateWindow If you have changed certain window data using
            // SetWindowLong, you must call SetWindowPos for the changes to take effect. Use the
            // following combination for uFlags: SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER | SWP_FRAMECHANGED.

            ExtendFrameIntoClientArea();
        }

        private IntPtr WindowProcedure(IntPtr hwnd, WindowsMessage msg, IntPtr wParam, IntPtr lParam)
        {
            switch (msg)
            {
                case WindowsMessage.WM_DESTROY:
                    return (IntPtr)0;

                case WindowsMessage.WM_ERASEBKGND:
                    User32.SendMessage(WindowHandle, WindowsMessage.WM_PAINT, (IntPtr)0, (IntPtr)0);
                    break;

                case WindowsMessage.WM_KEYDOWN:
                    return (IntPtr)0;

                case WindowsMessage.WM_PAINT:
                    return (IntPtr)0;

                case WindowsMessage.WM_DWMCOMPOSITIONCHANGED: // needed for windows 7 support
                    ExtendFrameIntoClientArea();
                    return (IntPtr)0;

                default: break;
            }

            if ((int)msg == 0x02E0) // DPI Changed
            {
                return (IntPtr)0; // block DPI Changed message
            }

            return User32.DefWindowProc(hwnd, msg, wParam, lParam);
        }

        private void WindowThreadProcedure(int x = 0, int y = 0, int width = 800, int height = 600)
        {
            SetupInstance(x, y, width, height);

            while (true)
            {
                User32.WaitMessage();

                Message message = new Message();

                if (User32.PeekMessageW(ref message, WindowHandle, 0, 0, 1) != 0)
                {
                    if (message.Msg == WindowsMessage.WM_QUIT) continue;

                    User32.TranslateMessage(ref message);
                    User32.DispatchMessage(ref message);
                }
            }
        }

        public void ExtendFrameIntoClientArea()
        {
            var margin = new PInvoke.MARGIN
            {
                cxLeftWidth = -1,
                cxRightWidth = -1,
                cyBottomHeight = -1,
                cyTopHeight = -1
            };

            DwmApi.DwmExtendFrameIntoClientArea(WindowHandle, ref margin);
        }

        public void HideWindow()
        {
            if (!IsVisible) return;

            User32.ShowWindow(WindowHandle, 0);
            IsVisible = false;
        }

        public void MoveWindow(int x, int y)
        {
            User32.MoveWindow(WindowHandle, x, y, Width, Height, 1);
            X = x;
            Y = y;
            ExtendFrameIntoClientArea();
        }

        public void ResizeWindow(int width, int height)
        {
            User32.MoveWindow(WindowHandle, X, Y, width, height, 1);
            Width = width;
            Height = height;
            ExtendFrameIntoClientArea();
        }

        public void SetWindowBounds(int x, int y, int width, int height)
        {
            User32.MoveWindow(WindowHandle, x, y, width, height, 1);
            X = x;
            Y = y;
            Width = width;
            Height = height;
            ExtendFrameIntoClientArea();
        }

        public void ShowWindow()
        {
            if (IsVisible) return;

            User32.ShowWindow(WindowHandle, 5);
            ExtendFrameIntoClientArea();
            IsVisible = true;
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

                if (WindowThread != null) WindowThread.Abort();

                try
                {
                    WindowThread.Join();
                }
                catch
                {
                }

                User32.DestroyWindow(WindowHandle);
                User32.UnregisterClass(WindowClassName, IntPtr.Zero);

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