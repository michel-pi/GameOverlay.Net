using System;
using System.Security;
using System.Runtime.InteropServices;

using Overlay.PInvoke.Structs;

namespace Overlay.PInvoke.Libraries
{
    [SuppressUnmanagedCodeSecurity()]
    internal static class DwmApi
    {
        [DllImport("dwmapi.dll", SetLastError = false)]
        public static extern void DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGIN pMargins);
    }
}
