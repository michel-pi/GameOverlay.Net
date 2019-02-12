using System;
using System.Runtime.InteropServices;

namespace GameOverlay.PInvoke
{
    /// <summary>
    /// An application-defined function that processes messages sent to a window.
    /// </summary>
    /// <param name="hWnd">A handle to the window. </param>
    /// <param name="msg">The message.</param>
    /// <param name="wParam">Additional message information. The contents of this parameter depend on the value of the uMsg parameter. </param>
    /// <param name="lParam">Additional message information. The contents of this parameter depend on the value of the uMsg parameter. </param>
    /// <returns>The return value is the result of the message processing and depends on the message sent.</returns>
    internal delegate IntPtr WindowProc(IntPtr hWnd, WindowMessage msg, IntPtr wParam, IntPtr lParam);

    /// <summary>
    /// Identifies the dots per inch (dpi) setting for a thread, process, or window.
    /// </summary>
    internal enum DpiAwareness
    {
        /// <summary>
        /// Invalid DPI awareness. This is an invalid DPI awareness value.
        /// </summary>
        Invalid,
        /// <summary>
        /// This process does not scale for DPI changes and is always assumed to have a scale factor of 100% (96 DPI). It will be automatically scaled by the system on any other DPI setting.
        /// </summary>
        Unaware,
        /// <summary>
        /// This process does not scale for DPI changes. It will query for the DPI once and use that value for the lifetime of the process. If the DPI changes, the process will not adjust to the new DPI value. It will be automatically scaled up or down by the system when the DPI changes from the system value.
        /// </summary>
        SystemAware,
        /// <summary>
        /// This process checks for the DPI when it is created and adjusts the scale factor whenever the DPI changes. These processes are not automatically scaled by the system.
        /// </summary>
        PerMonitorAware
    }

    /// <summary>
    /// Identifies the awareness context for a window.
    /// </summary>
    internal enum DpiAwarenessContext
    {
        /// <summary>
        /// This window does not scale for DPI changes and is always assumed to have a scale factor of 100% (96 DPI). It will be automatically scaled by the system on any other DPI setting.
        /// </summary>
        Unaware = -1,
        /// <summary>
        /// This window does not scale for DPI changes. It will query for the DPI once and use that value for the lifetime of the process. If the DPI changes, the process will not adjust to the new DPI value. It will be automatically scaled up or down by the system when the DPI changes from the system value.
        /// </summary>
        SystemAware = -2,
        /// <summary>
        /// This window checks for the DPI when it is created and adjusts the scale factor whenever the DPI changes. These processes are not automatically scaled by the system.
        /// </summary>
        PerMonitorAware = -3,
        /// <summary>
        /// An advancement over the original per-monitor DPI awareness mode, which enables applications to access new DPI-related scaling behaviors on a per top-level window basis. Per Monitor v2 was made available in the Creators Update of Windows 10, and is not available on earlier versions of the operating system.
        /// </summary>
        PerMonitorAwareV2 = -4,
        /// <summary>
        /// DPI unaware with improved quality of GDI-based content. This mode behaves similarly to DPI_AWARENESS_CONTEXT_UNAWARE, but also enables the system to automatically improve the rendering quality of text and other GDI-based primitives when the window is displayed on a high-DPI monitor.
        /// </summary>
        UnawareGdiScaled = -5
    }

    /// <summary>
    ///     Extended Window Styles
    /// </summary>
    [Flags]
    internal enum ExtendedWindowStyle : uint
    {
        /// <summary>
        ///     The window accepts drag-drop files.
        /// </summary>
        AcceptFiles = 0x00000010,

        /// <summary>
        ///     Forces a top-level window onto the taskbar when the window is visible.
        /// </summary>
        AppWindow = 0x00040000,

        /// <summary>
        ///     The window has a border with a sunken edge.
        /// </summary>
        ClientEdge = 0x00000200,

        /// <summary>
        ///     Paints all descendants of a window in bottom-to-top painting order using double-buffering. For more information,
        ///     see Remarks. This cannot be used if the window has a class style of either CS_OWNDC or CS_CLASSDC.
        /// </summary>
        Composited = 0x02000000,

        /// <summary>
        ///     The title bar of the window includes a question mark. When the user clicks the question mark, the cursor changes to
        ///     a question mark with a pointer. If the user then clicks a child window, the child receives a WM_HELP message. The
        ///     child window should pass the message to the parent window procedure, which should call the WinHelp function using
        ///     the HELP_WM_HELP command. The Help application displays a pop-up window that typically contains help for the child
        ///     window.
        /// </summary>
        ContextHelp = 0x00000400,

