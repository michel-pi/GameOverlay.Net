using SharpDX.DirectWrite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameOverlay.Drawing
{
    /// <summary>
    /// Defines drawing effect to apply to text.
    /// </summary>
    public class DrawingEffect
    {
        /// <summary>
        /// Brush to apply.
        /// </summary>
        public IBrush Brush;
        /// <summary>
        /// Range to apply brush to.
        /// </summary>
        public TextRange TextRange;

        private DrawingEffect()
        {
        }

        /// <summary>
        /// Initializes a new DrawingEffect with a bush and range.
        /// </summary>
        /// <param name="brush">The Brush to be applied.</param>
        /// <param name="startPostion">Start position in text to apply brush.</param>
        /// <param name="length">Length of characters to apply brush.</param>
        public DrawingEffect(IBrush brush, int startPostion, int length)
        {
            Brush = brush;
            TextRange = new TextRange(startPostion, length);
        }

    }
}
