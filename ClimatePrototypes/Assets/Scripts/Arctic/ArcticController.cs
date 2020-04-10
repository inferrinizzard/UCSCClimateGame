using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class ArcticController : RegionController {
	float timeLeft = 60f;
	[SerializeField] Text scoreText = default, timerText = default;
	int damage = 0;
	Buffer[] buffers;

	void Start() {
		buffers = GetComponentsInChildren<Buffer>();
	}

	void Update() {
		if ((timeLeft -= Time.deltaTime) < 0f) {
			timeLeft = 0f;
			Pause();
			CalculateScore();
		} else {
			damage = 0;
			foreach (Buffer b in buffers)
				damage += b.health + 1;
			scoreText.text = $"Ice Remaining: {damage}";
		}

		timerText.text = Mathf.Round(timeLeft).ToString();
	}

	void CalculateScore() {
		// TriggerUpdate(() => World.albedo.Update(World.Region.Arctic, World.Region.City, ProcessScore()));
	}

	double ProcessScore() => (Math.Log(Math.E * (5 * buffers.Length - damage) / 30d) / 3 + .75) / 1000d;
}
