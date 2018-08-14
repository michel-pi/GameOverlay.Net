using System;

namespace GameOverlayExample
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("GameOverlay.Net - Test");

            Console.WriteLine();

            Console.WriteLine("Author: " + GameOverlay.Library.Author);
            Console.WriteLine("Library Name: " + GameOverlay.Library.Name);
            Console.WriteLine("Project Url: " + GameOverlay.Library.ProjectUrl);
            Console.WriteLine("Version: " + GameOverlay.Library.Version);

            Console.WriteLine();

            Console.WriteLine("Type exit to close this program!");

            Example example = new Example();

            bool exit = false;
            while (!exit)
            {
                string line = Console.ReadLine();

                if(line == null) continue;

                if (line.ToLower() == "exit") exit = true;
            }
        }
    }
}
