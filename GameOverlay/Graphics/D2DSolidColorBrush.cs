using System;
using System.Runtime.CompilerServices;

using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;

namespace GameOverlay.Graphics
{
    /// <summary>
    /// Stores a Brush compatible with <c>Direct2DRenderer</c>
    /// </summary>
    public class D2DSolidColorBrush : ID2DBrush
    {
        /// <summary>
        /// A <c>SolidColorBrush</c> to use with a rendering device
        /// </summary>
        public SolidColorBrush Brush;

        private D2DSolidColorBrush()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Internal use only
        /// </summary>
        /// <param name="device"><c>RenderTarget</c> device</param>
        public D2DSolidColorBrush(RenderTarget device)
        {
            if (device == null) throw new ArgumentNullException(nameof(device));

            Brush = new SolidColorBrush(device, default(RawColor4));
        }

        /// <summary>
        /// Internal use only
        /// </summary>
        /// <param name="device"><c>RenderTarget</c> device</param>
        /// <param name="color"><c>Direct2DColor</c> compatible color</param>
        public D2DSolidColorBrush(RenderTarget device, D2DColor color)
        {
            if (device == null) throw new ArgumentNullException(nameof(device));

            Brush = new SolidColorBrush(device, color);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="D2DSolidColorBrush"/> class.
        /// </summary>
        /// <param name="device">The device.</param>
        public D2DSolidColorBrush(D2DDevice device) : this(device.GetRenderTarget())
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="D2DSolidColorBrush"/> class.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <param name="color">The color.</param>
        public D2DSolidColorBrush(D2DDevice device, D2DColor color) : this(device.GetRenderTarget(), color)
        {

        }

        /// <summary>
        /// Finalizes an instance of the <see cref="D2DSolidColorBrush"/> class.
        /// </summary>
        ~D2DSolidColorBrush()
        {
            Dispose();
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
        /// Performs an implicit conversion from <see cref="D2DSolidColorBrush"/> to <see cref="SolidColorBrush"/>.
        /// </summary>
        /// <param name="brush">The brush.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator SolidColorBrush(D2DSolidColorBrush brush)
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
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Brush GetBrush()
        {
            return Brush;
        }

        public void SetBrush(Brush brush)
        {
            Brush = (SolidColorBrush)brush;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                Brush.Dispose();

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