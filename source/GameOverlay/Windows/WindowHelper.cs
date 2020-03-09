using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

using GameOverlay.PInvoke;

namespace GameOverlay.Windows
{
	/// <summary>
	/// Provides methods to interact with windows.
	/// </summary>
	public static class WindowHelper
	{
		private const int MaxRandomStringLen = 16;
		private const int MinRandomStringLen = 8;

		private static readonly Lazy<IntPtr> _accentPolicyBuffer = new Lazy<IntPtr>(() =>
		{
			var buffer = Marshal.AllocHGlobal((int)AccentPolicy.MemorySize);

			var policy = new AccentPolicy()
			{
				AccentFlags = 2,
				AccentState = AccentState.EnableBlurBehind,
				AnimationId = 0,
				GradientColor = 0
			};

			Marshal.StructureToPtr(policy, buffer, true);

			return buffer;
		}, LazyThreadSafetyMode.ExecutionAndPublication);

		private static readonly object _blacklistLock = new object();
		private static readonly Random _random = new Random();
		private static readonly List<string> _windowClassesBlacklist = new List<string>();

		private static string GenerateRandomAsciiString(int minLength, int maxLength)
		{
			int length = _random.Next(minLength, maxLength);

			char[] chars = new char[length];

			for (int i = 0; i < chars.Length; i++)
			{
				chars[i] = (char)_random.Next(97, 123); // ascii range for small letters
			}

			return new string(chars);
		}

		private static bool GetWindowClientInternal(IntPtr hwnd, out NativeRect rect)
		{
			// calculates the window bounds based on the difference of the client and the window rect

			if (!User32.GetWindowRect(hwnd, out rect)) return false;
			if (!User32.GetClientRect(hwnd, out var clientRect)) return true;

			int clientWidth = clientRect.Right - clientRect.Left;
			int clientHeight = clientRect.Bottom - clientRect.Top;

			int windowWidth = rect.Right - rect.Left;
			int windowHeight = rect.Bottom - rect.Top;

			if (clientWidth == windowWidth && clientHeight == windowHeight) return true;

			if (clientWidth != windowWidth)
			{
				int difX = clientWidth > windowWidth ? clientWidth - windowWidth : windowWidth - clientWidth;
				difX /= 2;

				rect.Right -= difX;
				rect.Left += difX;

				if (clientHeight != windowHeight)
				{
					int difY = clientHeight > windowHeight ? clientHeight - windowHeight : windowHeight - clientHeight;

					rect.Top += difY - difX;
					rect.Bottom -= difX;
				}
			}
			else if (clientHeight != windowHeight)
			{
				int difY = clientHeight > windowHeight ? clientHeight - windowHeight : windowHeight - clientHeight;
				difY /= 2;

				rect.Bottom -= difY;
				rect.Top += difY;
			}

			return true;
		}

		/// <summary>
		/// Enables the blur effect for a window and makes it translucent.
		/// </summary>
		/// <param name="hwnd">A valid handle to a window. The desktop window is not supported.</param>
		public static void EnableBlurBehind(IntPtr hwnd)
		{
			if (hwnd == IntPtr.Zero) return;

			var data = new WindowCompositionAttributeData()
			{
				Attribute = (uint)WindowCompositionAttribute.AccentPolicy,
				Data = _accentPolicyBuffer.Value,
				DataSize = AccentPolicy.MemorySize
			};

			Undocumented.SetWindowCompositionAttribute(hwnd, ref data);
		}

		/// <summary>
		/// Extends a windows frame into the client area of the window.
		/// </summary>
		/// <param name="hwnd">A IntPtr representing the handle of a window.</param>
		public static void ExtendFrameIntoClientArea(IntPtr hwnd)
		{
			var margin = new NativeMargin
			{
				cxLeftWidth = -1,
				cxRightWidth = -1,
				cyBottomHeight = -1,
				cyTopHeight = -1
			};

			DwmApi.DwmExtendFrameIntoClientArea(hwnd, ref margin);
		}

