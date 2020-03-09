using System;

using GameOverlay.Drawing;

namespace GameOverlay.Windows
{
    /// <summary>
    /// Provides data for the DrawGraphics event.
    /// </summary>
    public class DrawGraphicsEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the Graphics surface.
        /// </summary>
        public Graphics Graphics { get; }

        private DrawGraphicsEventArgs()
        {
        }

        /// <summary>
        /// Initializes a new DrawGraphicsEventArgs with a Graphics surface.
        /// </summary>
        /// <param name="graphics"></param>
		public DrawGraphicsEventArgs(Graphics graphics)
            => Graphics = graphics ?? throw new ArgumentNullException(nameof(graphics));
    }

    /// <summary>
    /// Provides data for the SetupGraphics event.
    /// </summary>
    public class SetupGraphicsEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the Graphics surface.
        /// </summary>
        public Graphics Graphics { get; }

        /// <summary>
        /// Gets a boolean determining whether resources (brushes and images) have to be created again since the underlying device has changed.
        /// </summary>
        public bool RecreateResources { get; }

        private SetupGraphicsEventArgs()
        {
        }

        /// <summary>
        /// Initializes a new SetupGraphicsEventArgs with a Graphics surface.
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="recreateResources"></param>
        public SetupGraphicsEventArgs(Graphics graphics, bool recreateResources = false)
        {
            Graphics = graphics ?? throw new ArgumentNullException(nameof(graphics));
            RecreateResources = recreateResources;
        }
    }

    /// <summary>
    /// Provides data for the DestroyGraphics event.
    /// </summary>
    public class DestroyGraphicsEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the Graphics surface.
        /// </summary>
        public Graphics Graphics { get; }

        private DestroyGraphicsEventArgs()
        {
        }

        /// <summary>
        /// Initializes a new DestroyGraphicsEventArgs with a Graphics surface.
        /// </summary>
        /// <param name="graphics"></param>
        public DestroyGraphicsEventArgs(Graphics graphics)
            => Graphics = graphics ?? throw new ArgumentNullException(nameof(graphics));
    }
}
