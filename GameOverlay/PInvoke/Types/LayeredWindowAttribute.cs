using System;

namespace GameOverlay.PInvoke
{
    /// <summary>
    /// 
    /// </summary>
    public enum LayeredWindowAttribute : uint
    {
        /// <summary>
        /// The none
        /// </summary>
        None = 0x0,
        /// <summary>
        /// The color key
        /// </summary>
        ColorKey = 0x1,
        /// <summary>
        /// The alpha
        /// </summary>
        Alpha = 0x2,
        /// <summary>
        /// The opaque
        /// </summary>
        Opaque = 0x4
    }
}
