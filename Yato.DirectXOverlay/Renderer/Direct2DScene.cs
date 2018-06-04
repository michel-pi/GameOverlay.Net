using System;

namespace Yato.DirectXOverlay.Renderer
{
    /// <summary>
    /// </summary>
    /// <seealso cref="System.IDisposable"/>
    public class Direct2DScene : IDisposable
    {
        /// <summary>
        /// Gets the renderer.
        /// </summary>
        /// <value>The renderer.</value>
        public Direct2DRenderer Renderer { get; private set; }

        private Direct2DScene()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Direct2DScene"/> class.
        /// </summary>
        /// <param name="renderer">The renderer.</param>
        public Direct2DScene(Direct2DRenderer renderer)
        {
            Renderer = renderer;
            renderer.BeginScene();
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="Direct2DScene"/> class.
        /// </summary>
        ~Direct2DScene()
        {
            Dispose(false);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Direct2DScene"/> to <see cref="Direct2DRenderer"/>.
        /// </summary>
        /// <param name="scene">The scene.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Direct2DRenderer(Direct2DScene scene)
        {
            return scene.Renderer;
        }

        #region IDisposable Support

        /// <summary>
        /// The disposed value
        /// </summary>
        private bool disposedValue = false;

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        /// <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
        /// unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }

                Renderer.EndScene();

                disposedValue = true;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting
        /// unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);

            Dispose(true);
        }

        #endregion IDisposable Support
    }
}