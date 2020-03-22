using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class BallBehavior : MonoBehaviour {
	public float upForce = 5f;
	public float sideForce = 2f;
	Rigidbody2D rb;
	Vector2 screenMin;
	Vector2 screenMax;

	// Start is called before the first frame update
	void Start() {
		screenMin = Camera.main.ViewportToWorldPoint(Vector2.zero);
		screenMax = Camera.main.ViewportToWorldPoint(Vector2.one);
		rb = GetComponent<Rigidbody2D>();

		Vector2 force = new Vector2(Random.Range(-sideForce, sideForce), -Random.Range(upForce * 0.8f, upForce));
		rb.velocity = force;
	}

	void Update() {
		if (transform.position.x < screenMin.x || transform.position.x > screenMax.x || transform.position.y < screenMin.y || (transform.position.y > screenMax.y && rb.velocity.y > 0))
			Destroy(gameObject);
	}
}
