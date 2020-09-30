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
	public static string worldName = "";
	public static float money = 100f, publicOpinion = 0f;
	public static int turn = 1;
	public static double[] temp, energy, precip, startingTemp;
	public static float maxTempChange = 10f;
	public static double averageTemp { get => temp?.Average() ?? 0; }
	public static Impact impact = Impact.Stage1;
	public static Dictionary<string, Dictionary<double, List<double>>> ranges;

	public enum Impact { Stage1, Stage2, Stage3, Stage4, Stage5 }

	public static void DetermineImpact() {
		var avgTemp = averageTemp;
		impact = (Impact) Math.Max(0, ranges["temperature"].ToList().FindIndex(kvp => kvp.Value[turn] > avgTemp) - 1);
	}

	public enum Region { Arctic, City, Forest, Fire }
	public struct Factor {
		public string name, verbose;
		Action<double> update;

		public Factor(string name, string fullName, Action<double> updateFunction) {
			this.name = name;
			verbose = fullName;
			update = updateFunction;
		}

		public void Update(Region scene, Region? dest, double delta) {
			lineToDraw.Add((scene, dest ?? Region.Forest, name));
			Debug.Log($"change {verbose} by {delta}");
			update.Invoke(delta);
		}
	}

	public static List < (Region, Region, string) > lineToDraw = new List < (Region, Region, string) > ();

	public static readonly Factor co2 = new Factor("co2", "Emissions", new Action<double>(deltaF => EBM.F += deltaF)),
		albedo = new Factor("land", "LandUse", new Action<double>(deltaa0 => EBM.a0 += deltaa0)), //was s1
		economy = new Factor("money", "Economy", new Action<double>(delta => money += (float) delta)),
		opinion = new Factor("opinion", "PublicOpinion", new Action<double>(delta => publicOpinion += (float) delta));

	public static Factor? GetFactor(string factor) {
		bool RegexCheck(string pattern) => new Regex(pattern, RegexOptions.IgnoreCase).IsMatch(factor);
		switch (factor) {
			case var _ when RegexCheck(@"(co2|emissions)"):
				return co2;
			case var _ when RegexCheck(@"(land|albedo)"):
				return albedo;
			case var _ when RegexCheck(@"(money|economy)"):
				return economy;
			case "opinion":
				return opinion;
			default:
				return null;
		}
	}

	public static void Init() {
		Calc();
		startingTemp = new double[temp.Length];
		temp.CopyTo(startingTemp, 0);
		// FinishCalc(StartCalc().Wait());
	}

	public static void Calc(bool useTemp = true, int years = 0, int steps = 0) {
		var timer = new Stopwatch();
		timer.Start();
		(temp, energy, precip) = EBM.Calc(useTemp ? EBM.temp : null, years, steps);
		timer.Stop();
		Debug.Log($"Average Temp: {averageTemp} with regionals: {temp.AsString()}, calculated in {timer.ElapsedMilliseconds}ms");
	}
}
