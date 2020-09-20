using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class TreeID : MonoBehaviour {
	[Header("References")]
	[SerializeField] Sprite[] trees = default;
	[SerializeField] Sprite burntTree = default;
	[HideInInspector] public int alt = 0;
	public bool burnt = false;

	private SpriteRenderer sr;

	// Start is called before the first frame update
	void Start() {
		sr = GetComponent<SpriteRenderer>();
		alt = Random.Range(0, 2);
	}

	// Update is called once per frame
	void Update() {
		VFXUpdate();
	}

	void VFXUpdate() {
		sr.sprite = burnt ? burntTree : trees[alt];
	}
}
