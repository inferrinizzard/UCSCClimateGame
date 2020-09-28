using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Buffer : MonoBehaviour {
	[HideInInspector] public int health = 5;
	/// <summary> ice sprites in order </summary>
	[SerializeField] Sprite[] summerSprite = new Sprite[6];
	[SerializeField] Sprite[] winterSprite = new Sprite[6];
	SpriteRenderer sr;

	void Start() {
		health = summerSprite.Length - 1;
		sr = GetComponent<SpriteRenderer>();
		AssignSprite();
	}

	public void AssignSprite(int i = -1) => sr.sprite = ArcticController.Instance.dayNight.isDayTime ? summerSprite[i > 0 ? i : health] : winterSprite[i > 0 ? i : health];

	private void OnTriggerEnter2D(Collider2D collision) {
		if (collision.gameObject.CompareTag("SolarRadiation")) {
			if (health > 0)
				AssignSprite(--health);
			else
				Destroy(GetComponent<Collider2D>());
			Destroy(collision.gameObject);
		}
	}
}
