using System;
using System.Diagnostics;

using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;

using FontFactory = SharpDX.DirectWrite.Factory;
using Factory = SharpDX.Direct2D1.Factory;

using GameOverlay.Graphics.Containers;
using GameOverlay.PInvoke;
using GameOverlay.Utilities;

namespace GameOverlay.Graphics
{
    /// <summary>
    /// Represents a drawing device of a window
    /// </summary>
    /// <seealso cref="System.IDisposable"/>
    public class D2DDevice : IDisposable
    {
        #region private vars

        private WindowRenderTarget _device;
        private HwndRenderTargetProperties _deviceProperties;
        private Factory _factory;
        private FontFactory _fontFactory;
        private int _internalFps;
        private DeviceOptions _deviceOptions;
        private bool _resize;
        private int _resizeHeight;
        private int _resizeWidth;
        private StrokeStyle _sharedStrokeStyle;
        private Stopwatch _stopwatch = new Stopwatch();

        #endregion private vars

        #region public vars

        /// <summary>
        /// Gets a value indicating whether this instance is initialized.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is initialized; otherwise, <c>false</c>.
        /// </value>
        public bool IsInitialized { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is drawing.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is drawing; otherwise, <c>false</c>.
        /// </value>
        public bool IsDrawing { get; private set; }

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

        private D2DDevice()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="D2DDevice"/> class.
        /// </summary>
        /// <param name="hwnd">A valid window handle</param>
        public D2DDevice(IntPtr hwnd)
        {
            if (hwnd == IntPtr.Zero) throw new ArgumentException(nameof(D2DDevice) + " needs a valid window handle!", nameof(hwnd));

            var options = new DeviceOptions()
            {
                Hwnd = hwnd,
                VSync = false,
                MeasureFps = false,
                AntiAliasing = false
            };
            SetupInstance(options);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="D2DDevice"/> class.
        /// </summary>
        /// <param name="hwnd">A valid window handle</param>
        /// <param name="vsync">if set to <c>true</c> [vsync].</param>
        public D2DDevice(IntPtr hwnd, bool vsync)
        {
            if (hwnd == IntPtr.Zero) throw new ArgumentException(nameof(D2DDevice) + " needs a valid window handle!", nameof(hwnd));

            var options = new DeviceOptions()
            {
                Hwnd = hwnd,
                VSync = vsync,
                MeasureFps = false,
                AntiAliasing = false
            };
            SetupInstance(options);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="D2DDevice"/> class.
        /// </summary>
        /// <param name="hwnd">A valid window handle</param>
        /// <param name="vsync">if set to <c>true</c> [vsync].</param>
        /// <param name="measureFps">if set to <c>true</c> [measure FPS].</param>
        public D2DDevice(IntPtr hwnd, bool vsync, bool measureFps)
        {
            if (hwnd == IntPtr.Zero) throw new ArgumentException(nameof(D2DDevice) + " needs a valid window handle!", nameof(hwnd));

            var options = new DeviceOptions()
            {
                Hwnd = hwnd,
                VSync = vsync,
                MeasureFps = measureFps,
                AntiAliasing = false
            };
            SetupInstance(options);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="D2DDevice"/> class.
        /// </summary>
        /// <param name="hwnd">A valid window handle</param>
        /// <param name="vsync">if set to <c>true</c> [vsync].</param>
        /// <param name="measureFps">if set to <c>true</c> [measure FPS].</param>
        /// <param name="antiAliasing">if set to <c>true</c> [anti aliasing].</param>
        public D2DDevice(IntPtr hwnd, bool vsync, bool measureFps, bool antiAliasing)
        {
            if (hwnd == IntPtr.Zero) throw new ArgumentException(nameof(D2DDevice) + " needs a valid window handle!", nameof(hwnd));

            var options = new DeviceOptions()
            {
                Hwnd = hwnd,
                VSync = vsync,
                MeasureFps = measureFps,
                AntiAliasing = antiAliasing
            };
            SetupInstance(options);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="D2DDevice"/> class.
        /// </summary>
        /// <param name="options">Creation options</param>
        public D2DDevice(DeviceOptions options)
        {
            if (options.Hwnd == IntPtr.Zero) throw new ArgumentException(nameof(D2DDevice) + " needs a valid window handle!", nameof(options.Hwnd));

            SetupInstance(options);
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="D2DDevice"/> class.
        /// </summary>
        ~D2DDevice()
        {
            Dispose(false);
        }

        #endregion construct & destruct

        #region init & delete

        private void DestroyInstance()
        {
            if (!IsInitialized) throw new InvalidOperationException("Can not destroy an unitiliazed " + nameof(D2DDevice) + "!");

            IsInitialized = false;

            try
            {
                _fontFactory.Dispose();
                _factory.Dispose();
                _device.Dispose();
            }
            catch
            {
            }
        }

        private void SetupInstance(DeviceOptions options)
        {
            if (IsInitialized) throw new InvalidOperationException("Destroy this " + nameof(D2DDevice) + " before initializing it again!");

            _deviceOptions = options;

            if (options.Hwnd == IntPtr.Zero) throw new ArgumentNullException(nameof(options.Hwnd));

            if (User32.IsWindow(options.Hwnd) == 0) throw new ArgumentException("The window does not exist (hwnd = 0x" + options.Hwnd.ToString("X") + ")");

            RECT bounds = new RECT();

            if (!HelperMethods.GetWindowClientRect(options.Hwnd, out bounds)) throw new Exception("Failed to get the size of the given window (hwnd = 0x" + options.Hwnd.ToString("X") + ")");

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
                try
                {
                    renderProperties.PixelFormat = new PixelFormat(Format.B8G8R8A8_UNorm, SharpDX.Direct2D1.AlphaMode.Premultiplied);
                    _device = new WindowRenderTarget(_factory, renderProperties, _deviceProperties);
                }
                catch (Exception ex)
                {
                    throw new NotSupportedException("This computer does not support Direct2D1!" + Environment.NewLine + ex.ToString());
                }
            }

            _device.AntialiasMode = AntialiasMode.PerPrimitive; // AntialiasMode.PerPrimitive fails rendering some objects
            // other than in the documentation: Cleartype is much faster for me than GrayScale
            _device.TextAntialiasMode = options.AntiAliasing ? SharpDX.Direct2D1.TextAntialiasMode.Cleartype : SharpDX.Direct2D1.TextAntialiasMode.Aliased;

            _sharedStrokeStyle = new StrokeStyle(_factory, new StrokeStyleProperties()
            {
                DashCap = CapStyle.Flat,
                DashOffset = -1.0f,
                DashStyle = DashStyle.Dash,
                EndCap = CapStyle.Flat,
                LineJoin = LineJoin.MiterOrBevel,
                MiterLimit = 1.0f,
                StartCap = CapStyle.Flat
            });

            IsInitialized = true;
        }

        #endregion init & delete

        #region Scenes

        /// <summary>
        /// Begins a new scene
        /// </summary>
        public void BeginScene()
        {
            if (_device == null) throw new InvalidOperationException("The DirectX device is not initialized");
            if (!IsInitialized) throw new InvalidOperationException("The " + nameof(D2DDevice) + " hasn't finished initialization!");
            if (IsDrawing) return;

            if (MeasureFPS && !_stopwatch.IsRunning)
            {
                _stopwatch.Restart();
            }

            if (_resize)
            {
                try
                {
                    _device.Resize(new Size2(_resizeWidth, _resizeHeight));
                    _resize = false;
                    Width = _resizeWidth;
                    Height = _resizeHeight;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString()); // catches a weird error here
                }
            }

            _device.BeginDraw();

            IsDrawing = true;
        }

        /// <summary>
        /// Clears the scene. Transparent
        /// </summary>
        public void ClearScene()
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene or UseScene before ClearScene!");

            _device.Clear(null);
        }

        /// <summary>
        /// Clears the scene with background color
        /// </summary>
        /// <param name="color">The color</param>
        public void ClearScene(D2DColor color)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene or UseScene before ClearScene!");

            _device.Clear(color);
        }

        /// <summary>
        /// Clears the scene with background color
        /// </summary>
        /// <param name="brush">The brush</param>
        public void ClearScene(D2DSolidColorBrush brush)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene or UseScene before ClearScene!");

            _device.Clear(brush.Color);
        }

        /// <summary>
        /// Ends the scene
        /// </summary>
        public void EndScene()
        {
            if (_device == null) throw new InvalidOperationException("The DirectX device is not initialized");
            if (!IsInitialized) throw new InvalidOperationException("The " + nameof(D2DDevice) + " hasn't finished initialization!");
            if (!IsDrawing) return;

            var result = _device.TryEndDraw(out long tag_0, out long tag_1);

            if (result.Failure)
            {
                DestroyInstance();
                SetupInstance(_deviceOptions);
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

            IsDrawing = false;
        }

        /// <summary>
        /// Resizes the renderer (call this when the window has changed it's size)
        /// </summary>
        /// <param name="width">The width</param>
        /// <param name="height">The height</param>
        /// <returns>true if renderer state is okay</returns>
        public bool Resize(int width, int height)
        {
            if (!IsInitialized) return false;

            if (Width == width && height == Height) return true;

            _resizeWidth = width;
            _resizeHeight = height;
            _resize = true;

            return true;
        }

        /// <summary>
        /// Fancy IDisposable pattern for scenes
        /// </summary>
        /// <returns></returns>
        public D2DScene UseScene()
        {
            if (_device == null) throw new InvalidOperationException("The DirectX device is not initialized");
            if (!IsInitialized) throw new InvalidOperationException("The " + nameof(D2DDevice) + " hasn't finished initialization!");

            return new D2DScene(this);
        }

        #endregion Scenes

        #region Fonts & Brushes & Bitmaps

        /// <summary>
        /// Creates a new SolidColorBrush
        /// </summary>
        /// <param name="color">The color</param>
        /// <returns></returns>
        public D2DSolidColorBrush CreateSolidColorBrush(D2DColor color)
        {
            if (_device == null) throw new InvalidOperationException("The DirectX device is not initialized");
            if (!IsInitialized) throw new InvalidOperationException("The " + nameof(D2DDevice) + " hasn't finished initialization!");

            return new D2DSolidColorBrush(_device, color);
        }

        /// <summary>
        /// Creates a new SolidColorBrush
        /// </summary>
        /// <param name="r">Red 0 - 255</param>
        /// <param name="g">Green 0 - 255</param>
        /// <param name="b">Blue 0 - 255</param>
        /// <param name="a">Alpha 0 - 255</param>
        /// <returns></returns>
        public D2DSolidColorBrush CreateSolidColorBrush(int r, int g, int b, int a = 255)
        {
            if (_device == null) throw new InvalidOperationException("The DirectX device is not initialized");
            if (!IsInitialized) throw new InvalidOperationException("The " + nameof(D2DDevice) + " hasn't finished initialization!");

            return new D2DSolidColorBrush(_device, new D2DColor(r, g, b, a));
        }

        /// <summary>
        /// Creates a new SolidColorBrush
        /// </summary>
        /// <param name="r">Red 0.0f - 1.0f</param>
        /// <param name="g">Green 0.0f - 1.0f</param>
        /// <param name="b">Blue 0.0f - 1.0f</param>
        /// <param name="a">Alpha 0.0f - 1.0f</param>
        /// <returns></returns>
        public D2DSolidColorBrush CreateSolidColorBrush(float r, float g, float b, float a = 1.0f)
        {
            if (_device == null) throw new InvalidOperationException("The DirectX device is not initialized");
            if (!IsInitialized) throw new InvalidOperationException("The " + nameof(D2DDevice) + " hasn't finished initialization!");

            return new D2DSolidColorBrush(_device, new D2DColor(r, g, b, a));
        }

        /// <summary>
        /// Creates a new font. Used for drawing text
        /// </summary>
        /// <param name="fontFamilyName">Name of the font family.</param>
        /// <param name="size">The size.</param>
        /// <param name="bold">if set to <c>true</c> [bold].</param>
        /// <param name="italic">if set to <c>true</c> [italic].</param>
        /// <returns></returns>
        public D2DFont CreateFont(string fontFamilyName, float size, bool bold = false, bool italic = false)
        {
            if (_device == null) throw new InvalidOperationException("The DirectX device is not initialized");
            if (!IsInitialized) throw new InvalidOperationException("The " + nameof(D2DDevice) + " hasn't finished initialization!");

            return new D2DFont(_fontFactory, fontFamilyName, size, bold, italic);
        }

        /// <summary>
        /// Creates a new font. Used for drawing text
        /// </summary>
        /// <param name="options">Creation options</param>
        /// <returns></returns>
        public D2DFont CreateFont(FontOptions options)
        {
            if (_device == null) throw new InvalidOperationException("The DirectX device is not initialized");
            if (!IsInitialized) throw new InvalidOperationException("The " + nameof(D2DDevice) + " hasn't finished initialization!");

            TextFormat font = new TextFormat(_fontFactory, options.FontFamilyName, options.Bold ? FontWeight.Bold : FontWeight.Normal, options.GetStyle(), options.FontSize);
            return new D2DFont(font);
        }

        /// <summary>
        /// Loads a bitmap from a file
        /// </summary>
        /// <param name="file">Path</param>
        /// <returns></returns>
        public D2DImage LoadBitmap(string file)
        {
            if (_device == null) throw new InvalidOperationException("The DirectX device is not initialized");
            if (!IsInitialized) throw new InvalidOperationException("The " + nameof(D2DDevice) + " hasn't finished initialization!");

            return new D2DImage(_device, file);
        }

        /// <summary>
        /// Loads a bitmap from a <c>byte[]</c>
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <returns></returns>
        public D2DImage LoadBitmap(byte[] bytes)
        {
            if (_device == null) throw new InvalidOperationException("The DirectX device is not initialized");
            if (!IsInitialized) throw new InvalidOperationException("The " + nameof(D2DDevice) + " hasn't finished initialization!");

            return new D2DImage(_device, bytes);
        }

        #endregion Fonts & Brushes & Bitmaps
        
        #region Primitives

        public void DrawCircle(Primitives.Circle circle, float stroke, ID2DBrush brush)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing any primitives!");

            _device.DrawEllipse(circle, brush.GetBrush(), stroke);
        }

        public void DrawCircle(float x, float y, float radius, float stroke, ID2DBrush brush)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing any primitives!");

            _device.DrawEllipse(new Ellipse(new RawVector2(x, y), radius, radius), brush.GetBrush(), stroke);
        }

