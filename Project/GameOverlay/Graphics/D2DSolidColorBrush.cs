using System;
using System.Runtime.CompilerServices;
using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;

namespace GameOverlay.Graphics
{
    /// <inheritdoc />
    /// <summary>
    ///     Stores a Brush compatible with <c>Direct2DRenderer</c>
    /// </summary>
    public class D2DSolidColorBrush : ID2DBrush
    {
        /// <summary>
        ///     A <c>SolidColorBrush</c> to use with a rendering device
        /// </summary>
        public SolidColorBrush Brush;

        private D2DSolidColorBrush()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Internal use only
        /// </summary>
        /// <param name="device"><c>RenderTarget</c> device</param>
        public D2DSolidColorBrush(RenderTarget device)
        {
            if (device == null) throw new ArgumentNullException(nameof(device));

            Brush = new SolidColorBrush(device, default(RawColor4));
        }

        /// <summary>
        ///     Internal use only
        /// </summary>
        /// <param name="device"><c>RenderTarget</c> device</param>
        /// <param name="color"><c>Direct2DColor</c> compatible color</param>
        public D2DSolidColorBrush(RenderTarget device, D2DColor color)
        {
            if (device == null) throw new ArgumentNullException(nameof(device));

            Brush = new SolidColorBrush(device, color);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:GameOverlay.Graphics.D2DSolidColorBrush" /> class.
        /// </summary>
        /// <param name="device">The device.</param>
        public D2DSolidColorBrush(D2DDevice device) : this(device.GetRenderTarget())
        {
        }

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:GameOverlay.Graphics.D2DSolidColorBrush" /> class.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <param name="color">The color.</param>
        public D2DSolidColorBrush(D2DDevice device, D2DColor color) : this(device.GetRenderTarget(), color)
        {
        }

        /// <summary>
        ///     Gets or sets the used color by this brush
        /// </summary>
        /// <value>
        ///     <c>Direct2DColor</c>
        /// </value>
        public D2DColor Color
        {
            get => Brush.Color;
            set => Brush.Color = value;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Gets the brush.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Brush GetBrush()
        {
            return Brush;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Sets the brush.
        /// </summary>
        /// <param name="brush">The brush.</param>
        public void SetBrush(Brush brush)
        {
            Brush = (SolidColorBrush) brush;
        }

        /// <summary>
        ///     Finalizes an instance of the <see cref="D2DSolidColorBrush" /> class.
        /// </summary>
        ~D2DSolidColorBrush()
        {
            Dispose(false);
        }

        /// <summary>
        ///     Performs an implicit conversion from <see cref="D2DSolidColorBrush" /> to <see cref="SolidColorBrush" />.
        /// </summary>
        /// <param name="brush">The brush.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator SolidColorBrush(D2DSolidColorBrush brush)
        {
            return brush.Brush;
        }

        /// <summary>
        ///     Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        ///     A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return "{D2DBrush=" + Color + "}";
        }

        #region IDisposable Support

        private bool _disposedValue; // To detect redundant calls

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
        ///     unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposedValue) return;

            if(Brush != null) Brush.Dispose();

            _disposedValue = true;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}