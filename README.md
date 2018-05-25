# DirectXOverlay

Dependencies:
SharpDX.Direct2D1 (you may need to rebuild the packages)

Check the example (It's a console app)

# DPI Awareness

Copy paste this into your app.manifest

\<assembly xmlns="urn:schemas-microsoft-com:asm.v1" manifestVersion="1.0" xmlns:asmv3="urn:schemas-microsoft-com:asm.v3">
   \<asmv3:application>
      \<asmv3:windowsSettings xmlns="http://schemas.microsoft.com/SMI/2005/WindowsSettings">
         \<dpiAware>true</dpiAware>
      \</asmv3:windowsSettings>
   \</asmv3:application>
\</assembly>