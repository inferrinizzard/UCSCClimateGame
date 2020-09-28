using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Cloud : MonoBehaviour {
	SpriteRenderer sr;
	Rigidbody2D rb;
	Vector2 screenMin, screenMax;
	[SerializeField] float speed = 5f;
	public CloudSpawner.CloudType type = CloudSpawner.CloudType.Low;

	[HideInInspector] public bool flipped = false;

	void Start() {
		screenMin = Camera.main.ViewportToWorldPoint(Vector2.zero);
		screenMax = Camera.main.ViewportToWorldPoint(Vector2.one);
		sr = GetComponent<SpriteRenderer>();
		rb = GetComponent<Rigidbody2D>();

		rb.velocity = new Vector2(flipped ? -speed : speed, 0);
	}

	public void Flip() => flipped = true;

	void Update() {
		if (flipped && transform.position.x < screenMin.x || !flipped && transform.position.x > screenMax.x)
			Destroy(gameObject);
	}
}
