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

	public static void Calc(bool useTemp = false, int years = 0, int steps = 0) => (temp, energy, precip) = EBM.Calc(useTemp ? EBM.temp : null, years, steps);

	static void UpdateCO2(double deltaF) => EBM.F += deltaF;
	static void UpdateMoney(float delta) => money += delta;
	static void UpdateOpinion(float delta) => publicOpinion += delta;
	static void UpdateAlbedo(double deltaS1) => EBM.S1 = deltaS1;
	// add other albedo?
}
