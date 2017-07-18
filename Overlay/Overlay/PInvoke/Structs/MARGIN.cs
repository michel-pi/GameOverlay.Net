using System.Runtime.InteropServices;

namespace Overlay.PInvoke.Structs
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct MARGIN
    {
        public int cxLeftWidth;
        public int cxRightWidth;
        public int cyTopHeight;
        public int cyBottomHeight;
    }
}
