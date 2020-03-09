using System;

namespace GameOverlay.Drawing
{
	/// <summary>
	/// Provides data for the RecreateResources event.
	/// </summary>
	public class RecreateResourcesEventArgs : EventArgs
	{
		/// <summary>
		/// Gets the Graphics object associated with this event.
		/// </summary>
		public Graphics Graphics { get; }

		/// <summary>
		/// Initializes a new RecreateResourcesEventArgs using the given graphics object.
		/// </summary>
		/// <param name="graphics"></param>
		public RecreateResourcesEventArgs(Graphics graphics)
		{
			Graphics = graphics ?? throw new ArgumentNullException(nameof(graphics));
		}
	}
}
