using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Graph : MonoBehaviour {
	LineRenderer[] lr;
	public static readonly Dictionary<string, double[]> rcp = new Dictionary<string, double[]>() { { "years", new double[] { 2000, 2005, 2010, 2020, 2030, 2040, 2050, 2060, 2070, 2080, 2090, 2100 } }, { "2.6", new double[] { 1.723, 1.904, 2.129, 2.584, 2.862, 2.999, 2.998, 2.918, 2.854, 2.808, 2.759, 2.714 } }, { "4.5", new double[] { 1.723, 1.905, 2.126, 2.579, 3.005, 3.411, 3.766, 4.021, 4.188, 4.256, 4.265, 4.309 } }, { "6.0", new double[] { 1.723, 1.901, 2.089, 2.480, 2.854, 3.146, 3.521, 3.905, 4.443, 4.932, 5.255, 5.481 } }, { "8.5", new double[] { 1.723, 1.906, 2.154, 2.665, 3.276, 3.993, 4.762, 5.539, 6.299, 7.020, 7.742, 8.388 } },
	};
	public void Start() {
		lr = GetComponentsInChildren<LineRenderer>();
		// line.materials[0].mainTextureScale = new Vector3(distance, 1, 1);
		lr.OrderBy(l => l.name).
		Take(lr.Count() - 1).
		Map(l => {
			LineRenderer _lr = l.GetComponent<LineRenderer>();
			_lr.positionCount = rcp[_lr.name].Length;
			return _lr;
		}).ForEach(l =>
			l.SetPositions(
				rcp[l.name].Map((n, i) => new Vector3(i, (float)n, 0)).ToArray()
			)
		);
	}
	public void Update() {
		// Vector3[] pos = new Vector3[lr.positionCount];
		// lr.GetPositions(pos);
		// Debug.Log(String.Join(" ", pos));
	}

	public void AddPoint(float y) {
		LineRenderer FRenderer = lr[lr.Length - 1];
		FRenderer.positionCount++;
		FRenderer.SetPosition(FRenderer.positionCount - 1, new Vector3(FRenderer.positionCount - 1, y, 0));
	}
}
