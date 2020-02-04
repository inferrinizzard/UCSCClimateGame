using System;

public static class Func {
	public static Func<TResult> Lambda<TResult>(Func<TResult> func) => func;

	public static Func<T, TResult> Lambda<T, TResult>(Func<T, TResult> func) => func;

	public static Func<T1, T2, TResult> Lambda<T1, T2, TResult>(Func<T1, T2, TResult> func) => func;
}
