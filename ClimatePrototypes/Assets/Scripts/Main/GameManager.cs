using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager> {

	GameObject loadingScreen;
	Slider loadingBar;

	void Start() {
		loadingScreen = transform.GetChild(0).GetChild(0).gameObject;
		loadingBar = loadingScreen.GetComponentInChildren<Slider>();

		// World.Init();
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
		while (!asyncLoad.isDone) {

			instance.loadingScreen.SetActive(true);
			instance.loadingBar.normalizedValue = asyncLoad.progress / .9f;

			if (asyncLoad.progress >= .9f) {
				asyncLoad.allowSceneActivation = true;
				instance.loadingScreen.SetActive(false);
			}
			yield return null;
		}
	}
}
