using System;
using System.Runtime.InteropServices;

namespace GameOverlay.PInvoke
{
    internal static class WinApi
    {
        [DllImport("kernel32.dll", EntryPoint = "GetProcAddress", SetLastError = false, CharSet = CharSet.Ansi)]
        private static extern IntPtr InternalGetProcAddress(IntPtr hmodule, string procName);

        [DllImport("kernel32.dll", EntryPoint = "LoadLibraryW", SetLastError = false, CharSet = CharSet.Unicode)]
        private static extern IntPtr InternalLoadLibraryW(string lpFileName);

        [DllImport("kernel32.dll", EntryPoint = "GetModuleHandleW", SetLastError = false, CharSet = CharSet.Unicode)]
        private static extern IntPtr InternalGetModuleHandleW(string modulename);

        /// <summary>
        /// Returns the function pointer of an exported native method
        /// </summary>
        /// <param name="modulename">The modulename (with .dll)</param>
        /// <param name="procname">Function name</param>
        /// <returns>Function pointer</returns>
        public static IntPtr GetProcAddress(string modulename, string procname)
        {
            IntPtr hModule = InternalGetModuleHandleW(modulename);

            if (hModule == IntPtr.Zero) hModule = InternalLoadLibraryW(modulename);

            return InternalGetProcAddress(hModule, procname);
        }

        /// <summary>
        /// Returns a <c>delegate</c> of an exported native method
        /// </summary>
        /// <typeparam name="T"><c>delegate</c></typeparam>
        /// <param name="modulename">The modulename (with .dll)</param>
        /// <param name="procname">Function name</param>
        /// <returns></returns>
        /// <exception cref="Exception">module: " + modulename + "\tproc: " + procname</exception>
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