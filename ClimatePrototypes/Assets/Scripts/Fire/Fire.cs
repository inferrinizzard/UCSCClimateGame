using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Fire : MonoBehaviour {
	[SerializeField] int damageRate = 30;
	[SerializeField] float fadeRate = 1f;
	public float health = 100f;
	Vector3 start, end = Vector3.one * .05f;
	int step = 0;

	void Start() => start = transform.localScale;

	void Update() {
		if (step++ % damageRate == 0 && Time.timeScale != 0)
			FireController.damage += fadeRate;
		if (health <= 0)
			Destroy(gameObject);
	}

	public void Fade() {
		health -= fadeRate;
		transform.localScale = EaseMethods.EaseVector3(EaseMethods.QuadEaseIn, start, end, 100 - health, 100);
	}
}
