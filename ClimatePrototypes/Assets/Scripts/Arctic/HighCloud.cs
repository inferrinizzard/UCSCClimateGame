using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighCloud : MonoBehaviour {
	int heatThreshold = 4;
	int hits = 0;
	new SpriteRenderer renderer;

	void Start() {
		renderer = GetComponent<SpriteRenderer>();
		print(renderer.color);
	}

	void OnCollisionEnter2D(Collision2D other) {
		hits++;
		renderer.color -= new Color(0, .1f, .1f, 0);
		if (renderer.color.g < 1 - .1 * heatThreshold) {
			print("red");
			renderer.color = Color.white;
		}
	}
}
