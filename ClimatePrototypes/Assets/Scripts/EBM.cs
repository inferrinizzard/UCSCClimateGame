using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;

using UnityEngine;

public class EBM
{
	public static double A = 193;
	public static readonly double B = 2.1;
	public static readonly double cw = 9.8;
	public static readonly double D = 0.5;

	public static readonly double S0 = 420;
	public static readonly double S1 = 338;
	public static readonly double S2 = 240;
	public static readonly double a0 = 0.7;
	public static readonly double a2 = 0.1;
	public static readonly double aI = 0.4;
	public static readonly double F = 0; // 6

	public static readonly int bands = 6;

	public static readonly double Lv = 2500000;
	public static readonly double cp = 1004.6;
	public static readonly double Rh = 0.8;
	public static readonly double Ps = 100000;

	public static Vector<double> humidity(double[] temp, double press) => humidity(Vector<double>.Build.Dense(temp), press);
	public static Vector<double> humidity(Vector<double> temp, double press)
	{
		double es0 = 610.78;
		double t0 = 273.16;
		double Rv = 461.5;
		double ep = 0.622;
		Vector<double> es = es0 * (-Lv / Rv * (1 / (temp + 273.15f) - 1 / t0)).PointwiseExp();
		Vector<double> qs = ep * es / press;
		// Debug.Log(es);
		// Debug.Log(qs);
		return qs;
	}

	public static class simple
	{
		public static readonly double dx = 1f / (bands - 1f);
		public static Vector<double> x = Vector<double>.Build.Dense(bands, i => i++ * dx);

		public static Vector<double> simpleS = S0 - S2 * x.PointwisePower(2);
		public static Vector<double> aw = a0 - a2 * x.PointwisePower(2);

		public static Vector<double> odefunc(double[] temp, double t, bool moist = false) =>
			odefunc(Vector<double>.Build.Dense(temp), t, moist);

		public static Vector<double> odefunc(Vector<double> temp, double t, bool moist = false)
		{
			Vector<double> T = Vector<double>.Build.DenseOfVector(temp);

			Vector<double> alpha = T.PointwiseSign().PointwiseMultiply(aw).Map(x => x < 0 ? aI : x);

			Vector<double> C = alpha.PointwiseMultiply(simpleS) - A + F;

			if (moist)
			{
				Vector<double> qs = humidity(T, Ps);
				Vector<double> q = Rh * qs;
				// Debug.Log(T);
				T += Lv * q / cp;
				// Debug.Log(T);
			}

			Vector<double> Tdot = Vector<double>.Build.Dense(x.Count);
			for (int i = 1; i < bands - 1; i++)
				Tdot[i] = (D / dx / dx) * (1 - x[i] * x[i]) * (T[i + 1] - 2 * T[i] + T[i - 1]) - (D * x[i] / dx) * (T[i + 1] - T[i - 1]);
			Tdot[0] = D * 2 * (T[1] - T[0]) / dx / dx;
			Tdot[Tdot.Count - 1] = -D * 2 * x[x.Count - 1] * (T[T.Count - 1] - T[T.Count - 2]) / dx;
			// Debug.Log(Tdot);
			Vector<double> f = (Tdot + C - B * temp) / cw;
			// Debug.Log(String.Join(" ", f));
			return f;
		}

	}

	public static class fast
	{
		public static readonly int nt = 1000;
		public static readonly int dur = 30;
		public static readonly double dt = 1f / nt;
		public static readonly double dx = 1f / bands;
		public static readonly Vector<double> x = Vector<double>.Build.Dense(bands, i => dx / 2 + i++ * dx);
		public static readonly Vector<double> xb = Vector<double>.Build.Dense(bands, i => ++i * dx);

		public static readonly Vector<double> lam = D / dx / dx * (1 - xb.PointwisePower(2));
		public static readonly Vector<double> L1 = Vector<double>.Build.Dense(bands, i => i == 0 ? 0 : -lam[i++ - 1]);
		public static readonly Vector<double> L2 = Vector<double>.Build.Dense(bands, i => i >= bands ? 0 : -lam[i++]);
		public static readonly Vector<double> L3 = -L1 - L2;
		public static readonly Matrix<double> d3 = Matrix<double>.Build.DiagonalOfDiagonalVector(L3);
		public static readonly Matrix<double> d2 = new Func<Matrix<double>>(() =>
		{
			Matrix<double> mat = Matrix<double>.Build.Dense(bands, bands, 0);
			mat.SetSubMatrix(0, 1, Matrix<double>.Build.DiagonalOfDiagonalVector(L2.SubVector(0, bands - 1)));
			return mat;
		})();
		public static readonly Matrix<double> d1 = new Func<Matrix<double>>(() =>
		{
			Matrix<double> mat = Matrix<double>.Build.Dense(bands, bands, 0);
			mat.SetSubMatrix(1, 0, Matrix<double>.Build.DiagonalOfDiagonalVector(L1.SubVector(1, bands - 1)));
			return mat;
		})();

