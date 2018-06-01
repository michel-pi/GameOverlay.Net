# DirectXOverlay

Dependencies: SharpDX.Direct2D1, SharpDX.DXGI, SharpDX.

Example:
Compile in Debug Mode as Consol Application. It will execute the "Program.cs".

# DPI Awareness

If you don't want your drawing to be scaled with DPI or the monitors scale factor.

Copy paste this into your app.manifest.

\<assembly xmlns="urn:schemas-microsoft-com:asm.v1" manifestVersion="1.0" xmlns:asmv3="urn:schemas-microsoft-com:asm.v3">
   \<asmv3:application>
      \<asmv3:windowsSettings xmlns="http://schemas.microsoft.com/SMI/2005/WindowsSettings">
         \<dpiAware>true</dpiAware>
      \</asmv3:windowsSettings>
   \</asmv3:application>
\</assembly>