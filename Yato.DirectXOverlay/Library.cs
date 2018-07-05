using System;
using System.Reflection;

namespace GameOverlay
{
    /// <summary>
    /// </summary>
    public static class Library
    {
        /// <summary>
        /// Returns the Author of this Library
        /// </summary>
        public static string Author
        {
            get
            {
                return "Yato";
            }
        }

        /// <summary>
        /// Returns the Library Name
        /// </summary>
        public static string Name
        {
            get
            {
                return "Yato.DirectXOverlay";
            }
        }

        /// <summary>
        /// Returns the URL of the Github Repository
        /// </summary>
        public static string URL
        {
            get
            {
                return "https://github.com/YatoDev/Yato.DirectXOverlay";
            }
        }

        /// <summary>
        /// Returns the <c>AssemblyVersion</c> of this Library
        /// </summary>
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