using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class UIController : Singleton<UIController> {
	[SerializeField] GameObject settingsGroup = default;
	[SerializeField] InfoController infoGroup = default;
	[SerializeField] GameObject notificationGroup = default;
	[SerializeField] Text moneyText = default;
	[SerializeField] Text turnText = default;
	[SerializeField] Button backButton = default;
	[SerializeField] GameObject returnPrompt = default;
	bool settingsOn = false,
	infoOn = false,
	notificationsOn = false;

	void Start() {
		turnText.text = $"Year {World.turn}";
	}

	void Update() {
		moneyText.text = $"{World.money:F2}";
	}

	public void IncrementTurn() => turnText.text = $"Year {++World.turn}";

	public void ToggleSettings() {
		settingsOn = !settingsOn;

		if (settingsGroup)
			settingsGroup.SetActive(settingsOn);
	}

	public void ToggleInfo() {
		infoOn = !infoOn;

		if (infoGroup) {
			// infoGroup.gameObject.SetActive(infoOn);
			infoGroup.bRenderOnNextFrame = true;
		}
	}

	public void ToggleBackButton(bool on) => backButton.gameObject.SetActive(on);

	public void ToggleNotifications() {
		notificationsOn = !notificationsOn;

		if (notificationGroup)
			notificationGroup.SetActive(notificationsOn);
	}

	public void UIQuitGame() => GameManager.QuitGame();

	public void UITransition(string level) {
		returnPrompt.SetActive(false);
		GameManager.Transition(level);
	}

	public void ActivatePrompt() => returnPrompt.SetActive(true);
}