		/// <summary>
		/// Searches for the first child window matching the search criterias.
		/// </summary>
		/// <param name="parentWindow">A window handle.</param>
		/// <param name="childWindowName">The window title of the child window. Can be null.</param>
		/// <param name="childClassName">The window class of the child window. Can be null.</param>
		/// <param name="childAfter">
		/// A handle to a child window. The search begins with the next child window in the Z order. The child window must be a direct child window of
		/// hwndParent, not just a descendant window.
		/// </param>
		/// <returns>Returns the matching window handle or IntPtr.Zero if none matches.</returns>
		public static IntPtr FindChildWindow(IntPtr parentWindow, string childWindowName = null, string childClassName = null, IntPtr childAfter = default)
		{
			if (string.IsNullOrEmpty(childWindowName)) childWindowName = null;
			if (string.IsNullOrEmpty(childClassName)) childClassName = null;

			return User32.FindWindowEx(parentWindow, childAfter, childClassName, childWindowName);
		}

		/// <summary>
		/// Searches for the first window matching the given parameters.
		/// </summary>
		/// <param name="title">The window name. Can be null.</param>
		/// <param name="className">The windows class name. Can be null.</param>
		/// <returns>Returns the matching window handle or IntPtr.Zero if none matches.</returns>
		public static IntPtr FindWindow(string title, string className = null)
		{
			return string.IsNullOrEmpty(className)
				? User32.FindWindowEx(IntPtr.Zero, IntPtr.Zero, null, title)
				: User32.FindWindowEx(IntPtr.Zero, IntPtr.Zero, className, title);
		}

		/// <summary>
		/// Generates a random window class name.
		/// </summary>
		/// <returns>The string this method creates.</returns>
		public static string GenerateRandomClass()
		{
			lock (_blacklistLock)
			{
				while (true)
				{
					string name = GenerateRandomAsciiString(MinRandomStringLen, MaxRandomStringLen);

					if (!_windowClassesBlacklist.Contains(name))
					{
						_windowClassesBlacklist.Add(name);

						return name;
					}
				}
			}
		}

		/// <summary>
		/// Generates a random window title.
		/// </summary>
		/// <returns>The string this method creates.</returns>
		public static string GenerateRandomTitle()
		{
			return GenerateRandomAsciiString(MinRandomStringLen, MaxRandomStringLen);
		}

		/// <summary>
		/// Retrieves the window handle to the active window attached to the calling thread's message queue.
		/// </summary>
		/// <returns>
		/// The return value is the handle to the active window attached to the calling thread's message queue. Otherwise, the return value is NULL.
		/// </returns>
		public static IntPtr GetActiveWindow() => User32.GetActiveWindow();

		/// <summary>
		/// Retrieves the specified value from the WNDCLASSEX structure associated with the specified window.
		/// </summary>
		/// <param name="handle">A window handle.</param>
		/// <param name="index">The index can be a byte offset or one of the defined constants.</param>
		/// <returns>If the function succeeds, the return value is the requested value.</returns>
		public static long GetClassLong(IntPtr handle, int index) => User32.GetClassLongPtr(handle, index).ToInt64();

		/// <summary>
		/// Retrieves a handle to the desktop window. The desktop window covers the entire screen. The desktop window is the area on top of which
		/// other windows are painted.
		/// </summary>
		/// <returns>The return value is a handle to the desktop window.</returns>
		public static IntPtr GetDesktopWindow() => User32.GetDesktopWindow();

		/// <summary>
		/// Returns the first child window of the specified parent window if it has one.
		/// </summary>
		/// <param name="hwnd">A window handle. IntPtr.Zero for the desktop window.</param>
		/// <returns>
		/// If the function succeeds, the return value is a window handle. If no window exists with the specified relationship to the specified
		/// window, the return value is NULL.
		/// </returns>
		public static IntPtr GetFirstChildWindow(IntPtr hwnd) => User32.GetWindow(hwnd, WindowCommand.Child);

		/// <summary>
		/// Returns the window with the highest position in the Z order relative (Topmost or not) to the given handle.
		/// </summary>
		/// <param name="hwnd">A window handle. IntPtr.Zero for the desktop window.</param>
		/// <returns>
		/// If the function succeeds, the return value is a window handle. If no window exists with the specified relationship to the specified
		/// window, the return value is NULL.
		/// </returns>
		public static IntPtr GetFirstWindow(IntPtr hwnd) => User32.GetWindow(hwnd, WindowCommand.First);

