using System;
using System.Runtime.InteropServices;

namespace GameOverlay.Windows
{
    /// <summary>
    /// Represents the boundaries of a window.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct WindowBounds
    {
        /// <summary>
        /// The position on the x-axis of the upper-left corner of a window.
        /// </summary>
        public int Left;
        /// <summary>
        /// The position on the y-axis of the upper-left corner of a window.
        /// </summary>
        public int Top;
        /// <summary>
        /// The position on the x-axis of the lower-right corner of a window.
        /// </summary>
        public int Right;
        /// <summary>
        /// The position on the y-axis of the lower-right corner of a window.
        /// </summary>
        public int Bottom;
    }
}
