using System;

using Yato.DirectXOverlay.PInvoke;

namespace Yato.DirectXOverlay
{
    public static class HelperMethods
    {
        public static int GetRealWindowRect(IntPtr hwnd, out RECT rect)
        {
            RECT windowRect = new RECT();
            RECT clientRect = new RECT();

            int result = User32.GetWindowRect(hwnd, out windowRect);
            if (User32.GetClientRect(hwnd, out clientRect) == 0)
            {
                rect = windowRect;
                return result;
            }

            int windowWidth = windowRect.Right - windowRect.Left;
            int windowHeight = windowRect.Bottom - windowRect.Top;

            if (windowWidth == clientRect.Right && windowHeight == clientRect.Bottom)
            {
                rect = windowRect;
                return result;
            }

            int dif_x = windowWidth > clientRect.Right ? windowWidth - clientRect.Right : clientRect.Right - windowWidth;
            int dif_y = windowHeight > clientRect.Bottom ? windowHeight - clientRect.Bottom : clientRect.Bottom - windowHeight;

            dif_x /= 2;
            dif_y /= 2;

            windowRect.Left += dif_x;
            windowRect.Top += dif_y;

            windowRect.Right -= dif_x;
            windowRect.Bottom -= dif_y;

            rect = windowRect;
            return result;
        }
    }
}