        /// <summary>
        ///     The window itself contains child windows that should take part in dialog box navigation. If this style is
        ///     specified, the dialog manager recurses into children of this window when performing navigation operations such as
        ///     handling the TAB key, an arrow key, or a keyboard mnemonic.
        /// </summary>
        ControlParent = 0x00010000,

        /// <summary>
        ///     The window has a double border; the window can, optionally, be created with a title bar by specifying the
        ///     WS_CAPTION style in the dwStyle parameter.
        /// </summary>
        DlgModalFrame = 0x00000001,

        /// <summary>
        ///     The window is a layered window. This style cannot be used if the window has a class style of either CS_OWNDC or
        ///     CS_CLASSDC.
        /// </summary>
        Layered = 0x00080000,

        /// <summary>
        ///     If the shell language is Hebrew, Arabic, or another language that supports reading order alignment, the horizontal
        ///     origin of the window is on the right edge. Increasing horizontal values advance to the left.
        /// </summary>
        LayoutRtl = 0x00400000,

        /// <summary>
        ///     The window has generic left-aligned properties. This is the default.
        /// </summary>
        Left = 0x00000000,

        /// <summary>
        ///     If the shell language is Hebrew, Arabic, or another language that supports reading order alignment, the vertical
        ///     scroll bar (if present) is to the left of the client area. For other languages, the style is ignored.
        /// </summary>
        LeftScrollbar = 0x00004000,

        /// <summary>
        ///     The window text is displayed using left-to-right reading-order properties. This is the default.
        /// </summary>
        LtrReading = 0x00000000,

        /// <summary>
        ///     The window is a MDI child window.
        /// </summary>
        MdiChild = 0x00000040,

        /// <summary>
        ///     A top-level window created with this style does not become the foreground window when the user clicks it. The
        ///     system does not bring this window to the foreground when the user minimizes or closes the foreground window.
        /// </summary>
        NoActivate = 0x08000000,

        /// <summary>
        ///     The window does not pass its window layout to its child windows.
        /// </summary>
        NoInheritLayout = 0x00100000,

        /// <summary>
        ///     The child window created with this style does not send the WM_PARENTNOTIFY message to its parent window when it is
        ///     created or destroyed.
        /// </summary>
        NoParentNotify = 0x00000004,

        /// <summary>
        ///     The window does not render to a redirection surface. This is for windows that do not have visible content or that
        ///     use mechanisms other than surfaces to provide their visual.
        /// </summary>
        NoRedirectionBitmap = 0x00200000,

        /// <summary>
        ///     The window is an overlapped window.
        /// </summary>
        OverlappedWindow = WindowEdge | ClientEdge,

        /// <summary>
        ///     The window is palette window, which is a modeless dialog box that presents an array of commands.
        /// </summary>
        PaletteWindow = WindowEdge | ToolWindow | Topmost,

        /// <summary>
        ///     The window has generic "right-aligned" properties. This depends on the window class. This style has an effect only
        ///     if the shell language is Hebrew, Arabic, or another language that supports reading-order alignment; otherwise, the
        ///     style is ignored.
        /// </summary>
        Right = 0x00001000,

        /// <summary>
        ///     The vertical scroll bar (if present) is to the right of the client area. This is the default.
        /// </summary>
        RightScrollbar = 0x00000000,

        /// <summary>
        ///     If the shell language is Hebrew, Arabic, or another language that supports reading-order alignment, the window text
        ///     is displayed using right-to-left reading-order properties. For other languages, the style is ignored.
        /// </summary>
        RtlReading = 0x00002000,

        /// <summary>
        ///     The window has a three-dimensional border style intended to be used for items that do not accept user input.
        /// </summary>
        StaticEdge = 0x00020000,

        /// <summary>
        ///     The window is intended to be used as a floating toolbar. A tool window has a title bar that is shorter than a
        ///     normal title bar, and the window title is drawn using a smaller font. A tool window does not appear in the taskbar
        ///     or in the dialog that appears when the user presses ALT+TAB. If a tool window has a system menu, its icon is not
        ///     displayed on the title bar. However, you can display the system menu by right-clicking or by typing ALT+SPACE.
        /// </summary>
        ToolWindow = 0x00000080,

