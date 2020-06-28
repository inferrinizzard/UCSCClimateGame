using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class OverworldController : MonoBehaviour {
	[SerializeField] GameObject worldWrapper = default;
	[SerializeField] SpriteRenderer bg = default;

	public void SendToBottom() {
		Camera.main.transform.position = Vector3.forward * -10;
		bg.transform.position = new Vector3(bg.transform.position.x, -Camera.main.ViewportToWorldPoint(Vector2.zero).y - bg.bounds.extents.y, bg.transform.position.z);
		Camera.main.transform.position = new Vector3(0, bg.bounds.min.y - Camera.main.ViewportToWorldPoint(Vector2.zero).y, -10);
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
