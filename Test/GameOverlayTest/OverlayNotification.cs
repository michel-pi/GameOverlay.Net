using System;
using GameOverlay.Graphics;
using GameOverlay.Graphics.Primitives;

namespace GameOverlayTest
{
    public class OverlayNotification : IShape
    {
        private D2DDevice _device;
        private Geometry _geometry;

        public ID2DBrush BackgroundBrush { get; set; }
        public ID2DBrush ForegroundBrush { get; set; }
        public D2DFont Font { get; set; }

        public float X { get; private set; }
        public float Y { get; private set; }
        public float Width { get; private set; }
        public float Height { get; private set; }

        public float HeaderSize { get; set; }
        public float BodySize { get; set; }

        public string Header { get; set; }
        public string Body { get; set; }

        private OverlayNotification()
        {

        }

        public OverlayNotification(D2DDevice device, ID2DBrush background, ID2DBrush foreground, D2DFont font)
        {
            _device = device;
            BackgroundBrush = background;
            ForegroundBrush = foreground;
            Font = font;
        }

        public void Setup(int x, int y, int width, int height)
        {
            if (_geometry != null) _geometry.Dispose();

            _geometry = _device.CreateGeometry();

            _geometry.BeginFigure(new Point(x, y), true);
            _geometry.AddPoint(new Point(x + width, y));
            _geometry.AddCurve(new Point(x + width, y + height), -25.0f);
            _geometry.AddPoint(new Point(x, y + height));
            _geometry.EndFigure(true);

            _geometry.Close();

            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public void Draw(D2DDevice device)
        {
            _geometry.Fill(BackgroundBrush);

            device.DrawText(Header, X + 5, Y + 5, Font, HeaderSize, ForegroundBrush);
            device.DrawText(Body, X + 5, Y + HeaderSize * 2 + 5, Font, BodySize, ForegroundBrush);
        }
    }
}
