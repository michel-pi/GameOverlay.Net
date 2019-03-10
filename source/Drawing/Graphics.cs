using System;
using System.Diagnostics;

using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;

using GameOverlay.PInvoke;

using AlphaMode = SharpDX.Direct2D1.AlphaMode;
using TextAntialiasMode = SharpDX.Direct2D1.TextAntialiasMode;
using FactoryType = SharpDX.Direct2D1.FactoryType;

using Factory = SharpDX.Direct2D1.Factory;
using FontFactory = SharpDX.DirectWrite.Factory;

namespace GameOverlay.Drawing
{
    /// <summary>
    /// Encapsulates a Direct2D drawing surface.
    /// </summary>
    public class Graphics : IDisposable
    {
        private HwndRenderTargetProperties _deviceProperties;
        private WindowRenderTarget _device;

        private Factory _factory;
        private FontFactory _fontFactory;

        private StrokeStyle _strokeStyle;

        private Stopwatch _watch;

        private volatile bool _resize;

        private volatile int _resizeWidth;
        private volatile int _resizeHeight;

        private volatile int _fpsCount;
        
        /// <summary>
        /// Indicates whether this Graphics surface will change its size on the next Scene.
        /// </summary>
        public bool IsResizing => _resize;

        /// <summary>
        /// Indicates whether this Graphics surface is initialized.
        /// </summary>
        public bool IsInitialized { get; private set; }
        /// <summary>
        /// Indicates whether this Graphics surface is currently drawing on a Scene.
        /// </summary>
        public bool IsDrawing { get; private set; }

        /// <summary>
        /// Determines whether this Graphics device will measure the resulting frames per second.
        /// </summary>
        public bool MeasureFPS { get; set; }

        /// <summary>
        /// Determines whether Anti-Aliasing for each primitive (Line, Rectangle, Circle, Geometry) is enabled.
        /// </summary>
        public bool PerPrimitiveAntiAliasing { get; set; }
        /// <summary>
        /// Determines whether Anti-Aliasing for Text is enabled.
        /// </summary>
        public bool TextAntiAliasing { get; set; }
        /// <summary>
        /// Determines whether this Graphics surface will be locked to the monitors refresh rate.
        /// </summary>
        public bool VSync { get; set; }
        /// <summary>
        /// Determines whether factories (Font, Geometry, Brush) will be used in a multi-threaded environment.
        /// </summary>
        public bool UseMultiThreadedFactories { get; set; }

        /// <summary>
        /// Gets or sets the width of this Graphics surface.
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// Gets or sets the width of this Graphics surface.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Specifies the images per second in which this graphics device redraws.
        /// </summary>
        public int FPS { get; private set; }

        /// <summary>
        /// Gets or sets the window handle of the Graphics surface.
        /// </summary>
        public IntPtr WindowHandle { get; set; }

        /// <summary>
        /// Initializes a new Graphics surface.
        /// </summary>
        public Graphics()
        {
            _watch = new Stopwatch();

            PerPrimitiveAntiAliasing = false;
            TextAntiAliasing = true;
            VSync = false;
            UseMultiThreadedFactories = false;
        }

        /// <summary>
        /// Initializes a new Graphics surface using a window handle.
        /// </summary>
        /// <param name="windowHandle">A handle to the window used as a surface.</param>
        public Graphics(IntPtr windowHandle) : this()
        {
            WindowHandle = windowHandle;
        }

        /// <summary>
        /// Initializes a new Graphics surface using a window handle and its width and height.
        /// </summary>
        /// <param name="windowHandle">A handle to the window used as a surface.</param>
        /// <param name="width">A value indicating the width of the surface.</param>
        /// <param name="height">A value indicating the height of the surface.</param>
        public Graphics(IntPtr windowHandle, int width, int height) : this()
        {
            WindowHandle = windowHandle;
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Allows an object to try to free resources and perform other cleanup operations before it is reclaimed by garbage collection.
        /// </summary>
        ~Graphics()
        {
            Dispose(false);
        }

        /// <summary>
        /// Sets up and finishes the initialization of this Graphics surface by using this objects properties.
        /// </summary>
        public void Setup()
        {
            if (IsInitialized) throw new InvalidOperationException("Graphics device is already initialized");
            if (Width <= 0 || Height <= 0) throw new ArgumentOutOfRangeException("Width or Height is not valid");
            if (WindowHandle == IntPtr.Zero) throw new ArgumentOutOfRangeException("WindowHandle is zero");
            if (!User32.IsWindow(WindowHandle)) throw new ArgumentOutOfRangeException("WindowHandle is not valid");

            _factory = new Factory(UseMultiThreadedFactories ? FactoryType.MultiThreaded : FactoryType.SingleThreaded);
            _fontFactory = new FontFactory();

            _deviceProperties = new HwndRenderTargetProperties()
            {
                Hwnd = WindowHandle,
                PixelSize = new Size2(Width, Height),
                PresentOptions = VSync ? PresentOptions.None : PresentOptions.Immediately
            };

            // documentation: https://docs.microsoft.com/en-us/windows/desktop/direct2d/supported-pixel-formats-and-alpha-modes
            // those 3 PixelFormats are the only supported ones of a HwndRenderTarget (WindowRenderTarget)
            var renderProperties = new RenderTargetProperties(
                RenderTargetType.Default,
                // msdn: B8G8R8A8_UNorm should be used for best performance
                new PixelFormat(Format.B8G8R8A8_UNorm, AlphaMode.Premultiplied), // supports hardware rendering & software rendering
                96.0f,
                96.0f,
                RenderTargetUsage.None,
                FeatureLevel.Level_DEFAULT);

            try
            {
                _device = new WindowRenderTarget(_factory, renderProperties, _deviceProperties);
            }
            catch (SharpDXException)
            {
                try
                {
                    renderProperties.PixelFormat = new PixelFormat(Format.R8G8B8A8_UNorm, AlphaMode.Premultiplied); // supports hardware rendering
                    _device = new WindowRenderTarget(_factory, renderProperties, _deviceProperties);
                }
                catch (SharpDXException)
                {
                    renderProperties.PixelFormat = new PixelFormat(Format.Unknown, AlphaMode.Premultiplied); // supports hardware & software rendering
                    _device = new WindowRenderTarget(_factory, renderProperties, _deviceProperties);
                }
            }

            _device.AntialiasMode = PerPrimitiveAntiAliasing ? AntialiasMode.PerPrimitive : AntialiasMode.Aliased; // anti aliasing does not preserve colors correctly
            _device.TextAntialiasMode = TextAntiAliasing ? TextAntialiasMode.Grayscale : TextAntialiasMode.Aliased; // using ClearType makes text invisible on white background (white underlying windows)

            _strokeStyle = new StrokeStyle(_factory, new StrokeStyleProperties
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

        /// <summary>
        /// Destroys an already initialized Graphics surface and frees its resources.
        /// </summary>
        public void Destroy()
        {
            if (!IsInitialized) throw new InvalidOperationException("D2DDevice needs to be initialized first");

            try
            {
                _strokeStyle.Dispose();
                _fontFactory.Dispose();
                _factory.Dispose();
                _device.Dispose();
            }
            catch { }
            
            IsInitialized = false;
        }

        /// <summary>
        /// Tells the Graphics surface to resize itself on the next Scene.
        /// </summary>
        /// <param name="width">A value Determining the new width of this Graphics surface.</param>
        /// <param name="height">A value Determining the new height of this Graphics surface.</param>
        public void Resize(int width, int height)
        {
            if (Width == width && Height == height) return;

            if (IsInitialized)
            {
                _resizeWidth = width;
                _resizeHeight = height;
                _resize = true;
            }
            else
            {
                Width = width;
                Height = height;
            }
        }

        /// <summary>
        /// Starts a new Scene (Frame).
        /// </summary>
        public void BeginScene()
        {
            if (!IsInitialized) throw new InvalidOperationException("The DirectX device is not initialized");
            if (IsDrawing) return;

            if (MeasureFPS && !_watch.IsRunning)
            {
                _watch.Restart();
            }

            if (_resize)
            {
                try
                {
                    _resize = false;

                    Width = _resizeWidth;
                    Height = _resizeHeight;

                    _device.Resize(new Size2(_resizeWidth, _resizeHeight));
                }
                catch { } // idk sometimes fails?
            }

            _device.BeginDraw();

            IsDrawing = true;
        }

        /// <summary>
        /// Clears the current Scene (Frame) using a transparent background color.
        /// </summary>
        public void ClearScene()
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing anything");

            _device.Clear(null);
        }

        /// <summary>
        /// Clears the current Scene (Frame) using the given background color.
        /// </summary>
        /// <param name="color">The background color of this Scene.</param>
        public void ClearScene(Color color)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing anything");

            _device.Clear(color);
        }

        /// <summary>
        /// Clears the current Scene (Frame) using the given brush.
        /// </summary>
        /// <param name="brush">The brush used to draw the background of this Scene.</param>
        public void ClearScene(SolidBrush brush)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing anything");

            _device.Clear(brush.Color);
        }

        /// <summary>
        /// Ends the current Scene (Frame).
        /// </summary>
        public void EndScene()
        {
            if (!IsInitialized) throw new InvalidOperationException("The DirectX device is not initialized");
            if (!IsDrawing) return;

            var result = _device.TryEndDraw(out long _, out long _);

            if (result.Failure)
            {
                Destroy();
                Setup();
            }
            
            if (MeasureFPS && _watch.IsRunning)
            {
                _fpsCount++;

                if (_watch.ElapsedMilliseconds >= 1000)
                {
                    FPS = _fpsCount;

                    _fpsCount = 0;

                    _watch.Stop();
                }
            }

            IsDrawing = false;
        }

        /// <summary>
        /// Creates a new Scene which handles BeginScene and EndScene within a using block.
        /// </summary>
        /// <returns>The Scene this method creates.</returns>
        public Scene UseScene()
        {
            return new Scene(this);
        }

        /// <summary>
        /// Creates a new SolidBrush by using the given color components.
        /// </summary>
        /// <param name="r">The red component value of this color.</param>
        /// <param name="g">The green component value of this color.</param>
        /// <param name="b">The blue component value of this color.</param>
        /// <param name="a">The alpha component value of this color.</param>
        /// <returns>The SolidBrush this method creates.</returns>
        public SolidBrush CreateSolidBrush(float r, float g, float b, float a = 1.0f)
        {
            if (!IsInitialized) throw new InvalidOperationException("The DirectX device is not initialized");

            return new SolidBrush(_device, new Color(r, g, b, a));
        }

