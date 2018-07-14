#if DEBUG

using System;
using System.Diagnostics;

using GameOverlay.Graphics;
using GameOverlay.PInvoke;
using GameOverlay.Utilities;
using GameOverlay.Windows;

namespace GameOverlay
{
    public static class WindowBoundsTest
    {
        private static D2DDevice device;

        public static void Main(string[] args)
        {
            IntPtr handle = /*new IntPtr(0x30F98);//*/Process.GetCurrentProcess().MainWindowHandle;

            StickyOverlayWindow window = new StickyOverlayWindow(handle);

            window.OnWindowBoundsChanged += Window_OnWindowBoundsChanged;

            device = new D2DDevice(window.WindowHandle, false, true, true);

            D2DBrush red = device.CreateSolidColorBrush(255, 0, 0);
            D2DBrush green = device.CreateSolidColorBrush(0, 255, 0);
            D2DBrush blue = device.CreateSolidColorBrush(0, 0, 255);
            D2DBrush black = device.CreateSolidColorBrush(0, 0, 0);

            while (true)
            {
                //RECT windowRect = new RECT();
                //RECT clientRect = new RECT();
                RECT fixedRect = new RECT();

                //User32.GetWindowRect(handle, out windowRect);
                //User32.GetClientRect(handle, out clientRect);
                HelperMethods.GetWindowClientRect(handle, out fixedRect);

                //Console.WriteLine("WindowRect: " + windowRect.Output());
                //Console.WriteLine("ClientRect: " + clientRect.Output());
                //Console.WriteLine("FixedRect: " + fixedRect.Output());

                //Console.WriteLine();

                //Console.WriteLine("WindowRect: " + windowRect.ToBounds());
                //Console.WriteLine("ClientRect: " + clientRect.ToBounds());
                //Console.WriteLine("FixedRect: " + fixedRect.ToBounds());

                using (var scene = device.UseScene())
                {
                    device.FillRectangle(0, 0, fixedRect.Right - fixedRect.Left, fixedRect.Bottom - fixedRect.Top, red);

                    //device.DrawRectangle(0, 0, windowRect.Right - windowRect.Left, windowRect.Bottom - windowRect.Top, 16.0f, red);
                    //device.DrawRectangle(0, 0, clientRect.Right - clientRect.Left, clientRect.Bottom - clientRect.Top, 12.0f, green);
                    //device.DrawRectangle(0, 0, fixedRect.Right - fixedRect.Left, fixedRect.Bottom - fixedRect.Top, 6.0f, blue);
                }


                Console.ReadLine();
            }
        }

        private static void Window_OnWindowBoundsChanged(int x, int y, int width, int height)
        {
            Console.WriteLine("Resize: X=" + x + ", Y=" + y + ", width=" + width + ", height=" + height);

            if (device == null) return;

            device.Resize(width, height);
        }

        public static string ToBounds(this RECT rect)
        {
            return "{Width: " + (rect.Right - rect.Left) + ", Height: " + (rect.Bottom - rect.Top) + "}";
        }

        public static string Output(this RECT rect)
        {
            return "{Left: " + rect.Left.ToString() + ", " +
                "Top: " + rect.Top.ToString() + ", " +
                "Right: " + rect.Right.ToString() + ", " +
                "Bottom: " + rect.Bottom.ToString() + "}";
        }
    }
}
#endif