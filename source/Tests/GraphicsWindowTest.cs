using System;
using System.Collections.Generic;

using GameOverlay.Drawing;
using GameOverlay.Windows;

namespace Tests
{
	public class GraphicsWindowTest
	{
		private readonly GraphicsWindow _window;
		private readonly Graphics _graphics;

		private readonly Dictionary<string, SolidBrush> _brushes;
		private readonly Dictionary<string, Font> _fonts;
		private readonly Dictionary<string, Image> _images;

		public GraphicsWindowTest()
		{
			_brushes = new Dictionary<string, SolidBrush>();
			_fonts = new Dictionary<string, Font>();
			_images = new Dictionary<string, Image>();

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
			_window.Dispose();
			_graphics.Dispose();
		}

		public void Run()
		{
			_window.StartThread();

			_window.JoinGraphicsThread();
			_window.JoinWindowThread();
		}

		private void _window_SetupGraphics(object sender, SetupGraphicsEventArgs e)
		{
			var gfx = e.Graphics;

			_brushes.Add("black", gfx.CreateSolidBrush(0, 0, 0));
			_brushes.Add("white", gfx.CreateSolidBrush(255, 255, 255));
			_brushes.Add("background", gfx.CreateSolidBrush(0, 0x27, 0x31, 255.0f * 0.8f));

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
			gfx.DrawRectangle(_brushes["white"], 20, 60, 400, 400, 1.0f);
		}
	}
}
