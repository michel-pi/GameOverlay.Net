using System;

using SharpDX.DirectWrite;

namespace GameOverlay.Graphics
{
    /// <summary>
    /// </summary>
    public class FontOptions
    {
        /// <summary>
        /// The bold
        /// </summary>
        public bool Bold;

        /// <summary>
        /// The font family name
        /// </summary>
        public string FontFamilyName;

        /// <summary>
        /// The font size
        /// </summary>
        public float FontSize;

        /// <summary>
        /// The italic
        /// </summary>
        public bool Italic;

        /// <summary>
        /// The word wrapping
        /// </summary>
        public bool WordWrapping;

        /// <summary>
        /// Gets the style.
        /// </summary>
        /// <returns></returns>
        public FontStyle GetStyle()
        {
            if (Italic) return FontStyle.Italic;
            return FontStyle.Normal;
        }
    }
}