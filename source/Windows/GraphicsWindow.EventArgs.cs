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
        public Graphics Graphics { get; private set; }

		private DrawGraphicsEventArgs()
        {

        }

        /// <summary>
        /// Initializes a new DrawGraphicsEventArgs with a Graphics surface.
        /// </summary>
        /// <param name="graphics"></param>
		public DrawGraphicsEventArgs(Graphics graphics)
        {
            Graphics = graphics ?? throw new ArgumentNullException(nameof(graphics));
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
        public Graphics Graphics { get; private set; }

        private SetupGraphicsEventArgs()
        {

        }

        /// <summary>
        /// Initializes a new SetupGraphicsEventArgs with a Graphics surface.
        /// </summary>
        /// <param name="graphics"></param>
        public SetupGraphicsEventArgs(Graphics graphics)
        {
            Graphics = graphics ?? throw new ArgumentNullException(nameof(graphics));
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
        public Graphics Graphics { get; private set; }

        private DestroyGraphicsEventArgs()
        {

        }

        /// <summary>
        /// Initializes a new DestroyGraphicsEventArgs with a Graphics surface.
        /// </summary>
        /// <param name="graphics"></param>
        public DestroyGraphicsEventArgs(Graphics graphics)
        {
            Graphics = graphics ?? throw new ArgumentNullException(nameof(graphics));
        }
    }
}
