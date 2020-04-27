using System;
using System.Linq;

using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;

public partial class EBM {
	// output vectors
	/// <summary> Public temperature </summary>
	public static Vector<double> temp;
	/// <summary> Public energy </summary>
	public static Vector<double> energy;
	/// <summary> Public energy </summary>
	public static Vector<double> precip;

	// space-time vars
	/// <summary> Number of latitudinal bands </summary>
	static readonly int bands = 24;
	/// <summary> Number of game regions </summary>
	public static int regions = 3;
	/// <summary> Number of timesteps per year </summary>
	static readonly int nt = 1000;
	/// <summary> Number of years </summary>
	static readonly int dur = 30;
	/// <summary> Change in time </summary>
	static readonly double dt = 1f / nt;
	/// <summary> Change in position </summary>
	static readonly double dx = 1f / bands;
	/// <summary> Spatial latitudinal bands </summary>
	static readonly Vector<double> x = Vector<double>.Build.Dense(bands, i => dx / 2 + i++ * dx);
	/// <summary> Delta between bands </summary>
	static readonly Vector<double> xb = Vector<double>.Build.Dense(bands, i => ++i * dx);

	// standard consts
	/// <summary> OLR when T = 0(W m^-2) </summary>
	public static double A = 193;
	/// <summary> OLR temperature dependence(W M^-2 K^-1) </summary>
	static readonly double B = 2.1;
	/// <summary> Ocean mixed layer heat capacity(W yr m^-2 K^-1)  </summary>
	/// <remarks> Edit this to adjust model speed </remarks>
	static readonly double cw = 9.8;
	/// <summary> Diffusivity for heat transport(W m^-2 K^-1)  </summary>
	static readonly double D = 0.5;

	/// <summary> Insolation at equator(W m^-2)  </summary>
	static readonly double S0 = 420;
	/// <summary> Insolation seasonal dependence(W m^-2)  </summary>
	static readonly double S1 = 338;
	/// <summary> Insolation spatial dependence(W m^-2)  </summary>
	static readonly double S2 = 240;
	/// <summary> Ice-free co-albedo at equator  </summary>
	static double _a0 = 0.7;
	public static double a0 {
		get => _a0;
		set {
			_a0 = value;
			aw = a0 - a2 * x.PointwisePower(2);
		}
	}
	/// <summary> Ice-free co-albedo spatial dependence  </summary>
	static readonly double a2 = 0.1;
	/// <summary> Co-albedo where there is sea ice  </summary>
	static readonly double aI = 0.4;
	/// <summary> Open water albedos </summary>
	static Vector<double> aw = a0 - a2 * x.PointwisePower(2);
	/// <summary> Radiative forcing(W m^-2) </summary>
	public static double F = 0;
	public static double maxF = 16;

	// misc consts
	/// <summary> Latent heat of vaporization(J kg^-1) </summary>
	static readonly double Lv = 2500000;
	/// <summary> Heat capacity of air at constant pressure(J kg^-1 K^-1) </summary>
	static readonly double cp = 1004.6;
	/// <summary> Relative humidity </summary>
	static readonly double Rh = 0.8;
	/// <summary> Surface pressure(Pa) </summary>
	static readonly double Ps = 100000;
	/// <summary> Heat flux from ocean below(W m^-2) </summary>
	public static double Fb = 4;
	/// <summary> Sea ice thermal conductivity(W m^-2 K^-1) </summary>
	static readonly int k = 2;
	/// <summary> Sea ice latent heat of fusion(W yr m^-3) </summary>
	static readonly double Lf = 9.5;
	/// <summary> Ghost layer heat capacity(W yr m^-2 K^-1) </summary>
	static readonly double cg = cw / 100;
	/// <summary> Ratio of MSE aloft to near surface, equatorial MSE </summary>
	static readonly double gms_scale = 2; // was 1.06
	/// <summary> Characteristic width for gaussian weighting function </summary>
	static readonly double sigma = 0.4; // was 0.3

	// # Diffusion Operator (WE15, Appendix A)
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

	// # Definitions for implicit scheme on Tg
	/// <summary> Ghost layer coupling timescale(yr) </summary>
	static readonly double tau = 0.00001;
	static readonly double cg_tau = cg / tau;
	static readonly double dt_tau = dt / tau;
	static readonly double dc = dt_tau * cg_tau;
	static readonly double M = B + cg_tau;
	static readonly Matrix<double> I = Matrix<double>.Build.DenseIdentity(bands);
	static readonly Matrix<double> kappa = (1 + dt_tau) * I - dt * diffop / cg;

	// # Seasonal forcing (WE15 eq.3) 
	static readonly Vector<double> ty = Vector<double>.Build.Dense(nt, i => dt / 2 + i++ * dt);
	static readonly Vector<double> simpleS = S0 - S2 * x.PointwisePower(2);
	static readonly Matrix<double> S =
		Matrix<double>.Build.DenseOfRowVectors(new Vector<double>[nt].Map(v => simpleS).ToArray()) -
		Matrix<double>.Build.DenseOfRowVectors(new Vector<double>[bands].Map(v => S1 * (ty * 2 * Math.PI).PointwiseCos()).ToArray()).Transpose().PointwiseMultiply(
			Matrix<double>.Build.DenseOfRowVectors(new Vector<double>[nt].Map(v => x).ToArray()));
	//could optimise with indices if needed
}
