using System;
using System.Reflection;

namespace Yato.DirectXOverlay
{
    public static class Library
    {
        public static string Author
        {
            get
            {
                return "Yato";
            }
        }

        public static string Name
        {
            get
            {
                return "Yato.DirectXOverlay";
            }
        }

        public static string URL
        {
            get
            {
                return "https://github.com/YatoDev/Yato.DirectXOverlay";
            }
        }

        public static string Version
        {
            get
            {
                try
                {
                    Assembly assembly = Assembly.GetExecutingAssembly();
                    AssemblyName assemblyName = assembly.GetName();

                    return assemblyName.Version.ToString();
                }
                catch
                {
                    return "1.0.0.0";
                }
            }
        }
    }
}