		public static readonly Matrix<double> diffop = -d3 - d2 - d1;
		public static readonly Vector<double> simpleS = S0 - S2 * x.PointwisePower(2);
		public static readonly Vector<double> aw = a0 - a2 * x.PointwisePower(2);
		public static readonly Vector<double> t = Vector<double>.Build.Dense(dur * nt, i => i++ / nt);
		public static readonly Matrix<double> I = Matrix<double>.Build.DenseIdentity(bands);
		public static readonly Matrix<double> invMat = (I + dt / cw * (B * I - diffop)).Inverse();

		public static Matrix<double> integrate(int num = 0)
		{
			num = num == 0 ? dur * nt : num;
			Vector<double> T = Vector<double>.Build.Dense(bands, 10);
			Matrix<double> allT = Matrix<double>.Build.Dense(dur * nt, bands, 0);
			for (int i = 0; i < num; i++)
			{
				Vector<double> alpha = T.PointwiseSign().PointwiseMultiply(aw).Map(x => x < 0 ? aI : x);
				Vector<double> C = alpha.PointwiseMultiply(simpleS) - A + F;
				Vector<double> T0 = T + dt / cw * C;
				T = invMat * T0;
				allT.SetRow(i, T);
			}
			return allT;
		}

		public static readonly int Fb = 4;
		public static readonly int k = 2;
		public static readonly double Lf = 9.5;
		public static readonly double cg = cw / 100;
		public static readonly double tau = 0.00001;
		public static readonly double cg_tau = cg / tau;
		public static readonly double dt_tau = dt / tau;
		public static readonly double dc = dt_tau * cg_tau;
		public static readonly Matrix<double> kappa = (1 + dt_tau) * I - dt * diffop / cg;
		public static readonly Vector<double> ty = Vector<double>.Build.Dense(nt, i => dt / 2 + i++ * dt);
		public static readonly Matrix<double> S =
			Matrix<double>.Build.DenseOfRowVectors(new Vector<double>[nt].Select(v => simpleS).ToArray()) -
			Matrix<double>.Build.DenseOfRowVectors(new Vector<double>[bands].Select(v => S1 * (ty * 2 * Math.PI).PointwiseCos()).ToArray()).Transpose().PointwiseMultiply(
				Matrix<double>.Build.DenseOfRowVectors(new Vector<double>[nt].Select(v => x).ToArray()));
		//could optimise with indices if needed
		public static readonly double M = B + cg_tau;
		public static readonly double gms_scale = 1.06;
		public static readonly double sigma = .3;

