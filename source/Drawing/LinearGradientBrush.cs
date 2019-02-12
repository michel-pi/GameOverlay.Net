using System;

using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;

using SharpDXGradientBrush = SharpDX.Direct2D1.LinearGradientBrush;

namespace GameOverlay.Drawing
{
    /// <summary>
    /// Represents a linear gradient brush used with a Graphics surface.
    /// </summary>
    public class LinearGradientBrush : IDisposable, IBrush
    {
        private SharpDXGradientBrush _brush;

        /// <summary>
        /// Gets or sets the underlying Brush.
        /// </summary>
        public Brush Brush { get => _brush; set => _brush = (SharpDXGradientBrush)value; }

        /// <summary>
        /// Gets or sets the start point of this LineatGradientBrush.
        /// </summary>
        public Point Start { get => _brush.StartPoint; set => _brush.StartPoint = value; }
        /// <summary>
        /// Gets or sets the end point of this LineatGradientBrush.
        /// </summary>
        public Point End { get => _brush.EndPoint; set => _brush.EndPoint = value; }

        private LinearGradientBrush()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Initializes a new LinearGradientBrush using the target device and an Color[].
        /// </summary>
        /// <param name="device">The Graphics device.</param>
        /// <param name="colors">The colors</param>
        public LinearGradientBrush(RenderTarget device, params Color[] colors)
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

            Brush = new SharpDXGradientBrush(device,
                new LinearGradientBrushProperties(),
                new GradientStopCollection(device, gradientStops, ExtendMode.Clamp));
        }

        /// <summary>
        /// Initializes a new LinearGradientBrush using the target device and an Color[].
        /// </summary>
        /// <param name="device">The Graphics device.</param>
        /// <param name="colors">The colors</param>
        public LinearGradientBrush(Graphics device, params Color[] colors) : this(device.GetRenderTarget(), colors)
        {

        }

        /// <summary>
        /// Allows an object to try to free resources and perform other cleanup operations before it is reclaimed by garbage collection.
        /// </summary>
        ~LinearGradientBrush()
        {
            Dispose(false);
        }

        /// <summary>
        /// Sets the range where the gradient gets applied.
        /// </summary>
        /// <param name="start">A Point structure inclduing the coordinates for the start point of this LinearGradientBrush.</param>
        /// <param name="end">A Point structure inclduing the coordinates for the end point of this LinearGradientBrush.</param>
        public void SetRange(Point start, Point end)
        {
            _brush.StartPoint = start;
            _brush.EndPoint = end;
        }

        /// <summary>
        /// Sets the range where the gradient gets applied.
        /// </summary>
        /// <param name="startX">The x-coordinate of the start point of this LinearGradientBrush.</param>
        /// <param name="startY">The y-coordinate of the start point of this LinearGradientBrush.</param>
        /// <param name="endX">The x-coordinate of the end point of this LinearGradientBrush.</param>
        /// <param name="endY">The y-coordinate of the end point of this LinearGradientBrush.</param>
        public void SetRange(float startX, float startY, float endX, float endY)
        {
            _brush.StartPoint = new RawVector2(startX, startY);
            _brush.EndPoint = new RawVector2(endX, endY);
        }

        #region IDisposable Support
        private bool disposedValue = false;

        /// <summary>
        /// Releases all resources used by this LinearGradientBrush.
        /// </summary>
        /// <param name="disposing">A Boolean value indicating whether this is called from the destructor.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }

                disposedValue = true;
            }
        }

        /// <summary>
        /// Releases all resources used by this LinearGradientBrush.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
