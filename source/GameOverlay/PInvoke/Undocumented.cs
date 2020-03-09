using System;
using System.Runtime.InteropServices;

namespace GameOverlay.PInvoke
{
	internal static class Undocumented
	{
		[return: MarshalAs(UnmanagedType.Bool)]
		public delegate bool SetWindowCompositionAttributeDelegate(IntPtr hwnd, [In, Out] ref WindowCompositionAttributeData data);

		public static readonly SetWindowCompositionAttributeDelegate SetWindowCompositionAttribute;

		static Undocumented()
		{
			IntPtr library = DynamicImport.ImportLibrary("user32.dll");

			SetWindowCompositionAttribute = DynamicImport.Import<SetWindowCompositionAttributeDelegate>(library, "SetWindowCompositionAttribute");
		}
	}
}
