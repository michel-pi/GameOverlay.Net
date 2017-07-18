using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

using Overlay.PInvoke.Libraries;
using Overlay.PInvoke.Structs;

namespace Overlay
{
    public class OverlayWindow
    {
        private static Random _random = new Random();

        public IntPtr Handle { get; private set; }
        public IntPtr ParentWindowHandle { get; private set; }

        public int X { get; private set; }
        public int Y { get; private set; }

        public int Width { get; private set; }
        public int Height { get; private set; }

        public bool Topmost { get; private set; }
        public bool IsVisible { get; private set; }

        public Direct2DRenderer Graphics { get; private set; }

        public OverlayWindow(bool vsync = false, IntPtr parent = default(IntPtr))
        {
            X = 0;
            Y = 0;
            Width = 800;
            Height = 600;

            if (parent == default(IntPtr))
            {
                Width = User32.GetSystemMetrics(0);
                Height = User32.GetSystemMetrics(1);
            }

            string className = GenerateRandomString(5, 11);
            string menuName = GenerateRandomString(5, 11);
            string windowName = GenerateRandomString(5, 11);

            WNDCLASSEX wndClassEx = new WNDCLASSEX()
            {
                cbSize = WNDCLASSEX.Size(),
                style = 0,
                lpfnWndProc = getWindowProcPointer(),
                cbClsExtra = 0,
                cbWndExtra = 0,
                hInstance = IntPtr.Zero,
                hIcon = IntPtr.Zero,
                hCursor = IntPtr.Zero,
                hbrBackground = IntPtr.Zero,
                lpszMenuName = menuName,
                lpszClassName = className,
                hIconSm = IntPtr.Zero
            };

            if (User32.RegisterClassEx(ref wndClassEx) == 0) throw new Exception("RegisterClassExA failed with error code: " + Marshal.GetLastWin32Error());

            Handle = User32.CreateWindowEx(
                0x8 | 0x20 | 0x80000 | 0x80 | 0x8000000, // WS_EX_TOPMOST | WS_EX_TRANSPARENT | WS_EX_LAYERED |WS_EX_TOOLWINDOW
                className,
                windowName,
                0x80000000 | 0x10000000,
                X, Y,
                Width, Height,
                IntPtr.Zero,
                IntPtr.Zero,
                IntPtr.Zero,
                IntPtr.Zero
                );

            if (Handle == IntPtr.Zero) throw new Exception("CreateWindowEx failed with error code: " + Marshal.GetLastWin32Error());

            User32.SetLayeredWindowAttributes(Handle, 0, 255, 0x1 | 0x2);

            Graphics = new Direct2DRenderer(Handle, vsync);

            extendFrameIntoClientArea();

            User32.UpdateWindow(Handle);

            IsVisible = true;
            Topmost = true;

            if (parent == default(IntPtr)) return;

            ParentWindowHandle = parent;

            new Task(_parentServiceThread).Start();
        }

        public void SetBounds(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;

            //Console.WriteLine(x + " " + y + " " + width + " " + height);

            //User32.SetWindowPos(Handle, -1, x, y, Width, Height, 0x40);
            //User32.UpdateLayeredWindow(this.Handle, IntPtr.Zero, ref pos, ref size, IntPtr.Zero, IntPtr.Zero, 0, IntPtr.Zero, 0x1 | 0x2);
            User32.MoveWindow(Handle, X, Y, Width, Height, 1);

            if (this.Graphics != null)
                this.Graphics.Resize(this.Width, this.Height);

            extendFrameIntoClientArea();
        }

        public void Show()
        {
            if (IsVisible) return;

            User32.ShowWindow(Handle, 5);
            IsVisible = true;
        }

        public void Hide()
        {
            if (!IsVisible) return;

            User32.ShowWindow(Handle, 0);
            IsVisible = false;
        }

        private void _parentServiceThread() // TODO: Implement cancellation
        {
            RECT parentBounds = default(RECT);

            while (true)
            {
                Thread.Sleep(100);

                User32.GetWindowRect(ParentWindowHandle, out parentBounds);

                if (X != parentBounds.Left ||
                    Width != parentBounds.Right - parentBounds.Left ||
                    Y != parentBounds.Top ||
                    Height != parentBounds.Bottom - parentBounds.Top)
                {
                    X = parentBounds.Left;
                    Y = parentBounds.Top;
                    Width = parentBounds.Right - parentBounds.Left;
                    Height = parentBounds.Bottom - parentBounds.Top;

                    SetBounds(X, Y,
                        Width,
                        Height);
                }
            }
        }

        private void extendFrameIntoClientArea()
        {
            var margin = new MARGIN
            {
                cxLeftWidth = this.X,
                cxRightWidth = this.Width,
                cyBottomHeight = this.Height,
                cyTopHeight = this.Y
            };

            DwmApi.DwmExtendFrameIntoClientArea(Handle, ref margin);
        }

        private delegate int wndproc(IntPtr handle, uint message, uint wparam, uint lparam);
        private int window_procedure(IntPtr handle, uint message, uint wparam, uint lparam)
        {
            switch (message)
            {
                case 0x12:
                    return 0;
                case 0x14:
                    User32.SendMessage(handle, 0x12, 0, 0);
                    break;
                case 0x100:
                    return 0;
                default:
                    break;
            }

            //extendFrameIntoClientArea();
            return User32.DefWindowProc(handle, message, wparam, lparam);
        }

        private wndproc windowprochandle;
        private IntPtr getWindowProcPointer()
        {
            windowprochandle = (wndproc)window_procedure;
            return Marshal.GetFunctionPointerForDelegate(windowprochandle);
        }

        private static string GenerateRandomString(int minLen, int maxLen)
        {
            int len = _random.Next(minLen, maxLen);

            char[] chars = new char[len];

            for (int i = 0; i < chars.Length; i++)
            {
                chars[i] = (char)_random.Next(97, 123);
            }

            return new string(chars);
        }
    }
}
