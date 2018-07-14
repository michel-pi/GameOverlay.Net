using System;

using SharpDX.DirectWrite;

using FontFactory = SharpDX.DirectWrite.Factory;

namespace GameOverlay.Graphics
{
    /// <summary>
    /// Stores a <c>Direct2DRenderer</c> compatible font
    /// </summary>
    public class D2DFont
    {
        private FontFactory _fontFactory;

        /// <summary>
        /// The font
        /// </summary>
        public TextFormat Font;

        private D2DFont()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Internal use only
        /// </summary>
        /// <param name="font">The font.</param>
        public D2DFont(TextFormat font)
        {
            Font = font ?? throw new ArgumentNullException(nameof(font));
        }

        /// <summary>
        /// Internal use only
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="fontFamilyName">Name of the font family.</param>
        /// <param name="size">The size.</param>
        /// <param name="bold">if set to <c>true</c> [bold].</param>
        /// <param name="italic">if set to <c>true</c> [italic].</param>
        public D2DFont(FontFactory factory, string fontFamilyName, float size, bool bold = false, bool italic = false)
        {
            if (string.IsNullOrEmpty(fontFamilyName)) throw new ArgumentNullException(nameof(fontFamilyName));

            _fontFactory = factory ?? throw new ArgumentNullException(nameof(factory));
            Font = new TextFormat(factory, fontFamilyName, bold ? FontWeight.Bold : FontWeight.Normal, italic ? FontStyle.Italic : FontStyle.Normal, size);
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="D2DFont"/> class.
        /// </summary>
        ~D2DFont()
        {
            Font.Dispose();
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="D2DFont"/> is bold.
        /// </summary>
        /// <value><c>true</c> if bold; otherwise, <c>false</c>.</value>
        public bool Bold
        {
            get
            {
                return Font.FontWeight == FontWeight.Bold;
            }
            set
            {
                string familyName = FontFamilyName;
                float size = FontSize;
                FontStyle style = Italic ? FontStyle.Italic : FontStyle.Normal;
                bool wordWrapping = WordWrapping;

                Font.Dispose();

                Font = new TextFormat(_fontFactory, familyName, value ? FontWeight.Bold : FontWeight.Normal, style, size);
            }
        }

        /// <summary>
        /// Gets or sets the name of the font family.
        /// </summary>
        /// <value>The name of the font family.</value>
        public string FontFamilyName
        {
            get
            {
                return Font.FontFamilyName;
            }
            set
            {
                float size = FontSize;
                bool bold = Bold;
                FontStyle style = Italic ? FontStyle.Italic : FontStyle.Normal;
                bool wordWrapping = WordWrapping;

                Font.Dispose();

                Font = new TextFormat(_fontFactory, value, bold ? FontWeight.Bold : FontWeight.Normal, style, size);
            }
        }

        /// <summary>
        /// Gets or sets the size of the font.
        /// </summary>
        /// <value>The size of the font.</value>
        public float FontSize
        {
            get
            {
                return Font.FontSize;
            }
            set
            {
                string familyName = FontFamilyName;
                bool bold = Bold;
                FontStyle style = Italic ? FontStyle.Italic : FontStyle.Normal;
                bool wordWrapping = WordWrapping;

                Font.Dispose();

                Font = new TextFormat(_fontFactory, familyName, bold ? FontWeight.Bold : FontWeight.Normal, style, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="D2DFont"/> is italic.
        /// </summary>
        /// <value><c>true</c> if italic; otherwise, <c>false</c>.</value>
        public bool Italic
        {
            get
            {
                return Font.FontStyle == FontStyle.Italic;
            }
            set
            {
                string familyName = FontFamilyName;
                float size = FontSize;
                bool bold = Bold;
                bool wordWrapping = WordWrapping;

                Font.Dispose();

                Font = new TextFormat(_fontFactory, familyName, bold ? FontWeight.Bold : FontWeight.Normal, value ? FontStyle.Italic : FontStyle.Normal, size);
            }
        }

        /// <summary>
        /// Gets a value indicating whether [word wrapping].
        /// </summary>
        /// <value><c>true</c> if [word wrapping]; otherwise, <c>false</c>.</value>
        public bool WordWrapping
        {
            get
            {
                return Font.WordWrapping != SharpDX.DirectWrite.WordWrapping.NoWrap;
            }
            set
            {
                Font.SetWordWrapping(value ? SharpDX.DirectWrite.WordWrapping.Wrap : SharpDX.DirectWrite.WordWrapping.NoWrap);
            }
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="D2DFont"/> to <see cref="TextFormat"/>.
        /// </summary>
        /// <param name="font">The font.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator TextFormat(D2DFont font)
        {
            return font.Font;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return "{FontFamilyName=" + Font.FontFamilyName + ", Size=" + Font.FontSize + "}";
        }
    }
}