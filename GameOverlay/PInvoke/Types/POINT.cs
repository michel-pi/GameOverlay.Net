using System.Runtime.InteropServices;

namespace GameOverlay.PInvoke.Types
{
    /// <summary>
    ///     X and Y desktop coordinates
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct Point
    {
        public int X;
        public int Y;
    }
}