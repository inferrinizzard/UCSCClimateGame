using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour {
	public GameObject SettingsReference;
	public InfoController InfoReference;
	public GameObject NotificationReference;
	public Text MoneyText;
	public Text TurnText;
	public Text ActionText;

	private bool SettingsOn = false;
	private bool InfoOn = false;
	private bool NotificationsOn = false;
	private int BaseActions = 2;

	// Start is called before the first frame update
	void Start() {
		TurnText.text = "Turn " + World.turn;
		World.Init();
	}

	// Update is called once per frame
	void Update() {
		MoneyText.text = "Money: $" + string.Format("{0:0,0}", World.money);

		if (ActionText)
			ActionText.text = "Actions Remaining: " + World.actionsRemaining;
	}

	public void IncrementTurn() {
		World.turn++;
		TurnText.text = "Turn " + World.turn;
		World.actionsRemaining = BaseActions;
	}

	public void ToggleSettings() {
		SettingsOn = !SettingsOn;

		if (SettingsReference)
			SettingsReference.SetActive(SettingsOn);
	}

	public void ToggleInfo() {
		InfoOn = !InfoOn;

		if (InfoReference) {
			// InfoReference.gameObject.SetActive(InfoOn);
			InfoReference.bRenderOnNextFrame = true;
		}
	}

	public void ToggleNotifications() {
		NotificationsOn = !NotificationsOn;

		if (NotificationReference)
			NotificationReference.SetActive(NotificationsOn);
	}

	public void UpdateTemperature() {

		// float[] deltaT = new float[World.regions.Length];
		// World.regions.ToList().ForEach(r => World.UpdateTemp(r, deltaT[Array.IndexOf(World.regions, r)]));

		// World.temperature = World.temps.Values.Aggregate((sum, r) => sum += r) / World.regions.Length;
		// World.temperature = World.regions.Select(r => World.temps[r]).Aggregate((sum, r) => sum += r) / World.regions.Length;
		// World.temperature = float.Parse(s);
	}

	public void UpdateCO2() {
		float change = 0; //sum up contributions from regions
		World.UpdateCO2(change);
	}

	public void ChangeLevel(string name) {
		SceneManager.LoadScene(name);
	}

	public void QuitGame() {
		Application.Quit();
	}

	public void ReturnToMenu() {
		SceneManager.LoadScene(0);
	}
}
