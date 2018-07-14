using System;

using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;

namespace GameOverlay.Graphics
{
    public interface ID2DBrush : IDisposable
    {
        Brush GetBrush();
        void SetBrush(Brush brush);
    }
}
