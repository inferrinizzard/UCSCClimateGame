using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Regex = System.Text.RegularExpressions.Regex;
using RegexOptions = System.Text.RegularExpressions.RegexOptions;

using UnityEngine;
using Stopwatch = System.Diagnostics.Stopwatch;

public static class World {
	public static float money = 100f;
	public static float publicOpinion = 0f;
	public static int turn = 1;
	public static double[] temp;
	public static double[] energy;
	public static double[] precip;

	public static double averageTemp = 0;

	public enum Region { Arctic, City, Forest, Fire }
	public struct Factor {
		public string name, verbose;
		Action<float> update;

		public Factor(string name, string fullName, Action<float> updateFunction) {
			this.name = name;
			verbose = fullName;
			update = updateFunction;
		}

		public void Update(Region scene, Region? dest, float delta) {
			lineToDraw.Add((scene, dest ?? Region.Forest, name));
			update.Invoke(delta);
		}
	}

	public static List < (Region, Region, string) > lineToDraw = new List < (Region, Region, string) > ();

	public static Factor co2 = new Factor("co2", "Emissions", new Action<float>((float deltaF) => EBM.F += deltaF)),
		albedo = new Factor("land", "LandUse", new Action<float>((float deltaS1) => EBM.S1 = deltaS1)), //a0
		economy = new Factor("money", "Economy", new Action<float>((float delta) => money += delta)),
		opinion = new Factor("opinion", "PublicOpinion", new Action<float>((float delta) => publicOpinion += delta));

	public static Factor? GetFactor(string factor) {
		switch (factor) {
			case var f when new Regex(@"(co2|emissions)", RegexOptions.IgnoreCase).IsMatch(factor):
				return co2;
			case var f when new Regex(@"(land|albedo)", RegexOptions.IgnoreCase).IsMatch(factor):
				return albedo;
			case var f when new Regex(@"(money|economy)", RegexOptions.IgnoreCase).IsMatch(factor):
				return economy;
			case "opinion":
				return opinion;
			default:
				return null;
		}
	}

	public static void Init() {
		Calc();
		// FinishCalc(StartCalc().Wait());
	}

	public static void Calc(bool useTemp = false, int years = 0, int steps = 0) {
		var timer = new Stopwatch();
		timer.Start();
		(temp, energy, precip) = EBM.Calc(useTemp ? EBM.temp : null, years, steps);
		timer.Stop();
		averageTemp = temp.Average();
		Debug.Log($"Average Temp: {averageTemp} with regionals: {temp.AsString()}, calculated in {timer.ElapsedMilliseconds}ms");
	}
}
