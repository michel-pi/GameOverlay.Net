using System;
using System.Runtime.InteropServices;

namespace GameOverlay.PInvoke
{
    internal static class User32
    {
        public static readonly IntPtr HwndBroadcast = (IntPtr)0xffff;
        public static readonly IntPtr HwndInsertNoTopmost = (IntPtr)(-2);
        public static readonly IntPtr HwndInsertTopMost = (IntPtr)(-1);
        public static readonly IntPtr HwndInsertTop = IntPtr.Zero;
        public static readonly IntPtr HwndInsertBottom = (IntPtr)1;

        private delegate IntPtr SetThreadDpiAwarenessContextDelegate(ref DpiAwareness awareness); // returns a handle to a DpiAwarenessContext
        private static readonly SetThreadDpiAwarenessContextDelegate _setThreadDpiAwarenessContext;

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        public delegate IntPtr CreateWindowExDelegate(
            ExtendedWindowStyle dwExStyle,
            string lpClassName,
            string lpWindowName,
            WindowStyle dwStyle,
            int x,
            int y,
            int nWidth,
            int nHeight,
            IntPtr hWndParent,
            IntPtr hMenu,
            IntPtr hInstance,
            IntPtr lpParam);

        public static readonly CreateWindowExDelegate CreateWindowEx;

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        public delegate IntPtr DefWindowProcDelegate(IntPtr hwnd, WindowMessage msg, IntPtr wparam, IntPtr lparam);

        public static readonly DefWindowProcDelegate DefWindowProc;

        public delegate int DestroyWindowDelegate(IntPtr hwnd);

        public static readonly DestroyWindowDelegate DestroyWindow;

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public delegate bool DispatchMessageDelegate(ref Message msg);

        public static readonly DispatchMessageDelegate DispatchMessage;

        [return: MarshalAs(UnmanagedType.Bool)]
        public delegate bool GetClientRectDelegate(IntPtr hwnd, out NativeRect lpNativeRect);

        public static readonly GetClientRectDelegate GetClientRect;

        public delegate IntPtr GetForegroundWindowDelegate();

        public static readonly GetForegroundWindowDelegate GetForegroundWindow;

        public delegate IntPtr GetWindowDelegate(IntPtr hwnd, WindowCommand cmd);
        public static readonly GetWindowDelegate GetWindow;

        [return: MarshalAs(UnmanagedType.Bool)]
        public delegate bool GetWindowRectDelegate(IntPtr hwnd, out NativeRect lpNativeRect);

        public static readonly GetWindowRectDelegate GetWindowRect;

        [return: MarshalAs(UnmanagedType.Bool)]
        public delegate bool IsWindowDelegate(IntPtr hwnd);

        public static readonly IsWindowDelegate IsWindow;

        [return: MarshalAs(UnmanagedType.Bool)]
        public delegate bool IsWindowVisibleDelegate(IntPtr hwnd);

        public static readonly IsWindowVisibleDelegate IsWindowVisible;

        [return: MarshalAs(UnmanagedType.Bool)]
        public delegate bool MoveWindowDelegate(IntPtr hwnd, int x, int y, int width, int height, [MarshalAs(UnmanagedType.Bool)] bool repaint);

        public static readonly MoveWindowDelegate MoveWindow;

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public delegate bool PeekMessageWDelegate(ref Message msg, IntPtr hwnd, uint filterMin, uint filterMax, uint removeMsg);

        public static readonly PeekMessageWDelegate PeekMessage;

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        public delegate ushort RegisterClassExDelegate(ref WindowClassEx windowClassEx);

        public static readonly RegisterClassExDelegate RegisterClassEx;

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public delegate bool SendMessageDelegate(IntPtr hwnd, WindowMessage msg, IntPtr wparam, IntPtr lparam);

        public static readonly SendMessageDelegate SendMessage;

