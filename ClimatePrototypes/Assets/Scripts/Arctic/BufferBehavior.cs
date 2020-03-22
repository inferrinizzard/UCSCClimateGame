using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class BufferBehavior : MonoBehaviour {
	public int health = 4;
	[SerializeField] Sprite[] healthSprite = new Sprite[5];
	SpriteRenderer sr;

	void Start() {
		sr = GetComponent<SpriteRenderer>();
	}

	private void OnTriggerEnter2D(Collider2D collision) {
		if (collision.gameObject.tag == "SolarRadiation") {
			TakeDamage();
			sr.sprite = healthSprite[health];
			Destroy(collision.gameObject);
		}
	}

	private void TakeDamage() {
		if (health > 0)
			health--;
	}
}
