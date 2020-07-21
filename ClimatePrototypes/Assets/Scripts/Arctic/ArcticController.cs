using System.Collections;
using System.Collections.Generic;
using Math = System.Math;

using UnityEngine;
using UnityEngine.UI;

public class ArcticController : RegionController {
	/// <summary> Total level time </summary>
	/// <summary> references to scene text assets </summary>
	[SerializeField] Text scoreText = default, timerText = default;
	/// <summary> Balls that landed </summary>
	/// <summary> present Buffers </summary>
	Buffer[] buffers;
	/// <summary> Buffer parent </summary>
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

	protected override void Update() {
		base.Update();
		// check timer
		if ((timer -= Time.deltaTime) < 0f) {
			timer = 0f;
			Pause();
			CalculateScore();
		} else { // update text
			damage = 0;
			foreach (Buffer b in buffers)
				damage += b.health + 1;
			scoreText.text = $"Ice Remaining: {damage}";
		}

		timerText.text = Mathf.Round(timer).ToString();
	}

	void CalculateScore() {
		// TriggerUpdate(() => World.albedo.Update(World.Region.Arctic, World.Region.City, ProcessScore()));
	}

	double ProcessScore() => (Math.Log(Math.E * (5 * buffers.Length - damage) / 30d) / 3 + .75) / 1000d; // returns scale of 0-1ish
}