        public void DrawDashedCircle(Primitives.Circle circle, float stroke, ID2DBrush brush)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing any primitives!");

            _device.DrawEllipse(circle, brush.GetBrush(), stroke, _sharedStrokeStyle);
        }

        public void DrawDashedCircle(float x, float y, float radius, float stroke, ID2DBrush brush)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing any primitives!");

            _device.DrawEllipse(new Ellipse(new RawVector2(x, y), radius, radius), brush.GetBrush(), stroke, _sharedStrokeStyle);
        }

        public void FillCircle(Primitives.Circle circle, ID2DBrush brush)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing any primitives!");

            _device.FillEllipse(circle, brush.GetBrush());
        }

        public void FillCircle(float x, float y, float radius, ID2DBrush brush)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing any primitives!");

            _device.FillEllipse(new Ellipse(new RawVector2(x, y), radius, radius), brush.GetBrush());
        }

        public void DrawEllipse(Primitives.Ellipse ellipse, float stroke, ID2DBrush brush)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing any primitives!");

            _device.DrawEllipse(ellipse, brush.GetBrush(), stroke);
        }

        public void DrawEllipse(float x, float y, float radius_x, float radius_y, float stroke, ID2DBrush brush)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing any primitives!");

            _device.DrawEllipse(new Ellipse(new RawVector2(x, y), radius_x, radius_y), brush.GetBrush(), stroke);
        }

        public void DrawDashedEllipse(Primitives.Ellipse ellipse, float stroke, ID2DBrush brush)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing any primitives!");

            _device.DrawEllipse(ellipse, brush.GetBrush(), stroke, _sharedStrokeStyle);
        }

        public void DrawDashedEllipse(float x, float y, float radius_x, float radius_y, float stroke, ID2DBrush brush)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing any primitives!");

            _device.DrawEllipse(new Ellipse(new RawVector2(x, y), radius_x, radius_y), brush.GetBrush(), stroke, _sharedStrokeStyle);
        }

        public void FillEllipse(Primitives.Ellipse ellipse, ID2DBrush brush)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing any primitives!");

            _device.FillEllipse(ellipse, brush.GetBrush());
        }

        public void FillEllipse(float x, float y, float radius_x, float radius_y, ID2DBrush brush)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing any primitives!");

            _device.FillEllipse(new Ellipse(new RawVector2(x, y), radius_x, radius_y), brush.GetBrush());
        }

        public void DrawLine(Primitives.Line line, float stroke, ID2DBrush brush)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing any primitives!");

            _device.DrawLine(line.Start, line.End, brush.GetBrush(), stroke);
        }

        public void DrawLine(float start_x, float start_y, float end_x, float end_y, float stroke, ID2DBrush brush)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing any primitives!");

            _device.DrawLine(new RawVector2(start_x, start_y), new RawVector2(end_x, end_y), brush.GetBrush(), stroke);
        }

        public void DrawDashedLine(Primitives.Line line, float stroke, ID2DBrush brush)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing any primitives!");

            _device.DrawLine(line.Start, line.End, brush.GetBrush(), stroke, _sharedStrokeStyle);
        }

        public void DrawDashedLine(float start_x, float start_y, float end_x, float end_y, float stroke, ID2DBrush brush)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing any primitives!");

            _device.DrawLine(new RawVector2(start_x, start_y), new RawVector2(end_x, end_y), brush.GetBrush(), stroke, _sharedStrokeStyle);
        }

        public void DrawRectangle(Primitives.Rectangle rectangle, float stroke, ID2DBrush brush)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing any primitives!");

            _device.DrawRectangle(rectangle, brush.GetBrush(), stroke);
        }

        public void DrawRectangle(float left, float top, float right, float bottom, float stroke, ID2DBrush brush)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing any primitives!");

            _device.DrawRectangle(new RawRectangleF(left, top, right, bottom), brush.GetBrush(), stroke);
        }

        public void DrawDashedRectangle(Primitives.Rectangle rectangle, float stroke, ID2DBrush brush)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing any primitives!");

            _device.DrawRectangle(rectangle, brush.GetBrush(), stroke, _sharedStrokeStyle);
        }

        public void DrawDashedRectangle(float left, float top, float right, float bottom, float stroke, ID2DBrush brush)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing any primitives!");

            _device.DrawRectangle(new RawRectangleF(left, top, right, bottom), brush.GetBrush(), stroke, _sharedStrokeStyle);
        }

        public void FillRectangle(Primitives.Rectangle rectangle, ID2DBrush brush)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing any primitives!");

            _device.FillRectangle(rectangle, brush.GetBrush());
        }

        public void FillRectangle(float left, float top, float right, float bottom, ID2DBrush brush)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing any primitives!");

            _device.FillRectangle(new RawRectangleF(left, top, right, bottom), brush.GetBrush());
        }

        public void DrawRoundedRectangle(Primitives.RoundedRectangle rectangle, float stroke, ID2DBrush brush)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing any primitives!");

            _device.DrawRoundedRectangle(rectangle, brush.GetBrush(), stroke);
        }

        public void DrawRoundedRectangle(float left, float top, float right, float bottom, float radius_x, float radius_y, float stroke, ID2DBrush brush)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing any primitives!");

            _device.DrawRoundedRectangle(new RoundedRectangle()
            {
                RadiusX = radius_x,
                RadiusY = radius_y,
                Rect = new RawRectangleF(left, top, right, bottom)
            }
            , brush.GetBrush(), stroke);
        }

        public void DrawDashedRoundedRectangle(Primitives.RoundedRectangle rectangle, ID2DBrush brush, float stroke)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing any primitives!");

            _device.DrawRoundedRectangle(rectangle, brush.GetBrush(), stroke, _sharedStrokeStyle);
        }

        public void DrawDashedRoundedRectangle(float left, float top, float right, float bottom, float radius_x, float radius_y, float stroke, ID2DBrush brush)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing any primitives!");

            _device.DrawRoundedRectangle(new RoundedRectangle()
            {
                RadiusX = radius_x,
                RadiusY = radius_y,
                Rect = new RawRectangleF(left, top, right, bottom)
            }
            , brush.GetBrush(), stroke, _sharedStrokeStyle);
        }

        public void FillRoundedRectangle(Primitives.RoundedRectangle rectangle, ID2DBrush brush)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing any primitives!");

            _device.FillRoundedRectangle(rectangle, brush.GetBrush());
        }

        public void FillRoundedRectangle(float left, float top, float right, float bottom, float radius_x, float radius_y, ID2DBrush brush)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing any primitives!");

            _device.FillRoundedRectangle(new RoundedRectangle()
            {
                RadiusX = radius_x,
                RadiusY = radius_y,
                Rect = new RawRectangleF(left, top, right, bottom)
            }
            , brush.GetBrush());
        }

        public void DrawTriangle(Primitives.Triangle triangle, ID2DBrush brush, float stroke)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing any primitives!");

            var geometry = new PathGeometry(_factory);

            var sink = geometry.Open();

            sink.BeginFigure(triangle.A, FigureBegin.Hollow);
            sink.AddLine(triangle.B);
            sink.AddLine(triangle.C);
            sink.EndFigure(FigureEnd.Closed);

            sink.Close();

            _device.DrawGeometry(geometry, brush.GetBrush(), stroke);

            sink.Dispose();
            geometry.Dispose();
        }

        public void DrawTriangle(float a_x, float a_y, float b_x, float b_y, float c_x, float c_y, ID2DBrush brush, float stroke)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing any primitives!");

            var geometry = new PathGeometry(_factory);

            var sink = geometry.Open();

            sink.BeginFigure(new RawVector2(a_x, a_y), FigureBegin.Hollow);
            sink.AddLine(new RawVector2(b_x, b_y));
            sink.AddLine(new RawVector2(c_x, c_y));
            sink.EndFigure(FigureEnd.Closed);

            sink.Close();

            _device.DrawGeometry(geometry, brush.GetBrush(), stroke);

            sink.Dispose();
            geometry.Dispose();
        }

        public void DrawDashedTriangle(Primitives.Triangle triangle, float stroke, ID2DBrush brush)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing any primitives!");

            var geometry = new PathGeometry(_factory);

            var sink = geometry.Open();

            sink.BeginFigure(triangle.A, FigureBegin.Hollow);
            sink.AddLine(triangle.B);
            sink.AddLine(triangle.C);
            sink.EndFigure(FigureEnd.Closed);

            sink.Close();

            _device.DrawGeometry(geometry, brush.GetBrush(), stroke, _sharedStrokeStyle);

            sink.Dispose();
            geometry.Dispose();
        }

        public void DrawDashedTriangle(float a_x, float a_y, float b_x, float b_y, float c_x, float c_y, float stroke, ID2DBrush brush)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing any primitives!");

            var geometry = new PathGeometry(_factory);

            var sink = geometry.Open();

            sink.BeginFigure(new RawVector2(a_x, a_y), FigureBegin.Hollow);
            sink.AddLine(new RawVector2(b_x, b_y));
            sink.AddLine(new RawVector2(c_x, c_y));
            sink.EndFigure(FigureEnd.Closed);

            sink.Close();

            _device.DrawGeometry(geometry, brush.GetBrush(), stroke, _sharedStrokeStyle);

            sink.Dispose();
            geometry.Dispose();
        }

        public void FillTriangle(Primitives.Triangle triangle, ID2DBrush brush)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing any primitives!");

            var geometry = new PathGeometry(_factory);

            var sink = geometry.Open();

            sink.BeginFigure(triangle.A, FigureBegin.Filled);
            sink.AddLine(triangle.B);
            sink.AddLine(triangle.C);
            sink.EndFigure(FigureEnd.Closed);

            sink.Close();

            _device.FillGeometry(geometry, brush.GetBrush());

            sink.Dispose();
            geometry.Dispose();
        }

        public void FillTriangle(float a_x, float a_y, float b_x, float b_y, float c_x, float c_y, ID2DBrush brush)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing any primitives!");

            var geometry = new PathGeometry(_factory);

            var sink = geometry.Open();

            sink.BeginFigure(new RawVector2(a_x, a_y), FigureBegin.Hollow);
            sink.AddLine(new RawVector2(b_x, b_y));
            sink.AddLine(new RawVector2(c_x, c_y));
            sink.EndFigure(FigureEnd.Closed);

            sink.Close();

            _device.FillGeometry(geometry, brush.GetBrush());

            sink.Dispose();
            geometry.Dispose();
        }

        #endregion

        #region Outline

        public void OutlineLine(Primitives.Line line, float stroke, ID2DBrush brush, ID2DBrush outline)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing any primitives!");

            var geometry = new PathGeometry(_factory);

            var sink = geometry.Open();

            float half = stroke / 2.0f;

            sink.BeginFigure(new RawVector2(line.Start.X, line.Start.Y - half), FigureBegin.Filled);

            sink.AddLine(new RawVector2(line.End.X, line.End.Y - half));
            sink.AddLine(new RawVector2(line.End.X, line.End.Y + half));
            sink.AddLine(new RawVector2(line.Start.X, line.Start.Y + half));

            sink.EndFigure(FigureEnd.Closed);

            _device.DrawGeometry(geometry, outline.GetBrush(), half);
            _device.FillGeometry(geometry, brush.GetBrush());

            sink.Dispose();
            geometry.Dispose();
        }

        public void OutlineLine(float start_x, float start_y, float end_x, float end_y, float stroke, ID2DBrush brush, ID2DBrush outline)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing any primitives!");

            var geometry = new PathGeometry(_factory);

            var sink = geometry.Open();

            float half = stroke / 2.0f;

            sink.BeginFigure(new RawVector2(start_x, start_y - half), FigureBegin.Filled);

            sink.AddLine(new RawVector2(end_x, end_y - half));
            sink.AddLine(new RawVector2(end_x, end_y + half));
            sink.AddLine(new RawVector2(start_x, start_y + half));

            sink.EndFigure(FigureEnd.Closed);

            _device.DrawGeometry(geometry, outline.GetBrush(), half);
            _device.FillGeometry(geometry, brush.GetBrush());

            sink.Dispose();
            geometry.Dispose();
        }

        public void OutlineCircle(Primitives.Circle circle, float stroke, ID2DBrush brush, ID2DBrush outline)
        {
            OutlineCircle(circle.Location.X, circle.Location.Y, circle.Radius, stroke, brush, outline);
        }

        public void OutlineCircle(float x, float y, float radius, float stroke, ID2DBrush brush, ID2DBrush outline)
        {
            var ellipse = new Ellipse(new RawVector2(x, y), radius, radius);

            _device.DrawEllipse(ellipse, brush.GetBrush(), stroke);

            float half = stroke / 2.0f;

            ellipse.RadiusX += half;
            ellipse.RadiusY += half;

            _device.DrawEllipse(ellipse, outline.GetBrush(), half);

            ellipse.RadiusX -= stroke;
            ellipse.RadiusY -= stroke;

            _device.DrawEllipse(ellipse, outline.GetBrush(), half);
        }

        public void OutlineRectangle(Primitives.Rectangle rectangle , float stroke, ID2DBrush brush, ID2DBrush outline)
        {
            OutlineRectangle(rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom, stroke, brush, outline);
        }

        public void OutlineRectangle(float left, float top, float right, float bottom, float stroke, ID2DBrush brush, ID2DBrush outline)
        {
            float half = stroke / 2.0f;

            float width = right;
            float height = bottom;

            _device.DrawRectangle(new RawRectangleF(left - half, top - half, width + half, height + half), outline.GetBrush(), half);

            _device.DrawRectangle(new RawRectangleF(left + half, top + half, width - half, height - half), outline.GetBrush(), half);

            _device.DrawRectangle(new RawRectangleF(left, top, width, height), brush.GetBrush(), half);
        }

        public void OutlineFillCircle(Primitives.Circle circle, float stroke, ID2DBrush brush, ID2DBrush outline)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing any primitives!");

            EllipseGeometry ellipseGeometry = new EllipseGeometry(_factory, circle);

            PathGeometry geometry = new PathGeometry(_factory);

            var sink = geometry.Open();

            ellipseGeometry.Outline(sink);

            sink.Close();

            _device.FillGeometry(geometry, brush.GetBrush());
            _device.DrawGeometry(geometry, outline.GetBrush(), stroke);

            sink.Dispose();
            geometry.Dispose();
            ellipseGeometry.Dispose();
        }

        public void OutlineFillCircle(float x, float y, float radius, float stroke, ID2DBrush brush, ID2DBrush outline)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing any primitives!");

            EllipseGeometry ellipseGeometry = new EllipseGeometry(_factory, new Ellipse(new RawVector2(x, y), radius, radius));

            PathGeometry geometry = new PathGeometry(_factory);

            var sink = geometry.Open();

            ellipseGeometry.Outline(sink);

            sink.Close();

            _device.FillGeometry(geometry, brush.GetBrush());
            _device.DrawGeometry(geometry, outline.GetBrush(), stroke);

            sink.Dispose();
            geometry.Dispose();
            ellipseGeometry.Dispose();
        }

        public void OutlineFillRectangle(Primitives.Rectangle rectangle, float stroke, ID2DBrush brush, ID2DBrush outline)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing any primitives!");

            RectangleGeometry rectangleGeometry = new RectangleGeometry(_factory, rectangle);

            PathGeometry geometry = new PathGeometry(_factory);

            var sink = geometry.Open();

            rectangleGeometry.Widen(stroke, sink);
            rectangleGeometry.Outline(sink);

            sink.Close();

            _device.FillGeometry(geometry, brush.GetBrush());
            _device.DrawGeometry(geometry, outline.GetBrush(), stroke);

            sink.Dispose();
            geometry.Dispose();
            rectangleGeometry.Dispose();
        }

        public void OutlineFillRectangle(float left, float top, float right, float bottom, float stroke, ID2DBrush brush, ID2DBrush outline)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing any primitives!");

            RectangleGeometry rectangleGeometry = new RectangleGeometry(_factory, new RawRectangleF(left, top, right, bottom));

            PathGeometry geometry = new PathGeometry(_factory);

            var sink = geometry.Open();

            rectangleGeometry.Widen(stroke, sink);
            rectangleGeometry.Outline(sink);

            sink.Close();

            _device.FillGeometry(geometry, brush.GetBrush());
            _device.DrawGeometry(geometry, outline.GetBrush(), stroke);

            sink.Dispose();
            geometry.Dispose();
            rectangleGeometry.Dispose();
        }

        #endregion

        #region Special

        public void DrawBarH(Primitives.Rectangle rectangle, float percentage, float stroke, ID2DBrush outline, ID2DBrush fill)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing any primitives!");

            float half = stroke / 2.0f;
            float quarter = half / 2.0f;

            float width = rectangle.Right - rectangle.Left;
            float height = rectangle.Bottom - rectangle.Top;

            var rect = new RawRectangleF(rectangle.Left - half, rectangle.Top - half, rectangle.Left + width + half, rectangle.Top + height + half);

            _device.DrawRectangle(rect, outline.GetBrush(), half);

            if (percentage == 0.0f) return;

            rect.Left += quarter;
            rect.Right -= quarter;
            rect.Top += height - (height / 100.0f * percentage) + quarter;
            rect.Bottom -= quarter;

            _device.FillRectangle(rect, fill.GetBrush());
        }

        public void DrawBarH(float left, float top, float right, float bottom, float percentage, float stroke, ID2DBrush outline, ID2DBrush fill)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing any primitives!");

            float half = stroke / 2.0f;
            float quarter = half / 2.0f;

            float width = right - left;
            float height = bottom - top;

            var rect = new RawRectangleF(left - half, top - half, left + width + half, top + height + half);

            _device.DrawRectangle(rect, outline.GetBrush(), half);

            if (percentage == 0.0f) return;

            rect.Left += quarter;
            rect.Right -= quarter;
            rect.Top += height - (height / 100.0f * percentage) + quarter;
            rect.Bottom -= quarter;

            _device.FillRectangle(rect, fill.GetBrush());
        }

        public void DrawBarV(Primitives.Rectangle rectangle, float percentage, float stroke, ID2DBrush outline, ID2DBrush fill)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing any primitives!");

            float half = stroke / 2.0f;
            float quarter = half / 2.0f;

            float width = rectangle.Right - rectangle.Left;
            float height = rectangle.Bottom - rectangle.Top;

            var rect = new RawRectangleF(rectangle.Left - half, rectangle.Top - half, rectangle.Left + width + half, rectangle.Top + height + half);

            _device.DrawRectangle(rect, outline.GetBrush(), half);

            if (percentage == 0.0f) return;

            rect.Left += quarter;
            rect.Right -= width - (width / 100.0f * percentage) + quarter;
            rect.Top += quarter;
            rect.Bottom -= quarter;

            _device.FillRectangle(rect, fill.GetBrush());
        }

        public void DrawBarV(float left, float top, float right, float bottom, float percentage, float stroke, ID2DBrush outline, ID2DBrush fill)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing any primitives!");

            float half = stroke / 2.0f;
            float quarter = half / 2.0f;

            float width = right - left;
            float height = bottom - top;

            var rect = new RawRectangleF(left - half, top - half, left + width + half, top + height + half);

            _device.DrawRectangle(rect, outline.GetBrush(), half);

            if (percentage == 0.0f) return;

            rect.Left += quarter;
            rect.Right -= width - (width / 100.0f * percentage) + quarter;
            rect.Top += quarter;
            rect.Bottom -= quarter;

            _device.FillRectangle(rect, fill.GetBrush());
        }

        public void DrawCrosshair(CrosshairStyle style, Primitives.Point location, float size, float stroke, ID2DBrush brush)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing any primitives!");

            if (style == CrosshairStyle.Dot)
            {
                FillCircle(location.X, location.Y, size, brush);
            }
            else if (style == CrosshairStyle.Plus)
            {
                DrawLine(location.X - size, location.Y, location.X + size, location.Y, stroke, brush);
                DrawLine(location.X, location.Y - size, location.X, location.Y + size, stroke, brush);
            }
            else if (style == CrosshairStyle.Cross)
            {
                DrawLine(location.X - size, location.Y - size, location.X + size, location.Y + size, stroke, brush);
                DrawLine(location.X + size, location.Y - size, location.X - size, location.Y + size, stroke, brush);
            }
            else if (style == CrosshairStyle.Gap)
            {
                DrawLine(location.X - size - stroke, location.Y, location.X - stroke, location.Y, stroke, brush);
                DrawLine(location.X + size + stroke, location.Y, location.X + stroke, location.Y, stroke, brush);

                DrawLine(location.X, location.Y - size - stroke, location.X, location.Y - stroke, stroke, brush);
                DrawLine(location.X, location.Y + size + stroke, location.X, location.Y + stroke, stroke, brush);
            }
            else if (style == CrosshairStyle.Diagonal)
            {
                DrawLine(location.X - size, location.Y - size, location.X + size, location.Y + size, stroke, brush);
                DrawLine(location.X + size, location.Y - size, location.X - size, location.Y + size, stroke, brush);
            }
        }
        
        public void DrawCrosshair(CrosshairStyle style, float x, float y, float size, float stroke, ID2DBrush brush)
        {
            DrawCrosshair(style, new Primitives.Point(x, y), size, stroke, brush);
        }

        public void DrawArrowLine(Primitives.Point start, Primitives.Point end, float size, ID2DBrush brush)
        {
            DrawArrowLine(start.X, start.Y, end.X, end.Y, size, brush);
        }

        public void DrawArrowLine(float start_x, float start_y, float end_x, float end_y, float size, ID2DBrush brush)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing any primitives!");

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

        public void DrawBox2D(Primitives.Rectangle rectangle, float stroke, ID2DBrush outline, ID2DBrush fill)
        {
            DrawBox2D(rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom, stroke, outline, fill);
        }

        public void DrawBox2D(float left, float top, float right, float bottom, float stroke, ID2DBrush outline, ID2DBrush fill)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing any primitives!");

            float width = right - left;
            float height = bottom - top;

            var geometry = new PathGeometry(_factory);

            var sink = geometry.Open();

            sink.BeginFigure(new RawVector2(left, top), FigureBegin.Filled);
            sink.AddLine(new RawVector2(left + width, top));
            sink.AddLine(new RawVector2(left + width, top + height));
            sink.AddLine(new RawVector2(left, top + height));
            sink.EndFigure(FigureEnd.Closed);

            sink.Close();

            _device.DrawGeometry(geometry, outline.GetBrush(), stroke);

            _device.FillGeometry(geometry, fill.GetBrush());

            sink.Dispose();
            geometry.Dispose();
        }

        public void DrawRectangleEdges(Primitives.Rectangle rectangle, float stroke, ID2DBrush brush)
        {
            DrawRectangleEdges(rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom, stroke, brush);
        }

        public void DrawRectangleEdges(float left, float top, float right, float bottom, float stroke, ID2DBrush brush)
        {
            float width = right - left;
            float height = bottom - top;

            int length = (int)(((width + height) / 2.0f) * 0.2f);

            RawVector2 first = new RawVector2(left, top);
            RawVector2 second = new RawVector2(left, top + length);
            RawVector2 third = new RawVector2(left + length, top);

            _device.DrawLine(first, second, brush.GetBrush(), stroke);
            _device.DrawLine(first, third, brush.GetBrush(), stroke);

            first.Y += height;
            second.Y = first.Y - length;
            third.Y = first.Y;
            third.X = first.X + length;

            _device.DrawLine(first, second, brush.GetBrush(), stroke);
            _device.DrawLine(first, third, brush.GetBrush(), stroke);

            first.X = left + width;
            first.Y = top;
            second.X = first.X - length;
            second.Y = first.Y;
            third.X = first.X;
            third.Y = first.Y + length;

            _device.DrawLine(first, second, brush.GetBrush(), stroke);
            _device.DrawLine(first, third, brush.GetBrush(), stroke);

            first.Y += height;
            second.X += length;
            second.Y = first.Y - length;
            third.Y = first.Y;
            third.X = first.X - length;

            _device.DrawLine(first, second, brush.GetBrush(), stroke);
            _device.DrawLine(first, third, brush.GetBrush(), stroke);
        }

        #endregion

        #region Text

        public void DrawText(string text, Primitives.Point location, D2DFont font, ID2DBrush brush)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing any primitives!");
            if (text == null) throw new ArgumentNullException(nameof(text));

            _device.DrawText(text, text.Length, font, new RawRectangleF(location.X, location.Y, Width - location.X, Height - location.Y), brush.GetBrush(), DrawTextOptions.NoSnap, MeasuringMode.Natural);
        }

        public void DrawText(string text, float x, float y, D2DFont font, ID2DBrush brush)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing any primitives!");
            if (text == null) throw new ArgumentNullException(nameof(text));

            _device.DrawText(text, text.Length, font, new RawRectangleF(x, y, Width - x, Height - y), brush.GetBrush(), DrawTextOptions.NoSnap, MeasuringMode.Natural);
        }

        public void DrawText(string text, Primitives.Point location, D2DFont font, float fontSize, ID2DBrush brush)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing any primitives!");
            if (text == null) throw new ArgumentNullException(nameof(text));

            if (fontSize == font.FontSize)
            {
                _device.DrawText(text, text.Length, font, new RawRectangleF(location.X, location.Y, Width - location.X, Height - location.Y), brush.GetBrush(), DrawTextOptions.NoSnap, MeasuringMode.Natural);
            }
            else
            {
                var layout = new TextLayout(_fontFactory, text, font, Width - location.X, Height - location.Y);

                _device.DrawTextLayout(location, layout, brush.GetBrush(), DrawTextOptions.NoSnap);

                layout.Dispose();
            }
        }

        public void DrawText(string text, float x, float y, D2DFont font, float fontSize, ID2DBrush brush)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing any primitives!");
            if (text == null) throw new ArgumentNullException(nameof(text));

            if (fontSize == font.FontSize)
            {
                _device.DrawText(text, text.Length, font, new RawRectangleF(x, y, Width - x, Height - y), brush.GetBrush(), DrawTextOptions.NoSnap, MeasuringMode.Natural);
            }
            else
            {
                var layout = new TextLayout(_fontFactory, text, font, Width - x, Height - y);

                _device.DrawTextLayout(new RawVector2(x, y), layout, brush.GetBrush(), DrawTextOptions.NoSnap);

                layout.Dispose();
            }
        }

        public void DrawTextWithBackground(string text, Primitives.Point location, D2DFont font, ID2DBrush brush, ID2DBrush background)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing any primitives!");
            if (text == null) throw new ArgumentNullException(nameof(text));

            var layout = new TextLayout(_fontFactory, text, font, Width - location.X, Height - location.Y);

            float modifier = layout.FontSize / 4.0f;

            _device.FillRectangle(new RawRectangleF(location.X - modifier, location.Y - modifier, location.X + layout.Metrics.Width + modifier, location.Y + layout.Metrics.Height + modifier), background.GetBrush());

            _device.DrawTextLayout(location, layout, brush.GetBrush(), DrawTextOptions.NoSnap);

            layout.Dispose();
        }

        public void DrawTextWithBackground(string text, float x, float y, D2DFont font, ID2DBrush brush, ID2DBrush background)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing any primitives!");
            if (text == null) throw new ArgumentNullException(nameof(text));

            var layout = new TextLayout(_fontFactory, text, font, Width - x, Height - y);

            float modifier = layout.FontSize / 4.0f;

            _device.FillRectangle(new RawRectangleF(x - modifier, y - modifier, x + layout.Metrics.Width + modifier, y + layout.Metrics.Height + modifier), background.GetBrush());

            _device.DrawTextLayout(new RawVector2(x, y), layout, brush.GetBrush(), DrawTextOptions.NoSnap);

            layout.Dispose();
        }

        public void DrawTextWithBackground(string text, Primitives.Point location, D2DFont font, float fontSize, ID2DBrush brush, ID2DBrush background)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing any primitives!");
            if (text == null) throw new ArgumentNullException(nameof(text));

            var layout = new TextLayout(_fontFactory, text, font, Width - location.X, Height - location.Y);

            layout.SetFontSize(fontSize, new TextRange(0, text.Length));

            float modifier = layout.FontSize / 4.0f;

            _device.FillRectangle(new RawRectangleF(location.X - modifier, location.Y - modifier, location.X + layout.Metrics.Width + modifier, location.Y + layout.Metrics.Height + modifier), background.GetBrush());

            _device.DrawTextLayout(location, layout, brush.GetBrush(), DrawTextOptions.NoSnap);

            layout.Dispose();
        }

        public void DrawTextWithBackground(string text, float x, float y, D2DFont font, float fontSize, ID2DBrush brush, ID2DBrush background)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing any primitives!");
            if (text == null) throw new ArgumentNullException(nameof(text));

            var layout = new TextLayout(_fontFactory, text, font, Width - x, Height - y);

            layout.SetFontSize(fontSize, new TextRange(0, text.Length));

            float modifier = layout.FontSize / 4.0f;

            _device.FillRectangle(new RawRectangleF(x - modifier, y - modifier, x + layout.Metrics.Width + modifier, y + layout.Metrics.Height + modifier), background.GetBrush());

            _device.DrawTextLayout(new RawVector2(x, y), layout, brush.GetBrush(), DrawTextOptions.NoSnap);

            layout.Dispose();
        }

        #endregion

        #region Images

        public void DrawImage(D2DImage bmp, Primitives.Point location, float opacity = 1.0f)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing any primitives!");

            Bitmap bitmap = bmp;
            _device.DrawBitmap(bmp, new RawRectangleF(location.X, location.Y, location.X + bitmap.PixelSize.Width, location.Y + bitmap.PixelSize.Height), opacity, BitmapInterpolationMode.Linear);
        }

        public void DrawImage(D2DImage bmp, float x, float y, float opacity = 1.0f)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing any primitives!");

            Bitmap bitmap = bmp;
            _device.DrawBitmap(bmp, new RawRectangleF(x, y, x + bitmap.PixelSize.Width, y + bitmap.PixelSize.Height), opacity, BitmapInterpolationMode.Linear);
        }

        public void DrawImage(D2DImage bmp, Primitives.Rectangle rectangle, float opacity = 1.0f)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing any primitives!");

            Bitmap bitmap = bmp;
            _device.DrawBitmap(bmp, rectangle, opacity, BitmapInterpolationMode.Linear, new RawRectangleF(0, 0, bitmap.PixelSize.Width, bitmap.PixelSize.Height));
        }

        public void DrawImage(D2DImage bmp, float x, float y, float width, float height, float opacity = 1.0f)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing any primitives!");

            Bitmap bitmap = bmp;
            _device.DrawBitmap(bmp, new RawRectangleF(x, y, x + width, y + height), opacity, BitmapInterpolationMode.Linear, new RawRectangleF(0, 0, bitmap.PixelSize.Width, bitmap.PixelSize.Height));
        }

        #endregion

        #region Shapes and containers

        public void DrawShape(IShape shape)
        {
            shape.Draw(this);
        }

        public void DrawShapes(IShapeContainer container)
        {
            foreach (IShape shape in container)
                shape.Draw(this);
        }

        #endregion

        #region Geometry and meshes

        public void DrawGeometry(Geometry geometry, float stroke, ID2DBrush brush)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing any primitives!");

            _device.DrawGeometry(geometry, brush.GetBrush(), stroke);
        }

        public void DrawDashedGeometry(Geometry geometry, float stroke, ID2DBrush brush)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing any primitives!");

            _device.DrawGeometry(geometry, brush.GetBrush(), stroke, _sharedStrokeStyle);
        }

        public void FillGeometry(Geometry geometry, ID2DBrush brush)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing any primitives!");

            _device.FillGeometry(geometry, brush.GetBrush());
        }

        public void FillMesh(Mesh mesh, ID2DBrush brush)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing any primitives!");

            _device.FillMesh(mesh, brush.GetBrush());
        }

        #endregion

        #region Interop

        public RenderTarget GetRenderTarget()
        {
            if (!IsInitialized) throw new InvalidOperationException("The " + nameof(D2DDevice) + " hasn't finished initialization!");

            return _device;
        }

        public Factory GetFactory()
        {
            if (!IsInitialized) throw new InvalidOperationException("The " + nameof(D2DDevice) + " hasn't finished initialization!");

            return _factory;
        }

        public FontFactory GetFontFactory()
        {
            if (!IsInitialized) throw new InvalidOperationException("The " + nameof(D2DDevice) + " hasn't finished initialization!");

            return _fontFactory;
        }

        #endregion

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

                try
                {
                    if (IsInitialized && IsDrawing) EndScene();
                }
                catch
                {

                }

                try
                {
                    DestroyInstance();
                }
                catch
                {

                }

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

        #region Implicit conversions

        public static implicit operator RenderTarget(D2DDevice device)
        {
            return device.GetRenderTarget();
        }

        public static implicit operator Factory(D2DDevice device)
        {
            return device.GetFactory();
        }

        public static implicit operator FontFactory(D2DDevice device)
        {
            return device.GetFontFactory();
        }

        #endregion
    }
}