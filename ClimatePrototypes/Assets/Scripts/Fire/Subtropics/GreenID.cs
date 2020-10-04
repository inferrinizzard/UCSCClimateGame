using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class GreenID : Tile {
	[SerializeField] Sprite greenSprite = default;

	protected override void UpdateTile() {
		if (idManager.id == IdentityManager.Identity.Green)
			sr.color = Color.clear;
	}

	void OnDisable() {
		if (idManager.id != IdentityManager.Identity.Water)
			sr.color = Color.white;
	}
}
