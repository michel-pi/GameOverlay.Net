# DirectXOverlay

Dependencies:
SharpDX.Direct2D1 (you may need to rebuild the packages)

Check the example (It's a console app)

# DPI Awareness

Copy paste this into your app.manifest

<pre>
<application xmlns="urn:schemas-microsoft-com:asm.v3">
    <windowsSettings>
      <dpiAware xmlns="http://schemas.microsoft.com/SMI/2005/WindowsSettings">true</dpiAware>
	</windowsSettings>
</application>
</pre>

# How to include

To include this project into yours copy and paste the Yato.DirectXOverlay.Include.cs into your project.