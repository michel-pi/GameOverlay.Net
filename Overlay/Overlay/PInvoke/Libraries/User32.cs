using System;
using System.Security;
using System.Runtime.InteropServices;

using Overlay.PInvoke.Structs;

namespace Overlay.PInvoke.Libraries
{
    [SuppressUnmanagedCodeSecurity]
    internal static class User32
    {
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr CreateWindowEx(
           uint dwExStyle,
           string lpClassName,
           string lpWindowName,
           uint dwStyle,
           int x,
           int y,
           int nWidth,
           int nHeight,
           IntPtr hWndParent,
           IntPtr hMenu,
           IntPtr hInstance,
           IntPtr lpParam);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern ushort RegisterClassEx(ref WNDCLASSEX wndclassex);

        [DllImport("user32.dll", SetLastError = false)]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", SetLastError = false)]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", SetLastError = false)]
        public static extern int DestroyWindow(IntPtr hwnd);

        [DllImport("user32.dll", SetLastError = false)]
        public static extern int SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll", SetLastError = false)]
        public static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll", SetLastError = false)]
        public static extern int GetSystemMetrics(int index);

        [DllImport("user32.dll", SetLastError = false)]
        public static extern int ShowWindow(IntPtr hWnd, uint nCmdShow);

        [DllImport("user32.dll", SetLastError = false)]
        public static extern int GetWindowRect(IntPtr hwnd, out RECT lpRect);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

        [DllImport("user32.dll", SetLastError = false)]
        public static extern bool UpdateLayeredWindow(IntPtr hwnd, IntPtr hdc, ref POINT point, ref POINT size, IntPtr hdcSrc, IntPtr pptSrc, int crKey, IntPtr blend, uint flags);

        [DllImport("user32.dll", SetLastError = false)]
        public static extern int TranslateMessage(ref MSG msg);

        [DllImport("user32.dll", SetLastError = false)]
        public static extern int PeekMessage(out MSG msg, IntPtr hwnd, uint filterMin, uint filterMax, uint removeMsg);

        [DllImport("user32.dll", SetLastError = false)]
        public static extern int DispatchMessage(ref MSG msg);

        [DllImport("user32.dll", SetLastError = false)]
        public static extern int MoveWindow(IntPtr hwnd, int x, int y, int width, int height, int repaint);

        [DllImport("user32.dll", SetLastError = false)]
        public static extern int DefWindowProc(IntPtr hwnd, uint msg, uint wparam, uint lparam);


        [DllImport("user32.dll", SetLastError = false)]
        public static extern int SendMessage(IntPtr hwnd, uint msg, uint wparam, uint lparam);

        [DllImport("user32.dll")]
        public static extern bool UpdateWindow(IntPtr hWnd);
    }
}
