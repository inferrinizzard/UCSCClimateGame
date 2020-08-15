using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class StatsPanel : MonoBehaviour {
	[SerializeField] Slider landUse = default, publicOpinion = default, emissions = default, economy = default;

	void Start() { }

	void Update() {
		if (gameObject.activeSelf) { // only needs to update when turned on
			UpdateSlider(landUse, (float) EBM.a0);
			UpdateSlider(publicOpinion, 1 - World.publicOpinion / 100, invertColors : true);
			UpdateSlider(emissions, (float) EBM.F / 14);
			UpdateSlider(economy, World.money / 200);
		}
	}

	void UpdateSlider(Slider slider, float value, bool invertColors = false) {
		slider.value = value;
		slider.fillRect.GetComponentInChildren<Image>(true).color = invertColors ? Color.Lerp(Color.red, Color.green, value) : Color.Lerp(Color.green, Color.red, value);
	}

	public void Toggle() => gameObject.SetActive(!gameObject.activeSelf);
	public void Toggle(bool status) => gameObject.SetActive(status);
}
