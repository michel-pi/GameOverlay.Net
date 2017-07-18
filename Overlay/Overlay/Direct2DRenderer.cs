using System;
using System.Runtime.CompilerServices;

using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;

using Overlay.PInvoke.Libraries;
using Overlay.PInvoke.Structs;

using FontFactory = SharpDX.DirectWrite.Factory;
using Factory = SharpDX.Direct2D1.Factory;

namespace Overlay
{
    public class Direct2DRenderer
    {
        private static System.Drawing.Color GdiTransparentColor = System.Drawing.Color.Transparent;
        private static RawColor4 Direct2DTransparent = new RawColor4(GdiTransparentColor.R, GdiTransparentColor.G, GdiTransparentColor.B, GdiTransparentColor.A);

        private SolidColorBrush _brush;
        private TextFormat _font;

        private bool _resize = false;
        private int _resize_x = 0;
        private int _resize_y = 0;

        private WindowRenderTarget _device;
        private HwndRenderTargetProperties _targetProperties;
        private FontFactory _fontFactory;
        private Factory _factory;

        public IntPtr TargetHandle { get; private set; }
        public bool VSync { get; private set; }

        public Direct2DRenderer(IntPtr targetHwnd, bool vsync)
        {
            TargetHandle = targetHwnd;
            VSync = vsync;

            setupInstance();
        }

        ~Direct2DRenderer()
        {
            _brush.Dispose();
            _font.Dispose();

            _fontFactory.Dispose();
            _factory.Dispose();
            _device.Dispose();
        }

        private void setupInstance(bool deleteOld = false)
        {
            if (deleteOld)
            {
                try
                {
                    _brush.Dispose();
                    _font.Dispose();

                    _fontFactory.Dispose();
                    _factory.Dispose();
                    _device.Dispose();
                }
                catch
                {

                }
            }

            _factory = new Factory(SharpDX.Direct2D1.FactoryType.MultiThreaded, DebugLevel.None);
            _fontFactory = new FontFactory();

            RECT bounds = default(RECT);

            User32.GetWindowRect(TargetHandle, out bounds);

            _targetProperties = new HwndRenderTargetProperties()
            {
                Hwnd = TargetHandle,
                PixelSize = new Size2(Math.Abs(bounds.Right - bounds.Left), Math.Abs(bounds.Bottom - bounds.Top)),
                PresentOptions = VSync ? PresentOptions.None : PresentOptions.Immediately
            };

            var renderTargetProperties = new RenderTargetProperties(
                RenderTargetType.Hardware,
                new PixelFormat(Format.B8G8R8A8_UNorm, SharpDX.Direct2D1.AlphaMode.Premultiplied),
                0, 0,
                RenderTargetUsage.None, FeatureLevel.Level_DEFAULT
                );

            _device = new WindowRenderTarget(_factory, renderTargetProperties, _targetProperties);

            _device.TextAntialiasMode = SharpDX.Direct2D1.TextAntialiasMode.Aliased;
            _device.AntialiasMode = AntialiasMode.Aliased;

            _brush = new SolidColorBrush(_device, new RawColor4(0, 0, 0, 0));
        }

        public void Resize(int x, int y)
        {
            _resize_x = x;
            _resize_y = y;
            _resize = true;
        }

        public void CreateFont(string fontFamilyName, float size, bool bold = false, bool italic = false)
        {
            if (_font != null)
                _font.Dispose();
            _font = new TextFormat(_fontFactory, fontFamilyName, bold ? FontWeight.Bold : FontWeight.Normal, italic ? FontStyle.Italic : FontStyle.Normal, size);
        }

        public void BeginScene()
        {
            if (_device == null) return;

            MSG msg = default(MSG);

            if (User32.PeekMessage(out msg, IntPtr.Zero, 0, 0, 1) != 0)
            {
                User32.TranslateMessage(ref msg);
                User32.DispatchMessage(ref msg);
            }

            if (_resize)
            {
                _device.Resize(new Size2(_resize_x, _resize_y));
                _resize = false;
            }

            _device.BeginDraw();
        }

        public void ClearScene()
        {
            _device.Clear(Direct2DTransparent);
        }

