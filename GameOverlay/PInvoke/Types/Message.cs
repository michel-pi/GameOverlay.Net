using System;
using System.Runtime.InteropServices;

namespace GameOverlay.PInvoke
{
    /// <summary>
    /// Contains a WindowsMessage
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct Message
    {
        public IntPtr Hwnd;
        public WindowsMessage Msg;
        public IntPtr lParam;
        public IntPtr wParam;
        public uint Time;
        public int X;
        public int Y;
    }
}
