using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Fire : MonoBehaviour {
	[SerializeField] float fadeRate = 1f;
	public float health = 100f;
	Vector3 start, end = Vector3.one * .05f;
	int step = 0;

	void Start() => start = transform.localScale;

	void FixedUpdate() {
		if (step++ % FireController.damageRate == 0 && Time.timeScale != 0)
			FireController.damage += fadeRate;
		if (health <= 0) {
			Destroy(gameObject);
			FireController.fireCount--;
		}
	}

	public void Fade() {
		health -= fadeRate;
		transform.localScale = EaseMethods.EaseVector3(EaseMethods.QuadEaseIn, start, end, 100 - health, 100);
	}
}