		public static Matrix<double>[] integrateM(int n = 0, int d = 0)
		{
			n = n == 0 ? nt : n;
			d = d == 0 ? dur : d;
			Matrix<double> T100 = Matrix<double>.Build.Dense(bands, dur * 100, 0);
			Matrix<double> E100 = Matrix<double>.Build.Dense(bands, dur * 100, 0);
			Vector<double> T = 7.5f + 20 * (1 - 2 * x.PointwisePower(2));
			Vector<double> Tg = Vector<double>.Build.DenseOfVector(T);
			Vector<double> E = Tg * cw;

			int p = -1, m = -1;
			int z = 0;
			for (int i = 0; i < d; i++)
			{
				for (int j = 0; j < n; j++)
				{
					m++;
					if ((p + 1) * 10 == m)
					{
						p++;
						E100.SetColumn(p, E);
						T100.SetColumn(p, T);
					}
					Vector<double> alpha = E.PointwiseSign().PointwiseMultiply(aw).Map(x => x < 0 ? aI : x); //aw*(E > 0) + ai*(E < 0)
					Vector<double> C = alpha.PointwiseMultiply(S.Row(j)) + cg_tau * Tg - A; //alpha*S[i, :] + cg_tau*Tg - A
					Vector<double> T0 = C / (M - k * Lf / E);
					T = Sign0(GreatOrE, E) / cw + Sign0(Less, Sign0(Less, E, T0)); //E/cw*(E >= 0)+T0*(E < 0)*(T0 < 0)
					E = E + dt * (C - M * T + Fb + F);
					Vector<double> q = Rh * humidity(Tg, Ps);
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
			// Debug.Log(T100);
			// Debug.Log(E100);
			return new Matrix<double>[] {
				Matrix<double>.Build.DenseOfColumnArrays(T100.ToColumnArrays().Take(100).ToArray()), //Tfin
				Matrix<double>.Build.DenseOfColumnArrays(E100.ToColumnArrays().Take(100).ToArray())  //Efin
			};
		}

		public static Matrix<double>[] precip(Matrix<double>[] TEfin)
		{
			double[][] TfinArr = TEfin[0].ToRowArrays();
			Matrix<double> qfin = Rh * Matrix<double>.Build.DenseOfRowVectors(TfinArr.Select(r => humidity(r, Ps)));
			Matrix<double> hfin = TEfin[0] + Lv * qfin / cp;

			Matrix<double> gradVertical(Matrix<double> mat) => Matrix<double>.Build.DenseOfColumnArrays(mat.ToColumnArrays().Select(col =>
			{
				double[] gradCol = new double[col.Length];
				gradCol[0] = col[1] - col[0];
				for (int i = 1; i < col.Length - 1; i++)
					gradCol[i] = (col[i + 1] - col[i - 1]) / 2;
				gradCol[col.Length - 1] = col[col.Length - 1] - col[col.Length - 2];
				return gradCol;
			}));

			Matrix<double> MultiplyRowWise(Matrix<double> mat, Vector<double> vec) => mat.MapIndexed((x, y, i) => i * vec[x]);

			Vector<double> OneMinusX2 = (1 - x.PointwisePower(2));
			Matrix<double> Fa = -D * MultiplyRowWise(gradVertical(hfin), OneMinusX2) * bands;
			Matrix<double> Fla = -D * MultiplyRowWise(gradVertical(Lv * qfin / cp), OneMinusX2) * bands;

			Vector<double> w = ((OneMinusX2 - 1) / sigma / sigma).PointwiseExp();
			Matrix<double> F_hc = MultiplyRowWise(Fa, w);
			Matrix<double> F_eddy = MultiplyRowWise(Fa, (1 - w));
			Matrix<double> Fl_eddy = MultiplyRowWise(Fla, (1 - w));

			Vector<double> hfin_eq = hfin.Row(0);
			Matrix<double> gms = hfin.MapIndexed((x, y, i) => (hfin_eq * gms_scale)[x] - i);
			Matrix<double> psi = F_hc.PointwiseDivide(gms);
			Matrix<double> Fl_hc = -(Lv * qfin / cp).PointwiseMultiply(psi);

			Matrix<double> Fl = Fl_hc + Fl_eddy;
			Matrix<double> EminusP = gradVertical(Fl) * bands;

			return null;
		}

	}

	public static void printTest()
	{

		Matrix<double>[] TE100 = fast.integrateM();
		// Debug.Log(TE100[0]);
		// Debug.Log(TE100[1]);
		// fast.precip(TE100);
	}

	static Func<double, double, bool> Less = (a, b) => a < b;
	static Func<double, double, bool> Great = (a, b) => a > b;
	static Func<double, double, bool> LessOrE = (a, b) => a <= b;
	static Func<double, double, bool> GreatOrE = (a, b) => a >= b;
	public static Vector<double> Sign0(Func<double, double, bool> op, Vector<double> vec, Vector<double> result = null) => (result == null ? vec : result).PointwiseMultiply(vec.Map(x => op(x, 0d) ? 1d : 0d));

	public static Vector<double> Round(Vector<double> vec, int spec = 2) => vec.Map(x => Math.Round(x * Math.Pow(10, spec)) / Math.Pow(10, spec));

	public static void WriteToFile(string file, Vector<double> data) => WriteToFile(file, data.ToArray());
	public static void WriteToFile(string file, params double[] data) => System.IO.File.WriteAllText(file, String.Join(",", data.ToArray()));
}

// public sealed class Lambda<T> { public static Func<T, T> Cast = x => x; }
// cast lambda delegate