using System;

namespace Examples
{
	public static class Program
	{
		public static void Main(string[] _)
		{
			GameOverlay.TimerService.EnableHighPrecisionTimers();

			using (var example = new Example())
			{
				example.Run();
			}
		}
	}
}
