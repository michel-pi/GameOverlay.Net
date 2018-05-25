using System;

using SharpDX.DirectWrite;

namespace Yato.DirectXOverlay.Renderer
{
    public class FontCreationOptions
    {
        public bool Bold;
        public string FontFamilyName;

        public float FontSize;
        public bool Italic;

        public bool WordWrapping;

        public FontStyle GetStyle()
        {
            if (Italic) return FontStyle.Italic;
            return FontStyle.Normal;
        }
    }
}