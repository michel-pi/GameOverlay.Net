using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace GameOverlay.PInvoke
{
    internal static class DynamicImport
    {
        [DllImport("kernel32.dll", EntryPoint = "GetProcAddress", SetLastError = true, CharSet = CharSet.Ansi)]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string procname);

        [DllImport("kernel32.dll", EntryPoint = "LoadLibraryW", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("kernel32.dll", EntryPoint = "GetModuleHandleW", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern IntPtr GetModuleHandle(string modulename);

        public static IntPtr ImportLibrary(string libraryName)
        {
            if (libraryName == null) throw new ArgumentNullException(nameof(libraryName));
            if (libraryName.Length == 0) throw new ArgumentOutOfRangeException(nameof(libraryName));

            IntPtr hModule = GetModuleHandle(libraryName);

            if (hModule == IntPtr.Zero)
            {
                hModule = LoadLibrary(libraryName);
            }

            if (hModule == IntPtr.Zero)
            {
                throw new DynamicImportException($"DynamicImport failed to import library \"{ libraryName }\"!");
            }
            else
            {
                return hModule;
            }
        }

        public static IntPtr ImportMethod(IntPtr moduleHandle, string methodName)
        {
            if (moduleHandle == IntPtr.Zero) throw new ArgumentOutOfRangeException(nameof(moduleHandle));

            if (methodName == null) throw new ArgumentNullException(nameof(methodName));
            if (methodName.Length == 0) throw new ArgumentOutOfRangeException(nameof(methodName));

            IntPtr procAddress = GetProcAddress(moduleHandle, methodName);

            if (procAddress == IntPtr.Zero)
            {
                throw new DynamicImportException($"DynamicImport failed to find method \"{ methodName }\" in module \"0x{ moduleHandle.ToString("X") }\"!");
            }
            else
            {
                return procAddress;
            }
        }

        public static IntPtr ImportMethod(string libraryName, string methodName)
        {
            return ImportMethod(ImportLibrary(libraryName), methodName);
        }

        public static T Import<T>(IntPtr moduleHandle, string methodName)
        {
            var address = ImportMethod(moduleHandle, methodName);

            return (T)(object)Marshal.GetDelegateForFunctionPointer(address, typeof(T));
        }

        public static T Import<T>(string libraryName, string methodName)
        {
            var address = ImportMethod(libraryName, methodName);

            return (T)(object)Marshal.GetDelegateForFunctionPointer(address, typeof(T));
        }
    }

    internal class DynamicImportException : Win32Exception
    {
        public DynamicImportException()
        {
        }

        public DynamicImportException(int error) : base(error)
        {
        }

        public DynamicImportException(string message) : base(message + Environment.NewLine + "ErrorCode: " + Marshal.GetLastWin32Error())
        {
        }

        public DynamicImportException(int error, string message) : base(error, message)
        {
        }

        public DynamicImportException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DynamicImportException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
