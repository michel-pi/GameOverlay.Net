using System;

namespace GameOverlay.Drawing
{
    /// <summary>
    /// Represents a Scene / frame of a Graphics surface.
    /// </summary>
    public class Scene : IDisposable
    {
        /// <summary>
        /// The Graphics surface.
        /// </summary>
        public Graphics Device { get; private set; }

        private Scene()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Initializes a new Scene using a Graphics surface
        /// </summary>
        /// <param name="device">A Graphics surface</param>
        public Scene(Graphics device)
        {
            Device = device ?? throw new ArgumentNullException(nameof(device));
            device.BeginScene();
        }

        /// <summary>
        /// Allows an object to try to free resources and perform other cleanup operations before it is reclaimed by garbage collection.
        /// </summary>
        ~Scene()
        {
            Dispose(false);
        }

        #region IDisposable Support
        private bool disposedValue = false;

        /// <summary>
        /// Releases all resources used by this Scene.
        /// </summary>
        /// <param name="disposing">A Boolean value indicating whether this is called from the destructor.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                Device.EndScene();

                disposedValue = true;
            }
        }

        /// <summary>
        /// Releases all resources used by this Scene.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        /// <summary>
        /// Converts a Scene to a Graphics surface.
        /// </summary>
        /// <param name="scene">The Scene object.</param>
        public static implicit operator Graphics(Scene scene)
        {
            if (scene.Device == null) throw new InvalidOperationException(nameof(scene.Device) + " is null");

            return scene.Device;
        }
    }
}
