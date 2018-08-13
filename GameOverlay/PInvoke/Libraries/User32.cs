using System;
using System.Runtime.InteropServices;
using GameOverlay.PInvoke.Types;

namespace GameOverlay.PInvoke.Libraries
{
    internal static class User32
    {
        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        public delegate IntPtr CreateWindowExDelegate(
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

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        public delegate IntPtr DefWindowProcDelegate(IntPtr hwnd, WindowsMessage msg, IntPtr wparam, IntPtr lparam);

        public delegate int DestroyWindowDelegate(IntPtr hwnd);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        public delegate int DispatchMessageDelegate(ref Message msg);

        public delegate int GetClientRectDelegate(IntPtr hwnd, out NativeRect lpNativeRect);

        public delegate IntPtr GetWindowDelegate(IntPtr hwnd, uint cmd);

        public delegate int GetWindowRectDelegate(IntPtr hwnd, out NativeRect lpNativeRect);

        public delegate int IsProcessDPIAwareDelegate();

        public delegate int IsWindowDelegate(IntPtr hwnd);

        public delegate int IsWindowVisibleDelegate(IntPtr hwnd);

        public delegate int MoveWindowDelegate(IntPtr hwnd, int x, int y, int width, int height, int repaint);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        public delegate int PeekMessageWDelegate(ref Message msg, IntPtr hwnd, uint filterMin, uint filterMax,
            uint removeMsg);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        public delegate ushort RegisterClassExDelegate(ref WindowClassEx windowClassEx);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        public delegate int SendMessageDelegate(IntPtr hwnd, WindowsMessage msg, IntPtr wparam, IntPtr lparam);

        public delegate bool SetLayeredWindowAttributesDelegate(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

        public delegate IntPtr SetThreadDpiAwarenessContextDelegate(ref int dpiContext);

        public delegate int SetWindowPosDelegate(IntPtr hwnd, IntPtr hwndInsertAfter, int x, int y, int cx, int cy,
            uint flags);

        public delegate int ShowWindowDelegate(IntPtr hWnd, uint nCmdShow);

        public delegate int TranslateMessageDelegate(ref Message msg);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        public delegate int UnregisterClassDelegate(string lpClassName, IntPtr hInstance);

        public delegate bool UpdateWindowDelegate(IntPtr hWnd);

        public delegate int WaitMessageDelegate();

        public static CreateWindowExDelegate CreateWindowEx =
            WinApi.GetMethod<CreateWindowExDelegate>("user32.dll", "CreateWindowExW");

        public static DefWindowProcDelegate DefWindowProc = WinApi.GetMethod<DefWindowProcDelegate>("user32.dll", "DefWindowProcW");

        public static DestroyWindowDelegate DestroyWindow = WinApi.GetMethod<DestroyWindowDelegate>("user32.dll", "DestroyWindow");

        public static DispatchMessageDelegate DispatchMessage =
            WinApi.GetMethod<DispatchMessageDelegate>("user32.dll", "DispatchMessageW");

        public static GetClientRectDelegate GetClientRect = WinApi.GetMethod<GetClientRectDelegate>("user32.dll", "GetClientRect");

        public static GetWindowDelegate GetWindow = WinApi.GetMethod<GetWindowDelegate>("user32.dll", "GetWindow");

        public static GetWindowRectDelegate GetWindowRect = WinApi.GetMethod<GetWindowRectDelegate>("user32.dll", "GetWindowRect");

        public static IsProcessDPIAwareDelegate IsProcessDpiAware =
            WinApi.GetMethod<IsProcessDPIAwareDelegate>("user32.dll", "IsProcessDPIAware");

        public static IsWindowDelegate IsWindow = WinApi.GetMethod<IsWindowDelegate>("user32.dll", "IsWindow");

        public static IsWindowVisibleDelegate IsWindowVisible =
            WinApi.GetMethod<IsWindowVisibleDelegate>("user32.dll", "IsWindowVisible");

        public static MoveWindowDelegate MoveWindow = WinApi.GetMethod<MoveWindowDelegate>("user32.dll", "MoveWindow");

        public static PeekMessageWDelegate PeekMessageW = WinApi.GetMethod<PeekMessageWDelegate>("user32.dll", "PeekMessageW");

        public static RegisterClassExDelegate RegisterClassEx =
            WinApi.GetMethod<RegisterClassExDelegate>("user32.dll", "RegisterClassExW");

        public static SendMessageDelegate SendMessage = WinApi.GetMethod<SendMessageDelegate>("user32.dll", "SendMessageW");

        public static SetLayeredWindowAttributesDelegate SetLayeredWindowAttributes =
            WinApi.GetMethod<SetLayeredWindowAttributesDelegate>("user32.dll", "SetLayeredWindowAttributes");

        public static SetWindowPosDelegate SetWindowPos = WinApi.GetMethod<SetWindowPosDelegate>("user32.dll", "SetWindowPos");

        public static ShowWindowDelegate ShowWindow = WinApi.GetMethod<ShowWindowDelegate>("user32.dll", "ShowWindow");

        public static TranslateMessageDelegate TranslateMessage =
            WinApi.GetMethod<TranslateMessageDelegate>("user32.dll", "TranslateMessage");

        public static UnregisterClassDelegate UnregisterClass =
            WinApi.GetMethod<UnregisterClassDelegate>("user32.dll", "UnregisterClassW");

        public static UpdateWindowDelegate UpdateWindow = WinApi.GetMethod<UpdateWindowDelegate>("user32.dll", "UpdateWindow");

        public static WaitMessageDelegate WaitMessage = WinApi.GetMethod<WaitMessageDelegate>("user32.dll", "WaitMessage");

        public static void SetThreadDpiAware()
        {
            try
            {
                var procAddress = WinApi.GetProcAddress("user32.dll", "SetThreadDpiAwarenessContext");

                if (procAddress == IntPtr.Zero) return;

                var method =
                    WinApi.GetMethod<SetThreadDpiAwarenessContextDelegate>("user32.dll", "SetThreadDpiAwarenessContext");

                int dpiContext = -3;

                method(ref dpiContext);
            }
            catch
            {
                // ignored
            }
        }
    }
}