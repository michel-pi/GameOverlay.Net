using System;
using System.Runtime.InteropServices;

namespace GameOverlay.PInvoke
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct WindowCompositionAttributeData
    {
        public static readonly uint MemorySize = (uint)Marshal.SizeOf(typeof(WindowCompositionAttributeData));

        public uint Attribute; // can be DwmWindowAttribute and WindowCompositionAttribute
        public IntPtr Data;
        public uint DataSize;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct AccentPolicy
    {
        public static readonly uint MemorySize = (uint)Marshal.SizeOf(typeof(AccentPolicy));

        public AccentState AccentState;
        public uint AccentFlags;
        public uint GradientColor;
        public uint AnimationId;
    }

    /*
     * Docs: https://docs.microsoft.com/en-us/windows/win32/api/dwmapi/ne-dwmapi-dwmwindowattribute
     * Used by: https://docs.microsoft.com/en-us/windows/win32/api/dwmapi/nf-dwmapi-dwmsetwindowattribute
     */
    internal enum DwmWindowAttribute : uint
    {
        NonClientRenderingEnabled,
        NonClientRenderingPolicy,
        TransitionsForceDisabled,
        AllowNonClientPaint,
        CaptionButtonBounds,
        NonClientRtlLayout,
        ForceIconicRepresentation,
        Flip3dPolicy,
        ExtendedFrameBounds,
        HasIconicBitmap,
        DisallowPeek,
        ExcludedFromPeek,
        Cloak,
        Cloaked,
        FreezeRepresentation,
        Last
    }

    /*
     * Undocumented enum used by http://undoc.airesoft.co.uk/user32.dll/SetWindowCompositionAttribute.php
     * The arguments used by the documented method and this one are nearly the same.
    */
    internal enum WindowCompositionAttribute : uint
    {
        Undefined = 0,
        NonClientRenderingEnabled,
        NonClientRenderingPolicy,
        TransitionsForceDisabled,
        AllowNonClientPaint,
        CaptionButtonBounds,
        NonClientRtlLayout,
        ForceIconicRepresentation,
        ExtendedFrameBounds,
        HasIconicBitmap,
        ThemeAttributes,
        NonClientRenderingExiled,
        NonClientAdornmentInfo,
        ExcludedFromLivePreview,
        VideoOverlayActive,
        ForceActiveWindowAppearance,
        DisallowPeek,
        Cloak,
        Cloaked,
        AccentPolicy,
        FreezeRepresentation,
        EverUncloaked,
        VisualOwner,
        Holographic,
        ExcludedFromDDA,
        PassiveUpdateMode,
        UseDarkModeColors,
        Last
    }

    internal enum AccentState : uint
    {
        Disabled,
        EnableGradient,
        EnableTransparentGradient,
        EnableBlurBehind,
        EnableAcrylicBlurBehind,
        HostBackDrop,
        InvalidState
    }
}
