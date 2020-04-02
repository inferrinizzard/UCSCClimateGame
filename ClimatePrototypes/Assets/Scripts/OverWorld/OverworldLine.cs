using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class OverworldLine : MonoBehaviour {
	[SerializeField] float dist = 0;
	public float duration = .25f;
	// List<LineRenderer> arrows = new List<LineRenderer>();

	[HideInInspector] public WorldBubble CityNode, ForestNode, ArcticNode, FireNode;
	[SerializeField] Transform nodeParent = default;
	GameObject baseLine;

	System.Reflection.FieldInfo Fetch(string name) => this.GetType().GetField(name);

	void Awake() {
		baseLine = GetComponentInChildren<LineRenderer>(true).gameObject;
		foreach (WorldBubble node in nodeParent.GetComponentsInChildren<WorldBubble>())
			Fetch(node.name).SetValue(this, node);
	}

	void Start() {
		foreach (var(from, to, tag) in World.lineToDraw) {
			WorldBubble dest = Fetch($"{to.ToString()}Node").GetValue(this) as WorldBubble;
			StartCoroutine(DrawLine(Fetch($"{from.ToString()}Node").GetValue(this) as WorldBubble, dest, dest.colour, tag));
		}
		World.lineToDraw = new List < (World.Region, World.Region, string) > ();
	}

	// Update is called once per frame
	void Update() { }

	IEnumerator DrawLine(WorldBubble startNode, WorldBubble destNode, Color c, string factor, float time = -1, int verts = 100, float delay = 1) {
		Vector3 start = startNode.transform.position, dest = destNode.transform.position;
		time = time == -1 ? duration : time;

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

		// TODO: animation curve lerp
		List<Vector3> points = new int[verts].Map((_, i) =>
			Func.Lambda<float, Vector3>((float newAngle) => new Vector3(-Mathf.Cos(newAngle), -Mathf.Sin(newAngle), -1) * (centre - start).magnitude + centre)
			(Mathf.LerpAngle(startAngle, destAngle, (float) i / verts) * Mathf.Deg2Rad)
		).ToList();
		lr.SetPositions(points.Take(2).ToArray());
		float begin = Time.time;
		bool inProgress = true;

		while (inProgress) {
			yield return null;
			int curCount = lr.positionCount;
			float step = Time.time - begin;
			lr.positionCount = System.Math.Min((int) (step / time * verts), verts);
			for (int i = curCount; i < lr.positionCount; i++)
				lr.SetPosition(i, points[i]);
			if (step > time)
				inProgress = false;
		}
		StartCoroutine(ShowLogo(destNode.icons[World.GetFactor(factor)?.verbose], delay));
		yield return new WaitForSeconds(delay);
		// lr.Simplify(1E-8);

		StartCoroutine(EraseLine(lr, points, .25f));
	}

	IEnumerator EraseLine(LineRenderer lr, List<Vector3> points, float time) {
		int verts = points.Count;
		float begin = Time.time;
		bool inProgress = true;

		while (inProgress) {
			yield return null;
			int curCount = lr.positionCount;
			float step = Time.time - begin;
			lr.positionCount = System.Math.Max((int) ((1 - step / time) * verts), 0);
			lr.SetPositions(points.Skip(verts - lr.positionCount).ToArray());
			if (step > time)
				inProgress = false;
		}
		// lr.gameObject.SetActive(false);
		Destroy(lr.gameObject);
	}

	IEnumerator ShowLogo(SpriteRenderer icon, float time) {
		float begin = Time.time;
		bool inProgress = true;

		float fadeIn = time / 2;
		icon.color = new Color(icon.color.r, icon.color.g, icon.color.b, 0);
		icon.gameObject.SetActive(true);

		while (inProgress) {
			yield return null;
			float step = Time.time - begin;
			icon.color = new Color(icon.color.r, icon.color.g, icon.color.b, step < fadeIn ? step / fadeIn : 1 - step / time);
			if (step > time)
				inProgress = false;
		}
		icon.gameObject.SetActive(false);
	}
}
