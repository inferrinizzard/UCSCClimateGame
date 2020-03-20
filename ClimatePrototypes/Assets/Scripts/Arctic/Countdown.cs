using System;
using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;
using UnityEngine.UI;
public class Countdown : MonoBehaviour {
	public float timeLeft = 60f;
	public GameObject ice;
	private TextMeshProUGUI textBox;
	public TextMeshProUGUI scoreTextBox;
	public float score;

	private void Start() {
		textBox = GetComponent<TextMeshProUGUI>();
		score = 0f;
		scoreTextBox.text = "Score";
	}

	void Update() {
		if ((timeLeft -= Time.deltaTime) < 0f) {
			timeLeft = 0f;
			calculateScore();
		} else {
			scoreTextBox.text = "Score: ";
		}

		textBox.text = Mathf.Round(timeLeft).ToString();

	}

	void calculateScore() {
		if (score > 0) {
			return;
		}
		score = 0;
		foreach (var buffer in ice.GetComponentsInChildren<BufferBehavior>()) {
			score += buffer.health;
		}

		scoreTextBox.text = "Score: " + score.ToString();

	}

}