        /// <summary>
        ///     The window should be placed above all non-topmost windows and should stay above them, even when the window is
        ///     deactivated. To add or remove this style, use the SetWindowPos function.
        /// </summary>
        Topmost = 0x00000008,

        /// <summary>
        ///     The window should not be painted until siblings beneath the window (that were created by the same thread) have been
        ///     painted. The window appears transparent because the bits of underlying sibling windows have already been painted.
        /// </summary>
        Transparent = 0x00000020,

        /// <summary>
        ///     The window has a border with a raised edge.
        /// </summary>
        WindowEdge = 0x00000100
    }

    /// <summary>
    /// Layered Window Attributes
    /// </summary>
    internal enum LayeredWindowAttributes : uint
    {
        /// <summary>
        ///     The none
        /// </summary>
        None = 0x0,

        /// <summary>
        ///     The color key
        /// </summary>
        ColorKey = 0x1,

        /// <summary>
        ///     The alpha
        /// </summary>
        Alpha = 0x2,

        /// <summary>
        ///     The opaque
        /// </summary>
        Opaque = 0x4
    }

    /// <summary>
    /// Controls how the window is to be shown.
    /// </summary>
    internal enum ShowWindowCommand : uint
    {
        /// <summary>
        /// Minimizes a window, even if the thread that owns the window is not responding. This flag should only be used when minimizing windows from a different thread. 
        /// </summary>
        ForceMinimize = 11,
        /// <summary>
        /// Hides the window and activates another window. 
        /// </summary>
        Hide = 0,
        /// <summary>
        /// Maximizes the specified window. 
        /// </summary>
        Maximize = 3,
        /// <summary>
        /// Minimizes the specified window and activates the next top-level window in the Z order. 
        /// </summary>
        Minimize = 6,
        /// <summary>
        /// Activates and displays the window. If the window is minimized or maximized, the system restores it to its original size and position. An application should specify this flag when restoring a minimized window. 
        /// </summary>
        Restore = 9,
        /// <summary>
        /// Activates the window and displays it in its current size and position. 
        /// </summary>
        Show = 5,
        /// <summary>
        /// Sets the show state based on the SW_ value specified in the STARTUPINFO structure passed to the CreateProcess function by the program that started the application. 
        /// </summary>
        ShowDefault = 10,
        /// <summary>
        /// Activates the window and displays it as a maximized window. 
        /// </summary>
        ShowMaximized = 3,
        /// <summary>
        /// Activates the window and displays it as a minimized window. 
        /// </summary>
        ShowMinimized = 2,
        /// <summary>
        /// Displays the window as a minimized window. This value is similar to SW_SHOWMINIMIZED, except the window is not activated. 
        /// </summary>
        ShowMinNoActivate = 7,
        /// <summary>
        /// Displays the window in its current size and position. This value is similar to SW_SHOW, except that the window is not activated. 
        /// </summary>
        ShowNA = 8,
        /// <summary>
        /// Displays a window in its most recent size and position. This value is similar to SW_SHOWNORMAL, except that the window is not activated. 
        /// </summary>
        ShowNoActivate = 4,
        /// <summary>
        /// Activates and displays a window. If the window is minimized or maximized, the system restores it to its original size and position. An application should specify this flag when displaying the window for the first time. 
        /// </summary>
        ShowNormal = 1
    }

