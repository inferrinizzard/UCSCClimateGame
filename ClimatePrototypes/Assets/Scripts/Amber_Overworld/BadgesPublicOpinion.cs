using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BadgesPublicOpinion : MonoBehaviour {
	Image myImageComponent;
	public Sprite[] badge = new Sprite[3];

	// Start is called before the first frame update
	void Start() {
		myImageComponent = GetComponent<Image>();
	}

	// Update is called once per frame
	void Update() {
		if (World.publicOpinion > 50) {
			myImageComponent.sprite = badge[2];
		} else if (World.publicOpinion < 0) {
			myImageComponent.sprite = badge[1];
		} else {
			myImageComponent.sprite = badge[0];
		}
	}
}
