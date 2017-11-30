using System;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using FontFactory = SharpDX.DirectWrite.Factory;
using Factory = SharpDX.Direct2D1.Factory;
using System.Threading;
using System.Runtime.CompilerServices;
namespace Yato.DirectXOverlay
{
    public class Direct2DRenderer : IDisposable
    {
        #region private vars
        private Direct2DRendererOptions rendererOptions;
        private WindowRenderTarget device;
        private HwndRenderTargetProperties deviceProperties;
        private FontFactory fontFactory;
        private Factory factory;
        private SolidColorBrush sharedBrush;
        private TextFormat sharedFont;
        private bool isDrawing;
        private bool resize;
        private int resizeWidth;
        private int resizeHeight;
        private Stopwatch stopwatch = new Stopwatch();
        private int internalFps;
        #endregion
        #region public vars
        public IntPtr RenderTargetHwnd { get; private set; }
        public bool VSync { get; private set; }
        public int FPS { get; private set; }
        public bool MeasureFPS { get; set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        #endregion
        #region construct & destruct
        private Direct2DRenderer()
        {
            throw new NotSupportedException();
        }
        public Direct2DRenderer(IntPtr hwnd)
        {
            var options = new Direct2DRendererOptions()
            {
                Hwnd = hwnd,
                VSync = false,
                MeasureFps = false,
                AntiAliasing = false
            };
            setupInstance(options);
        }
        public Direct2DRenderer(IntPtr hwnd, bool vsync)
        {
            var options = new Direct2DRendererOptions()
            {
                Hwnd = hwnd,
                VSync = vsync,
                MeasureFps = false,
                AntiAliasing = false
            };
            setupInstance(options);
        }
        public Direct2DRenderer(IntPtr hwnd, bool vsync, bool measureFps)
        {
            var options = new Direct2DRendererOptions()
            {
                Hwnd = hwnd,
                VSync = vsync,
                MeasureFps = measureFps,
                AntiAliasing = false
            };
            setupInstance(options);
        }
        public Direct2DRenderer(IntPtr hwnd, bool vsync, bool measureFps, bool antiAliasing)
        {
            var options = new Direct2DRendererOptions()
            {
                Hwnd = hwnd,
                VSync = vsync,
                MeasureFps = measureFps,
                AntiAliasing = antiAliasing
            };
            setupInstance(options);
        }
        public Direct2DRenderer(Direct2DRendererOptions options)
        {
            setupInstance(options);
        }
        ~Direct2DRenderer()
        {
            Dispose(false);
        }
        #endregion
        #region init & delete
        private void setupInstance(Direct2DRendererOptions options)
        {
            rendererOptions = options;
            if (options.Hwnd == IntPtr.Zero) throw new ArgumentNullException(nameof(options.Hwnd));
            if (PInvoke.IsWindow(options.Hwnd) == 0) throw new ArgumentException("The window does not exist (hwnd = 0x" + options.Hwnd.ToString("X") + ")");
            PInvoke.RECT bounds = new PInvoke.RECT();
            if (PInvoke.GetRealWindowRect(options.Hwnd, out bounds) == 0) throw new Exception("Failed to get the size of the given window (hwnd = 0x" + options.Hwnd.ToString("X") + ")");
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
                96.0f, 96.0f, // May need to change window and render targets dpi according to windows. but this seems to fix it at least for me (looks better somehow)
                RenderTargetUsage.None,
                FeatureLevel.Level_DEFAULT);
            factory = new Factory();
            fontFactory = new FontFactory();
            device = new WindowRenderTarget(factory, renderProperties, deviceProperties);
            device.AntialiasMode = AntialiasMode.Aliased; // AntialiasMode.PerPrimitive fails rendering some objects
            // other than in the documentation: Cleartype is much faster for me than GrayScale
            device.TextAntialiasMode = options.AntiAliasing ? SharpDX.Direct2D1.TextAntialiasMode.Cleartype : SharpDX.Direct2D1.TextAntialiasMode.Aliased;
            sharedBrush = new SolidColorBrush(device, default(RawColor4));
        }
        private void deleteInstance()
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
        #endregion
        #region Scenes
        public void Resize(int width, int height)
        {
            resizeWidth = width;
            resizeHeight = height;
            resize = true;
        }
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
        public Direct2DScene UseScene()
        {
            // really expensive to use but i like the pattern
            return new Direct2DScene(this);
        }
        public void EndScene()
        {
            if (device == null) return;
            if (!isDrawing) return;
            long tag_0 = 0L, tag_1 = 0L;
            var result = device.TryEndDraw(out tag_0, out tag_1);
            if (result.Failure)
            {
                deleteInstance();
                setupInstance(rendererOptions);
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
        #endregion
        #region Fonts & Brushes & Bitmaps
        public void SetSharedFont(string fontFamilyName, float size, bool bold = false, bool italic = false)
        {
            sharedFont = new TextFormat(fontFactory, fontFamilyName, bold ? FontWeight.Bold : FontWeight.Normal, italic ? FontStyle.Italic : FontStyle.Normal, size);
            sharedFont.WordWrapping = SharpDX.DirectWrite.WordWrapping.NoWrap;
        }
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
        public Direct2DFont CreateFont(Direct2DFontCreationOptions options)
        {
            TextFormat font = new TextFormat(fontFactory, options.FontFamilyName, options.Bold ? FontWeight.Bold : FontWeight.Normal, options.GetStyle(), options.FontSize);
            font.WordWrapping = options.WordWrapping ? WordWrapping.Wrap : WordWrapping.NoWrap;
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
        #endregion
        #region Primitives
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
        #endregion
        #region Filled
        public void FillRectangle(float x, float y, float width, float height, Direct2DBrush brush)
        {
            device.FillRectangle(new RawRectangleF(x, y, x + width, y + height), brush);
        }
        public void FillRectangle(float x, float y, float width, float height, Direct2DColor color)
        {
            sharedBrush.Color = color;
            device.FillRectangle(new RawRectangleF(x, y, x + width, y + height), sharedBrush);
        }
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
        #endregion
        #region Bordered
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
        #endregion
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
        #endregion
        #region Special
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
        private Stopwatch swastikaDeltaTimer = new Stopwatch();
        float rotationState = 0.0f;
        int lastTime = 0;
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
        #endregion
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
        #endregion
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
                deleteInstance();
                disposedValue = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
    public enum CrosshairStyle
    {
        Dot,
        Plus,
        Cross,
        Gap,
        Diagonal,
        Swastika
    }
    public struct Direct2DRendererOptions
    {
        public IntPtr Hwnd;
        public bool VSync;
        public bool MeasureFps;
        public bool AntiAliasing;
    }
    public class Direct2DFontCreationOptions
    {
        public string FontFamilyName;
        public float FontSize;
        public bool Bold;
        public bool Italic;
        public bool WordWrapping;
        public FontStyle GetStyle()
        {
            if (Italic) return FontStyle.Italic;
            return FontStyle.Normal;
        }
    }
    public struct Direct2DColor
    {
        public float Red;
        public float Green;
        public float Blue;
        public float Alpha;
        public Direct2DColor(int red, int green, int blue)
        {
            Red = red / 255.0f;
            Green = green / 255.0f;
            Blue = blue / 255.0f;
            Alpha = 1.0f;
        }
        public Direct2DColor(int red, int green, int blue, int alpha)
        {
            Red = red / 255.0f;
            Green = green / 255.0f;
            Blue = blue / 255.0f;
            Alpha = alpha / 255.0f;
        }
        public Direct2DColor(float red, float green, float blue)
        {
            Red = red;
            Green = green;
            Blue = blue;
            Alpha = 1.0f;
        }
        public Direct2DColor(float red, float green, float blue, float alpha)
        {
            Red = red;
            Green = green;
            Blue = blue;
            Alpha = alpha;
        }
        public static implicit operator RawColor4(Direct2DColor color)
        {
            return new RawColor4(color.Red, color.Green, color.Blue, color.Alpha);
        }
        public static implicit operator Direct2DColor(RawColor4 color)
        {
            return new Direct2DColor(color.R, color.G, color.B, color.A);
        }
    }
    public class Direct2DBrush
    {
        public Direct2DColor Color
        {
            get
            {
                return Brush.Color;
            }
            set
            {
                Brush.Color = value;
            }
        }
        public SolidColorBrush Brush;
        private Direct2DBrush()
        {
            throw new NotImplementedException();
        }
        public Direct2DBrush(RenderTarget renderTarget)
        {
            Brush = new SolidColorBrush(renderTarget, default(RawColor4));
        }
        public Direct2DBrush(RenderTarget renderTarget, Direct2DColor color)
        {
            Brush = new SolidColorBrush(renderTarget, color);
        }
        ~Direct2DBrush()
        {
            Brush.Dispose();
        }
        public static implicit operator SolidColorBrush(Direct2DBrush brush)
        {
            return brush.Brush;
        }
        public static implicit operator Direct2DColor(Direct2DBrush brush)
        {
            return brush.Color;
        }
        public static implicit operator RawColor4(Direct2DBrush brush)
        {
            return brush.Color;
        }
    }
    public class Direct2DFont
    {
        private FontFactory factory;
        public TextFormat Font;
        public string FontFamilyName
        {
            get
            {
                return Font.FontFamilyName;
            }
            set
            {
                float size = FontSize;
                bool bold = Bold;
                FontStyle style = Italic ? FontStyle.Italic : FontStyle.Normal;
                bool wordWrapping = WordWrapping;
                Font.Dispose();
                Font = new TextFormat(factory, value, bold ? FontWeight.Bold : FontWeight.Normal, style, size);
                Font.WordWrapping = wordWrapping ? SharpDX.DirectWrite.WordWrapping.Wrap : SharpDX.DirectWrite.WordWrapping.NoWrap;
            }
        }
        public float FontSize
        {
            get
            {
                return Font.FontSize;
            }
            set
            {
                string familyName = FontFamilyName;
                bool bold = Bold;
                FontStyle style = Italic ? FontStyle.Italic : FontStyle.Normal;
                bool wordWrapping = WordWrapping;
                Font.Dispose();
                Font = new TextFormat(factory, familyName, bold ? FontWeight.Bold : FontWeight.Normal, style, value);
                Font.WordWrapping = wordWrapping ? SharpDX.DirectWrite.WordWrapping.Wrap : SharpDX.DirectWrite.WordWrapping.NoWrap;
            }
        }
        public bool Bold
        {
            get
            {
                return Font.FontWeight == FontWeight.Bold;
            }
            set
            {
                string familyName = FontFamilyName;
                float size = FontSize;
                FontStyle style = Italic ? FontStyle.Italic : FontStyle.Normal;
                bool wordWrapping = WordWrapping;
                Font.Dispose();
                Font = new TextFormat(factory, familyName, value ? FontWeight.Bold : FontWeight.Normal, style, size);
                Font.WordWrapping = wordWrapping ? SharpDX.DirectWrite.WordWrapping.Wrap : SharpDX.DirectWrite.WordWrapping.NoWrap;
            }
        }
        public bool Italic
        {
            get
            {
                return Font.FontStyle == FontStyle.Italic;
            }
            set
            {
                string familyName = FontFamilyName;
                float size = FontSize;
                bool bold = Bold;
                bool wordWrapping = WordWrapping;
                Font.Dispose();
                Font = new TextFormat(factory, familyName, bold ? FontWeight.Bold : FontWeight.Normal, value ? FontStyle.Italic : FontStyle.Normal, size);
                Font.WordWrapping = wordWrapping ? SharpDX.DirectWrite.WordWrapping.Wrap : SharpDX.DirectWrite.WordWrapping.NoWrap;
            }
        }
        public bool WordWrapping
        {
            get
            {
                return Font.WordWrapping != SharpDX.DirectWrite.WordWrapping.NoWrap;
            }
            set
            {
                Font.WordWrapping = value ? SharpDX.DirectWrite.WordWrapping.Wrap : SharpDX.DirectWrite.WordWrapping.NoWrap;
            }
        }
        private Direct2DFont()
        {
            throw new NotImplementedException();
        }
        public Direct2DFont(TextFormat font)
        {
            Font = font;
        }
        public Direct2DFont(FontFactory factory, string fontFamilyName, float size, bool bold = false, bool italic = false)
        {
            this.factory = factory;
            Font = new TextFormat(factory, fontFamilyName, bold ? FontWeight.Bold : FontWeight.Normal, italic ? FontStyle.Italic : FontStyle.Normal, size);
            Font.WordWrapping = SharpDX.DirectWrite.WordWrapping.NoWrap;
        }
        ~Direct2DFont()
        {
            Font.Dispose();
        }
        public static implicit operator TextFormat(Direct2DFont font)
        {
            return font.Font;
        }
    }
    public class Direct2DScene : IDisposable
    {
        public Direct2DRenderer Renderer { get; private set; }
        private Direct2DScene()
        {
            throw new NotImplementedException();
        }
        public Direct2DScene(Direct2DRenderer renderer)
        {
            GC.SuppressFinalize(this);
            Renderer = renderer;
            renderer.BeginScene();
        }
        ~Direct2DScene()
        {
            Dispose(false);
        }
        public static implicit operator Direct2DRenderer(Direct2DScene scene)
        {
            return scene.Renderer;
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
                Renderer.EndScene();
                disposedValue = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
    public class Direct2DBitmap
    {
        private static SharpDX.WIC.ImagingFactory factory = new SharpDX.WIC.ImagingFactory();
        public Bitmap SharpDXBitmap;
        private Direct2DBitmap()
        {
        }
        public Direct2DBitmap(RenderTarget device, byte[] bytes)
        {
            loadBitmap(device, bytes);
        }
        public Direct2DBitmap(RenderTarget device, string file)
        {
            loadBitmap(device, File.ReadAllBytes(file));
        }
        ~Direct2DBitmap()
        {
            SharpDXBitmap.Dispose();
        }
        private void loadBitmap(RenderTarget device, byte[] bytes)
        {
            var stream = new MemoryStream(bytes);
            SharpDX.WIC.BitmapDecoder decoder = new SharpDX.WIC.BitmapDecoder(factory, stream, SharpDX.WIC.DecodeOptions.CacheOnDemand);
            var frame = decoder.GetFrame(0);
            SharpDX.WIC.FormatConverter converter = new SharpDX.WIC.FormatConverter(factory);
            try
            {
                // normal ARGB images (Bitmaps / png tested)
                converter.Initialize(frame, SharpDX.WIC.PixelFormat.Format32bppRGBA1010102);
            }
            catch
            {
                // falling back to RGB if unsupported
                converter.Initialize(frame, SharpDX.WIC.PixelFormat.Format32bppRGB);
            }
            SharpDXBitmap = Bitmap.FromWicBitmap(device, converter);
            converter.Dispose();
            frame.Dispose();
            decoder.Dispose();
            stream.Dispose();
        }
        public static implicit operator Bitmap(Direct2DBitmap bmp)
        {
            return bmp.SharpDXBitmap;
        }
    }
}
namespace Yato.DirectXOverlay
{
    public class OverlayManager : IDisposable
    {
        private bool exitThread;
        private Thread serviceThread;
        public IntPtr ParentWindowHandle { get; private set; }
        public OverlayWindow Window { get; private set; }
        public Direct2DRenderer Graphics { get; private set; }
        public bool IsParentWindowVisible { get; private set; }
        private OverlayManager()
        {
        }
        public OverlayManager(IntPtr parentWindowHandle, bool vsync = false, bool measurefps = false, bool antialiasing = true)
        {
            Direct2DRendererOptions options = new Direct2DRendererOptions()
            {
                AntiAliasing = antialiasing,
                Hwnd = IntPtr.Zero,
                MeasureFps = measurefps,
                VSync = vsync
            };
            setupInstance(parentWindowHandle, options);
        }
        public OverlayManager(IntPtr parentWindowHandle, Direct2DRendererOptions options)
        {
            setupInstance(parentWindowHandle, options);
        }
        ~OverlayManager()
        {
            Dispose(false);
        }
        private void setupInstance(IntPtr parentWindowHandle, Direct2DRendererOptions options)
        {
            ParentWindowHandle = parentWindowHandle;
            if (PInvoke.IsWindow(parentWindowHandle) == 0) throw new Exception("The parent window does not exist");
            PInvoke.RECT bounds = new PInvoke.RECT();
            PInvoke.GetRealWindowRect(parentWindowHandle, out bounds);
            int x = bounds.Left;
            int y = bounds.Top;
            int width = bounds.Right - x;
            int height = bounds.Bottom - y;
            Window = new OverlayWindow(x, y, width, height);
            options.Hwnd = Window.WindowHandle;
            Graphics = new Direct2DRenderer(options);
            serviceThread = new Thread(new ThreadStart(windowServiceThread))
            {
                IsBackground = true,
                Priority = ThreadPriority.BelowNormal
            };
            serviceThread.Start();
        }
        private void windowServiceThread()
        {
            PInvoke.RECT bounds = new PInvoke.RECT();
            while (!exitThread)
            {
                Thread.Sleep(100);
                IsParentWindowVisible = PInvoke.IsWindowVisible(ParentWindowHandle) != 0;
                if (!IsParentWindowVisible)
                {
                    if (Window.IsVisible) Window.HideWindow();
                    continue;
                }
                if (!Window.IsVisible) Window.ShowWindow();
                PInvoke.GetRealWindowRect(ParentWindowHandle, out bounds);
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
        #endregion
    }
}
namespace Yato.DirectXOverlay
{
    public class OverlayWindow : IDisposable
    {
        private Random rng;
        private delegate IntPtr WndProc(IntPtr hWnd, PInvoke.WindowsMessage msg, IntPtr wParam, IntPtr lParam);
        private IntPtr wndProcPointer;
        private WndProc wndProc;
        private Thread windowThread;
        private string randomClassName;
        public IntPtr WindowHandle { get; private set; }
        public int X { get; private set; }
        public int Y { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public bool IsVisible { get; private set; }
        public bool Topmost { get; private set; }
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
        private void setupInstance(int x = 0, int y = 0, int width = 800, int height = 600)
        {
            IsVisible = true;
            Topmost = true;
            X = x;
            Y = y;
            Width = width;
            Height = height;
            randomClassName = generateRandomString(5, 11);
            string randomMenuName = generateRandomString(5, 11);
            string randomWindowName = generateRandomString(5, 11);
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
            WindowHandle = PInvoke.CreateWindowEx(
                0x8 | 0x20 | 0x80000 | 0x80 | 0x8000000, // WS_EX_TOPMOST | WS_EX_TRANSPARENT | WS_EX_LAYERED |WS_EX_TOOLWINDOW
                randomClassName,
                randomWindowName,
                0x80000000 | 0x10000000,
                X, Y,
                Width, Height,
                IntPtr.Zero,
                IntPtr.Zero,
                IntPtr.Zero,
                IntPtr.Zero
                );
            PInvoke.SetLayeredWindowAttributes(WindowHandle, 0, 255, /*0x1 |*/ 0x2);
            extendFrameIntoClientArea();
            PInvoke.UpdateWindow(WindowHandle);
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
            if((int)msg == 0x02E0) // DPI Changed
            {
                return (IntPtr)0;
            }
            return PInvoke.DefWindowProc(hwnd, msg, wParam, lParam);
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
        public void ShowWindow()
        {
            if (IsVisible) return;
            PInvoke.ShowWindow(WindowHandle, 5);
            extendFrameIntoClientArea();
            IsVisible = true;
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
        #endregion
    }
}
namespace Yato.DirectXOverlay
{
    internal static class PInvoke
    {
        public static int GetRealWindowRect(IntPtr hwnd, out RECT rect)
        {
            RECT windowRect = new RECT();
            RECT clientRect = new RECT();
            int result = GetWindowRect(hwnd, out windowRect);
            if(GetClientRect(hwnd, out clientRect) == 0)
            {
                rect = windowRect;
                return result;
            }
            int windowWidth = windowRect.Right - windowRect.Left;
            int windowHeight = windowRect.Bottom - windowRect.Top;
            if (windowWidth == clientRect.Right && windowHeight == clientRect.Bottom)
            {
                rect = windowRect;
                return result;
            }
            int dif_x = windowWidth > clientRect.Right ? windowWidth - clientRect.Right : clientRect.Right - windowWidth;
            int dif_y = windowHeight > clientRect.Bottom ? windowHeight - clientRect.Bottom : clientRect.Bottom - windowHeight;
            dif_x /= 2;
            dif_y /= 2;
            windowRect.Left += dif_x;
            windowRect.Top += dif_y;
            windowRect.Right -= dif_x;
            windowRect.Bottom -= dif_y;
            rect = windowRect;
            return result;
        }
        #region User32
        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        public delegate IntPtr CreateWindowEx_t(
           uint dwExStyle,
           string lpClassName,
           string lpWindowName,
           uint dwStyle,
           int x,
           int y,
           int nWidth,
           int nHeight,
           IntPtr hWndParent,
           IntPtr hMenu,
           IntPtr hInstance,
           IntPtr lpParam);
        public static CreateWindowEx_t CreateWindowEx = WinApi.GetMethod<CreateWindowEx_t>("user32.dll", "CreateWindowExW");
        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        public delegate ushort RegisterClassEx_t(ref WNDCLASSEX wndclassex);
        public static RegisterClassEx_t RegisterClassEx = WinApi.GetMethod<RegisterClassEx_t>("user32.dll", "RegisterClassExW");
        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        public delegate int UnregisterClass_t(string lpClassName, IntPtr hInstance);
        public static UnregisterClass_t UnregisterClass = WinApi.GetMethod<UnregisterClass_t>("user32.dll", "UnregisterClassW");
        public delegate bool SetLayeredWindowAttributes_t(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);
        public static SetLayeredWindowAttributes_t SetLayeredWindowAttributes = WinApi.GetMethod<SetLayeredWindowAttributes_t>("user32.dll", "SetLayeredWindowAttributes");
        public delegate int TranslateMessage_t(ref Message msg);
        public static TranslateMessage_t TranslateMessage = WinApi.GetMethod<TranslateMessage_t>("user32.dll", "TranslateMessage");
        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        public delegate int PeekMessageW_t(ref Message msg, IntPtr hwnd, uint filterMin, uint filterMax, uint removeMsg);
        public static PeekMessageW_t PeekMessageW = WinApi.GetMethod<PeekMessageW_t>("user32.dll", "PeekMessageW");
        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        public delegate int DispatchMessage_t(ref Message msg);
        public static DispatchMessage_t DispatchMessage = WinApi.GetMethod<DispatchMessage_t>("user32.dll", "DispatchMessageW");
        public delegate int MoveWindow_t(IntPtr hwnd, int x, int y, int width, int height, int repaint);
        public static MoveWindow_t MoveWindow = WinApi.GetMethod<MoveWindow_t>("user32.dll", "MoveWindow");
        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        public delegate IntPtr DefWindowProc_t(IntPtr hwnd, WindowsMessage msg, IntPtr wparam, IntPtr lparam);
        public static DefWindowProc_t DefWindowProc = WinApi.GetMethod<DefWindowProc_t>("user32.dll", "DefWindowProcW");
        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        public delegate int SendMessage_t(IntPtr hwnd, WindowsMessage msg, IntPtr wparam, IntPtr lparam);
        public static SendMessage_t SendMessage = WinApi.GetMethod<SendMessage_t>("user32.dll", "SendMessageW");
        public delegate bool UpdateWindow_t(IntPtr hWnd);
        public static UpdateWindow_t UpdateWindow = WinApi.GetMethod<UpdateWindow_t>("user32.dll", "UpdateWindow");
        public delegate int DestroyWindow_t(IntPtr hwnd);
        public static DestroyWindow_t DestroyWindow = WinApi.GetMethod<DestroyWindow_t>("user32.dll", "DestroyWindow");
        public delegate int ShowWindow_t(IntPtr hWnd, uint nCmdShow);
        public static ShowWindow_t ShowWindow = WinApi.GetMethod<ShowWindow_t>("user32.dll", "ShowWindow");
        public delegate int WaitMessage_t();
        public static WaitMessage_t WaitMessage = WinApi.GetMethod<WaitMessage_t>("user32.dll", "WaitMessage");
        public delegate int GetWindowRect_t(IntPtr hwnd, out RECT lpRect);
        public static GetWindowRect_t GetWindowRect = WinApi.GetMethod<GetWindowRect_t>("user32.dll", "GetWindowRect");
        public delegate int GetClientRect_t(IntPtr hwnd, out RECT lpRect);
        public static GetClientRect_t GetClientRect = WinApi.GetMethod<GetClientRect_t>("user32.dll", "GetClientRect");
        public delegate int IsWindowVisible_t(IntPtr hwnd);
        public static IsWindowVisible_t IsWindowVisible = WinApi.GetMethod<IsWindowVisible_t>("user32.dll", "IsWindowVisible");
        public delegate int IsWindow_t(IntPtr hwnd);
        public static IsWindow_t IsWindow = WinApi.GetMethod<IsWindow_t>("user32.dll", "IsWindow");
        #endregion
        #region DwmApi
        public delegate void DwmExtendFrameIntoClientArea_t(IntPtr hWnd, ref MARGIN pMargins);
        public static DwmExtendFrameIntoClientArea_t DwmExtendFrameIntoClientArea = WinApi.GetMethod<DwmExtendFrameIntoClientArea_t>("dwmapi.dll", "DwmExtendFrameIntoClientArea");
        #endregion
        #region Enums & Structs
        [StructLayout(LayoutKind.Sequential)]
        public struct Message
        {
            public IntPtr Hwnd;
            public WindowsMessage Msg;
            public IntPtr lParam;
            public IntPtr wParam;
            //public IntPtr Result;
            public uint Time;
            public int X;
            public int Y;
        }
        public enum WindowsMessage : uint
        {
            WM_NULL = 0x0000,
            WM_CREATE = 0x0001,
            WM_DESTROY = 0x0002,
            WM_MOVE = 0x0003,
            WM_SIZE = 0x0005,
            WM_ACTIVATE = 0x0006,
            WM_SETFOCUS = 0x0007,
            WM_KILLFOCUS = 0x0008,
            WM_ENABLE = 0x000A,
            WM_SETREDRAW = 0x000B,
            WM_SETTEXT = 0x000C,
            WM_GETTEXT = 0x000D,
            WM_GETTEXTLENGTH = 0x000E,
            WM_PAINT = 0x000F,
            WM_CLOSE = 0x0010,
            WM_QUERYENDSESSION = 0x0011,
            WM_QUERYOPEN = 0x0013,
            WM_ENDSESSION = 0x0016,
            WM_QUIT = 0x0012,
            WM_ERASEBKGND = 0x0014,
            WM_SYSCOLORCHANGE = 0x0015,
            WM_SHOWWINDOW = 0x0018,
            WM_WININICHANGE = 0x001A,
            WM_SETTINGCHANGE = WM_WININICHANGE,
            WM_DEVMODECHANGE = 0x001B,
            WM_ACTIVATEAPP = 0x001C,
            WM_FONTCHANGE = 0x001D,
            WM_TIMECHANGE = 0x001E,
            WM_CANCELMODE = 0x001F,
            WM_SETCURSOR = 0x0020,
            WM_MOUSEACTIVATE = 0x0021,
            WM_CHILDACTIVATE = 0x0022,
            WM_QUEUESYNC = 0x0023,
            WM_GETMINMAXINFO = 0x0024,
            WM_PAINTICON = 0x0026,
            WM_ICONERASEBKGND = 0x0027,
            WM_NEXTDLGCTL = 0x0028,
            WM_SPOOLERSTATUS = 0x002A,
            WM_DRAWITEM = 0x002B,
            WM_MEASUREITEM = 0x002C,
            WM_DELETEITEM = 0x002D,
            WM_VKEYTOITEM = 0x002E,
            WM_CHARTOITEM = 0x002F,
            WM_SETFONT = 0x0030,
            WM_GETFONT = 0x0031,
            WM_SETHOTKEY = 0x0032,
            WM_GETHOTKEY = 0x0033,
            WM_QUERYDRAGICON = 0x0037,
            WM_COMPAREITEM = 0x0039,
            WM_GETOBJECT = 0x003D,
            WM_COMPACTING = 0x0041,
            WM_COMMNOTIFY = 0x0044,
            WM_WINDOWPOSCHANGING = 0x0046,
            WM_WINDOWPOSCHANGED = 0x0047,
            WM_POWER = 0x0048,
            WM_COPYDATA = 0x004A,
            WM_CANCELJOURNAL = 0x004B,
            WM_NOTIFY = 0x004E,
            WM_INPUTLANGCHANGEREQUEST = 0x0050,
            WM_INPUTLANGCHANGE = 0x0051,
            WM_TCARD = 0x0052,
            WM_HELP = 0x0053,
            WM_USERCHANGED = 0x0054,
            WM_NOTIFYFORMAT = 0x0055,
            WM_CONTEXTMENU = 0x007B,
            WM_STYLECHANGING = 0x007C,
            WM_STYLECHANGED = 0x007D,
            WM_DISPLAYCHANGE = 0x007E,
            WM_GETICON = 0x007F,
            WM_SETICON = 0x0080,
            WM_NCCREATE = 0x0081,
            WM_NCDESTROY = 0x0082,
            WM_NCCALCSIZE = 0x0083,
            WM_NCHITTEST = 0x0084,
            WM_NCPAINT = 0x0085,
            WM_NCACTIVATE = 0x0086,
            WM_GETDLGCODE = 0x0087,
            WM_SYNCPAINT = 0x0088,
            WM_NCMOUSEMOVE = 0x00A0,
            WM_NCLBUTTONDOWN = 0x00A1,
            WM_NCLBUTTONUP = 0x00A2,
            WM_NCLBUTTONDBLCLK = 0x00A3,
            WM_NCRBUTTONDOWN = 0x00A4,
            WM_NCRBUTTONUP = 0x00A5,
            WM_NCRBUTTONDBLCLK = 0x00A6,
            WM_NCMBUTTONDOWN = 0x00A7,
            WM_NCMBUTTONUP = 0x00A8,
            WM_NCMBUTTONDBLCLK = 0x00A9,
            WM_NCXBUTTONDOWN = 0x00AB,
            WM_NCXBUTTONUP = 0x00AC,
            WM_NCXBUTTONDBLCLK = 0x00AD,
            WM_INPUT_DEVICE_CHANGE = 0x00FE,
            WM_INPUT = 0x00FF,
            WM_KEYFIRST = 0x0100,
            WM_KEYDOWN = 0x0100,
            WM_KEYUP = 0x0101,
            WM_CHAR = 0x0102,
            WM_DEADCHAR = 0x0103,
            WM_SYSKEYDOWN = 0x0104,
            WM_SYSKEYUP = 0x0105,
            WM_SYSCHAR = 0x0106,
            WM_SYSDEADCHAR = 0x0107,
            WM_UNICHAR = 0x0109,
            WM_KEYLAST = 0x0109,
            WM_IME_STARTCOMPOSITION = 0x010D,
            WM_IME_ENDCOMPOSITION = 0x010E,
            WM_IME_COMPOSITION = 0x010F,
            WM_IME_KEYLAST = 0x010F,
            WM_INITDIALOG = 0x0110,
            WM_COMMAND = 0x0111,
            WM_SYSCOMMAND = 0x0112,
            WM_TIMER = 0x0113,
            WM_HSCROLL = 0x0114,
            WM_VSCROLL = 0x0115,
            WM_INITMENU = 0x0116,
            WM_INITMENUPOPUP = 0x0117,
            WM_MENUSELECT = 0x011F,
            WM_MENUCHAR = 0x0120,
            WM_ENTERIDLE = 0x0121,
            WM_MENURBUTTONUP = 0x0122,
            WM_MENUDRAG = 0x0123,
            WM_MENUGETOBJECT = 0x0124,
            WM_UNINITMENUPOPUP = 0x0125,
            WM_MENUCOMMAND = 0x0126,
            WM_CHANGEUISTATE = 0x0127,
            WM_UPDATEUISTATE = 0x0128,
            WM_QUERYUISTATE = 0x0129,
            WM_CTLCOLORMSGBOX = 0x0132,
            WM_CTLCOLOREDIT = 0x0133,
            WM_CTLCOLORLISTBOX = 0x0134,
            WM_CTLCOLORBTN = 0x0135,
            WM_CTLCOLORDLG = 0x0136,
            WM_CTLCOLORSCROLLBAR = 0x0137,
            WM_CTLCOLORSTATIC = 0x0138,
            MN_GETHMENU = 0x01E1,
            WM_MOUSEFIRST = 0x0200,
            WM_MOUSEMOVE = 0x0200,
            WM_LBUTTONDOWN = 0x0201,
            WM_LBUTTONUP = 0x0202,
            WM_LBUTTONDBLCLK = 0x0203,
            WM_RBUTTONDOWN = 0x0204,
            WM_RBUTTONUP = 0x0205,
            WM_RBUTTONDBLCLK = 0x0206,
            WM_MBUTTONDOWN = 0x0207,
            WM_MBUTTONUP = 0x0208,
            WM_MBUTTONDBLCLK = 0x0209,
            WM_MOUSEWHEEL = 0x020A,
            WM_XBUTTONDOWN = 0x020B,
            WM_XBUTTONUP = 0x020C,
            WM_XBUTTONDBLCLK = 0x020D,
            WM_MOUSEHWHEEL = 0x020E,
            WM_PARENTNOTIFY = 0x0210,
            WM_ENTERMENULOOP = 0x0211,
            WM_EXITMENULOOP = 0x0212,
            WM_NEXTMENU = 0x0213,
            WM_SIZING = 0x0214,
            WM_CAPTURECHANGED = 0x0215,
            WM_MOVING = 0x0216,
            WM_POWERBROADCAST = 0x0218,
            WM_DEVICECHANGE = 0x0219,
            WM_MDICREATE = 0x0220,
            WM_MDIDESTROY = 0x0221,
            WM_MDIACTIVATE = 0x0222,
            WM_MDIRESTORE = 0x0223,
            WM_MDINEXT = 0x0224,
            WM_MDIMAXIMIZE = 0x0225,
            WM_MDITILE = 0x0226,
            WM_MDICASCADE = 0x0227,
            WM_MDIICONARRANGE = 0x0228,
            WM_MDIGETACTIVE = 0x0229,
            WM_MDISETMENU = 0x0230,
            WM_ENTERSIZEMOVE = 0x0231,
            WM_EXITSIZEMOVE = 0x0232,
            WM_DROPFILES = 0x0233,
            WM_MDIREFRESHMENU = 0x0234,
            WM_IME_SETCONTEXT = 0x0281,
            WM_IME_NOTIFY = 0x0282,
            WM_IME_CONTROL = 0x0283,
            WM_IME_COMPOSITIONFULL = 0x0284,
            WM_IME_SELECT = 0x0285,
            WM_IME_CHAR = 0x0286,
            WM_IME_REQUEST = 0x0288,
            WM_IME_KEYDOWN = 0x0290,
            WM_IME_KEYUP = 0x0291,
            WM_MOUSEHOVER = 0x02A1,
            WM_MOUSELEAVE = 0x02A3,
            WM_NCMOUSEHOVER = 0x02A0,
            WM_NCMOUSELEAVE = 0x02A2,
            WM_WTSSESSION_CHANGE = 0x02B1,
            WM_TABLET_FIRST = 0x02c0,
            WM_TABLET_LAST = 0x02df,
            WM_CUT = 0x0300,
            WM_COPY = 0x0301,
            WM_PASTE = 0x0302,
            WM_CLEAR = 0x0303,
            WM_UNDO = 0x0304,
            WM_RENDERFORMAT = 0x0305,
            WM_RENDERALLFORMATS = 0x0306,
            WM_DESTROYCLIPBOARD = 0x0307,
            WM_DRAWCLIPBOARD = 0x0308,
            WM_PAINTCLIPBOARD = 0x0309,
            WM_VSCROLLCLIPBOARD = 0x030A,
            WM_SIZECLIPBOARD = 0x030B,
            WM_ASKCBFORMATNAME = 0x030C,
            WM_CHANGECBCHAIN = 0x030D,
            WM_HSCROLLCLIPBOARD = 0x030E,
            WM_QUERYNEWPALETTE = 0x030F,
            WM_PALETTEISCHANGING = 0x0310,
            WM_PALETTECHANGED = 0x0311,
            WM_HOTKEY = 0x0312,
            WM_PRINT = 0x0317,
            WM_PRINTCLIENT = 0x0318,
            WM_APPCOMMAND = 0x0319,
            WM_THEMECHANGED = 0x031A,
            WM_CLIPBOARDUPDATE = 0x031D,
            WM_DWMCOMPOSITIONCHANGED = 0x031E,
            WM_DWMNCRENDERINGCHANGED = 0x031F,
            WM_DWMCOLORIZATIONCOLORCHANGED = 0x0320,
            WM_DWMWINDOWMAXIMIZEDCHANGE = 0x0321,
            WM_GETTITLEBARINFOEX = 0x033F,
            WM_HANDHELDFIRST = 0x0358,
            WM_HANDHELDLAST = 0x035F,
            WM_AFXFIRST = 0x0360,
            WM_AFXLAST = 0x037F,
            WM_PENWINFIRST = 0x0380,
            WM_PENWINLAST = 0x038F,
            WM_APP = 0x8000,
            WM_USER = 0x0400,
            WM_REFLECT = WM_USER + 0x1C00,
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct MARGIN
        {
            public int cxLeftWidth;
            public int cxRightWidth;
            public int cyTopHeight;
            public int cyBottomHeight;
        }
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct WNDCLASSEX
        {
            public uint cbSize;
            public uint style;
            public IntPtr lpfnWndProc;
            public int cbClsExtra;
            public int cbWndExtra;
            public IntPtr hInstance;
            public IntPtr hIcon;
            public IntPtr hCursor;
            public IntPtr hbrBackground;
            public string lpszMenuName;
            public string lpszClassName;
            public IntPtr hIconSm;
            public static uint Size()
            {
                return (uint)Marshal.SizeOf(ObfuscatorNeedsThis<WNDCLASSEX>());
            }
            private static Type ObfuscatorNeedsThis<T>()
            {
                return typeof(T);
            }
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;        // x position of upper-left corner
            public int Top;         // y position of upper-left corner
            public int Right;       // x position of lower-right corner
            public int Bottom;      // y position of lower-right corner
        }
        #endregion
        #region LoadLibrary and GetProcAddress
        internal static class WinApi
        {
            [DllImport("kernel32.dll", EntryPoint = "GetProcAddress", SetLastError = false, CharSet = CharSet.Ansi)]
            private static extern IntPtr getProcAddress(IntPtr hmodule, string procName);
            [DllImport("kernel32.dll", EntryPoint = "LoadLibraryW", SetLastError = false, CharSet = CharSet.Unicode)]
            private static extern IntPtr loadLibraryW(string lpFileName);
            [DllImport("kernel32.dll", EntryPoint = "GetModuleHandleW", SetLastError = false, CharSet = CharSet.Unicode)]
            private static extern IntPtr getModuleHandle(string modulename);
            public static IntPtr GetProcAddress(string modulename, string procname)
            {
                IntPtr hModule = getModuleHandle(modulename);
                if (hModule == IntPtr.Zero) hModule = loadLibraryW(modulename);
                return getProcAddress(hModule, procname);
            }
            public static T GetMethod<T>(string modulename, string procname)
            {
                IntPtr hModule = getModuleHandle(modulename);
                if (hModule == IntPtr.Zero) hModule = loadLibraryW(modulename);
                IntPtr procAddress = getProcAddress(hModule, procname);
#if DEBUG
                if(hModule == IntPtr.Zero || procAddress == IntPtr.Zero)
                    throw new Exception("module: " + modulename + "\tproc: " + procname);
#endif
                return (T)(object)Marshal.GetDelegateForFunctionPointer(procAddress, ObfuscatorNeedsThis<T>());
            }
            private static Type ObfuscatorNeedsThis<T>()
            {
                return typeof(T);
            }
        }
#endregion
    }
}