        /// <summary>
        /// Creates a new SolidBrush by using the given color components.
        /// </summary>
        /// <param name="r">The red component value of this color.</param>
        /// <param name="g">The green component value of this color.</param>
        /// <param name="b">The blue component value of this color.</param>
        /// <param name="a">The alpha component value of this color.</param>
        /// <returns>The SolidBrush this method creates.</returns>
        public SolidBrush CreateSolidBrush(int r, int g, int b, int a = 255)
        {
            if (!IsInitialized) throw new InvalidOperationException("The DirectX device is not initialized");

            return new SolidBrush(_device, new Color(r, g, b, a));
        }

        /// <summary>
        /// Creates a new SolidBrush by using the given color structure.
        /// </summary>
        /// <param name="color">A value representing the ARGB components used to create a SolidBrush.</param>
        /// <returns>The SolidBrush this method creates.</returns>
        public SolidBrush CreateSolidBrush(Color color)
        {
            if (!IsInitialized) throw new InvalidOperationException("The DirectX device is not initialized");

            return new SolidBrush(_device, color);
        }

        /// <summary>
        /// Creates a new Font by using the given font family, size and styles.
        /// </summary>
        /// <param name="fontFamilyName">The name of any installed font family.</param>
        /// <param name="size">A value indicating the size of a font in pixels.</param>
        /// <param name="bold">A Boolean determining whether this font is bold.</param>
        /// <param name="italic">A Boolean determining whether this font is italic.</param>
        /// <param name="wordWrapping">A Boolean determining whether this font uses word wrapping.</param>
        /// <returns></returns>
        public Font CreateFont(string fontFamilyName, float size, bool bold = false, bool italic = false, bool wordWrapping = false)
        {
            if (!IsInitialized) throw new InvalidOperationException("The DirectX device is not initialized");

            return new Font(_fontFactory, fontFamilyName, size, bold, italic, wordWrapping);
        }

        /// <summary>
        /// Creates a new Image by using the given bytes.
        /// </summary>
        /// <param name="bytes">An image loaded into a byte array.</param>
        /// <returns>The Image this method creates.</returns>
        public Image CreateImage(byte[] bytes)
        {
            if (!IsInitialized) throw new InvalidOperationException("The DirectX device is not initialized");

            return new Image(_device, bytes);
        }

        /// <summary>
        /// Creates a new Image from an image file on the disk.
        /// </summary>
        /// <param name="path">The path to an image file.</param>
        /// <returns>The Image this method creates.</returns>
        public Image CreateImage(string path)
        {
            if (!IsInitialized) throw new InvalidOperationException("The DirectX device is not initialized");

            return new Image(_device, path);
        }

        /// <summary>
        /// Creates a new Geometry used to draw complex figures.
        /// </summary>
        /// <returns>The Geometry this method creates.</returns>
        public Geometry CreateGeometry()
        {
            return new Geometry(this);
        }

        /// <summary>
        /// Draws a circle using the given brush and dimension.
        /// </summary>
        /// <param name="brush">A brush that determines the color of the circle.</param>
        /// <param name="x">The x-coordinate of the center of the circle.</param>
        /// <param name="y">The y-coordinate of the center of the circle.</param>
        /// <param name="radius">The radius of the circle.</param>
        /// <param name="stroke">A value that determines the width/thickness of the circle.</param>
        public void DrawCircle(IBrush brush, float x, float y, float radius, float stroke)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing anything");

            _device.DrawEllipse(new SharpDX.Direct2D1.Ellipse(new RawVector2(x, y), radius, radius), brush.Brush, stroke);
        }

        /// <summary>
        /// Draws a circle using the given brush and dimension.
        /// </summary>
        /// <param name="brush">A brush that determines the color of the circle.</param>
        /// <param name="location">A Point structureure which includes the x- and y-coordinate of the center of the circle.</param>
        /// <param name="radius">The radius of the circle.</param>
        /// <param name="stroke">A value that determines the width/thickness of the circle.</param>
        public void DrawCircle(IBrush brush, Point location, float radius, float stroke) => DrawCircle(brush, location.X, location.Y, radius, stroke);

        /// <summary>
        /// Draws a circle using the given brush and dimension.
        /// </summary>
        /// <param name="brush">A brush that determines the color of the circle.</param>
        /// <param name="circle">A Circle structure which includes the dimension of the circle.</param>
        /// <param name="stroke">A value that determines the width/thickness of the circle.</param>
        public void DrawCircle(IBrush brush, Circle circle, float stroke) => DrawCircle(brush, circle.Location.X, circle.Location.Y, circle.Radius, stroke);

        /// <summary>
        /// Draws a circle with an outline around it using the given brush and dimension.
        /// </summary>
        /// <param name="outline">A brush that determines the color of the outline.</param>
        /// <param name="fill">A brush that determines the color of the circle.</param>
        /// <param name="x">The x-coordinate of the center of the circle.</param>
        /// <param name="y">The y-coordinate of the center of the circle.</param>
        /// <param name="radius">The radius of the circle.</param>
        /// <param name="stroke">A value that determines the width/thickness of the circle.</param>
        public void OutlineCircle(IBrush outline, IBrush fill, float x, float y, float radius, float stroke)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing anything");

            var ellipse = new SharpDX.Direct2D1.Ellipse(new RawVector2(x, y), radius, radius);
            
            _device.DrawEllipse(ellipse, fill.Brush, stroke);

            float halfStroke = stroke * 0.5f;

            ellipse.RadiusX += halfStroke;
            ellipse.RadiusY += halfStroke;

            _device.DrawEllipse(ellipse, outline.Brush, halfStroke);

            ellipse.RadiusX -= stroke;
            ellipse.RadiusY -= stroke;

