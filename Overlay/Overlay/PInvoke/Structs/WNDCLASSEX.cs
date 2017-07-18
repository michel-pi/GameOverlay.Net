using System;
using System.Runtime.InteropServices;

namespace Overlay.PInvoke.Structs
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct WNDCLASSEX
    {
        public uint cbSize;
        public uint style;
        public IntPtr lpfnWndProc;
        public int cbClsExtra;
        public int cbWndExtra;
        public IntPtr hInstance;
        public IntPtr hIcon;
        public IntPtr hCursor;
        public IntPtr hbrBackground;
        public string lpszMenuName;
        public string lpszClassName;
        public IntPtr hIconSm;
            
        public static uint Size()
        {
            return (uint)Marshal.SizeOf(typeof(WNDCLASSEX));
        }
    }
}
