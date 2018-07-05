using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOverlay.Graphics.Primitives
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="GameOverlay.Graphics.IShape" />
    public class Text : IShape
    {
        /// <summary>
        /// The content
        /// </summary>
        public string Content;

        /// <summary>
        /// The font
        /// </summary>
        public D2DFont Font;

        /// <summary>
        /// Draws the specified device.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <exception cref="NotImplementedException"></exception>
        public void Draw(D2DDevice device)
        {
            throw new NotImplementedException();
        }
    }
}
