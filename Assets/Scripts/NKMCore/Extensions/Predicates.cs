using System;

namespace NKMCore.Extensions
{
	public static class Predicates
	{
		public static Predicate<T> Or<T>(this Predicate<T> predicate, Predicate<T> secondPredicate) 
			=> item => predicate != null && predicate(item) || secondPredicate!=null && secondPredicate(item);
		
		public static Predicate<T> And<T>(this Predicate<T> predicate, Predicate<T> secondPredicate) 
			=> item => predicate != null && predicate(item) && secondPredicate!=null && secondPredicate(item);
	}
}
