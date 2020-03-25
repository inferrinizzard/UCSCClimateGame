using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class SmokeVignette : MonoBehaviour {
	[SerializeField] Sprite[] smoke = new Sprite[3];
	[SerializeField] float[] breakpoints = new float[3] { 500f, 300f, 100f };
	SpriteRenderer sr;
	void Start() => sr = GetComponent<SpriteRenderer>();

	void Update() {
		for (int i = 0; i < breakpoints.Length; i++)
			if (FireController.damage > breakpoints[i]) {
				sr.enabled = true;
				sr.sprite = smoke[i];
				break;
			}
		if (FireController.damage < breakpoints[breakpoints.Length - 1])
			sr.enabled = false;
	}
}
