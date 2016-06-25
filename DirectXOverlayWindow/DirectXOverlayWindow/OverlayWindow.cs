using System;
using System.Threading;
using System.Threading.Tasks;

namespace DirectXOverlayWindow
{
    public class OverlayWindow
    {
        public Direct2DRenderer Graphics;

        #region private fields
        public bool IsDisposing { get; private set; }
        public bool ParentWindowExists { get; private set; }
        #endregion

        #region public fields
        public bool IsTopMost { get; private set; }
        public bool IsVisible { get; private set; }
        public int X { get; private set; }
        public int Y { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public IntPtr Handle { get; private set; }

        public IntPtr ParentWindow { get; private set; }
        #endregion

        private Native.RawMargin Margin;

        #region construct and destruct
        /// <summary>
        /// Makes a transparent Fullscreen window
        /// </summary>
        /// <param name="limitFPS">VSync</param>
        public OverlayWindow(bool limitFPS = true)
        {
            this.IsDisposing = false;
            this.IsVisible = true;
            this.IsTopMost = true;

            this.ParentWindowExists = false;

            this.X = 0;
            this.Y = 0;
            this.Width = Native.GetSystemMetrics(WindowConstants.SM_CX_SCREEN);
            this.Height = Native.GetSystemMetrics(WindowConstants.SM_CY_SCREEN);

            this.ParentWindow = IntPtr.Zero;

            if (!this.CreateWindow())
                throw new Exception("Could not create OverlayWindow");

            this.Graphics = new Direct2DRenderer(this.Handle, limitFPS);

            this.SetBounds(this.X, this.Y, this.Width, this.Height);
        }
        /// <summary>
        /// Makes a transparent window which adjust it's size and position to fit the parent window
        /// </summary>
        /// <param name="parent">HWND/Handle of a window</param>
        /// <param name="limitFPS">VSync</param>
        public OverlayWindow(IntPtr parent, bool limitFPS = true)
        {
            if (parent == IntPtr.Zero)
                throw new Exception("The handle of the parent window isn't valid");

            Native.RECT bounds;
            Native.GetWindowRect(parent, out bounds);

            this.IsDisposing = false;
            this.IsVisible = true;
            this.IsTopMost = true;

            this.ParentWindowExists = true;

            this.X = bounds.Left;
            this.Y = bounds.Top;

            this.Width = bounds.Right - bounds.Left;
            this.Height = bounds.Bottom - bounds.Top;

            this.ParentWindow = parent;

            if (!this.CreateWindow())
                throw new Exception("Could not create OverlayWindow");

            this.Graphics = new Direct2DRenderer(this.Handle, limitFPS);

            this.SetBounds(this.X, this.Y, this.Width, this.Height);

            new Task(new Action(this.ParentServiceThread)).Start();
        }

        ~OverlayWindow()
        {
            this.Dispose();
        }

        /// <summary>
        /// Clean up used ressources and destroy window
        /// </summary>
        public void Dispose()
        {
            this.IsDisposing = true;
            this.Graphics.Dispose();
            Native.DestroyWindow(this.Handle);
        }
        #endregion

        /// <summary>
        /// Creates a window with the information's stored in this class
        /// </summary>
        /// <returns>true on success</returns>
        private bool CreateWindow()
        {
            this.Handle = Native.CreateWindowEx(
                WindowConstants.WINDOW_EX_STYLE_DX,
                WindowConstants.DESKTOP_CLASS,
                "",
                WindowConstants.WINDOW_STYLE_DX,
                this.X,
                this.Y,
                this.Width,
                this.Height,
                IntPtr.Zero,
                IntPtr.Zero,
                IntPtr.Zero,
                IntPtr.Zero);

            if (this.Handle == IntPtr.Zero)
                return false;

            this.ExtendFrameIntoClient();

            return true;
        }

        /// <summary>
        /// resize and set new position if the parent window's bounds change
        /// </summary>
        private void ParentServiceThread()
        {
            Native.RECT bounds;

            while (!this.IsDisposing)
            {
                Thread.Sleep(10);

                Native.GetWindowRect(this.ParentWindow, out bounds);

                if ((this.X != bounds.Left) || (this.Y != bounds.Top) || (this.Width != bounds.Right - bounds.Left) || (this.Height != bounds.Bottom - bounds.Top))
                    this.SetBounds(bounds.Left, bounds.Top, bounds.Right - bounds.Left, bounds.Bottom - bounds.Top);
            }
        }

        private void ExtendFrameIntoClient()
        {
            this.Margin = new Native.RawMargin
            {
                cxLeftWidth = this.X,
                cxRightWidth = this.Width,
                cyBottomHeight = this.Height,
                cyTopHeight = this.Y
            };

            Native.DwmExtendFrameIntoClientArea(this.Handle, ref this.Margin);
        }

        #region window position and size
        public void SetPos(int x, int y)
        {
            this.X = x;
            this.Y = y;

            Native.POINT pos;
            pos.X = x;
            pos.Y = y;

            Native.POINT size;
            size.X = this.Width;
            size.Y = this.Height;

            Native.UpdateLayeredWindow(this.Handle, IntPtr.Zero, ref pos, ref size, IntPtr.Zero, IntPtr.Zero, 0, IntPtr.Zero, 0);

            this.ExtendFrameIntoClient();
        }
        public void SetSize(int width, int height)
        {
            this.Width = width;
            this.Height = height;

            Native.POINT pos;
            pos.X = this.X;
            pos.Y = this.Y;

            Native.POINT size;
            size.X = this.Width;
            size.Y = this.Height;

            Native.UpdateLayeredWindow(this.Handle, IntPtr.Zero, ref pos, ref size, IntPtr.Zero, IntPtr.Zero, 0, IntPtr.Zero, 0);


            this.Graphics.AutoResize(this.Width, this.Height);

            this.ExtendFrameIntoClient();
        }
        public void SetBounds(int x, int y, int width, int height)
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;

            Native.POINT pos;
            pos.X = x;
            pos.Y = y;

            Native.POINT size;
            size.X = this.Width;
            size.Y = this.Height;

            Native.UpdateLayeredWindow(this.Handle, IntPtr.Zero, ref pos, ref size, IntPtr.Zero, IntPtr.Zero, 0, IntPtr.Zero, 0);

            if (this.Graphics != null)
                this.Graphics.AutoResize(this.Width, this.Height);

            this.ExtendFrameIntoClient();
        }
        #endregion

        #region window show and hide
        public void Show()
        {
            if (this.IsVisible)
                return;

            Native.ShowWindow(this.Handle, WindowConstants.SW_SHOW);
            this.IsVisible = true;

            this.ExtendFrameIntoClient();
        }
        public void Hide()
        {
            if (!this.IsVisible)
                return;

            Native.ShowWindow(this.Handle, WindowConstants.SW_HIDE);
            this.IsVisible = false;
        }
        #endregion
    }
}
