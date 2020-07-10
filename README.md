<div align="center">

# GameOverlay.Net

[![Nuget](https://img.shields.io/nuget/v/GameOverlay.Net.svg?logo=nuget)](https://www.nuget.org/packages/GameOverlay.Net/ "GameOverlay.Net on NuGet") [![Nuget](https://img.shields.io/nuget/dt/GameOverlay.Net.svg)](https://www.nuget.org/packages/GameOverlay.Net/ "Downloads on NuGet") [![Open issues](https://img.shields.io/github/issues-raw/michel-pi/GameOverlay.Net.svg?logo=github)](https://github.com/michel-pi/GameOverlay.Net/issues "Open issues on Github") [![Closed issues](https://img.shields.io/github/issues-closed-raw/michel-pi/GameOverlay.Net.svg)](https://github.com/michel-pi/GameOverlay.Net/issues?q=is%3Aissue+is%3Aclosed "Closed issues on Github") [![MIT License](https://img.shields.io/github/license/michel-pi/GameOverlay.Net.svg)](https://github.com/michel-pi/GameOverlay.Net/blob/master/LICENSE "GameOverlay.Net license")

![Net Framework 4.0](https://img.shields.io/badge/.Net-4.0-informational.svg) ![Net Framework 4.5](https://img.shields.io/badge/.Net-4.5-informational.svg) ![Net Framework 4.7](https://img.shields.io/badge/.Net-4.7-informational.svg) ![Net Standard 2.0](https://img.shields.io/badge/.Net_Standard-2.0-informational.svg)
</div>

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

### [Documentation](https://michel-pi.github.io/GameOverlay.Net/ "GameOverlay.Net Documentation")
### [Examples](https://github.com/michel-pi/GameOverlay.Net/tree/master/source/ "GameOverlay.Net examples") | C# and VB

# Contribute

The project file was generated using Visual Studio 2017.

Clone or download the repository and update/install the required NuGet packages.

You can help by reporting issues, adding new features, fixing bugs and by providing a better documentation.  

### Dependencies

    SharpDX.Direct2D1, SharpDX.DXGI, SharpDX

# Donate

Do you like this project and want to help me to keep working on it?

Then maybe consider to donate any amount you like.

[![Donate via PayPal](https://media.wtf/assets/img/pp.gif)](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=YJDWMDUSM8KKQ "Donate via PayPal")

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
