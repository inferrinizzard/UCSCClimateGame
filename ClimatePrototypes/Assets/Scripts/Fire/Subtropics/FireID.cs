using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class FireID : Tile {
	[SerializeField] Sprite fireGreenSprite = default;
	[SerializeField] Sprite[] treeFires = default;

	void Start() {
		StartCoroutine(WaitForFire(4f));
	}

	protected override void UpdateTile() {
		if (idManager.id == IdentityManager.Identity.Fire) {
			if (idManager.fireVariance == 0)
				sr.sprite = fireGreenSprite;
			else
				sr.sprite = treeFires[GetComponent<TreeID>().alt];
		}
	}

	void FireGrowth() {
		// if I am fire, I ignite my non-fire neighbors coroutine
		GameObject[] myNeighbors = SubtropicsController.Instance.world.GetNeighbors(gameObject);

		foreach (var neighbor in myNeighbors) {
			if (neighbor != null) {
				IdentityManager.Identity neighborID = neighbor.GetComponent<IdentityManager>().id;
				IdentityManager.Moisture neighborMoisture = neighbor.GetComponent<IdentityManager>().moisture;
				if (neighborID == IdentityManager.Identity.Green && neighborMoisture != IdentityManager.Moisture.Moist) // if it is not already fire, or is water
					SubtropicsController.Instance.world.MutateCell(neighbor, IdentityManager.Identity.Fire);
			}
		}
	}

	IEnumerator WaitForFire(float seconds) {
		yield return new WaitForSeconds(seconds);
		FireGrowth();
		UpdateTile();
		StartCoroutine(WaitForFire(seconds));
	}
}
