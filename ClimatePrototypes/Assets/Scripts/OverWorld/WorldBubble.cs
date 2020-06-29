using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class WorldBubble : MonoBehaviour {
	[SerializeField] float size = 8;
	public Color colour;
	GameObject bubble;
	bool active = false;
	Vector3 startPos;
	public Dictionary<string, SpriteRenderer> icons = new Dictionary<string, SpriteRenderer>();

	void Awake() {
		bubble = transform.GetChild(0).gameObject;
		startPos = bubble.transform.localPosition;
		bubble.transform.localScale = Vector3.one * .01f;
		foreach (SpriteRenderer icon in transform.Find("Icons").GetComponentsInChildren<SpriteRenderer>()) {
			icons.Add(icon.name.Replace("Icon", string.Empty), icon);
			icon.gameObject.SetActive(false);
		}
	}

	void OnMouseEnter() {
		if (!active)
			StartCoroutine(Bubble(entering: true, dur: .25f));
	}

	void OnMouseOver() {
		if (!active)
			StartCoroutine(Bubble(entering: true, dur: .25f));
		if (Input.GetButtonDown("Fire1")) {
			foreach (var node in transform.parent.GetComponentsInChildren<WorldBubble>())
				foreach (var kvp in node.icons)
					kvp.Value.gameObject.SetActive(false);
			StartCoroutine(EnterRegion(new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y, 0)));
		}
	}

	void OnMouseExit() {
		if (active)
			StartCoroutine(Bubble(entering: false, dur: .25f));
	}

	IEnumerator Bubble(bool entering, float dur, Vector2 pos = default(Vector2)) {
		active = entering;
		if (entering)
			bubble.SetActive(true);
		for (var(start, step) = (Time.time, 0f); step < dur; step = Time.time - start) {
			yield return null;
			bubble.transform.localScale = (entering ? Mathf.Lerp(.001f, size, step / dur) : Mathf.Lerp(size, .001f, step / dur)) * Vector3.one;
			bubble.transform.localPosition = entering ? Vector3.Lerp(transform.position, startPos, step / dur) : Vector3.Lerp(startPos, transform.position, step / dur);
		}
		if (!entering)
			bubble.SetActive(false);
	}

	IEnumerator EnterRegion(Vector3 bubblePos, float time = .5f) {
		StartCoroutine(UIController.SlideNav(UIController.Instance.navbar.transform, up : true));
		Vector3 camStartPos = Camera.main.transform.position;
		for (var(start, step) = (Time.time, 0f); step < time; step = Time.time - start) {
			yield return null;
			Camera.main.transform.position = Vector3.Lerp(camStartPos, bubblePos, step / time);
			Camera.main.orthographicSize = 5 * (1 - step / time); // slow
			Camera.main.GetComponent<OverworldController>().fadeMat.SetFloat("_Alpha", step / time); // slow
		}
		GameManager.Transition(name.Replace("Node", string.Empty));
		// Shader.SetGlobalFloat("_Alpha", 1);
	}
}
