using System;
using System.Collections.Generic;

using GameOverlay.PInvoke;

namespace GameOverlay.Windows
{
    internal static class WindowHelper
    {
        private const int MinRandomStringLen = 8;
        private const int MaxRandomStringLen = 16;

        private static readonly Random _random = new Random();
        private static readonly object _blacklistLock = new object();

        private static List<string> _windowClassesBlacklist = new List<string>();

        public static string GenerateRandomTitle()
        {
            return GenerateRandomAsciiString(MinRandomStringLen, MaxRandomStringLen);
        }
        // Generates a random window class name and respects the already used ones in the blacklist.
        public static string GenerateRandomClass()
        {
            lock (_blacklistLock)
            {
                while (true)
                {
                    string name = GenerateRandomAsciiString(MinRandomStringLen, MaxRandomStringLen);

                    if (_windowClassesBlacklist.Contains(name))
                    {
                        continue;
                    }
                    else
                    {
                        _windowClassesBlacklist.Add(name);

                        return name;
                    }
                }
            }
        }

        public static void MakeTopmost(IntPtr hwnd) => User32.SetWindowPos(hwnd, User32.HwndInsertTopMost, 0, 0, 0, 0, SwpFlags.ShowWindow | SwpFlags.NoActivate | SwpFlags.NoMove | SwpFlags.NoSize);
        public static void RemoveTopmost(IntPtr hwnd) => User32.SetWindowPos(hwnd, User32.HwndInsertNoTopmost, 0, 0, 0, 0, SwpFlags.NoActivate | SwpFlags.NoMove | SwpFlags.NoSize);
        
        // Gets the boundaries of a window using its handle.
        public static bool GetWindowRect(IntPtr hwnd, out NativeRect rect) => User32.GetWindowRect(hwnd, out rect);
        // Gets the boundaries of a window client area using its handle.
        public static bool GetWindowClient(IntPtr hwnd, out NativeRect rect)
        {
            // calculates the window bounds based on the difference of the client and the window rect

            rect = new NativeRect();

            if (!User32.GetWindowRect(hwnd, out rect)) return false;
            if (!User32.GetClientRect(hwnd, out var clientRect)) return true;
            
            int clientWidth = clientRect.Right - clientRect.Left;
            int clientHeight = clientRect.Bottom - clientRect.Top;

            int windowWidth = rect.Right - rect.Left;
            int windowHeight = rect.Bottom - rect.Top;

            if (clientWidth == windowWidth && clientHeight == windowHeight) return true;

            if (clientWidth != windowWidth)
            {
                int difX = clientWidth > windowWidth ? clientWidth - windowWidth : windowWidth - clientWidth;
                difX /= 2;

                rect.Right -= difX;
                rect.Left += difX;

                if (clientHeight != windowHeight)
                {
                    int difY = clientHeight > windowHeight ? clientHeight - windowHeight : windowHeight - clientHeight;

                    rect.Top += difY - difX;
                    rect.Bottom -= difX;
                }
            }
            else if (clientHeight != windowHeight)
            {
                int difY = clientHeight > windowHeight ? clientHeight - windowHeight : windowHeight - clientHeight;
                difY /= 2;

                rect.Bottom -= difY;
                rect.Top += difY;
            }
            
            return true;
        }
        
        // Extends the window frame into the client area.
        public static void ExtendFrameIntoClientArea(IntPtr hwnd)
        {
            var margin = new NativeMargin
            {
                cxLeftWidth = -1,
                cxRightWidth = -1,
                cyBottomHeight = -1,
                cyTopHeight = -1
            };

            DwmApi.DwmExtendFrameIntoClientArea(hwnd, ref margin);
        }

        private static string GenerateRandomAsciiString(int minLength, int maxLength)
        {
            int length = _random.Next(minLength, maxLength);

            char[] chars = new char[length];

            for (int i = 0; i < chars.Length; i++)
            {
                chars[i] = (char)_random.Next(97, 123); // ascii range for small letters
            }

            return new string(chars);
        }
    }
}
