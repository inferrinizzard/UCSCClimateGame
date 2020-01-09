using System.Collections;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using UnityEngine;

public static class World {
	public static float money = 100f;
	public static int turn = 1;
	public static int actionsRemaining = 2;
	public static int billIndex = 0;
	static double[] _temp;
	public static double[] temp {
		get { return _temp; }
		set { _temp = value; }
	}
	static double[] _energy;
	public static double[] energy {
		get { return _energy; }
		set { _energy = value; }
	}
	static double[] _precip;
	public static double[] precip {
		get { return _precip; }
		set { _precip = value; }
	}

	public static void Init() {
		Calc();
	}

	public static void Calc(bool useTemp = false) => (_temp, _energy, _precip) = EBM.Calc(useTemp ? EBM.temp : null);

	public static void Update(double F = -1, double Fb = -1, double S0 = -1, double S1 = -1) {
		if (F != -1)
			EBM.F = F;
		if (Fb != -1)
			EBM.Fb = Fb;
		if (S0 != -1)
			EBM.S0 = S0;
		if (S1 != -1)
			EBM.S1 = S1;
		Calc();
	}

	//will add other editor functions

	public static void UpdateTemp(string region, float deltaT) {
		// temps[region]--;
	}

	public static void UpdateCO2(float ppm) {
		// co2 += ppm;
	}
}
