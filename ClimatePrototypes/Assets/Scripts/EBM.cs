using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using Extreme.Mathematics.Calculus.OrdinaryDifferentialEquations;

public class EBM
{
	public static float A = 193;
	public static readonly float B = 2.1f;
	public static readonly float cw = 9.8f;
	public static readonly float D = 0.6f;

	public static readonly float S0 = 420;
	public static readonly float S1 = 338;
	public static readonly float S2 = 240;
	public static readonly float a0 = 0.7f;
	public static readonly float a2 = 0.1f;
	public static readonly float aI = 0.4f;
	public static readonly float F = 6;

	public static readonly int bands = 6;

	public static readonly float Lv = 2500000;
	public static readonly float cp = 1004.6f;
	public static readonly float Rh = 0.8f;
	public static readonly float Ps = 100000;

	public static Vector<float> humidity(float[] temp, float press) => humidity(Vector<float>.Build.Dense(temp), press);
	public static Vector<float> humidity(Vector<float> temp, float press)
	{
		float es0 = 610.78f;
		float t0 = 273.16f;
		float Rv = 461.5f;
		float ep = 0.622f;
		Vector<float> es = es0 * (-Lv / Rv * (1 / (temp + 273.15f) - 1 / t0)).PointwiseExp();
		Vector<float> qs = ep * es / press;
		// Debug.Log(es);
		// Debug.Log(qs);
		return qs;
	}

	public static class simple
	{
		public static readonly float dx = 1f / (bands - 1f);
		public static Vector<float> x = Vector<float>.Build.Dense(bands, i => i++ * dx);

		public static Vector<float> simpleS = S0 - S2 * x.PointwisePower(2);
		public static Vector<float> aw = a0 - a2 * x.PointwisePower(2);

		public static Extreme.Mathematics.Vector<double> odefunc(double t, Extreme.Mathematics.Vector<double> y, Extreme.Mathematics.Vector<double> dy)
		{
			return Extreme.Mathematics.Vector.Create<double>(odefunc(Vector<float>.Build.DenseOfEnumerable(y.Select(n => (float)n)), (float)t).Select(n => (double)n).ToArray());
		}
		public static Vector<float> odefunc(float[] temp, float t, bool moist = false) =>
			 odefunc(Vector<float>.Build.Dense(temp), t, moist);

		public static Vector<float> odefunc(Vector<float> temp, float t, bool moist = false)
		{
			Vector<float> T = Vector<float>.Build.DenseOfVector(temp);

			Vector<float> alpha = T.PointwiseSign().PointwiseMultiply(aw).Map(x => x < 0 ? aI : x);

			Vector<float> C = alpha.PointwiseMultiply(simpleS) - A + F;

			if (moist)
			{
				Vector<float> qs = humidity(T, Ps);
				Vector<float> q = Rh * qs;
				// Debug.Log(T);
				T += Lv * q / cp;
				// Debug.Log(T);
			}

			Vector<float> Tdot = Vector<float>.Build.Dense(x.Count);
			for (int i = 1; i < bands - 1; i++)
				Tdot[i] = (D / dx / dx) * (1 - x[i] * x[i]) * (T[i + 1] - 2 * T[i] + T[i - 1]) - (D * x[i] / dx) * (T[i + 1] - T[i - 1]);
			Tdot[0] = D * 2 * (T[1] - T[0]) / dx / dx;
			Tdot[Tdot.Count - 1] = -D * 2 * x[x.Count - 1] * (T[T.Count - 1] - T[T.Count - 2]) / dx;
			// Debug.Log(Tdot);
			Vector<float> f = (Tdot + C - B * temp) / cw;
			// Debug.Log(String.Join(" ", f));
			return f;
		}

		//figure out odeint, it's kinda right at 100* steps
		public static float[] odeint(float[] temp, int steps, bool useMoisture = false)
		{
			float[] time = new float[steps].Select((T, k) => k * 30f / steps).ToArray();
			// Vector<float> t = Vector<float>.Build.DenseOfArray(temp);
			Extreme.Mathematics.Vector<double> t = Extreme.Mathematics.Vector.Create<double>(temp.Select(n => (double)n).ToArray());

			ClassicRungeKuttaIntegrator rk = new ClassicRungeKuttaIntegrator();
			// rk.DifferentialFunction = odefunc;
			rk.DifferentialFunction = Lorentz;
			rk.InitialTime = 0;
			rk.InitialValue = t;
			rk.StepSize = 30f / steps;

			for (int i = 0; i < steps; i++)
			{
				t = rk.Integrate(time[i]);
				Debug.Log(t);
			}
			// t += odefunc(t, time[i], useMoisture) / 100f;
			return t.Select(n => (float)n).ToArray();
		}

