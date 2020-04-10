using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class UIController : Singleton<UIController> {
	[SerializeField] Text moneyText = default;
	[SerializeField] Text turnText = default;
	[SerializeField] Button backButton = default;
	[SerializeField] GameObject returnPrompt = default;

	void Start() {
		turnText.text = $"Year {World.turn}";
	}

	void Update() {
		moneyText.text = $"{World.money:F2}";
	}

	public void IncrementTurn() => turnText.text = $"Year {++World.turn}";

	public void ToggleBackButton(bool on) => backButton.gameObject.SetActive(on);

	public void UIQuitGame() => GameManager.QuitGame();

	public void UITransition(string level) {
		returnPrompt.SetActive(false);
		GameManager.Transition(level);
	}

	public void SetPrompt(bool status) => returnPrompt.SetActive(status);
}
