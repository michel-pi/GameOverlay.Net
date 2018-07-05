using System;

using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;

namespace GameOverlay.Graphics
{
    /// <summary>
    /// Stores a Brush compatible with <c>Direct2DRenderer</c>
    /// </summary>
    public class D2DBrush
    {
        /// <summary>
        /// A <c>SolidColorBrush</c> to use with a rendering device
        /// </summary>
        public SolidColorBrush Brush;

        private D2DBrush()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Internal use only
        /// </summary>
        /// <param name="renderTarget"><c>RenderTarget</c> device</param>
        public D2DBrush(RenderTarget renderTarget)
        {
            Brush = new SolidColorBrush(renderTarget, default(RawColor4));
        }

        /// <summary>
        /// Internal use only
        /// </summary>
        /// <param name="renderTarget"><c>RenderTarget</c> device</param>
        /// <param name="color"><c>Direct2DColor</c> compatible color</param>
        public D2DBrush(RenderTarget renderTarget, D2DColor color)
        {
            Brush = new SolidColorBrush(renderTarget, color);
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="D2DBrush"/> class.
        /// </summary>
        ~D2DBrush()
        {
            Brush.Dispose();
        }

        /// <summary>
        /// Gets or sets the used color by this brush
        /// </summary>
        /// <value><c>Direct2DColor</c></value>
        public D2DColor Color
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

        /// <summary>
        /// Performs an implicit conversion from <see cref="D2DBrush"/> to <see cref="D2DColor"/>.
        /// </summary>
        /// <param name="brush">The brush.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator D2DColor(D2DBrush brush)
        {
            return brush.Color;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="D2DBrush"/> to <see cref="RawColor4"/>.
        /// </summary>
        /// <param name="brush">The brush.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator RawColor4(D2DBrush brush)
        {
            return brush.Color;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="D2DBrush"/> to <see cref="SolidColorBrush"/>.
        /// </summary>
        /// <param name="brush">The brush.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator SolidColorBrush(D2DBrush brush)
        {
            return brush.Brush;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return "{D2DBrush=" + Color.ToString() + "}";
        }
    }
}