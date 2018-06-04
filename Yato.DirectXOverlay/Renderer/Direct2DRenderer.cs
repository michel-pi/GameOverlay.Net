using System;
using System.Diagnostics;

using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;

using FontFactory = SharpDX.DirectWrite.Factory;
using Factory = SharpDX.Direct2D1.Factory;

using Yato.DirectXOverlay.PInvoke;

namespace Yato.DirectXOverlay.Renderer
{
    /// <summary>
    /// Represents a drawing device of a window
    /// </summary>
    /// <seealso cref="System.IDisposable"/>
    public class Direct2DRenderer : IDisposable
    {
        #region private vars

        private WindowRenderTarget _device;
        private HwndRenderTargetProperties _deviceProperties;
        private Factory _factory;
        private FontFactory _fontFactory;
        private int _internalFps;
        private bool _isDrawing;
        private RendererOptions _rendererOptions;
        private bool _resize;
        private int _resizeHeight;
        private int _resizeWidth;
        private SolidColorBrush _sharedBrush;
        private TextFormat _sharedFont;
        private Stopwatch _stopwatch = new Stopwatch();

        #endregion private vars

        #region public vars

        /// <summary>
        /// Gets the FPS
        /// </summary>
        /// <value>The FPS</value>
        public int FPS { get; private set; }