            _device.DrawEllipse(ellipse, outline.Brush, halfStroke);
        }

        /// <summary>
        /// Draws a circle with an outline around it using the given brush and dimension.
        /// </summary>
        /// <param name="outline">A brush that determines the color of the outline.</param>
        /// <param name="fill">A brush that determines the color of the circle.</param>
        /// <param name="location">A Point structureure which includes the x- and y-coordinate of the center of the circle.</param>
        /// <param name="radius">The radius of the circle.</param>
        /// <param name="stroke">A value that determines the width/thickness of the circle.</param>
        public void OutlineCircle(IBrush outline, IBrush fill, Point location, float radius, float stroke) => OutlineCircle(outline, fill, location.X, location.Y, radius, stroke);

        /// <summary>
        /// Draws a circle with an outline around it using the given brush and dimension.
        /// </summary>
        /// <param name="outline">A brush that determines the color of the outline.</param>
        /// <param name="fill">A brush that determines the color of the circle.</param>
        /// <param name="circle">A Circle structure which includes the dimension of the circle.</param>
        /// <param name="stroke">A value that determines the width/thickness of the circle.</param>
        public void OutlineCircle(IBrush outline, IBrush fill, Circle circle, float stroke) => OutlineCircle(outline, fill, circle.Location.X, circle.Location.Y, circle.Radius, stroke);

        /// <summary>
        /// Draws a circle with a dashed line by using the given brush and dimension.
        /// </summary>
        /// <param name="brush">A brush that determines the color of the circle.</param>
        /// <param name="x">The x-coordinate of the center of the circle.</param>
        /// <param name="y">The y-coordinate of the center of the circle.</param>
        /// <param name="radius">The radius of the circle.</param>
        /// <param name="stroke">A value that determines the width/thickness of the circle.</param>
        public void DashedCircle(IBrush brush, float x, float y, float radius, float stroke)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing anything");

            _device.DrawEllipse(new SharpDX.Direct2D1.Ellipse(new RawVector2(x, y), radius, radius), brush.Brush, stroke, _strokeStyle);
        }

        /// <summary>
        /// Draws a circle with a dashed line by using the given brush and dimension.
        /// </summary>
        /// <param name="brush">A brush that determines the color of the circle.</param>
        /// <param name="location">A Point structureure which includes the x- and y-coordinate of the center of the circle.</param>
        /// <param name="radius">The radius of the circle.</param>
        /// <param name="stroke">A value that determines the width/thickness of the circle.</param>
        public void DashedCircle(IBrush brush, Point location, float radius, float stroke) => DashedCircle(brush, location.X, location.Y, radius, stroke);

        /// <summary>
        /// Draws a circle with a dashed line by using the given brush and dimension.
        /// </summary>
        /// <param name="brush">A brush that determines the color of the circle.</param>
        /// <param name="circle">A Circle structure which includes the dimension of the circle.</param>
        /// <param name="stroke">A value that determines the width/thickness of the circle.</param>
        public void DashedCircle(IBrush brush, Circle circle, float stroke) => DashedCircle(brush, circle.Location.X, circle.Location.Y, circle.Radius, stroke);

        /// <summary>
        /// Fills a circle by using the given brush and dimesnion.
        /// </summary>
        /// <param name="brush">A brush that determines the color of the circle.</param>
        /// <param name="x">The x-coordinate of the center of the circle.</param>
        /// <param name="y">The y-coordinate of the center of the circle.</param>
        /// <param name="radius">The radius of the circle.</param>
        public void FillCircle(IBrush brush, float x, float y, float radius)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing anything");

            _device.FillEllipse(new SharpDX.Direct2D1.Ellipse(new RawVector2(x, y), radius, radius), brush.Brush);
        }

        /// <summary>
        /// Fills a circle by using the given brush and dimesnion.
        /// </summary>
        /// <param name="brush">A brush that determines the color of the circle.</param>
        /// <param name="location">A Point structureure which includes the x- and y-coordinate of the center of the circle.</param>
        /// <param name="radius">The radius of the circle.</param>
        public void FillCircle(IBrush brush, Point location, float radius) => FillCircle(brush, location.X, location.Y, radius);

        /// <summary>
        /// Fills a circle by using the given brush and dimesnion.
        /// </summary>
        /// <param name="brush">A brush that determines the color of the circle.</param>
        /// <param name="circle">A Circle structure which includes the dimension of the circle.</param>
        public void FillCircle(IBrush brush, Circle circle) => FillCircle(brush, circle.Location.X, circle.Location.Y, circle.Radius);

        /// <summary>
        /// Draws a filled circle with an outline around it.
        /// </summary>
        /// <param name="outline">A brush that determines the color of the outline.</param>
        /// <param name="fill">A brush that determines the color of the circle.</param>
        /// <param name="x">The x-coordinate of the center of the circle.</param>
        /// <param name="y">The y-coordinate of the center of the circle.</param>
        /// <param name="radius">The radius of the circle.</param>
        /// <param name="stroke">A value that determines the width/thickness of the circle.</param>
        public void OutlineFillCircle(IBrush outline, IBrush fill, float x, float y, float radius, float stroke)
        {
            var ellipseGeometry = new EllipseGeometry(_factory, new SharpDX.Direct2D1.Ellipse(new RawVector2(x, y), radius, radius));

            var geometry = new PathGeometry(_factory);

            var sink = geometry.Open();

            ellipseGeometry.Outline(sink);

            sink.Close();

            _device.FillGeometry(geometry, fill.Brush);
            _device.DrawGeometry(geometry, outline.Brush, stroke);

            sink.Dispose();
            geometry.Dispose();
            ellipseGeometry.Dispose();
        }

        /// <summary>
        /// Draws a filled circle with an outline around it.
        /// </summary>
        /// <param name="outline">A brush that determines the color of the outline.</param>
        /// <param name="fill">A brush that determines the color of the circle.</param>
        /// <param name="location">A Point structureure which includes the x- and y-coordinate of the center of the circle.</param>
        /// <param name="radius">The radius of the circle.</param>
        /// <param name="stroke">A value that determines the width/thickness of the circle.</param>
        public void OutlineFillCircle(IBrush outline, IBrush fill, Point location, float radius, float stroke) => OutlineFillCircle(outline, fill, location.X, location.Y, radius, stroke);

        /// <summary>
        /// Draws a filled circle with an outline around it.
        /// </summary>
        /// <param name="outline">A brush that determines the color of the outline.</param>
        /// <param name="fill">A brush that determines the color of the circle.</param>
        /// <param name="circle">A Circle structure which includes the dimension of the circle.</param>
        /// <param name="stroke">A value that determines the width/thickness of the circle.</param>
        public void OutlineFillCircle(IBrush outline, IBrush fill, Circle circle, float stroke) => OutlineFillCircle(outline, fill, circle.Location.X, circle.Location.Y, circle.Radius, stroke);

        /// <summary>
        /// Draws an ellipse by using the given brush and dimension.
        /// </summary>
        /// <param name="brush">A brush that determines the color of the ellipse.</param>
        /// <param name="x">The x-coordinate of the center of the ellipse.</param>
        /// <param name="y">The y-coordinate of the center of the ellipse.</param>
        /// <param name="radiusX">The radius of this ellipse on the x-axis.</param>
        /// <param name="radiusY">The radius of this ellipse on the y-axis.</param>
        /// <param name="stroke">A value that determines the width/thickness of the circle.</param>
        public void DrawEllipse(IBrush brush, float x, float y, float radiusX, float radiusY, float stroke)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing anything");

            _device.DrawEllipse(new SharpDX.Direct2D1.Ellipse(new RawVector2(x, y), radiusX, radiusY), brush.Brush, stroke);
        }

        /// <summary>
        /// Draws an ellipse by using the given brush and dimension.
        /// </summary>
        /// <param name="brush">A brush that determines the color of the ellipse.</param>
        /// <param name="location">A Point structureure which includes the x- and y-coordinate of the center of the ellipse.</param>
        /// <param name="radiusX">The radius of this ellipse on the x-axis.</param>
        /// <param name="radiusY">The radius of this ellipse on the y-axis.</param>
        /// <param name="stroke">A value that determines the width/thickness of the circle.</param>
        public void DrawEllipse(IBrush brush, Point location, float radiusX, float radiusY, float stroke) => DrawEllipse(brush, location.X, location.Y, radiusX, radiusY, stroke);

        /// <summary>
        /// Draws an ellipse by using the given brush and dimension.
        /// </summary>
        /// <param name="brush">A brush that determines the color of the ellipse.</param>
        /// <param name="ellipse">An Ellipse structure which includes the dimension of the ellipse.</param>
        /// <param name="stroke">A value that determines the width/thickness of the circle.</param>
        public void DrawEllipse(IBrush brush, Ellipse ellipse, float stroke) => DrawEllipse(brush, ellipse.Location.X, ellipse.Location.Y, ellipse.RadiusX, ellipse.RadiusY, stroke);

        /// <summary>
        /// Draws an ellipse with an outline around it using the given brush and dimension.
        /// </summary>
        /// <param name="outline">A brush that determines the color of the outline.</param>
        /// <param name="fill">A brush that determines the color of the ellipse.</param>
        /// <param name="x">The x-coordinate of the center of the ellipse.</param>
        /// <param name="y">The y-coordinate of the center of the ellipse.</param>
        /// <param name="radiusX">The radius of the ellipse on the x-axis.</param>
        /// <param name="radiusY">The radius of the ellipse on the y-axis.</param>
        /// <param name="stroke">A value that determines the width/thickness of the ellipse.</param>
        public void OutlineEllipse(IBrush outline, IBrush fill, float x, float y, float radiusX, float radiusY, float stroke)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing anything");

            var ellipse = new SharpDX.Direct2D1.Ellipse(new RawVector2(x, y), radiusX, radiusY);

            _device.DrawEllipse(ellipse, fill.Brush, stroke);

            float halfStroke = stroke * 0.5f;

            ellipse.RadiusX += halfStroke;
            ellipse.RadiusY += halfStroke;

            _device.DrawEllipse(ellipse, outline.Brush, halfStroke);

            ellipse.RadiusX -= stroke;
            ellipse.RadiusY -= stroke;

            _device.DrawEllipse(ellipse, outline.Brush, halfStroke);
        }

        /// <summary>
        /// Draws an ellipse with an outline around it using the given brush and dimension.
        /// </summary>
        /// <param name="outline">A brush that determines the color of the outline.</param>
        /// <param name="fill">A brush that determines the color of the ellipse.</param>
        /// <param name="location">A Point structureure which includes the x- and y-coordinate of the center of the ellipse.</param>
        /// <param name="radiusX">The radius of the ellipse on the x-axis.</param>
        /// <param name="radiusY">The radius of the ellipse on the y-axis.</param>
        /// <param name="stroke">A value that determines the width/thickness of the ellipse.</param>
        public void OutlineEllipse(IBrush outline, IBrush fill, Point location, float radiusX, float radiusY, float stroke) => OutlineEllipse(outline, fill, location.X, location.Y, radiusX, radiusY, stroke);

        /// <summary>
        /// Draws an ellipse with an outline around it using the given brush and dimension.
        /// </summary>
        /// <param name="outline">A brush that determines the color of the outline.</param>
        /// <param name="fill">A brush that determines the color of the ellipse.</param>
        /// <param name="ellipse">An Ellipse structure which includes the dimension of the ellipse.</param>
        /// <param name="stroke">A value that determines the width/thickness of the ellipse.</param>
        public void OutlineEllipse(IBrush outline, IBrush fill, Ellipse ellipse, float stroke) => OutlineEllipse(outline, fill, ellipse.Location.X, ellipse.Location.Y, ellipse.RadiusX, ellipse.RadiusY, stroke);

        /// <summary>
        /// Draws an ellipse with a dashed line by using the given brush and dimension.
        /// </summary>
        /// <param name="brush">A brush that determines the color of the ellipse.</param>
        /// <param name="x">The x-coordinate of the center of the ellipse.</param>
        /// <param name="y">The y-coordinate of the center of the ellipse.</param>
        /// <param name="radiusX">The radius of the ellipse on the x-axis.</param>
        /// <param name="radiusY">The radius of the ellipse on the y-axis.</param>
        /// <param name="stroke">A value that determines the width/thickness of the ellipse.</param>
        public void DashedEllipse(IBrush brush, float x, float y, float radiusX, float radiusY, float stroke)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing anything");

            _device.DrawEllipse(new SharpDX.Direct2D1.Ellipse(new RawVector2(x, y), radiusX, radiusY), brush.Brush, stroke, _strokeStyle);
        }

        /// <summary>
        /// Draws an ellipse with a dashed line by using the given brush and dimension.
        /// </summary>
        /// <param name="brush">A brush that determines the color of the ellipse.</param>
        /// <param name="location">A Point structureure which includes the x- and y-coordinate of the center of the ellipse.</param>
        /// <param name="radiusX">The radius of the ellipse on the x-axis.</param>
        /// <param name="radiusY">The radius of the ellipse on the y-axis.</param>
        /// <param name="stroke">A value that determines the width/thickness of the ellipse.</param>
        public void DashedEllipse(IBrush brush, Point location, float radiusX, float radiusY, float stroke) => DashedEllipse(brush, location.X, location.Y, radiusX, radiusY, stroke);

        /// <summary>
        /// Draws an ellipse with a dashed line by using the given brush and dimension.
        /// </summary>
        /// <param name="brush">A brush that determines the color of the ellipse.</param>
        /// <param name="ellipse">An Ellipse structure which includes the dimension of the ellipse.</param>
        /// <param name="stroke">A value that determines the width/thickness of the ellipse.</param>
        public void DashedEllipse(IBrush brush, Ellipse ellipse, float stroke) => DashedEllipse(brush, ellipse.Location.X, ellipse.Location.Y, ellipse.RadiusX, ellipse.RadiusY, stroke);

        /// <summary>
        /// Fills an ellipse by using the given brush and dimesnion.
        /// </summary>
        /// <param name="brush">A brush that determines the color of the ellipse.</param>
        /// <param name="x">The x-coordinate of the center of the ellipse.</param>
        /// <param name="y">The y-coordinate of the center of the ellipse.</param>
        /// <param name="radiusX">The radius of the ellipse on the x-axis.</param>
        /// <param name="radiusY">The radius of the ellipse on the y-axis.</param>
        public void FillEllipse(IBrush brush, float x, float y, float radiusX, float radiusY)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing anything");

            _device.FillEllipse(new SharpDX.Direct2D1.Ellipse(new RawVector2(x, y), radiusX, radiusY), brush.Brush);
        }

        /// <summary>
        /// Fills an ellipse by using the given brush and dimesnion.
        /// </summary>
        /// <param name="brush">A brush that determines the color of the ellipse.</param>
        /// <param name="location">A Point structureure which includes the x- and y-coordinate of the center of the ellipse.</param>
        /// <param name="radiusX">The radius of the ellipse on the x-axis.</param>
        /// <param name="radiusY">The radius of the ellipse on the y-axis.</param>
        public void FillEllipse(IBrush brush, Point location, float radiusX, float radiusY) => FillEllipse(brush, location.X, location.Y, radiusX, radiusY);

        /// <summary>
        /// Fills an ellipse by using the given brush and dimesnion.
        /// </summary>
        /// <param name="brush">A brush that determines the color of the ellipse.</param>
        /// <param name="ellipse">An Ellipse structure which includes the dimension of the ellipse.</param>
        public void FillEllipse(IBrush brush, Ellipse ellipse) => FillEllipse(brush, ellipse.Location.X, ellipse.Location.Y, ellipse.RadiusX, ellipse.RadiusY);

        /// <summary>
        /// Draws a filled ellipse with an outline around it.
        /// </summary>
        /// <param name="outline">A brush that determines the color of the outline.</param>
        /// <param name="fill">A brush that determines the color of the ellipse.</param>
        /// <param name="x">The x-coordinate of the center of the ellipse.</param>
        /// <param name="y">The y-coordinate of the center of the ellipse.</param>
        /// <param name="radiusX">The radius of the ellipse on the x-axis.</param>
        /// <param name="radiusY">The radius of the ellipse on the y-axis.</param>
        /// <param name="stroke">A value that determines the width/thickness of the ellipse.</param>
        public void OutlineFillEllipse(IBrush outline, IBrush fill, float x, float y, float radiusX, float radiusY, float stroke)
        {
            var ellipseGeometry = new EllipseGeometry(_factory, new SharpDX.Direct2D1.Ellipse(new RawVector2(x, y), radiusX, radiusY));

            var geometry = new PathGeometry(_factory);

            var sink = geometry.Open();

            ellipseGeometry.Outline(sink);

            sink.Close();

            _device.FillGeometry(geometry, fill.Brush);
            _device.DrawGeometry(geometry, outline.Brush, stroke);

            sink.Dispose();
            geometry.Dispose();
            ellipseGeometry.Dispose();
        }

        /// <summary>
        /// Draws a filled ellipse with an outline around it.
        /// </summary>
        /// <param name="outline">A brush that determines the color of the outline.</param>
        /// <param name="fill">A brush that determines the color of the ellipse.</param>
        /// <param name="location">A Point structureure which includes the x- and y-coordinate of the center of the ellipse.</param>
        /// <param name="radiusX">The radius of the ellipse on the x-axis.</param>
        /// <param name="radiusY">The radius of the ellipse on the y-axis.</param>
        /// <param name="stroke">A value that determines the width/thickness of the ellipse.</param>
        public void OutlineFillEllipse(IBrush outline, IBrush fill, Point location, float radiusX, float radiusY, float stroke) => OutlineFillEllipse(outline, fill, location.X, location.Y, radiusX, radiusY, stroke);

        /// <summary>
        /// Draws a filled ellipse with an outline around it.
        /// </summary>
        /// <param name="outline">A brush that determines the color of the outline.</param>
        /// <param name="fill">A brush that determines the color of the ellipse.</param>
        /// <param name="ellipse">An Ellipse structure which includes the dimension of the ellipse.</param>
        /// <param name="stroke">A value that determines the width/thickness of the ellipse.</param>
        public void OutlineFillEllipse(IBrush outline, IBrush fill, Ellipse ellipse, float stroke) => OutlineFillEllipse(outline, fill, ellipse.Location.X, ellipse.Location.Y, ellipse.RadiusX, ellipse.RadiusY, stroke);

        /// <summary>
        /// Draws a line starting and ending at the given points.
        /// </summary>
        /// <param name="brush">A brush that determines the color of the line.</param>
        /// <param name="startX">The start position of the line on the x-axis</param>
        /// <param name="startY">The start position of the line on the y-axis</param>
        /// <param name="endX">The end position of the line on the x-axis</param>
        /// <param name="endY">The end position of the line on the y-axis</param>
        /// <param name="stroke">A value that determines the width/thickness of the line.</param>
        public void DrawLine(IBrush brush, float startX, float startY, float endX, float endY, float stroke)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing anything");

            _device.DrawLine(new RawVector2(startX, startY), new RawVector2(endX, endY), brush.Brush, stroke);
        }

        /// <summary>
        /// Draws a line starting and ending at the given points.
        /// </summary>
        /// <param name="brush">A brush that determines the color of the line.</param>
        /// <param name="start">A Point structure including the start position of the line.</param>
        /// <param name="end">A Point structure including the end position of the line.</param>
        /// <param name="stroke">A value that determines the width/thickness of the line.</param>
        public void DrawLine(IBrush brush, Point start, Point end, float stroke) => DrawLine(brush, start.X, start.Y, end.X, end.Y, stroke);

        /// <summary>
        /// Draws a line starting and ending at the given points.
        /// </summary>
        /// <param name="brush">A brush that determines the color of the line.</param>
        /// <param name="line">A Line structure including the start and end Point of the line.</param>
        /// <param name="stroke">A value that determines the width/thickness of the line.</param>
        public void DrawLine(IBrush brush, Line line, float stroke) => DrawLine(brush, line.Start.X, line.Start.Y, line.End.X, line.End.Y, stroke);

        /// <summary>
        /// Draws a line at the given start and end point with an outline around it.
        /// </summary>
        /// <param name="outline">A brush that determines the color of the outline.</param>
        /// <param name="fill">A brush that determines the color of the line.</param>
        /// <param name="startX">The start position of the line on the x-axis</param>
        /// <param name="startY">The start position of the line on the y-axis</param>
        /// <param name="endX">The end position of the line on the x-axis</param>
        /// <param name="endY">The end position of the line on the y-axis</param>
        /// <param name="stroke">A value that determines the width/thickness of the line.</param>
        public void OutlineLine(IBrush outline, IBrush fill, float startX, float startY, float endX, float endY, float stroke)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing anything");

            var geometry = new PathGeometry(_factory);

            var sink = geometry.Open();

            float half = stroke / 2.0f;

            sink.BeginFigure(new RawVector2(startX, startY - half), FigureBegin.Filled);

            sink.AddLine(new RawVector2(endX, endY - half));
            sink.AddLine(new RawVector2(endX, endY + half));
            sink.AddLine(new RawVector2(startX, startY + half));

            sink.EndFigure(FigureEnd.Closed);

            _device.DrawGeometry(geometry, outline.Brush, half);
            _device.FillGeometry(geometry, fill.Brush);

            sink.Dispose();
            geometry.Dispose();
        }

        /// <summary>
        /// Draws a line at the given start and end point with an outline around it.
        /// </summary>
        /// <param name="outline">A brush that determines the color of the outline.</param>
        /// <param name="fill">A brush that determines the color of the line.</param>
        /// <param name="start">A Point structure including the start position of the line.</param>
        /// <param name="end">A Point structure including the end position of the line.</param>
        /// <param name="stroke">A value that determines the width/thickness of the line.</param>
        public void OutlineLine(IBrush outline, IBrush fill, Point start, Point end, float stroke) => OutlineLine(outline, fill, start.X, start.Y, end.X, end.Y, stroke);

        /// <summary>
        /// Draws a line at the given start and end point with an outline around it.
        /// </summary>
        /// <param name="outline">A brush that determines the color of the outline.</param>
        /// <param name="fill">A brush that determines the color of the line.</param>
        /// <param name="line">A Line structure including the start and end Point of the line.</param>
        /// <param name="stroke">A value that determines the width/thickness of the line.</param>
        public void OutlineLine(IBrush outline, IBrush fill, Line line, float stroke) => OutlineLine(outline, fill, line.Start.X, line.Start.Y, line.End.X, line.End.Y, stroke);

        /// <summary>
        /// Draws a dashed line at the given start and end point.
        /// </summary>
        /// <param name="brush">A brush that determines the color of the line.</param>
        /// <param name="startX">The start position of the line on the x-axis</param>
        /// <param name="startY">The start position of the line on the y-axis</param>
        /// <param name="endX">The end position of the line on the x-axis</param>
        /// <param name="endY">The end position of the line on the y-axis</param>
        /// <param name="stroke">A value that determines the width/thickness of the line.</param>
        public void DashedLine(IBrush brush, float startX, float startY, float endX, float endY, float stroke)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing anything");

            _device.DrawLine(new RawVector2(startX, startY), new RawVector2(endX, endY), brush.Brush, stroke, _strokeStyle);
        }

        /// <summary>
        /// Draws a dashed line at the given start and end point.
        /// </summary>
        /// <param name="brush">A brush that determines the color of the line.</param>
        /// <param name="start">A Point structure including the start position of the line.</param>
        /// <param name="end">A Point structure including the end position of the line.</param>
        /// <param name="stroke">A value that determines the width/thickness of the line.</param>
        public void DashedLine(IBrush brush, Point start, Point end, float stroke) => DashedLine(brush, start.X, start.Y, end.X, end.Y, stroke);

        /// <summary>
        /// Draws a dashed line at the given start and end point.
        /// </summary>
        /// <param name="brush">A brush that determines the color of the line.</param>
        /// <param name="line">A Line structure including the start and end Point of the line.</param>
        /// <param name="stroke">A value that determines the width/thickness of the line.</param>
        public void DashedLine(IBrush brush, Line line, float stroke) => DashedLine(brush, line.Start.X, line.Start.Y, line.End.X, line.End.Y, stroke);

        /// <summary>
        /// Draws a rectangle by using the given brush and dimension.
        /// </summary>
        /// <param name="brush">A brush that determines the color of the rectangle.</param>
        /// <param name="left">The x-coordinate of the upper-left corner of the rectangle.</param>
        /// <param name="top">The y-coordinate of the upper-left corner of the rectangle.</param>
        /// <param name="right">The x-coordinate of the lower-right corner of the rectangle.</param>
        /// <param name="bottom">The y-coordinate of the lower-right corner of the rectangle.</param>
        /// <param name="stroke">A value that determines the width/thickness of the line.</param>
        public void DrawRectangle(IBrush brush, float left, float top, float right, float bottom, float stroke)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing anything");

            _device.DrawRectangle(new RawRectangleF(left, top, right, bottom), brush.Brush, stroke);
        }

        /// <summary>
        /// Draws a rectangle by using the given brush and dimension.
        /// </summary>
        /// <param name="brush">A brush that determines the color of the rectangle.</param>
        /// <param name="rectangle">A Rectangle structure that determines the boundaries of the rectangle.</param>
        /// <param name="stroke">A value that determines the width/thickness of the line.</param>
        public void DrawRectangle(IBrush brush, Rectangle rectangle, float stroke) => DrawRectangle(brush, rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom, stroke);

        /// <summary>
        /// Draws a rectangle with an outline around it by using the given brush and dimension.
        /// </summary>
        /// <param name="outline">A brush that determines the color of the outline.</param>
        /// <param name="fill">A brush that determines the color of the rectangle.</param>
        /// <param name="left">The x-coordinate of the upper-left corner of the rectangle.</param>
        /// <param name="top">The y-coordinate of the upper-left corner of the rectangle.</param>
        /// <param name="right">The x-coordinate of the lower-right corner of the rectangle.</param>
        /// <param name="bottom">The y-coordinate of the lower-right corner of the rectangle.</param>
        /// <param name="stroke">A value that determines the width/thickness of the line.</param>
        public void OutlineRectangle(IBrush outline, IBrush fill, float left, float top, float right, float bottom, float stroke)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing anything");

            float halfStroke = stroke / 2.0f;

            float width = right;
            float height = bottom;

            _device.DrawRectangle(new RawRectangleF(left - halfStroke, top - halfStroke, width + halfStroke, height + halfStroke), outline.Brush, halfStroke);

            _device.DrawRectangle(new RawRectangleF(left + halfStroke, top + halfStroke, width - halfStroke, height - halfStroke), outline.Brush, halfStroke);

            _device.DrawRectangle(new RawRectangleF(left, top, width, height), fill.Brush, halfStroke);
        }

        /// <summary>
        /// Draws a rectangle with an outline around it by using the given brush and dimension.
        /// </summary>
        /// <param name="outline">A brush that determines the color of the outline.</param>
        /// <param name="fill">A brush that determines the color of the rectangle.</param>
        /// <param name="rectangle">A Rectangle structure that determines the boundaries of the rectangle.</param>
        /// <param name="stroke">A value that determines the width/thickness of the line.</param>
        public void OutlineRectangle(IBrush outline, IBrush fill, Rectangle rectangle, float stroke) => OutlineRectangle(outline, fill, rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom, stroke);

        /// <summary>
        /// Draws a rectangle with dashed lines by using the given brush and dimension.
        /// </summary>
        /// <param name="brush">A brush that determines the color of the rectangle.</param>
        /// <param name="left">The x-coordinate of the upper-left corner of the rectangle.</param>
        /// <param name="top">The y-coordinate of the upper-left corner of the rectangle.</param>
        /// <param name="right">The x-coordinate of the lower-right corner of the rectangle.</param>
        /// <param name="bottom">The y-coordinate of the lower-right corner of the rectangle.</param>
        /// <param name="stroke">A value that determines the width/thickness of the line.</param>
        public void DashedRectangle(IBrush brush, float left, float top, float right, float bottom, float stroke)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing anything");

            _device.DrawRectangle(new RawRectangleF(left, top, right, bottom), brush.Brush, stroke, _strokeStyle);
        }

        /// <summary>
        /// Draws a rectangle with dashed lines by using the given brush and dimension.
        /// </summary>
        /// <param name="brush">A brush that determines the color of the rectangle.</param>
        /// <param name="rectangle">A Rectangle structure that determines the boundaries of the rectangle.</param>
        /// <param name="stroke">A value that determines the width/thickness of the line.</param>
        public void DashedRectangle(IBrush brush, Rectangle rectangle, float stroke) => DashedRectangle(brush, rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom, stroke);

        /// <summary>
        /// Fills a rectangle by using the given brush and dimension.
        /// </summary>
        /// <param name="brush">A brush that determines the color of the rectangle.</param>
        /// <param name="left">The x-coordinate of the upper-left corner of the rectangle.</param>
        /// <param name="top">The y-coordinate of the upper-left corner of the rectangle.</param>
        /// <param name="right">The x-coordinate of the lower-right corner of the rectangle.</param>
        /// <param name="bottom">The y-coordinate of the lower-right corner of the rectangle.</param>
        public void FillRectangle(IBrush brush, float left, float top, float right, float bottom)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing anything");

            _device.FillRectangle(new RawRectangleF(left, top, right, bottom), brush.Brush);
        }

        /// <summary>
        /// Fills a rectangle by using the given brush and dimension.
        /// </summary>
        /// <param name="brush">A brush that determines the color of the rectangle.</param>
        /// <param name="rectangle">A Rectangle structure that determines the boundaries of the rectangle.</param>
        public void FillRectangle(IBrush brush, Rectangle rectangle) => FillRectangle(brush, rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom);

        /// <summary>
        /// Draws a filled rectangle with an outline around it by using the given brush and dimension.
        /// </summary>
        /// <param name="outline">A brush that determines the color of the outline.</param>
        /// <param name="fill">A brush that determines the color of the rectangle.</param>
        /// <param name="left">The x-coordinate of the upper-left corner of the rectangle.</param>
        /// <param name="top">The y-coordinate of the upper-left corner of the rectangle.</param>
        /// <param name="right">The x-coordinate of the lower-right corner of the rectangle.</param>
        /// <param name="bottom">The y-coordinate of the lower-right corner of the rectangle.</param>
        /// <param name="stroke">A value that determines the width/thickness of the line.</param>
        public void OutlineFillRectangle(IBrush outline, IBrush fill, float left, float top, float right, float bottom, float stroke)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing anything");

            var rectangleGeometry = new RectangleGeometry(_factory, new RawRectangleF(left, top, right, bottom));

            var geometry = new PathGeometry(_factory);

            var sink = geometry.Open();

            rectangleGeometry.Widen(stroke, sink);
            rectangleGeometry.Outline(sink);

            sink.Close();

            _device.FillGeometry(geometry, fill.Brush);
            _device.DrawGeometry(geometry, outline.Brush, stroke);

            sink.Dispose();
            geometry.Dispose();
            rectangleGeometry.Dispose();
        }

        /// <summary>
        /// Draws a filled rectangle with an outline around it by using the given brush and dimension.
        /// </summary>
        /// <param name="outline">A brush that determines the color of the outline.</param>
        /// <param name="fill">A brush that determines the color of the rectangle.</param>
        /// <param name="rectangle">A Rectangle structure that determines the boundaries of the rectangle.</param>
        /// <param name="stroke">A value that determines the width/thickness of the line.</param>
        public void OutlineFillRectangle(IBrush outline, IBrush fill, Rectangle rectangle, float stroke) => OutlineFillRectangle(outline, fill, rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom, stroke);

        /// <summary>
        /// Draws a rectangle with rounded edges by using the given brush and dimension.
        /// </summary>
        /// <param name="brush">A brush that determines the color of the rectangle.</param>
        /// <param name="left">The x-coordinate of the upper-left corner of the rectangle.</param>
        /// <param name="top">The y-coordinate of the upper-left corner of the rectangle.</param>
        /// <param name="right">The x-coordinate of the lower-right corner of the rectangle.</param>
        /// <param name="bottom">The y-coordinate of the lower-right corner of the rectangle.</param>
        /// <param name="radius">A value that determines radius of corners.</param>
        /// <param name="stroke">A value that determines the width/thickness of the line.</param>
        public void DrawRoundedRectangle(IBrush brush, float left, float top, float right, float bottom, float radius, float stroke)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing anything");

            var rect = new SharpDX.Direct2D1.RoundedRectangle()
            {
                RadiusX = radius,
                RadiusY = radius,
                Rect = new RawRectangleF(left, top, right, bottom)
            };

            _device.DrawRoundedRectangle(rect, brush.Brush, stroke);
        }

        /// <summary>
        /// Draws a rectangle with rounded edges by using the given brush and dimension.
        /// </summary>
        /// <param name="brush">A brush that determines the color of the rectangle.</param>
        /// <param name="rectangle">A RoundedRectangle structure including the dimension of the rounded rectangle.</param>
        /// <param name="stroke">A value that determines the width/thickness of the line.</param>
        public void DrawRoundedRectangle(IBrush brush, RoundedRectangle rectangle, float stroke)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing anything");
            
            _device.DrawRoundedRectangle(rectangle, brush.Brush, stroke);
        }

        /// <summary>
        /// Draws a rectangle with rounded edges and dashed lines by using the given brush and dimension.
        /// </summary>
        /// <param name="brush">A brush that determines the color of the rectangle.</param>
        /// <param name="left">The x-coordinate of the upper-left corner of the rectangle.</param>
        /// <param name="top">The y-coordinate of the upper-left corner of the rectangle.</param>
        /// <param name="right">The x-coordinate of the lower-right corner of the rectangle.</param>
        /// <param name="bottom">The y-coordinate of the lower-right corner of the rectangle.</param>
        /// <param name="radius">A value that determines radius of corners.</param>
        /// <param name="stroke">A value that determines the width/thickness of the line.</param>
        public void DashedRoundedRectangle(IBrush brush, float left, float top, float right, float bottom, float radius, float stroke)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing anything");

            var rect = new SharpDX.Direct2D1.RoundedRectangle()
            {
                RadiusX = radius,
                RadiusY = radius,
                Rect = new RawRectangleF(left, top, right, bottom)
            };

            _device.DrawRoundedRectangle(rect, brush.Brush, stroke, _strokeStyle);
        }

        /// <summary>
        /// Draws a rectangle with rounded edges and dashed lines by using the given brush and dimension.
        /// </summary>
        /// <param name="brush">A brush that determines the color of the rectangle.</param>
        /// <param name="rectangle">A RoundedRectangle structure including the dimension of the rounded rectangle.</param>
        /// <param name="stroke">A value that determines the width/thickness of the line.</param>
        public void DashedRoundedRectangle(IBrush brush, RoundedRectangle rectangle, float stroke)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing anything");

            _device.DrawRoundedRectangle(rectangle, brush.Brush, stroke, _strokeStyle);
        }

        /// <summary>
        /// Fills a rounded rectangle using the given brush and dimension.
        /// </summary>
        /// <param name="brush">A brush that determines the color of the rectangle.</param>
        /// <param name="left">The x-coordinate of the upper-left corner of the rectangle.</param>
        /// <param name="top">The y-coordinate of the upper-left corner of the rectangle.</param>
        /// <param name="right">The x-coordinate of the lower-right corner of the rectangle.</param>
        /// <param name="bottom">The y-coordinate of the lower-right corner of the rectangle.</param>
        /// <param name="radius">A value that determines radius of corners.</param>
        public void FillRoundedRectangle(IBrush brush, float left, float top, float right, float bottom, float radius)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing anything");

            var rect = new SharpDX.Direct2D1.RoundedRectangle()
            {
                RadiusX = radius,
                RadiusY = radius,
                Rect = new RawRectangleF(left, top, right, bottom)
            };

            _device.FillRoundedRectangle(rect, brush.Brush);
        }

        /// <summary>
        /// Fills a rounded rectangle using the given brush and dimension.
        /// </summary>
        /// <param name="brush">A brush that determines the color of the rectangle.</param>
        /// <param name="rectangle">A RoundedRectangle structure including the dimension of the rounded rectangle.</param>
        public void FillRoundedRectangle(IBrush brush, RoundedRectangle rectangle)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing anything");
            
            _device.FillRoundedRectangle(rectangle, brush.Brush);
        }

        /// <summary>
        /// Draws a triangle using the given brush and dimension.
        /// </summary>
        /// <param name="brush">A brush that determines the color of the triangle.</param>
        /// <param name="aX">The x-coordinate lower-left corner of the triangle.</param>
        /// <param name="aY">The y-coordinate lower-left corner of the triangle.</param>
        /// <param name="bX">The x-coordinate lower-right corner of the triangle.</param>
        /// <param name="bY">The y-coordinate lower-right corner of the triangle.</param>
        /// <param name="cX">The x-coordinate upper-center corner of the triangle.</param>
        /// <param name="cY">The y-coordinate upper-center corner of the triangle.</param>
        /// <param name="stroke">A value that determines the width/thickness of the line.</param>
        public void DrawTriangle(IBrush brush, float aX, float aY, float bX, float bY, float cX, float cY, float stroke)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing anything");

            var geometry = new PathGeometry(_factory);

            var sink = geometry.Open();

            sink.BeginFigure(new RawVector2(aX, aY), FigureBegin.Hollow);
            sink.AddLine(new RawVector2(bX, bY));
            sink.AddLine(new RawVector2(cX, cY));
            sink.EndFigure(FigureEnd.Closed);

            sink.Close();

            _device.DrawGeometry(geometry, brush.Brush, stroke);

            sink.Dispose();
            geometry.Dispose();
        }

        /// <summary>
        /// Draws a triangle using the given brush and dimension.
        /// </summary>
        /// <param name="brush">A brush that determines the color of the triangle.</param>
        /// <param name="a">A Point structure including the coordinates of the lower-left corner of the triangle.</param>
        /// <param name="b">A Point structure including the coordinates of the lower-right corner of the triangle.</param>
        /// <param name="c">A Point structure including the coordinates of the upper-center corner of the triangle.</param>
        /// <param name="stroke">A value that determines the width/thickness of the line.</param>
        public void DrawTriangle(IBrush brush, Point a, Point b, Point c, float stroke) => DrawTriangle(brush, a.X, a.Y, b.X, b.Y, c.X, c.Y, stroke);

        /// <summary>
        /// Draws a triangle using the given brush and dimension.
        /// </summary>
        /// <param name="brush">A brush that determines the color of the triangle.</param>
        /// <param name="triangle">A Triangle structure including the dimension of the triangle.</param>
        /// <param name="stroke">A value that determines the width/thickness of the line.</param>
        public void DrawTriangle(IBrush brush, Triangle triangle, float stroke) => DrawTriangle(brush, triangle.A.X, triangle.A.Y, triangle.B.X, triangle.B.Y, triangle.C.X, triangle.C.Y, stroke);

        /// <summary>
        /// Draws a triangle with dashed lines using the given brush and dimension.
        /// </summary>
        /// <param name="brush">A brush that determines the color of the triangle.</param>
        /// <param name="aX">The x-coordinate lower-left corner of the triangle.</param>
        /// <param name="aY">The y-coordinate lower-left corner of the triangle.</param>
        /// <param name="bX">The x-coordinate lower-right corner of the triangle.</param>
        /// <param name="bY">The y-coordinate lower-right corner of the triangle.</param>
        /// <param name="cX">The x-coordinate upper-center corner of the triangle.</param>
        /// <param name="cY">The y-coordinate upper-center corner of the triangle.</param>
        /// <param name="stroke">A value that determines the width/thickness of the line.</param>
        public void DashedTriangle(IBrush brush, float aX, float aY, float bX, float bY, float cX, float cY, float stroke)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing anything");

            var geometry = new PathGeometry(_factory);

            var sink = geometry.Open();

            sink.BeginFigure(new RawVector2(aX, aY), FigureBegin.Hollow);
            sink.AddLine(new RawVector2(bX, bY));
            sink.AddLine(new RawVector2(cX, cY));
            sink.EndFigure(FigureEnd.Closed);

            sink.Close();

            _device.DrawGeometry(geometry, brush.Brush, stroke, _strokeStyle);

            sink.Dispose();
            geometry.Dispose();
        }

        /// <summary>
        /// Draws a triangle with dashed lines using the given brush and dimension.
        /// </summary>
        /// <param name="brush">A brush that determines the color of the triangle.</param>
        /// <param name="a">A Point structure including the coordinates of the lower-left corner of the triangle.</param>
        /// <param name="b">A Point structure including the coordinates of the lower-right corner of the triangle.</param>
        /// <param name="c">A Point structure including the coordinates of the upper-center corner of the triangle.</param>
        /// <param name="stroke">A value that determines the width/thickness of the line.</param>
        public void DashedTriangle(IBrush brush, Point a, Point b, Point c, float stroke) => DashedTriangle(brush, a.X, a.Y, b.X, b.Y, c.X, c.Y, stroke);

        /// <summary>
        /// Draws a triangle with dashed lines using the given brush and dimension.
        /// </summary>
        /// <param name="brush">A brush that determines the color of the triangle.</param>
        /// <param name="triangle">A Triangle structure including the dimension of the triangle.</param>
        /// <param name="stroke">A value that determines the width/thickness of the line.</param>
        public void DashedTriangle(IBrush brush, Triangle triangle, float stroke) => DashedTriangle(brush, triangle.A.X, triangle.A.Y, triangle.B.X, triangle.B.Y, triangle.C.X, triangle.C.Y, stroke);

        /// <summary>
        /// Fills a triangle using the given brush and dimension.
        /// </summary>
        /// <param name="brush">A brush that determines the color of the triangle.</param>
        /// <param name="aX">The x-coordinate lower-left corner of the triangle.</param>
        /// <param name="aY">The y-coordinate lower-left corner of the triangle.</param>
        /// <param name="bX">The x-coordinate lower-right corner of the triangle.</param>
        /// <param name="bY">The y-coordinate lower-right corner of the triangle.</param>
        /// <param name="cX">The x-coordinate upper-center corner of the triangle.</param>
        /// <param name="cY">The y-coordinate upper-center corner of the triangle.</param>
        public void FillTriangle(IBrush brush, float aX, float aY, float bX, float bY, float cX, float cY)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing anything");

            var geometry = new PathGeometry(_factory);

            var sink = geometry.Open();

            sink.BeginFigure(new RawVector2(aX, aY), FigureBegin.Filled);
            sink.AddLine(new RawVector2(bX, bY));
            sink.AddLine(new RawVector2(cX, cY));
            sink.EndFigure(FigureEnd.Closed);

            sink.Close();

            _device.FillGeometry(geometry, brush.Brush);

            sink.Dispose();
            geometry.Dispose();
        }

        /// <summary>
        /// Fills a triangle using the given brush and dimension.
        /// </summary>
        /// <param name="brush">A brush that determines the color of the triangle.</param>
        /// <param name="a">A Point structure including the coordinates of the lower-left corner of the triangle.</param>
        /// <param name="b">A Point structure including the coordinates of the lower-right corner of the triangle.</param>
        /// <param name="c">A Point structure including the coordinates of the upper-center corner of the triangle.</param>
        public void FillTriangle(IBrush brush, Point a, Point b, Point c) => FillTriangle(brush, a.X, a.Y, b.X, b.Y, c.X, c.Y);

        /// <summary>
        /// Fills a triangle using the given brush and dimension.
        /// </summary>
        /// <param name="brush">A brush that determines the color of the triangle.</param>
        /// <param name="triangle">A Triangle structure including the dimension of the triangle.</param>
        public void FillTriangle(IBrush brush, Triangle triangle) => FillTriangle(brush, triangle.A.X, triangle.A.Y, triangle.B.X, triangle.B.Y, triangle.C.X, triangle.C.Y);

        /// <summary>
        /// Draws a horizontal progrss bar using the given brush, dimension and percentage value.
        /// </summary>
        /// <param name="outline">A brush that determines the color of the outline.</param>
        /// <param name="fill">A brush that determines the color of the progress bar.</param>
        /// <param name="left">The x-coordinate of the upper-left corner of the rectangle.</param>
        /// <param name="top">The y-coordinate of the upper-left corner of the rectangle.</param>
        /// <param name="right">The x-coordinate of the lower-right corner of the rectangle.</param>
        /// <param name="bottom">The y-coordinate of the lower-right corner of the rectangle.</param>
        /// <param name="stroke">A value that determines the width/thickness of the line.</param>
        /// <param name="percentage">A value indicating the progress in percent.</param>
        public void DrawHorizontalProgressBar(IBrush outline, IBrush fill, float left, float top, float right, float bottom, float stroke, float percentage)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing anything");

            var outer = new RawRectangleF(left, top, right, bottom);

            if (percentage < 1.0f)
            {
                _device.DrawRectangle(outer, outline.Brush, stroke);
            }
            else
            {
                float height = bottom - top;
                float filledHeight = (height / 100.0f) * percentage;

                float halfStroke = stroke * 0.5f;

                var inner = new RawRectangleF(left + halfStroke, top + (height - filledHeight) + halfStroke, right - halfStroke, bottom - halfStroke);

                _device.FillRectangle(inner, fill.Brush);
                _device.DrawRectangle(outer, outline.Brush, stroke);
            }
        }

        /// <summary>
        /// Draws a horizontal progrss bar using the given brush, dimension and percentage value.
        /// </summary>
        /// <param name="outline">A brush that determines the color of the outline.</param>
        /// <param name="fill">A brush that determines the color of the progress bar.</param>
        /// <param name="rectangle">A Rectangle structure including the dimension of the rectangle.</param>
        /// <param name="stroke">A value that determines the width/thickness of the line.</param>
        /// <param name="percentage">A value indicating the progress in percent.</param>
        public void DrawHorizontalProgressBar(IBrush outline, IBrush fill, Rectangle rectangle, float stroke, float percentage) => DrawHorizontalProgressBar(outline, fill, rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom, stroke, percentage);

        /// <summary>
        /// Draws a vertical progrss bar using the given brush, dimension and percentage value.
        /// </summary>
        /// <param name="outline">A brush that determines the color of the outline.</param>
        /// <param name="fill">A brush that determines the color of the progress bar.</param>
        /// <param name="left">The x-coordinate of the upper-left corner of the rectangle.</param>
        /// <param name="top">The y-coordinate of the upper-left corner of the rectangle.</param>
        /// <param name="right">The x-coordinate of the lower-right corner of the rectangle.</param>
        /// <param name="bottom">The y-coordinate of the lower-right corner of the rectangle.</param>
        /// <param name="stroke">A value that determines the width/thickness of the line.</param>
        /// <param name="percentage">A value indicating the progress in percent.</param>
        public void DrawVerticalProgressBar(IBrush outline, IBrush fill, float left, float top, float right, float bottom, float stroke, float percentage)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing anything");
            
            var outer = new RawRectangleF(left, top, right, bottom);

            if (percentage < 1.0f)
            {
                _device.DrawRectangle(outer, outline.Brush, stroke);
            }
            else
            {
                float width = right - left;
                float filledWidth = (width / 100.0f) * percentage;

                float halfStroke = stroke * 0.5f;

                var inner = new RawRectangleF(left + halfStroke, top + halfStroke, right - (width - filledWidth) - halfStroke, bottom - halfStroke);

                _device.FillRectangle(inner, fill.Brush);
                _device.DrawRectangle(outer, outline.Brush, stroke);
            }
        }

        /// <summary>
        /// Draws a vertical progrss bar using the given brush, dimension and percentage value.
        /// </summary>
        /// <param name="outline">A brush that determines the color of the outline.</param>
        /// <param name="fill">A brush that determines the color of the progress bar.</param>
        /// <param name="rectangle">A Rectangle structure including the dimension of the rectangle.</param>
        /// <param name="stroke">A value that determines the width/thickness of the line.</param>
        /// <param name="percentage">A value indicating the progress in percent.</param>
        public void DrawVerticalProgressBar(IBrush outline, IBrush fill, Rectangle rectangle, float stroke, float percentage) => DrawVerticalProgressBar(outline, fill, rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom, stroke, percentage);

        /// <summary>
        /// Draws a crosshair by using the given brush and style.
        /// </summary>
        /// <param name="brush">A brush that determines the color of the crosshair.</param>
        /// <param name="x">The x-coordinate of the center of the crosshair.</param>
        /// <param name="y">The y-coordinate of the center of the crosshair.</param>
        /// <param name="size">The size of the crosshair in pixels.</param>
        /// <param name="stroke">A value that determines the width/thickness of the line.</param>
        /// <param name="style">A value that determines the appearance of the crosshair.</param>
        public void DrawCrosshair(IBrush brush, float x, float y, float size, float stroke, CrosshairStyle style)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing anything");

            if (style == CrosshairStyle.Dot)
            {
                FillCircle(brush, x, y, size);
            }
            else if (style == CrosshairStyle.Plus)
            {
                DrawLine(brush, x - size, y, x + size, y, stroke);
                DrawLine(brush, x, y - size, x, y + size, stroke);
            }
            else if (style == CrosshairStyle.Cross)
            {
                DrawLine(brush, x - size, y - size, x + size, y + size, stroke);
                DrawLine(brush, x + size, y - size, x - size, y + size, stroke);
            }
            else if (style == CrosshairStyle.Gap)
            {
                DrawLine(brush, x - size - stroke, y, x - stroke, y, stroke);
                DrawLine(brush, x + size + stroke, y, x + stroke, y, stroke);

                DrawLine(brush, x, y - size - stroke, x, y - stroke, stroke);
                DrawLine(brush, x, y + size + stroke, x, y + stroke, stroke);
            }
            else if (style == CrosshairStyle.Diagonal)
            {
                DrawLine(brush, x - size, y - size, x + size, y + size, stroke);
                DrawLine(brush, x + size, y - size, x - size, y + size, stroke);
            }
        }

        /// <summary>
        /// Draws a crosshair by using the given brush and style.
        /// </summary>
        /// <param name="brush">A brush that determines the color of the crosshair.</param>
        /// <param name="location">A Location structure including the position of the crosshair.</param>
        /// <param name="size">The size of the crosshair in pixels.</param>
        /// <param name="stroke">A value that determines the width/thickness of the line.</param>
        /// <param name="style">A value that determines the appearance of the crosshair.</param>
        public void DrawCrosshair(IBrush brush, Point location, float size, float stroke, CrosshairStyle style) => DrawCrosshair(brush, location.X, location.Y, size, stroke, style);

        /// <summary>
        /// Draws a pointed line using the given brush and dimension.
        /// </summary>
        /// <param name="brush">A brush that determines the color of the arrow line.</param>
        /// <param name="startX">The x-coordinate of the start of the arrow line. (the direction it points to)</param>
        /// <param name="startY">The y-coordinate of the start of the arrow line. (the direction it points to)</param>
        /// <param name="endX">The x-coordinate of the end of the arrow line.</param>
        /// <param name="endY">The y-coordinate of the end of the arrow line.</param>
        /// <param name="size">A value determining the size of the arrow line.</param>
        public void DrawArrowLine(IBrush brush, float startX, float startY, float endX, float endY, float size)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing anything");

            float deltaX = endX >= startX ? endX - startX : startX - endX;
            float deltaY = endY >= startY ? endY - startY : startY - endY;

            float length = (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

            float xm = length - size;
            float xn = xm;

            float ym = size;
            float yn = -ym;

            float sin = deltaY / length;
            float cos = deltaX / length;

            float x = xm * cos - ym * sin + endX;
            ym = xm * sin + ym * cos + endY;
            xm = x;

            x = xn * cos - yn * sin + endX;
            yn = xn * sin + yn * cos + endY;
            xn = x;

            FillTriangle(brush, startX, startY, xm, ym, xn, yn);
        }

        /// <summary>
        /// Draws a pointed line using the given brush and dimension.
        /// </summary>
        /// <param name="brush">A brush that determines the color of the arrow line.</param>
        /// <param name="start">A Point structure including the start position of the arrow line. (the direction it points to)</param>
        /// <param name="end">A Point structure including the end position of the arrow line. (the direction it points to)</param>
        /// <param name="size">A value determining the size of the arrow line.</param>
        public void DrawArrowLine(IBrush brush, Point start, Point end, float size) => DrawArrowLine(brush, start.X, start.Y, end.X, end.Y, size);

        /// <summary>
        /// Draws a pointed line using the given brush and dimension.
        /// </summary>
        /// <param name="brush">A brush that determines the color of the arrow line.</param>
        /// <param name="line">A Line structure including the start (direction) and end point of the arrow line.</param>
        /// <param name="size">A value determining the size of the arrow line.</param>
        public void DrawArrowLine(IBrush brush, Line line, float size) => DrawArrowLine(brush, line.Start.X, line.Start.Y, line.End.X, line.End.Y, size);

        /// <summary>
        /// Draws a 2D Box with an outline using the given brush and dimension.
        /// </summary>
        /// <param name="outline">A brush that determines the color of the outline.</param>
        /// <param name="fill">A brush that determines the color of the rectangle.</param>
        /// <param name="left">The x-coordinate of the upper-left corner of the rectangle.</param>
        /// <param name="top">The y-coordinate of the upper-left corner of the rectangle.</param>
        /// <param name="right">The x-coordinate of the lower-right corner of the rectangle.</param>
        /// <param name="bottom">The y-coordinate of the lower-right corner of the rectangle.</param>
        /// <param name="stroke">A value that determines the width/thickness of the line.</param>
        public void DrawBox2D(IBrush outline, IBrush fill, float left, float top, float right, float bottom, float stroke)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing anything");

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
            _device.DrawGeometry(geometry, outline.Brush, stroke);
            _device.FillGeometry(geometry, fill.Brush);
            

            sink.Dispose();
            geometry.Dispose();
        }

        /// <summary>
        /// Draws a 2D Box with an outline using the given brush and dimension.
        /// </summary>
        /// <param name="outline">A brush that determines the color of the outline.</param>
        /// <param name="fill">A brush that determines the color of the rectangle.</param>
        /// <param name="rectangle">A Rectangle structure including the dimension of the rectangle.</param>
        /// <param name="stroke">A value that determines the width/thickness of the line.</param>
        public void DrawBox2D(IBrush outline, IBrush fill, Rectangle rectangle, float stroke) => DrawBox2D(outline, fill, rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom, stroke);

        /// <summary>
        /// Draws the corners (edges) of a rectangle using the given brush and dimension.
        /// </summary>
        /// <param name="brush">A brush that determines the color of the rectangle.</param>
        /// <param name="left">The x-coordinate of the upper-left corner of the rectangle.</param>
        /// <param name="top">The y-coordinate of the upper-left corner of the rectangle.</param>
        /// <param name="right">The x-coordinate of the lower-right corner of the rectangle.</param>
        /// <param name="bottom">The y-coordinate of the lower-right corner of the rectangle.</param>
        /// <param name="stroke">A value that determines the width/thickness of the line.</param>
        public void DrawRectangleEdges(IBrush brush, float left, float top, float right, float bottom, float stroke)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing anything");

            float width = right - left;
            float height = bottom - top;

            int length = (int)((width + height) / 2.0f * 0.2f);

            var first = new RawVector2(left, top);
            var second = new RawVector2(left, top + length);
            var third = new RawVector2(left + length, top);

            _device.DrawLine(first, second, brush.Brush, stroke);
            _device.DrawLine(first, third, brush.Brush, stroke);

            first.Y += height;
            second.Y = first.Y - length;
            third.Y = first.Y;
            third.X = first.X + length;

            _device.DrawLine(first, second, brush.Brush, stroke);
            _device.DrawLine(first, third, brush.Brush, stroke);

            first.X = left + width;
            first.Y = top;
            second.X = first.X - length;
            second.Y = first.Y;
            third.X = first.X;
            third.Y = first.Y + length;

            _device.DrawLine(first, second, brush.Brush, stroke);
            _device.DrawLine(first, third, brush.Brush, stroke);

            first.Y += height;
            second.X += length;
            second.Y = first.Y - length;
            third.Y = first.Y;
            third.X = first.X - length;

            _device.DrawLine(first, second, brush.Brush, stroke);
            _device.DrawLine(first, third, brush.Brush, stroke);
        }

        /// <summary>
        /// Draws the corners (edges) of a rectangle using the given brush and dimension.
        /// </summary>
        /// <param name="brush">A brush that determines the color of the rectangle.</param>
        /// <param name="rectangle">A Rectangle structure including the dimension of the rectangle.</param>
        /// <param name="stroke">A value that determines the width/thickness of the line.</param>
        public void DrawRectangleEdges(IBrush brush, Rectangle rectangle, float stroke) => DrawRectangle(brush, rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom, stroke);

        /// <summary>
        /// Draws a string using the given font, size and position.
        /// </summary>
        /// <param name="font">The Font to be used to draw the string.</param>
        /// <param name="fontSize">The size of the Font. (does not need to be the same as in Font.FontSize)</param>
        /// <param name="brush">A brush that determines the color of the text.</param>
        /// <param name="x">The x-coordinate of the starting position.</param>
        /// <param name="y">The y-coordinate of the starting position.</param>
        /// <param name="text">The string to be drawn.</param>
        public void DrawText(Font font, float fontSize, IBrush brush, float x, float y, string text)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing anything");

            if (text == null) throw new ArgumentNullException(nameof(text));
            if (text == string.Empty) return;

            float clippedWidth = Width - x;
            float clippedHeight = Height - y;

            if (clippedWidth <= fontSize)
            {
                clippedWidth = Width;
            }
            if (clippedHeight <= fontSize)
            {
                clippedHeight = Height;
            }

            var layout = new TextLayout(_fontFactory, text, font.TextFormat, clippedWidth, clippedHeight);

            if (fontSize != font.FontSize)
            {
                layout.SetFontSize(fontSize, new TextRange(0, text.Length));
            }
            
            _device.DrawTextLayout(new RawVector2(x, y), layout, brush.Brush, DrawTextOptions.Clip);

            layout.Dispose();
        }

        /// <summary>
        /// Draws a string using the given font, size and position.
        /// </summary>
        /// <param name="font">The Font to be used to draw the string.</param>
        /// <param name="fontSize">The size of the Font. (does not need to be the same as in Font.FontSize)</param>
        /// <param name="brush">A brush that determines the color of the text.</param>
        /// <param name="location">A Point structure including the starting position.</param>
        /// <param name="text">The string to be drawn.</param>
        public void DrawText(Font font, float fontSize, IBrush brush, Point location, string text) => DrawText(font, fontSize, brush, location.X, location.Y, text);

        /// <summary>
        /// Draws a string using the given font and position.
        /// </summary>
        /// <param name="font">The Font to be used to draw the string.</param>
        /// <param name="brush">A brush that determines the color of the text.</param>
        /// <param name="x">The x-coordinate of the starting position.</param>
        /// <param name="y">The y-coordinate of the starting position.</param>
        /// <param name="text">The string to be drawn.</param>
        public void DrawText(Font font, IBrush brush, float x, float y, string text) => DrawText(font, font.FontSize, brush, x, y, text);

        /// <summary>
        /// Draws a string using the given font and position.
        /// </summary>
        /// <param name="font">The Font to be used to draw the string.</param>
        /// <param name="brush">A brush that determines the color of the text.</param>
        /// <param name="location">A Point structure including the starting position.</param>
        /// <param name="text">The string to be drawn.</param>
        public void DrawText(Font font, IBrush brush, Point location, string text) => DrawText(font, font.FontSize, brush, location.X, location.Y, text);

        /// <summary>
        /// Draws a string with a background box in behind using the given font, size and position.
        /// </summary>
        /// <param name="font">The Font to be used to draw the string.</param>
        /// <param name="fontSize">The size of the Font. (does not need to be the same as in Font.FontSize)</param>
        /// <param name="brush">A brush that determines the color of the text.</param>
        /// <param name="background">A brush that determines the color of the background box.</param>
        /// <param name="x">The x-coordinate of the starting position.</param>
        /// <param name="y">The y-coordinate of the starting position.</param>
        /// <param name="text">The string to be drawn.</param>
        public void DrawTextWithBackground(Font font, float fontSize, IBrush brush, IBrush background, float x, float y, string text)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing anything");
            
            if (text == null) throw new ArgumentNullException(nameof(text));
            if (text == string.Empty) return;

            float clippedWidth = Width - x;
            float clippedHeight = Height - y;

            if (clippedWidth <= fontSize)
            {
                clippedWidth = Width;
            }
            if (clippedHeight <= fontSize)
            {
                clippedHeight = Height;
            }

            var layout = new TextLayout(_fontFactory, text, font.TextFormat, clippedWidth, clippedHeight);

            if (fontSize != font.FontSize)
            {
                layout.SetFontSize(fontSize, new TextRange(0, text.Length));
            }

            float modifier = layout.FontSize * 0.25f;
            var rectangle = new RawRectangleF(x - modifier, y - modifier, x + layout.Metrics.Width + modifier, y + layout.Metrics.Height + modifier);

            _device.FillRectangle(rectangle, background.Brush);

            _device.DrawTextLayout(new RawVector2(x, y), layout, brush.Brush, DrawTextOptions.Clip);

            layout.Dispose();
        }

        /// <summary>
        /// Draws a string with a background box in behind using the given font, size and position.
        /// </summary>
        /// <param name="font">The Font to be used to draw the string.</param>
        /// <param name="fontSize">The size of the Font. (does not need to be the same as in Font.FontSize)</param>
        /// <param name="brush">A brush that determines the color of the text.</param>
        /// <param name="background">A brush that determines the color of the background box.</param>
        /// <param name="location">A Point structure including the starting position.</param>
        /// <param name="text">The string to be drawn.</param>
        public void DrawTextWithBackground(Font font, float fontSize, IBrush brush, IBrush background, Point location, string text) => DrawTextWithBackground(font, fontSize, brush, background, location.X, location.Y, text);

        /// <summary>
        /// Draws a string with a background box in behind using the given font, size and position.
        /// </summary>
        /// <param name="font">The Font to be used to draw the string.</param>
        /// <param name="brush">A brush that determines the color of the text.</param>
        /// <param name="background">A brush that determines the color of the background box.</param>
        /// <param name="x">The x-coordinate of the starting position.</param>
        /// <param name="y">The y-coordinate of the starting position.</param>
        /// <param name="text">The string to be drawn.</param>
        public void DrawTextWithBackground(Font font, IBrush brush, IBrush background, float x, float y, string text) => DrawTextWithBackground(font, font.FontSize, brush, background, x, y, text);

        /// <summary>
        /// Draws a string with a background box in behind using the given font, size and position.
        /// </summary>
        /// <param name="font">The Font to be used to draw the string.</param>
        /// <param name="brush">A brush that determines the color of the text.</param>
        /// <param name="background">A brush that determines the color of the background box.</param>
        /// <param name="location">A Point structure including the starting position.</param>
        /// <param name="text">The string to be drawn.</param>
        public void DrawTextWithBackground(Font font, IBrush brush, IBrush background, Point location, string text) => DrawTextWithBackground(font, font.FontSize, brush, background, location.X, location.Y, text);

        /// <summary>
        /// Draws an image to the given position and optional applies an alpha value.
        /// </summary>
        /// <param name="image">The Image to be drawn.</param>
        /// <param name="x">The x-coordinate upper-left corner of the image.</param>
        /// <param name="y">The y-coordinate upper-left corner of the image.</param>
        /// <param name="opacity">A value indicating the opacity of the image. (alpha)</param>
        public void DrawImage(Image image, float x, float y, float opacity = 1.0f)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing anything");

            float destRight = x + image.Bitmap.PixelSize.Width;
            float destBottom = y + image.Bitmap.PixelSize.Height;

            _device.DrawBitmap(
                image.Bitmap,
                new RawRectangleF(x, y, destRight, destBottom),
                opacity,
                BitmapInterpolationMode.Linear);
        }

        /// <summary>
        /// Draws an image to the given position and optional applies an alpha value.
        /// </summary>
        /// <param name="image">The Image to be drawn.</param>
        /// <param name="location">A Point structure inclduing the position of the upper-left corner of the image.</param>
        /// <param name="opacity">A value indicating the opacity of the image. (alpha)</param>
        public void DrawImage(Image image, Point location, float opacity = 1.0f) => DrawImage(image, location.X, location.Y, opacity);

        /// <summary>
        /// Draws an image to the given position, scales it and optional applies an alpha value.
        /// </summary>
        /// <param name="image">The Image to be drawn.</param>
        /// <param name="rectangle">A Rectangle structure inclduing the dimension of the image.</param>
        /// <param name="opacity">A value indicating the opacity of the image. (alpha)</param>
        /// <param name="linearScale">A Boolean indicating whether linear scaling should be applied</param>
        public void DrawImage(Image image, Rectangle rectangle, float opacity = 1.0f, bool linearScale = true)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing anything");

            _device.DrawBitmap(
                image.Bitmap,
                rectangle,
                opacity,
                linearScale ? BitmapInterpolationMode.Linear : BitmapInterpolationMode.NearestNeighbor,
                new RawRectangleF(0, 0, image.Bitmap.PixelSize.Width, image.Bitmap.PixelSize.Height));
        }

        /// <summary>
        /// Draws an image to the given position, scales it and optional applies an alpha value.
        /// </summary>
        /// <param name="image">The Image to be drawn.</param>
        /// <param name="left">The x-coordinate of the upper-left corner of the image.</param>
        /// <param name="top">The y-coordinate of the upper-left corner of the image.</param>
        /// <param name="right">The x-coordinate of the lower-right corner of the image.</param>
        /// <param name="bottom">The y-coordinate of the lower-right corner of the image.</param>
        /// <param name="opacity">A value indicating the opacity of the image. (alpha)</param>
        /// <param name="linearScale">A Boolean indicating whether linear scaling should be applied</param>
        public void DrawImage(Image image, float left, float top, float right, float bottom, float opacity = 1.0f, bool linearScale = true) => DrawImage(image, new Rectangle(left, top, right, bottom), opacity, linearScale);

        /// <summary>
        /// Draws a Geometry using the given brush and thickness.
        /// </summary>
        /// <param name="geometry">The Geometry to be drawn.</param>
        /// <param name="brush">A brush that determines the color of the text.</param>
        /// <param name="stroke">A value that determines the width/thickness of the lines.</param>
        public void DrawGeometry(Geometry geometry, IBrush brush, float stroke)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing anything");

            _device.DrawGeometry(geometry, brush.Brush, stroke);
        }

        /// <summary>
        /// Draws a Geometry with dashed lines using the given brush and thickness.
        /// </summary>
        /// <param name="geometry">The Geometry to be drawn.</param>
        /// <param name="brush">A brush that determines the color of the text.</param>
        /// <param name="stroke">A value that determines the width/thickness of the lines.</param>
        public void DashedGeometry(Geometry geometry, IBrush brush, float stroke)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing anything");

            _device.DrawGeometry(geometry, brush.Brush, stroke, _strokeStyle);
        }

        /// <summary>
        /// Fills the Geometry using the given brush.
        /// </summary>
        /// <param name="geometry">The Geometry to be drawn.</param>
        /// <param name="brush">A brush that determines the color of the text.</param>
        public void FillGeometry(Geometry geometry, IBrush brush)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing anything");

            _device.FillGeometry(geometry, brush.Brush);
        }

        /// <summary>
        /// Fills the Mesh using the given brush.
        /// </summary>
        /// <param name="mesh">The Mesh to be drawn.</param>
        /// <param name="brush">A brush that determines the color of the text.</param>
        public void FillMesh(Mesh mesh, IBrush brush)
        {
            if (!IsDrawing) throw new InvalidOperationException("Use BeginScene before drawing anything");

            _device.FillMesh(mesh, brush.Brush);
        }
        
        /// <summary>
        /// Gets the RenderTarget used by this Graphics surface.
        /// </summary>
        /// <returns>The RenderTarget of this Graphics surface.</returns>
        public RenderTarget GetRenderTarget()
        {
            if (!IsInitialized) throw new InvalidOperationException("The DirectX device is not initialized");

            return _device;
        }

        /// <summary>
        /// Gets the Factory used by this Graphics surface.
        /// </summary>
        /// <returns>The Factory of this Graphics surface.</returns>
        public Factory GetFactory()
        {
            if (!IsInitialized) throw new InvalidOperationException("The DirectX device is not initialized");

            return _factory;
        }

        /// <summary>
        /// Gets the FontFactory used by this Graphics surface.
        /// </summary>
        /// <returns>The FontFactory of this Graphics surface.</returns>
        public FontFactory GetFontFactory()
        {
            if (!IsInitialized) throw new InvalidOperationException("The DirectX device is not initialized");

            return _fontFactory;
        }

        #region IDisposable Support
        private bool disposedValue = false;

        /// <summary>
        /// Releases all resources used by this Graphics surface.
        /// </summary>
        /// <param name="disposing">A Boolean value indicating whether this is called from the destructor.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (IsInitialized)
                {
                    if (IsDrawing)
                    {
                        try
                        {
                            _device.EndDraw();
                        }
                        catch { }
                    }

                    Destroy();
                }

                disposedValue = true;
            }
        }

        /// <summary>
        /// Releases all resources used by this Graphics surface.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
