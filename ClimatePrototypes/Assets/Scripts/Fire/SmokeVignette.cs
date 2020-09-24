using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class SmokeVignette : MonoBehaviour {
	[SerializeField] Sprite[] smoke = new Sprite[3];
	[SerializeField] float[] breakpoints = new float[3] { .8f, .5f, .3f };
	SpriteRenderer sr;
	void Start() => sr = GetComponent<SpriteRenderer>();

	void Update() {
		for (int i = 0; i < breakpoints.Length; i++)
			if (FireController.Instance.damage > breakpoints[i] * FireController.Instance.damageLimit) {
				sr.enabled = true;
				sr.sprite = smoke[i]; // TODO: fade these
				// sr.color = new Color(1,1,1,EaseMethods.QuadEaseIn)
				break;
			}
		if (FireController.Instance.damage < breakpoints[breakpoints.Length - 1])
			sr.enabled = false;
	}
}
