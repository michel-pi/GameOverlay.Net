using System;
using SharpDX.Direct2D1;

namespace GameOverlay.Graphics
{
    /// <inheritdoc />
    /// <summary>
    /// </summary>
    /// <seealso cref="T:System.IDisposable" />
    public interface ID2DBrush : IDisposable
    {
        /// <summary>
        ///     Gets the brush.
        /// </summary>
        /// <returns></returns>
        Brush GetBrush();

        /// <summary>
        ///     Sets the brush.
        /// </summary>
        /// <param name="brush">The brush.</param>
        void SetBrush(Brush brush);
    }
}