using System;
using System.Collections.Generic;

namespace NKMCore.Extensions
{
	public static class SystemGeneric
	{
		public static List<T> AddOne<T>(this List<T> list, T element)
		{
			list.Add(element);
			return list;
		}

		public static T GetRandomNoLog<T>(this List<T> list)
		{
			if (list.Count == 0) return default(T);
			var r = new Random();
			return list[r.Next(list.Count)];
		}
		public static T GetRandom<T>(this List<T> list)
		{
			if (list.Count == 0) return default(T);
			return list[NKMRandom.Get("System Generic Random" + NKMID.GetNext("System Generic Random"), 0, list.Count)];
		}
		public static T SecondLast<T>(this List<T> list)
		{
			if (list.Count < 2) throw new Exception("Sequence does not contain at least two elements.");
			return list[list.Count - 2];
		}
	}
}
