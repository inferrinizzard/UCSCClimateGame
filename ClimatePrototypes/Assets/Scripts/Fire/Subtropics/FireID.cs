using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class FireID : MonoBehaviour {
	[SerializeField] Sprite fireGreenSprite = default;
	[SerializeField] Sprite[] treeFires = default;
	private SpriteRenderer sr;

	private bool growing;

	void Start() {
		sr = GetComponent<SpriteRenderer>();
	}

	private void OnEnable() {
		//VFXUpdate();
	}

	// Update is called once per frame
	void Update() {
		if (!growing) {
			growing = true;
			StartCoroutine(WaitForFire(4f));
		}

		//FireGrowth();
		VFXUpdate();
	}

	void VFXUpdate() {
		if (GetComponent<IdentityManager>().fireVariance == 0) {
			sr.sprite = fireGreenSprite;
		} else {
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
		growing = false;
	}
}
