using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public static class World {
	public static float money = 100f;
	public static float publicOpinion = 0f;
	public static int turn = 1;
	public static int actionsRemaining = 2;
	public static double[] temp;
	public static double[] energy;
	public static double[] precip;

	public static double averageTemp = 80.0;

	public static void Init() {
		Calc();
		// FinishCalc(StartCalc().Wait());
	}

	public static void Calc(bool useTemp = false, int years = 0, int steps = 0) {
		(temp, energy, precip) = EBM.Calc(useTemp ? EBM.temp : null, years, steps);
		averageTemp = temp.Average();
		Debug.Log(averageTemp);
		Debug.Log(temp.AsString());
	}

	public static async Task StartCalc(bool useTemp = false, int years = 0, int steps = 0) => await Task.Run(() => EBM.Calc(useTemp ? EBM.temp : null, years, steps));

	public static void FinishCalc((double[], double[], double[])state) {
		(temp, energy, precip) = state;
		averageTemp = temp.Average();
	}

	public readonly static Dictionary<string, System.Action<float>> tagUpdates = new Dictionary<string, System.Action<float>> { { "co2", UpdateCO2 }, { "land", UpdateAlbedo }, { "money", UpdateMoney }, { "opinion", UpdateOpinion } };

	public static void UpdateFactor(string tag, float delta) => tagUpdates[tag].Invoke(delta);

	static void UpdateCO2(float deltaF) => EBM.F += deltaF;
	static void UpdateMoney(float delta) => money += delta;
	static void UpdateOpinion(float delta) => publicOpinion += delta;
	static void UpdateAlbedo(float deltaS1) => EBM.S1 = deltaS1;
	// add other albedo?
}
