#if DEBUG
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Yato.DirectXOverlay;
using Yato.DirectXOverlay.Renderer;
using Yato.DirectXOverlay.Windows;

namespace Yato.DirectXOverlay
{
    class Issue9
    {
        private static Direct2DRenderer gfx;
        private static Direct2DFont font;
        private static Direct2DBrush brush;
        private static Direct2DBrush backgroundBrush;

        private static void Main(string[] args)
        {
            var game_process = GetProcess("csgo", 5000);

            var overlay = new StickyOverlayWindow(game_process.MainWindowHandle, new OverlayCreationOptions
            {
                BypassTopmost = true,
                WindowTitle = HelperMethods.GenerateRandomString(5, 11)
            });

            overlay.OnWindowBoundsChanged += Overlay_OnWindowBoundsChanged;

            gfx = new Direct2DRenderer(new RendererOptions
            {
                AntiAliasing = true,
                Hwnd = overlay.WindowHandle,
                MeasureFps = true,
                VSync = false
            });

            font = gfx.CreateFont("Arial", 22);
            brush = gfx.CreateBrush(255, 0, 0, 255);
            backgroundBrush = gfx.CreateBrush(0xCC, 0xCC, 0xCC, 80);

            var timer = new FrameTimer(60);

            timer.OnFrameStart += Timer_OnFrameStart;
            timer.FPS = 900;
            timer.Start();

            Console.WriteLine("drawing...");
            Console.ReadLine();
        }

        private static void Overlay_OnWindowBoundsChanged(int x, int y, int width, int height)
        {
            gfx.Resize(width, height);
        }

        private static void Timer_OnFrameStart()
        {
            gfx.BeginScene();
            gfx.ClearScene();

            gfx.DrawText($"FPS: {gfx.FPS}", 50, 50, font, brush);

            //Console.WriteLine($"Window Height: {gfx.Height} Width: {gfx.Width}");
            //Console.WriteLine($"Target Hwnd: 0x{gfx.RenderTargetHwnd.ToString("X")}");

            gfx.EndScene();
        }

        #region Helpers

        static Process GetProcess(string name, int timeout_ms)
        {
            var start_tickcount = Environment.TickCount;

            Process process;
            while ((process = Process.GetProcessesByName(name).FirstOrDefault()) == null)
            {
                Thread.Sleep(1);

                if (Environment.TickCount - start_tickcount > timeout_ms)
                    return null;
            }

            return process;
        }

        #endregion
    }
}
#endif