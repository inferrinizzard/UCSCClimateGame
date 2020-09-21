using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class WaterID : MonoBehaviour {
	private Color color = Color.clear; // color to debug cell id
	private SpriteRenderer sr;

	void Start() {
		sr = GetComponent<SpriteRenderer>();
	}

	void Update() {
		VFXUpdate();
	}

	void VFXUpdate() {
		sr.color = color;
		//sr.sprite = waterSprite;
	}
}
