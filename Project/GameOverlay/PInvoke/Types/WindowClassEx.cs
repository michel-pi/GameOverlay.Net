using System;
using System.Runtime.InteropServices;

namespace GameOverlay.PInvoke.Types
{
    /// <summary>
    ///     Stores information for window creation
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct WindowClassEx
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
            return (uint) Marshal.SizeOf(ObfuscatorNeedsThis<WindowClassEx>());
        }

        private static Type ObfuscatorNeedsThis<T>()
        {
            return typeof(T);
        }
    }
}