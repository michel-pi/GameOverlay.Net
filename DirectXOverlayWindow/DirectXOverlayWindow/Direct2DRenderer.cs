using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;

using FontFactory = SharpDX.DirectWrite.Factory;
using Factory = SharpDX.Direct2D1.Factory;

namespace DirectXOverlayWindow
{
    public class Direct2DRenderer
    {
        private List<SolidColorBrush> BrushContainer = new List<SolidColorBrush>(32);
        private List<TextFormat> FontContainer = new List<TextFormat>(32);

        private List<LayoutBuffer> LayoutContainer = new List<LayoutBuffer>(32);

        public int BufferBrushSize { get; private set; }
        public int BufferFontSize { get; private set; }
        public int BufferLayoutSize { get; private set; }

        //thread safe resizing
        private bool DoResize = false;
        private int ResizeX = 0;
        private int ResizeY = 0;

        //transparent background color
        private static System.Drawing.Color GDITransparent = System.Drawing.Color.Transparent;
        private static RawColor4 Transparent = new RawColor4(GDITransparent.R, GDITransparent.G, GDITransparent.B, GDITransparent.A);

        //direct x vars
        private WindowRenderTarget device;
        private HwndRenderTargetProperties targetProperties;
        private FontFactory fontFactory;
        private Factory factory;

        public Direct2DRenderer(IntPtr hwnd, bool limitFPS)
        {
            factory = new Factory();

            fontFactory = new FontFactory();

            Native.RECT bounds;
            Native.GetWindowRect(hwnd, out bounds);

            targetProperties = new HwndRenderTargetProperties
            {
                Hwnd = hwnd,
                PixelSize = new Size2(bounds.Right - bounds.Left, bounds.Bottom - bounds.Top),
                PresentOptions = limitFPS ? PresentOptions.None : PresentOptions.Immediately
            };

            RenderTargetProperties prop = new RenderTargetProperties(RenderTargetType.Hardware, new PixelFormat(Format.B8G8R8A8_UNorm, SharpDX.Direct2D1.AlphaMode.Premultiplied), 0, 0, RenderTargetUsage.None, FeatureLevel.Level_DEFAULT);

            device = new WindowRenderTarget(factory, prop, targetProperties);

            device.TextAntialiasMode = SharpDX.Direct2D1.TextAntialiasMode.Aliased;

            device.AntialiasMode = AntialiasMode.Aliased;
        }

        /// <summary>
        /// Do not call if you use OverlayWindow class
        /// </summary>
        public void Dispose()
        {
            this.DeleteBrushContainer();
            this.DeleteFontContainer();
            this.DeleteLayoutContainer();

            this.BrushContainer = null;
            this.FontContainer = null;
            this.LayoutContainer = null;

            fontFactory.Dispose();
            factory.Dispose();
            device.Dispose();
        }

        /// <summary>
        /// tells renderer to resize when possible
        /// </summary>
        /// <param name="x">Width</param>
        /// <param name="y">Height</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AutoResize(int x, int y)
        {
            this.DoResize = true;
            this.ResizeX = x;
            this.ResizeY = y;
        }

        #region Ressource Management
        /// <summary>
        /// Call this after EndScene if you created brushes within a loop
        /// </summary>
        public void DeleteBrushContainer()
        {
            this.BufferBrushSize = this.BrushContainer.Count;
            for (int i = 0; i < this.BrushContainer.Count; i++)
            {
                this.BrushContainer[i].Dispose();
            }
            this.BrushContainer = new List<SolidColorBrush>(this.BufferBrushSize);
        }
        /// <summary>
        /// Call this after EndScene if you created fonts within a loop
        /// </summary>
        public void DeleteFontContainer()
        {
            this.BufferFontSize = this.FontContainer.Count;
            for (int i = 0; i < this.FontContainer.Count; i++)
            {
                this.FontContainer[i].Dispose();
            }
            this.FontContainer = new List<TextFormat>(this.BufferFontSize);
        }
        /// <summary>
        /// Call this after EndScene if you changed your text's font or have problems with huge memory usage
        /// </summary>
        public void DeleteLayoutContainer()
        {
            this.BufferLayoutSize = this.LayoutContainer.Count;
            for (int i = 0; i < this.LayoutContainer.Count; i++)
            {
                this.LayoutContainer[i].Dispose();
            }
            this.LayoutContainer = new List<LayoutBuffer>(this.BufferLayoutSize);
        }

