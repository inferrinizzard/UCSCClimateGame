using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class BufferBehavior : MonoBehaviour {

	public float health;
	public Sprite health4;
	public Sprite health3;
	public Sprite health2;
	public Sprite health1;
	public Sprite health0;

	private void Start() {
		health = 4f;
	}

	private void OnTriggerEnter2D(Collider2D collision) {
		Debug.Log("hit");
		if (collision.gameObject.tag == "SolarRadiation") {
			Debug.Log("hitSR");
			TakeDamage();
			UpdateGraphics();
			Destroy(collision.gameObject);
		}
	}

	private void TakeDamage() {
		if (health > 0)
			health--;
		Debug.Log(health);
	}

	private void UpdateGraphics() {
		switch (health) {
			case 4:
				GetComponent<SpriteRenderer>().sprite = health4;
				break;
			case 3:
				GetComponent<SpriteRenderer>().sprite = health3;
				break;
			case 2:
				GetComponent<SpriteRenderer>().sprite = health2;
				break;
			case 1:
				GetComponent<SpriteRenderer>().sprite = health1;
				break;
			case 0:
				GetComponent<SpriteRenderer>().sprite = health0;
				break;
		}
	}
}