    /// <summary>
    /// SetWindowPos flags
    /// </summary>
    [Flags]
    internal enum SwpFlags : uint
    {
        /// <summary>
        /// If the calling thread and the thread that owns the window are attached to different input queues, the system posts the request to the thread that owns the window. This prevents the calling thread from blocking its execution while other threads process the request. 
        /// </summary>
        AsyncWindowPos = 0x4000,
        /// <summary>
        /// Prevents generation of the WM_SYNCPAINT message. 
        /// </summary>
        DeferErase = 0x2000,
        /// <summary>
        /// Draws a frame (defined in the window's class description) around the window. 
        /// </summary>
        DrawFrame = 0x0020,
        /// <summary>
        /// Applies new frame styles set using the SetWindowLong function. Sends a WM_NCCALCSIZE message to the window, even if the window's size is not being changed. If this flag is not specified, WM_NCCALCSIZE is sent only when the window's size is being changed. 
        /// </summary>
        FrameChanged = 0x0020,
        /// <summary>
        /// Hides the window. 
        /// </summary>
        HideWindow = 0x0080,
        /// <summary>
        /// Does not activate the window. If this flag is not set, the window is activated and moved to the top of either the topmost or non-topmost group (depending on the setting of the hWndInsertAfter parameter). 
        /// </summary>
        NoActivate = 0x0010,
        /// <summary>
        /// Discards the entire contents of the client area. If this flag is not specified, the valid contents of the client area are saved and copied back into the client area after the window is sized or repositioned. 
        /// </summary>
        NoCopyBits = 0x0100,
        /// <summary>
        /// Retains the current position (ignores X and Y parameters). 
        /// </summary>
        NoMove = 0x0002,
        /// <summary>
        /// Does not change the owner window's position in the Z order. 
        /// </summary>
        NoOwnerZOrder = 0x0200,
        /// <summary>
        /// Does not redraw changes. If this flag is set, no repainting of any kind occurs. This applies to the client area, the nonclient area (including the title bar and scroll bars), and any part of the parent window uncovered as a result of the window being moved. When this flag is set, the application must explicitly invalidate or redraw any parts of the window and parent window that need redrawing. 
        /// </summary>
        NoRedraw = 0x0008,
        /// <summary>
        /// Same as the SWP_NOOWNERZORDER flag. 
        /// </summary>
        NoReposition = 0x0200,
        /// <summary>
        /// Prevents the window from receiving the WM_WINDOWPOSCHANGING message. 
        /// </summary>
        NoSendChanging = 0x0400,
        /// <summary>
        /// Retains the current size (ignores the cx and cy parameters). 
        /// </summary>
        NoSize = 0x0001,
        /// <summary>
        /// Retains the current Z order (ignores the hWndInsertAfter parameter). 
        /// </summary>
        NoZOrder = 0x0004,
        /// <summary>
        /// Displays the window. 
        /// </summary>
        ShowWindow = 0x0040,
    }

    /// <summary>
    ///     Window Styles
    /// </summary>
    [Flags]
    internal enum WindowStyle : uint
    {
        /// <summary>
        ///     The window has a thin-line border.
        /// </summary>
        Border = 0x00800000,

        /// <summary>
        ///     The window has a title bar (includes the Border style).
        /// </summary>
        Caption = 0x00C00000,

        /// <summary>
        ///     The window is a child window. A window with this style cannot have a menu bar. This style cannot be used with the
        ///     Popup style.
        /// </summary>
        Child = 0x40000000,

        /// <summary>
        ///     Same as the Child style.
        /// </summary>
        ChildWindow = 0x40000000,

        /// <summary>
        ///     Excludes the area occupied by child windows when drawing occurs within the parent window. This style is used when
        ///     creating the parent window.
        /// </summary>
        ClipChildren = 0x02000000,

        /// <summary>
        ///     Clips child windows relative to each other; that is, when a particular child window receives a WM_PAINT message,
        ///     the ClipSiblings style clips all other overlapping child windows out of the region of the child window to be
        ///     updated. If ClipSiblings is not specified and child windows overlap, it is possible, when drawing within the client
        ///     area of a child window, to draw within the client area of a neighboring child window.
        /// </summary>
        ClipSiblings = 0x04000000,

        /// <summary>
        ///     The window is initially disabled. A disabled window cannot receive input from the user. To change this after a
        ///     window has been created, use the EnableWindow function.
        /// </summary>
        Disabled = 0x08000000,

        /// <summary>
        ///     The window has a border of a style typically used with dialog boxes. A window with this style cannot have a title
        ///     bar.
        /// </summary>
        DlgFrame = 0x00400000,

        /// <summary>
        ///     The window is the first control of a group of controls. The group consists of this first control and all controls
        ///     defined after it, up to the next control with the Group style. The first control in each group usually has the
        ///     Tabstop style so that the user can move from group to group. The user can subsequently change the keyboard focus
        ///     from one control in the group to the next control in the group by using the direction keys.
        /// </summary>
        Group = 0x00020000,

        /// <summary>
        ///     The window has a horizontal scroll bar.
        /// </summary>
        HScroll = 0x00100000,

        /// <summary>
        ///     The window is initially minimized. Same as the Minimize style.
        /// </summary>
        Iconic = 0x20000000,

        /// <summary>
        ///     The window is initially maximized.
        /// </summary>
        Maximize = 0x01000000,

