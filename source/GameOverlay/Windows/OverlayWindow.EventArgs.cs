using System;

namespace GameOverlay.Windows
{
    /// <summary>
    /// Provides data for the VisibilityChanged event.
    /// </summary>
    public class OverlayVisibilityEventArgs : EventArgs
    {
        /// <summary>
        /// Gets a Boolean indicating the visibility of the window.
        /// </summary>
        public bool IsVisible { get; }

        private OverlayVisibilityEventArgs()
        {
        }

        /// <summary>
        /// Initializes a new OverlayVisibilityEventArgs using the given visibility.
        /// </summary>
        /// <param name="isVisible"></param>
        public OverlayVisibilityEventArgs(bool isVisible)
            => IsVisible = isVisible;
    }

    public class OverlayPropertyChangedEventArgs : EventArgs
	{
        /// <summary>
        /// Contains the name of the changed property. (case-sensitive)
        /// </summary>
        public string PropertyName { get; }

        /// <summary>
        /// Contains the new value of the property.
        /// </summary>
        public object Value { get; }

        private OverlayPropertyChangedEventArgs()
        {
        }

        /// <summary>
        /// Initializes a new OverlayPropertyChangedEventArgs using the given arguments.
        /// </summary>
        /// <param name="propertyName">The name of a property. (nameof(x))</param>
        /// <param name="value">The new value of the property.</param>
        public OverlayPropertyChangedEventArgs(string propertyName, object value)
		{
            PropertyName = propertyName;
            Value = value;
		}
    }

    /// <summary>
    /// Provides data for the PositionChanged event.
    /// </summary>
    public class OverlayPositionEventArgs : EventArgs
    {
        /// <summary>
        /// The new x-coordinate of the window.
        /// </summary>
        public int X { get; }

        /// <summary>
        /// The new y-coordinate of the window.
        /// </summary>
        public int Y { get; }

        private OverlayPositionEventArgs()
        {
        }

        /// <summary>
        /// Initializes a new OverlayPositionEventArgs using the given coordinates.
        /// </summary>
        /// <param name="x">The new x-coordinate of the window.</param>
        /// <param name="y">The new y-coordinate of the window.</param>
        public OverlayPositionEventArgs(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    /// <summary>
    /// Provides data for the SizeChanged event.
    /// </summary>
    public class OverlaySizeEventArgs : EventArgs
    {
        /// <summary>
        /// The new width of the window.
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// The new height of the window.
        /// </summary>
        public int Height { get; }

        private OverlaySizeEventArgs()
        {
        }

        /// <summary>
        /// Initializes a new OverlaySizeEventArgs using the given width and height.
        /// </summary>
        /// <param name="width">The new width of the window.</param>
        /// <param name="height">The new height of the window.</param>
        public OverlaySizeEventArgs(int width, int height)
        {
            Width = width;
            Height = height;
        }
    }
}
