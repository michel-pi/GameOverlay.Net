using System;
using System.Security;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace GameOverlay.PInvoke
{
    [SuppressUnmanagedCodeSecurity()]
    internal static class WinApi
    {
        [DllImport("kernel32.dll", EntryPoint = "GetProcAddress", SetLastError = false, CharSet = CharSet.Ansi)]
        private static extern IntPtr InternalGetProcAddress(IntPtr hmodule, string procName);

        [DllImport("kernel32.dll", EntryPoint = "LoadLibraryW", SetLastError = false, CharSet = CharSet.Unicode)]
        private static extern IntPtr InternalLoadLibraryW(string lpFileName);

        [DllImport("kernel32.dll", EntryPoint = "GetModuleHandleW", SetLastError = false, CharSet = CharSet.Unicode)]
        private static extern IntPtr InternalGetModuleHandleW(string modulename);

        public static void ThrowWin32Exception(string message)
        {
            throw new Win32Exception(Marshal.GetLastWin32Error(), message + Environment.NewLine + "HResult: " + Marshal.GetHRForLastWin32Error());
        }

        public static IntPtr GetProcAddress(string modulename, string procname)
        {
            IntPtr hModule = InternalGetModuleHandleW(modulename);

            if (hModule == IntPtr.Zero)
            {
                hModule = InternalLoadLibraryW(modulename);

                if (hModule == IntPtr.Zero) ThrowWin32Exception("Failed to load \"" + modulename + "\".");
            }

            IntPtr result = InternalGetProcAddress(hModule, procname);

            if (result == IntPtr.Zero) ThrowWin32Exception("Failed to find exported symbol \"" + procname + "\" in \"" + modulename + "\".");

            return result;
        }

        public static T GetMethod<T>(string modulename, string procname)
        {
            IntPtr procAddress = GetProcAddress(modulename, procname);

            return (T)(object)Marshal.GetDelegateForFunctionPointer(procAddress, ObfuscatorNeedsThis<T>());
        }

        private static Type ObfuscatorNeedsThis<T>()
        {
            return typeof(T);
        }
    }
}