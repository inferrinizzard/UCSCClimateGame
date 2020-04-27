using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Newtonsoft.Json;

using UnityEngine;
using UnityEngine.UI;

public class Graph : MonoBehaviour {
	LineRenderer[] lr;
	public static Dictionary<string, double[]> rcp;

	public void Start() {
		var rcp_raw = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, double[]>>>(new StreamReader(Directory.GetFiles(Directory.GetCurrentDirectory(), $"ipcc.json", SearchOption.AllDirectories) [0]).ReadToEnd());
		rcp = rcp_raw["forcing"];
		lr = rcp.Select((kvp, i) => {
			var line = new GameObject(kvp.Key).AddComponent<LineRenderer>();
			line.transform.SetParent(transform);
			return (line, kvp);
		}).
		OrderBy(l => l.Item2.Key).
		Take(rcp.Count() - 1).
		Select(l => {
			l.Item1.materials[0] = default(Material);
			l.Item1.materials[0].color = Color.white;
			l.Item1.materials[0].mainTextureScale = new Vector3(0, 1, 1);
			l.Item1.positionCount = l.Item2.Value.Length;
			l.Item1.SetPositions(
				l.Item2.Value.Map((n, i) => new Vector3(i, (float) n, 0)).ToArray()
			);
			return l.Item1;
		}).ToArray();
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
