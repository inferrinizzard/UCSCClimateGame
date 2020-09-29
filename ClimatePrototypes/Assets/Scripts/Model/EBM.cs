using System;
using System.Collections.Generic;
using System.Linq;

using MathNet.Numerics.LinearAlgebra;
using Interpolate = MathNet.Numerics.Interpolate;

public partial class EBM {
	// array overload
	// public static Vector<double> Humidity(double[] temp, double press) => Humidity(Vector<double>.Build.Dense(temp), press);
	// // calculates saturation specific humidity based on temperature
	// public static Vector<double> Humidity(Vector<double> temp, double press) {
	// 	const double es0 = 610.78;
	// 	const double t0 = 273.16;
	// 	const double Rv = 461.5;
	// 	const double ep = 0.622;
	// 	Vector<double> es = es0 * (-Lv / Rv * (1 / (temp + 273.15f) - 1 / t0)).PointwiseExp();
	// 	Vector<double> qs = ep / press * es;
	// 	return qs;
	// }

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
				// Vector<double> q = Rh * Humidity(Tg, Ps);
				// Vector<double> lht = (dt * diffop) * ((Lv / cp) * q);
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
		// TODO: rewrite this?
		return (
			Matrix<double>.Build.DenseOfColumnArrays(T100.ToColumnArrays().Skip(T100.ColumnCount - 100).ToArray()), // Tfin
			Matrix<double>.Build.DenseOfColumnArrays(E100.ToColumnArrays().Skip(E100.ColumnCount - 100).ToArray()) // Efin
		);
	}
	static Matrix<double> MultiplyRowWise(Matrix<double> mat, Vector<double> vec) => mat.MapIndexed((x, y, i) => i * vec[x]);

	static Matrix<double> GradVertical(Matrix<double> mat) => Matrix<double>.Build.DenseOfColumnArrays(mat.ToColumnArrays().Select(col => {
		double[] gradCol = new double[col.Length];
		gradCol[0] = col[1] - col[0];
		for (int i = 1; i < col.Length - 1; i++)
			gradCol[i] = (col[i + 1] - col[i - 1]) / 2;
		gradCol[col.Length - 1] = col[col.Length - 1] - col[col.Length - 2];
		gradCol[0] -= gradCol[1] - gradCol[0];
		gradCol[col.Length - 1] += gradCol[col.Length - 1] - gradCol[col.Length - 2];
		return gradCol;
	}));

	/// <summary> Calculates Precipitation of regions</summary>
	/// <param name="Tfin"> <c>Matrix</c> of final temp </param>
	/// <returns> <c>Matrix</c> of Evapouration - Precipitation per region </returns>
	// public static Matrix<double> CalcPrecip(Matrix<double> Tfin) {
	// 	double[][] TfinArr = Tfin.ToRowArrays();
	// 	Matrix<double> qfin = Rh * Matrix<double>.Build.DenseOfRowVectors(TfinArr.Select(r => Humidity(r, Ps)));
	// 	Matrix<double> hfin = Tfin + Lv * qfin / cp;

	// 	Vector<double> OneMinusX2 = (1 - x.PointwisePower(2));
	// 	Matrix<double> Fa = -D * bands * MultiplyRowWise(GradVertical(hfin), OneMinusX2);
	// 	Matrix<double> Fla = -D * bands * MultiplyRowWise(GradVertical(Lv * qfin / cp), OneMinusX2);

	// 	Vector<double> w = (1d / sigma / sigma * (OneMinusX2 - 1)).PointwiseExp();
	// 	var oneMinusw = 1 - w;
	// 	Matrix<double> F_hc = MultiplyRowWise(Fa, w);
	// 	Matrix<double> F_eddy = MultiplyRowWise(Fa, oneMinusw);
	// 	Matrix<double> Fl_eddy = MultiplyRowWise(Fla, oneMinusw);

	// 	Vector<double> hfin_eq = hfin.Row(0);
	// 	Matrix<double> gms = hfin.MapIndexed((x, y, i) => (hfin_eq * gms_scale) [x] - i);
	// 	Matrix<double> psi = F_hc.PointwiseDivide(gms);
	// 	Matrix<double> Fl_hc = (-Lv / cp * qfin).PointwiseMultiply(psi);

	// 	Matrix<double> Fl = Fl_hc + Fl_eddy;
	// 	Matrix<double> EminusP = GradVertical(Fl) * bands;

	// 	return EminusP;
	// }

	public static Vector<double> CalcPrecip(Vector<double> temp) {
		// move these out
		Vector<double> lat = Vector<double>.Asin(x).Map(x => x / Math.PI * 180);
		Vector<double> lat_p_e = Vector<double>.Build.Dense(p_e.Length, i => i / 2d - 90).SubVector(181, 180);
		var f = Interpolate.Common(lat_p_e, p_e.Skip(181));
		Vector<double> np_e = lat.Map(l => f.Interpolate(l));
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
		if (tempControl is null) {
			tempControl = Vector<double>.Build.DenseOfEnumerable(T100.FoldByRow((mean, col) => mean + col / T100.ColumnCount, 0d));
			// (tempControl, energyControl) = (temp, energy);
			p_e = p_e_raw.Split(',').Select(num => Double.Parse(num.Trim(new [] { '\n', ' ', '\t' }))).ToArray();
		}
		precip = CalcPrecip(Vector<double>.Build.DenseOfEnumerable(T100.FoldByRow((mean, col) => mean + col / T100.ColumnCount, 0d)));
		Print(precip);
		return (Condense(temp, regions), Condense(energy, regions), Condense(precip, regions));
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
