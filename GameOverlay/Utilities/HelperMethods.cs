using System;

using GameOverlay.PInvoke;

namespace GameOverlay.Utilities
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

            if (string.IsNullOrEmpty(name)) throw new NullReferenceException("The executable module name is null!");

            try
            {
                mod.Dispose();
                proc.Dispose();
            }
            catch
            {

            }

            int index = name.IndexOf(@"\");

            if (index < 0) return name;

            if (name.Length == 1) return name;

            return name.Substring(index + 1);
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
            rect = new RECT();
            RECT clientRect = new RECT();

            int result = User32.GetWindowRect(hwnd, out rect);

            if (User32.GetClientRect(hwnd, out clientRect) == 0) return result;

            if (result == 0) return result; 

            int clientWidth = clientRect.Right - clientRect.Left;
            int clientHeight = clientRect.Bottom - clientRect.Top;

            int windowWidth = rect.Right - rect.Left;
            int windowHeight = rect.Bottom - rect.Top;

            if(clientWidth == windowWidth && clientHeight == windowHeight) return result;

            if(clientWidth != windowWidth)
            {
                int dif_x = clientWidth > windowWidth ? clientWidth - windowWidth : windowWidth - clientWidth;
                dif_x /= 2;

                rect.Right -= dif_x;
                rect.Left += dif_x;
            }

            if(clientHeight != windowHeight)
            {
                int dif_y = clientHeight > windowHeight ? clientHeight - windowHeight : windowHeight - clientHeight;
                dif_y /= 2;

                rect.Bottom -= dif_y;
                rect.Top += dif_y;
            }

            return result;
        }

        public static bool GetWindowClientRect(IntPtr hwnd, out RECT rect)
        {
            rect = new RECT();
            var client = new RECT();

            if (User32.GetWindowRect(hwnd, out rect) == 0) return false;
            if (User32.GetClientRect(hwnd, out client) == 0) return true;

            int clientWidth = client.Right - client.Left;
            int clientHeight = client.Bottom - client.Top;

            int windowWidth = rect.Right - rect.Left;
            int windowHeight = rect.Bottom - rect.Top;

            int dif_x = clientWidth > windowWidth ? clientWidth - windowWidth : windowWidth - clientWidth;
            int dif_y = clientHeight > windowHeight ? clientHeight - windowHeight : windowHeight - clientHeight;

            dif_x /= 2;

            rect.Left += dif_x;
            rect.Right -= dif_x;

            rect.Top += (dif_y - dif_x);
            rect.Bottom -= dif_x;

            return true;
        }
    }
}