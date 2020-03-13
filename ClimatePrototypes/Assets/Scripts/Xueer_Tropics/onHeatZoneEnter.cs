using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class onHeatZoneEnter : MonoBehaviour {

	public TropicsCloudMovement tropicsCloud;
	[SerializeField]
	//private SpriteRenderer cloudSprite;
	public Sprite waterVapourSprite;

	void OnTriggerEnter2D(Collider2D col) {
		// change heat to water vapour 
		Debug.Log("collided! ");
		SpriteRenderer[] sprites;
		sprites = GetComponentsInChildren<SpriteRenderer>();
		Debug.Log("sprites name: " + sprites[0].GetType());

		if (sprites != null) {
			for (int k = 0; k < sprites.Length; k++) {
				sprites[k].sprite = waterVapourSprite;
			}
		}

		// update cloud from origin to mature
		StartCoroutine(WaitForCloudMature());
		tropicsCloud.matureCloud();

	}
	IEnumerator WaitForCloudMature() {
		yield return new WaitForSeconds(3);
	}

}
