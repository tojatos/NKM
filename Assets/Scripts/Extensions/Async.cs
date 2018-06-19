using System;
using System.Threading.Tasks;

namespace Extensions
{
	public static class Async
	{
		public static async Task WaitToBeTrue(this Func<bool> predicate)
		{
			while (!predicate.Invoke())
			{
				await Task.Delay(1);
			}
		}
	}
}