using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class WaterID : MonoBehaviour {
	private SpriteRenderer sr;
	// [SerializeField] Sprite waterSprite = default;

	void Start() {
		sr = GetComponent<SpriteRenderer>();
	}

	void Update() {
		VFXUpdate();
	}

	void VFXUpdate() { }
}