        [return: MarshalAs(UnmanagedType.Bool)]
        public delegate bool SetLayeredWindowAttributesDelegate(IntPtr hwnd, uint crKey, byte bAlpha, LayeredWindowAttributes dwFlags);

        public static readonly SetLayeredWindowAttributesDelegate SetLayeredWindowAttributes;

        [return: MarshalAs(UnmanagedType.Bool)]
        public delegate bool SetWindowPosDelegate(IntPtr hwnd, IntPtr hwndInsertAfter, int x, int y, int cx, int cy, SwpFlags flags);

        public static readonly SetWindowPosDelegate SetWindowPos;

        [return: MarshalAs(UnmanagedType.Bool)]
        public delegate bool ShowWindowDelegate(IntPtr hWnd, ShowWindowCommand nCmdShow);

        public static readonly ShowWindowDelegate ShowWindow;

        [return: MarshalAs(UnmanagedType.Bool)]
        public delegate bool TranslateMessageDelegate(ref Message msg);

        public static readonly TranslateMessageDelegate TranslateMessage;

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public delegate bool UnregisterClassDelegate(string lpClassName, IntPtr hInstance);

        public static readonly UnregisterClassDelegate UnregisterClass;

        [return: MarshalAs(UnmanagedType.Bool)]
        public delegate bool UpdateWindowDelegate(IntPtr hWnd);

        public static readonly UpdateWindowDelegate UpdateWindow;

        [return: MarshalAs(UnmanagedType.Bool)]
        public delegate bool WaitMessageDelegate();

        public static readonly WaitMessageDelegate WaitMessage;

        [return: MarshalAs(UnmanagedType.Bool)]
        public delegate bool PostMessageWDelegate(IntPtr hwnd, WindowMessage message, IntPtr wparam, IntPtr lparam);

        public static readonly PostMessageWDelegate PostMessage;
        
        public delegate void PostQuitMessageDelegate(int nExitCode);

        public static readonly PostQuitMessageDelegate PostQuitMessage;

        public delegate IntPtr GenericGetWindowDelegate();

        public static readonly GenericGetWindowDelegate GetDesktopWindow;

        public static readonly GenericGetWindowDelegate GetShellWindow;

        public static readonly GenericGetWindowDelegate GetActiveWindow;

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        public delegate IntPtr FindWindowExWDelegate(IntPtr parent, IntPtr childAfter, string className, string windowName);

        public static readonly FindWindowExWDelegate FindWindowEx;

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        public delegate IntPtr GenericGetWindowLongDelegate(IntPtr handle, int index);

        public static readonly GenericGetWindowLongDelegate GetClassLongPtr;
        public static readonly GenericGetWindowLongDelegate GetWindowLongPtr;

        [return: MarshalAs(UnmanagedType.Bool)]
        public delegate bool GetWindowInfoDelegate(IntPtr handle, ref WindowInfo windowInfo);

        public static readonly GetWindowInfoDelegate GetWindowInfo;

        public delegate uint GetWindowThreadProcessIdDelegate(IntPtr handle, [In, Out] ref uint processId);
        
        public static readonly GetWindowThreadProcessIdDelegate GetWindowThreadProcessId;

