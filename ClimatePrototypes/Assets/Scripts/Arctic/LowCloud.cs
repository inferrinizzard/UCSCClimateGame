using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LowCloud : MonoBehaviour {
	public float brightness = .5f;

	void Start() { }

	void OnTriggerEnter2D(Collider2D other) {
		if (Random.value > brightness) {
			GetComponent<Collider2D>().enabled = false;
		}
	}
	void OnTriggerExit2D(Collider2D other) => GetComponent<Collider2D>().enabled = true;
}
