using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldBubble : MonoBehaviour {
	GameObject bubble;
	bool active = false;

	// Start is called before the first frame update
	void Start() {
		bubble = transform.GetChild(0).gameObject;
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
			bubble.transform.localScale = (entering ? Mathf.Lerp(.001f, 10, step / dur) : Mathf.Lerp(10, .001f, step / dur)) * Vector3.one;
			if (step > dur)
				inProgress = false;
		}
		if (!entering)
			bubble.SetActive(false);

	}
}
