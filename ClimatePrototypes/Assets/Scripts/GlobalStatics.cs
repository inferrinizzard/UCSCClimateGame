using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalStatics
{
	public static string[] regions = new string[] { "arctic", "desert", "forest", "tropics" };
	public static float temperature = 35f;
	public static float cashMoney = 100f;
	public static int turn = 1;
	public static float co2 = 400;    //global CO2 in ppm
	public static int actionsRemaining = 2;
	public static int billIndex = 0;
	public static Dictionary<string, float> temps = new Dictionary<string, float>() { { "arctic", 5 }, { "Pdesert", 40 }, { "forest", 25 }, { "tropics", 20 } };

	public static void updateTemp(string region, float deltaT)
	{
		float lookupPhi = 0; //using region name to look up phi value from chart
												 // temps[region] = ClimateMath.FindTemp(temps[region], deltaT, lookupPhi);
		temps[region]--;
	}

	public static void updateCO2(float ppm)
	{
		co2 += ppm;
		ClimateMath2.A = co2 / 1600 * 8 + 210;
		// ClimateMath.A = (1 + co2 / 1600) * 210;
	}
}