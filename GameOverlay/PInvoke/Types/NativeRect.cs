using System.Runtime.InteropServices;

namespace GameOverlay.PInvoke.Types
{
    /// <summary>
    ///     Contains Desktop Coordinates
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct NativeRect
    {
        /// <summary>
        ///     The left
        /// </summary>
        public int Left; // x position of upper-left corner

        /// <summary>
        ///     The top
        /// </summary>
        public int Top; // y position of upper-left corner

        /// <summary>
        ///     The right
        /// </summary>
        public int Right; // x position of lower-right corner

        /// <summary>
        ///     The bottom
        /// </summary>
        public int Bottom; // y position of lower-right corner
    }
}