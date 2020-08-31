using System;
using System.Collections.Generic;
using System.Linq;

using MathNet.Numerics.LinearAlgebra;
using Interpolate = MathNet.Numerics.Interpolate;

public partial class EBM {
	public static void Reset() {
		F = 0;
		a0 = 0.7;
		Clear();
	}

	public static(Matrix<double>, Matrix<double>) Integrate(Vector<double> T = null, int years = 0, int timesteps = 0) {
		T = T ?? 7.5f + 20 * (1 - 2 * x.PointwisePower(2));
		years = years == 0 ? dur : years;
		timesteps = timesteps == 0 ? nt : timesteps;
		Matrix<double> T100 = Matrix<double>.Build.Dense(bands, dur * 100, 0);
		Matrix<double> E100 = Matrix<double>.Build.Dense(bands, dur * 100, 0);
		Vector<double> Tg = Vector<double>.Build.DenseOfVector(T);
		Vector<double> E = Tg * cw;

		for (var(i, p) = (0, 0); i < years; i++)
			for (int j = 0; j < timesteps; j++) {
				if (j % (nt / 100f) == 0) {
					E100.SetColumn(p, E);
					T100.SetColumn(p, T);
					p++;
				}
				Vector<double> alpha = E.PointwiseSign().PointwiseMultiply(aw).Map(x => x < 0 ? aI : x); // aw * (E > 0) + ai * (E < 0)
				Vector<double> C = alpha.PointwiseMultiply(S.Row(j)) + cg_tau * Tg - A + F; // alpha * S[i, :] + cg_tau * Tg - A
				Vector<double> T0 = C / (M - k * Lf / E);
				T = Sign0(GreatOrE, E) / cw + Sign0(Less, Sign0(Less, E, T0)); // E/cw*(E >= 0)+T0*(E < 0)*(T0 < 0)
				E = E + dt * (C - M * T + Fb);
				var mklfe = M - k * Lf / E;
				var signlesset0 = Sign0(Less, E, T0);
				Tg = (kappa - Matrix<double>.Build.DiagonalOfDiagonalVector(
					Sign0(Less, signlesset0, dc / mklfe) // np.diag(dc / (M - kLf / E) * (T0 < 0) * (E < 0)
				)).Solve(Tg + dt_tau * (
					Sign0(GreatOrE, E) / cw + (aI * S.Row(j) - A + F). // E / cw * (E >= 0) + (ai * S[i, :] - A)
					Map2((a, b) => b != 0 ? a / b : 0, // funky division
						Sign0(Less, signlesset0, mklfe)) // (M - kLf / E) * (T0 < 0) * (E < 0)
				));
			}

		return (T100.SubMatrix(0, T100.RowCount, T100.ColumnCount - 100, 100), E100.SubMatrix(0, E100.RowCount, E100.ColumnCount - 100, 100));
	}

	/// <summary> Calculates Precipitation of regions</summary>
	/// <param name="temp"> <c>Vector</c> of final temp column means</param>
	/// <returns> <c>Vector</c> of Precipitation - Evapouration per region </returns>
	public static Vector<double> CalcPrecip(Vector<double> temp) {
		Vector<double> dT = temp - tempControl;
		Vector<double> dp_e = dT.PointwiseMultiply(np_e) * alpha;
		return np_e + dp_e;
	}

	/// <summary> Runs main integration model </summary>
	/// <param name="input"> Optional starting temp for model, will default in model </param>
	/// <param name="years"> Optional duration to run model, will default to <see cref="dur"/> </param>
	/// <param name="timesteps"> Optional number of steps per year, will default to <see cref="nt"/> </param>
	/// <example>
	/// <code>
	/// Calc(temp[]) →(temp[], energy[], precip[])
	/// </code>
	/// </example>
	/// <returns> Tuple of double arrays(temp, energy, precip)</returns>
	public static(double[], double[], double[]) Calc(IEnumerable<double> input = null, int years = 0, int timesteps = 0) {
		var(T100, E100) = Integrate(input == null ? null : Vector<double>.Build.Dense(input.ToArray()), years, timesteps);
		temp = T100.Column(99);
		energy = E100.Column(99);

		if (tempControl is null) InitFirstRun(T100);

		precip = CalcPrecip(Vector<double>.Build.DenseOfEnumerable(T100.FoldByRow((mean, col) => mean + col / T100.ColumnCount, 0d)));
		return (Condense(temp, regions), Condense(energy, regions), Condense(precip, regions));
	}

	static void InitFirstRun(Matrix<double> T100) {
		tempControl = Vector<double>.Build.DenseOfEnumerable(T100.FoldByRow((mean, col) => mean + col / T100.ColumnCount, 0d));
		// (tempControl, energyControl) = (temp, energy);
		p_e = p_e_raw.Split(',').Select(num => Double.Parse(num.Trim(new [] { '\n', ' ', '\t' }))).ToArray();
		lat_p_e = Vector<double>.Build.Dense(p_e.Length, i => i / 2d - 90).SubVector(181, 180);
		f = Interpolate.Common(lat_p_e, p_e.Skip(181));
		np_e = lat.Map(l => f.Interpolate(l));
	}

	public static void Clear() => (temp, energy, precip) = (null, null, null);

	public static IEnumerable<IEnumerable<double>> Slice(IEnumerable<double> vec, int n = -1, int[] cuts = null) =>
		Func.Lambda((int m, int j) => vec.Select((x, i) =>
				new { Index = i, Value = x })
			.GroupBy(x =>
				cuts == null ?
				x.Index / (vec.Count() / m) :
				j == cuts.Length || x.Index <= cuts[j] ? j : ++j)
			.Select(x => x.Select(v => v.Value)))
		(n == -1 ? regions : n, 0);

	public static double[] Condense(IEnumerable<double> vec, int n, int[] cuts = null) => Slice(vec, n, cuts).Select(x => x.Average()).ToArray();
	// static double Average(IEnumerable<double> vec) { return x.Average(); }

	static Predicate < (double, double) > Less = ((double, double) t) => t.Item1 < t.Item2;
	static Predicate < (double, double) > GreatOrE = ((double, double) t) => t.Item1 >= t.Item2;
	public static Vector<double> Sign0(Predicate < (double, double) > op, Vector<double> vec, Vector<double> result = null) => (result ?? vec).PointwiseMultiply(vec.Map(x => op((x, 0d)) ? 1d : 0d));
	static void Print(IEnumerable<double> nums) => UnityEngine.Debug.Log(nums == null ? "null" : nums.AsString());
	static void Print(double num) => UnityEngine.Debug.Log(num);
}

// public sealed class Lambda<T> { public static Func<T, T> Cast = x => x; }
// cast lambda delegate
