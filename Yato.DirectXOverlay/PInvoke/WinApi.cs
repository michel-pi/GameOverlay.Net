using System;
using System.Runtime.InteropServices;

namespace Yato.DirectXOverlay.PInvoke
{
    internal static class WinApi
    {
        [DllImport("kernel32.dll", EntryPoint = "GetProcAddress", SetLastError = false, CharSet = CharSet.Ansi)]
        private static extern IntPtr InternalGetProcAddress(IntPtr hmodule, string procName);

        [DllImport("kernel32.dll", EntryPoint = "LoadLibraryW", SetLastError = false, CharSet = CharSet.Unicode)]
        private static extern IntPtr InternalLoadLibraryW(string lpFileName);

        [DllImport("kernel32.dll", EntryPoint = "GetModuleHandleW", SetLastError = false, CharSet = CharSet.Unicode)]
        private static extern IntPtr InternalGetModuleHandleW(string modulename);

        public static IntPtr GetProcAddress(string modulename, string procname)
        {
            IntPtr hModule = InternalGetModuleHandleW(modulename);

            if (hModule == IntPtr.Zero) hModule = InternalLoadLibraryW(modulename);

            return InternalGetProcAddress(hModule, procname);
        }

        public static T GetMethod<T>(string modulename, string procname)
        {
            IntPtr hModule = InternalGetModuleHandleW(modulename);

            if (hModule == IntPtr.Zero) hModule = InternalLoadLibraryW(modulename);

            IntPtr procAddress = InternalGetProcAddress(hModule, procname);

#if DEBUG
            if (hModule == IntPtr.Zero || procAddress == IntPtr.Zero)
                throw new Exception("module: " + modulename + "\tproc: " + procname);
#endif

            return (T)(object)Marshal.GetDelegateForFunctionPointer(procAddress, ObfuscatorNeedsThis<T>());
        }

        private static Type ObfuscatorNeedsThis<T>()
        {
            return typeof(T);
        }
    }
}