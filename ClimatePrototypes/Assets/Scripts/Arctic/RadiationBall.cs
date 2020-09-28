using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Radiation = SolarRadiationSpawner.Radiation;

public class RadiationBall : MonoBehaviour {
	[SerializeField] Vector2 force = new Vector2(2, 5);
	Rigidbody2D rb;
	Vector2 screenMin, screenMax;
	public Radiation radiationType = Radiation.ShortWave;

	void Start() {
		screenMin = Camera.main.ViewportToWorldPoint(Vector2.zero);
		screenMax = Camera.main.ViewportToWorldPoint(Vector2.one);
		rb = GetComponent<Rigidbody2D>();

		if (radiationType == Radiation.ShortWave)
			rb.velocity = new Vector2(Random.Range(-force.x, force.x), -Random.Range(force.y * 0.8f, force.y));
		else {
			force = new Vector2(Random.Range(-force.x, force.x), Random.Range(-force.y, force.y));
			rb.velocity = force.normalized * 5f;
		}
	}

	void Update() {
		if (transform.position.x < screenMin.x || transform.position.x > screenMax.x || transform.position.y < screenMin.y || (transform.position.y > screenMax.y && rb.velocity.y > 0))
			Destroy(gameObject);
	}
}
