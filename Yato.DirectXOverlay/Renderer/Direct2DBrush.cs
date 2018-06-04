using System;

using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;

namespace Yato.DirectXOverlay.Renderer
{
    /// <summary>
    /// Stores a Brush compatible with <c>Direct2DRenderer</c>
    /// </summary>
    public class Direct2DBrush
    {
        /// <summary>
        /// A <c>SolidColorBrush</c> to use with a rendering device
        /// </summary>
        public SolidColorBrush Brush;

        private Direct2DBrush()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Internal use only
        /// </summary>
        /// <param name="renderTarget"><c>RenderTarget</c> device</param>
        public Direct2DBrush(RenderTarget renderTarget)
        {
            Brush = new SolidColorBrush(renderTarget, default(RawColor4));
        }

        /// <summary>
        /// Internal use only
        /// </summary>
        /// <param name="renderTarget"><c>RenderTarget</c> device</param>
        /// <param name="color"><c>Direct2DColor</c> compatible color</param>
        public Direct2DBrush(RenderTarget renderTarget, Direct2DColor color)
        {
            Brush = new SolidColorBrush(renderTarget, color);
        }

        ~Direct2DBrush()
        {
            Brush.Dispose();
        }

        /// <summary>
        /// Gets or sets the used color by this brush
        /// </summary>
        /// <value><c>Direct2DColor</c></value>
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

        public static implicit operator Direct2DColor(Direct2DBrush brush)
        {
            return brush.Color;
        }

        public static implicit operator RawColor4(Direct2DBrush brush)
        {
            return brush.Color;
        }

        public static implicit operator SolidColorBrush(Direct2DBrush brush)
        {
            return brush.Brush;
        }
    }
}