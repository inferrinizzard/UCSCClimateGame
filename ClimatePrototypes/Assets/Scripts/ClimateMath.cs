using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ClimateMath
{
	//official scientific values from rcp graph projections
	public static Dictionary<int, Dictionary<string, float>> refs = new Dictionary<int, Dictionary<string, float>>() { { 2000, new Dictionary<string, float> () { { "8.5", 1.723f }, { "6.0", 1.723f }, { "4.5", 1.723f }, { "2.6", 1.723f } } }, { 2005, new Dictionary<string, float> () { { "8.5", 1.906f }, { "6.0", 1.901f }, { "4.5", 1.905f }, { "2.6", 1.904f } } }, { 2010, new Dictionary<string, float> () { { "8.5", 2.154f }, { "6.0", 2.089f }, { "4.5", 2.126f }, { "2.6", 2.129f } } }, { 2020, new Dictionary<string, float> () { { "8.5", 2.665f }, { "6.0", 2.480f }, { "4.5", 2.579f }, { "2.6", 2.584f } } }, { 2030, new Dictionary<string, float> () { { "8.5", 3.276f }, { "6.0", 2.854f }, { "4.5", 3.005f }, { "2.6", 2.862f } } }, { 2040, new Dictionary<string, float> () { { "8.5", 3.993f }, { "6.0", 3.146f }, { "4.5", 3.411f }, { "2.6", 2.999f } } }, { 2050, new Dictionary<string, float> () { { "8.5", 4.762f }, { "6.0", 3.521f }, { "4.5", 3.766f }, { "2.6", 2.998f } } }, { 2060, new Dictionary<string, float> () { { "8.5", 5.539f }, { "6.0", 3.905f }, { "4.5", 4.021f }, { "2.6", 2.918f } } }, { 2070, new Dictionary<string, float> () { { "8.5", 6.299f }, { "6.0", 4.443f }, { "4.5", 4.188f }, { "2.6", 2.854f } } }, { 2080, new Dictionary<string, float> () { { "8.5", 7.020f }, { "6.0", 4.932f }, { "4.5", 4.256f }, { "2.6", 2.808f } } }, { 2090, new Dictionary<string, float> () { { "8.5", 7.742f }, { "6.0", 5.255f }, { "4.5", 4.265f }, { "2.6", 2.759f } } }, { 2100, new Dictionary<string, float> () { { "8.5", 8.388f }, { "6.0", 5.481f }, { "4.5", 4.309f }, { "2.6", 2.714f } } },
	};

	//takes in array of previous temps and current year according to ^, returns array of temps for remaining years
	// limited to above indices
	public static float[] Predict(float[] history, int year)
	{
		float ema = EMA(history);
		float score = Mathf.InverseLerp(refs[year]["8.5"], refs[year]["2.6"], ema) * 3;
		List<float> prediction = new List<float>();
		foreach (KeyValuePair<int, Dictionary<string, float>> kvp in refs)
		{
			if (kvp.Key > year)
				if (score > 2)
					prediction.Add(Mathf.Lerp(kvp.Value["8.5"], kvp.Value["6.0"], score - 2));
				else if (score < 1)
					prediction.Add(Mathf.Lerp(kvp.Value["4.5"], kvp.Value["2.6"], score));
				else
					prediction.Add(Mathf.Lerp(kvp.Value["6.0"], kvp.Value["4.5"], score - 1));
		}
		return prediction.ToArray();
	}

	//wip, calc changes in temp
	static float[] CalcDeltas(float[] history)
	{
		float[] avgs = new float[history.Length];
		return new float[1];
	}

	//calc temp weighted average, may refactor inline
	static float CalcTemp(float[] temps, float[] weights)
	{
		return temps.Aggregate((float sum, float cur) => sum + cur * weights[Array.IndexOf(temps, cur)]);
	}

	//weighted average of array of numbers, good for temp/emissions/etc
	static float EMA(float[] nums)
	{
		// float[] prev = (float[])(new float[nums.Length]).Zip(nums, (a, b)=>nums[Array.IndexOf(nums, b)]);
		return nums[nums.Length - 1] * 2 / (nums.Length + 1) + EMA((float[])(new float[nums.Length - 1]).Select((a, b) => nums[Array.IndexOf(nums, b)])) * (1 - 2 / (nums.Length + 1));
		// float[] prev = (float[])nums.Clone();
		// Array.Resize(ref prev, nums.Length-1);
		// float k = 2/(nums.Length+1);
		// return nums[nums.Length-1] * k + EMA(prev) * (1-k);
	}

	#region ebm

	struct ebmv
	{
		public static float A = 193;
		public static readonly float B = 2.1f;
		public static readonly float cw = 9.8f;
		public static readonly float D = 0.6f;

		public static float S0 = 420;
		public static float S2 = 240;
		public static float a0 = 0.7f;
		public static float a2 = 0.1f;
		public static float aI = 0.4f;
		public static float F = 0;

		public static readonly int bands = 3;
	}

	struct ebm
	{
		public static List<float> x = new Func<List<float>>(() =>
		{
			List<float> linspace = new List<float>();
			for (int i = 0; i < ebmv.bands; i++)
				linspace.Add(i / (ebmv.bands - 1f));
			return linspace;
		})();
		public static float dx = 1f / (ebmv.bands - 1f);

		public static List<float> S = x.Select(n => ebmv.S0 - ebmv.S2 * n * n).ToList();
		public static List<float> aw = x.Select(n => ebmv.a0 - ebmv.a2 * n * n).ToList();
	}
	public static float[] ode(float[] temp, float time)
	{
		List<float> x = ebm.x;
		float dx = ebm.dx;
		// Debug.Log("x: " + String.Join(" ", x));
		// Debug.Log("aw: " + String.Join(" ", aw));
		// return null;
		List<float> alpha = new List<float>();
		for (int i = 0; i < ebmv.bands; i++)
			alpha.Add(temp[i] > 0 ? ebm.aw[i] : ebmv.aI); //aw * (temp > 0) + aI * (temp < 0);
		List<float> C = alpha.Zip(ebm.S, (a, b) => a * b - ebmv.A + ebmv.F).ToList();

		float[] Tdot = new float[x.Count];
		for (int i = 1; i < ebmv.bands - 1; i++)
			Tdot[i] = (ebmv.D / dx / dx) * (1 - x[i] * x[i]) * (temp[i + 1] - 2 * temp[i] + temp[i - 1]) - (ebmv.D * x[i] / dx) * (temp[i + 1] - temp[i - 1]);
		Tdot[0] = ebmv.D * 2 * (temp[1] - temp[0]) / dx / dx;
		Tdot[Tdot.Length - 1] = -ebmv.D * 2 * x[x.Count - 1] * (temp[temp.Length - 1] - temp[temp.Length - 2]) / dx;
		float[] f = Tdot.Zip(C, (a, b) => a + b).Zip(temp.Select(n => ebmv.B * n), (float a, float b) => (a - b) / ebmv.cw).ToArray();
		// Debug.Log("time: " + time);
		// Debug.Log("alpha: " + String.Join(" ", alpha));
		// Debug.Log("C: " + String.Join(" ", C));
		// Debug.Log("Tdot: " + String.Join(" ", Tdot));
		// Debug.Log("f: " + String.Join(" ", f));
		return f;
	}

	struct ebmf
	{
		public static float nt = 5;
		public static float dt = 1 / nt;

		public static float dur = 100;
		public static float dx = 1 / (float)ebmv.bands;

		public static List<float> x = new Func<List<float>>(() =>
		{
			List<float> arange = new List<float>();
			for (int i = 0; i < ebmv.bands; i++)
				arange.Add(dx / 2 + i * dx);
			return arange;
		})();

		public static List<float> xb = new Func<List<float>>(() =>
		{
			List<float> arange = new List<float>();
			for (int i = 1; i <= ebmv.bands; i++)
				arange.Add(i * dx);
			return arange;
		})();

		public static List<List<float>> diffop = initDiffop();

		public static List<float> S = x.Select(n => ebmv.S0 - ebmv.S2 * n * n).ToList();
		public static List<float> aw = x.Select(n => ebmv.a0 - ebmv.a2 * n * n).ToList();

		//invert this somehow
		public static List<List<float>> invmat = new Func<List<List<float>>>(() =>
		{
			int i = 0;
			return diffop.Select(row => row.Select(n =>
			{
				if (row.IndexOf(n) == i++)
					return (ebmv.B - n) * dt / ebmv.cw + 1;
				else return -n * dt / ebmv.cw;
			}).ToList()).ToList();
		})();
	}

	public static List<List<float>> initDiffop()
	{
		List<float> lam = ebmf.xb.Select(n => (1 - n * n) * ebmv.D / ebmf.dx / ebmf.dx).ToList();
		List<float> L1 = (new List<float>() { 0 }).Concat(lam.Select(n => -n)).ToList();
		List<float> L2 = lam.Select(n => -n).Concat((new List<float>() { 0 })).ToList();
		List<float> L3 = L1.Zip(L2, (a, b) => -a - b).ToList();

		int i = 0;
		List<List<float>> D3 = L3.Take(L3.Count - 1).Select(l =>
			{
				List<float> row = new List<float>(new float[L3.Count - 1]);
				row[i++] = -l;
				Debug.Log("row" + i + ": " + String.Join(" ", row));
				return row;
			}).ToList();
		i = 0;
		List<List<float>> D2 = L2.Take(L2.Count - 1).Select(l =>
		{
			List<float> row = new List<float>(new float[L2.Count - 1]);
			if (i < L2.Count - 2)
				row[++i] = -l;
			Debug.Log("row" + i + ": " + String.Join(" ", row));
			return row;
		}).ToList();
		i = -1;
		List<List<float>> D1 = L1.Take(L1.Count - 1).Select(l =>
			{
				List<float> row = new List<float>(new float[L1.Count - 1]);
				if (i < 0)
					i++;
				else
					row[i++] = -l;
				Debug.Log("row" + i + ": " + String.Join(" ", row));
				return row;
			}).ToList();
		// Concat(new List<float>(new float[L1.Count - 1])).ToList();
		List<List<float>> diffop = D3.Zip(D2.Zip(D1, (a, b) => a.Zip(b, (c, d) => c + d).ToList()), (a, b) => a.Zip(b, (c, d) => c + d).ToList()).ToList();
		Debug.Log(String.Join("/", diffop.Select(d => String.Join(" ", d))));

		return diffop;
	}

	public static float[] odeFast(float[] temp, float t)
	{
		float dx = ebmf.dx;
		// Debug.Log(String.Join("/", ebmf.invmat.Select(d => String.Join(" ", d))));
		List<float> alpha = new List<float>();
		for (int i = 0; i < ebmv.bands; i++)
			alpha.Add(temp[i] > 0 ? ebm.aw[i] : ebmv.aI); //aw * (temp > 0) + aI * (temp < 0);
		List<float> C = alpha.Zip(ebm.S, (a, b) => a * b - ebmv.A + ebmv.F).ToList();
		// List<float> T0 = temp.Zip(C, (a, b) => a + dt / ebmv.cw * b).ToList();
		//temp = dot invMat, T0

		return new float[0];
	}

	#endregion
}