using System;
using System.Runtime.InteropServices;

namespace Yato.DirectXOverlay.PInvoke
{
    [StructLayout(LayoutKind.Sequential)]
    public struct MARGIN
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