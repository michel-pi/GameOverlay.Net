using System;
using System.Threading;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace Yato.DirectXOverlay
{
    public enum OverlayWindowNameGenerator
    {
        None,
        Random,
        Legit,
        Executable,
        Custom
    }

    public class OverlayWindow : IDisposable
    {
        private string randomClassName;
        private Random rng;
        private Thread windowThread;
        private WndProc wndProc;
        private IntPtr wndProcPointer;
        public static bool BypassTopmost = false;
        public static string CustomWindowName = string.Empty;
        public static OverlayWindowNameGenerator WindowNameGenerator = OverlayWindowNameGenerator.Random;

        public OverlayWindow()
        {
            windowThread = new Thread(() => windowThreadMethod())
            {
                IsBackground = true,
                Priority = ThreadPriority.BelowNormal
            };
            windowThread.Start();

            while (WindowHandle == IntPtr.Zero) Thread.Sleep(10);
        }

        public OverlayWindow(int x, int y, int width, int height)
        {
            windowThread = new Thread(() => windowThreadMethod(x, y, width, height))
            {
                IsBackground = true,
                Priority = ThreadPriority.BelowNormal
            };
            windowThread.Start();

            while (WindowHandle == IntPtr.Zero) Thread.Sleep(10);
        }

        ~OverlayWindow()
        {
            Dispose(false);
        }

        private delegate IntPtr WndProc(IntPtr hWnd, PInvoke.WindowsMessage msg, IntPtr wParam, IntPtr lParam);

        public int Height { get; private set; }
        public bool IsVisible { get; private set; }
        public bool Topmost { get; private set; }
        public int Width { get; private set; }
        public IntPtr WindowHandle { get; private set; }

        public int X { get; private set; }
        public int Y { get; private set; }

        private string generateRandomString(int minlen, int maxlen)
        {
            if (rng == null) rng = new Random();

            int len = rng.Next(minlen, maxlen);

            char[] chars = new char[len];

            for (int i = 0; i < chars.Length; i++)
            {
                chars[i] = (char)rng.Next(97, 123);
            }

            return new string(chars);
        }

        private string getExecutableName()
        {
            var proc = System.Diagnostics.Process.GetCurrentProcess();
            var mod = proc.MainModule;

            string name = mod.FileName;

            mod.Dispose();
            proc.Dispose();

            // Path class tends to throw errors. microsoft is lazy af
            return name.Contains(@"\") ? System.IO.Path.GetFileNameWithoutExtension(name) : name;
        }

        private string getLegitWindowName()
        {
            string[] legitWindows = new string[]
            {
                "Teamspeak 3",
                "Steam",
                "Discord",
                "Mozilla Firefox"
            };

            return legitWindows[rng.Next(0, legitWindows.Length)]; // Note: random max value is exclusive ;)
        }

        private void setupInstance(int x = 0, int y = 0, int width = 800, int height = 600)
        {
            IsVisible = true;
            Topmost = BypassTopmost ? true : false;

            X = x;
            Y = y;
            Width = width;
            Height = height;

            randomClassName = generateRandomString(5, 11);
            string randomMenuName = generateRandomString(5, 11);

            string randomWindowName = string.Empty;//generateRandomString(5, 11);

            switch (WindowNameGenerator)
            {
                case OverlayWindowNameGenerator.None:
                    randomWindowName = string.Empty;
                    break;

                case OverlayWindowNameGenerator.Random:
                    randomWindowName = generateRandomString(5, 11);
                    break;

                case OverlayWindowNameGenerator.Legit:
                    randomWindowName = getLegitWindowName();
                    break;

                case OverlayWindowNameGenerator.Executable:
                    randomWindowName = getExecutableName();
                    break;

                case OverlayWindowNameGenerator.Custom:
                    randomWindowName = CustomWindowName;
                    break;

                default:
                    randomWindowName = string.Empty;
                    break;
            }

            // prepare method
            wndProc = windowProcedure;
            RuntimeHelpers.PrepareDelegate(wndProc);
            wndProcPointer = Marshal.GetFunctionPointerForDelegate(wndProc);

            PInvoke.WNDCLASSEX wndClassEx = new PInvoke.WNDCLASSEX()
            {
                cbSize = PInvoke.WNDCLASSEX.Size(),
                style = 0,
                lpfnWndProc = wndProcPointer,
                cbClsExtra = 0,
                cbWndExtra = 0,
                hInstance = IntPtr.Zero,
                hIcon = IntPtr.Zero,
                hCursor = IntPtr.Zero,
                hbrBackground = IntPtr.Zero,
                lpszMenuName = randomMenuName,
                lpszClassName = randomClassName,
                hIconSm = IntPtr.Zero
            };

            PInvoke.RegisterClassEx(ref wndClassEx);

            uint exStyle;

            if (BypassTopmost)
            {
                exStyle = 0x20 | 0x80000 | 0x80 | 0x8000000;
            }
            else
            {
                exStyle = 0x8 | 0x20 | 0x80000 | 0x80 | 0x8000000; // WS_EX_TOPMOST | WS_EX_TRANSPARENT | WS_EX_LAYERED |WS_EX_TOOLWINDOW | WS_EX_NOACTIVATE
            }

            WindowHandle = PInvoke.CreateWindowEx(
                exStyle, // WS_EX_TOPMOST | WS_EX_TRANSPARENT | WS_EX_LAYERED |WS_EX_TOOLWINDOW | WS_EX_NOACTIVATE
                randomClassName,
                randomWindowName,
                0x80000000 | 0x10000000, // WS_POPUP | WS_VISIBLE
                X, Y,
                Width, Height,
                IntPtr.Zero,
                IntPtr.Zero,
                IntPtr.Zero,
                IntPtr.Zero
                );

            PInvoke.SetLayeredWindowAttributes(WindowHandle, 0, 255, /*0x1 |*/ 0x2);
            PInvoke.UpdateWindow(WindowHandle);

            // TODO: If window is incompatible on some platforms use SetWindowLong to set the style
            //       again and UpdateWindow If you have changed certain window data using
            // SetWindowLong, you must call SetWindowPos for the changes to take effect. Use the
            // following combination for uFlags: SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER | SWP_FRAMECHANGED.

            extendFrameIntoClientArea();
        }

        private IntPtr windowProcedure(IntPtr hwnd, PInvoke.WindowsMessage msg, IntPtr wParam, IntPtr lParam)
        {
            switch (msg)
            {
                case PInvoke.WindowsMessage.WM_DESTROY:
                    return (IntPtr)0;

                case PInvoke.WindowsMessage.WM_ERASEBKGND:
                    PInvoke.SendMessage(WindowHandle, PInvoke.WindowsMessage.WM_PAINT, (IntPtr)0, (IntPtr)0);
                    break;

                case PInvoke.WindowsMessage.WM_KEYDOWN:
                    return (IntPtr)0;

                case PInvoke.WindowsMessage.WM_PAINT:
                    return (IntPtr)0;

                case PInvoke.WindowsMessage.WM_DWMCOMPOSITIONCHANGED: // needed for windows 7 support
                    extendFrameIntoClientArea();
                    return (IntPtr)0;

                default: break;
            }

            if ((int)msg == 0x02E0) // DPI Changed
            {
                return (IntPtr)0; // block DPI Changed message
            }

            return PInvoke.DefWindowProc(hwnd, msg, wParam, lParam);
        }

        private void windowThreadMethod(int x = 0, int y = 0, int width = 800, int height = 600)
        {
            setupInstance(x, y, width, height);

            while (true)
            {
                PInvoke.WaitMessage();

                PInvoke.Message message = new PInvoke.Message();

                if (PInvoke.PeekMessageW(ref message, WindowHandle, 0, 0, 1) != 0)
                {
                    if (message.Msg == PInvoke.WindowsMessage.WM_QUIT) continue;

                    PInvoke.TranslateMessage(ref message);
                    PInvoke.DispatchMessage(ref message);
                }
            }
        }

        public void extendFrameIntoClientArea()
        {
            //var margin = new MARGIN
            //{
            //    cxLeftWidth = this.X,
            //    cxRightWidth = this.Width,
            //    cyBottomHeight = this.Height,
            //    cyTopHeight = this.Y
            //};

            var margin = new PInvoke.MARGIN
            {
                cxLeftWidth = -1,
                cxRightWidth = -1,
                cyBottomHeight = -1,
                cyTopHeight = -1
            };

            PInvoke.DwmExtendFrameIntoClientArea(WindowHandle, ref margin);
        }

        public void HideWindow()
        {
            if (!IsVisible) return;

            PInvoke.ShowWindow(WindowHandle, 0);
            IsVisible = false;
        }

        public void MoveWindow(int x, int y)
        {
            PInvoke.MoveWindow(WindowHandle, x, y, Width, Height, 1);
            X = x;
            Y = y;
            extendFrameIntoClientArea();
        }

        public void ResizeWindow(int width, int height)
        {
            PInvoke.MoveWindow(WindowHandle, X, Y, width, height, 1);
            Width = width;
            Height = height;
            extendFrameIntoClientArea();
        }

        public void SetWindowBounds(int x, int y, int width, int height)
        {
            PInvoke.MoveWindow(WindowHandle, x, y, width, height, 1);
            X = x;
            Y = y;
            Width = width;
            Height = height;
            extendFrameIntoClientArea();
        }

        public void ShowWindow()
        {
            if (IsVisible) return;

            PInvoke.ShowWindow(WindowHandle, 5);
            extendFrameIntoClientArea();
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
                    rng = null;
                }

                if (windowThread != null) windowThread.Abort();

                try
                {
                    windowThread.Join();
                }
                catch
                {
                }

                PInvoke.DestroyWindow(WindowHandle);
                PInvoke.UnregisterClass(randomClassName, IntPtr.Zero);

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