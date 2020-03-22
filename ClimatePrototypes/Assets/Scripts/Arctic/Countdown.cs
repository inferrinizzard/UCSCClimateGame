using System;
using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class Countdown : MonoBehaviour {
	public float timeLeft = 60f;
	[SerializeField] GameObject ice = default;
	private TextMeshProUGUI textBox;
	public TextMeshProUGUI scoreTextBox;
	public float score = 0f;

	private void Start() {
		textBox = GetComponent<TextMeshProUGUI>();
		scoreTextBox.text = "Score";
	}

	void Update() {
		if ((timeLeft -= Time.deltaTime) < 0f) {
			timeLeft = 0f;
			CalculateScore();
		} else {
			scoreTextBox.text = "Score: ";
		}

		textBox.text = Mathf.Round(timeLeft).ToString();
	}

	void CalculateScore() {
		score = 0;
		foreach (var buffer in ice.GetComponentsInChildren<BufferBehavior>()) {
			score += buffer.health;
		}

		scoreTextBox.text = "Score: " + score.ToString();

		// TODO:freeze and prompt here
	}
}
