using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Examples
{
	public static class Program
	{
		public static void Main(string[] _)
		{
			using (var example = new Example())
			{
				example.Run();
			}
		}
	}
}
