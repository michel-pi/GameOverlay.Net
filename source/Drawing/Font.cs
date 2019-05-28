using System;

using SharpDX.DirectWrite;

using FontFactory = SharpDX.DirectWrite.Factory;

namespace GameOverlay.Drawing
{
    /// <summary>
    /// Defines a particular format for text, including font family name, size, and style attributes.
    /// </summary>
    public class Font : IDisposable
    {
        /// <summary>
        /// A Direct2D TextFormat.
        /// </summary>
        public TextFormat TextFormat;

        /// <summary>
        /// Gets a value that indicates whether this Font is bold.
        /// </summary>
        public bool Bold => TextFormat.FontWeight == FontWeight.Bold;

        /// <summary>
        /// Gets a value that indicates whether this Font is italic.
        /// </summary>
        public bool Italic => TextFormat.FontStyle == FontStyle.Italic;

        /// <summary>
        /// Enables or disables word wrapping for this Font.
        /// </summary>
        public bool WordWeapping
        {
            get => TextFormat.WordWrapping == WordWrapping.Wrap;
            set => TextFormat.WordWrapping = value ? WordWrapping.Wrap : WordWrapping.NoWrap;
        }

        /// <summary>
        /// Gets the size of this Font measured in pixels.
        /// </summary>
        public float FontSize => TextFormat.FontSize;

        /// <summary>
        /// Gets the name of this Fonts family
        /// </summary>
        public string FontFamilyName => TextFormat.FontFamilyName;

        private Font()
        {
        }

        /// <summary>
        /// Initializes a new Font by using the given text format.
        /// </summary>
        /// <param name="textFormat"></param>
        public Font(TextFormat textFormat)
            => TextFormat = textFormat ?? throw new ArgumentNullException();

        /// <summary>
        /// Initializes a new Font by using the specified name and style.
        /// </summary>
        /// <param name="factory">The FontFactory from your Graphics device.</param>
        /// <param name="fontFamilyName">The name of the font family.</param>
        /// <param name="size">The size of this Font.</param>
        /// <param name="bold">A Boolean value indicating whether this Font is bold.</param>
        /// <param name="italic">A Boolean value indicating whether this Font is italic.</param>
        /// <param name="wordWrapping">A Boolean value indicating whether this Font uses word wrapping.</param>
        public Font(FontFactory factory, string fontFamilyName, float size, bool bold = false, bool italic = false, bool wordWrapping = false)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            if (string.IsNullOrEmpty(fontFamilyName)) throw new ArgumentNullException(nameof(fontFamilyName));

            TextFormat = new TextFormat(factory, fontFamilyName, bold ? FontWeight.Bold : FontWeight.Normal, italic ? FontStyle.Italic : FontStyle.Normal, size)
            {
                WordWrapping = wordWrapping ? WordWrapping.Wrap : WordWrapping.NoWrap
            };
        }

        /// <summary>
        /// Allows an object to try to free resources and perform other cleanup operations before it is reclaimed by garbage collection.
        /// </summary>
        ~Font() => Dispose(false);

        /// <summary>
        /// Returns a value indicating whether this instance and a specified <see cref="T:System.Object" /> represent the same type and value.
        /// </summary>
        /// <param name="obj">The object to compare with this instance.</param>
        /// <returns><see langword="true" /> if <paramref name="obj" /> is a Font and equal to this instance; otherwise, <see langword="false" />.</returns>
        public override bool Equals(object obj)
        {
            if (obj is Font font)
            {
                return font.Bold == Bold
                    && font.Italic == Italic
                    && font.WordWeapping == WordWeapping
                    && font.FontSize == FontSize
                    && font.FontFamilyName == FontFamilyName;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns a value indicating whether two specified instances of Font represent the same value.
        /// </summary>
        /// <param name="value">An object to compare to this instance.</param>
        /// <returns><see langword="true" /> if <paramref name="value" /> is equal to this instance; otherwise, <see langword="false" />.</returns>
        public bool Equals(Font value)
        {
            return value != null
                && value.Bold == Bold
                && value.Italic == Italic
                && value.WordWeapping == WordWeapping
                && value.FontSize == FontSize
                && value.FontFamilyName == FontFamilyName;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            return OverrideHelper.HashCodes(
                Bold.GetHashCode(),
                Italic.GetHashCode(),
                WordWeapping.GetHashCode(),
                FontSize.GetHashCode(),
                FontFamilyName.GetHashCode());
        }

        /// <summary>
        /// Converts this Font instance to a human-readable string.
        /// </summary>
        /// <returns>A string representation of this Font.</returns>
        public override string ToString()
        {
            return OverrideHelper.ToString(
                "FontFamilyName", FontFamilyName,
                "FontSize", FontSize.ToString(),
                "Italic", Italic.ToString(),
                "Bold", Bold.ToString(),
                "WordWrapping", WordWeapping.ToString());
        }

        #region IDisposable Support
        private bool disposedValue = false;

        /// <summary>
        /// Releases all resources used by this Font.
        /// </summary>
        /// <param name="disposing">A Boolean value indicating whether this is called from the destructor.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                TextFormat?.Dispose();

                disposedValue = true;
            }
        }

        /// <summary>
        /// Releases all resources used by this Font.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        /// <summary>
        /// Converts this Font to a Direct2D TextFormat.
        /// </summary>
        /// <param name="font"></param>
        public static implicit operator TextFormat(Font font)
        {
            if (font == null) throw new ArgumentNullException(nameof(font));

            return font.TextFormat;
        }

        /// <summary>
        /// Returns a value indicating whether two specified instances of Font represent the same value.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns> <see langword="true" /> if <paramref name="left" /> and <paramref name="right" /> are equal; otherwise, <see langword="false" />.</returns>
        public static bool Equals(Font left, Font right)
        {
            return left?.Equals(right) == true;
        }
    }
}
