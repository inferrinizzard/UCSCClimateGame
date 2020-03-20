using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class OverworldLine : MonoBehaviour {

	[SerializeField] float dist = 0;
	public float duration = .25f;
	List<LineRenderer> arrows = new List<LineRenderer> ();
	readonly Dictionary<string, Color> nodeColours = new Dictionary<string, Color> { { "Forest", Color.green }, { "Arctic", Color.cyan }, { "Tropic", Color.red }, };
	readonly Dictionary<string, WorldBubble> nodes = new Dictionary<string, WorldBubble> ();
	[SerializeField] Transform nodeParent = default;
	[SerializeField] GameObject baseLine = default;

	// Start is called before the first frame update
	void Start() {
		foreach(WorldBubble node in nodeParent.GetComponentsInChildren<WorldBubble> ())
			nodes.Add (node.name.Replace("Node", string.Empty), node);
		// print(nodes.AsString());
		// print(nodes["Forest"].icons.AsString());

		var logoCoroutine = ShowLogo(nodes["Forest"].icons[World.verbose["co2"]], 1);
		StartCoroutine(DrawLine(nodes["City"].transform.position, nodes["Forest"].transform.position, .5f, Color.green, logoCoroutine));
		StartCoroutine(DrawLine(nodes["City"].transform.position, nodes["Arctic"].transform.position, .5f, Color.cyan, logoCoroutine));

		foreach(var(from, to, tag) in GameManager.Instance.lineToDraw) {
			// this.print(from, to);
			// var logoCoroutine = ShowLogo(nodes[to].icons[World.verbose[tag]], .2f);
			StartCoroutine(DrawLine(nodes[from].transform.position, nodes[to].transform.position, duration, nodeColours[to], logoCoroutine));
		}

	}

	// Update is called once per frame
	void Update() { }

	IEnumerator DrawLine(Vector3 start, Vector3 dest, float time, Color c, IEnumerator showIcon, int verts = 100, float delay = 1) {
		GameObject lrgo = GameObject.Instantiate(baseLine, Vector3.zero, Quaternion.identity, transform);
		lrgo.SetActive(true);
		LineRenderer lr = lrgo.GetComponent<LineRenderer> ();
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
		List<Vector3> points = new int[verts].Map((_, i) => {
			var newAngle = Mathf.LerpAngle(startAngle, destAngle, (float) i / verts) * Mathf.Deg2Rad;
			return new Vector3(-Mathf.Cos (newAngle), -Mathf.Sin(newAngle), -1) * (centre - start).magnitude + centre;
		}).ToList();
		lr.SetPositions (points.Take (2).ToArray());
		float begin = Time.time;
		bool inProgress = true;

		while(inProgress) {
			yield return null;
			int curCount = lr.positionCount;
			float step = Time.time - begin;
			lr.positionCount = System.Math.Min((int) (step / time * verts), verts);
			for(int i = curCount; i < lr.positionCount; i++)
				lr.SetPosition(i, points[i]);
			if(step > time)
				inProgress = false;
		}
		StartCoroutine(showIcon);
		yield return new WaitForSeconds(delay);
		// lr.Simplify(1E-8);

		StartCoroutine(EraseLine(lr, points, .25f));
	}

	IEnumerator EraseLine(LineRenderer lr, List<Vector3> points, float time) {
		int verts = points.Count;
		float begin = Time.time;
		bool inProgress = true;

		while(inProgress) {
			yield return null;
			int curCount = lr.positionCount;
			float step = Time.time - begin;
			lr.positionCount = System.Math.Max((int) ((1 - step / time) * verts), 0);
			lr.SetPositions (points.Skip (verts - lr.positionCount).ToArray());
			if(step > time)
				inProgress = false;
		}
		// lr.gameObject.SetActive(false);
		Destroy(lr.gameObject);
	}

	IEnumerator ShowLogo(SpriteRenderer icon, float time) {
		float begin = Time.time;
		bool inProgress = true;

		float fadeIn = time / 2;
		icon.gameObject.SetActive(true);

		while(inProgress) {
			yield return null;
			float step = Time.time - begin;

			if(step < fadeIn)
				icon.color = new Color(icon.color.r, icon.color.g, icon.color.b, step / fadeIn);
			else
				icon.color = new Color(icon.color.r, icon.color.g, icon.color.b, 1 - step / time);

			if(step > time)
				inProgress = false;
		}
		icon.gameObject.SetActive(false);
	}
}
