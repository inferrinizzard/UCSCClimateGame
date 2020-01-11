using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using UnityEngine;

public class EBM {
	public static Vector<double> temp; // public temperature
	public static Vector<double> energy; // public energy
	public static Vector<double> precip; // public energy
	public static double A = 193; // OLR when T = 0 (W m^-2)
	static readonly double B = 2.1; // OLR temperature dependence (W M^-2 K^-1)
	static readonly double cw = 9.8; // ocean mixed layer heat capacity (W yr m^-2 K^-1) 
	static readonly double D = 0.5; // diffusivity for heat transport (W m^-2 K^-1) 

	public static double S0 = 420; // insolation at equator (W m^-2) 
	public static double S1 = 338; // insolation seasonal dependence (W m^-2) 
	static readonly double S2 = 240; // insolation spatial dependence (W m^-2) 
	static readonly double a0 = 0.7; // ice-free co-albedo at equator 
	static readonly double a2 = 0.1; // ice=free co-albedo spatial dependence 
	static readonly double aI = 0.4; // co-albedo where there is sea ice 
	public static double F = 0; // radiative forcing (W m^-2)

	static readonly int bands = 24; //number of latitudinal bands
	public static int regions = 3;

	static readonly double Lv = 2500000; //  latent heat of vaporization (J kg^-1)
	static readonly double cp = 1004.6; //  heat capacity of air at constant pressure (J kg^-1 K^-1)
	static readonly double Rh = 0.8; //  relative humidity
	static readonly double Ps = 100000; //  surface pressure (Pa)

	// double overload
	public static Vector<double> Humidity(double[] temp, double press) => Humidity(Vector<double>.Build.Dense(temp), press);
	// calculates saturation specific humidity based on temperature
	public static Vector<double> Humidity(Vector<double> temp, double press) {
		double es0 = 610.78;
		double t0 = 273.16;
		double Rv = 461.5;
		double ep = 0.622;
		Vector<double> es = es0 * (-Lv / Rv * (1 / (temp + 273.15f) - 1 / t0)).PointwiseExp();
		Vector<double> qs = ep * es / press;
		return qs;
	}
	static readonly int nt = 1000; // number of timesteps per year
	static readonly int dur = 30; // number of years
	static readonly double dt = 1f / nt; // change in time
	static readonly double dx = 1f / bands; // change in position
	static readonly Vector<double> x = Vector<double>.Build.Dense(bands, i => dx / 2 + i++ * dx);
	static readonly Vector<double> xb = Vector<double>.Build.Dense(bands, i => ++i * dx);

	static readonly Vector<double> lam = D / dx / dx * (1 - xb.PointwisePower(2));
	static readonly Vector<double> L1 = Vector<double>.Build.Dense(bands, i => i == 0 ? 0 : -lam[i++ - 1]);
	static readonly Vector<double> L2 = Vector<double>.Build.Dense(bands, i => i >= bands ? 0 : -lam[i++]);
	static readonly Vector<double> L3 = -L1 - L2;
	static readonly Matrix<double> d3 = Matrix<double>.Build.DiagonalOfDiagonalVector(L3);
	static readonly Matrix<double> d2 = new Func<Matrix<double>>(() => {
		Matrix<double> mat = Matrix<double>.Build.Dense(bands, bands, 0);
		mat.SetSubMatrix(0, 1, Matrix<double>.Build.DiagonalOfDiagonalVector(L2.SubVector(0, bands - 1)));
		return mat;
	}) ();
	static readonly Matrix<double> d1 = new Func<Matrix<double>>(() => {
		Matrix<double> mat = Matrix<double>.Build.Dense(bands, bands, 0);
		mat.SetSubMatrix(1, 0, Matrix<double>.Build.DiagonalOfDiagonalVector(L1.SubVector(1, bands - 1)));
		return mat;
	}) ();