        public void EndScene()
        {
            if (_device == null) return;

            long tag_1 = 0L, tag_2 = 0L;

            var result = _device.TryEndDraw(out tag_1, out tag_2);

            if (result.Failure)
            {
                setupInstance(true);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawLine(int start_x, int start_y, int end_x, int end_y, float stroke, System.Drawing.Color color)
        {
            _brush.Color = new RawColor4(color.R, color.G, color.B, color.A / 255.0f);
            _device.DrawLine(new RawVector2(start_x, start_y), new RawVector2(end_x, end_y), _brush, stroke);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawRectangle(float x, float y, float width, float height, float stroke, System.Drawing.Color color)
        {
            _brush.Color = new RawColor4(color.R, color.G, color.B, color.A / 255.0f);
            _device.DrawRectangle(new RawRectangleF(x, y, x + width, y + height), _brush, stroke);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawCircle(float x, float y, float radius, float stroke, System.Drawing.Color color)
        {
            _brush.Color = new RawColor4(color.R, color.G, color.B, color.A / 255.0f);
            _device.DrawEllipse(new Ellipse(new RawVector2(x, y), radius, radius), _brush, stroke);
        }

        public void DrawBox2D(float x, float y, float width, float height, float stroke, System.Drawing.Color color, System.Drawing.Color interiorColor)
        {
            _brush.Color = new RawColor4(color.R, color.G, color.B, color.A / 255.0f);
            _device.DrawRectangle(new RawRectangleF(x, y, x + width, y + height), _brush, stroke);
            _brush.Color = new RawColor4(interiorColor.R, interiorColor.G, interiorColor.B, interiorColor.A / 255.0f);
            _device.FillRectangle(new RawRectangleF(x + stroke, y + stroke, x + width - stroke, y + height - stroke), _brush);
        }

        public void DrawBox3D(float x, float y, float width, float height, int length, float stroke, System.Drawing.Color color, System.Drawing.Color interiorColor)
        {
            RawRectangleF first = new RawRectangleF(x, y, x + width, y + height);
            RawRectangleF second = new RawRectangleF(x + length, y - length, first.Right + length, first.Bottom - length);

            RawVector2 line_start = new RawVector2(x, y);
            RawVector2 line_end = new RawVector2(second.Left, second.Top);

            _brush.Color = new RawColor4(color.R, color.G, color.B, color.A / 255.0f);

            _device.DrawRectangle(first, _brush, stroke);
            _device.DrawRectangle(second, _brush, stroke);

            _device.DrawLine(line_start, line_end, _brush, stroke);

            line_start.X += width;
            line_end.X = line_start.X + length;

            _device.DrawLine(line_start, line_end, _brush, stroke);

            line_start.Y += height;
            line_end.Y += height;

            _device.DrawLine(line_start, line_end, _brush, stroke);

            line_start.X -= width;
            line_end.X -= width;

            _device.DrawLine(line_start, line_end, _brush, stroke);

            _brush.Color = new RawColor4(interiorColor.R, interiorColor.G, interiorColor.B, interiorColor.A / 255.0f);

            _device.FillRectangle(first, _brush);
            _device.FillRectangle(second, _brush);
        }

        public void DrawRectangle3D(float x, float y, float width, float height, int length, float stroke, System.Drawing.Color color)
        {
            RawRectangleF first = new RawRectangleF(x, y, x + width, y + height);
            RawRectangleF second = new RawRectangleF(x + length, y - length, first.Right + length, first.Bottom - length);

            RawVector2 line_start = new RawVector2(x, y);
            RawVector2 line_end = new RawVector2(second.Left, second.Top);

            _brush.Color = new RawColor4(color.R, color.G, color.B, color.A / 255.0f);

            _device.DrawRectangle(first, _brush, stroke);
            _device.DrawRectangle(second, _brush, stroke);

            _device.DrawLine(line_start, line_end, _brush, stroke);

            line_start.X += width;
            line_end.X = line_start.X + length;

            _device.DrawLine(line_start, line_end, _brush, stroke);

            line_start.Y += height;
            line_end.Y += height;

            _device.DrawLine(line_start, line_end, _brush, stroke);

            line_start.X -= width;
            line_end.X -= width;

            _device.DrawLine(line_start, line_end, _brush, stroke);
        }

        public void DrawPlus(float x, float y, int length, float stroke, System.Drawing.Color color)
        {
            RawVector2 first = new RawVector2(x - length, y);
            RawVector2 second = new RawVector2(x + length, y);

            RawVector2 third = new RawVector2(x, y - length);
            RawVector2 fourth = new RawVector2(x, y + length);

            _brush.Color = new RawColor4(color.R, color.G, color.B, color.A / 255.0f);

            _device.DrawLine(first, second, _brush, stroke);
            _device.DrawLine(third, fourth, _brush, stroke);
        }

        public void DrawEdge(float x, float y, float width, float height, int length, float stroke, System.Drawing.Color color)
        {
            RawVector2 first = new RawVector2(x, y);
            RawVector2 second = new RawVector2(x, y + length);
            RawVector2 third = new RawVector2(x + length, y);

            _brush.Color = new RawColor4(color.R, color.G, color.B, color.A / 255.0f);

            _device.DrawLine(first, second, _brush, stroke);
            _device.DrawLine(first, third, _brush, stroke);

            first.Y += height;
            second.Y = first.Y - length;
            third.Y = first.Y;
            third.X = first.X + length;

            _device.DrawLine(first, second, _brush, stroke);
            _device.DrawLine(first, third, _brush, stroke);

            first.X = x + width;
            first.Y = y;
            second.X = first.X - length;
            second.Y = first.Y;
            third.X = first.X;
            third.Y = first.Y + length;

            _device.DrawLine(first, second, _brush, stroke);
            _device.DrawLine(first, third, _brush, stroke);

            first.Y += height;
            second.X += length;
            second.Y = first.Y - length;
            third.Y = first.Y;
            third.X = first.X - length;

            _device.DrawLine(first, second, _brush, stroke);
            _device.DrawLine(first, third, _brush, stroke);
        }

        public void DrawBarH(float x, float y, float width, float height, float value, float stroke, System.Drawing.Color color, System.Drawing.Color interiorColor)
        {
            RawRectangleF first = new RawRectangleF(x, y, x + width, y + height);

            _brush.Color = new RawColor4(color.R, color.G, color.B, color.A / 255.0f);

            _device.DrawRectangle(first, _brush, stroke);

            if (value == 0)
                return;

            first.Top += height - ((float)height / 100.0f * value);

            _brush.Color = new RawColor4(interiorColor.R, interiorColor.G, interiorColor.B, interiorColor.A / 255.0f);

            _device.FillRectangle(first, _brush);
        }
        public void DrawBarV(float x, float y, float width, float height, float value, float stroke, System.Drawing.Color color, System.Drawing.Color interiorColor)
        {
            RawRectangleF first = new RawRectangleF(x, y, x + width, y + height);

            _brush.Color = new RawColor4(color.R, color.G, color.B, color.A / 255.0f);

            _device.DrawRectangle(first, _brush, stroke);

            if (value == 0)
                return;

            first.Right -= width - ((float)width / 100.0f * value);

            _brush.Color = new RawColor4(interiorColor.R, interiorColor.G, interiorColor.B, interiorColor.A / 255.0f);

            _device.FillRectangle(first, _brush);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void FillRectangle(float x, float y, float width, float height, System.Drawing.Color color)
        {
            _brush.Color = new RawColor4(color.R, color.G, color.B, color.A / 255.0f);
            _device.FillRectangle(new RawRectangleF(x, y, x + width, y + height), _brush);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void FillCircle(float x, float y, float radius, System.Drawing.Color color)
        {
            _brush.Color = new RawColor4(color.R, color.G, color.B, color.A / 255.0f);
            _device.FillEllipse(new Ellipse(new RawVector2(x, y), radius, radius), _brush);
        }

        public void BorderedLine(int start_x, int start_y, int end_x, int end_y, float stroke, System.Drawing.Color color, System.Drawing.Color borderColor)
        {
            _brush.Color = new RawColor4(color.R, color.G, color.B, color.A / 255.0f);

            _device.DrawLine(new RawVector2(start_x, start_y), new RawVector2(end_x, end_y), _brush, stroke);

            _brush.Color = new RawColor4(borderColor.R, borderColor.G, borderColor.B, borderColor.A / 255.0f);

            _device.DrawLine(new RawVector2(start_x, start_y - stroke), new RawVector2(end_x, end_y - stroke), _brush, stroke);
            _device.DrawLine(new RawVector2(start_x, start_y + stroke), new RawVector2(end_x, end_y + stroke), _brush, stroke);

            _device.DrawLine(new RawVector2(start_x - stroke / 2, start_y - stroke * 1.5f), new RawVector2(start_x - stroke / 2, start_y + stroke * 1.5f), _brush, stroke);
            _device.DrawLine(new RawVector2(end_x - stroke / 2, end_y - stroke * 1.5f), new RawVector2(end_x - stroke / 2, end_y + stroke * 1.5f), _brush, stroke);
        }

        public void BorderedRectangle(float x, float y, float width, float height, float stroke, float borderStroke, System.Drawing.Color color, System.Drawing.Color borderColor)
        {
            _brush.Color = new RawColor4(color.R, color.G, color.B, color.A / 255.0f);

            _device.DrawRectangle(new RawRectangleF(x, y, x + width, y + height), _brush, stroke);

            _brush.Color = new RawColor4(borderColor.R, borderColor.G, borderColor.B, borderColor.A / 255.0f);

            _device.DrawRectangle(new RawRectangleF(x - (stroke - borderStroke), y - (stroke - borderStroke), x + width + stroke - borderStroke, y + height + stroke - borderStroke), _brush, borderStroke);

            _device.DrawRectangle(new RawRectangleF(x + (stroke - borderStroke), y + (stroke - borderStroke), x + width - stroke + borderStroke, y + height - stroke + borderStroke), _brush, borderStroke);
        }

        public void BorderedCircle(float x, float y, float radius, float stroke, System.Drawing.Color color, System.Drawing.Color borderColor)
        {
            _brush.Color = new RawColor4(color.R, color.G, color.B, color.A / 255.0f);

            _device.DrawEllipse(new Ellipse(new RawVector2(x, y), radius, radius), _brush, stroke);

            _brush.Color = new RawColor4(borderColor.R, borderColor.G, borderColor.B, borderColor.A / 255.0f);

            _device.DrawEllipse(new Ellipse(new RawVector2(x, y), radius + stroke, radius + stroke), _brush, stroke);

            _device.DrawEllipse(new Ellipse(new RawVector2(x, y), radius - stroke, radius - stroke), _brush, stroke);
        }

        public void DrawText(string text, System.Drawing.Color color, float x, float y)
        {
            _brush.Color = new RawColor4(color.R, color.G, color.B, color.A / 255.0f);
            TextLayout layout = new TextLayout(_fontFactory, text, _font, float.MaxValue, float.MaxValue);
            _device.DrawTextLayout(new RawVector2(x, y), layout, _brush);
            layout.Dispose();
        }
    }
}
