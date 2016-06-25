using System;
using System.Diagnostics;

using DirectXOverlayWindow;

namespace OverlayExample
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "OverlayExample";

            OverlayWindow overlay = new OverlayWindow(false);

            Stopwatch watch = Stopwatch.StartNew();

            int redBrush = overlay.Graphics.CreateBrush(0x7FFF0000);
            int redOpacityBrush = overlay.Graphics.CreateBrush(System.Drawing.Color.FromArgb(80, 255, 0, 0));
            int yellowBrush = overlay.Graphics.CreateBrush(0x7FFFFF00);

            int font = overlay.Graphics.CreateFont("Arial", 20);
            int hugeFont = overlay.Graphics.CreateFont("Arial", 50, true);

            float rotation = 0.0f;
            int fps = 0;
            int displayFPS = 0;

            while(true)
            {
                overlay.Graphics.BeginScene();
                overlay.Graphics.ClearScene();

                //first row
                overlay.Graphics.DrawText("DrawBarH", font, redBrush, 50, 40);
                overlay.Graphics.DrawBarH(50, 70, 20, 100, 80, 2, redBrush, yellowBrush);

                overlay.Graphics.DrawText("DrawBarV", font, redBrush, 200, 40);
                overlay.Graphics.DrawBarV(200, 120, 100, 20, 80, 2, redBrush, yellowBrush);

                overlay.Graphics.DrawText("DrawBox2D", font, redBrush, 350, 40);
                overlay.Graphics.DrawBox2D(350, 70, 50, 100, 2, redBrush, redOpacityBrush);

                overlay.Graphics.DrawText("DrawBox3D", font, redBrush, 500, 40);
                overlay.Graphics.DrawBox3D(500, 80, 50, 100, 10, 2, redBrush, redOpacityBrush);

                overlay.Graphics.DrawText("DrawCircle3D", font, redBrush, 650, 40);
                overlay.Graphics.DrawCircle(700, 120, 35, 2, redBrush);

                overlay.Graphics.DrawText("DrawEdge", font, redBrush, 800, 40);
                overlay.Graphics.DrawEdge(800, 70, 50, 100, 10, 2, redBrush);

                overlay.Graphics.DrawText("DrawLine", font, redBrush, 950, 40);
                overlay.Graphics.DrawLine(950, 70, 1000, 200, 2, redBrush);

                //second row
                overlay.Graphics.DrawText("DrawPlus", font, redBrush, 50, 250);
                overlay.Graphics.DrawPlus(70, 300, 15, 2, redBrush);

                overlay.Graphics.DrawText("DrawRectangle", font, redBrush, 200, 250);
                overlay.Graphics.DrawRectangle(200, 300, 50, 100, 2, redBrush);

                overlay.Graphics.DrawText("DrawRectangle3D", font, redBrush, 350, 250);
                overlay.Graphics.DrawRectangle3D(350, 320, 50, 100, 10, 2, redBrush);

                overlay.Graphics.DrawText("DrawSwastika", font, redBrush, 600, 250);
                overlay.Graphics.DrawSwastika(650, 350, 50, 2, redBrush);

                overlay.Graphics.DrawText("FillCircle", font, redBrush, 800, 250);
                overlay.Graphics.FillCircle(850, 350, 50, redBrush);

                overlay.Graphics.DrawText("FillRectangle", font, redBrush, 950, 250);
                overlay.Graphics.FillRectangle(950, 300, 50, 100, redBrush);

                //third row
                overlay.Graphics.DrawText("RotateSwastika", font, redBrush, 50, 450);
                overlay.Graphics.RotateSwastika(150, 600, 50, 2, rotation, redBrush);

                rotation += 0.03f;//related to speed
                if (rotation > 50.0f)//size of the swastika
                    rotation = -50.0f;

                if(watch.ElapsedMilliseconds > 1000)
                {
                    displayFPS = fps;
                    fps = 0;
                    watch.Restart();
                }
                else
                {
                    fps++;
                }

                overlay.Graphics.DrawText("FPS: " + displayFPS.ToString(), hugeFont, redBrush, 400, 600, false);

                overlay.Graphics.EndScene();
            }
        }
    }
}
