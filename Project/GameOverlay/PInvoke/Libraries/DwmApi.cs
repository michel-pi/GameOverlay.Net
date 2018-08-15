using System;
using GameOverlay.PInvoke.Types;

namespace GameOverlay.PInvoke.Libraries
{
    internal static class DwmApi
    {
        public delegate void DwmExtendFrameIntoClientAreaDelegate(IntPtr hWnd, ref NativeMargin pMargins);

        public static DwmExtendFrameIntoClientAreaDelegate DwmExtendFrameIntoClientArea =
            WinApi.GetMethod<DwmExtendFrameIntoClientAreaDelegate>("dwmapi.dll", "DwmExtendFrameIntoClientArea");
    }
}