using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

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

	public static void Print(this UnityEngine.MonoBehaviour @this, params object[] words) => UnityEngine.Debug.Log(string.Join(" ", words));
	public static dynamic Log(this object @this) {
		UnityEngine.Debug.Log(@this);
		return @this;
	}
	public static T Log<T>(this object @this, T t) {
		UnityEngine.Debug.Log(t);
		return t;
	}

	public static IEnumerable < (TSource, int) > Enumerator<TSource>(this IEnumerable<TSource> @this) => @this.Map((T, i) => (T, i));
	public static IEnumerable<TResult> Map<TSource, TResult>(this IEnumerable<TSource> @this, Func<TSource, TResult> func) => @this.Select(func);
	public static IEnumerable<TResult> Map<TSource, TResult>(this IEnumerable<TSource> @this, Func<TSource, int, TResult> func) => @this.Select(func);

	///<summary> Same as GetComponentInChildren but does not return parent </summary>
	///<returns> Component of type <typeparamref name="T"/> or null </returns>
	///<remarks> also works on interfaces </remarks>
	public static T GetComponentOnlyInChildren<T>(this MonoBehaviour script) where T : Component {
		if (typeof(T).IsInterface || typeof(T).IsSubclassOf(typeof(Component)) || typeof(T) == typeof(Component))
			foreach (Transform child in script.transform) {
				var component = child.GetComponentInChildren<T>();
				if (component != null)
					return component;
			}
		return default(T);
	}

	///<summary> Same as GetComponentsInChildren but does not return parent </summary>
	///<returns> Array of Component type <typeparamref name="T"/> or null </returns>
	///<remarks> also works on interfaces </remarks>
	public static T[] GetComponentsOnlyInChildren<T>(this MonoBehaviour script) where T : Component {
		List<T> group = new List<T>();
		if (typeof(T).IsInterface || typeof(T).IsSubclassOf(typeof(Component)) || typeof(T) == typeof(Component))
			foreach (Transform child in script.transform)
				group.AddRange(child.GetComponentsInChildren<T>());
		return group.ToArray();
	}

	public static bool TryComponent<T>(this MonoBehaviour @this, out T c) where T : Component => @this.TryGetComponent(out c);
	public static bool TryComponent<T>(this MonoBehaviour @this) where T : Component => @this.TryGetComponent(out T c);
	public static bool TryComponent<T>(this GameObject @this, out T c) where T : Component => @this.TryGetComponent(out c);
	public static bool TryComponent<T>(this GameObject @this) where T : Component => @this.TryGetComponent(out T c);
	public static bool TryComponent<T>(this Component @this, out T c) where T : Component => @this.TryGetComponent(out c);
	public static bool TryComponent<T>(this Component @this) where T : Component => @this.TryGetComponent(out T c);
	public static bool TryComponent<T>(this Transform @this, out T c) where T : Component => @this.TryGetComponent(out c);
	public static bool TryComponent<T>(this Transform @this) where T : Component => @this.TryGetComponent(out T c);
}
