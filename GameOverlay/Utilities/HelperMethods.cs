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
    public static class HelperMethods
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
        ///     Gets the real window rect.
        /// </summary>
        /// <param name="hwnd">Window Handle</param>
        /// <param name="rect">Real window bounds</param>
        /// <returns>Non-zero on success</returns>
        public static int GetRealWindowRect(IntPtr hwnd, out Rect rect)
        {
            rect = new Rect();

            int result = User32.GetWindowRect(hwnd, out rect);

            if (User32.GetClientRect(hwnd, out var clientRect) == 0) return result;

            if (result == 0) return result;

            int clientWidth = clientRect.Right - clientRect.Left;
            int clientHeight = clientRect.Bottom - clientRect.Top;

            int windowWidth = rect.Right - rect.Left;
            int windowHeight = rect.Bottom - rect.Top;

            if (clientWidth == windowWidth && clientHeight == windowHeight) return result;

            if (clientWidth != windowWidth)
            {
                int difX = clientWidth > windowWidth ? clientWidth - windowWidth : windowWidth - clientWidth;
                difX /= 2;

                rect.Right -= difX;
                rect.Left += difX;
            }

            if (clientHeight == windowHeight) return result;

            int difY = clientHeight > windowHeight ? clientHeight - windowHeight : windowHeight - clientHeight;
            difY /= 2;

            rect.Bottom -= difY;
            rect.Top += difY;

            return result;
        }

        /// <summary>
        ///     Gets the window client rect.
        /// </summary>
        /// <param name="hwnd">The HWND.</param>
        /// <param name="rect">The rect.</param>
        /// <returns></returns>
        public static bool GetWindowClientRect(IntPtr hwnd, out Rect rect)
        {
            rect = new Rect();

            if (User32.GetWindowRect(hwnd, out rect) == 0) return false;
            if (User32.GetClientRect(hwnd, out var client) == 0) return true;

            int clientWidth = client.Right - client.Left;
            int clientHeight = client.Bottom - client.Top;

            int windowWidth = rect.Right - rect.Left;
            int windowHeight = rect.Bottom - rect.Top;

            int difX = clientWidth > windowWidth ? clientWidth - windowWidth : windowWidth - clientWidth;
            int difY = clientHeight > windowHeight ? clientHeight - windowHeight : windowHeight - clientHeight;

            difX /= 2;

            rect.Left += difX;
            rect.Right -= difX;

            rect.Top += difY - difX;
            rect.Bottom -= difX;

            return true;
        }
    }
}