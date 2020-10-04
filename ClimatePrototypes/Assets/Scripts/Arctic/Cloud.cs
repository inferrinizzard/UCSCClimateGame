using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Cloud : MonoBehaviour { // TODO: rename to ArcticCloud
	SpriteRenderer sr;
	Vector2 screenMin, screenMax; // TODO: move to global variable class
	[SerializeField] float speed = 5f;
	public CloudSpawner.CloudType type = CloudSpawner.CloudType.Low;
	[HideInInspector] public bool flipped = false;

	void Start() {
		screenMin = Camera.main.ViewportToWorldPoint(Vector2.zero);
		screenMax = Camera.main.ViewportToWorldPoint(Vector2.one);
		sr = GetComponent<SpriteRenderer>();
		// TODO: add colour change based on temp influence here
		GetComponent<Rigidbody2D>().velocity = new Vector2(flipped ? -speed : speed, 0);
	}

	void Update() {
		if (flipped && transform.position.x < screenMin.x || !flipped && transform.position.x > screenMax.x)
			Destroy(gameObject);
	}
}
