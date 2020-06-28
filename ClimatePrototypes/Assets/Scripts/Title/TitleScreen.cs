using System.Collections;
using System.Collections.Generic;

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

		cam = Camera.main;
		overworldController = cam.GetComponent<OverworldController>();
		overworldController.ClearWorld();
		overworldController.SendToBottom();

		StartCoroutine(SlideUp());
	}

	IEnumerator PanUp(float time = 1) {
		float startHeight = cam.transform.position.y;
		for (var(start, step) = (Time.time, 0f); step < time; step = Time.time - start) {
			yield return null;
			cam.transform.position = new Vector3(0, Mathf.Lerp(startHeight, 0, step / time), -10);
		}
	}

	IEnumerator SlideUp() {
		yield return StartCoroutine(PanUp());
		yield return StartCoroutine(overworldController.EnterWorld());
	}
}
