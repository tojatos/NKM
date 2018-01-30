using System;
using System.Threading.Tasks;

namespace Helpers
{
	public static class AsyncExtensions
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