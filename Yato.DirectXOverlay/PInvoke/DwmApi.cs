using System;
using System.Runtime.InteropServices;

namespace Yato.DirectXOverlay.PInvoke
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

    internal static class DwmApi
    {
        public static DwmExtendFrameIntoClientArea_t DwmExtendFrameIntoClientArea = WinApi.GetMethod<DwmExtendFrameIntoClientArea_t>("dwmapi.dll", "DwmExtendFrameIntoClientArea");

        public delegate void DwmExtendFrameIntoClientArea_t(IntPtr hWnd, ref MARGIN pMargins);
    }
}