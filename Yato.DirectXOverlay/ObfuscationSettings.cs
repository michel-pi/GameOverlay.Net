using System.Reflection;
using System.Runtime.CompilerServices;

// Prevent string interning
[assembly: CompilationRelaxations(CompilationRelaxations.NoStringInterning)]

// Standard features

[assembly: Obfuscation(Feature = "rename serializable symbols", Exclude = false)]
[assembly: Obfuscation(Feature = "sanitize resources", Exclude = false)]
[assembly: Obfuscation(Feature = "ildasm suppression", Exclude = false)]

// Advanced features

[assembly: Obfuscation(Feature = "string encryption", Exclude = false)]
[assembly: Obfuscation(Feature = "code control flow obfuscation", Exclude = false)]
[assembly: Obfuscation(Feature = "encrypt resources [compress]", Exclude = false)]

// assembly embedding

[assembly: Obfuscation(Feature = "merge with SharpDX.dll", Exclude = false)]
[assembly: Obfuscation(Feature = "merge with SharpDX.DXGI.dll", Exclude = false)]
[assembly: Obfuscation(Feature = "merge with SharpDX.Direct2D1.dll", Exclude = false)]