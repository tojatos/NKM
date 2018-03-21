using System;
using System.Threading.Tasks;

namespace Helpers
{
	public static class SynchronizableExtensions
	{
		public static string SynchronizableSerialize<T>(this T value, string name)
		{
			string serializedValue;
            if (value == null) serializedValue = null;
            else
            {
                switch (name)
                {
                    case "GamePlayer":
                        serializedValue = (value as GamePlayer).Name;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();

                }
            }

            return serializedValue;
            }
		
	}
}