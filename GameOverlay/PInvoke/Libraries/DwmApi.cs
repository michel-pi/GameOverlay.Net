using System;

namespace GameOverlay.PInvoke
{
    internal static class DwmApi
    {
        public static DwmExtendFrameIntoClientArea_t DwmExtendFrameIntoClientArea = WinApi.GetMethod<DwmExtendFrameIntoClientArea_t>("dwmapi.dll", "DwmExtendFrameIntoClientArea");

        public delegate void DwmExtendFrameIntoClientArea_t(IntPtr hWnd, ref MARGIN pMargins);
    }
}