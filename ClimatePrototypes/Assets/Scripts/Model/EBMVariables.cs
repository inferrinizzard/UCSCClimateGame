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
	static Vector<double> tempControl = null, energyControl = null;

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
	static readonly double D = 0.6;

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

	// // misc consts
	// /// <summary> Latent heat of vaporization(J kg^-1) </summary>
	// static readonly double Lv = 2500000;
	// /// <summary> Heat capacity of air at constant pressure(J kg^-1 K^-1) </summary>
	// static readonly double cp = 1004.6;
	// /// <summary> Relative humidity </summary>
	// static readonly double Rh = 0.8;
	// /// <summary> Surface pressure(Pa) </summary>
	// static readonly double Ps = 100000;
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

	static readonly double alpha = 0.07;
	static double[] p_e;

	static readonly string p_e_raw =
		@"0.10862098,0.11183009,0.10432099,0.10314173,0.11085312,0.12200008,0.13416383,0.14448051,0.17378414,0.17229489,
0.14769356,0.15501857,0.15935379,0.15786913,0.17113985,0.166769,0.16908537,0.17241426,0.17072816,0.17882186,
0.19009153,0.22191951,0.25060132,0.28568895,0.30585849,0.29497306,0.27293069,0.34249228,0.48946987,0.56134559,
0.55492675,0.53519221,0.62751412,0.68381974,0.61754225,0.61137311,0.61574398,0.65693447,0.68600673,0.7311493,
0.75752272,0.82395823,0.87400506,0.91194786,0.95357552,1.03335272,1.11760409,1.13341319,1.09219133,1.14574047,
1.14008872,1.1591753,1.15338251,1.1430743,1.1354989,1.13774253,1.13651183,1.13999316,1.14316355,1.14789052,
1.15370038,1.16306861,1.16463485,1.1672566,1.16454416,1.1639622,1.16302579,1.16345041,1.16676745,1.16941124,
1.16615158,1.17620456,1.16698562,1.16865926,1.16502859,1.16924299,1.16676557,1.16503217,1.17065379,1.16242259,
1.1544551,1.13699821,1.11378,1.07531373,1.00517866,0.95758798,0.92646061,0.85926072,0.79624939,0.735649,
0.68476212,0.61967299,0.54010743,0.47709366,0.37592223,0.28044083,0.17822998,0.0692228,-0.03031476,-0.13125741,
-0.21523285,-0.29007724,-0.35113765,-0.40383231,-0.45938789,-0.52434977,-0.59436902,-0.66079244,-0.71823444,-0.77571252,
-0.82438832,-0.8499418,-0.86387935,-0.89348629,-0.94840869,-1.00052332,-1.0302126,-1.06335745,-1.11528371,-1.17489918,
-1.23271353,-1.2882454,-1.33782437,-1.38971286,-1.44200838,-1.50682493,-1.55172115,-1.60225515,-1.65363389,-1.69259765,
-1.70512033,-1.72477121,-1.75476714,-1.78374319,-1.79820921,-1.81566965,-1.83647154,-1.86442593,-1.87789909,-1.90211801,
-1.92432954,-1.93019604,-1.9228293,-1.89783099,-1.85433179,-1.8145491,-1.76632446,-1.73028866,-1.6773278,-1.62808729,
-1.57444576,-1.50652569,-1.44265876,-1.37635646,-1.31196807,-1.25340215,-1.20396925,-1.11226513,-1.04156211,-0.94252503,
-0.82167726,-0.68565187,-0.55792115,-0.43055552,-0.28568947,-0.16354922,-0.05624261,0.06268511,0.17881295,0.2360902,
0.32709873,0.43201582,0.54007626,0.53563977,0.42080825,0.49376369,0.52973182,0.56098102,0.56146934,0.56929587,
0.55499649,0.58357286,0.60004204,0.69104763,0.82955258,0.993619,1.20171519,1.45454815,1.73642478,2.02712352,
2.26897738,2.46673953,2.60326199,2.65048115,2.62033008,2.47503133,2.29892324,2.06939614,1.83629867,1.5576531,
1.2889776,1.0003467,0.70213108,0.37393898,0.11720419,-0.08598687,-0.26046715,-0.42141048,-0.5407853,-0.62748818,
-0.69592536,-0.76917159,-0.87265771,-0.90773235,-0.91836339,-0.92728175,-0.96319871,-0.97775362,-0.95504534,-1.00311805,
-1.07388723,-1.11433028,-1.12982064,-1.13731903,-1.14320389,-1.16280006,-1.14942461,-1.1547134,-1.17171868,-1.17608531,
-1.16547704,-1.10553184,-1.0813952,-1.06797593,-0.97405166,-0.85110585,-0.76351415,-0.70894397,-0.69588247,-0.66379218,
-0.60357542,-0.57212353,-0.59333303,-0.59353179,-0.56502375,-0.53320356,-0.52394687,-0.48400582,-0.44607772,-0.38258784,
-0.27542378,-0.22253008,-0.19309222,-0.12571912,-0.07113165,-0.04120409,-0.00673027,0.03122497,0.08923378,0.15271483,
0.2194575,0.31558943,0.37005947,0.45123019,0.48869115,0.53600194,0.58436101,0.6209299,0.62523533,0.64024863,
0.65327988,0.67049404,0.70885739,0.74281533,0.78209547,0.79934455,0.76463605,0.77051241,0.76382528,0.78704477,
0.8021004,0.77044249,0.78768867,0.7775553,0.73073085,0.72614996,0.71832732,0.70666177,0.7097351,0.69620204,
0.6666466,0.69593226,0.69709261,0.69714748,0.69063467,0.657819,0.64781068,0.68530937,0.71151576,0.7305571,
0.75754713,0.79872102,0.69090815,0.73073223,0.64792352,0.60812053,0.59559317,0.55322095,0.56784079,0.613171,
0.59731001,0.5821381,0.57316361,0.55709459,0.56782523,0.57144564,0.52136119,0.50228769,0.45840379,0.41985547,
0.4024571,0.36088715,0.34363499,0.34189064,0.35084968,0.3370243,0.33955731,0.35158015,0.35766908,0.36773288,
0.38942817,0.39532161,0.3682182,0.42369399,0.41299302,0.38753914,0.40478542,0.41630737,0.42293699,0.43993322,
0.41371101,0.38626827,0.38614385,0.39546437,0.36854392,0.38938585,0.3653638,0.36526506,0.34498391,0.33771299,
0.32608182,0.31822199,0.30916282,0.30288618,0.29469955,0.28085571,0.27620448,0.27370551,0.27059146,0.27116078,
0.21769079";
}
