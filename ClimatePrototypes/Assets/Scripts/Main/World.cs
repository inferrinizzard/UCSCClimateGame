using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class World {
	public static float money = 100f;
	public static float publicOpinion = 5f;
	public static int turn = 1;
	public static int actionsRemaining = 2;
	public static double[] temp;
	public static double[] energy;
	public static double[] precip;

	public static void Init() {
		Calc();
	}

	public static void Calc(bool useTemp = false) => (temp, energy, precip) = EBM.Calc(useTemp ? EBM.temp : null);

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

	public static void UpdateAlbedo(float delta) {
		// 
	}

}
