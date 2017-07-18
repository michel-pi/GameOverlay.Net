using System.Runtime.InteropServices;

namespace Overlay.PInvoke.Structs
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct POINT
    {
        public int X;
        public int Y;
    }
}