        /// <summary>
        ///     The window has a maximize button. Cannot be combined with the WS_EX_CONTEXTHELP style. The SysMenu style must also
        ///     be specified.
        /// </summary>
        MaximizeBox = 00010000,

        /// <summary>
        ///     The window is initially minimized. Same as the Iconic style.
        /// </summary>
        Minimize = 0x20000000,

        /// <summary>
        ///     The window has a minimize button. Cannot be combined with the WS_EX_CONTEXTHELP style. The SysMenu style must also
        ///     be specified.
        /// </summary>
        MinimizeBox = 0x00020000,

        /// <summary>
        ///     The window is an overlapped window. An overlapped window has a title bar and a border. Same as the WS_TILED style.
        /// </summary>
        Overlapped = 0x00000000,

        /// <summary>
        ///     The window is an overlapped window. Same as the TiledWindow style.
        /// </summary>
        OverlappedWindow = Overlapped | Caption | SysMenu | ThickFrame | MinimizeBox | MaximizeBox,

        /// <summary>
        ///     The windows is a pop-up window. This style cannot be used with the Child style.
        /// </summary>
        Popup = 0x80000000,

        /// <summary>
        ///     The window is a pop-up window. The Caption and PopupWindow styles must be combined to make the window menu visible.
        /// </summary>
        PopupWindow = Popup | Border | SysMenu,

        /// <summary>
        ///     he window has a sizing border. Same as the ThickFrame style.
        /// </summary>
        Sizebox = 0x00040000,

        /// <summary>
        ///     The window has a window menu on its title bar. The Caption style must also be specified.
        /// </summary>
        SysMenu = 0x00080000,

        /// <summary>
        ///     The window is a control that can receive the keyboard focus when the user presses the TAB key. Pressing the TAB key
        ///     changes the keyboard focus to the next control with the Tabstop style.
        /// </summary>
        Tabstop = 0x00010000,

        /// <summary>
        ///     The window has a sizing border. Same as the Sizebox style.
        /// </summary>
        ThickFrame = 0x00040000,

        /// <summary>
        ///     The window is an overlapped window. An overlapped window has a title bar and a border. Same as the Overlapped
        ///     style.
        /// </summary>
        Tiled = 0x00000000,

        /// <summary>
        ///     The window is an overlapped window. Same as the OverlappedWindow style.
        /// </summary>
        TiledWindow = Overlapped | Caption | SysMenu | ThickFrame | MinimizeBox | MaximizeBox,

        /// <summary>
        ///     The window is initially visible.
        /// </summary>
        Visible = 0x10000000,

        /// <summary>
        ///     The window has a vertical scroll bar.
        /// </summary>
        VScroll = 0x00200000
    }

    /// <summary>
    ///     All WindowMessages
    /// </summary>
    internal enum WindowMessage : uint
    {
        Null = 0x0000,
        Create = 0x0001,
        Destroy = 0x0002,
        Move = 0x0003,
        Size = 0x0005,
        Activate = 0x0006,
        Setfocus = 0x0007,
        Killfocus = 0x0008,
        Enable = 0x000A,
        Setredraw = 0x000B,
        Settext = 0x000C,
        Gettext = 0x000D,
        Gettextlength = 0x000E,
        Paint = 0x000F,
        Close = 0x0010,
        Queryendsession = 0x0011,
        Queryopen = 0x0013,
        Endsession = 0x0016,
        Quit = 0x0012,
        EraseBackground = 0x0014,
        Syscolorchange = 0x0015,
        Showwindow = 0x0018,
        Wininichange = 0x001A,
        Settingchange = Wininichange,
        Devmodechange = 0x001B,
        Activateapp = 0x001C,
        Fontchange = 0x001D,
        Timechange = 0x001E,
        Cancelmode = 0x001F,
        Setcursor = 0x0020,
        Mouseactivate = 0x0021,
        Childactivate = 0x0022,
        Queuesync = 0x0023,
        Getminmaxinfo = 0x0024,
        Painticon = 0x0026,
        Iconerasebkgnd = 0x0027,
        Nextdlgctl = 0x0028,
        Spoolerstatus = 0x002A,
        Drawitem = 0x002B,
        Measureitem = 0x002C,
        Deleteitem = 0x002D,
        Vkeytoitem = 0x002E,
        Chartoitem = 0x002F,
        Setfont = 0x0030,
        Getfont = 0x0031,
        Sethotkey = 0x0032,
        Gethotkey = 0x0033,
        Querydragicon = 0x0037,
        Compareitem = 0x0039,
        Getobject = 0x003D,
        Compacting = 0x0041,
        Commnotify = 0x0044,
        Windowposchanging = 0x0046,
        Windowposchanged = 0x0047,
        Power = 0x0048,
        Copydata = 0x004A,
        Canceljournal = 0x004B,
        Notify = 0x004E,
        Inputlangchangerequest = 0x0050,
        Inputlangchange = 0x0051,
        Tcard = 0x0052,
        Help = 0x0053,
        Userchanged = 0x0054,
        Notifyformat = 0x0055,
        Contextmenu = 0x007B,
        Stylechanging = 0x007C,
        Stylechanged = 0x007D,
        Displaychange = 0x007E,
        Geticon = 0x007F,
        Seticon = 0x0080,
        Nccreate = 0x0081,
        Ncdestroy = 0x0082,
        Nccalcsize = 0x0083,
        Nchittest = 0x0084,
        NcPaint = 0x0085,
        Ncactivate = 0x0086,
        Getdlgcode = 0x0087,
        Syncpaint = 0x0088,