        /// <summary>
        /// Creates a new SolidColorBrush
        /// </summary>
        /// <param name="color">0x7FFFFFF Premultiplied alpha color</param>
        /// <returns>int Brush identifier</returns>
        public int CreateBrush(int color)
        {
            this.BrushContainer.Add(new SolidColorBrush(this.device, new RawColor4(color >> 16 & 255L, color >> 8 & 255L, (byte)color & 255L, (float)(color >> 24 & 255L))));
            return this.BrushContainer.Count - 1;
        }
        /// <summary>
        /// Creates a new SolidColorBrush. Make sure you applied an alpha value
        /// </summary>
        /// <param name="color">System.Drawing.Color struct</param>
        /// <returns>int Brush identifier</returns>
        public int CreateBrush(System.Drawing.Color color)
        {
            if (color.A == 0)
                color = System.Drawing.Color.FromArgb(255, color);

            this.BrushContainer.Add(new SolidColorBrush(this.device, new RawColor4(color.R, color.G, color.B, (float)color.A / 255.0f)));
            return this.BrushContainer.Count - 1;
        }

        /// <summary>
        /// Creates a new Font
        /// </summary>
        /// <param name="fontFamilyName">i.e. Arial</param>
        /// <param name="size">size in units</param>
        /// <param name="bold">print bold text</param>
        /// <param name="italic">print italic text</param>
        /// <returns></returns>
        public int CreateFont(string fontFamilyName, float size, bool bold = false, bool italic = false)
        {
            this.FontContainer.Add(new TextFormat(this.fontFactory, fontFamilyName, bold ? FontWeight.Bold : FontWeight.Normal, italic ? FontStyle.Italic : FontStyle.Normal, size));
            return this.FontContainer.Count - 1;
        }
        #endregion

        #region Scene related
        /// <summary>
        /// Do your drawing after this
        /// </summary>
        public void BeginScene()
        {
            if (this.DoResize)
            {
                this.device.Resize(new Size2(this.ResizeX, this.ResizeY));

                this.DoResize = false;
            }
            device.BeginDraw();
        }
        /// <summary>
        /// Present frame. Do not draw after this.
        /// </summary>
        public void EndScene()
        {
            device.EndDraw();
            if (this.DoResize)
            {
                this.device.Resize(new Size2(this.ResizeX, this.ResizeY));

                this.DoResize = false;
            }
        }
        /// <summary>
        /// Clears the frame
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ClearScene()
        {
            //var color = System.Drawing.Color.Transparent;
            device.Clear(Transparent);//new RawColor4(color.R, color.G, color.B, color.A));
        }
        #endregion

        #region Special stuff
        public void DrawSwastika(int x, int y, int size, float stroke, int brush)
        {
            RawVector2 first = new RawVector2(x - size, y);
            RawVector2 second = new RawVector2(x + size, y);

            RawVector2 third = new RawVector2(x, y - size);
            RawVector2 fourth = new RawVector2(x, y + size);

            RawVector2 haken_1 = new RawVector2(third.X + size, third.Y);
            RawVector2 haken_2 = new RawVector2(second.X, second.Y + size);
            RawVector2 haken_3 = new RawVector2(fourth.X - size, fourth.Y);
            RawVector2 haken_4 = new RawVector2(first.X, first.Y - size);

            device.DrawLine(first, second, this.BrushContainer[brush], stroke);
            device.DrawLine(third, fourth, this.BrushContainer[brush], stroke);

            device.DrawLine(third, haken_1, this.BrushContainer[brush], stroke);
            device.DrawLine(second, haken_2, this.BrushContainer[brush], stroke);
            device.DrawLine(fourth, haken_3, this.BrushContainer[brush], stroke);
            device.DrawLine(first, haken_4, this.BrushContainer[brush], stroke);
        }

