using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScreen : MonoBehaviour {
	Camera cam;
	Scene overworldScene;
	OverworldController overworldController;
	[SerializeField] Graphic[] uiReveal = default;

	public void Quit() => Application.Quit();

	public void ChangeName(string name) => World.worldName = name;

	void Start() {
		if (!SceneManager.GetSceneByName("Overworld").isLoaded) {
			SceneManager.LoadScene("Overworld", LoadSceneMode.Additive);
			SceneManager.sceneLoaded += SetOverWorldActive;
		} else SetOverWorldActive(SceneManager.GetSceneByName("Overworld"), LoadSceneMode.Single);

		AudioManager.Instance.Play("BGM_Menu"); // TODO: global sound name class

		for (int i = 0; i < uiReveal.Length; i++) {
			foreach (Graphic g in uiReveal[i].GetComponentsInChildren<Graphic>())
				g.color = new Color(g.color.r, g.color.g, g.color.b, 0);
			StartCoroutine(DropReveal(uiReveal[i].transform, i * .5f, uiReveal[i].TryGetComponent(out Button _)));
		}
	}

	IEnumerator DropReveal(Transform g, float delay = 0, bool drop = true, bool fade = true, float time = .5f) {
		yield return new WaitForSeconds(delay);
		float height = 0, startingHeight = 0;

		if (drop) {
			height = (g.transform as RectTransform).rect.height;
			g.transform.position = g.transform.position + Vector3.up * height;
			startingHeight = g.transform.position.y;
		}

		for (var(start, step) = (Time.time, 0f); step < time; step = Time.time - start) {
			yield return null;
			if (drop)
				g.transform.position = new Vector3(g.transform.position.x, startingHeight - step / time * height, g.transform.position.z);
			if (fade)
				foreach (Graphic child in g.GetComponentsInChildren<Graphic>())
					child.color = new Color(child.color.r, child.color.g, child.color.b, step / time);
		}
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

		// StartCoroutine(SlideUp());
	}

	public void ExitTitle() {
		StartCoroutine(SlideUp());
	}

	IEnumerator PanUp(float time = 1) {
		float startHeight = cam.transform.position.y;
		for (var (start, step) = (Time.time, 0f); step < time; step = Time.time - start) {
			yield return null;
			cam.transform.position = new Vector3(0, Mathf.Lerp(startHeight, 0, step / time), -10);
		}
	}

	IEnumerator SlideUp() {
		yield return StartCoroutine(PanUp());
		UIController.Instance.gameObject.SetActive(true);
		yield return StartCoroutine(UIController.SlideNav(UIController.Instance.navbar.transform));
		yield return StartCoroutine(overworldController.EnterWorld());
		SceneManager.UnloadSceneAsync(gameObject.scene);
	}
}
