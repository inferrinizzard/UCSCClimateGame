using System.Collections.Generic;

public static class Extensions {
	public static string String(this double[] nums) => $"[{string.Join(", ", nums)}]";
	public static string String(this IEnumerable<double> nums) => $"[{string.Join(", ", nums)}]";
}
