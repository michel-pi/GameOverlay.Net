using System;
using System.Runtime.InteropServices;

namespace GameOverlay.PInvoke
{
    /// <summary>
    ///     <c>MARGIN</c> struct used with DwmApi
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct NativeMargin
    {
        public int cxLeftWidth;
        public int cxRightWidth;
        public int cyBottomHeight;
        public int cyTopHeight;
    }
}
