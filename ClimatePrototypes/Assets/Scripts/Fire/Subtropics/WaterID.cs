using UnityEngine;

public class WaterID : Tile {
	protected override void UpdateTile() {
		if (idManager.id == IdentityManager.Identity.Water)
			sr.color = Color.clear;
	}
}
