using System;

namespace Yato.DirectXOverlay.Windows
{
    public struct OverlayCreationOptions // struct -> not nullable
    {
        public bool BypassTopmost;
        public int Height;
        public int Width;
        public string WindowTitle;
        public int X;
        public int Y;
    }
}