		//test ode
		static Extreme.Mathematics.Vector<double> Lorentz(double t, Extreme.Mathematics.Vector<double> y, Extreme.Mathematics.Vector<double> dy)
		{
			if (dy == null)
				dy = Extreme.Mathematics.Vector.Create<double>(3);

			double sigma = 10.0;
			double beta = 8.0 / 3.0;
			double rho = 28.0;

			dy[0] = sigma * (y[1] - y[0]);
			dy[1] = y[0] * (rho - y[2]) - y[1];
			dy[2] = y[0] * y[1] - beta * y[2];

			return dy;
		}
	}

	public static class fast
	{
		public static readonly int nt = 10;
		public static readonly int dur = 30;
		public static readonly float dt = 1f / nt;
		public static readonly float dx = 1f / bands;
		public static readonly Vector<float> x = Vector<float>.Build.Dense(bands, i => dx / 2 + i++ * dx);
		public static readonly Vector<float> xb = Vector<float>.Build.Dense(bands, i => ++i * dx);

		public static readonly Vector<float> lam = D / dx / dx * (1 - xb.PointwisePower(2));
		public static readonly Vector<float> L1 = Vector<float>.Build.Dense(bands, i => i == 0 ? 0 : -lam[i++ - 1]);
		public static readonly Vector<float> L2 = Vector<float>.Build.Dense(bands, i => i >= bands ? 0 : -lam[i++]);
		public static readonly Vector<float> L3 = -L1 - L2;
		public static readonly Matrix<float> d3 = Matrix<float>.Build.DiagonalOfDiagonalVector(L3);
		public static readonly Matrix<float> d2 = new Func<Matrix<float>>(() =>
		{
			Matrix<float> mat = Matrix<float>.Build.Dense(bands, bands, 0);
			mat.SetSubMatrix(0, 1, Matrix<float>.Build.DiagonalOfDiagonalVector(L2.SubVector(0, bands - 1)));
			return mat;
		})();
		public static readonly Matrix<float> d1 = new Func<Matrix<float>>(() =>
		{
			Matrix<float> mat = Matrix<float>.Build.Dense(bands, bands, 0);
			mat.SetSubMatrix(1, 0, Matrix<float>.Build.DiagonalOfDiagonalVector(L1.SubVector(1, bands - 1)));
			return mat;
		})();

		public static readonly Matrix<float> diffop = -d3 - d2 - d1;
		public static readonly Vector<float> simpleS = S0 - S2 * x.PointwisePower(2);
		public static readonly Vector<float> aw = a0 - a2 * x.PointwisePower(2);
		public static readonly Vector<float> t = Vector<float>.Build.Dense(dur * nt, i => i++ / nt);
		public static readonly Matrix<float> I = Matrix<float>.Build.DenseIdentity(bands);
		public static readonly Matrix<float> invMat = (I + dt / cw * (B * I - diffop)).Inverse();

		public static Matrix<float> integrate(int num = 0)
		{
			num = num == 0 ? dur * nt : num;
			Vector<float> T = Vector<float>.Build.Dense(bands, 10);
			Matrix<float> allT = Matrix<float>.Build.Dense(dur * nt, bands, 0);
			for (int i = 0; i < num; i++)
			{
				Vector<float> alpha = T.PointwiseSign().PointwiseMultiply(aw).Map(x => x < 0 ? aI : x);
				Vector<float> C = alpha.PointwiseMultiply(simpleS) - A + F;
				Vector<float> T0 = T + dt / cw * C;
				T = invMat * T0;
				allT.SetRow(i, T);
			}
			return allT;
		}