        Ncmousemove = 0x00A0,
        Nclbuttondown = 0x00A1,
        Nclbuttonup = 0x00A2,
        Nclbuttondblclk = 0x00A3,
        Ncrbuttondown = 0x00A4,
        Ncrbuttonup = 0x00A5,
        Ncrbuttondblclk = 0x00A6,
        Ncmbuttondown = 0x00A7,
        Ncmbuttonup = 0x00A8,
        Ncmbuttondblclk = 0x00A9,
        Ncxbuttondown = 0x00AB,
        Ncxbuttonup = 0x00AC,
        Ncxbuttondblclk = 0x00AD,

        InputDeviceChange = 0x00FE,
        Input = 0x00FF,

        Keyfirst = 0x0100,
        Keydown = 0x0100,
        Keyup = 0x0101,
        Char = 0x0102,
        Deadchar = 0x0103,
        Syskeydown = 0x0104,
        Syskeyup = 0x0105,
        Syschar = 0x0106,
        Sysdeadchar = 0x0107,
        Unichar = 0x0109,
        Keylast = 0x0109,

        ImeStartcomposition = 0x010D,
        ImeEndcomposition = 0x010E,
        ImeComposition = 0x010F,
        ImeKeylast = 0x010F,

        Initdialog = 0x0110,
        Command = 0x0111,
        Syscommand = 0x0112,
        Timer = 0x0113,
        Hscroll = 0x0114,
        Vscroll = 0x0115,
        Initmenu = 0x0116,
        Initmenupopup = 0x0117,
        Menuselect = 0x011F,
        Menuchar = 0x0120,
        Enteridle = 0x0121,
        Menurbuttonup = 0x0122,
        Menudrag = 0x0123,
        Menugetobject = 0x0124,
        Uninitmenupopup = 0x0125,
        Menucommand = 0x0126,

        Changeuistate = 0x0127,
        Updateuistate = 0x0128,
        Queryuistate = 0x0129,

        Ctlcolormsgbox = 0x0132,
        Ctlcoloredit = 0x0133,
        Ctlcolorlistbox = 0x0134,
        Ctlcolorbtn = 0x0135,
        Ctlcolordlg = 0x0136,
        Ctlcolorscrollbar = 0x0137,
        Ctlcolorstatic = 0x0138,
        Gethmenu = 0x01E1,

        Mousefirst = 0x0200,
        Mousemove = 0x0200,
        Lbuttondown = 0x0201,
        Lbuttonup = 0x0202,
        Lbuttondblclk = 0x0203,
        Rbuttondown = 0x0204,
        Rbuttonup = 0x0205,
        Rbuttondblclk = 0x0206,
        Mbuttondown = 0x0207,
        Mbuttonup = 0x0208,
        Mbuttondblclk = 0x0209,
        Mousewheel = 0x020A,
        Xbuttondown = 0x020B,
        Xbuttonup = 0x020C,
        Xbuttondblclk = 0x020D,
        Mousehwheel = 0x020E,

        Parentnotify = 0x0210,
        Entermenuloop = 0x0211,
        Exitmenuloop = 0x0212,

        Nextmenu = 0x0213,
        Sizing = 0x0214,
        Capturechanged = 0x0215,
        Moving = 0x0216,

