using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Newtonsoft.Json;

using UnityEngine;
using UnityEngine.UI;

public class Graph : MonoBehaviour {
	List<LineRenderer> lr;

	public void Start() {
		var rcp = World.ranges["forcing"];
		lr = rcp.Select((kvp, i) => {
			var line = new GameObject(kvp.Key.ToString()).AddComponent<LineRenderer>();
			line.transform.SetParent(transform);
			return (line, kvp);
		}).
		// OrderBy(l => l.Item2.Key).
		// Take(rcp.Count() - 1).
		Select(l => {
			l.Item1.widthMultiplier = .15f;
			l.Item1.materials[0] = default(Material);
			l.Item1.materials[0].color = Color.white;
			l.Item1.materials[0].mainTextureScale = new Vector3(0, 1, 1);
			l.Item1.positionCount = l.Item2.Value.Count;
			l.Item1.SetPositions(
				l.Item2.Value.Map((n, i) => new Vector3(i, (float) n, 0)).ToArray()
			);
			return l.Item1;
		}).ToList();

		var bounds = lr.Select(l => l.bounds).Aggregate((b, cur) => { b.Encapsulate(cur); return b; });
		this.Print(bounds.size);

		lr.ForEach(l => {
			Vector3[] points = new Vector3[l.positionCount];
			l.GetPositions(points);
			l.SetPositions(points.Select(p => (p + Camera.main.ViewportToWorldPoint(new Vector2(0, 0))) * Camera.main.ViewportToWorldPoint(new Vector2(1, 1)).x * 1.75f / bounds.size.x + Vector3.right).ToArray());
		});

		// transform.position = Camera.main.ViewportToWorldPoint(new Vector2(0, 0));
		// transform.localScale = Vector3.one * Camera.main.ViewportToWorldPoint(new Vector2(1, 1)).x * 2 / bounds.size.x;
	}
	public void Update() {
		// Vector3[] pos = new Vector3[lr.positionCount];
		// lr.GetPositions(pos);
		// Debug.Log(String.Join(" ", pos));
	}

	public void AddPoint(float y) {
		LineRenderer FRenderer = lr[lr.Count - 1];
		FRenderer.positionCount++;
		FRenderer.SetPosition(FRenderer.positionCount - 1, new Vector3(FRenderer.positionCount - 1, y, 0));
	}
}