        /// <summary>
        /// Gets the renderers height
        /// </summary>
        /// <value>The height</value>
        public int Height { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether [measure FPS].
        /// </summary>
        /// <value><c>true</c> if [measure FPS]; otherwise, <c>false</c>.</value>
        public bool MeasureFPS { get; set; }

        /// <summary>
        /// Gets the render target HWND.
        /// </summary>
        /// <value>The render target HWND.</value>
        public IntPtr RenderTargetHwnd { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [vertical synchronize].
        /// </summary>
        /// <value><c>true</c> if [vertical synchronize]; otherwise, <c>false</c>.</value>
        public bool VSync { get; private set; }

        /// <summary>
        /// Gets the renderers width
        /// </summary>
        /// <value>The width</value>
        public int Width { get; private set; }

        #endregion public vars

        #region construct & destruct

        private Direct2DRenderer()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Direct2DRenderer"/> class.
        /// </summary>
        /// <param name="hwnd">A valid window handle</param>
        public Direct2DRenderer(IntPtr hwnd)
        {
            var options = new RendererOptions()
            {
                Hwnd = hwnd,
                VSync = false,
                MeasureFps = false,
                AntiAliasing = false
            };
            SetupInstance(options);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Direct2DRenderer"/> class.
        /// </summary>
        /// <param name="hwnd">A valid window handle</param>
        /// <param name="vsync">if set to <c>true</c> [vsync].</param>
        public Direct2DRenderer(IntPtr hwnd, bool vsync)
        {
            var options = new RendererOptions()
            {
                Hwnd = hwnd,
                VSync = vsync,
                MeasureFps = false,
                AntiAliasing = false
            };
            SetupInstance(options);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Direct2DRenderer"/> class.
        /// </summary>
        /// <param name="hwnd">A valid window handle</param>
        /// <param name="vsync">if set to <c>true</c> [vsync].</param>
        /// <param name="measureFps">if set to <c>true</c> [measure FPS].</param>
        public Direct2DRenderer(IntPtr hwnd, bool vsync, bool measureFps)
        {
            var options = new RendererOptions()
            {
                Hwnd = hwnd,
                VSync = vsync,
                MeasureFps = measureFps,
                AntiAliasing = false
            };
            SetupInstance(options);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Direct2DRenderer"/> class.
        /// </summary>
        /// <param name="hwnd">A valid window handle</param>
        /// <param name="vsync">if set to <c>true</c> [vsync].</param>
        /// <param name="measureFps">if set to <c>true</c> [measure FPS].</param>
        /// <param name="antiAliasing">if set to <c>true</c> [anti aliasing].</param>
        public Direct2DRenderer(IntPtr hwnd, bool vsync, bool measureFps, bool antiAliasing)
        {
            var options = new RendererOptions()
            {
                Hwnd = hwnd,
                VSync = vsync,
                MeasureFps = measureFps,
                AntiAliasing = antiAliasing
            };
            SetupInstance(options);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Direct2DRenderer"/> class.
        /// </summary>
        /// <param name="options">Creation options</param>
        public Direct2DRenderer(RendererOptions options)
        {
            SetupInstance(options);
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="Direct2DRenderer"/> class.
        /// </summary>
        ~Direct2DRenderer()
        {
            Dispose(false);
        }

        #endregion construct & destruct

        #region init & delete

        private void DestroyInstance()
        {
            try
            {
                _sharedBrush.Dispose();
                _fontFactory.Dispose();
                _factory.Dispose();
                _device.Dispose();
            }
            catch
            {
            }
        }

        private void SetupInstance(RendererOptions options)
        {
            _rendererOptions = options;

            if (options.Hwnd == IntPtr.Zero) throw new ArgumentNullException(nameof(options.Hwnd));

            if (User32.IsWindow(options.Hwnd) == 0) throw new ArgumentException("The window does not exist (hwnd = 0x" + options.Hwnd.ToString("X") + ")");

            RECT bounds = new RECT();

            if (HelperMethods.GetRealWindowRect(options.Hwnd, out bounds) == 0) throw new Exception("Failed to get the size of the given window (hwnd = 0x" + options.Hwnd.ToString("X") + ")");

            this.Width = bounds.Right - bounds.Left;
            this.Height = bounds.Bottom - bounds.Top;

            this.VSync = options.VSync;
            this.MeasureFPS = options.MeasureFps;

            _deviceProperties = new HwndRenderTargetProperties()
            {
                Hwnd = options.Hwnd,
                PixelSize = new Size2(this.Width, this.Height),
                PresentOptions = options.VSync ? PresentOptions.None : PresentOptions.Immediately
            };

            var renderProperties = new RenderTargetProperties(
                RenderTargetType.Default,
                new PixelFormat(Format.R8G8B8A8_UNorm, SharpDX.Direct2D1.AlphaMode.Premultiplied),
                96.0f, 96.0f, // we use 96.0f because it's the default value. This will scale every drawing by 1.0f (it obviously does not scale anything). Our drawing will be dpi aware!
                RenderTargetUsage.None,
                FeatureLevel.Level_DEFAULT);

            _factory = new Factory();
            _fontFactory = new FontFactory();

            try
            {
                _device = new WindowRenderTarget(_factory, renderProperties, _deviceProperties);
            }
            catch (SharpDXException) // D2DERR_UNSUPPORTED_PIXEL_FORMAT
            {
                renderProperties.PixelFormat = new PixelFormat(Format.B8G8R8A8_UNorm, SharpDX.Direct2D1.AlphaMode.Premultiplied);
                _device = new WindowRenderTarget(_factory, renderProperties, _deviceProperties);
            }

            _device.AntialiasMode = AntialiasMode.Aliased; // AntialiasMode.PerPrimitive fails rendering some objects
            // other than in the documentation: Cleartype is much faster for me than GrayScale
            _device.TextAntialiasMode = options.AntiAliasing ? SharpDX.Direct2D1.TextAntialiasMode.Cleartype : SharpDX.Direct2D1.TextAntialiasMode.Aliased;

            _sharedBrush = new SolidColorBrush(_device, default(RawColor4));
        }

        #endregion init & delete

        #region Scenes

        /// <summary>
        /// Begins a new scene
        /// </summary>
        public void BeginScene()
        {
            if (_device == null) return;
            if (_isDrawing) return;

            if (MeasureFPS && !_stopwatch.IsRunning)
            {
                _stopwatch.Restart();
            }

            if (_resize)
            {
                _device.Resize(new Size2(_resizeWidth, _resizeHeight));
                _resize = false;
            }

            _device.BeginDraw();

            _isDrawing = true;
        }

        /// <summary>
        /// Clears the scene. Transparent
        /// </summary>
        public void ClearScene()
        {
            _device.Clear(null);
        }

        /// <summary>
        /// Clears the scene with background color
        /// </summary>
        /// <param name="color">The color</param>
        public void ClearScene(Direct2DColor color)
        {
            _device.Clear(color);
        }

        /// <summary>
        /// Clears the scene with background color
        /// </summary>
        /// <param name="brush">The brush</param>
        public void ClearScene(Direct2DBrush brush)
        {
            _device.Clear(brush);
        }

        /// <summary>
        /// Ends the scene
        /// </summary>
        public void EndScene()
        {
            if (_device == null) return;
            if (!_isDrawing) return;

            var result = _device.TryEndDraw(out long tag_0, out long tag_1);

            if (result.Failure)
            {
                DestroyInstance();
                SetupInstance(_rendererOptions);
            }

            if (MeasureFPS && _stopwatch.IsRunning)
            {
                _internalFps++;

                if (_stopwatch.ElapsedMilliseconds > 1000)
                {
                    FPS = _internalFps;
                    _internalFps = 0;
                    _stopwatch.Stop();
                }
            }

            _isDrawing = false;
        }

        /// <summary>
        /// Resizes the renderer (call this when the window has changed it's size)
        /// </summary>
        /// <param name="width">The width</param>
        /// <param name="height">The height</param>
        public void Resize(int width, int height)
        {
            if (Width == width && height == Height) return;

            _resizeWidth = width;
            _resizeHeight = height;
            _resize = true;
        }

        /// <summary>
        /// Fancy IDisposable pattern for scenes
        /// </summary>
        /// <returns></returns>
        public Direct2DScene UseScene()
        {
            // really expensive to use but i like the pattern
            return new Direct2DScene(this);
        }

        #endregion Scenes

        #region Fonts & Brushes & Bitmaps

        /// <summary>
        /// Creates a new SolidColorBrush
        /// </summary>
        /// <param name="color">The color</param>
        /// <returns></returns>
        public Direct2DBrush CreateBrush(Direct2DColor color)
        {
            return new Direct2DBrush(_device, color);
        }

        /// <summary>
        /// Creates a new SolidColorBrush
        /// </summary>
        /// <param name="r">Red 0 - 255</param>
        /// <param name="g">Green 0 - 255</param>
        /// <param name="b">Blue 0 - 255</param>
        /// <param name="a">Alpha 0 - 255</param>
        /// <returns></returns>
        public Direct2DBrush CreateBrush(int r, int g, int b, int a = 255)
        {
            return new Direct2DBrush(_device, new Direct2DColor(r, g, b, a));
        }

        /// <summary>
        /// Creates a new SolidColorBrush
        /// </summary>
        /// <param name="r">Red 0.0f - 1.0f</param>
        /// <param name="g">Green 0.0f - 1.0f</param>
        /// <param name="b">Blue 0.0f - 1.0f</param>
        /// <param name="a">Alpha 0.0f - 1.0f</param>
        /// <returns></returns>
        public Direct2DBrush CreateBrush(float r, float g, float b, float a = 1.0f)
        {
            return new Direct2DBrush(_device, new Direct2DColor(r, g, b, a));
        }

        /// <summary>
        /// Creates a new font. Used for drawing text
        /// </summary>
        /// <param name="fontFamilyName">Name of the font family.</param>
        /// <param name="size">The size.</param>
        /// <param name="bold">if set to <c>true</c> [bold].</param>
        /// <param name="italic">if set to <c>true</c> [italic].</param>
        /// <returns></returns>
        public Direct2DFont CreateFont(string fontFamilyName, float size, bool bold = false, bool italic = false)
        {
            return new Direct2DFont(_fontFactory, fontFamilyName, size, bold, italic);
        }

        /// <summary>
        /// Creates a new font. Used for drawing text
        /// </summary>
        /// <param name="options">Creation options</param>
        /// <returns></returns>
        public Direct2DFont CreateFont(FontCreationOptions options)
        {
            TextFormat font = new TextFormat(_fontFactory, options.FontFamilyName, options.Bold ? FontWeight.Bold : FontWeight.Normal, options.GetStyle(), options.FontSize);
            return new Direct2DFont(font);
        }

        /// <summary>
        /// Loads a bitmap from a file
        /// </summary>
        /// <param name="file">Path</param>
        /// <returns></returns>
        public Direct2DBitmap LoadBitmap(string file)
        {
            return new Direct2DBitmap(_device, file);
        }

        /// <summary>
        /// Loads a bitmap from a <c>byte[]</c>
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <returns></returns>
        public Direct2DBitmap LoadBitmap(byte[] bytes)
        {
            return new Direct2DBitmap(_device, bytes);
        }

        /// <summary>
        /// Sets the font to use when no font is specified
        /// </summary>
        /// <param name="fontFamilyName">Name of the font family.</param>
        /// <param name="size">The size.</param>
        /// <param name="bold">if set to <c>true</c> [bold].</param>
        /// <param name="italic">if set to <c>true</c> [italic].</param>
        public void SetSharedFont(string fontFamilyName, float size, bool bold = false, bool italic = false)
        {
            _sharedFont = new TextFormat(_fontFactory, fontFamilyName, bold ? FontWeight.Bold : FontWeight.Normal, italic ? FontStyle.Italic : FontStyle.Normal, size);
        }

        #endregion Fonts & Brushes & Bitmaps

        #region Primitives

        /// <summary>
        /// Draws a circle
        /// </summary>
        /// <param name="x">X - Circle center</param>
        /// <param name="y">Y - Circel center</param>
        /// <param name="radius">Circle radius</param>
        /// <param name="stroke">Line stroke</param>
        /// <param name="brush">Brush to use</param>
        public void DrawCircle(float x, float y, float radius, float stroke, Direct2DBrush brush)
        {
            _device.DrawEllipse(new Ellipse(new RawVector2(x, y), radius, radius), brush, stroke);
        }

        /// <summary>
        /// Draws a circle (not thread safe)
        /// </summary>
        /// <param name="x">X - Circle center</param>
        /// <param name="y">Y - Circel center</param>
        /// <param name="radius">Circle radius</param>
        /// <param name="stroke">Line stroke</param>
        /// <param name="color"><c>Direct2DColor</c></param>
        public void DrawCircle(float x, float y, float radius, float stroke, Direct2DColor color)
        {
            _sharedBrush.Color = color;
            _device.DrawEllipse(new Ellipse(new RawVector2(x, y), radius, radius), _sharedBrush, stroke);
        }

        /// <summary>
        /// Draws the ellipse.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="radius_x">The radius x.</param>
        /// <param name="radius_y">The radius y.</param>
        /// <param name="stroke">The stroke.</param>
        /// <param name="brush">The brush.</param>
        public void DrawEllipse(float x, float y, float radius_x, float radius_y, float stroke, Direct2DBrush brush)
        {
            _device.DrawEllipse(new Ellipse(new RawVector2(x, y), radius_x, radius_y), brush, stroke);
        }

        /// <summary>
        /// Draws an ellipse (not thread safe)
        /// </summary>
        /// <param name="x">X - Ellipse center</param>
        /// <param name="y">Y - Ellipse center</param>
        /// <param name="radius_x">The radius on x axis</param>
        /// <param name="radius_y">The radius on y axis</param>
        /// <param name="stroke">Line stroke</param>
        /// <param name="color"><c>Direct2DColor</c></param>
        public void DrawEllipse(float x, float y, float radius_x, float radius_y, float stroke, Direct2DColor color)
        {
            _sharedBrush.Color = color;
            _device.DrawEllipse(new Ellipse(new RawVector2(x, y), radius_x, radius_y), _sharedBrush, stroke);
        }

        /// <summary>
        /// Draws a line
        /// </summary>
        /// <param name="start_x">The start x.</param>
        /// <param name="start_y">The start y.</param>
        /// <param name="end_x">The end x.</param>
        /// <param name="end_y">The end y.</param>
        /// <param name="stroke">Line stroke</param>
        /// <param name="brush">The brush.</param>
        public void DrawLine(float start_x, float start_y, float end_x, float end_y, float stroke, Direct2DBrush brush)
        {
            _device.DrawLine(new RawVector2(start_x, start_y), new RawVector2(end_x, end_y), brush, stroke);
        }

        /// <summary>
        /// Draws a line
        /// </summary>
        /// <param name="start_x">The start x.</param>
        /// <param name="start_y">The start y.</param>
        /// <param name="end_x">The end x.</param>
        /// <param name="end_y">The end y.</param>
        /// <param name="stroke">Line stroke</param>
        /// <param name="color">The color.</param>
        public void DrawLine(float start_x, float start_y, float end_x, float end_y, float stroke, Direct2DColor color)
        {
            _sharedBrush.Color = color;
            _device.DrawLine(new RawVector2(start_x, start_y), new RawVector2(end_x, end_y), _sharedBrush, stroke);
        }

        /// <summary>
        /// Draws the rectangle.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="stroke">The stroke.</param>
        /// <param name="brush">The brush.</param>
        public void DrawRectangle(float x, float y, float width, float height, float stroke, Direct2DBrush brush)
        {
            _device.DrawRectangle(new RawRectangleF(x, y, x + width, y + height), brush, stroke);
        }

        /// <summary>
        /// Draws the rectangle.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="stroke">The stroke.</param>
        /// <param name="color">The color.</param>
        public void DrawRectangle(float x, float y, float width, float height, float stroke, Direct2DColor color)
        {
            _sharedBrush.Color = color;
            _device.DrawRectangle(new RawRectangleF(x, y, x + width, y + height), _sharedBrush, stroke);
        }

        /// <summary>
        /// Draws the rectangle edges.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="stroke">The stroke.</param>
        /// <param name="brush">The brush.</param>
        public void DrawRectangleEdges(float x, float y, float width, float height, float stroke, Direct2DBrush brush)
        {
            int length = (int)(((width + height) / 2.0f) * 0.2f);

            RawVector2 first = new RawVector2(x, y);
            RawVector2 second = new RawVector2(x, y + length);
            RawVector2 third = new RawVector2(x + length, y);

            _device.DrawLine(first, second, brush, stroke);
            _device.DrawLine(first, third, brush, stroke);

            first.Y += height;
            second.Y = first.Y - length;
            third.Y = first.Y;
            third.X = first.X + length;

            _device.DrawLine(first, second, brush, stroke);
            _device.DrawLine(first, third, brush, stroke);

            first.X = x + width;
            first.Y = y;
            second.X = first.X - length;
            second.Y = first.Y;
            third.X = first.X;
            third.Y = first.Y + length;

            _device.DrawLine(first, second, brush, stroke);
            _device.DrawLine(first, third, brush, stroke);

            first.Y += height;
            second.X += length;
            second.Y = first.Y - length;
            third.Y = first.Y;
            third.X = first.X - length;

            _device.DrawLine(first, second, brush, stroke);
            _device.DrawLine(first, third, brush, stroke);
        }

        /// <summary>
        /// Draws the rectangle edges.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="stroke">The stroke.</param>
        /// <param name="color">The color.</param>
        public void DrawRectangleEdges(float x, float y, float width, float height, float stroke, Direct2DColor color)
        {
            _sharedBrush.Color = color;

            int length = (int)(((width + height) / 2.0f) * 0.2f);

            RawVector2 first = new RawVector2(x, y);
            RawVector2 second = new RawVector2(x, y + length);
            RawVector2 third = new RawVector2(x + length, y);

            _device.DrawLine(first, second, _sharedBrush, stroke);
            _device.DrawLine(first, third, _sharedBrush, stroke);

            first.Y += height;
            second.Y = first.Y - length;
            third.Y = first.Y;
            third.X = first.X + length;

            _device.DrawLine(first, second, _sharedBrush, stroke);
            _device.DrawLine(first, third, _sharedBrush, stroke);

            first.X = x + width;
            first.Y = y;
            second.X = first.X - length;
            second.Y = first.Y;
            third.X = first.X;
            third.Y = first.Y + length;

            _device.DrawLine(first, second, _sharedBrush, stroke);
            _device.DrawLine(first, third, _sharedBrush, stroke);

            first.Y += height;
            second.X += length;
            second.Y = first.Y - length;
            third.Y = first.Y;
            third.X = first.X - length;

            _device.DrawLine(first, second, _sharedBrush, stroke);
            _device.DrawLine(first, third, _sharedBrush, stroke);
        }

        #endregion Primitives

        #region Filled

        /// <summary>
        /// Fills the circle.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="radius">The radius.</param>
        /// <param name="brush">The brush.</param>
        public void FillCircle(float x, float y, float radius, Direct2DBrush brush)
        {
            _device.FillEllipse(new Ellipse(new RawVector2(x, y), radius, radius), brush);
        }

        /// <summary>
        /// Fills the circle.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="radius">The radius.</param>
        /// <param name="color">The color.</param>
        public void FillCircle(float x, float y, float radius, Direct2DColor color)
        {
            _sharedBrush.Color = color;
            _device.FillEllipse(new Ellipse(new RawVector2(x, y), radius, radius), _sharedBrush);
        }

        /// <summary>
        /// Fills the ellipse.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="radius_x">The radius x.</param>
        /// <param name="radius_y">The radius y.</param>
        /// <param name="brush">The brush.</param>
        public void FillEllipse(float x, float y, float radius_x, float radius_y, Direct2DBrush brush)
        {
            _device.FillEllipse(new Ellipse(new RawVector2(x, y), radius_x, radius_y), brush);
        }

        /// <summary>
        /// Fills the ellipse.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="radius_x">The radius x.</param>
        /// <param name="radius_y">The radius y.</param>
        /// <param name="color">The color.</param>
        public void FillEllipse(float x, float y, float radius_x, float radius_y, Direct2DColor color)
        {
            _sharedBrush.Color = color;
            _device.FillEllipse(new Ellipse(new RawVector2(x, y), radius_x, radius_y), _sharedBrush);
        }

        /// <summary>
        /// Fills the rectangle.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="brush">The brush.</param>
        public void FillRectangle(float x, float y, float width, float height, Direct2DBrush brush)
        {
            _device.FillRectangle(new RawRectangleF(x, y, x + width, y + height), brush);
        }

        /// <summary>
        /// Fills the rectangle.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="color">The color.</param>
        public void FillRectangle(float x, float y, float width, float height, Direct2DColor color)
        {
            _sharedBrush.Color = color;
            _device.FillRectangle(new RawRectangleF(x, y, x + width, y + height), _sharedBrush);
        }

        #endregion Filled

        #region Bordered

        /// <summary>
        /// Bordereds the circle.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="radius">The radius.</param>
        /// <param name="stroke">The stroke.</param>
        /// <param name="color">The color.</param>
        /// <param name="borderColor">Color of the border.</param>
        public void BorderedCircle(float x, float y, float radius, float stroke, Direct2DColor color, Direct2DColor borderColor)
        {
            _sharedBrush.Color = color;

            var ellipse = new Ellipse(new RawVector2(x, y), radius, radius);

            _device.DrawEllipse(ellipse, _sharedBrush, stroke);

            float half = stroke / 2.0f;

            _sharedBrush.Color = borderColor;

            ellipse.RadiusX += half;
            ellipse.RadiusY += half;

            _device.DrawEllipse(ellipse, _sharedBrush, half);

            ellipse.RadiusX -= stroke;
            ellipse.RadiusY -= stroke;

            _device.DrawEllipse(ellipse, _sharedBrush, half);
        }

        /// <summary>
        /// Bordereds the circle.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="radius">The radius.</param>
        /// <param name="stroke">The stroke.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="borderBrush">The border brush.</param>
        public void BorderedCircle(float x, float y, float radius, float stroke, Direct2DBrush brush, Direct2DBrush borderBrush)
        {
            var ellipse = new Ellipse(new RawVector2(x, y), radius, radius);

            _device.DrawEllipse(ellipse, brush, stroke);

            float half = stroke / 2.0f;

            ellipse.RadiusX += half;
            ellipse.RadiusY += half;

            _device.DrawEllipse(ellipse, borderBrush, half);

            ellipse.RadiusX -= stroke;
            ellipse.RadiusY -= stroke;

            _device.DrawEllipse(ellipse, borderBrush, half);
        }

        /// <summary>
        /// Bordereds the line.
        /// </summary>
        /// <param name="start_x">The start x.</param>
        /// <param name="start_y">The start y.</param>
        /// <param name="end_x">The end x.</param>
        /// <param name="end_y">The end y.</param>
        /// <param name="stroke">The stroke.</param>
        /// <param name="color">The color.</param>
        /// <param name="borderColor">Color of the border.</param>
        public void BorderedLine(float start_x, float start_y, float end_x, float end_y, float stroke, Direct2DColor color, Direct2DColor borderColor)
        {
            var geometry = new PathGeometry(_factory);

            var sink = geometry.Open();

            float half = stroke / 2.0f;
            float quarter = half / 2.0f;

            sink.BeginFigure(new RawVector2(start_x, start_y - half), FigureBegin.Filled);

            sink.AddLine(new RawVector2(end_x, end_y - half));
            sink.AddLine(new RawVector2(end_x, end_y + half));
            sink.AddLine(new RawVector2(start_x, start_y + half));

            sink.EndFigure(FigureEnd.Closed);

            sink.Close();

            _sharedBrush.Color = borderColor;

            _device.DrawGeometry(geometry, _sharedBrush, half);

            _sharedBrush.Color = color;

            _device.FillGeometry(geometry, _sharedBrush);

            sink.Dispose();
            geometry.Dispose();
        }

        /// <summary>
        /// Bordereds the line.
        /// </summary>
        /// <param name="start_x">The start x.</param>
        /// <param name="start_y">The start y.</param>
        /// <param name="end_x">The end x.</param>
        /// <param name="end_y">The end y.</param>
        /// <param name="stroke">The stroke.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="borderBrush">The border brush.</param>
        public void BorderedLine(float start_x, float start_y, float end_x, float end_y, float stroke, Direct2DBrush brush, Direct2DBrush borderBrush)
        {
            var geometry = new PathGeometry(_factory);

            var sink = geometry.Open();

            float half = stroke / 2.0f;
            float quarter = half / 2.0f;

            sink.BeginFigure(new RawVector2(start_x, start_y - half), FigureBegin.Filled);

            sink.AddLine(new RawVector2(end_x, end_y - half));
            sink.AddLine(new RawVector2(end_x, end_y + half));
            sink.AddLine(new RawVector2(start_x, start_y + half));

            sink.EndFigure(FigureEnd.Closed);

            sink.Close();

            _device.DrawGeometry(geometry, borderBrush, half);

            _device.FillGeometry(geometry, brush);

            sink.Dispose();
            geometry.Dispose();
        }

        /// <summary>
        /// Bordereds the rectangle.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="stroke">The stroke.</param>
        /// <param name="color">The color.</param>
        /// <param name="borderColor">Color of the border.</param>
        public void BorderedRectangle(float x, float y, float width, float height, float stroke, Direct2DColor color, Direct2DColor borderColor)
        {
            float half = stroke / 2.0f;

            width += x;
            height += y;

            _sharedBrush.Color = color;

            _device.DrawRectangle(new RawRectangleF(x, y, width, height), _sharedBrush, half);

            _sharedBrush.Color = borderColor;

            _device.DrawRectangle(new RawRectangleF(x - half, y - half, width + half, height + half), _sharedBrush, half);

            _device.DrawRectangle(new RawRectangleF(x + half, y + half, width - half, height - half), _sharedBrush, half);
        }

        /// <summary>
        /// Bordereds the rectangle.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="stroke">The stroke.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="borderBrush">The border brush.</param>
        public void BorderedRectangle(float x, float y, float width, float height, float stroke, Direct2DBrush brush, Direct2DBrush borderBrush)
        {
            float half = stroke / 2.0f;

            width += x;
            height += y;

            _device.DrawRectangle(new RawRectangleF(x - half, y - half, width + half, height + half), borderBrush, half);

            _device.DrawRectangle(new RawRectangleF(x + half, y + half, width - half, height - half), borderBrush, half);

            _device.DrawRectangle(new RawRectangleF(x, y, width, height), brush, half);
        }

        #endregion Bordered

        #region Geometry

        /// <summary>
        /// Draws the triangle.
        /// </summary>
        /// <param name="a_x">a x.</param>
        /// <param name="a_y">a y.</param>
        /// <param name="b_x">The b x.</param>
        /// <param name="b_y">The b y.</param>
        /// <param name="c_x">The c x.</param>
        /// <param name="c_y">The c y.</param>
        /// <param name="stroke">The stroke.</param>
        /// <param name="brush">The brush.</param>
        public void DrawTriangle(float a_x, float a_y, float b_x, float b_y, float c_x, float c_y, float stroke, Direct2DBrush brush)
        {
            var geometry = new PathGeometry(_factory);

            var sink = geometry.Open();

            sink.BeginFigure(new RawVector2(a_x, a_y), FigureBegin.Hollow);
            sink.AddLine(new RawVector2(b_x, b_y));
            sink.AddLine(new RawVector2(c_x, c_y));
            sink.EndFigure(FigureEnd.Closed);

            sink.Close();

            _device.DrawGeometry(geometry, brush, stroke);

            sink.Dispose();
            geometry.Dispose();
        }

        /// <summary>
        /// Draws the triangle.
        /// </summary>
        /// <param name="a_x">a x.</param>
        /// <param name="a_y">a y.</param>
        /// <param name="b_x">The b x.</param>
        /// <param name="b_y">The b y.</param>
        /// <param name="c_x">The c x.</param>
        /// <param name="c_y">The c y.</param>
        /// <param name="stroke">The stroke.</param>
        /// <param name="color">The color.</param>
        public void DrawTriangle(float a_x, float a_y, float b_x, float b_y, float c_x, float c_y, float stroke, Direct2DColor color)
        {
            _sharedBrush.Color = color;

            var geometry = new PathGeometry(_factory);

            var sink = geometry.Open();

            sink.BeginFigure(new RawVector2(a_x, a_y), FigureBegin.Hollow);
            sink.AddLine(new RawVector2(b_x, b_y));
            sink.AddLine(new RawVector2(c_x, c_y));
            sink.EndFigure(FigureEnd.Closed);

            sink.Close();

            _device.DrawGeometry(geometry, _sharedBrush, stroke);

            sink.Dispose();
            geometry.Dispose();
        }

        /// <summary>
        /// Fills the triangle.
        /// </summary>
        /// <param name="a_x">a x.</param>
        /// <param name="a_y">a y.</param>
        /// <param name="b_x">The b x.</param>
        /// <param name="b_y">The b y.</param>
        /// <param name="c_x">The c x.</param>
        /// <param name="c_y">The c y.</param>
        /// <param name="brush">The brush.</param>
        public void FillTriangle(float a_x, float a_y, float b_x, float b_y, float c_x, float c_y, Direct2DBrush brush)
        {
            var geometry = new PathGeometry(_factory);

            var sink = geometry.Open();

            sink.BeginFigure(new RawVector2(a_x, a_y), FigureBegin.Filled);
            sink.AddLine(new RawVector2(b_x, b_y));
            sink.AddLine(new RawVector2(c_x, c_y));
            sink.EndFigure(FigureEnd.Closed);

            sink.Close();

            _device.FillGeometry(geometry, brush);

            sink.Dispose();
            geometry.Dispose();
        }

        /// <summary>
        /// Fills the triangle.
        /// </summary>
        /// <param name="a_x">a x.</param>
        /// <param name="a_y">a y.</param>
        /// <param name="b_x">The b x.</param>
        /// <param name="b_y">The b y.</param>
        /// <param name="c_x">The c x.</param>
        /// <param name="c_y">The c y.</param>
        /// <param name="color">The color.</param>
        public void FillTriangle(float a_x, float a_y, float b_x, float b_y, float c_x, float c_y, Direct2DColor color)
        {
            _sharedBrush.Color = color;

            var geometry = new PathGeometry(_factory);

            var sink = geometry.Open();

            sink.BeginFigure(new RawVector2(a_x, a_y), FigureBegin.Filled);
            sink.AddLine(new RawVector2(b_x, b_y));
            sink.AddLine(new RawVector2(c_x, c_y));
            sink.EndFigure(FigureEnd.Closed);

            sink.Close();

            _device.FillGeometry(geometry, _sharedBrush);

            sink.Dispose();
            geometry.Dispose();
        }

        #endregion Geometry

        #region Special

        /// <summary>
        /// Draws the arrow line.
        /// </summary>
        /// <param name="start_x">The start x.</param>
        /// <param name="start_y">The start y.</param>
        /// <param name="end_x">The end x.</param>
        /// <param name="end_y">The end y.</param>
        /// <param name="size">The size.</param>
        /// <param name="color">The color.</param>
        public void DrawArrowLine(float start_x, float start_y, float end_x, float end_y, float size, Direct2DColor color)
        {
            float delta_x = end_x >= start_x ? end_x - start_x : start_x - end_x;
            float delta_y = end_y >= start_y ? end_y - start_y : start_y - end_y;

            float length = (float)Math.Sqrt(delta_x * delta_x + delta_y * delta_y);

            float xm = length - size;
            float xn = xm;

            float ym = size;
            float yn = -ym;

            float sin = delta_y / length;
            float cos = delta_x / length;

            float x = xm * cos - ym * sin + end_x;
            ym = xm * sin + ym * cos + end_y;
            xm = x;

            x = xn * cos - yn * sin + end_x;
            yn = xn * sin + yn * cos + end_y;
            xn = x;

            FillTriangle(start_x, start_y, xm, ym, xn, yn, color);
        }

        /// <summary>
        /// Draws the arrow line.
        /// </summary>
        /// <param name="start_x">The start x.</param>
        /// <param name="start_y">The start y.</param>
        /// <param name="end_x">The end x.</param>
        /// <param name="end_y">The end y.</param>
        /// <param name="size">The size.</param>
        /// <param name="brush">The brush.</param>
        public void DrawArrowLine(float start_x, float start_y, float end_x, float end_y, float size, Direct2DBrush brush)
        {
            float delta_x = end_x >= start_x ? end_x - start_x : start_x - end_x;
            float delta_y = end_y >= start_y ? end_y - start_y : start_y - end_y;

            float length = (float)Math.Sqrt(delta_x * delta_x + delta_y * delta_y);

            float xm = length - size;
            float xn = xm;

            float ym = size;
            float yn = -ym;

            float sin = delta_y / length;
            float cos = delta_x / length;

            float x = xm * cos - ym * sin + end_x;
            ym = xm * sin + ym * cos + end_y;
            xm = x;

            x = xn * cos - yn * sin + end_x;
            yn = xn * sin + yn * cos + end_y;
            xn = x;

            FillTriangle(start_x, start_y, xm, ym, xn, yn, brush);
        }

        /// <summary>
        /// Draws the bitmap.
        /// </summary>
        /// <param name="bmp">The BMP.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="opacity">The opacity.</param>
        public void DrawBitmap(Direct2DBitmap bmp, float x, float y, float opacity)
        {
            Bitmap bitmap = bmp;
            _device.DrawBitmap(bitmap, new RawRectangleF(x, y, x + bitmap.PixelSize.Width, y + bitmap.PixelSize.Height), opacity, BitmapInterpolationMode.Linear);
        }

        /// <summary>
        /// Draws the bitmap.
        /// </summary>
        /// <param name="bmp">The BMP.</param>
        /// <param name="opacity">The opacity.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public void DrawBitmap(Direct2DBitmap bmp, float opacity, float x, float y, float width, float height)
        {
            Bitmap bitmap = bmp;
            _device.DrawBitmap(bitmap, new RawRectangleF(x, y, x + width, y + height), opacity, BitmapInterpolationMode.Linear, new RawRectangleF(0, 0, bitmap.PixelSize.Width, bitmap.PixelSize.Height));
        }

        /// <summary>
        /// Draws the box2 d.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="stroke">The stroke.</param>
        /// <param name="interiorColor">Color of the interior.</param>
        /// <param name="color">The color.</param>
        public void DrawBox2D(float x, float y, float width, float height, float stroke, Direct2DColor interiorColor, Direct2DColor color)
        {
            var geometry = new PathGeometry(_factory);

            var sink = geometry.Open();

            sink.BeginFigure(new RawVector2(x, y), FigureBegin.Filled);
            sink.AddLine(new RawVector2(x + width, y));
            sink.AddLine(new RawVector2(x + width, y + height));
            sink.AddLine(new RawVector2(x, y + height));
            sink.EndFigure(FigureEnd.Closed);

            sink.Close();

            _sharedBrush.Color = color;

            _device.DrawGeometry(geometry, _sharedBrush, stroke);

            _sharedBrush.Color = interiorColor;

            _device.FillGeometry(geometry, _sharedBrush);

            sink.Dispose();
            geometry.Dispose();
        }

        /// <summary>
        /// Draws the box2 d.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="stroke">The stroke.</param>
        /// <param name="interiorBrush">The interior brush.</param>
        /// <param name="brush">The brush.</param>
        public void DrawBox2D(float x, float y, float width, float height, float stroke, Direct2DBrush interiorBrush, Direct2DBrush brush)
        {
            var geometry = new PathGeometry(_factory);

            var sink = geometry.Open();

            sink.BeginFigure(new RawVector2(x, y), FigureBegin.Filled);
            sink.AddLine(new RawVector2(x + width, y));
            sink.AddLine(new RawVector2(x + width, y + height));
            sink.AddLine(new RawVector2(x, y + height));
            sink.EndFigure(FigureEnd.Closed);

            sink.Close();

            _device.DrawGeometry(geometry, brush, stroke);

            _device.FillGeometry(geometry, interiorBrush);

            sink.Dispose();
            geometry.Dispose();
        }

        /// <summary>
        /// Draws the crosshair.
        /// </summary>
        /// <param name="style">The style.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="size">The size.</param>
        /// <param name="stroke">The stroke.</param>
        /// <param name="color">The color.</param>
        public void DrawCrosshair(CrosshairStyle style, float x, float y, float size, float stroke, Direct2DColor color)
        {
            _sharedBrush.Color = color;

            if (style == CrosshairStyle.Dot)
            {
                FillCircle(x, y, size, color);
            }
            else if (style == CrosshairStyle.Plus)
            {
                DrawLine(x - size, y, x + size, y, stroke, color);
                DrawLine(x, y - size, x, y + size, stroke, color);
            }
            else if (style == CrosshairStyle.Cross)
            {
                DrawLine(x - size, y - size, x + size, y + size, stroke, color);
                DrawLine(x + size, y - size, x - size, y + size, stroke, color);
            }
            else if (style == CrosshairStyle.Gap)
            {
                DrawLine(x - size - stroke, y, x - stroke, y, stroke, color);
                DrawLine(x + size + stroke, y, x + stroke, y, stroke, color);

                DrawLine(x, y - size - stroke, x, y - stroke, stroke, color);
                DrawLine(x, y + size + stroke, x, y + stroke, stroke, color);
            }
            else if (style == CrosshairStyle.Diagonal)
            {
                DrawLine(x - size, y - size, x + size, y + size, stroke, color);
                DrawLine(x + size, y - size, x - size, y + size, stroke, color);
            }
        }

        /// <summary>
        /// Draws the crosshair.
        /// </summary>
        /// <param name="style">The style.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="size">The size.</param>
        /// <param name="stroke">The stroke.</param>
        /// <param name="brush">The brush.</param>
        public void DrawCrosshair(CrosshairStyle style, float x, float y, float size, float stroke, Direct2DBrush brush)
        {
            if (style == CrosshairStyle.Dot)
            {
                FillCircle(x, y, size, brush);
            }
            else if (style == CrosshairStyle.Plus)
            {
                DrawLine(x - size, y, x + size, y, stroke, brush);
                DrawLine(x, y - size, x, y + size, stroke, brush);
            }
            else if (style == CrosshairStyle.Cross)
            {
                DrawLine(x - size, y - size, x + size, y + size, stroke, brush);
                DrawLine(x + size, y - size, x - size, y + size, stroke, brush);
            }
            else if (style == CrosshairStyle.Gap)
            {
                DrawLine(x - size - stroke, y, x - stroke, y, stroke, brush);
                DrawLine(x + size + stroke, y, x + stroke, y, stroke, brush);

                DrawLine(x, y - size - stroke, x, y - stroke, stroke, brush);
                DrawLine(x, y + size + stroke, x, y + stroke, stroke, brush);
            }
            else if (style == CrosshairStyle.Diagonal)
            {
                DrawLine(x - size, y - size, x + size, y + size, stroke, brush);
                DrawLine(x + size, y - size, x - size, y + size, stroke, brush);
            }
        }

        /// <summary>
        /// Draws the horizontal bar.
        /// </summary>
        /// <param name="percentage">The percentage.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="stroke">The stroke.</param>
        /// <param name="interiorColor">Color of the interior.</param>
        /// <param name="color">The color.</param>
        public void DrawHorizontalBar(float percentage, float x, float y, float width, float height, float stroke, Direct2DColor interiorColor, Direct2DColor color)
        {
            float half = stroke / 2.0f;

            _sharedBrush.Color = color;

            var rect = new RawRectangleF(x - half, y - half, x + width + half, y + height + half);

            _device.DrawRectangle(rect, _sharedBrush, stroke);

            if (percentage == 0.0f) return;

            rect.Left += half;
            rect.Right -= half;
            rect.Top += height - (height / 100.0f * percentage) + half;
            rect.Bottom -= half;

            _sharedBrush.Color = interiorColor;

            _device.FillRectangle(rect, _sharedBrush);
        }

        /// <summary>
        /// Draws the horizontal bar.
        /// </summary>
        /// <param name="percentage">The percentage.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="stroke">The stroke.</param>
        /// <param name="interiorBrush">The interior brush.</param>
        /// <param name="brush">The brush.</param>
        public void DrawHorizontalBar(float percentage, float x, float y, float width, float height, float stroke, Direct2DBrush interiorBrush, Direct2DBrush brush)
        {
            float half = stroke / 2.0f;
            float quarter = half / 2.0f;

            var rect = new RawRectangleF(x - half, y - half, x + width + half, y + height + half);

            _device.DrawRectangle(rect, brush, half);

            if (percentage == 0.0f) return;

            rect.Left += quarter;
            rect.Right -= quarter;
            rect.Top += height - (height / 100.0f * percentage) + quarter;
            rect.Bottom -= quarter;

            _device.FillRectangle(rect, interiorBrush);
        }

        /// <summary>
        /// Draws the vertical bar.
        /// </summary>
        /// <param name="percentage">The percentage.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="stroke">The stroke.</param>
        /// <param name="interiorColor">Color of the interior.</param>
        /// <param name="color">The color.</param>
        public void DrawVerticalBar(float percentage, float x, float y, float width, float height, float stroke, Direct2DColor interiorColor, Direct2DColor color)
        {
            float half = stroke / 2.0f;
            float quarter = half / 2.0f;

            _sharedBrush.Color = color;

            var rect = new RawRectangleF(x - half, y - half, x + width + half, y + height + half);

            _device.DrawRectangle(rect, _sharedBrush, half);

            if (percentage == 0.0f) return;

            rect.Left += quarter;
            rect.Right -= width - (width / 100.0f * percentage) + quarter;
            rect.Top += quarter;
            rect.Bottom -= quarter;

            _sharedBrush.Color = interiorColor;

            _device.FillRectangle(rect, _sharedBrush);
        }

        /// <summary>
        /// Draws the vertical bar.
        /// </summary>
        /// <param name="percentage">The percentage.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="stroke">The stroke.</param>
        /// <param name="interiorBrush">The interior brush.</param>
        /// <param name="brush">The brush.</param>
        public void DrawVerticalBar(float percentage, float x, float y, float width, float height, float stroke, Direct2DBrush interiorBrush, Direct2DBrush brush)
        {
            float half = stroke / 2.0f;
            float quarter = half / 2.0f;

            var rect = new RawRectangleF(x - half, y - half, x + width + half, y + height + half);

            _device.DrawRectangle(rect, brush, half);

            if (percentage == 0.0f) return;

            rect.Left += quarter;
            rect.Right -= width - (width / 100.0f * percentage) + quarter;
            rect.Top += quarter;
            rect.Bottom -= quarter;

            _device.FillRectangle(rect, interiorBrush);
        }

        #endregion Special

        #region Text

        /// <summary>
        /// Draws the text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="font">The font.</param>
        /// <param name="color">The color.</param>
        public void DrawText(string text, float x, float y, Direct2DFont font, Direct2DColor color)
        {
            _sharedBrush.Color = color;
            _device.DrawText(text, text.Length, font, new RawRectangleF(x, y, float.MaxValue, float.MaxValue), _sharedBrush, DrawTextOptions.NoSnap, MeasuringMode.Natural);
        }

        /// <summary>
        /// Draws the text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="font">The font.</param>
        /// <param name="brush">The brush.</param>
        public void DrawText(string text, float x, float y, Direct2DFont font, Direct2DBrush brush)
        {
            _device.DrawText(text, text.Length, font, new RawRectangleF(x, y, float.MaxValue, float.MaxValue), brush, DrawTextOptions.NoSnap, MeasuringMode.Natural);
        }

        /// <summary>
        /// Draws the text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="fontSize">Size of the font.</param>
        /// <param name="font">The font.</param>
        /// <param name="color">The color.</param>
        public void DrawText(string text, float x, float y, float fontSize, Direct2DFont font, Direct2DColor color)
        {
            _sharedBrush.Color = color;

            var layout = new TextLayout(_fontFactory, text, font, float.MaxValue, float.MaxValue);

            layout.SetFontSize(fontSize, new TextRange(0, text.Length));

            _device.DrawTextLayout(new RawVector2(x, y), layout, _sharedBrush, DrawTextOptions.NoSnap);

            layout.Dispose();
        }

        /// <summary>
        /// Draws the text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="fontSize">Size of the font.</param>
        /// <param name="font">The font.</param>
        /// <param name="brush">The brush.</param>
        public void DrawText(string text, float x, float y, float fontSize, Direct2DFont font, Direct2DBrush brush)
        {
            var layout = new TextLayout(_fontFactory, text, font, float.MaxValue, float.MaxValue);

            layout.SetFontSize(fontSize, new TextRange(0, text.Length));

            _device.DrawTextLayout(new RawVector2(x, y), layout, brush, DrawTextOptions.NoSnap);

            layout.Dispose();
        }

        /// <summary>
        /// Draws the text with background.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="font">The font.</param>
        /// <param name="color">The color.</param>
        /// <param name="backgroundColor">Color of the background.</param>
        public void DrawTextWithBackground(string text, float x, float y, Direct2DFont font, Direct2DColor color, Direct2DColor backgroundColor)
        {
            var layout = new TextLayout(_fontFactory, text, font, float.MaxValue, float.MaxValue);

            float modifier = layout.FontSize / 4.0f;

            _sharedBrush.Color = backgroundColor;

            _device.FillRectangle(new RawRectangleF(x - modifier, y - modifier, x + layout.Metrics.Width + modifier, y + layout.Metrics.Height + modifier), _sharedBrush);

            _sharedBrush.Color = color;

            _device.DrawTextLayout(new RawVector2(x, y), layout, _sharedBrush, DrawTextOptions.NoSnap);

            layout.Dispose();
        }

        /// <summary>
        /// Draws the text with background.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="font">The font.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="backgroundBrush">The background brush.</param>
        public void DrawTextWithBackground(string text, float x, float y, Direct2DFont font, Direct2DBrush brush, Direct2DBrush backgroundBrush)
        {
            var layout = new TextLayout(_fontFactory, text, font, float.MaxValue, float.MaxValue);

            float modifier = layout.FontSize / 4.0f;

            _device.FillRectangle(new RawRectangleF(x - modifier, y - modifier, x + layout.Metrics.Width + modifier, y + layout.Metrics.Height + modifier), backgroundBrush);

            _device.DrawTextLayout(new RawVector2(x, y), layout, brush, DrawTextOptions.NoSnap);

            layout.Dispose();
        }

        /// <summary>
        /// Draws the text with background.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="fontSize">Size of the font.</param>
        /// <param name="font">The font.</param>
        /// <param name="color">The color.</param>
        /// <param name="backgroundColor">Color of the background.</param>
        public void DrawTextWithBackground(string text, float x, float y, float fontSize, Direct2DFont font, Direct2DColor color, Direct2DColor backgroundColor)
        {
            var layout = new TextLayout(_fontFactory, text, font, float.MaxValue, float.MaxValue);

            layout.SetFontSize(fontSize, new TextRange(0, text.Length));

            float modifier = fontSize / 4.0f;

            _sharedBrush.Color = backgroundColor;

            _device.FillRectangle(new RawRectangleF(x - modifier, y - modifier, x + layout.Metrics.Width + modifier, y + layout.Metrics.Height + modifier), _sharedBrush);

            _sharedBrush.Color = color;

            _device.DrawTextLayout(new RawVector2(x, y), layout, _sharedBrush, DrawTextOptions.NoSnap);

            layout.Dispose();
        }

        /// <summary>
        /// Draws the text with background.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="fontSize">Size of the font.</param>
        /// <param name="font">The font.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="backgroundBrush">The background brush.</param>
        public void DrawTextWithBackground(string text, float x, float y, float fontSize, Direct2DFont font, Direct2DBrush brush, Direct2DBrush backgroundBrush)
        {
            var layout = new TextLayout(_fontFactory, text, font, float.MaxValue, float.MaxValue);

            layout.SetFontSize(fontSize, new TextRange(0, text.Length));

            float modifier = fontSize / 4.0f;

            _device.FillRectangle(new RawRectangleF(x - modifier, y - modifier, x + layout.Metrics.Width + modifier, y + layout.Metrics.Height + modifier), backgroundBrush);

            _device.DrawTextLayout(new RawVector2(x, y), layout, brush, DrawTextOptions.NoSnap);

            layout.Dispose();
        }

        #endregion Text

        #region IDisposable Support

        /// <summary>
        /// The disposed value
        /// </summary>
        private bool disposedValue = false;

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        /// <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
        /// unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // Free managed objects
                }

                DestroyInstance();

                disposedValue = true;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting
        /// unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable Support
    }
}