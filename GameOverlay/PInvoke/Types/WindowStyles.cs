using System;

namespace GameOverlay.PInvoke
{
    /// <summary>
    /// Window Styles
    /// </summary>
    [Flags]
    public enum WindowStyles : uint
    {
        /// <summary>
        /// The window has a thin-line border.
        /// </summary>
        Border = 0x00800000,
        /// <summary>
        /// The window has a title bar (includes the Border style).
        /// </summary>
        Caption = 0x00C00000,
        /// <summary>
        /// The window is a child window. A window with this style cannot have a menu bar. This style cannot be used with the Popup style.
        /// </summary>
        Child = 0x40000000,
        /// <summary>
        /// Same as the Child style.
        /// </summary>
        ChildWindow = 0x40000000,
        /// <summary>
        /// Excludes the area occupied by child windows when drawing occurs within the parent window. This style is used when creating the parent window.
        /// </summary>
        ClipChildren = 0x02000000,
        /// <summary>
        /// Clips child windows relative to each other; that is, when a particular child window receives a WM_PAINT message, the ClipSiblings style clips all other overlapping child windows out of the region of the child window to be updated. If ClipSiblings is not specified and child windows overlap, it is possible, when drawing within the client area of a child window, to draw within the client area of a neighboring child window.
        /// </summary>
        ClipSiblings = 0x04000000,
        /// <summary>
        /// The window is initially disabled. A disabled window cannot receive input from the user. To change this after a window has been created, use the EnableWindow function.
        /// </summary>
        Disabled = 0x08000000,
        /// <summary>
        /// The window has a border of a style typically used with dialog boxes. A window with this style cannot have a title bar.
        /// </summary>
        DlgFrame = 0x00400000,
        /// <summary>
        /// The window is the first control of a group of controls. The group consists of this first control and all controls defined after it, up to the next control with the Group style. The first control in each group usually has the Tabstop style so that the user can move from group to group. The user can subsequently change the keyboard focus from one control in the group to the next control in the group by using the direction keys.
        /// </summary>
        Group = 0x00020000,
        /// <summary>
        /// The window has a horizontal scroll bar.
        /// </summary>
        HScroll = 0x00100000,
        /// <summary>
        /// The window is initially minimized. Same as the Minimize style.
        /// </summary>
        Iconic = 0x20000000,
        /// <summary>
        /// The window is initially maximized.
        /// </summary>
        Maximize = 0x01000000,
        /// <summary>
        /// The window has a maximize button. Cannot be combined with the WS_EX_CONTEXTHELP style. The SysMenu style must also be specified. 
        /// </summary>
        MaximizeBox = 00010000,
        /// <summary>
        /// The window is initially minimized. Same as the Iconic style.
        /// </summary>
        Minimize = 0x20000000,
        /// <summary>
        /// The window has a minimize button. Cannot be combined with the WS_EX_CONTEXTHELP style. The SysMenu style must also be specified. 
        /// </summary>
        MinimizeBox = 0x00020000,
        /// <summary>
        /// The window is an overlapped window. An overlapped window has a title bar and a border. Same as the WS_TILED style.
        /// </summary>
        Overlapped = 0x00000000,
        /// <summary>
        /// The window is an overlapped window. Same as the TiledWindow style. 
        /// </summary>
        OverlappedWindow = Overlapped | Caption | SysMenu | ThickFrame | MinimizeBox | MaximizeBox,
        /// <summary>
        /// The windows is a pop-up window. This style cannot be used with the Child style.
        /// </summary>
        Popup = 0x80000000,
        /// <summary>
        /// The window is a pop-up window. The Caption and PopupWindow styles must be combined to make the window menu visible.
        /// </summary>
        PopupWindow = Popup | Border | SysMenu,
        /// <summary>
        /// he window has a sizing border. Same as the ThickFrame style.
        /// </summary>
        Sizebox = 0x00040000,
        /// <summary>
        /// The window has a window menu on its title bar. The Caption style must also be specified.
        /// </summary>
        SysMenu = 0x00080000,
        /// <summary>
        /// The window is a control that can receive the keyboard focus when the user presses the TAB key. Pressing the TAB key changes the keyboard focus to the next control with the Tabstop style.
        /// </summary>
        Tabstop = 0x00010000,
        /// <summary>
        /// The window has a sizing border. Same as the Sizebox style.
        /// </summary>
        ThickFrame = 0x00040000,
        /// <summary>
        /// The window is an overlapped window. An overlapped window has a title bar and a border. Same as the Overlapped style. 
        /// </summary>
        Tiled = 0x00000000,
        /// <summary>
        /// The window is an overlapped window. Same as the OverlappedWindow style. 
        /// </summary>
        TiledWindow = Overlapped | Caption | SysMenu | ThickFrame | MinimizeBox | MaximizeBox,
        /// <summary>
        /// The window is initially visible.
        /// </summary>
        Visible = 0x10000000,
        /// <summary>
        /// The window has a vertical scroll bar.
        /// </summary>
        VScroll = 0x00200000
    }
}
