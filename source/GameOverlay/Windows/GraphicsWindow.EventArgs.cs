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

        /// <summary>
        /// Gets the number of frames rendered in the current loop.
        /// </summary>
        public int FrameCount { get; }

        /// <summary>
        /// Gets the current time in milliseconds.
        /// </summary>
        public long FrameTime { get; }

        /// <summary>
        /// Gets the elapsed time in milliseconds since the last frame.
        /// </summary>
        public long DeltaTime { get; }

        private DrawGraphicsEventArgs()
        {
        }

        /// <summary>
        /// Initializes a new DrawGraphicsEventArgs with a Graphics surface.
        /// </summary>
        /// <param name="graphics">A graphics surface.</param>
        /// <param name="frameCount">The number of the currently rendered frame. Starting at 1.</param>
		/// <param name="frameTime">The current time in milliseconds.</param>
		/// <param name="deltaTime">The elapsed time in milliseconds since the last frame.</param>
		public DrawGraphicsEventArgs(Graphics graphics, int frameCount, long frameTime, long deltaTime)
        {
            Graphics = graphics ?? throw new ArgumentNullException(nameof(graphics));

            FrameCount = frameCount;
            FrameTime = frameTime;
            DeltaTime = deltaTime;
        }
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
