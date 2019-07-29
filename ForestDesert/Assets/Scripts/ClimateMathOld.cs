using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ClimateMath2
{
	//official scientific values from rcp graph projections
	public static Dictionary<int, Dictionary<string, float>> refs = new Dictionary<int, Dictionary<string, float>>(){
		{2000, new Dictionary<string,float>(){{"8.5", 1.723f}, {"6.0", 1.723f}, {"4.5", 1.723f}, {"2.6", 1.723f}} },
		{2005, new Dictionary<string,float>(){{"8.5", 1.906f}, {"6.0", 1.901f}, {"4.5", 1.905f}, {"2.6", 1.904f}} },
		{2010, new Dictionary<string,float>(){{"8.5", 2.154f}, {"6.0", 2.089f}, {"4.5", 2.126f}, {"2.6", 2.129f}} },
		{2020, new Dictionary<string,float>(){{"8.5", 2.665f}, {"6.0", 2.480f}, {"4.5", 2.579f}, {"2.6", 2.584f}} },
		{2030, new Dictionary<string,float>(){{"8.5", 3.276f}, {"6.0", 2.854f}, {"4.5", 3.005f}, {"2.6", 2.862f}} },
		{2040, new Dictionary<string,float>(){{"8.5", 3.993f}, {"6.0", 3.146f}, {"4.5", 3.411f}, {"2.6", 2.999f}} },
		{2050, new Dictionary<string,float>(){{"8.5", 4.762f}, {"6.0", 3.521f}, {"4.5", 3.766f}, {"2.6", 2.998f}} },
		{2060, new Dictionary<string,float>(){{"8.5", 5.539f}, {"6.0", 3.905f}, {"4.5", 4.021f}, {"2.6", 2.918f}} },
		{2070, new Dictionary<string,float>(){{"8.5", 6.299f}, {"6.0", 4.443f}, {"4.5", 4.188f}, {"2.6", 2.854f}} },
		{2080, new Dictionary<string,float>(){{"8.5", 7.020f}, {"6.0", 4.932f}, {"4.5", 4.256f}, {"2.6", 2.808f}} },
		{2090, new Dictionary<string,float>(){{"8.5", 7.742f}, {"6.0", 5.255f}, {"4.5", 4.265f}, {"2.6", 2.759f}} },
		{2100, new Dictionary<string,float>(){{"8.5", 8.388f}, {"6.0", 5.481f}, {"4.5", 4.309f}, {"2.6", 2.714f}} },
	};

	//takes in array of previous temps and current year according to ^, returns array of temps for remaining years
	// limited to above indices
	public static float[] Predict(float[] history, int year)
	{
		float ema = EMA(history);
		float score = Mathf.InverseLerp(refs[year]["8.5"], refs[year]["2.6"], ema) * 3;
		List<float> prediction = new List<float>();
		foreach (KeyValuePair<int, Dictionary<string, float>> kvp in refs)
		{
			if (kvp.Key > year)
				if (score > 2)
					prediction.Add(Mathf.Lerp(kvp.Value["8.5"], kvp.Value["6.0"], score - 2));
				else if (score < 1)
					prediction.Add(Mathf.Lerp(kvp.Value["4.5"], kvp.Value["2.6"], score));
				else
					prediction.Add(Mathf.Lerp(kvp.Value["6.0"], kvp.Value["4.5"], score - 1));
		}
		return prediction.ToArray();
	}

	//wip, calc changes in temp
	static float[] CalcDeltas(float[] history)
	{
		float[] avgs = new float[history.Length];
		return new float[1];
	}

	//calc temp weighted average, may refactor inline
	static float CalcTemp(float[] temps, float[] weights)
	{
		return temps.Aggregate((float sum, float cur) => sum + cur * weights[Array.IndexOf(temps, cur)]);
	}

	//weighted average of array of numbers, good for temp/emissions/etc
	static float EMA(float[] nums)
	{
		// float[] prev = (float[])(new float[nums.Length]).Zip(nums, (a, b)=>nums[Array.IndexOf(nums, b)]);
		return nums[nums.Length - 1] * 2 / (nums.Length + 1) + EMA((float[])(new float[nums.Length - 1]).Select((a, b) => nums[Array.IndexOf(nums, b)])) * (1 - 2 / (nums.Length + 1));
		// float[] prev = (float[])nums.Clone();
		// Array.Resize(ref prev, nums.Length-1);
		// float k = 2/(nums.Length+1);
		// return nums[nums.Length-1] * k + EMA(prev) * (1-k);
	}

	#region physicsEqs

	public static readonly float Q = 341.3f;  //global standard amount of radiation in
	public static readonly Dictionary<int, float> sol = new Dictionary<int, float>() { { 75, 200 }, { 45, 300 }, { 30, 350 }, { 15, 400 } }; //standard amount of radiation in
	public static readonly float F = 101.9f;  //standard amount of reflected heat
	public static float alpha = F / Q;  //albedo
	public static float A = 210;    //longwave emission at 0ºC in W m −2, +4 with double carbon, +8 with quad
	public static readonly float B = 2f;    //increase in emission per degree, in W m −2  ºC −1
	public static readonly double C = 4E8;    //heat capacity constant
	public static readonly float D = 0.6f;    //horizontal (north-south) diffusivity of the climate system in W m −2  ºC −1
	public static readonly double sigma = 5.67E-8; //Stefan-Boltzmann constant
	public static readonly double a = 6.378E6; //radius of Earth

	//double check and send constants

	public static readonly int dt = 60 * 60 * 24 * 365 / 90; //90 steps per year

	public static readonly Dictionary<int, float> albedo = new Dictionary<int, float>() { { 15, .1f }, { 45, .2f }, { 30, .3f }, { 75, .6f } };

	//outgoing longwave-radiation (reflected light)
	public static float tau = 0.61f; //will vary
	public static float OLR(float temp, float tau) { return tau * (float)sigma * Mathf.Pow(temp, 4); }
	public static double OLR(double temp) { return A + B * temp; }
	public static float Tau(float olr, float temp) { return olr / (float)sigma / Mathf.Pow(temp, 4); }

	//absorbed shortwave radiation (light taken in) 
	public static float ASR()
	{ return Q * (1 - alpha); }
	public static float ASR(float a /*custom alpha*/) { return Q * (1 - a); }
	public static float ASR(int phi) { return sol[phi] * (1 - albedo[phi]); }

	//find equilibrium temp, where olr=asr
	public static float FindEquilibrium(float asr) { return Mathf.Pow(asr / tau / (float)sigma, .25f); }
	public static float FindDeltaPerPhi(float phi, float deltaTperPhi, float deltaPhi)
	{
		return D / Mathf.Cos(Mathf.Deg2Rad * phi) / (Mathf.Deg2Rad * deltaPhi)
		* (Mathf.Cos(Mathf.Deg2Rad * phi) * deltaTperPhi / (Mathf.Deg2Rad * deltaPhi));
		//change in (Mathf.Cos(Mathf.Deg2Rad * phi) * deltaTperPhi / (Mathf.Deg2Rad * deltaPhi)) per phi / dH?
		//see ebm climlab code
		//maybe have temperatures on boundaries
	}

	public static float H(float phi, float deltaTperPhi, float deltaPhi)
	{
		return (float)(-2 * Mathf.PI * (float)(a * a)
						* Mathf.Cos(Mathf.Deg2Rad * phi)
						* D * deltaTperPhi / (Mathf.Deg2Rad * deltaPhi) * 1E-15);
	}

	//get new temp given current conditions

	// public static float FindTemp(float curTemp, float deltaTperPhi, float deltaPhi)
	// {
	// 	Debug.Log("cur " + curTemp);
	// 	Debug.Log("asr " + ASR());
	// 	Debug.Log("olr " + OLR(curTemp));
	// 	Debug.Log("d/phi " + FindDeltaPerPhi(deltaTperPhi, deltaPhi));
	// 	Debug.Log("delta " + FindDeltaPerPhi(deltaTperPhi, deltaPhi) * dt);
	// 	return (curTemp + ASR() - OLR(curTemp) + FindDeltaPerPhi(deltaTperPhi, deltaPhi) * dt) / (float)C;
	// }
	public static double FindTemp(float phi, double curTemp, float deltaTperPhi, float deltaPhi)
	{
		// Debug.Log("cur " + curTemp);
		// Debug.Log("phi " + phi);
		// Debug.Log("asr " + ASR((int)phi));
		// Debug.Log("olr " + OLR(curTemp));
		// Debug.Log("d/phi " + FindDeltaPerPhi(deltaTperPhi, deltaPhi));
		// Debug.Log("delta " + FindDeltaPerPhi(deltaTperPhi, deltaPhi) * dt);
		Debug.Log("delta " + H(phi, deltaTperPhi, deltaPhi)
			/ Mathf.Deg2Rad * deltaPhi
			/ Mathf.Cos(Mathf.Deg2Rad * phi)
			/ (float)(a * a) / -2 * dt);

		return
			curTemp + (
				ASR((int)phi)
			- OLR(curTemp)
			+ H(phi, deltaTperPhi, deltaPhi)
			/ Mathf.Deg2Rad * deltaPhi
			/ Mathf.Cos(Mathf.Deg2Rad * phi)
			/ (a * a) / -2 * dt)
			/ C;

		// return
		// 	curTemp + (
		// 		ASR((int)phi)
		// 	- OLR(curTemp)
		// 	+ FindDeltaPerPhi(phi, deltaTperPhi, deltaPhi) * dt)
		// 	/ (float)C;
	}

	#endregion
}
