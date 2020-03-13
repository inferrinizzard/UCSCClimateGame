using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldBubble : MonoBehaviour {

	[SerializeField] float size = 8;
	GameObject bubble;
	bool active = false;
	Vector3 startPos;
	public Dictionary<string, SpriteRenderer> icons = new Dictionary<string, SpriteRenderer>();

	// Start is called before the first frame update
	void Awake() {
		bubble = transform.GetChild(0).gameObject;
		startPos = bubble.transform.localPosition;
		bubble.transform.localScale = Vector3.one * .01f;
		foreach (SpriteRenderer icon in transform.Find("Icons").GetComponentsInChildren<SpriteRenderer>())
			icons.Add(icon.name.Replace("Icon", string.Empty), icon);
		foreach (var i in icons)
			i.Value.gameObject.SetActive(false);
	}

	// Update is called once per frame
	void Update() { }

	void OnMouseEnter() {
		if (!active)
			StartCoroutine(Bubble(entering: true, dur: .25f));
	}

	void OnMouseOver() {
		if (!active)
			StartCoroutine(Bubble(entering: true, dur: .25f));
		if (Input.GetButtonDown("Fire1")) {
			GameManager.Transition(gameObject.name.Replace("Node", string.Empty));
			// Debug.Log(gameObject.name.Replace("Node", string.Empty));
		}
	}

	void OnMouseExit() {
		if (active)
			StartCoroutine(Bubble(entering: false, dur: .25f));
	}

	IEnumerator Bubble(bool entering, float dur, Vector2 pos = default(Vector2)) {
		active = entering;
		float start = Time.time;
		if (entering)
			bubble.SetActive(true);
		bool inProgress = true;
		while (inProgress) {
			yield return null;
			float step = Time.time - start;
			bubble.transform.localScale = (entering ? Mathf.Lerp(.001f, size, step / dur) : Mathf.Lerp(size, .001f, step / dur)) * Vector3.one;
			bubble.transform.localPosition = entering ? Vector3.Lerp(transform.position, startPos, step / dur) : Vector3.Lerp(startPos, transform.position, step / dur);
			if (step > dur)
				inProgress = false;
		}
		if (!entering)
			bubble.SetActive(false);

	}
}
