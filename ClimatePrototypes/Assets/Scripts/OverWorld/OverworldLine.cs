using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OverworldLine : MonoBehaviour {

	[SerializeField] float dist = 0;
	public float duration = .25f;
	List<LineRenderer> arrows = new List<LineRenderer>();
	Dictionary<string, Vector3> nodes = new Dictionary<string, Vector3>();
	Dictionary<string, Color> nodeColours = new Dictionary<string, Color> { { "Forest", Color.green }, { "Arctic", Color.cyan }, { "Tropic", Color.red }, };
	[SerializeField] Transform nodeParent = default;
	[SerializeField] GameObject baseLine = default;

	// Start is called before the first frame update
	void Start() {
		foreach (Transform child in nodeParent)
			nodes.Add(child.name, child.position);

		StartCoroutine(DrawLine(nodes["CityNode"], nodes["ForestNode"], .5f, Color.green));
		StartCoroutine(DrawLine(nodes["CityNode"], nodes["ArcticNode"], .5f, Color.cyan));

		foreach (var(from, to)in GameManager.Instance.lineToDraw) {
			this.print(from, to);
			// StartCoroutine(DrawLine(nodes[$"{from}Node"], nodes[$"{to}Node"], duration, nodeColours[to]));
		}

	}

	// Update is called once per frame
	void Update() { }

	IEnumerator DrawLine(Vector3 start, Vector3 dest, float time, Color c, int verts = 100) {
		GameObject lrgo = GameObject.Instantiate(baseLine, Vector3.zero, Quaternion.identity, transform);
		lrgo.SetActive(true);
		LineRenderer lr = lrgo.GetComponent<LineRenderer>();
		lr.material.color = c;
		lr.positionCount = 2;

		// float mag = (dest - start).magnitude / 2;
		// float angle = Mathf.Atan2((dest - start).y, (dest - start).x);

		Vector3 half = Vector3.Lerp(start, dest, .5f);
		var dir = Vector2.Perpendicular(dest - start).normalized * dist;
		var centre = half + new Vector3(dir.x, dir.y, 0);

		var startAngle = Mathf.Atan2((centre - start).y, (centre - start).x) * Mathf.Rad2Deg;
		var destAngle = Mathf.Atan2((centre - dest).y, (centre - dest).x) * Mathf.Rad2Deg;

		List<Vector3> points = new int[verts].Map((_, i) => {
			var newAngle = Mathf.LerpAngle(startAngle, destAngle, (float)i / verts) * Mathf.Deg2Rad;
			return new Vector3(-Mathf.Cos(newAngle), -Mathf.Sin(newAngle), -1) * (centre - start).magnitude + centre;
		}).ToList();
		print(points[0]);
		lr.SetPositions(points.Take(2).ToArray());
		float begin = Time.time;
		bool inProgress = true;

		while (inProgress) {
			yield return null;
			int curCount = lr.positionCount;
			float step = Time.time - begin;
			lr.positionCount = System.Math.Min((int)(step / time * verts), verts);
			for (int i = curCount; i < lr.positionCount; i++)
				lr.SetPosition(i, points[i]);
			if (step > time)
				inProgress = false;
		}
		yield return new WaitForSeconds(1);
		// lr.Simplify(1E-8);

		// StartCoroutine(EraseLine(lr, points, .25f));
	}

	IEnumerator EraseLine(LineRenderer lr, List<Vector3> points, float time) {
		int verts = points.Count;
		float begin = Time.time;
		bool inProgress = true;

		while (inProgress) {
			yield return null;
			int curCount = lr.positionCount;
			float step = Time.time - begin;
			lr.positionCount = System.Math.Max((int)((1 - step / time) * verts), 0);
			lr.SetPositions(points.Skip(verts - lr.positionCount).ToArray());
			if (step > time)
				inProgress = false;
		}
		// lr.gameObject.SetActive(false);
		Destroy(lr.gameObject);
	}
}
