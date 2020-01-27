using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Indicators : MonoBehaviour {

	[SerializeField] Image temperatureImage = default;
	[SerializeField] Image opinionImage = default;

	[SerializeField] Sprite[] temperatureSprites = new Sprite[3];
	[SerializeField] Sprite[] opinionSprites = new Sprite[3];

	// Start is called before the first frame update
	void Start() {

	}

	// Update is called once per frame
	void Update() {
		if (World.publicOpinion > 1) {
			opinionImage.sprite = opinionSprites[2];
		} else if (World.publicOpinion < 0) {
			opinionImage.sprite = opinionSprites[1];
		} else {
			opinionImage.sprite = opinionSprites[0];
		}

		if (World.averageTemp > 50.0) {
			temperatureImage.sprite = temperatureSprites[2];
		} else if (World.publicOpinion < 0) {
			temperatureImage.sprite = temperatureSprites[1];
		} else {
			temperatureImage.sprite = temperatureSprites[0];
		}
	}
}
