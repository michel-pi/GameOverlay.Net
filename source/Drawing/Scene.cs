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

        /// <summary>
        /// Returns a value indicating whether this instance and a specified <see cref="T:System.Object" /> represent the same type and value.
        /// </summary>
        /// <param name="obj">The object to compare with this instance.</param>
        /// <returns><see langword="true" /> if <paramref name="obj" /> is a Scene and equal to this instance; otherwise, <see langword="false" />.</returns>
        public override bool Equals(object obj)
        {
            var scene = obj as Scene;

            if (scene == null)
            {
                return false;
            }
            else
            {
                return scene.Device == Device;
            }
        }

        /// <summary>
        /// Returns a value indicating whether two specified instances of Scene represent the same value.
        /// </summary>
        /// <param name="value">An object to compare to this instance.</param>
        /// <returns><see langword="true" /> if <paramref name="value" /> is equal to this instance; otherwise, <see langword="false" />.</returns>
        public bool Equals(Scene value)
        {
            return value != null
                && value.Device == Device;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            return Device.GetHashCode();
        }

        /// <summary>
        /// Converts this Scene to a human-readable string.
        /// </summary>
        /// <returns>A string representation of this Scene.</returns>
        public override string ToString()
        {
            return OverrideHelper.ToString(
                "Scene", GetHashCode().ToString(),
                "Device", Device.ToString());
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

        /// <summary>
        /// Returns a value indicating whether two specified instances of Scene represent the same value.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns> <see langword="true" /> if <paramref name="left" /> and <paramref name="right" /> are equal; otherwise, <see langword="false" />.</returns>
        public static bool Equals(Scene left, Scene right)
        {
            return left != null
                && left.Equals(right);
        }
    }
}
