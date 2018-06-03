using System;

using Yato.DirectXOverlay.PInvoke;

namespace Yato.DirectXOverlay
{
    /// <summary>
    /// Useful Methods
    /// </summary>
    public static class HelperMethods
    {
        private static Random _rng = new Random();

        private static string[] legitWindows = new string[]
            {
                "Teamspeak 3",
                "Steam",
                "Discord",
                "Mozilla Firefox"
            };

        /// <summary>
        /// Generates a random string
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
            {
                chars[i] = (char)_rng.Next(97, 123);
            }

            return new string(chars);
        }

        /// <summary>
        /// Gets the name of the executable
        /// </summary>
        /// <returns></returns>
        public static string GetExecutableName()
        {
            var proc = System.Diagnostics.Process.GetCurrentProcess();
            var mod = proc.MainModule;

            string name = mod.FileName;

            mod.Dispose();
            proc.Dispose();

            // Path class tends to throw errors. microsoft is lazy af
            return name.Contains(@"\") ? System.IO.Path.GetFileNameWithoutExtension(name) : name;
        }

        /// <summary>
        /// Returns a legit window name
        /// </summary>
        /// <returns></returns>
        public static string GetLegitWindowName()
        {
            return legitWindows[_rng.Next(0, legitWindows.Length)]; // Note: random max value is exclusive ;)
        }

        /// <summary>
        /// Gets the real window rect.
        /// </summary>
        /// <param name="hwnd">Window Handle</param>
        /// <param name="rect">Real window bounds</param>
        /// <returns>Non-zero on success</returns>
        public static int GetRealWindowRect(IntPtr hwnd, out RECT rect)
        {
            RECT windowRect = new RECT();
            RECT clientRect = new RECT();

            int result = User32.GetWindowRect(hwnd, out windowRect);
            if (User32.GetClientRect(hwnd, out clientRect) == 0)
            {
                rect = windowRect;
                return result;
            }

            int windowWidth = windowRect.Right - windowRect.Left;
            int windowHeight = windowRect.Bottom - windowRect.Top;

            if (windowWidth == clientRect.Right && windowHeight == clientRect.Bottom)
            {
                rect = windowRect;
                return result;
            }

            int dif_x = windowWidth > clientRect.Right ? windowWidth - clientRect.Right : clientRect.Right - windowWidth;
            int dif_y = windowHeight > clientRect.Bottom ? windowHeight - clientRect.Bottom : clientRect.Bottom - windowHeight;

            dif_x /= 2;
            dif_y /= 2;

            windowRect.Left += dif_x;
            windowRect.Top += dif_y;

            windowRect.Right -= dif_x;
            windowRect.Bottom -= dif_y;

            rect = windowRect;
            return result;
        }
    }
}