		/// <summary>
		/// Retrieves a handle to the foreground window (the window with which the user is currently working). The system assigns a slightly higher
		/// priority to the thread that creates the foreground window than it does to other threads.
		/// </summary>
		/// <returns>
		/// The return value is a handle to the foreground window. The foreground window can be NULL in certain circumstances, such as when a window
		/// is losing activation.
		/// </returns>
		public static IntPtr GetForegroundWindow() => User32.GetForegroundWindow();

		/// <summary>
		/// Returns the window with the lowest position in the Z order relative (Topmost or not) to the given handle.
		/// </summary>
		/// <param name="hwnd">A window handle. IntPtr.Zero for the desktop window.</param>
		/// <returns>
		/// If the function succeeds, the return value is a window handle. If no window exists with the specified relationship to the specified
		/// window, the return value is NULL.
		/// </returns>
		public static IntPtr GetLastWindow(IntPtr hwnd) => User32.GetWindow(hwnd, WindowCommand.Last);

		/// <summary>
		/// Returns the window below the specified window in the Z order.
		/// </summary>
		/// <param name="hwnd">A window handle. IntPtr.Zero for the desktop window.</param>
		/// <returns>
		/// If the function succeeds, the return value is a window handle. If no window exists with the specified relationship to the specified
		/// window, the return value is NULL.
		/// </returns>
		public static IntPtr GetNextWindow(IntPtr hwnd) => User32.GetWindow(hwnd, WindowCommand.Next);

		/// <summary>
		/// Returns the owner window of the specified window if it exists.
		/// </summary>
		/// <param name="hwnd">A window handle. IntPtr.Zero for the desktop window.</param>
		/// <returns>
		/// If the function succeeds, the return value is a window handle. If no window exists with the specified relationship to the specified
		/// window, the return value is NULL.
		/// </returns>
		public static IntPtr GetOwnerWindow(IntPtr hwnd) => User32.GetWindow(hwnd, WindowCommand.Owner);

		/// <summary>
		/// Returns the window above the specified window in the Z order.
		/// </summary>
		/// <param name="hwnd">A window handle. IntPtr.Zero for the desktop window.</param>
		/// <returns>
		/// If the function succeeds, the return value is a window handle. If no window exists with the specified relationship to the specified
		/// window, the return value is NULL.
		/// </returns>
		public static IntPtr GetPreviousWindow(IntPtr hwnd) => User32.GetWindow(hwnd, WindowCommand.Previous);

		/// <summary>
		/// Retrieves the identifier of the process that created the window.
		/// </summary>
		/// <param name="handle">A handle to the window.</param>
		/// <returns>The return value is the identifier of the process that created the window.</returns>
		public static int GetProcessIdFromWindow(IntPtr handle)
		{
			uint id = 0u;

			User32.GetWindowThreadProcessId(handle, ref id);

			return (int)id;
		}

		/// <summary>
		/// Retrieves a handle to the Shell's desktop window.
		/// </summary>
		/// <returns>The return value is the handle of the Shell's desktop window. If no Shell process is present, the return value is NULL.</returns>
		public static IntPtr GetShellWindow() => User32.GetShellWindow();

		/// <summary>
		/// Returns the boundaries of a window.
		/// </summary>
		/// <param name="hwnd">A IntPtr representing the handle of a window.</param>
		/// <param name="bounds">A WindowBounds structure representing the boundaries of a window.</param>
		/// <returns></returns>
		public static bool GetWindowBounds(IntPtr hwnd, out WindowBounds bounds)
		{
			if (User32.GetWindowRect(hwnd, out var rect))
			{
				bounds = new WindowBounds()
				{
					Left = rect.Left,
					Top = rect.Top,
					Right = rect.Right,
					Bottom = rect.Bottom
				};

				return true;
			}
			else
			{
				bounds = default;

				return false;
			}
		}

