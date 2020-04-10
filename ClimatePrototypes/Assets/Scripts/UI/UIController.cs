using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class UIController : Singleton<UIController> {
	public Text worldNameText;
	[SerializeField] Text moneyText = default;
	[SerializeField] Text turnText = default;
	[SerializeField] Button backButton = default;
	[SerializeField] Button exitButton = default;
	[SerializeField] GameObject returnPrompt = default;

	void OnEnable() {
		worldNameText.text = World.worldName;
		turnText.text = $"Year {World.turn}";
	}

	void Update() {
		moneyText.text = $"{World.money:F2}";
	}

	public void IncrementTurn() => turnText.text = $"Year {++World.turn}";

	public void ToggleBackButton(bool on) {
		backButton.gameObject.SetActive(on);
		exitButton.gameObject.SetActive(!on);
	}

	public void UIQuitGame(int status) => GameManager.Instance.QuitGame(status);

	public void UITransition(string level) {
		returnPrompt.SetActive(false);
		GameManager.Transition(level);
	}

	public void SetPrompt(bool status) => returnPrompt.SetActive(status);
}
