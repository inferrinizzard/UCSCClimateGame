using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class TreeID : Tile { // TODO: rename to like TreeTile or something
	[SerializeField] Sprite[] trees = default;
	[SerializeField] Sprite burntTree = default;
	[HideInInspector] public int alt = 0;
	public bool burnt = false;

	void Start() {
		alt = Random.Range(0, 2);
		UpdateTile();
	}

	protected override void UpdateTile() {
		if (idManager.id == IdentityManager.Identity.Tree)
			sr.sprite = burnt ? burntTree : trees[alt];
	}
}
