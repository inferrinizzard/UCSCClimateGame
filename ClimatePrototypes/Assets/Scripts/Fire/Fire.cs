using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Fire : MonoBehaviour {
	[SerializeField] float fadeRate = 1f;
	public float health = 100f;
	Vector3 start, end = Vector3.one * .05f;
	int step = 0;

	void Start() {
		start = transform.localScale;
	}

	void Update() {
		if (step++ % 30 == 0)
			FireController.damage += fadeRate;
		if (health <= 0)
			Destroy(gameObject);
	}

	public void Fade() {
		health -= fadeRate;
		transform.localScale = Vector3.Slerp(start, end, (100f - health) / 100f);
	}
}