        /// <summary>
        /// Fake rotation and pulsating
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="size"></param>
        /// <param name="stroke"></param>
        /// <param name="rotation">0 to 180</param>
        /// <param name="brush"></param>
        public void RotateSwastika(int x, int y, int size, float stroke, float rotation, int brush)
        {
            RawVector2 first = new RawVector2(x - size, y - rotation);
            RawVector2 second = new RawVector2(x + size, y + rotation);

            RawVector2 third = new RawVector2(x + rotation, y - size);
            RawVector2 fourth = new RawVector2(x - rotation, y + size);

            RawVector2 haken_1 = new RawVector2(third.X + size, third.Y + rotation);
            RawVector2 haken_2 = new RawVector2(second.X - rotation, second.Y + size);
            RawVector2 haken_3 = new RawVector2(fourth.X - size, fourth.Y - rotation);
            RawVector2 haken_4 = new RawVector2(first.X + rotation, first.Y - size);

            device.DrawLine(first, second, this.BrushContainer[brush], stroke);
            device.DrawLine(third, fourth, this.BrushContainer[brush], stroke);

            device.DrawLine(third, haken_1, this.BrushContainer[brush], stroke);
            device.DrawLine(second, haken_2, this.BrushContainer[brush], stroke);
            device.DrawLine(fourth, haken_3, this.BrushContainer[brush], stroke);
            device.DrawLine(first, haken_4, this.BrushContainer[brush], stroke);
        }
        #endregion