		public static readonly int Fb = 4;
		public static readonly int k = 2;
		public static readonly float Lf = 9.5f;
		public static readonly float cg = cw / 100f;
		public static readonly float tau = 0.00001f;
		public static readonly float cg_tau = cg / tau;
		public static readonly float dt_tau = dt / tau;
		public static readonly float dc = dt_tau * cg_tau;
		public static readonly Matrix<float> kappa = (1 + dt_tau) * I - dt * diffop / cg;
		public static readonly Vector<float> ty = Vector<float>.Build.Dense(nt, i => dt / 2 + i++ * dt);
		public static readonly Matrix<float> S =
		Matrix<float>.Build.DenseOfRowVectors(new Vector<float>[nt].Select(v => simpleS).ToArray()) -
		Matrix<float>.Build.DenseOfRowVectors(new Vector<float>[bands].Select(v => S1 * (ty * 2 * Mathf.PI).PointwiseCos()).ToArray()).Transpose().PointwiseMultiply(
		Matrix<float>.Build.DenseOfRowVectors(new Vector<float>[nt].Select(v => x).ToArray()));
		//could optimise with indices if needed
		public static readonly float M = B + cg_tau;
		public static readonly float gms_scale = 1.06f;
		public static readonly float sigma = .3f;

		public static Matrix<float>[] integrateM(int n = 0, int d = 0)
		{
			n = n == 0 ? nt : n;
			d = d == 0 ? dur : d;
			Matrix<float> T100 = Matrix<float>.Build.Dense(bands, dur * 100, 0);
			Matrix<float> E100 = Matrix<float>.Build.Dense(bands, dur * 100, 0);
			Vector<float> T = 7.5f + 20 * (1 - 2 * x.PointwisePower(2));
			Vector<float> Tg = Vector<float>.Build.DenseOfVector(T);
			Vector<float> E = Tg * cw;
			int p = -1, m = -1;
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
					Vector<float> alpha = E.PointwiseSign().PointwiseMultiply(aw).Map(x => x < 0 ? aI : x);
					Vector<float> C = alpha.PointwiseMultiply(S.Row(j)) - A + cg_tau * Tg;
					Vector<float> T0 = C / (M - k * Lf / E);
					T = Sign0(GreatOrE, E) / cw + Sign0(Less, Sign0(Less, E, T0));
					E = E + dt * (C - M * T + Fb + F);
					Vector<float> q = Rh * humidity(Tg, Ps);
					Debug.Log(q);
					Vector<float> lht = dt * (diffop * (Lv * q / cp));
					Debug.Log(lht);
					Tg = (kappa - Matrix<float>.Build.DiagonalOfDiagonalVector(
						Sign0(Less, Sign0(Less, E, T0), dc / (M - k * Lf / E))
					)).Solve(Tg + lht + dt_tau * (
						Sign0(GreatOrE, E) / cw + (aI * S.Row(j) - A).Map2((a, b) => b != 0 ? a / b : 0, Sign0(Less, Sign0(Less, E, T0), M - k * Lf / E))
					));
					Debug.Log(Tg);
					return null;
					// Debug.Log(Tg);
				}
			}
			// Permutation reverse = new Permutation(new int[fast.dur * fast.nt].Select((x, k) => fast.dur * fast.nt - k - 1).ToArray());
			// T100.PermuteRows(reverse);
			// E100.PermuteRows(reverse);
			return new Matrix<float>[] { T100, E100 };
		}

		public static Matrix<float>[] precip(Matrix<float>[] TEfin)
		{
			float[][] TfinArr = TEfin[0].ToRowArrays();
			Matrix<float> qfin = Matrix<float>.Build.DenseOfRowVectors(TfinArr.Select(r => humidity(r, Ps)));
			Matrix<float> hfin = TEfin[0] + Lv * q / cp;
		}
	}

	public static void printTest()
	{

		Matrix<float>[] TE100 = fast.integrateM();
		// Debug.Log(TE100[0]);
		// Debug.Log(TE100[1]);
		// Permutation reverse = new Permutation(new int[fast.dur * fast.nt].Select((x, k) => fast.dur * fast.nt - k - 1).ToArray());
		// T100.PermuteRows(reverse);
		// E100.PermuteRows(reverse);

		// simple.odefunc(new float[bands].Select(x => 10f).ToArray(), 5, true);
		// Debug.Log(fast.S);
		// Debug.Log(fast.aw);
		// Debug.Log(Sign0(Great, fast.aw));
	}

	static Func<double, double, bool> Less = (a, b) => a < b;
	static Func<double, double, bool> Great = (a, b) => a > b;
	static Func<double, double, bool> LessOrE = (a, b) => a <= b;
	static Func<double, double, bool> GreatOrE = (a, b) => a >= b;
	public static Vector<float> Sign0(Func<double, double, bool> op, Vector<float> vec, Vector<float> result = null) => (result == null ? vec : result).PointwiseMultiply(vec.Map(x => op(x, 0) ? 1f : 0));

}

// public sealed class Lambda<T> { public static Func<T, T> Cast = x => x; }
// cast lambda delegate