	static readonly Matrix<double> diffop = -d3 - d2 - d1;
	static readonly Vector<double> simpleS = S0 - S2 * x.PointwisePower(2);
	static readonly Vector<double> aw = a0 - a2 * x.PointwisePower(2);
	static readonly Matrix<double> I = Matrix<double>.Build.DenseIdentity(bands);
	public static double Fb = 4; // heat flux from ocean below (W m^-2)
	static readonly int k = 2; // sea ice thermal conductivity (W m^-2 K^-1)
	static readonly double Lf = 9.5; // sea ice latent heat of fusion (W yr m^-3)
	static readonly double cg = cw / 100; // ghost layer heat capacity(W yr m^-2 K^-1)
	static readonly double tau = 0.00001; // ghost layer coupling timescale (yr)
	static readonly double cg_tau = cg / tau;
	static readonly double dt_tau = dt / tau;
	static readonly double dc = dt_tau * cg_tau;
	static readonly Matrix<double> kappa = (1 + dt_tau) * I - dt * diffop / cg;
	static readonly Vector<double> ty = Vector<double>.Build.Dense(nt, i => dt / 2 + i++ * dt);
	static readonly Matrix<double> S =
		Matrix<double>.Build.DenseOfRowVectors(new Vector<double>[nt].Select(v => simpleS).ToArray()) -
		Matrix<double>.Build.DenseOfRowVectors(new Vector<double>[bands].Select(v => S1 * (ty * 2 * Math.PI).PointwiseCos()).ToArray()).Transpose().PointwiseMultiply(
			Matrix<double>.Build.DenseOfRowVectors(new Vector<double>[nt].Select(v => x).ToArray()));
	//could optimise with indices if needed
	static readonly double M = B + cg_tau;
	static readonly double gms_scale = 1.06; // ratio of MSE aloft to near surface, equatorial MSE
	static readonly double sigma = .3; // characteristic width for gaussian weighting function

	public static(Matrix<double>, Matrix<double>) Integrate(Vector<double> T = null, int years = 0, int timesteps = 0) {
		T = T == null ? 7.5f + 20 * (1 - 2 * x.PointwisePower(2)) : T;
		years = years == 0 ? dur : years;
		timesteps = timesteps == 0 ? nt : timesteps;
		Matrix<double> T100 = Matrix<double>.Build.Dense(bands, dur * 100, 0);
		Matrix<double> E100 = Matrix<double>.Build.Dense(bands, dur * 100, 0);
		Vector<double> Tg = Vector<double>.Build.DenseOfVector(T);
		Vector<double> E = Tg * cw;

		int p = -1, m = -1;
		for (int i = 0; i < years; i++) {
			for (int j = 0; j < timesteps; j++) {
				m++;
				if ((p + 1) * 10 == m) {
					p++;
					E100.SetColumn(p, E);
					T100.SetColumn(p, T);
				}
				Vector<double> alpha = E.PointwiseSign().PointwiseMultiply(aw).Map(x => x < 0 ? aI : x); //aw*(E > 0) + ai*(E < 0)
				Vector<double> C = alpha.PointwiseMultiply(S.Row(j)) + cg_tau * Tg - A; //alpha*S[i, :] + cg_tau*Tg - A
				Vector<double> T0 = C / (M - k * Lf / E);
				T = Sign0(GreatOrE, E) / cw + Sign0(Less, Sign0(Less, E, T0)); //E/cw*(E >= 0)+T0*(E < 0)*(T0 < 0)
				E = E + dt * (C - M * T + Fb + F);
				Vector<double> q = Rh * Humidity(Tg, Ps);
				Vector<double> lht = dt * (diffop * (Lv * q / cp));
				Tg = (kappa - Matrix<double>.Build.DiagonalOfDiagonalVector(
					Sign0(Less, Sign0(Less, E, T0), dc / (M - k * Lf / E)) //np.diag(dc/(M-kLf/E)*(T0 < 0)*(E < 0)
				)).Solve(Tg + lht + dt_tau * (
					Sign0(GreatOrE, E) / cw + (aI * S.Row(j) - A). //E/cw*(E >= 0)+(ai*S[i, :]-A)
					Map2((a, b) => b != 0 ? a / b : 0, //funky division
						Sign0(Less, Sign0(Less, E, T0), M - k * Lf / E)) //(M-kLf/E)*(T0 < 0)*(E < 0)
				));
			}
		}
		return (
			Matrix<double>.Build.DenseOfColumnArrays(T100.ToColumnArrays().Skip(T100.ColumnCount - 100).ToArray()), //Tfin
			Matrix<double>.Build.DenseOfColumnArrays(E100.ToColumnArrays().Skip(E100.ColumnCount - 100).ToArray()) //Efin
		);
	}

