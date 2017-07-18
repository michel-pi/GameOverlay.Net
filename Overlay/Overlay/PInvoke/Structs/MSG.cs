using System;
using System.Runtime.InteropServices;

namespace Overlay.PInvoke.Structs
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct MSG
    {
        public IntPtr hwnd;
        public uint message;
        public IntPtr wparam;
        public IntPtr lparam;
        public uint time;
        public int x;
        public int y;
    }
}
