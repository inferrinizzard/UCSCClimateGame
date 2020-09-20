using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class TreeID : MonoBehaviour {
	[Header("References")]
	public Sprite tree1Sprite;
	public Sprite tree2Sprite;
	public Sprite tree1BurntSprite;
	public Sprite tree2BurntSprite;

	public int alt = 0;

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
		//sr.color = color;
		sr.sprite = burnt ? (alt == 1) ? tree1BurntSprite : tree2BurntSprite : (alt == 1) ? tree1Sprite : tree2Sprite;
	}
}
