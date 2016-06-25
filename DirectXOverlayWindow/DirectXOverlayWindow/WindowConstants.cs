using System;

namespace DirectXOverlayWindow
{
    internal class WindowConstants
    {
        public const int SM_CX_SCREEN = 0;
        public const int SM_CY_SCREEN = 1;

        public const string DESKTOP_CLASS = "#32769";

        public const uint WINDOW_STYLE_DX = 0x8000000     //WS_DISABLED
                                            | 0x10000000    //WS_VISIBLE
                                            | 0x80000000;   //WS_POPUP

        public const uint WINDOW_EX_STYLE_DX = 0x8000000     //WS_EX_NOACTIVATE
                                            | 0x80000       //WS_EX_LAYERED
                                            | 0x80          //WS_EX_TOOLWINDOW -> Not in taskbar
                                            | 0x8            //WS_EX_TOPMOST
                                            | 0x20;         //WS_EX_TRANSPARENT

        public const int HWND_NOTOPMOST = -2;
        public const int HWND_TOPMOST = -1;
        public const int HWND_TOP = 0;
        public const int HWND_BOTTOM = 1;

        public const uint SWP_FLAGS_SHOW = 0x40;
        public const uint SWP_FLAGS_HIDE = 0x80;

        public const uint SW_SHOW = 5;
        public const uint SW_HIDE = 0;
    }
}
