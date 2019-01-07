using System;
using JetBrains.Annotations;

namespace Extensions
{
	public static class Predicates
	{
		public static Predicate<T> Or<T>([CanBeNull] this Predicate<T> predicate, [CanBeNull] Predicate<T> secondPredicate) 
			=> item => secondPredicate != null && (predicate != null && (predicate(item) || secondPredicate(item)));
		
		public static Predicate<T> And<T>([CanBeNull] this Predicate<T> predicate, [CanBeNull] Predicate<T> secondPredicate) 
			=> item => secondPredicate != null && (predicate != null && (predicate(item) || secondPredicate(item)));
	}
}
