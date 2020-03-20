using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class HighCloud : MonoBehaviour {
	// int heatThreshold = 3;
	// int hits = 0;
	new SpriteRenderer renderer;
	Vector2 screenMin;
	private Vector2 screenMax;
	public float sideForce = 5f;
	Rigidbody2D rb;

	void Start() {
		renderer = GetComponent<SpriteRenderer>();
		print(renderer.color);

		screenMin = Camera.main.ViewportToWorldPoint(Vector2.zero);
		screenMax = Camera.main.ViewportToWorldPoint(Vector2.one);
		rb = GetComponent<Rigidbody2D>();

		Vector2 force = new Vector2(sideForce, 0);
		rb.velocity = force;

	}

	/*void OnCollisionEnter2D(Collision2D other) {
		hits++;
		//renderer.color -= new Color(0, .1f, .1f, 0);
		if (renderer.color.g < 1 - .1 * heatThreshold) {
			Destroy(other.gameObject);
			//renderer.color = Color.white;
		}
	}*/

	void Update() {

		if (transform.position.x < screenMin.x || transform.position.x > screenMax.x)
			Destroy(gameObject);
	}
}
