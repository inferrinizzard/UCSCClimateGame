using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LowCloud : MonoBehaviour {
	public float brightness = .5f;
	
	Vector2 screenMin;
	private Vector2 screenMax;
	public float sideForce = 3f;
	Rigidbody2D rb;

	void Start()
	{
		screenMin = Camera.main.ViewportToWorldPoint(Vector2.zero);
		screenMax = Camera.main.ViewportToWorldPoint(Vector2.one);
		rb = GetComponent<Rigidbody2D>();

		Vector2 force = new Vector2(sideForce, 0);
		rb.velocity = force;
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (Random.value > brightness) {
			GetComponent<Collider2D>().enabled = false;
		}
	}
	void OnTriggerExit2D(Collider2D other) => GetComponent<Collider2D>().enabled = true;
	
	void Update() {

		if (transform.position.x < screenMin.x || transform.position.x > screenMax.x)
			Destroy(gameObject);
	}
}
