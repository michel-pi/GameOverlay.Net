using System;

namespace GameOverlay.Renderer
{
    /// <summary>
    /// </summary>
    public struct RendererOptions
    {
        /// <summary>
        /// The anti aliasing
        /// </summary>
        public bool AntiAliasing;

        /// <summary>
        /// The HWND
        /// </summary>
        public IntPtr Hwnd;

        /// <summary>
        /// The measure FPS
        /// </summary>
        public bool MeasureFps;

        /// <summary>
        /// The v synchronize
        /// </summary>
        public bool VSync;
    }
}