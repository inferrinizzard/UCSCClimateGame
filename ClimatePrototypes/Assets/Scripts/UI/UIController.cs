using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class UIController : Singleton<UIController> {
	public GameObject settingsGroup;
	public InfoController infoGroup;
	public GameObject notificationGroup;
	public Text moneyText;
	public Text turnText;

	[SerializeField] Button backButton = default;

	bool settingsOn = false;
	bool infoOn = false;
	bool notificationsOn = false;

	void Start() {
		turnText.text = $"Turn {World.turn}";
	}

	void Update() {
		moneyText.text = $"Money: ${World.money:0,0}";
	}

	public void IncrementTurn() {
		World.turn++;
		turnText.text = $"Turn {World.turn}";
	}

	public void ToggleSettings() {
		settingsOn = !settingsOn;

		if(settingsGroup)
			settingsGroup.SetActive(settingsOn);
	}

	public void ToggleInfo() {
		infoOn = !infoOn;

		if(infoGroup) {
			// infoGroup.gameObject.SetActive(infoOn);
			infoGroup.bRenderOnNextFrame = true;
		}
	}

	public void ToggleBackButton(bool on) => backButton.gameObject.SetActive(on);

	public void ToggleNotifications() {
		notificationsOn = !notificationsOn;

		if(notificationGroup)
			notificationGroup.SetActive(notificationsOn);
	}

	public void UIQuitGame() => GameManager.QuitGame();

	public void UITransition(string level) => GameManager.Transition(level);

}
