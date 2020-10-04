using UnityEngine;

public class WaterID : Tile { // TODO: rename to like WaterTile or remove, this class is not super useful
	protected override void UpdateTile() {
		if (idManager.id == IdentityManager.Identity.Water)
			sr.color = Color.clear;
	}
}
