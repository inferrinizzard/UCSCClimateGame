using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class UIController : Singleton<UIController> {
	public Text worldNameText;
	[SerializeField] Text moneyText = default;
	[SerializeField] Text turnText = default;
	[SerializeField] Button backButton = default;
	[SerializeField] Button exitButton = default;
	[SerializeField] GameObject returnPrompt = default;

	void OnEnable() {
		worldNameText.text = World.worldName;
		turnText.text = $"Year {World.turn}";
	}

	void Update() {
		moneyText.text = $"{World.money:F2}";
	}

	public void IncrementTurn() => turnText.text = $"Year {++World.turn}";

	public void ToggleBackButton(bool on) {
		backButton.gameObject.SetActive(on);
		exitButton.gameObject.SetActive(!on);
	}

	public void UIQuitGame(int status) => GameManager.Instance.QuitGame(status);

	public void UITransition(string level) {
		returnPrompt.SetActive(false);
		GameManager.Transition(level);
	}

	public void SetPrompt(bool status) => returnPrompt.SetActive(status);

	static IEnumerator WaitForRealSeconds(float seconds) {
		float startTime = Time.realtimeSinceStartup;
		while (Time.realtimeSinceStartup - startTime < seconds)
			yield return null;
	}

	public static IEnumerator Typewriter(Text print, string text, float delay = .05f) { //given text to print, text ref, and print speed, does typewriter effect
		if (print.text == "Title") {
			print.text = text;
			print.transform.position += print.preferredWidth * Vector3.right;
		}
		print.text = "";
		for (int i = 0; i < text.Length; i++) {
			print.text += text[i];
			yield return WaitForRealSeconds(delay);
		}
	}

	public static IEnumerator ClickToAdvance(Text text, string[] words, GameObject button = null) {
		yield return instance.StartCoroutine(Typewriter(text, words[0]));
		text.GetComponentOnlyInChildren<Text>()?.gameObject.SetActive(true);

		for (int i = 1; i < words.Length; i++) {
			yield return new WaitForMouseDown();
			yield return instance.StartCoroutine(Typewriter(text, words[i]));
			if (i == words.Length - 1) {
				text.GetComponentOnlyInChildren<Text>()?.gameObject.SetActive(false);
				if (button)
					button.SetActive(true);
			}
		}
	}
}
