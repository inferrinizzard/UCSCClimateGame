using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour {
	public void Unload() {
		SceneManager.UnloadSceneAsync(GameManager.Instance.titleScene.Value);
		UIController.Instance.gameObject.SetActive(true);
	}

	public void Quit() => Application.Quit();

	public void ChangeName(string name) => World.worldName = name;
}
