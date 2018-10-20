using System;
using System.Diagnostics;
using GameOverlay.PInvoke;
using GameOverlay.PInvoke.Libraries;
using GameOverlay.PInvoke.Types;

namespace GameOverlay.Utilities
{
    /// <summary>
    ///     Useful Methods
    /// </summary>
    public static class WindowHelpers
    {
        private static Random _rng = new Random();

        private static readonly string[] LegitWindows =
        {
            "Teamspeak 3",
            "Steam",
            "Discord",
            "Mozilla Firefox"
        };

        /// <summary>
        ///     Generates a random string
        /// </summary>
        /// <param name="minlen">Minimum Length</param>
        /// <param name="maxlen">Maximum Length</param>
        /// <returns>A Random <c>string</c></returns>
        public static string GenerateRandomString(int minlen, int maxlen)
        {
            if (_rng == null) _rng = new Random();

            int len = _rng.Next(minlen, maxlen);

            char[] chars = new char[len];

            for (int i = 0; i < chars.Length; i++)
                chars[i] = (char) _rng.Next(97, 123);

            return new string(chars);
        }

        /// <summary>
        ///     Gets the name of the executable
        /// </summary>
        /// <returns></returns>
        public static string GetExecutableName()
        {
            var proc = Process.GetCurrentProcess();
            var mod = proc.MainModule;

            string name = mod.FileName;

            if (string.IsNullOrEmpty(name)) throw new NullReferenceException("The executable module name is null!");

            try
            {
                mod.Dispose();
                proc.Dispose();
            }
            catch
            {
                // ignored
            }

            int index = name.IndexOf(@"\", StringComparison.Ordinal);

            if (index < 0) return name;

            return name.Length == 1 ? name : name.Substring(index + 1);
        }

        /// <summary>
        ///     Returns a legit window name
        /// </summary>
        /// <returns></returns>
        public static string GetLegitWindowName()
        {
            return LegitWindows[_rng.Next(0, LegitWindows.Length)]; // Note: random max value is exclusive ;)
        }

        /// <summary>
        /// Gets the size of the window client.
        /// </summary>
        /// <param name="hwnd">The HWND.</param>
        /// <returns></returns>
        public static Tuple<int, int> GetWindowClientSize(IntPtr hwnd)
        {
            GetWindowClientRectInternal(hwnd, out NativeRect rect);

            return new Tuple<int, int>(rect.Right - rect.Left, rect.Bottom - rect.Top);
        }

        /// <summary>
        ///     Retrieves a handle to the foreground window (the window with which the user is currently working). The system assigns a slightly higher priority to the thread that creates the foreground window than it does to other threads.
        /// </summary>
        /// <returns>The return value is a handle to the foreground window. The foreground window can be NULL in certain circumstances, such as when a window is losing activation.</returns>
        public static IntPtr GetForegroundWindow()
        {
            return User32.GetForegroundWindow();
        }

        /// <summary>
        ///     Gets the real window nativeRect.
        /// </summary>
        /// <param name="hwnd">Window Handle</param>
        /// <param name="nativeRect">Real window bounds</param>
        /// <returns>true on success</returns>
        internal static bool GetRealWindowRectInternal(IntPtr hwnd, out NativeRect nativeRect)
        {
            nativeRect = new NativeRect();

            bool result = User32.GetWindowRect(hwnd, out nativeRect) != 0;

            if (User32.GetClientRect(hwnd, out var clientRect) == 0) return result;

            if (!result) return result;

            int clientWidth = clientRect.Right - clientRect.Left;
            int clientHeight = clientRect.Bottom - clientRect.Top;

            int windowWidth = nativeRect.Right - nativeRect.Left;
            int windowHeight = nativeRect.Bottom - nativeRect.Top;

            if (clientWidth == windowWidth && clientHeight == windowHeight) return result;

            if (clientWidth != windowWidth)
            {
                int difX = clientWidth > windowWidth ? clientWidth - windowWidth : windowWidth - clientWidth;
                difX /= 2;

                nativeRect.Right -= difX;
                nativeRect.Left += difX;
            }

            if (clientHeight == windowHeight) return result;

            int difY = clientHeight > windowHeight ? clientHeight - windowHeight : windowHeight - clientHeight;
            difY /= 2;

            nativeRect.Bottom -= difY;
            nativeRect.Top += difY;

            return result;
        }

        /// <summary>
        ///     Gets the window client nativeRect.
        /// </summary>
        /// <param name="hwnd">The HWND.</param>
        /// <param name="nativeRect">The nativeRect.</param>
        /// <returns></returns>
        internal static bool GetWindowClientRectInternal(IntPtr hwnd, out NativeRect nativeRect)
        {
            nativeRect = new NativeRect();

            if (User32.GetWindowRect(hwnd, out nativeRect) == 0) return false;
            if (User32.GetClientRect(hwnd, out var client) == 0) return true;

            int clientWidth = client.Right - client.Left;
            int clientHeight = client.Bottom - client.Top;

            int windowWidth = nativeRect.Right - nativeRect.Left;
            int windowHeight = nativeRect.Bottom - nativeRect.Top;

            int difX = clientWidth > windowWidth ? clientWidth - windowWidth : windowWidth - clientWidth;
            int difY = clientHeight > windowHeight ? clientHeight - windowHeight : windowHeight - clientHeight;

            difX /= 2;

            nativeRect.Left += difX;
            nativeRect.Right -= difX;

            nativeRect.Top += difY - difX;
            nativeRect.Bottom -= difX;

            return true;
        }
    }
}