        static User32()
        {
            IntPtr library = DynamicImport.ImportLibrary("user32.dll");

            CreateWindowEx = DynamicImport.Import<CreateWindowExDelegate>(library, "CreateWindowExW");
            DefWindowProc = DynamicImport.Import<DefWindowProcDelegate>(library, "DefWindowProcW");
            DestroyWindow = DynamicImport.Import<DestroyWindowDelegate>(library, "DestroyWindow");
            DispatchMessage = DynamicImport.Import<DispatchMessageDelegate>(library, "DispatchMessageW");
            GetClientRect = DynamicImport.Import<GetClientRectDelegate>(library, "GetClientRect");
            GetWindow = DynamicImport.Import<GetWindowDelegate>(library, "GetWindow");
            GetWindowRect = DynamicImport.Import<GetWindowRectDelegate>(library, "GetWindowRect");
            IsWindow = DynamicImport.Import<IsWindowDelegate>(library, "IsWindow");
            IsWindowVisible = DynamicImport.Import<IsWindowVisibleDelegate>(library, "IsWindowVisible");
            MoveWindow = DynamicImport.Import<MoveWindowDelegate>(library, "MoveWindow");
            PeekMessage = DynamicImport.Import<PeekMessageWDelegate>(library, "PeekMessageW");
            RegisterClassEx = DynamicImport.Import<RegisterClassExDelegate>(library, "RegisterClassExW");
            SendMessage = DynamicImport.Import<SendMessageDelegate>(library, "SendMessageW");
            SetLayeredWindowAttributes = DynamicImport.Import<SetLayeredWindowAttributesDelegate>(library, "SetLayeredWindowAttributes");
            SetWindowPos = DynamicImport.Import<SetWindowPosDelegate>(library, "SetWindowPos");
            ShowWindow = DynamicImport.Import<ShowWindowDelegate>(library, "ShowWindow");
            TranslateMessage = DynamicImport.Import<TranslateMessageDelegate>(library, "TranslateMessage");
            UnregisterClass = DynamicImport.Import<UnregisterClassDelegate>(library, "UnregisterClassW");
            UpdateWindow = DynamicImport.Import<UpdateWindowDelegate>(library, "UpdateWindow");
            WaitMessage = DynamicImport.Import<WaitMessageDelegate>(library, "WaitMessage");
            PostMessage = DynamicImport.Import<PostMessageWDelegate>(library, "PostMessageW");
            PostQuitMessage = DynamicImport.Import<PostQuitMessageDelegate>(library, "PostQuitMessage");

            GetForegroundWindow = DynamicImport.Import<GetForegroundWindowDelegate>(library, "GetForegroundWindow");

            GetDesktopWindow = DynamicImport.Import<GenericGetWindowDelegate>(library, "GetDesktopWindow");
            GetShellWindow = DynamicImport.Import<GenericGetWindowDelegate>(library, "GetShellWindow");
            GetActiveWindow = DynamicImport.Import<GenericGetWindowDelegate>(library, "GetActiveWindow");
            FindWindowEx = DynamicImport.Import<FindWindowExWDelegate>(library, "FindWindowExW");
            GetWindowInfo = DynamicImport.Import<GetWindowInfoDelegate>(library, "GetWindowInfo");
            GetWindowThreadProcessId = DynamicImport.Import<GetWindowThreadProcessIdDelegate>(library, "GetWindowThreadProcessId");

            if (IntPtr.Size == 4)
            {
                GetClassLongPtr = DynamicImport.Import<GenericGetWindowLongDelegate>(library, "GetClassLongW");
                GetWindowLongPtr = DynamicImport.Import<GenericGetWindowLongDelegate>(library, "GetWindowLongW");
            }
            else
            {
                GetClassLongPtr = DynamicImport.Import<GenericGetWindowLongDelegate>(library, "GetClassLongPtrW");
                GetWindowLongPtr = DynamicImport.Import<GenericGetWindowLongDelegate>(library, "GetWindowLongPtrW");
            }

            try
            {
                _setThreadDpiAwarenessContext = DynamicImport.Import<SetThreadDpiAwarenessContextDelegate>(library, "SetThreadDpiAwarenessContext");
            }
            catch { } // ignored
        }

        public static void MakeThreadDpiAware()
        {
            if (_setThreadDpiAwarenessContext == null) return;

            var dpiAwareness = DpiAwareness.PerMonitorAware;

            // i dont know if this actually takes a DpiAwareness or DpiAwarenessContext
            if (_setThreadDpiAwarenessContext(ref dpiAwareness) == IntPtr.Zero)
            {
                dpiAwareness = (DpiAwareness)DpiAwarenessContext.PerMonitorAware;
                _setThreadDpiAwarenessContext(ref dpiAwareness);
            }
        }
    }
}
