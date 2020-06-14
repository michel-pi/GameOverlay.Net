using System;
using System.Collections.Generic;

using GameOverlay.Drawing;
using GameOverlay.Windows;

using SharpDX.Direct2D1;

namespace Tests
{
	public class GraphicsWindowTest
	{
		private readonly GraphicsWindow _window;
		private readonly Graphics _graphics;

		private readonly Dictionary<string, SolidBrush> _brushes;
		private readonly Dictionary<string, Font> _fonts;
		private readonly Dictionary<string, GameOverlay.Drawing.Image> _images;

		public GraphicsWindowTest()
		{
			_brushes = new Dictionary<string, SolidBrush>();
			_fonts = new Dictionary<string, Font>();
			_images = new Dictionary<string, GameOverlay.Drawing.Image>();

			_graphics = new Graphics()
			{
				MeasureFPS = true,
				PerPrimitiveAntiAliasing = true,
				TextAntiAliasing = true,
				UseMultiThreadedFactories = false,
				VSync = false,
				WindowHandle = IntPtr.Zero
			};

			_window = new GraphicsWindow(_graphics)
			{
				IsTopmost = true,
				IsVisible = true,
				FPS = 60,
				X = 0,
				Y = 0,
				Width = 1280,
				Height = 720
			};

			_window.SetupGraphics += _window_SetupGraphics;
			_window.DestroyGraphics += _window_DestroyGraphics;
			_window.DrawGraphics += _window_DrawGraphics;
		}

		~GraphicsWindowTest()
		{
			//_window.Dispose();
			//_graphics.Dispose();
		}

		public void Run()
		{
			_window.Create();

			Console.WriteLine(_window.Handle.ToString("X"));
		}

		public void Join()
		{
			_window.Join();
		}

		public void Stop()
		{
			_window.Dispose();
			_graphics.Dispose();
		}

		public void ReCreate()
		{
			_window.Recreate();
		}

		private void _window_SetupGraphics(object sender, SetupGraphicsEventArgs e)
		{
			var gfx = e.Graphics;

			_brushes["black"] = gfx.CreateSolidBrush(0, 0, 0);
			_brushes["white"] = gfx.CreateSolidBrush(255, 255, 255);
			_brushes["background"] = gfx.CreateSolidBrush(0, 0x27, 0x31, 255.0f * 0.8f);

			Console.WriteLine(_window.Handle.ToString("X"));

			// fonts don't need to be recreated since they are owned by the font factory and not the drawing device
			if (e.RecreateResources) return;

			_fonts.Add("arial", gfx.CreateFont("Arial", 14));
		}

		private void _window_DestroyGraphics(object sender, DestroyGraphicsEventArgs e)
		{
			foreach (var pair in _brushes) pair.Value.Dispose();
			foreach (var pair in _fonts) pair.Value.Dispose();
			foreach (var pair in _images) pair.Value.Dispose();
		}

		private void _window_DrawGraphics(object sender, DrawGraphicsEventArgs e)
		{
			var gfx = e.Graphics;

			gfx.ClearScene(_brushes["background"]);

			gfx.DrawText(_fonts["arial"], 22, _brushes["white"], 20, 20, $"FPS: {gfx.FPS}");

			var device = gfx.GetRenderTarget() as WindowRenderTarget;
			var factory = gfx.GetFactory();

			//var region = new SharpDX.Direct2D1.RoundedRectangle()
			//{
			//	RadiusX = 16.0f,
			//	RadiusY = 16.0f,
			//	Rect = new SharpDX.Mathematics.Interop.RawRectangleF(200, 200, 300, 300)
			//};

			var geometry = new PathGeometry(factory);

			var sink = geometry.Open();
			sink.SetFillMode(FillMode.Winding);
			sink.BeginFigure(new SharpDX.Mathematics.Interop.RawVector2(200, 200), FigureBegin.Filled);

			sink.AddLine(new SharpDX.Mathematics.Interop.RawVector2(300 , 200));
			sink.AddArc(new ArcSegment()
			{
				ArcSize = ArcSize.Small,
				Point = new SharpDX.Mathematics.Interop.RawVector2(300, 300),
				RotationAngle = 0.0f,
				Size = new SharpDX.Size2F(16.0f, 16.0f),
				SweepDirection = SweepDirection.Clockwise
			});
			sink.AddLine(new SharpDX.Mathematics.Interop.RawVector2(200, 300));
			sink.AddArc(new ArcSegment()
			{
				ArcSize = ArcSize.Small,
				Point = new SharpDX.Mathematics.Interop.RawVector2(200, 200),
				RotationAngle = 0.0f,
				Size = new SharpDX.Size2F(16.0f, 16.0f),
				SweepDirection = SweepDirection.Clockwise
			});

			sink.EndFigure(FigureEnd.Open);
			sink.Close();
			sink.Dispose();

			// device.FillGeometry(geometry, _brushes["white"]);

			var options = new LayerParameters()
			{
				//ContentBounds = new SharpDX.Mathematics.Interop.RawRectangleF(float.NegativeInfinity, float.NegativeInfinity, float.PositiveInfinity, float.PositiveInfinity),
				GeometricMask = geometry,
				//Opacity = 1.0f
			};

			var layer = new Layer(device, new SharpDX.Size2F(gfx.Width, gfx.Height));

			device.PushLayer(ref options, layer);

			gfx.FillRectangle(_brushes["white"], 100, 100, 400, 400);

			device.PopLayer();

			layer.Dispose();
			geometry.Dispose();
		}
	}
}
