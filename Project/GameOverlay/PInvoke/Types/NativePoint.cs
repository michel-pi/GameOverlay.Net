using System.Runtime.InteropServices;

namespace GameOverlay.PInvoke.Types
{
    /// <summary>
    ///     X and Y desktop coordinates
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct NativePoint
    {
        public int X;
        public int Y;
    }
}