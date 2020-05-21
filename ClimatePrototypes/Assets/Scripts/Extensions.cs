using System;
using System.Collections.Generic;
using System.Linq;

public static class Extensions {
	public static string AsString<T>(this IEnumerable<T> nums) => $"[{string.Join(", ", nums)}]";
	public static string AsString<T>(this T[] nums) => $"[{string.Join(", ", nums)}]";

	public static void ForEach<TSource>(this IEnumerable<TSource> @this, Action<TSource> action) {
		if (@this == null)
			throw new ArgumentNullException("Source");
		if (action == null)
			throw new ArgumentNullException("Action");
		var e = @this.GetEnumerator();
		while (e.MoveNext())
			action(e.Current);
		e.Dispose();
	}

	public static void ForEach<TSource>(this IEnumerable<TSource> @this, Action<TSource, int> action) {
		if (@this == null)
			throw new ArgumentNullException("Source");
		if (action == null)
			throw new ArgumentNullException("Action");
		int index = 0;
		var e = @this.GetEnumerator();
		while (e.MoveNext())
			action(e.Current, index++);
		e.Dispose();
	}

	public static void print(this UnityEngine.MonoBehaviour @this, params object[] words) => UnityEngine.Debug.Log(string.Join(" ", words));
	public static dynamic Log(this object @this) {
		UnityEngine.Debug.Log(@this);
		return @this;
	}
	public static T Log<T>(this object @this, T t) {
		UnityEngine.Debug.Log(t);
		return t;
	}

	public static IEnumerable<TResult> Map<TSource, TResult>(this IEnumerable<TSource> @this, Func<TSource, TResult> func) => @this.Select(func);
	public static IEnumerable<TResult> Map<TSource, TResult>(this IEnumerable<TSource> @this, Func<TSource, int, TResult> func) => @this.Select(func);
	public static IEnumerable<TSource> Filter<TSource>(this IEnumerable<TSource> @this, Func<TSource, bool> func) => @this.Where(func);
	public static IEnumerable<TSource> Filter<TSource>(this IEnumerable<TSource> @this, Func<TSource, int, bool> func) => @this.Where(func);
	public static TSource Reduce<TSource>(this IEnumerable<TSource> @this, Func<TSource, TSource, TSource> func) => @this.Aggregate(func);
	public static TAccumulate Reduce<TSource, TAccumulate>(this IEnumerable<TSource> @this, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func) => @this.Aggregate(seed, func);
	public static TResult Reduce<TSource, TAccumulate, TResult>(this IEnumerable<TSource> @this, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func, Func<TAccumulate, TResult> resultSelector) => @this.Aggregate(seed, func, resultSelector);
}
