using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager> {

	GameObject loadingScreen;
	Slider loadingBar;

	void Start() {
		loadingScreen = transform.GetChild(0).GetChild(0).gameObject; //do better
		loadingBar = loadingScreen.GetComponentInChildren<Slider>();

		World.Init();

		// async?
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
			instance.loadingBar.normalizedValue = asyncLoad.progress / .9f;

			if (asyncLoad.progress >= .9f && Time.time - start > 1) {
				if (name != "Overworld")
					calcDone = true;

				asyncLoad.allowSceneActivation = true;
				instance.loadingScreen.SetActive(false);
				yield break;
			}
			yield return null;
		}
		yield break;
	}
}
