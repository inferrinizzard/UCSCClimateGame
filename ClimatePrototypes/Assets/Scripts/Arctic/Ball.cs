using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Ball : MonoBehaviour {
	/// <summary> . </summary>
	Vector2 force = new Vector2(2, 5);
	Rigidbody2D rb;
	Vector2 screenMin, screenMax;

	// Start is called before the first frame update
	void Start() {
		screenMin = Camera.main.ViewportToWorldPoint(Vector2.zero);
		screenMax = Camera.main.ViewportToWorldPoint(Vector2.one);
		rb = GetComponent<Rigidbody2D>();

		rb.velocity = new Vector2(Random.Range(-force.x, force.x), -Random.Range(force.y * 0.8f, force.y));
	}

	void Update() {
		// check position
		if (transform.position.x < screenMin.x || transform.position.x > screenMax.x || transform.position.y < screenMin.y || (transform.position.y > screenMax.y && rb.velocity.y > 0))
			Destroy(gameObject);
	}
}
