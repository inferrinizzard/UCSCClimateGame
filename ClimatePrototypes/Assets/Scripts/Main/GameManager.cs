using System.Collections;
using System.Collections.Generic;
using System.Threading;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager> {
	public bool runModel = true;
	[SerializeField] GameObject loadingScreen = default;
	[SerializeField] GameObject exitPrompt = default;
	bool titleScreen = true;
	[HideInInspector] public Scene? titleScene = null;

	public override void Awake() {
		base.Awake();
		if (runModel)
			if (World.averageTemp == 0)
				World.Init();
		// SpeedTest.VectorAllocTest();
	}

	void Start() {
		SceneManager.activeSceneChanged += instance.InitScene;

		if (titleScreen && SceneManager.GetActiveScene().name == "Overworld" && titleScene == null) {
			UIController.Instance.gameObject.SetActive(false);
			SceneManager.LoadScene("TitleScreen", LoadSceneMode.Additive);
			titleScene = SceneManager.GetSceneByName("TitleScreen");
			titleScreen = false;
		}
	}

	public static void QuitGame(int exitStatus = 0) {
		switch (exitStatus) {
			case 0:
				instance.exitPrompt.SetActive(true);
				break;
			case 1:
				instance.exitPrompt.SetActive(false);
				break;
			case 2:
				Application.Quit();
				break;
			default:
				break;
		}
	}

	void InitScene(Scene to, Scene from) {
		instance.loadingScreen.SetActive(false);
		UIController.Instance.ToggleBackButton(to.name != "Overworld");
	}

	public static void Transition(string scene) => instance.StartCoroutine(LoadScene(scene));

	static IEnumerator LoadScene(string name) {
		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(name);
		asyncLoad.allowSceneActivation = false;
		float start = Time.realtimeSinceStartup;

		bool calcDone = true;
		if (name == "Overworld") {
			calcDone = false;
			Thread calcThread = new Thread(() => { World.Calc(); calcDone = true; });
			calcThread.Priority = System.Threading.ThreadPriority.AboveNormal;
			calcThread.Start();
		}

		Instance.loadingScreen.SetActive(true);

		while (!asyncLoad.isDone || !calcDone) {
			yield return null;
			// instance.loadingBar.normalizedValue = asyncLoad.progress / .9f;

			if (asyncLoad.progress >= .9f && Time.realtimeSinceStartup - start > 1 && calcDone) {
				yield break;
			}
		}
		Time.timeScale = 1;
		asyncLoad.allowSceneActivation = true;
		if (name == "Overworld")
			UIController.Instance.IncrementTurn();
		UIController.Instance.SetPrompt(false);
	}
}