		/// <summary>
		/// Returns the boundaries of a windows client area.
		/// </summary>
		/// <param name="hwnd">A IntPtr representing the handle of a window.</param>
		/// <param name="bounds">A WindowBounds structure representing the boundaries of a window.</param>
		/// <returns></returns>
		public static bool GetWindowClientBounds(IntPtr hwnd, out WindowBounds bounds)
		{
			if (GetWindowClientInternal(hwnd, out var rect))
			{
				bounds = new WindowBounds()
				{
					Left = rect.Left,
					Top = rect.Top,
					Right = rect.Right,
					Bottom = rect.Bottom
				};

				return true;
			}
			else
			{
				bounds = default;

				return false;
			}
		}

		/// <summary>
		/// Returns the size of the client area of the window possibly with borders.
		/// </summary>
		/// <param name="hwnd">A IntPtr representing the handle of a window.</param>
		/// <param name="bounds">A WindowBounds structure representing the boundaries of a window.</param>
		/// <returns></returns>
		public static bool GetWindowClientBoundsExtra(IntPtr hwnd, out WindowBounds bounds)
		{
			if (User32.GetClientRect(hwnd, out var rect))
			{
				bounds = new WindowBounds()
				{
					Left = rect.Left,
					Top = rect.Top,
					Right = rect.Right,
					Bottom = rect.Bottom
				};

				return true;
			}
			else
			{
				bounds = default;

				return false;
			}
		}

		/// <summary>
		/// Retrieves information about the specified window. The function also retrieves the value at a specified offset into the extra window
		/// memory.
		/// </summary>
		/// <param name="handle">A window handle.</param>
		/// <param name="index">The index can be a byte offset or one of the defined constants.</param>
		/// <returns>If the function succeeds, the return value is the requested value.</returns>
		public static long GetWindowLong(IntPtr handle, int index) => User32.GetWindowLongPtr(handle, index).ToInt64();

		/// <summary>
		/// Retrieves the style and extended style of a given window.
		/// </summary>
		/// <param name="handle">A window handle.</param>
		/// <param name="style">Contains the window style on success.</param>
		/// <param name="extendedStyle">Contains the extended window style on success.</param>
		/// <returns>Returns true if the function succeeds.</returns>
		public static bool GetWindowStyle(IntPtr handle, out uint style, out uint extendedStyle)
		{
			var info = new WindowInfo()
			{
				Size = WindowInfo.MemorySize
			};

			if (User32.GetWindowInfo(handle, ref info))
			{
				style = (uint)info.Style;
				extendedStyle = (uint)info.ExtendedStyle;

				return true;
			}
			else
			{
				style = default;
				extendedStyle = default;

				return false;
			}
		}

		/// <summary>
		/// Determines whether the specified window handle identifies an existing window.
		/// </summary>
		/// <param name="handle">A handle to the window to be tested.</param>
		/// <returns>Returns true when the given handle identifies an existing window.</returns>
		public static bool IsWindow(IntPtr handle) => User32.IsWindow(handle);

		/// <summary>
		/// Determines the visibility state of the specified window.
		/// </summary>
		/// <param name="hwnd">A handle to the window to be tested.</param>
		/// <returns>
		/// If the specified window, its parent window, its parent's parent window, and so forth, have the WS_VISIBLE style, the return value is
		/// nonzero. Otherwise, the return value is zero.
		/// </returns>
		public static bool IsWindowVisible(IntPtr hwnd) => User32.IsWindowVisible(hwnd);

		/// <summary>
		/// Adds the topmost flag to a window.
		/// </summary>
		/// <param name="hwnd">A IntPtr representing the handle of a window.</param>
		public static void MakeTopmost(IntPtr hwnd) => User32.SetWindowPos(hwnd, User32.HwndInsertTopMost, 0, 0, 0, 0, SwpFlags.ShowWindow | SwpFlags.NoActivate | SwpFlags.NoMove | SwpFlags.NoSize);

		/// <summary>
		/// Removes the topmost flag from a window.
		/// </summary>
		/// <param name="hwnd">A IntPtr representing the handle of a window.</param>
		public static void RemoveTopmost(IntPtr hwnd) => User32.SetWindowPos(hwnd, User32.HwndInsertNoTopmost, 0, 0, 0, 0, SwpFlags.NoActivate | SwpFlags.NoMove | SwpFlags.NoSize);
	}
}
