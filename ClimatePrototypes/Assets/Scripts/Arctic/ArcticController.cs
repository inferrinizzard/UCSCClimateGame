using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class ArcticController : RegionController {
	float timeLeft = 60f;
	[SerializeField] GameObject ice = default;
	Text timerText;
	[SerializeField] Text scoreText = default;
	public float score = 0f;

	private void Start() {
		timerText = GetComponent<Text>();
		scoreText.text = "Score";
	}

	void Update() {
		if ((timeLeft -= Time.deltaTime) < 0f) {
			timeLeft = 0f;
			Pause();
			CalculateScore();
		} else {
			scoreText.text = "Score: ";
		}

		timerText.text = Mathf.Round(timeLeft).ToString();
	}

	void CalculateScore() {
		score = 0;
		foreach (var buffer in ice.GetComponentsInChildren<Buffer>())
			score += buffer.health;

		scoreText.text = "Score: " + score.ToString();
	}
}
