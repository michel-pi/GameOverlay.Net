# GameOverlay.Net

[![GameOverlay.Net on NuGet](https://buildstats.info/nuget/GameOverlay.Net)](https://nuget.org/packages/GameOverlay.Net)

This library offers a comprehensive interface for drawing hardware accelerated graphics using our [Direct2D1 renderer](https://github.com/michel-pi/GameOverlay.Net/blob/master/source/Drawing/Graphics.cs "Direct2D1 renderer") and creating transparent click-through windows.

![A running Overlay Window](https://raw.githubusercontent.com/michel-pi/GameOverlay.Net/master/example_picture.png)

### NuGet

    Install-Package GameOverlay.Net

# Features

- Supports Windows 7, 8, 8.1 and 10
- Hardware accelerated
- Create transparent overlay windows
- Make your overlay stick to a parent window
- Draw Text, Lines, Rectangles, Circles, Triangles...
- Load and Draw images (.bmp, .png, .jpg)
- Multithreaded rendering
- Already implemented render loop with fps limitation

### [Example](https://github.com/michel-pi/GameOverlay.Net/tree/master/example)
### [Documentation](https://michel-pi.github.io/GameOverlay.Net/)

# Contribute

The project file was generated using Visual Studio 2017.

Clone or download the repository and update/install the required NuGet packages.

You can help by reporting issues, adding new features, fixing bugs and by providing a better documentation.  

### Dependencies

    SharpDX.Direct2D1, SharpDX.DXGI, SharpDX

# Donate

Do you like this project and want to help me to keep working on it?

Then maybe consider to donate any amount you like.

[![](https://www.paypalobjects.com/en_US/i/btn/btn_donateCC_LG.gif)](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=YJDWMDUSM8KKQ)

```
BTC     14ES7f4GB3vD1C8Faz6ywqTcdDevxZoMyY

ETH     0xd9E2CB12d310E7BF5E72F591D7A2b8820adced04
```

# License

- [GameOverlay.Net License](https://github.com/michel-pi/GameOverlay.Net/blob/master/LICENSE "GameOverlay.Net License")
- [SharpDX License](https://github.com/sharpdx/SharpDX/blob/master/LICENSE "SharpDX License")

# Special Thanks

- [BigMo (Zat)](https://github.com/BigMo "BigMo (Zat)") for his [SharpDXRenderer](https://github.com/BigMo/ExternalUtilsCSharp/tree/master/ExternalUtilsCSharp.SharpDXRenderer "SharpDXRenderer")
- [ReactiioN](https://github.com/ReactiioN1337 "ReactiioN") for his [C++ aero-overlay](https://github.com/ReactiioN1337/aero-overlay "C++ aero-overlay")
- [SharpDX](http://sharpdx.org/ "SharpDX") for their open source DirectX wrapper
