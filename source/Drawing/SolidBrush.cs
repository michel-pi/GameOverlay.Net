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
        }

        /// <summary>
        /// Initializes a new SolidBrush for the given Graphics device using a transparent Color.
        /// </summary>
        /// <param name="renderTarget">A Graphics device.</param>
        public SolidBrush(RenderTarget renderTarget)
        {
            if (renderTarget == null) throw new ArgumentNullException(nameof(renderTarget));

            _brush = new SolidColorBrush(renderTarget, default);
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
        ~SolidBrush() => Dispose(false);

        /// <summary>
        /// Returns a value indicating whether this instance and a specified <see cref="T:System.Object" /> represent the same type and value.
        /// </summary>
        /// <param name="obj">The object to compare with this instance.</param>
        /// <returns><see langword="true" /> if <paramref name="obj" /> is a SolidBrush and equal to this instance; otherwise, <see langword="false" />.</returns>
        public override bool Equals(object obj)
        {
            if (obj is SolidBrush value)
            {
                return value._brush.NativePointer == _brush.NativePointer;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns a value indicating whether two specified instances of SolidBrush represent the same value.
        /// </summary>
        /// <param name="value">An object to compare to this instance.</param>
        /// <returns><see langword="true" /> if <paramref name="value" /> is equal to this instance; otherwise, <see langword="false" />.</returns>
        public bool Equals(SolidBrush value)
        {
            return value != null
                && value._brush.NativePointer == _brush.NativePointer;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            return OverrideHelper.HashCodes(
                _brush.NativePointer.GetHashCode());
        }

        /// <summary>
        /// Converts this SolidBrush to a human-readable string.
        /// </summary>
        /// <returns>The string representation of this SolidBrush.</returns>
        public override string ToString()
        {
            return OverrideHelper.ToString(
                "Brush", "SolidBrush",
                "Color", Color.ToString());
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

        /// <summary>
        /// Returns a value indicating whether two specified instances of SolidBrush represent the same value.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns> <see langword="true" /> if <paramref name="left" /> and <paramref name="right" /> are equal; otherwise, <see langword="false" />.</returns>
        public static bool Equals(SolidBrush left, SolidBrush right)
        {
            return left?.Equals(right) == true;
        }
    }
}