	public static Matrix<double> CalcPrecip(Matrix<double> Tfin) {
		double[][] TfinArr = Tfin.ToRowArrays();
		Matrix<double> qfin = Rh * Matrix<double>.Build.DenseOfRowVectors(TfinArr.Select(r => Humidity(r, Ps)));
		Matrix<double> hfin = Tfin + Lv * qfin / cp;

		Matrix<double> GradVertical(Matrix<double> mat) => Matrix<double>.Build.DenseOfColumnArrays(mat.ToColumnArrays().Select(col => {
			double[] gradCol = new double[col.Length];
			gradCol[0] = col[1] - col[0];
			for (int i = 1; i < col.Length - 1; i++)
				gradCol[i] = (col[i + 1] - col[i - 1]) / 2;
			gradCol[col.Length - 1] = col[col.Length - 1] - col[col.Length - 2];
			gradCol[0] -= gradCol[1] - gradCol[0];
			gradCol[col.Length - 1] += gradCol[col.Length - 1] - gradCol[col.Length - 2];
			return gradCol;
		}));

		Matrix<double> MultiplyRowWise(Matrix<double> mat, Vector<double> vec) => mat.MapIndexed((x, y, i) => i * vec[x]);

		Vector<double> OneMinusX2 = (1 - x.PointwisePower(2));
		Matrix<double> Fa = -D * MultiplyRowWise(GradVertical(hfin), OneMinusX2) * bands;
		Matrix<double> Fla = -D * MultiplyRowWise(GradVertical(Lv * qfin / cp), OneMinusX2) * bands;

		Vector<double> w = ((OneMinusX2 - 1) / sigma / sigma).PointwiseExp();
		Matrix<double> F_hc = MultiplyRowWise(Fa, w);
		Matrix<double> F_eddy = MultiplyRowWise(Fa, (1 - w));
		Matrix<double> Fl_eddy = MultiplyRowWise(Fla, (1 - w));

		Vector<double> hfin_eq = hfin.Row(0);
		Matrix<double> gms = hfin.MapIndexed((x, y, i) => (hfin_eq * gms_scale) [x] - i);
		Matrix<double> psi = F_hc.PointwiseDivide(gms);
		Matrix<double> Fl_hc = -(Lv * qfin / cp).PointwiseMultiply(psi);

		Matrix<double> Fl = Fl_hc + Fl_eddy;
		Matrix<double> EminusP = GradVertical(Fl) * bands;

		return EminusP;
	}

	public static(double[], double[], double[]) Calc(IEnumerable<double> input = null, int years = 0, int timesteps = 0) {
		var(T100, E100) = Integrate(input == null ? null : Vector<double>.Build.Dense(input.ToArray()), years, timesteps);
		temp = T100.Column(99);
		energy = E100.Column(99);
		precip = CalcPrecip(T100).Column(99);
		return (Reduce(temp, regions), Reduce(energy, regions), Reduce(precip, regions));
	}

	public static void Clear() => (temp, energy, precip) = (null, null, null);

	public static IEnumerable<IEnumerable<double>> Slice(IEnumerable<double> vec, int n = -1, int[] cuts = null, int j = 0) =>
		new Func<int, IEnumerable<IEnumerable<double>>>(m => vec.Select((x, i) =>
				new { Index = i, Value = x })
			.GroupBy(x =>
				cuts == null ?
				x.Index / (vec.Count() / m) :
				j == cuts.Length || x.Index <= cuts[j] ? j : ++j)
			.Select(x => x.Select(v => v.Value)))
		(n == -1 ? regions : n);
	public static double[] Reduce(IEnumerable<double> vec, int n, int[] cuts = null) => Slice(vec, n, cuts).Select(x => Average(x)).ToArray();
	static double Average(IEnumerable<double> vec) { return x.Average(); }

	static Predicate < (double, double) > Less = ((double, double) t) => t.Item1 < t.Item2;
	static Predicate < (double, double) > GreatOrE = ((double, double) t) => t.Item1 >= t.Item2;
	public static Vector<double> Sign0(Predicate < (double, double) > op, Vector<double> vec, Vector<double> result = null) => (result == null ? vec : result).PointwiseMultiply(vec.Map(x => op((x, 0d)) ? 1d : 0d));
	static void Print(IEnumerable<double> nums) => Debug.Log(nums == null ? "null" : String.Join(" ", nums));
}

// public sealed class Lambda<T> { public static Func<T, T> Cast = x => x; }
// cast lambda delegate
