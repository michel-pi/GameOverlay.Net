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
    public class Direct2DRenderer : IDisposable
    {
        #region private vars

        private WindowRenderTarget device;
        private HwndRenderTargetProperties deviceProperties;
        private Factory factory;
        private FontFactory fontFactory;
        private int internalFps;
        private bool isDrawing;
        private RendererOptions rendererOptions;
        private bool resize;
        private int resizeHeight;
        private int resizeWidth;
        private SolidColorBrush sharedBrush;
        private TextFormat sharedFont;
        private Stopwatch stopwatch = new Stopwatch();

        #endregion private vars

        #region public vars

        public int FPS { get; private set; }
        public int Height { get; private set; }
        public bool MeasureFPS { get; set; }
        public IntPtr RenderTargetHwnd { get; private set; }
        public bool VSync { get; private set; }
        public int Width { get; private set; }

        #endregion public vars

        #region construct & destruct

        private Direct2DRenderer()
        {
            throw new NotSupportedException();
        }

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

        public Direct2DRenderer(RendererOptions options)
        {
            SetupInstance(options);
        }

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
                sharedBrush.Dispose();
                fontFactory.Dispose();
                factory.Dispose();
                device.Dispose();
            }
            catch
            {
            }
        }

        private void SetupInstance(RendererOptions options)
        {
            rendererOptions = options;

            if (options.Hwnd == IntPtr.Zero) throw new ArgumentNullException(nameof(options.Hwnd));

            if (User32.IsWindow(options.Hwnd) == 0) throw new ArgumentException("The window does not exist (hwnd = 0x" + options.Hwnd.ToString("X") + ")");

            RECT bounds = new RECT();

            if (HelperMethods.GetRealWindowRect(options.Hwnd, out bounds) == 0) throw new Exception("Failed to get the size of the given window (hwnd = 0x" + options.Hwnd.ToString("X") + ")");

            this.Width = bounds.Right - bounds.Left;
            this.Height = bounds.Bottom - bounds.Top;

            this.VSync = options.VSync;
            this.MeasureFPS = options.MeasureFps;

            deviceProperties = new HwndRenderTargetProperties()
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

            factory = new Factory();
            fontFactory = new FontFactory();

            try
            {
                device = new WindowRenderTarget(factory, renderProperties, deviceProperties);
            }
            catch (SharpDXException) // D2DERR_UNSUPPORTED_PIXEL_FORMAT
            {
                renderProperties.PixelFormat = new PixelFormat(Format.B8G8R8A8_UNorm, SharpDX.Direct2D1.AlphaMode.Premultiplied);
                device = new WindowRenderTarget(factory, renderProperties, deviceProperties);
            }

            device.AntialiasMode = AntialiasMode.Aliased; // AntialiasMode.PerPrimitive fails rendering some objects
            // other than in the documentation: Cleartype is much faster for me than GrayScale
            device.TextAntialiasMode = options.AntiAliasing ? SharpDX.Direct2D1.TextAntialiasMode.Cleartype : SharpDX.Direct2D1.TextAntialiasMode.Aliased;

            sharedBrush = new SolidColorBrush(device, default(RawColor4));
        }

        #endregion init & delete

        #region Scenes

        public void BeginScene()
        {
            if (device == null) return;
            if (isDrawing) return;

            if (MeasureFPS && !stopwatch.IsRunning)
            {
                stopwatch.Restart();
            }

            if (resize)
            {
                device.Resize(new Size2(resizeWidth, resizeHeight));
                resize = false;
            }

            device.BeginDraw();

            isDrawing = true;
        }

        public void ClearScene()
        {
            device.Clear(null);
        }

        public void ClearScene(Direct2DColor color)
        {
            device.Clear(color);
        }

        public void ClearScene(Direct2DBrush brush)
        {
            device.Clear(brush);
        }

        public void EndScene()
        {
            if (device == null) return;
            if (!isDrawing) return;

            var result = device.TryEndDraw(out long tag_0, out long tag_1);

            if (result.Failure)
            {
                DestroyInstance();
                SetupInstance(rendererOptions);
            }

            if (MeasureFPS && stopwatch.IsRunning)
            {
                internalFps++;

                if (stopwatch.ElapsedMilliseconds > 1000)
                {
                    FPS = internalFps;
                    internalFps = 0;
                    stopwatch.Stop();
                }
            }

            isDrawing = false;
        }

        public void Resize(int width, int height)
        {
            resizeWidth = width;
            resizeHeight = height;
            resize = true;
        }

        public Direct2DScene UseScene()
        {
            // really expensive to use but i like the pattern
            return new Direct2DScene(this);
        }

        #endregion Scenes

        #region Fonts & Brushes & Bitmaps

        public Direct2DBrush CreateBrush(Direct2DColor color)
        {
            return new Direct2DBrush(device, color);
        }

        public Direct2DBrush CreateBrush(int r, int g, int b, int a = 255)
        {
            return new Direct2DBrush(device, new Direct2DColor(r, g, b, a));
        }

        public Direct2DBrush CreateBrush(float r, float g, float b, float a = 1.0f)
        {
            return new Direct2DBrush(device, new Direct2DColor(r, g, b, a));
        }

        public Direct2DFont CreateFont(string fontFamilyName, float size, bool bold = false, bool italic = false)
        {
            return new Direct2DFont(fontFactory, fontFamilyName, size, bold, italic);
        }

        public Direct2DFont CreateFont(FontCreationOptions options)
        {
            TextFormat font = new TextFormat(fontFactory, options.FontFamilyName, options.Bold ? FontWeight.Bold : FontWeight.Normal, options.GetStyle(), options.FontSize)
            {
                WordWrapping = options.WordWrapping ? WordWrapping.Wrap : WordWrapping.NoWrap
            };
            return new Direct2DFont(font);
        }

        public Direct2DBitmap LoadBitmap(string file)
        {
            return new Direct2DBitmap(device, file);
        }

        public Direct2DBitmap LoadBitmap(byte[] bytes)
        {
            return new Direct2DBitmap(device, bytes);
        }

        public void SetSharedFont(string fontFamilyName, float size, bool bold = false, bool italic = false)
        {
            sharedFont = new TextFormat(fontFactory, fontFamilyName, bold ? FontWeight.Bold : FontWeight.Normal, italic ? FontStyle.Italic : FontStyle.Normal, size)
            {
                WordWrapping = SharpDX.DirectWrite.WordWrapping.NoWrap
            };
        }

        #endregion Fonts & Brushes & Bitmaps

        #region Primitives

        public void DrawCircle(float x, float y, float radius, float stroke, Direct2DBrush brush)
        {
            device.DrawEllipse(new Ellipse(new RawVector2(x, y), radius, radius), brush, stroke);
        }

        public void DrawCircle(float x, float y, float radius, float stroke, Direct2DColor color)
        {
            sharedBrush.Color = color;
            device.DrawEllipse(new Ellipse(new RawVector2(x, y), radius, radius), sharedBrush, stroke);
        }

        public void DrawEllipse(float x, float y, float radius_x, float radius_y, float stroke, Direct2DBrush brush)
        {
            device.DrawEllipse(new Ellipse(new RawVector2(x, y), radius_x, radius_y), brush, stroke);
        }

        public void DrawEllipse(float x, float y, float radius_x, float radius_y, float stroke, Direct2DColor color)
        {
            sharedBrush.Color = color;
            device.DrawEllipse(new Ellipse(new RawVector2(x, y), radius_x, radius_y), sharedBrush, stroke);
        }

        public void DrawLine(float start_x, float start_y, float end_x, float end_y, float stroke, Direct2DBrush brush)
        {
            device.DrawLine(new RawVector2(start_x, start_y), new RawVector2(end_x, end_y), brush, stroke);
        }

        public void DrawLine(float start_x, float start_y, float end_x, float end_y, float stroke, Direct2DColor color)
        {
            sharedBrush.Color = color;
            device.DrawLine(new RawVector2(start_x, start_y), new RawVector2(end_x, end_y), sharedBrush, stroke);
        }

        public void DrawRectangle(float x, float y, float width, float height, float stroke, Direct2DBrush brush)
        {
            device.DrawRectangle(new RawRectangleF(x, y, x + width, y + height), brush, stroke);
        }

        public void DrawRectangle(float x, float y, float width, float height, float stroke, Direct2DColor color)
        {
            sharedBrush.Color = color;
            device.DrawRectangle(new RawRectangleF(x, y, x + width, y + height), sharedBrush, stroke);
        }

        public void DrawRectangleEdges(float x, float y, float width, float height, float stroke, Direct2DBrush brush)
        {
            int length = (int)(((width + height) / 2.0f) * 0.2f);

            RawVector2 first = new RawVector2(x, y);
            RawVector2 second = new RawVector2(x, y + length);
            RawVector2 third = new RawVector2(x + length, y);

            device.DrawLine(first, second, brush, stroke);
            device.DrawLine(first, third, brush, stroke);

            first.Y += height;
            second.Y = first.Y - length;
            third.Y = first.Y;
            third.X = first.X + length;

            device.DrawLine(first, second, brush, stroke);
            device.DrawLine(first, third, brush, stroke);

            first.X = x + width;
            first.Y = y;
            second.X = first.X - length;
            second.Y = first.Y;
            third.X = first.X;
            third.Y = first.Y + length;

            device.DrawLine(first, second, brush, stroke);
            device.DrawLine(first, third, brush, stroke);

            first.Y += height;
            second.X += length;
            second.Y = first.Y - length;
            third.Y = first.Y;
            third.X = first.X - length;

            device.DrawLine(first, second, brush, stroke);
            device.DrawLine(first, third, brush, stroke);
        }

        public void DrawRectangleEdges(float x, float y, float width, float height, float stroke, Direct2DColor color)
        {
            sharedBrush.Color = color;

            int length = (int)(((width + height) / 2.0f) * 0.2f);

            RawVector2 first = new RawVector2(x, y);
            RawVector2 second = new RawVector2(x, y + length);
            RawVector2 third = new RawVector2(x + length, y);

            device.DrawLine(first, second, sharedBrush, stroke);
            device.DrawLine(first, third, sharedBrush, stroke);

            first.Y += height;
            second.Y = first.Y - length;
            third.Y = first.Y;
            third.X = first.X + length;

            device.DrawLine(first, second, sharedBrush, stroke);
            device.DrawLine(first, third, sharedBrush, stroke);

            first.X = x + width;
            first.Y = y;
            second.X = first.X - length;
            second.Y = first.Y;
            third.X = first.X;
            third.Y = first.Y + length;

            device.DrawLine(first, second, sharedBrush, stroke);
            device.DrawLine(first, third, sharedBrush, stroke);

            first.Y += height;
            second.X += length;
            second.Y = first.Y - length;
            third.Y = first.Y;
            third.X = first.X - length;

            device.DrawLine(first, second, sharedBrush, stroke);
            device.DrawLine(first, third, sharedBrush, stroke);
        }

        #endregion Primitives

        #region Filled

        public void FillCircle(float x, float y, float radius, Direct2DBrush brush)
        {
            device.FillEllipse(new Ellipse(new RawVector2(x, y), radius, radius), brush);
        }

        public void FillCircle(float x, float y, float radius, Direct2DColor color)
        {
            sharedBrush.Color = color;
            device.FillEllipse(new Ellipse(new RawVector2(x, y), radius, radius), sharedBrush);
        }

        public void FillEllipse(float x, float y, float radius_x, float radius_y, Direct2DBrush brush)
        {
            device.FillEllipse(new Ellipse(new RawVector2(x, y), radius_x, radius_y), brush);
        }

        public void FillEllipse(float x, float y, float radius_x, float radius_y, Direct2DColor color)
        {
            sharedBrush.Color = color;
            device.FillEllipse(new Ellipse(new RawVector2(x, y), radius_x, radius_y), sharedBrush);
        }

        public void FillRectangle(float x, float y, float width, float height, Direct2DBrush brush)
        {
            device.FillRectangle(new RawRectangleF(x, y, x + width, y + height), brush);
        }

        public void FillRectangle(float x, float y, float width, float height, Direct2DColor color)
        {
            sharedBrush.Color = color;
            device.FillRectangle(new RawRectangleF(x, y, x + width, y + height), sharedBrush);
        }

        #endregion Filled

        #region Bordered

        public void BorderedCircle(float x, float y, float radius, float stroke, Direct2DColor color, Direct2DColor borderColor)
        {
            sharedBrush.Color = color;

            var ellipse = new Ellipse(new RawVector2(x, y), radius, radius);

            device.DrawEllipse(ellipse, sharedBrush, stroke);

            float half = stroke / 2.0f;

            sharedBrush.Color = borderColor;

            ellipse.RadiusX += half;
            ellipse.RadiusY += half;

            device.DrawEllipse(ellipse, sharedBrush, half);

            ellipse.RadiusX -= stroke;
            ellipse.RadiusY -= stroke;

            device.DrawEllipse(ellipse, sharedBrush, half);
        }

        public void BorderedCircle(float x, float y, float radius, float stroke, Direct2DBrush brush, Direct2DBrush borderBrush)
        {
            var ellipse = new Ellipse(new RawVector2(x, y), radius, radius);

            device.DrawEllipse(ellipse, brush, stroke);

            float half = stroke / 2.0f;

            ellipse.RadiusX += half;
            ellipse.RadiusY += half;

            device.DrawEllipse(ellipse, borderBrush, half);

            ellipse.RadiusX -= stroke;
            ellipse.RadiusY -= stroke;

            device.DrawEllipse(ellipse, borderBrush, half);
        }

        public void BorderedLine(float start_x, float start_y, float end_x, float end_y, float stroke, Direct2DColor color, Direct2DColor borderColor)
        {
            var geometry = new PathGeometry(factory);

            var sink = geometry.Open();

            float half = stroke / 2.0f;
            float quarter = half / 2.0f;

            sink.BeginFigure(new RawVector2(start_x, start_y - half), FigureBegin.Filled);

            sink.AddLine(new RawVector2(end_x, end_y - half));
            sink.AddLine(new RawVector2(end_x, end_y + half));
            sink.AddLine(new RawVector2(start_x, start_y + half));

            sink.EndFigure(FigureEnd.Closed);

            sink.Close();

            sharedBrush.Color = borderColor;

            device.DrawGeometry(geometry, sharedBrush, half);

            sharedBrush.Color = color;

            device.FillGeometry(geometry, sharedBrush);

            sink.Dispose();
            geometry.Dispose();
        }

        public void BorderedLine(float start_x, float start_y, float end_x, float end_y, float stroke, Direct2DBrush brush, Direct2DBrush borderBrush)
        {
            var geometry = new PathGeometry(factory);

            var sink = geometry.Open();

            float half = stroke / 2.0f;
            float quarter = half / 2.0f;

            sink.BeginFigure(new RawVector2(start_x, start_y - half), FigureBegin.Filled);

            sink.AddLine(new RawVector2(end_x, end_y - half));
            sink.AddLine(new RawVector2(end_x, end_y + half));
            sink.AddLine(new RawVector2(start_x, start_y + half));

            sink.EndFigure(FigureEnd.Closed);

            sink.Close();

            device.DrawGeometry(geometry, borderBrush, half);

            device.FillGeometry(geometry, brush);

            sink.Dispose();
            geometry.Dispose();
        }

        public void BorderedRectangle(float x, float y, float width, float height, float stroke, Direct2DColor color, Direct2DColor borderColor)
        {
            float half = stroke / 2.0f;

            width += x;
            height += y;

            sharedBrush.Color = color;

            device.DrawRectangle(new RawRectangleF(x, y, width, height), sharedBrush, half);

            sharedBrush.Color = borderColor;

            device.DrawRectangle(new RawRectangleF(x - half, y - half, width + half, height + half), sharedBrush, half);

            device.DrawRectangle(new RawRectangleF(x + half, y + half, width - half, height - half), sharedBrush, half);
        }

        public void BorderedRectangle(float x, float y, float width, float height, float stroke, Direct2DBrush brush, Direct2DBrush borderBrush)
        {
            float half = stroke / 2.0f;

            width += x;
            height += y;

            device.DrawRectangle(new RawRectangleF(x - half, y - half, width + half, height + half), borderBrush, half);

            device.DrawRectangle(new RawRectangleF(x + half, y + half, width - half, height - half), borderBrush, half);

            device.DrawRectangle(new RawRectangleF(x, y, width, height), brush, half);
        }

        #endregion Bordered

        #region Geometry

        public void DrawTriangle(float a_x, float a_y, float b_x, float b_y, float c_x, float c_y, float stroke, Direct2DBrush brush)
        {
            var geometry = new PathGeometry(factory);

            var sink = geometry.Open();

            sink.BeginFigure(new RawVector2(a_x, a_y), FigureBegin.Hollow);
            sink.AddLine(new RawVector2(b_x, b_y));
            sink.AddLine(new RawVector2(c_x, c_y));
            sink.EndFigure(FigureEnd.Closed);

            sink.Close();

            device.DrawGeometry(geometry, brush, stroke);

            sink.Dispose();
            geometry.Dispose();
        }

        public void DrawTriangle(float a_x, float a_y, float b_x, float b_y, float c_x, float c_y, float stroke, Direct2DColor color)
        {
            sharedBrush.Color = color;

            var geometry = new PathGeometry(factory);

            var sink = geometry.Open();

            sink.BeginFigure(new RawVector2(a_x, a_y), FigureBegin.Hollow);
            sink.AddLine(new RawVector2(b_x, b_y));
            sink.AddLine(new RawVector2(c_x, c_y));
            sink.EndFigure(FigureEnd.Closed);

            sink.Close();

            device.DrawGeometry(geometry, sharedBrush, stroke);

            sink.Dispose();
            geometry.Dispose();
        }

        public void FillTriangle(float a_x, float a_y, float b_x, float b_y, float c_x, float c_y, Direct2DBrush brush)
        {
            var geometry = new PathGeometry(factory);

            var sink = geometry.Open();

            sink.BeginFigure(new RawVector2(a_x, a_y), FigureBegin.Filled);
            sink.AddLine(new RawVector2(b_x, b_y));
            sink.AddLine(new RawVector2(c_x, c_y));
            sink.EndFigure(FigureEnd.Closed);

            sink.Close();

            device.FillGeometry(geometry, brush);

            sink.Dispose();
            geometry.Dispose();
        }

        public void FillTriangle(float a_x, float a_y, float b_x, float b_y, float c_x, float c_y, Direct2DColor color)
        {
            sharedBrush.Color = color;

            var geometry = new PathGeometry(factory);

            var sink = geometry.Open();

            sink.BeginFigure(new RawVector2(a_x, a_y), FigureBegin.Filled);
            sink.AddLine(new RawVector2(b_x, b_y));
            sink.AddLine(new RawVector2(c_x, c_y));
            sink.EndFigure(FigureEnd.Closed);

            sink.Close();

            device.FillGeometry(geometry, sharedBrush);

            sink.Dispose();
            geometry.Dispose();
        }

        #endregion Geometry

        #region Special

        private int lastTime = 0;

        private float rotationState = 0.0f;

        private Stopwatch swastikaDeltaTimer = new Stopwatch();

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

        public void DrawBitmap(Direct2DBitmap bmp, float x, float y, float opacity)
        {
            Bitmap bitmap = bmp;
            device.DrawBitmap(bitmap, new RawRectangleF(x, y, x + bitmap.PixelSize.Width, y + bitmap.PixelSize.Height), opacity, BitmapInterpolationMode.Linear);
        }

        public void DrawBitmap(Direct2DBitmap bmp, float opacity, float x, float y, float width, float height)
        {
            Bitmap bitmap = bmp;
            device.DrawBitmap(bitmap, new RawRectangleF(x, y, x + width, y + height), opacity, BitmapInterpolationMode.Linear, new RawRectangleF(0, 0, bitmap.PixelSize.Width, bitmap.PixelSize.Height));
        }

        public void DrawBox2D(float x, float y, float width, float height, float stroke, Direct2DColor interiorColor, Direct2DColor color)
        {
            var geometry = new PathGeometry(factory);

            var sink = geometry.Open();

            sink.BeginFigure(new RawVector2(x, y), FigureBegin.Filled);
            sink.AddLine(new RawVector2(x + width, y));
            sink.AddLine(new RawVector2(x + width, y + height));
            sink.AddLine(new RawVector2(x, y + height));
            sink.EndFigure(FigureEnd.Closed);

            sink.Close();

            sharedBrush.Color = color;

            device.DrawGeometry(geometry, sharedBrush, stroke);

            sharedBrush.Color = interiorColor;

            device.FillGeometry(geometry, sharedBrush);

            sink.Dispose();
            geometry.Dispose();
        }

        public void DrawBox2D(float x, float y, float width, float height, float stroke, Direct2DBrush interiorBrush, Direct2DBrush brush)
        {
            var geometry = new PathGeometry(factory);

            var sink = geometry.Open();

            sink.BeginFigure(new RawVector2(x, y), FigureBegin.Filled);
            sink.AddLine(new RawVector2(x + width, y));
            sink.AddLine(new RawVector2(x + width, y + height));
            sink.AddLine(new RawVector2(x, y + height));
            sink.EndFigure(FigureEnd.Closed);

            sink.Close();

            device.DrawGeometry(geometry, brush, stroke);

            device.FillGeometry(geometry, interiorBrush);

            sink.Dispose();
            geometry.Dispose();
        }

        public void DrawCrosshair(CrosshairStyle style, float x, float y, float size, float stroke, Direct2DColor color)
        {
            sharedBrush.Color = color;

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
            else if (style == CrosshairStyle.Swastika)
            {
                RawVector2 first = new RawVector2(x - size, y);
                RawVector2 second = new RawVector2(x + size, y);

                RawVector2 third = new RawVector2(x, y - size);
                RawVector2 fourth = new RawVector2(x, y + size);

                RawVector2 haken_1 = new RawVector2(third.X + size, third.Y);
                RawVector2 haken_2 = new RawVector2(second.X, second.Y + size);
                RawVector2 haken_3 = new RawVector2(fourth.X - size, fourth.Y);
                RawVector2 haken_4 = new RawVector2(first.X, first.Y - size);

                device.DrawLine(first, second, sharedBrush, stroke);
                device.DrawLine(third, fourth, sharedBrush, stroke);

                device.DrawLine(third, haken_1, sharedBrush, stroke);
                device.DrawLine(second, haken_2, sharedBrush, stroke);
                device.DrawLine(fourth, haken_3, sharedBrush, stroke);
                device.DrawLine(first, haken_4, sharedBrush, stroke);
            }
        }

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
            else if (style == CrosshairStyle.Swastika)
            {
                RawVector2 first = new RawVector2(x - size, y);
                RawVector2 second = new RawVector2(x + size, y);

                RawVector2 third = new RawVector2(x, y - size);
                RawVector2 fourth = new RawVector2(x, y + size);

                RawVector2 haken_1 = new RawVector2(third.X + size, third.Y);
                RawVector2 haken_2 = new RawVector2(second.X, second.Y + size);
                RawVector2 haken_3 = new RawVector2(fourth.X - size, fourth.Y);
                RawVector2 haken_4 = new RawVector2(first.X, first.Y - size);

                device.DrawLine(first, second, brush, stroke);
                device.DrawLine(third, fourth, brush, stroke);

                device.DrawLine(third, haken_1, brush, stroke);
                device.DrawLine(second, haken_2, brush, stroke);
                device.DrawLine(fourth, haken_3, brush, stroke);
                device.DrawLine(first, haken_4, brush, stroke);
            }
        }

        public void DrawHorizontalBar(float percentage, float x, float y, float width, float height, float stroke, Direct2DColor interiorColor, Direct2DColor color)
        {
            float half = stroke / 2.0f;

            sharedBrush.Color = color;

            var rect = new RawRectangleF(x - half, y - half, x + width + half, y + height + half);

            device.DrawRectangle(rect, sharedBrush, stroke);

            if (percentage == 0.0f) return;

            rect.Left += half;
            rect.Right -= half;
            rect.Top += height - (height / 100.0f * percentage) + half;
            rect.Bottom -= half;

            sharedBrush.Color = interiorColor;

            device.FillRectangle(rect, sharedBrush);
        }

        public void DrawHorizontalBar(float percentage, float x, float y, float width, float height, float stroke, Direct2DBrush interiorBrush, Direct2DBrush brush)
        {
            float half = stroke / 2.0f;
            float quarter = half / 2.0f;

            var rect = new RawRectangleF(x - half, y - half, x + width + half, y + height + half);

            device.DrawRectangle(rect, brush, half);

            if (percentage == 0.0f) return;

            rect.Left += quarter;
            rect.Right -= quarter;
            rect.Top += height - (height / 100.0f * percentage) + quarter;
            rect.Bottom -= quarter;

            device.FillRectangle(rect, interiorBrush);
        }

        public void DrawVerticalBar(float percentage, float x, float y, float width, float height, float stroke, Direct2DColor interiorColor, Direct2DColor color)
        {
            float half = stroke / 2.0f;
            float quarter = half / 2.0f;

            sharedBrush.Color = color;

            var rect = new RawRectangleF(x - half, y - half, x + width + half, y + height + half);

            device.DrawRectangle(rect, sharedBrush, half);

            if (percentage == 0.0f) return;

            rect.Left += quarter;
            rect.Right -= width - (width / 100.0f * percentage) + quarter;
            rect.Top += quarter;
            rect.Bottom -= quarter;

            sharedBrush.Color = interiorColor;

            device.FillRectangle(rect, sharedBrush);
        }

        public void DrawVerticalBar(float percentage, float x, float y, float width, float height, float stroke, Direct2DBrush interiorBrush, Direct2DBrush brush)
        {
            float half = stroke / 2.0f;
            float quarter = half / 2.0f;

            var rect = new RawRectangleF(x - half, y - half, x + width + half, y + height + half);

            device.DrawRectangle(rect, brush, half);

            if (percentage == 0.0f) return;

            rect.Left += quarter;
            rect.Right -= width - (width / 100.0f * percentage) + quarter;
            rect.Top += quarter;
            rect.Bottom -= quarter;

            device.FillRectangle(rect, interiorBrush);
        }

        public void RotateSwastika(float x, float y, float size, float stroke, Direct2DColor color)
        {
            if (!swastikaDeltaTimer.IsRunning) swastikaDeltaTimer.Start();

            int thisTime = (int)swastikaDeltaTimer.ElapsedMilliseconds;

            if (Math.Abs(thisTime - lastTime) >= 3)
            {
                rotationState += 0.1f;
                lastTime = (int)swastikaDeltaTimer.ElapsedMilliseconds;
            }

            if (thisTime >= 1000) swastikaDeltaTimer.Restart();

            if (rotationState > size)
            {
                rotationState = size * -1.0f;
            }

            sharedBrush.Color = color;

            RawVector2 first = new RawVector2(x - size, y - rotationState);
            RawVector2 second = new RawVector2(x + size, y + rotationState);

            RawVector2 third = new RawVector2(x + rotationState, y - size);
            RawVector2 fourth = new RawVector2(x - rotationState, y + size);

            RawVector2 haken_1 = new RawVector2(third.X + size, third.Y + rotationState);
            RawVector2 haken_2 = new RawVector2(second.X - rotationState, second.Y + size);
            RawVector2 haken_3 = new RawVector2(fourth.X - size, fourth.Y - rotationState);
            RawVector2 haken_4 = new RawVector2(first.X + rotationState, first.Y - size);

            device.DrawLine(first, second, sharedBrush, stroke);
            device.DrawLine(third, fourth, sharedBrush, stroke);

            device.DrawLine(third, haken_1, sharedBrush, stroke);
            device.DrawLine(second, haken_2, sharedBrush, stroke);
            device.DrawLine(fourth, haken_3, sharedBrush, stroke);
            device.DrawLine(first, haken_4, sharedBrush, stroke);
        }

        #endregion Special

        #region Text

        public void DrawText(string text, float x, float y, Direct2DFont font, Direct2DColor color)
        {
            sharedBrush.Color = color;
            device.DrawText(text, text.Length, font, new RawRectangleF(x, y, float.MaxValue, float.MaxValue), sharedBrush, DrawTextOptions.NoSnap, MeasuringMode.Natural);
        }

        public void DrawText(string text, float x, float y, Direct2DFont font, Direct2DBrush brush)
        {
            device.DrawText(text, text.Length, font, new RawRectangleF(x, y, float.MaxValue, float.MaxValue), brush, DrawTextOptions.NoSnap, MeasuringMode.Natural);
        }

        public void DrawText(string text, float x, float y, float fontSize, Direct2DFont font, Direct2DColor color)
        {
            sharedBrush.Color = color;

            var layout = new TextLayout(fontFactory, text, font, float.MaxValue, float.MaxValue);

            layout.SetFontSize(fontSize, new TextRange(0, text.Length));

            device.DrawTextLayout(new RawVector2(x, y), layout, sharedBrush, DrawTextOptions.NoSnap);

            layout.Dispose();
        }

        public void DrawText(string text, float x, float y, float fontSize, Direct2DFont font, Direct2DBrush brush)
        {
            var layout = new TextLayout(fontFactory, text, font, float.MaxValue, float.MaxValue);

            layout.SetFontSize(fontSize, new TextRange(0, text.Length));

            device.DrawTextLayout(new RawVector2(x, y), layout, brush, DrawTextOptions.NoSnap);

            layout.Dispose();
        }

        public void DrawTextWithBackground(string text, float x, float y, Direct2DFont font, Direct2DColor color, Direct2DColor backgroundColor)
        {
            var layout = new TextLayout(fontFactory, text, font, float.MaxValue, float.MaxValue);

            float modifier = layout.FontSize / 4.0f;

            sharedBrush.Color = backgroundColor;

            device.FillRectangle(new RawRectangleF(x - modifier, y - modifier, x + layout.Metrics.Width + modifier, y + layout.Metrics.Height + modifier), sharedBrush);

            sharedBrush.Color = color;

            device.DrawTextLayout(new RawVector2(x, y), layout, sharedBrush, DrawTextOptions.NoSnap);

            layout.Dispose();
        }

        public void DrawTextWithBackground(string text, float x, float y, Direct2DFont font, Direct2DBrush brush, Direct2DBrush backgroundBrush)
        {
            var layout = new TextLayout(fontFactory, text, font, float.MaxValue, float.MaxValue);

            float modifier = layout.FontSize / 4.0f;

            device.FillRectangle(new RawRectangleF(x - modifier, y - modifier, x + layout.Metrics.Width + modifier, y + layout.Metrics.Height + modifier), backgroundBrush);

            device.DrawTextLayout(new RawVector2(x, y), layout, brush, DrawTextOptions.NoSnap);

            layout.Dispose();
        }

        public void DrawTextWithBackground(string text, float x, float y, float fontSize, Direct2DFont font, Direct2DColor color, Direct2DColor backgroundColor)
        {
            var layout = new TextLayout(fontFactory, text, font, float.MaxValue, float.MaxValue);

            layout.SetFontSize(fontSize, new TextRange(0, text.Length));

            float modifier = fontSize / 4.0f;

            sharedBrush.Color = backgroundColor;

            device.FillRectangle(new RawRectangleF(x - modifier, y - modifier, x + layout.Metrics.Width + modifier, y + layout.Metrics.Height + modifier), sharedBrush);

            sharedBrush.Color = color;

            device.DrawTextLayout(new RawVector2(x, y), layout, sharedBrush, DrawTextOptions.NoSnap);

            layout.Dispose();
        }

        public void DrawTextWithBackground(string text, float x, float y, float fontSize, Direct2DFont font, Direct2DBrush brush, Direct2DBrush backgroundBrush)
        {
            var layout = new TextLayout(fontFactory, text, font, float.MaxValue, float.MaxValue);

            layout.SetFontSize(fontSize, new TextRange(0, text.Length));

            float modifier = fontSize / 4.0f;

            device.FillRectangle(new RawRectangleF(x - modifier, y - modifier, x + layout.Metrics.Width + modifier, y + layout.Metrics.Height + modifier), backgroundBrush);

            device.DrawTextLayout(new RawVector2(x, y), layout, brush, DrawTextOptions.NoSnap);

            layout.Dispose();
        }

        #endregion Text

        #region IDisposable Support

        private bool disposedValue = false;

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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable Support
    }
}