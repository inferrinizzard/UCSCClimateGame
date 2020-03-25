using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class WaterPrompt : MonoBehaviour {
	GameObject yesButton;
	Text text;
	Image sprite;

	void Start() {
		sprite = GetComponent<Image>();
		text = GetComponentInChildren<Text>();
		yesButton = this.GetComponentOnlyInChildren<Button>().gameObject;
		SetActive(false);
	}

	// Update is called once per frame
	void Update() {

	}

	public void SetActive(bool status) {
		text.enabled = status;
		sprite.enabled = status;
		yesButton.SetActive(status);
	}
}
