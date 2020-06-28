using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour {
	Camera cam;
	Scene overworldScene;
	OverworldController overworldController;

	public void Unload() {
		UIController.Instance.gameObject.SetActive(true);
		SceneManager.UnloadSceneAsync(gameObject.scene);
	}

	public void Quit() => Application.Quit();

	public void ChangeName(string name) => World.worldName = name;

	void Start() {
		if (!SceneManager.GetSceneByName("Overworld").isLoaded) {
			SceneManager.LoadScene("Overworld", LoadSceneMode.Additive);
			SceneManager.sceneLoaded += SetOverWorldActive;
		} else SetOverWorldActive(SceneManager.GetSceneByName("Overworld"), LoadSceneMode.Single);
	}

	void SetOverWorldActive(Scene scene, LoadSceneMode mode) {
		overworldScene = scene;
		SceneManager.SetActiveScene(overworldScene);
		SceneManager.sceneLoaded -= SetOverWorldActive;

		UIController.Instance.gameObject.SetActive(false);

		overworldController = overworldScene.GetRootGameObjects().ToList().Find(g => g.TryGetComponent(out OverworldController c)).GetComponent<OverworldController>();
		cam = overworldController.GetComponent<Camera>();
		overworldController.ClearWorld();

		StartCoroutine(SlideUp());
	}

	IEnumerator PanUp(float time = 1) {
		for (var(start, step) = (Time.time, 0f); step < time; step = Time.time - start) {
			yield return null;
		}
	}

	IEnumerator SlideUp() {
		yield return StartCoroutine(PanUp());
		yield return StartCoroutine(overworldController.EnterWorld());
	}
}
