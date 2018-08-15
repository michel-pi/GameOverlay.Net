using System;
using System.Runtime.CompilerServices;

using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;

namespace GameOverlay.Graphics
{
    /// <summary>
    /// 
    /// </summary>
    public class D2DLinearGradientBrush : ID2DBrush
    {
        /// <summary>
        /// The brush
        /// </summary>
        public LinearGradientBrush Brush;

        /// <summary>
        /// Gets or sets the start point.
        /// </summary>
        /// <value>
        /// The start point.
        /// </value>
        public Primitives.Point StartPoint
        {
            get
            {
                return Brush.StartPoint;
            }
            set
            {
                Brush.StartPoint = value;
            }
        }

        /// <summary>
        /// Gets or sets the end point.
        /// </summary>
        /// <value>
        /// The end point.
        /// </value>
        public Primitives.Point EndPoint
        {
            get
            {
                return Brush.EndPoint;
            }
            set
            {
                Brush.EndPoint = value;
            }
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="D2DLinearGradientBrush"/> class from being created.
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private D2DLinearGradientBrush()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="D2DLinearGradientBrush"/> class.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <param name="colors">The colors.</param>
        /// <exception cref="ArgumentNullException">
        /// device
        /// or
        /// colors
        /// </exception>
        public D2DLinearGradientBrush(RenderTarget device, params D2DColor[] colors)
        {
            if (device == null) throw new ArgumentNullException(nameof(device));
            if (colors == null || colors.Length == 0) throw new ArgumentNullException(nameof(colors));

            float position = 0.0f;
            float stepSize = 1.0f / colors.Length;

            var gradientStops = new GradientStop[colors.Length];

            for (int i = 0; i < colors.Length; i++)
            {
                gradientStops[i] = new GradientStop()
                {
                    Color = colors[i],
                    Position = position
                };

                position += stepSize;
            }

            Brush = new LinearGradientBrush(device,
                new LinearGradientBrushProperties(),
                new GradientStopCollection(device, gradientStops, ExtendMode.Clamp));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="D2DLinearGradientBrush"/> class.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <param name="colors">The colors.</param>
        public D2DLinearGradientBrush(D2DDevice device, params D2DColor[] colors) : this(device.GetRenderTarget(), colors)
        {

        }

        /// <summary>
        /// Finalizes an instance of the <see cref="D2DLinearGradientBrush"/> class.
        /// </summary>
        ~D2DLinearGradientBrush()
        {
            Dispose(false);
        }

        /// <summary>
        /// Sets the range.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        public void SetRange(Primitives.Point start, Primitives.Point end)
        {
            StartPoint = start;
            EndPoint = end;
        }

        /// <summary>
        /// Sets the range.
        /// </summary>
        /// <param name="startX">The start x.</param>
        /// <param name="startY">The start y.</param>
        /// <param name="endX">The end x.</param>
        /// <param name="endY">The end y.</param>
        public void SetRange(float startX, float startY, float endX, float endY)
        {
            Brush.StartPoint = new RawVector2(startX, startY);
            Brush.EndPoint = new RawVector2(endX, endY);
        }

        /// <summary>
        /// Gets the brush.
        /// </summary>
        /// <returns></returns>
        public Brush GetBrush()
        {
            return Brush;
        }

        /// <summary>
        /// Sets the brush.
        /// </summary>
        /// <param name="brush">The brush.</param>
        public void SetBrush(Brush brush)
        {
            Brush = (LinearGradientBrush)brush;
        }

        #region IDisposable Support
        private bool _disposedValue = false;

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if(Brush != null) Brush.Dispose();

                _disposedValue = true;
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
