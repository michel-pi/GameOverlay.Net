using System;
using System.Runtime.InteropServices;
using System.Threading;

using GameOverlay.PInvoke;

namespace GameOverlay.Windows
{
	/// <summary>
	/// Represents a transparent overlay window.
	/// </summary>
	public class OverlayWindow : IDisposable
	{
		private const WindowMessage CustomDestroyWindowMessage = (WindowMessage)0x1337;
		private const WindowMessage CustomRecreateWindowMessage = (WindowMessage)0x1338;

		private readonly object _lock;

		private volatile string _className;
		private volatile IntPtr _handle;
		private volatile int _height;
		private volatile bool _isInitialized;
		private volatile bool _isTopmost;
		private volatile bool _isVisible;
		private volatile string _title;
		private volatile int _width;
		private WindowProc _windowProc;
		private IntPtr _windowProcAddress;
		private Thread _windowThread;
		private volatile int _x;
		private volatile int _y;

		/// <summary>
		/// Gets or sets the windows class name.
		/// </summary>
		public string ClassName
		{
			get => _className;
			set
			{
				if (_isInitialized) throw new InvalidOperationException("OverlayWindow already running");

				_className = value;
			}
		}

		/// <summary>
		/// Gets the window handle of this instance.
		/// </summary>
		public IntPtr Handle { get => _handle; private set => _handle = value; }

		/// <summary>
		/// Gets or sets the height of the window.
		/// </summary>
		public int Height
		{
			get => _height;
			set
			{
				if (_isInitialized)
				{
					Resize(_width, value);
				}
				else
				{
					_height = value;
				}
			}
		}

		/// <summary>
		/// A Boolean indicating whether this instance is initialized.
		/// </summary>
		public bool IsInitialized => _isInitialized;

