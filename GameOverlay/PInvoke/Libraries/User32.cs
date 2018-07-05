using System;
using System.Runtime.InteropServices;

namespace GameOverlay.PInvoke
{
    internal static class User32
    {
        public static CreateWindowEx_t CreateWindowEx = WinApi.GetMethod<CreateWindowEx_t>("user32.dll", "CreateWindowExW");

        public static DefWindowProc_t DefWindowProc = WinApi.GetMethod<DefWindowProc_t>("user32.dll", "DefWindowProcW");

        public static DestroyWindow_t DestroyWindow = WinApi.GetMethod<DestroyWindow_t>("user32.dll", "DestroyWindow");

        public static DispatchMessage_t DispatchMessage = WinApi.GetMethod<DispatchMessage_t>("user32.dll", "DispatchMessageW");

        public static GetClientRect_t GetClientRect = WinApi.GetMethod<GetClientRect_t>("user32.dll", "GetClientRect");

        public static GetWindow_t GetWindow = WinApi.GetMethod<GetWindow_t>("user32.dll", "GetWindow");

        public static GetWindowRect_t GetWindowRect = WinApi.GetMethod<GetWindowRect_t>("user32.dll", "GetWindowRect");

        public static IsProcessDPIAware_t IsProcessDPIAware = WinApi.GetMethod<IsProcessDPIAware_t>("user32.dll", "IsProcessDPIAware");

        public static IsWindow_t IsWindow = WinApi.GetMethod<IsWindow_t>("user32.dll", "IsWindow");

        public static IsWindowVisible_t IsWindowVisible = WinApi.GetMethod<IsWindowVisible_t>("user32.dll", "IsWindowVisible");

        public static MoveWindow_t MoveWindow = WinApi.GetMethod<MoveWindow_t>("user32.dll", "MoveWindow");

        public static PeekMessageW_t PeekMessageW = WinApi.GetMethod<PeekMessageW_t>("user32.dll", "PeekMessageW");

        public static RegisterClassEx_t RegisterClassEx = WinApi.GetMethod<RegisterClassEx_t>("user32.dll", "RegisterClassExW");

        public static SendMessage_t SendMessage = WinApi.GetMethod<SendMessage_t>("user32.dll", "SendMessageW");

        public static SetLayeredWindowAttributes_t SetLayeredWindowAttributes = WinApi.GetMethod<SetLayeredWindowAttributes_t>("user32.dll", "SetLayeredWindowAttributes");

        public static SetWindowPos_t SetWindowPos = WinApi.GetMethod<SetWindowPos_t>("user32.dll", "SetWindowPos");

        public static ShowWindow_t ShowWindow = WinApi.GetMethod<ShowWindow_t>("user32.dll", "ShowWindow");

        public static TranslateMessage_t TranslateMessage = WinApi.GetMethod<TranslateMessage_t>("user32.dll", "TranslateMessage");

        public static UnregisterClass_t UnregisterClass = WinApi.GetMethod<UnregisterClass_t>("user32.dll", "UnregisterClassW");

        public static UpdateWindow_t UpdateWindow = WinApi.GetMethod<UpdateWindow_t>("user32.dll", "UpdateWindow");

        public static WaitMessage_t WaitMessage = WinApi.GetMethod<WaitMessage_t>("user32.dll", "WaitMessage");

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        public delegate IntPtr CreateWindowEx_t(
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
        public delegate IntPtr DefWindowProc_t(IntPtr hwnd, WindowsMessage msg, IntPtr wparam, IntPtr lparam);

        public delegate int DestroyWindow_t(IntPtr hwnd);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        public delegate int DispatchMessage_t(ref Message msg);

        public delegate int GetClientRect_t(IntPtr hwnd, out RECT lpRect);

        public delegate IntPtr GetWindow_t(IntPtr hwnd, uint cmd);

        public delegate int GetWindowRect_t(IntPtr hwnd, out RECT lpRect);

        public delegate int IsProcessDPIAware_t();

        public delegate int IsWindow_t(IntPtr hwnd);

        public delegate int IsWindowVisible_t(IntPtr hwnd);

        public delegate int MoveWindow_t(IntPtr hwnd, int x, int y, int width, int height, int repaint);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        public delegate int PeekMessageW_t(ref Message msg, IntPtr hwnd, uint filterMin, uint filterMax, uint removeMsg);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        public delegate ushort RegisterClassEx_t(ref WNDCLASSEX wndclassex);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        public delegate int SendMessage_t(IntPtr hwnd, WindowsMessage msg, IntPtr wparam, IntPtr lparam);

        public delegate bool SetLayeredWindowAttributes_t(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

        public delegate int SetWindowPos_t(IntPtr hwnd, IntPtr hwndInsertAfter, int x, int y, int cx, int cy, uint flags);

        public delegate int ShowWindow_t(IntPtr hWnd, uint nCmdShow);

        public delegate int TranslateMessage_t(ref Message msg);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        public delegate int UnregisterClass_t(string lpClassName, IntPtr hInstance);

        public delegate bool UpdateWindow_t(IntPtr hWnd);

        public delegate int WaitMessage_t();

        public delegate IntPtr SetThreadDpiAwarenessContext_t(ref int dpiContext);

        public static void SetThreadDpiAware()
        {
            IntPtr procAddress = WinApi.GetProcAddress("user32.dll", "SetThreadDpiAwarenessContext");

            if (procAddress == IntPtr.Zero) return;

            var method = WinApi.GetMethod<SetThreadDpiAwarenessContext_t>("user32.dll", "SetThreadDpiAwarenessContext");

            int dpiContext = -3;

            method(ref dpiContext);
        }
    }
}