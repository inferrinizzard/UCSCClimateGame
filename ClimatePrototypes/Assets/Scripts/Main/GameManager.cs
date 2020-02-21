using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager> {

	public bool runModel = true;
	GameObject loadingScreen;
	Slider loadingBar;
	public List < (string, string) > lineToDraw = new List < (string, string) > ();

	public override void Awake() {
		if (runModel)
			World.Init();
	}
	void Start() {
		loadingScreen = transform.GetChild(0).GetChild(0).gameObject; //do better
		// loadingBar = loadingScreen.GetComponentInChildren<Slider>();
	}
	public static void QuitGame() {
		// prompt
		Application.Quit();
	}

	public static void Transition(string scene) {

		instance.StartCoroutine(LoadScene(scene));
	}

	static IEnumerator LoadScene(string name) {
		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(name);
		asyncLoad.allowSceneActivation = false;
		float start = Time.time;

		bool calcDone = false;
		if (name == "Overworld") {
			Thread calcThread = new Thread(() => { World.Calc(); calcDone = true; });
			calcThread.Priority = System.Threading.ThreadPriority.AboveNormal;
			calcThread.Start();
		}

		instance.loadingScreen.SetActive(true);

		while (!asyncLoad.isDone && !calcDone) {
			// instance.loadingBar.normalizedValue = asyncLoad.progress / .9f;

			if (asyncLoad.progress >= .9f && Time.time - start > 1) {
				if (name != "Overworld")
					calcDone = true;

				asyncLoad.allowSceneActivation = true;
				instance.loadingScreen.SetActive(false);
				World.turn++;
				yield break;
			}
			yield return null;
		}
		yield break;
	}
}