		/// <summary>
		/// Gets or sets a Boolean indicating whether this window is topmost.
		/// </summary>
		public bool IsTopmost
		{
			get => _isTopmost;
			set
			{
				if (_isInitialized)
				{
					if (value && !_isTopmost)
					{
						MakeTopmost();
					}
					else if (!value && _isTopmost)
					{
						RemoveTopmost();
					}
				}
				else
				{
					_isTopmost = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets a Boolean indicating whether this window is visible.
		/// </summary>
		public bool IsVisible
		{
			get => _isVisible;
			set
			{
				if (_isInitialized)
				{
					if (value && !_isVisible)
					{
						Show();
					}
					else if (!value && _isVisible)
					{
						Hide();
					}
				}
				else
				{
					_isVisible = value;
				}
			}
		}

		/// <summary>
		/// Gets the windows menu name.
		/// </summary>
		public string MenuName { get; private set; }

		/// <summary>
		/// Gets or sets the windows title.
		/// </summary>
		public string Title
		{
			get => _title;
			set
			{
				if (_isInitialized) throw new InvalidOperationException("OverlayWindow already running");

				_title = value;
			}
		}

		/// <summary>
		/// Gets or sets the width of the window.
		/// </summary>
		public int Width
		{
			get => _width;
			set
			{
				if (_isInitialized)
				{
					Resize(value, _height);
				}
				else
				{
					_width = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the x-coordinate of the window.
		/// </summary>
		public int X
		{
			get => _x;
			set
			{
				if (_isInitialized)
				{
					Move(value, _y);
				}
				else
				{
					_x = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the y-coordinate of the window.
		/// </summary>
		public int Y
		{
			get => _y;
			set
			{
				if (_isInitialized)
				{
					Move(_x, value);
				}
				else
				{
					_y = value;
				}
			}
		}

		/// <summary>
		/// Fires when the postion of the window has changed.
		/// </summary>
		public event EventHandler<OverlayPositionEventArgs> PositionChanged;

		/// <summary>
		/// Fires when the size of the window has changed.
		/// </summary>
		public event EventHandler<OverlaySizeEventArgs> SizeChanged;

		/// <summary>
		/// Fires when the visibility of the window has changed.
		/// </summary>
		public event EventHandler<OverlayVisibilityEventArgs> VisibilityChanged;

		/// <summary>
		/// Initializes a new OverlayWindow.
		/// </summary>
		public OverlayWindow()
		{
			_lock = new object();

			Title = string.Empty;
			ClassName = string.Empty;

			Width = 800;
			Height = 600;

			IsVisible = true;
		}

		/// <summary>
		/// Initializes a new OverlayWindow using the given postion and size.
		/// </summary>
		/// <param name="x">The x-coordinate of the window.</param>
		/// <param name="y">The y-coordinate of the window.</param>
		/// <param name="width">The width of the window.</param>
		/// <param name="height">The height of the window.</param>
		public OverlayWindow(int x, int y, int width, int height) : this()
		{
			X = x;
			Y = y;
			Width = width;
			Height = height;
		}

		/// <summary>
		/// Allows an object to try to free resources and perform other cleanup operations before it is reclaimed by garbage collection.
		/// </summary>
		~OverlayWindow() => Dispose(false);

		private void DestroyWindow()
		{
			lock (_lock)
			{
				if (!_isInitialized) throw new InvalidOperationException("OverlayWindow is not initialized");

				User32.PostMessage(Handle, CustomDestroyWindowMessage, IntPtr.Zero, IntPtr.Zero);
			}
		}

		private void InstantiateNewWindow()
		{
			var extendedWindowStyle = ExtendedWindowStyle.Transparent | ExtendedWindowStyle.Layered | ExtendedWindowStyle.NoActivate;
			if (_isTopmost) extendedWindowStyle |= ExtendedWindowStyle.Topmost;

			var windowStyle = WindowStyle.Popup;
			if (_isVisible) windowStyle |= WindowStyle.Visible;

			_handle = User32.CreateWindowEx(
				extendedWindowStyle,
				_className,
				_title,
				windowStyle,
				_x, _y,
				_width, _height,
				IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);

			User32.SetLayeredWindowAttributes(_handle, 0, 255, LayeredWindowAttributes.Alpha);
			User32.UpdateWindow(_handle);

			if (_isVisible) WindowHelper.ExtendFrameIntoClientArea(_handle);
		}

		private void MakeTopmost()
		{
			if (_isTopmost) return;

			WindowHelper.MakeTopmost(_handle);

			_isTopmost = true;

			if (!_isVisible)
			{
				WindowHelper.ExtendFrameIntoClientArea(_handle);

				_isVisible = true;
				OnVisibilityChanged(true);
			}
		}

		private void RemoveTopmost()
		{
			if (!_isTopmost) return;

			WindowHelper.RemoveTopmost(_handle);

			_isTopmost = false;

			if (_isVisible)
			{
				WindowHelper.ExtendFrameIntoClientArea(_handle);
			}
		}

		private void SetupWindow()
		{
			// generate a random title if it's null (invalid)
			if (_title == null) _title = WindowHelper.GenerateRandomTitle();
			if (string.IsNullOrEmpty(MenuName)) MenuName = WindowHelper.GenerateRandomTitle();

			// if no class name is given then generate a "unique" one
			if (string.IsNullOrEmpty(_className)) _className = WindowHelper.GenerateRandomClass();

			// prepare window procedure
			_windowProc = WindowProcedure;
			_windowProcAddress = Marshal.GetFunctionPointerForDelegate(_windowProc);

			// try to register our class
			while (true)
			{
				var wndClassEx = new WindowClassEx
				{
					Size = WindowClassEx.MemorySize,
					Style = 0,
					WindowProc = _windowProcAddress,
					ClsExtra = 0,
					WindowExtra = 0,
					Instance = IntPtr.Zero,
					Icon = IntPtr.Zero,
					Curser = IntPtr.Zero,
					Background = IntPtr.Zero,
					MenuName = MenuName,
					ClassName = _className,
					IconSm = IntPtr.Zero
				};

				if (User32.RegisterClassEx(ref wndClassEx) != 0)
				{
					break;
				}
				else
				{
					// already taken name?
					_className = WindowHelper.GenerateRandomClass();
				}
			}

			InstantiateNewWindow();
		}

		private IntPtr WindowProcedure(IntPtr hwnd, WindowMessage msg, IntPtr wParam, IntPtr lParam)
		{
			switch (msg)
			{
				case WindowMessage.EraseBackground:
					User32.SendMessage(hwnd, WindowMessage.Paint, (IntPtr)0, (IntPtr)0);
					break;

				case WindowMessage.Keyup:
				case WindowMessage.Keydown:
				case WindowMessage.Syscommand:
				case WindowMessage.Syskeydown:
				case WindowMessage.Syskeyup:
					return (IntPtr)0;

				case WindowMessage.NcPaint:
				case WindowMessage.Paint:
					return (IntPtr)0;

				case WindowMessage.DwmCompositionChanged:
					WindowHelper.ExtendFrameIntoClientArea(hwnd);
					return (IntPtr)0;

				case WindowMessage.DpiChanged:
					return (IntPtr)0; // block DPI changed message

				default: break;
			}

			return User32.DefWindowProc(hwnd, msg, wParam, lParam);
		}

		private void WindowThread()
		{
			User32.MakeThreadDpiAware();

			SetupWindow();

			_isInitialized = true;

			int supressDestroyMessages = 0;

			while (true)
			{
				User32.WaitMessage();

				var message = default(Message);

				if (User32.PeekMessage(ref message, _handle, 0, 0, 1))
				{
					switch (message.Msg)
					{
						//case WindowMessage.Quit:
						//    continue; // TODO: test
						case CustomDestroyWindowMessage:
							User32.DestroyWindow(_handle);
							break;

						case CustomRecreateWindowMessage:
							supressDestroyMessages = 2;

							User32.DestroyWindow(_handle);

							InstantiateNewWindow();

							break;

						default: break;
					}

					User32.TranslateMessage(ref message);
					User32.DispatchMessage(ref message);

					if (message.Msg == WindowMessage.Destroy || message.Msg == WindowMessage.Ncdestroy)
					{
						if (supressDestroyMessages == 0)
						{
							break;
						}
						else
						{
							supressDestroyMessages--;
						}
					}
				}
			}

			User32.UnregisterClass(_className, IntPtr.Zero);

			_isInitialized = false;

			_handle = IntPtr.Zero;

			IsVisible = false;
			IsTopmost = false;
		}

		/// <summary>
		/// Gets called whenever the position of the window changes.
		/// </summary>
		/// <param name="x">The new x-coordinate of the window.</param>
		/// <param name="y">The new y-coordinate of the window.</param>
		protected virtual void OnPositionChanged(int x, int y)
		{
			PositionChanged?.Invoke(this, new OverlayPositionEventArgs(x, y));
		}

		/// <summary>
		/// Gets called whenever the size of the window changes.
		/// </summary>
		/// <param name="width">The new width of the window.</param>
		/// <param name="height">The new height of the window.</param>
		protected virtual void OnSizeChanged(int width, int height)
		{
			SizeChanged?.Invoke(this, new OverlaySizeEventArgs(width, height));
		}

		/// <summary>
		/// Gets called whenever the visibility of the window changes.
		/// </summary>
		/// <param name="isVisible">A Boolean indicating the new visibility of the window.</param>
		protected virtual void OnVisibilityChanged(bool isVisible)
		{
			VisibilityChanged?.Invoke(this, new OverlayVisibilityEventArgs(isVisible));
		}

		/// <summary>
		/// Returns a value indicating whether two specified instances of OverlayWindow represent the same value.
		/// </summary>
		/// <param name="left">The first object to compare.</param>
		/// <param name="right">The second object to compare.</param>
		/// <returns>
		/// <see langword="true"/> if <paramref name="left"/> and <paramref name="right"/> are equal; otherwise, <see langword="false"/>.
		/// </returns>
		public static bool Equals(OverlayWindow left, OverlayWindow right)
		{
			return left?.Equals(right) == true;
		}

		/// <summary>
		/// Setup and initializes the window.
		/// </summary>
		public void CreateWindow()
		{
			lock (_lock)
			{
				if (_isInitialized) throw new InvalidOperationException("OverlayWindow is already initialized");

				_windowThread = new Thread(WindowThread)
				{
					IsBackground = true
				};

				_windowThread.SetApartmentState(ApartmentState.STA);

				_windowThread.Start();

				while (!_isInitialized) Thread.Sleep(10);
			}
		}

		/// <summary>
		/// Returns a value indicating whether this instance and a specified <see cref="T:System.Object"/> represent the same type and value.
		/// </summary>
		/// <param name="obj">The object to compare with this instance.</param>
		/// <returns>
		/// <see langword="true"/> if <paramref name="obj"/> is a OverlayWindow and equal to this instance; otherwise, <see langword="false"/>.
		/// </returns>
		public override bool Equals(object obj)
		{
			if (obj is OverlayWindow value)
			{
				return value.IsInitialized == IsInitialized
					&& value.Handle == Handle;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Returns a value indicating whether two specified instances of OverlayWindow represent the same value.
		/// </summary>
		/// <param name="value">An object to compare to this instance.</param>
		/// <returns><see langword="true"/> if <paramref name="value"/> is equal to this instance; otherwise, <see langword="false"/>.</returns>
		public bool Equals(OverlayWindow value)
		{
			return value != null
				&& value.IsInitialized == IsInitialized
				&& value.Handle == Handle;
		}

		/// <summary>
		/// Adapts to another window in the postion and size.
		/// </summary>
		/// <param name="windowHandle">The target window handle.</param>
		/// <param name="attachToClientArea">A Boolean determining whether to fit to the client area of the target window.</param>
		public void FitToWindow(IntPtr windowHandle, bool attachToClientArea = false)
		{
			bool result = attachToClientArea ? WindowHelper.GetWindowClientBounds(windowHandle, out WindowBounds rect) : WindowHelper.GetWindowBounds(windowHandle, out rect);

			if (result)
			{
				int x = rect.Left;
				int y = rect.Top;
				int width = rect.Right - rect.Left;
				int height = rect.Bottom - rect.Top;

				if (X != x
					|| Y != y
					|| Width != width
					|| Height != height)
				{
					Resize(x, y, width, height);
				}
			}
		}

		/// <summary>
		/// Returns the hash code for this instance.
		/// </summary>
		/// <returns>A 32-bit signed integer hash code.</returns>
		public override int GetHashCode()
		{
			return OverrideHelper.HashCodes(
				IsInitialized.GetHashCode(),
				Handle.GetHashCode());
		}

		/// <summary>
		/// Makes the window invisible.
		/// </summary>
		public void Hide()
		{
			if (!_isInitialized) throw new InvalidOperationException("OverlayWindow not initialized");

			User32.ShowWindow(_handle, ShowWindowCommand.Hide);

			_isVisible = false;

			OnVisibilityChanged(false);
		}

		/// <summary>
		/// Waits until the Thread used by this instance has exited.
		/// </summary>
		public void JoinWindowThread()
		{
			try
			{
				_windowThread.Join();
			}
			catch { }
		}

		/// <summary>
		/// Changes the position of the window using the given coordinates.
		/// </summary>
		/// <param name="x">The new x-coordinate of the window.</param>
		/// <param name="y">The new y-coordinate of the window.</param>
		public void Move(int x, int y)
		{
			if (!_isInitialized) throw new InvalidOperationException("OverlayWindow not initialized");

			User32.MoveWindow(_handle, x, y, _width, _height, true);

			_x = x;
			_y = y;

			WindowHelper.ExtendFrameIntoClientArea(_handle);

			OnPositionChanged(x, y);
		}

		/// <summary>
		/// Places the OverlayWindow above the target window according to the windows z-order.
		/// </summary>
		/// <param name="windowHandle">The target window handle.</param>
		public void PlaceAboveWindow(IntPtr windowHandle)
		{
			var windowAboveParentWindow = User32.GetWindow(windowHandle, WindowCommand.Previous);

			if (windowAboveParentWindow != Handle)
			{
				User32.SetWindowPos(
					Handle,
					windowAboveParentWindow,
					0, 0, 0, 0,
					SwpFlags.NoActivate | SwpFlags.NoMove | SwpFlags.NoSize | SwpFlags.AsyncWindowPos);
			}
		}

		/// <summary>
		/// Destroys the current window and creates a new one using the same attributes.
		/// </summary>
		public void RecreateWindow()
		{
			lock (_lock)
			{
				if (!_isInitialized) throw new InvalidOperationException("OverlayWindow is not initialized");

				var previousHandle = Handle;

				User32.PostMessage(Handle, CustomRecreateWindowMessage, IntPtr.Zero, IntPtr.Zero);

				while (previousHandle == Handle) Thread.Sleep(10);
			}
		}

		/// <summary>
		/// Changes the size of the window using the given width and height.
		/// </summary>
		/// <param name="width">The new width of the window.</param>
		/// <param name="height">The new height of the window.</param>
		public void Resize(int width, int height)
		{
			if (!_isInitialized) throw new InvalidOperationException("OverlayWindow not initialized");

			User32.MoveWindow(_handle, _x, _y, width, height, true);

			_width = width;
			_height = height;

			WindowHelper.ExtendFrameIntoClientArea(_handle);

			OnSizeChanged(width, height);
		}

		/// <summary>
		/// Changes the size of the window using the given dimension.
		/// </summary>
		/// <param name="x">The new x-coordinate of the window.</param>
		/// <param name="y">The new y-coordinate of the window.</param>
		/// <param name="width">The new width of the window.</param>
		/// <param name="height">The new height of the window.</param>
		public void Resize(int x, int y, int width, int height)
		{
			if (!_isInitialized) throw new InvalidOperationException("OverlayWindow not initialized");

			User32.MoveWindow(_handle, x, y, width, height, true);

			_x = x;
			_y = y;

			_width = width;
			_height = height;

			WindowHelper.ExtendFrameIntoClientArea(_handle);

			OnPositionChanged(x, y);
			OnSizeChanged(width, height);
		}

		/// <summary>
		/// Makes the window visible.
		/// </summary>
		public void Show()
		{
			if (!_isInitialized) throw new InvalidOperationException("OverlayWindow not initialized");

			User32.ShowWindow(_handle, ShowWindowCommand.Show);

			_isVisible = true;

			WindowHelper.ExtendFrameIntoClientArea(_handle);

			OnVisibilityChanged(true);
		}

		/// <summary>
		/// Converts this OverlayWindow structure to a human-readable string.
		/// </summary>
		/// <returns>A string representation of this OverlayWindow.</returns>
		public override string ToString()
		{
			return OverrideHelper.ToString(
				"Handle", Handle.ToString("X"),
				"IsInitialized", IsInitialized.ToString(),
				"IsVisible", IsVisible.ToString(),
				"IsTopmost", IsTopmost.ToString(),
				"X", X.ToString(),
				"Y", Y.ToString(),
				"Width", Width.ToString(),
				"Height", Height.ToString());
		}

		#region IDisposable Support

		private bool disposedValue = false;

		/// <summary>
		/// Releases all resources used by this OverlayWindow.
		/// </summary>
		/// <param name="disposing">A Boolean value indicating whether this is called from the destructor.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (_isInitialized)
				{
					DestroyWindow();
				}

				SizeChanged = null;
				PositionChanged = null;
				VisibilityChanged = null;

				_windowThread = null;
				_windowProc = null;

				_title = null;
				_className = null;

				disposedValue = true;
			}
		}

		/// <summary>
		/// Releases all resources used by this OverlayWindow.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion IDisposable Support
	}
}
