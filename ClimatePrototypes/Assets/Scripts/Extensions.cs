using System.Collections.Generic;

public static class Extensions {
	public static string AsString(this double[] nums) => $"[{string.Join(", ", nums)}]";
	public static string AsString(this IEnumerable<double> nums) => $"[{string.Join(", ", nums)}]";
}
