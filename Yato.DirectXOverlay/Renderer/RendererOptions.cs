using System;

namespace Yato.DirectXOverlay.Renderer
{
    public struct RendererOptions
    {
        public bool AntiAliasing;
        public IntPtr Hwnd;
        public bool MeasureFps;
        public bool VSync;
    }
}