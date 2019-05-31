using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ClimateMath : MonoBehaviour{
	//official scientific values from rcp graph projections
	public static Dictionary<int, Dictionary<string,float>> refs = new Dictionary<int, Dictionary<string,float>>(){
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
	public float[] Predict(float[] history, int year){
		float ema = EMA(history);
		float score = Mathf.InverseLerp(refs[year]["8.5"], refs[year]["2.6"], ema) * 3;
		List<float> prediction = new List<float>();
		foreach(KeyValuePair<int, Dictionary<string, float>> kvp in refs){
			if(kvp.Key>year)
				if(score>2)
					prediction.Add(Mathf.Lerp(kvp.Value["8.5"], kvp.Value["6.0"], score-2));
				else if(score<1)
					prediction.Add(Mathf.Lerp(kvp.Value["4.5"], kvp.Value["2.6"], score));
				else
					prediction.Add(Mathf.Lerp(kvp.Value["6.0"], kvp.Value["4.5"], score-1));
		}
		return prediction.ToArray();
	}

    //wip, calc changes in temp
	float[] CalcDeltas(float[] history){
		float[] avgs = new float[history.Length];
		return new float[1];
	}

    //calc temp weighted average, may refactor inline
    float CalcTemp(float[] temps, float[] weights){
        return temps.Aggregate((float sum, float cur)=>sum + cur * weights[Array.IndexOf(temps, cur)]);
    }

	//weighted average of array of numbers, good for temp/emissions/etc
	float EMA(float[] nums){
        // float[] prev = (float[])(new float[nums.Length]).Zip(nums, (a, b)=>nums[Array.IndexOf(nums, b)]);
        return nums[nums.Length-1] * 2/(nums.Length+1) + EMA((float[])(new float[nums.Length-1]).Zip(nums, (a, b)=>nums[Array.IndexOf(nums, b)])) * (1-2/(nums.Length+1));
		// float[] prev = (float[])nums.Clone();
		// Array.Resize(ref prev, nums.Length-1);
		// float k = 2/(nums.Length+1);
		// return nums[nums.Length-1] * k + EMA(prev) * (1-k);
	}
    
    #region physicsEqs

    public static readonly float Q = 341.3f; //standard amount of radiation in
    public static readonly float F = 101.9f; //standard amount of reflected heat
    public static float alpha = F / Q; //albedo
    public static readonly double C = 4E8;

    //outgoing longwave-radiation (reflected light)
    static readonly double sigma = 5.67E-8;
    static float tau = 0.61f; //will vary
    float OLR(float temp){ return tau * (float)sigma * Mathf.Pow(temp, 4); }
    float Tau(float olr, float temp) { return olr / (float)sigma / Mathf.Pow(temp, 4); }

    //absorbed shortwave radiation (light taken in)
    float ASR(){ return Q * (1 - alpha);}
    float ASR(float a /*custom alpha*/){ return Q * (1 - a);}

    //find equilibrium temp, where olr=asr
    float FindEquilibrium(float asr) { return Mathf.Pow(asr / tau / (float)sigma,.25f);}

    //get new temp given current conditions
    float FindTemp(float curTemp, float deltaT) {return curTemp + deltaT / (float)C * (ASR() - OLR(curTemp)); }

    #endregion
}
