using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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
    /// <seealso cref="T:System.IDisposable" />
    public class OverlayWindow : IDisposable
    {
        /// <summary>
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public delegate void WindowBoundsChanged(int x, int y, int width, int height);

        private WndProc _windowProc;

        private IntPtr _windowProcPtr;

        private readonly Thread _windowThread;

        /// <summary>
        ///     Initializes a new instance of the <see cref="OverlayWindow" /> class.
        /// </summary>
        /// <param name="bypassTopmost">if set to <c>true</c> [bypass topmost].</param>
        public OverlayWindow(bool bypassTopmost = false)
        {
            BypassTopmost = bypassTopmost;

            _windowThread = new Thread(() => WindowThreadProcedure())
            {
                IsBackground = true,
                Priority = ThreadPriority.BelowNormal
            };
            _windowThread.Start();

            while (!IsInitialized) Thread.Sleep(10);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="OverlayWindow" /> class.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="bypassTopmost">if set to <c>true</c> [bypass topmost].</param>
        public OverlayWindow(int x, int y, int width, int height, bool bypassTopmost = false)
        {
            BypassTopmost = bypassTopmost;

            _windowThread = new Thread(() => WindowThreadProcedure(x, y, width, height))
            {
                IsBackground = true,
                Priority = ThreadPriority.BelowNormal
            };
            _windowThread.Start();

            while (!IsInitialized) Thread.Sleep(10);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="OverlayWindow" /> class.
        /// </summary>
        /// <param name="options">The options.</param>
        public OverlayWindow(OverlayCreationOptions options)
        {
            WindowTitle = options.WindowTitle;
            BypassTopmost = options.BypassTopmost;

            _windowThread = new Thread(() => WindowThreadProcedure(options.X, options.Y, options.Width, options.Height))
            {
                IsBackground = true,
                Priority = ThreadPriority.BelowNormal
            };
            _windowThread.Start();

            while (!IsInitialized) Thread.Sleep(10);
        }

        /// <summary>
        ///     Gets a value indicating whether this instance is initialized.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is initialized; otherwise, <c>false</c>.
        /// </value>
        public bool IsInitialized { get; private set; }

        /// <summary>
        ///     Gets a value indicating whether [bypass topmost].
        /// </summary>
        /// <value><c>true</c> if [bypass topmost]; otherwise, <c>false</c>.</value>
        public bool BypassTopmost { get; }

        /// <summary>
        ///     Gets the height.
        /// </summary>
        /// <value>The height.</value>
        public int Height { get; private set; }

        /// <summary>
        ///     Gets a value indicating whether this instance is visible.
        /// </summary>
        /// <value><c>true</c> if this instance is visible; otherwise, <c>false</c>.</value>
        public bool IsVisible { get; private set; }

        /// <summary>
        ///     Gets a value indicating whether this <see cref="OverlayWindow" /> is topmost.
        /// </summary>
        /// <value><c>true</c> if topmost; otherwise, <c>false</c>.</value>
        public bool Topmost { get; private set; }

        /// <summary>
        ///     Gets the width.
        /// </summary>
        /// <value>The width.</value>
        public int Width { get; private set; }

        /// <summary>
        ///     Gets the name of the window class.
        /// </summary>
        /// <value>The name of the window class.</value>
        public string WindowClassName { get; private set; }

        /// <summary>
        ///     Gets the window handle.
        /// </summary>
        /// <value>The window handle.</value>
        public IntPtr WindowHandle { get; private set; }

        /// <summary>
        ///     Gets the window title.
        /// </summary>
        /// <value>The window title.</value>
        public string WindowTitle { get; private set; }

        /// <summary>
        ///     Gets the x.
        /// </summary>
        /// <value>The x.</value>
        public int X { get; private set; }

        /// <summary>
        ///     Gets the y.
        /// </summary>
        /// <value>The y.</value>
        public int Y { get; private set; }

        /// <summary>
        ///     Occurs when [on window bounds changed]. Assign after device creation
        /// </summary>
        public event WindowBoundsChanged OnWindowBoundsChanged;

        /// <summary>
        ///     Finalizes an instance of the <see cref="OverlayWindow" /> class.
        /// </summary>
        ~OverlayWindow()
        {
            Dispose(false);
        }

        /// <summary>
        ///     Setups the instance.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        private void SetupInstance(int x = 0, int y = 0, int width = 800, int height = 600)
        {
            IsVisible = true;
            Topmost = !BypassTopmost;

            X = x;
            Y = y;
            Width = width;
            Height = height;

            WindowClassName = HelperMethods.GenerateRandomString(5, 11);
            string randomMenuName = HelperMethods.GenerateRandomString(5, 11);

            if (WindowTitle == null) WindowTitle = HelperMethods.GenerateRandomString(5, 11);

            // prepare method
            _windowProc = WindowProcedure;
            RuntimeHelpers.PrepareDelegate(_windowProc);
            _windowProcPtr = Marshal.GetFunctionPointerForDelegate(_windowProc);

            var wndClassEx = new WindowClassEx
            {
                cbSize = WindowClassEx.Size(),
                style = 0,
                lpfnWndProc = _windowProcPtr,
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

            if (User32.RegisterClassEx(ref wndClassEx) == 0)
                WinApi.ThrowWin32Exception("Failed to register window class!");

            uint exStyle;

            if (BypassTopmost)
                exStyle = (uint) (ExtendedWindowStyles.Transparent | ExtendedWindowStyles.Layered |
                                  ExtendedWindowStyles.NoActivate);
            else
                exStyle = (uint) (ExtendedWindowStyles.TopMost | ExtendedWindowStyles.Transparent |
                                  ExtendedWindowStyles.Layered | ExtendedWindowStyles.NoActivate);

            WindowHandle = User32.CreateWindowEx(
                exStyle,
                WindowClassName,
                WindowTitle,
                (uint) (WindowStyles.Popup | WindowStyles.Visible),
                X, Y,
                Width, Height,
                IntPtr.Zero,
                IntPtr.Zero,
                IntPtr.Zero,
                IntPtr.Zero
            );

            if (WindowHandle == IntPtr.Zero) WinApi.ThrowWin32Exception("Failed to create window!");

            User32.SetLayeredWindowAttributes(WindowHandle, 0, 255, (uint) LayeredWindowAttribute.Alpha);
            User32.UpdateWindow(WindowHandle);

            IsInitialized = true;

            // If window is incompatible on some platforms use SetWindowLong to set the style again
            // and UpdateWindow If you have changed certain window data using SetWindowLong, you must
            // call SetWindowPos for the changes to take effect. Use the following combination for
            // uFlags: SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER | SWP_FRAMECHANGED.

            ExtendFrameIntoClientArea();
        }

        private IntPtr WindowProcedure(IntPtr hwnd, WindowsMessage msg, IntPtr wParam, IntPtr lParam)
        {
            switch (msg)
            {
                case WindowsMessage.Destroy:
                    return (IntPtr) 0;

                case WindowsMessage.Erasebkgnd:
                    User32.SendMessage(WindowHandle, WindowsMessage.Paint, (IntPtr) 0, (IntPtr) 0);
                    break;

                case WindowsMessage.Keydown:
                    return (IntPtr) 0;

                case WindowsMessage.Paint:
                    return (IntPtr) 0;

                case WindowsMessage.Dwmcompositionchanged: // needed for windows 7 support
                    ExtendFrameIntoClientArea();
                    return (IntPtr) 0;

                default: break;
            }

            if ((int) msg == 0x02E0) // DPI Changed
                return (IntPtr) 0; // block DPI Changed message

            return User32.DefWindowProc(hwnd, msg, wParam, lParam);
        }

        private void WindowThreadProcedure(int x = 0, int y = 0, int width = 800, int height = 600)
        {
            User32.SetThreadDpiAware();

            SetupInstance(x, y, width, height);

            while (true)
            {
                User32.WaitMessage();

                var message = new Message();

                if (User32.PeekMessageW(ref message, WindowHandle, 0, 0, 1) == 0) continue;

                if (message.Msg == WindowsMessage.Quit) continue;

                User32.TranslateMessage(ref message);
                User32.DispatchMessage(ref message);
            }
        }

        /// <summary>
        ///     Hides the window.
        /// </summary>
        public void HideWindow()
        {
            if (!IsInitialized) throw new InvalidOperationException("The window has not been initialized!");
            if (!IsVisible) return;

            User32.ShowWindow(WindowHandle, 0);
            IsVisible = false;
        }

        /// <summary>
        ///     Moves the window.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        public void MoveWindow(int x, int y)
        {
            if (!IsInitialized) throw new InvalidOperationException("The window has not been initialized!");

            User32.MoveWindow(WindowHandle, x, y, Width, Height, 1);
            X = x;
            Y = y;
            ExtendFrameIntoClientArea();

            InvokeEvents();
        }

        /// <summary>
        ///     Resizes the window.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public void ResizeWindow(int width, int height)
        {
            if (!IsInitialized) throw new InvalidOperationException("The window has not been initialized!");

            User32.MoveWindow(WindowHandle, X, Y, width, height, 1);
            Width = width;
            Height = height;
            ExtendFrameIntoClientArea();

            InvokeEvents();
        }

        /// <summary>
        ///     Sets the window bounds.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public void SetWindowBounds(int x, int y, int width, int height)
        {
            if (!IsInitialized) throw new InvalidOperationException("The window has not been initialized!");

            User32.MoveWindow(WindowHandle, x, y, width, height, 1);
            X = x;
            Y = y;
            Width = width;
            Height = height;
            ExtendFrameIntoClientArea();

            InvokeEvents();
        }

        /// <summary>
        ///     Shows the window.
        /// </summary>
        public void ShowWindow()
        {
            if (!IsInitialized) throw new InvalidOperationException("The window has not been initialized!");
            if (IsVisible) return;

            User32.ShowWindow(WindowHandle, 5);
            ExtendFrameIntoClientArea();
            IsVisible = true;
        }

        /// <summary>
        ///     Extends the frame into client area.
        /// </summary>
        private void ExtendFrameIntoClientArea()
        {
            if (!IsInitialized) throw new InvalidOperationException("The window has not been initialized!");

            var margin = new NativeMargin
            {
                cxLeftWidth = -1,
                cxRightWidth = -1,
                cyBottomHeight = -1,
                cyTopHeight = -1
            };

            DwmApi.DwmExtendFrameIntoClientArea(WindowHandle, ref margin);
        }

        private void InvokeEvents()
        {
            OnWindowBoundsChanged?.Invoke(X, Y, Width, Height);
        }

        private delegate IntPtr WndProc(IntPtr hWnd, WindowsMessage msg, IntPtr wParam, IntPtr lParam);

        #region IDisposable Support

        /// <summary>
        ///     The disposed value
        /// </summary>
        private bool _disposedValue;

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
        ///     unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposedValue || !IsInitialized) return;

            if (_windowThread != null)
            {
                try
                {
                    _windowThread.Abort();
                    _windowThread.Join();
                }
                catch
                {
                    // ignored
                }
            }

            User32.DestroyWindow(WindowHandle);
            User32.UnregisterClass(WindowClassName, IntPtr.Zero);

            _disposedValue = true;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting
        ///     unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable Support
    }
}