using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class WaterPrompt : MonoBehaviour {
	GameObject yesButton, noButton;
	Text text;
	Image sprite;

	void Start() {
		sprite = GetComponent<Image>();
		text = GetComponentInChildren<Text>();
		var buttons = this.GetComponentsOnlyInChildren<Button>();
		(yesButton, noButton) = (buttons[0].gameObject, buttons[1].gameObject);
		SetActive(false);
	}

	public void SetActive(bool status) {
		text.enabled = status;
		sprite.enabled = status;
		yesButton.SetActive(status);
		noButton.SetActive(status);
	}
}
