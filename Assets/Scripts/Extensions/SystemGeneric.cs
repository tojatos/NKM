using System;
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

		public static T GetRandom<T>(this List<T> list)
		{
			if (list.Count == 0) return default(T);
			var r = new Random();
			return list[r.Next(list.Count)];
		}
	}
}