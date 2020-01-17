using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
	public GameObject settingsReference;
	public InfoController infoReference;
	public GameObject notificationReference;
	public Text moneyText;
	public Text turnText;
	public Text actionText;
	public Text opinionText;

	bool settingsOn = false;
	bool infoOn = false;
	bool notificationsOn = false;
	int baseActions = 2;

	// Start is called before the first frame update
	void Start() {
		turnText.text = $"Turn {World.turn}";
		// World.Init();
	}

	// Update is called once per frame
	void Update() {
		moneyText.text = $"Money: ${World.money:0,0}";
		opinionText.text = $"Public Opinion: {World.publicOpinion:0,0}";

		if (actionText)
			actionText.text = $"Actions Remaining: {World.actionsRemaining}";
	}

	public void IncrementTurn() {
		World.turn++;
		turnText.text = $"Turn {World.turn}";
		World.actionsRemaining = baseActions;
	}

	public void ToggleSettings() {
		settingsOn = !settingsOn;

		if (settingsReference)
			settingsReference.SetActive(settingsOn);
	}

	public void ToggleInfo() {
		infoOn = !infoOn;

		if (infoReference) {
			// infoReference.gameObject.SetActive(infoOn);
			infoReference.bRenderOnNextFrame = true;
		}
	}

	public void ToggleNotifications() {
		notificationsOn = !notificationsOn;

		if (notificationReference)
			notificationReference.SetActive(notificationsOn);
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

	public void QuitGame() {
		// prompt
		Application.Quit();
	}

	public void ChangeLevel(string scene) => Transition(scene);

	public static void Transition(string scene) {
		SceneManager.LoadScene(scene);
	}

	static IEnumerator LoadScene(string name) {
		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(name);
		asyncLoad.allowSceneActivation = false;
		while (!asyncLoad.isDone) {
			// loading scene here
			if (asyncLoad.progress >.9f)
				asyncLoad.allowSceneActivation = true;
			yield return null;
		}
	}
}
