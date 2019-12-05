using System;
using SharpDX.Direct2D1;

namespace GameOverlay.Drawing
{
    /// <summary>
    /// Represents a Brush used to draw with a Graphics surface.
    /// </summary>
    public interface IBrush
    {
        /// <summary>
        /// Gets or sets the Brush
        /// </summary>
        Brush Brush { get; set; }
    }
}
