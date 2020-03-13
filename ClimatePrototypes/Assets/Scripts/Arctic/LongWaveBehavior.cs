using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongWaveBehavior : MonoBehaviour {
	private float upForce = 10f;
	private float sideForce = 10f;
	Rigidbody2D rb;
	Vector2 screenMin;
	Vector2 screenMax;
	// Start is called before the first frame update
	void Start() {
		screenMin = Camera.main.ViewportToWorldPoint(Vector2.zero);
		screenMax = Camera.main.ViewportToWorldPoint(Vector2.one);
		rb = GetComponent<Rigidbody2D>();

		Vector2 force = new Vector2(Random.Range(-sideForce, sideForce), Random.Range(-upForce, upForce));
		rb.velocity = force.normalized * 5f;
	}

	void Update() {

		if (transform.position.x < screenMin.x || transform.position.x > screenMax.x || transform.position.y < screenMin.y || (transform.position.y > screenMax.y && rb.velocity.y > 0))
			Destroy(gameObject);
	}
}
