using System.Runtime.InteropServices;

namespace GameOverlay.PInvoke.Types
{
    /// <summary>
    ///     <c>MARGIN</c> struct used with DesktopWindowManager
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