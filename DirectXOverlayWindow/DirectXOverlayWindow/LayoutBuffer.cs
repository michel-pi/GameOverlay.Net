using System;

using SharpDX.DirectWrite;

namespace DirectXOverlayWindow
{
    internal class LayoutBuffer
    {
        public string Text;
        public TextLayout TextLayout;

        public LayoutBuffer(string text, TextLayout layout)
        {
            this.Text = text;
            this.TextLayout = layout;
            this.TextLayout.TextAlignment = TextAlignment.Leading;
            this.TextLayout.WordWrapping = WordWrapping.NoWrap;
        }

        public void Dispose()
        {
            this.TextLayout.Dispose();
            this.Text = null;
        }
    }
}
