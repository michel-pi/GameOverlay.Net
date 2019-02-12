using System;

using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;

namespace GameOverlay.Drawing
{
    /// <summary>
    /// Represents a SolidBrush which is used for drawing on a Graphics surface.
    /// </summary>
    public class SolidBrush : IDisposable, IBrush
    {
        private SolidColorBrush _brush;

        /// <summary>
        /// Gets or sets the underlying Brush.
        /// </summary>
        public Brush Brush { get => _brush; set => _brush = (SolidColorBrush)value; }

        /// <summary>
        /// Gets or sets the Color of the underlying Brush.
        /// </summary>
        public Color Color { get => _brush.Color; set => _brush.Color = value; }

        private SolidBrush()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Initializes a new SolidBrush for the given Graphics device using a transparent Color.
        /// </summary>
        /// <param name="renderTarget">A Graphics device.</param>
        public SolidBrush(RenderTarget renderTarget)
        {
            if (renderTarget == null) throw new ArgumentNullException(nameof(renderTarget));

            _brush = new SolidColorBrush(renderTarget, default(RawColor4));
        }

        /// <summary>
        /// Initializes a new SolidBrush for the given Graphics device using the given Color.
        /// </summary>
        /// <param name="renderTarget">A Graphics device.</param>
        /// <param name="color">A Color structure including the color components for this SolidBrush.</param>
        public SolidBrush(RenderTarget renderTarget, Color color)
        {
            if (renderTarget == null) throw new ArgumentNullException(nameof(renderTarget));

            _brush = new SolidColorBrush(renderTarget, color);
        }

        /// <summary>
        /// Allows an object to try to free resources and perform other cleanup operations before it is reclaimed by garbage collection.
        /// </summary>
        ~SolidBrush()
        {
            Dispose(false);
        }

        /// <summary>
        /// Converts this SolidBrush to a human-readable string.
        /// </summary>
        /// <returns>The string representation of this SolidBrush.</returns>
        public override string ToString()
        {
            return "{Brush=" + Color.ToString() + "}"; 
        }

        #region IDisposable Support
        private bool disposedValue = false;

        /// <summary>
        /// Releases all resources used by this SolidBrush.
        /// </summary>
        /// <param name="disposing">A Boolean value indicating whether this is called from the destructor.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (_brush != null)
                {
                    _brush.Dispose();
                    _brush = null;
                }

                disposedValue = true;
            }
        }

        /// <summary>
        /// Releases all resources used by this SolidBrush.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        /// <summary>
        /// Converts a SolidBrush to a SharpDX SolidColorBrush-
        /// </summary>
        /// <param name="brush">A SolidBrush.</param>
        public static implicit operator SolidColorBrush(SolidBrush brush)
        {
            return brush._brush;
        }
    }
}
