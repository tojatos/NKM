using System.Collections.Generic;

namespace Extensions
{
	public static class SystemGeneric
	{
		public static List<T> AddOne<T>(this List<T> list, T element)
		{
			list.Add(element);
			return list;
		}
	}
}