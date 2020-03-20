using System;
using System.Collections.Generic;
using System.Linq;

using MathNet.Numerics.LinearAlgebra;
using Stopwatch = System.Diagnostics.Stopwatch;

public static class SpeedTest {
	public static void VectorAllocTest() {
		int bands = 24;
		double dx = 1f / bands;
		Vector<double> x = Vector<double>.Build.Dense(bands, i => dx / 2 + i++ * dx);
		Vector<double> T = 7.5f + 20 * (1 - 2 * x.PointwisePower(2));

		var timer = new Stopwatch();
		timer.Start();
		for (int i = 0; i < 100000; i++) {
			Vector<double> t = T * 1234;
		}
		timer.Stop();
		UnityEngine.Debug.Log(timer.ElapsedMilliseconds);

		Vector<double> test = T;
		timer.Restart();
		for (int i = 0; i < 100000; i++) {
			test = T * 1234;
		}
		timer.Stop();
		UnityEngine.Debug.Log(timer.ElapsedMilliseconds);
	}
}
