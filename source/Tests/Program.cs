using System;

namespace Tests
{
	public static class Program
	{
		public static void Main(string[] _)
		{
			var overlay = new GraphicsWindowTest();

			overlay.Run();

			Console.ReadLine();

			overlay.ReCreate();

			Console.WriteLine("Recreated");

			Console.ReadLine();

			overlay.Stop();

			Console.WriteLine("Stopped");

			Console.ReadLine();
		}
	}
}
