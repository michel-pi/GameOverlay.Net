using System;

using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;

namespace GameOverlay.Graphics
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public interface ID2DBrush : IDisposable
    {
        /// <summary>
        /// Gets the brush.
        /// </summary>
        /// <returns></returns>
        Brush GetBrush();
        /// <summary>
        /// Sets the brush.
        /// </summary>
        /// <param name="brush">The brush.</param>
        void SetBrush(Brush brush);
    }
}
