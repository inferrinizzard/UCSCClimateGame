using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class OverworldController : MonoBehaviour {
	[SerializeField] GameObject worldWrapper;

	void Start() {

	}

	// Update is called once per frame
	void Update() {

	}

	public void ClearWorld() {
		foreach (SpriteRenderer sr in worldWrapper.GetComponentsInChildren<SpriteRenderer>())
			sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0);
	}

	public IEnumerator EnterWorld(float time = 1) {
		ClearWorld();
		SpriteRenderer[] sprites = worldWrapper.GetComponentsInChildren<SpriteRenderer>();
		for (var(start, step) = (Time.time, 0f); step < time; step = Time.time - start) {
			yield return null;
			foreach (var sr in sprites)
				sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, step);
		}
	}
}
