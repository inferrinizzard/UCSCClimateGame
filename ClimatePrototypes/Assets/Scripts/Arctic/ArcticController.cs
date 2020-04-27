using System.Collections;
using System.Collections.Generic;
using Math = System.Math;

using UnityEngine;
using UnityEngine.UI;

public class ArcticController : RegionController {
	float timeLeft = 60f;
	[SerializeField] Text scoreText = default, timerText = default;
	int damage = 0;
	Buffer[] buffers;
	[SerializeField] Transform ice = default;

	void Start() {
		buffers = ice.GetComponentsInChildren<Buffer>();
		// Intro();
		int totalHealth = buffers.Length * buffers[0].health;
		for (int i = 0; i < Math.Floor(EBM.F / EBM.maxF * totalHealth);) { //TODO: with temp instead
			var buff = buffers[Random.Range(0, buffers.Length)];
			if (buff.health > 0) {
				buff.health--;
				i++;
				buff.AssignSprite();
			}
		}
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