        Powerbroadcast = 0x0218,

        Devicechange = 0x0219,

        Mdicreate = 0x0220,
        Mdidestroy = 0x0221,
        Mdiactivate = 0x0222,
        Mdirestore = 0x0223,
        Mdinext = 0x0224,
        Mdimaximize = 0x0225,
        Mditile = 0x0226,
        Mdicascade = 0x0227,
        Mdiiconarrange = 0x0228,
        Mdigetactive = 0x0229,

        Mdisetmenu = 0x0230,
        Entersizemove = 0x0231,
        Exitsizemove = 0x0232,
        Dropfiles = 0x0233,
        Mdirefreshmenu = 0x0234,

        ImeSetcontext = 0x0281,
        ImeNotify = 0x0282,
        ImeControl = 0x0283,
        ImeCompositionfull = 0x0284,
        ImeSelect = 0x0285,
        ImeChar = 0x0286,
        ImeRequest = 0x0288,
        ImeKeydown = 0x0290,
        ImeKeyup = 0x0291,

        Mousehover = 0x02A1,
        Mouseleave = 0x02A3,
        Ncmousehover = 0x02A0,
        Ncmouseleave = 0x02A2,

        WtssessionChange = 0x02B1,

        TabletFirst = 0x02c0,
        TabletLast = 0x02df,

        DpiChanged = 0x02E0,

        Cut = 0x0300,
        Copy = 0x0301,
        Paste = 0x0302,
        Clear = 0x0303,
        Undo = 0x0304,
        Renderformat = 0x0305,
        Renderallformats = 0x0306,
        Destroyclipboard = 0x0307,
        Drawclipboard = 0x0308,
        Paintclipboard = 0x0309,
        Vscrollclipboard = 0x030A,
        Sizeclipboard = 0x030B,
        Askcbformatname = 0x030C,
        Changecbchain = 0x030D,
        Hscrollclipboard = 0x030E,
        Querynewpalette = 0x030F,
        Paletteischanging = 0x0310,
        Palettechanged = 0x0311,
        Hotkey = 0x0312,

        Print = 0x0317,
        Printclient = 0x0318,

        Appcommand = 0x0319,

        Themechanged = 0x031A,

        Clipboardupdate = 0x031D,

        DwmCompositionChanged = 0x031E,
        Dwmncrenderingchanged = 0x031F,
        Dwmcolorizationcolorchanged = 0x0320,
        Dwmwindowmaximizedchange = 0x0321,

        Gettitlebarinfoex = 0x033F,

        Handheldfirst = 0x0358,
        Handheldlast = 0x035F,

        Afxfirst = 0x0360,
        Afxlast = 0x037F,

        Penwinfirst = 0x0380,
        Penwinlast = 0x038F,

        App = 0x8000,

        User = 0x0400,

        Reflect = User + 0x1C00
    }

    /// <summary>
    ///     Contains a WindowsMessage
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct Message
    {
        public IntPtr Hwnd;
        public WindowMessage Msg;
        public IntPtr lParam;
        public IntPtr wParam;
        public uint Time;
        public int X;
        public int Y;
    }

    /// <summary>
    ///     X and Y desktop coordinates
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct NativePoint
    {
        public int X;
        public int Y;
    }

    /// <summary>
    ///     Contains Desktop Coordinates
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct NativeRect
    {
        /// <summary>
        ///     x position of upper-left corner
        /// </summary>
        public int Left; // x position of upper-left corner

        /// <summary>
        ///     x position of upper-left corner
        /// </summary>
        public int Top; // x position of upper-left corner

        /// <summary>
        ///     x position of lower-right corner
        /// </summary>
        public int Right; // x position of lower-right corner

        /// <summary>
        ///     y position of lower-right corner
        /// </summary>
        public int Bottom; // y position of lower-right corner
    }

    /// <summary>
    ///     Stores information for window creation
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct WindowClassEx
    {
        public uint Size;
        public uint Style;
        public IntPtr WindowProc;
        public int ClsExtra;
        public int WindowExtra;
        public IntPtr Instance;
        public IntPtr Icon;
        public IntPtr Curser;
        public IntPtr Background;
        public string MenuName;
        public string ClassName;
        public IntPtr IconSm;

        public static uint NativeSize()
        {
            return (uint)Marshal.SizeOf(typeof(WindowClassEx));
        }
    }
}
