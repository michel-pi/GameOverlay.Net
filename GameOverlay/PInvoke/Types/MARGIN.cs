using System;
using System.Runtime.InteropServices;

namespace GameOverlay.PInvoke
{
    /// <summary>
    /// <c>MARGIN</c> struct used with DesktopWindowManager
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct MARGIN
    {
        public int cxLeftWidth;
        public int cxRightWidth;
        public int cyBottomHeight;
        public int cyTopHeight;
    }
}
