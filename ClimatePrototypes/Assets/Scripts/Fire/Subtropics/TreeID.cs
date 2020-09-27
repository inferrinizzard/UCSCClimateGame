using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class TreeID : Tile {
	[Header("References")]
	[SerializeField] Sprite[] trees = default;
	[SerializeField] Sprite burntTree = default;
	[HideInInspector] public int alt = 0;
	public bool burnt = false;

	void Start() {
		alt = Random.Range(0, 2);
	}

	protected override void UpdateTile() {
		if (idManager.id == IdentityManager.Identity.Tree)
			sr.sprite = burnt ? burntTree : trees[alt];
	}
}
