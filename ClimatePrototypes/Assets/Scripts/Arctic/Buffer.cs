using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Buffer : MonoBehaviour {
	public int health = 4;
	/// <summary> ice sprites in order </summary>
	[SerializeField] Sprite[] healthSprite = new Sprite[5];
	SpriteRenderer sr;

	void Start() {
		sr = GetComponent<SpriteRenderer>();
	}

	public void AssignSprite() => sr.sprite = healthSprite[health];

	private void OnTriggerEnter2D(Collider2D collision) {
		if (collision.gameObject.CompareTag("SolarRadiation")) {
			if (health > 0)
				sr.sprite = healthSprite[--health];
			Destroy(collision.gameObject);
		}
	}
}
