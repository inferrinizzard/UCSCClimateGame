using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathClient : MonoBehaviour
{
	// Start is called before the first frame update
	void Start()
	{
		float[] sol = RunTempH(new float[3] { 10, 10, 10, }, 1000);
		Debug.Log(String.Join(" ", sol));
		RunTempF();
		// float[] temp = new float[3] { 10, 10, 10 };
		// ClimateMath.odeIce(out temp);
	}

	// Update is called once per frame
	void Update()
	{
	}

	float[] RunTemp(float[] temp, int steps)
	{
		float[] time = new float[steps];
		for (int i = 0; i < steps; i++)
			time[i] = i * 30f / steps;
		float[] t = temp;
		Debug.Log("t: " + String.Join(" ", t));
		for (int i = 0; i < steps; i++)
		{
			t = t.Zip(ClimateMath.ode(t, time[i]), (a, b) => a + b / (float)steps * (10)).ToArray();
			Debug.Log("t" + (i + 1) + ": " + String.Join(" ", t));
		}
		return t;
	}

	float[] RunTempH(float[] temp, int steps)
	{
		float[] time = new float[steps];
		for (int i = 0; i < steps; i++)
			time[i] = i * 30f / steps;
		float[] h = temp;
		Debug.Log("h: " + String.Join(" ", h));
		for (int i = 0; i < steps; i++)
		{
			h = h.Zip(ClimateMath.odeMoist(h, time[i]), (a, b) => a + b / (float)steps * (10)).ToArray();
			// Debug.Log("h" + (i + 1) + ": " + String.Join(" ", h));
			if (i == 3)
				return new float[0];
		}
		return h;
	}

	float[] RunTempF()
	{
		ClimateMath.odeFast(new float[0], 0);
		return new float[0];
	}
}