        #region Drawing stuff
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawLine(int start_x, int start_y, int end_x, int end_y, float stroke, int brush)
        {
            device.DrawLine(new RawVector2(start_x, start_y), new RawVector2(end_x, end_y), this.BrushContainer[brush], stroke);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawRectangle(int x, int y, int width, int height, float stroke, int brush)
        {
            device.DrawRectangle(new RawRectangleF(x, y, x + width, y + height), this.BrushContainer[brush], stroke);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawCircle(int x, int y, int radius, float stroke, int brush)
        {
            device.DrawEllipse(new Ellipse(new RawVector2(x, y), radius, radius), this.BrushContainer[brush], stroke);
        }

        public void DrawBox2D(int x, int y, int width, int height, float stroke, int brush, int interiorBrush)
        {
            device.DrawRectangle(new RawRectangleF(x, y, x + width, y + height), this.BrushContainer[brush], stroke);
            device.FillRectangle(new RawRectangleF(x + stroke, y + stroke, x + width - stroke, y + height - stroke), this.BrushContainer[interiorBrush]);
        }

        public void DrawBox3D(int x, int y, int width, int height, int length, float stroke, int brush, int interiorBrush)
        {
            RawRectangleF first = new RawRectangleF(x, y, x + width, y + height);
            RawRectangleF second = new RawRectangleF(x + length, y - length, first.Right + length, first.Bottom - length);

            RawVector2 line_start = new RawVector2(x, y);
            RawVector2 line_end = new RawVector2(second.Left, second.Top);

            device.DrawRectangle(first, this.BrushContainer[brush], stroke);
            device.DrawRectangle(second, this.BrushContainer[brush], stroke);

            device.FillRectangle(first, this.BrushContainer[interiorBrush]);
            device.FillRectangle(second, this.BrushContainer[interiorBrush]);

            device.DrawLine(line_start, line_end, this.BrushContainer[brush], stroke);

            line_start.X += width;
            line_end.X = line_start.X + length;

            device.DrawLine(line_start, line_end, this.BrushContainer[brush], stroke);

            line_start.Y += height;
            line_end.Y += height;

            device.DrawLine(line_start, line_end, this.BrushContainer[brush], stroke);

            line_start.X -= width;
            line_end.X -= width;

            device.DrawLine(line_start, line_end, this.BrushContainer[brush], stroke);
        }

        public void DrawRectangle3D(int x, int y, int width, int height, int length, float stroke, int brush)
        {
            RawRectangleF first = new RawRectangleF(x, y, x + width, y + height);
            RawRectangleF second = new RawRectangleF(x + length, y - length, first.Right + length, first.Bottom - length);

            RawVector2 line_start = new RawVector2(x, y);
            RawVector2 line_end = new RawVector2(second.Left, second.Top);

            device.DrawRectangle(first, this.BrushContainer[brush], stroke);
            device.DrawRectangle(second, this.BrushContainer[brush], stroke);

            device.DrawLine(line_start, line_end, this.BrushContainer[brush], stroke);

            line_start.X += width;
            line_end.X = line_start.X + length;

            device.DrawLine(line_start, line_end, this.BrushContainer[brush], stroke);

            line_start.Y += height;
            line_end.Y += height;

            device.DrawLine(line_start, line_end, this.BrushContainer[brush], stroke);

            line_start.X -= width;
            line_end.X -= width;

            device.DrawLine(line_start, line_end, this.BrushContainer[brush], stroke);
        }

        public void DrawPlus(int x, int y, int length, float stroke, int brush)
        {
            RawVector2 first = new RawVector2(x - length, y);
            RawVector2 second = new RawVector2(x + length, y);

            RawVector2 third = new RawVector2(x, y - length);
            RawVector2 fourth = new RawVector2(x, y + length);

            device.DrawLine(first, second, this.BrushContainer[brush], stroke);
            device.DrawLine(third, fourth, this.BrushContainer[brush], stroke);
        }

        public void DrawEdge(int x, int y, int width, int height, int length, float stroke, int brush)//geht
        {
            RawVector2 first = new RawVector2(x, y);
            RawVector2 second = new RawVector2(x, y + length);
            RawVector2 third = new RawVector2(x + length, y);

            device.DrawLine(first, second, this.BrushContainer[brush], stroke);
            device.DrawLine(first, third, this.BrushContainer[brush], stroke);

            first.Y += height;
            second.Y = first.Y - length;
            third.Y = first.Y;
            third.X = first.X + length;

            device.DrawLine(first, second, this.BrushContainer[brush], stroke);
            device.DrawLine(first, third, this.BrushContainer[brush], stroke);

            first.X = x + width;
            first.Y = y;
            second.X = first.X - length;
            second.Y = first.Y;
            third.X = first.X;
            third.Y = first.Y + length;

            device.DrawLine(first, second, this.BrushContainer[brush], stroke);
            device.DrawLine(first, third, this.BrushContainer[brush], stroke);

            first.Y += height;
            second.X += length;
            second.Y = first.Y - length;
            third.Y = first.Y;
            third.X = first.X - length;

            device.DrawLine(first, second, this.BrushContainer[brush], stroke);
            device.DrawLine(first, third, this.BrushContainer[brush], stroke);
        }

        public void DrawBarH(int x, int y, int width, int height, float value, float stroke, int brush, int interiorBrush)
        {
            RawRectangleF first = new RawRectangleF(x, y, x + width, y + height);

            device.DrawRectangle(first, this.BrushContainer[brush], stroke);

            if (value == 0)
                return;

            first.Top += height - ((float)height / 100.0f * value);

            device.FillRectangle(first, this.BrushContainer[interiorBrush]);
        }
        public void DrawBarV(int x, int y, int width, int height, float value, float stroke, int brush, int interiorBrush)
        {
            RawRectangleF first = new RawRectangleF(x, y, x + width, y + height);

            device.DrawRectangle(first, this.BrushContainer[brush], stroke);

            if (value == 0)
                return;

            first.Right -= width - ((float)width / 100.0f * value);

            device.FillRectangle(first, this.BrushContainer[interiorBrush]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void FillRectangle(int x, int y, int width, int height, int brush)
        {
            device.FillRectangle(new RawRectangleF(x, y, x + width, y + height), this.BrushContainer[brush]);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void FillCircle(int x, int y, int radius, int brush)
        {
            device.FillEllipse(new Ellipse(new RawVector2(x, y), radius, radius), this.BrushContainer[brush]);
        }

        public void BorderedLine(int start_x, int start_y, int end_x, int end_y, float stroke, int brush, int borderBrush)
        {
            device.DrawLine(new RawVector2(start_x, start_y), new RawVector2(end_x, end_y), this.BrushContainer[brush], stroke);

            device.DrawLine(new RawVector2(start_x, start_y - stroke), new RawVector2(end_x, end_y - stroke), this.BrushContainer[borderBrush], stroke);
            device.DrawLine(new RawVector2(start_x, start_y + stroke), new RawVector2(end_x, end_y + stroke), this.BrushContainer[borderBrush], stroke);

            device.DrawLine(new RawVector2(start_x - stroke / 2, start_y - stroke * 1.5f), new RawVector2(start_x - stroke / 2, start_y + stroke * 1.5f), this.BrushContainer[borderBrush], stroke);
            device.DrawLine(new RawVector2(end_x - stroke / 2, end_y - stroke * 1.5f), new RawVector2(end_x - stroke / 2, end_y + stroke * 1.5f), this.BrushContainer[borderBrush], stroke);
        }

        public void BorderedRectangle(int x, int y, int width, int height, float stroke, float borderStroke, int brush, int borderBrush)
        {
            device.DrawRectangle(new RawRectangleF(x - (stroke - borderStroke), y - (stroke - borderStroke), x + width + stroke - borderStroke, y + height + stroke - borderStroke), this.BrushContainer[borderBrush], borderStroke);

            device.DrawRectangle(new RawRectangleF(x, y, x + width, y + height), this.BrushContainer[brush], stroke);

            device.DrawRectangle(new RawRectangleF(x + (stroke - borderStroke), y + (stroke - borderStroke), x + width - stroke + borderStroke, y + height - stroke + borderStroke), this.BrushContainer[borderBrush], borderStroke);
        }

        public void BorderedCircle(int x, int y, int radius, float stroke, int brush, int borderBrush)
        {
            device.DrawEllipse(new Ellipse(new RawVector2(x, y), radius + stroke, radius + stroke), this.BrushContainer[borderBrush], stroke);

            device.DrawEllipse(new Ellipse(new RawVector2(x, y), radius, radius), this.BrushContainer[brush], stroke);

            device.DrawEllipse(new Ellipse(new RawVector2(x, y), radius - stroke, radius - stroke), this.BrushContainer[borderBrush], stroke);
        }

        /// <summary>
        /// Do not buffer text if you draw i.e. FPS. Use buffer for player names, rank....
        /// </summary>
        public void DrawText(string text, int font, int brush, int x, int y, bool bufferText = true)
        {
            if (bufferText)
            {
                int bufferPos = -1;

                for (int i = 0; i < this.LayoutContainer.Count; i++)
                {
                    if (this.LayoutContainer[i].Text.Length == text.Length && this.LayoutContainer[i].Text == text)
                    {
                        bufferPos = i;
                        break;
                    }
                }

                if (bufferPos == -1)
                {
                    this.LayoutContainer.Add(new LayoutBuffer(text, new TextLayout(this.fontFactory, text, this.FontContainer[font], float.MaxValue, float.MaxValue)));
                    bufferPos = this.LayoutContainer.Count - 1;
                }

                device.DrawTextLayout(new RawVector2(x, y), this.LayoutContainer[bufferPos].TextLayout, this.BrushContainer[brush], DrawTextOptions.NoSnap);
            }
            else
            {
                TextLayout layout = new TextLayout(this.fontFactory, text, this.FontContainer[font], float.MaxValue, float.MaxValue);
                device.DrawTextLayout(new RawVector2(x, y), layout, this.BrushContainer[brush]);
                layout.Dispose();
            }
        }
        #endregion
    }
}
