using System.Reflection;

namespace GameOverlay
{
    /// <summary>
    /// </summary>
    public static class Library
    {
        /// <summary>
        ///     Returns the Author of this Library
        /// </summary>
        public static string Author => "michel-pi";

        /// <summary>
        ///     Returns the Library Name
        /// </summary>
        public static string Name => "GameOverlay.Net";

        /// <summary>
        ///     Returns the URL of the Github Repository
        /// </summary>
        public static string ProjectUrl => "https://github.com/michel-pi/GameOverlay.Net";

        /// <summary>
        ///     Returns the <c>AssemblyVersion</c> of this Library
        /// </summary>
        public static string Version
        {
            get
            {
                try
                {
                    var assembly = Assembly.GetExecutingAssembly();
                    var assemblyName = assembly.GetName();

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