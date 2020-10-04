using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Math = System.Math;

using UnityEngine;
using UnityEngine.UI;

public class ArcticController : RegionController {
	public static ArcticController Instance { get => instance as ArcticController; }

	public bool summer { get => cycle.isSummer; }

	[SerializeField] SeasonCycle cycle = default;
	/// <summary> present Buffers </summary>
	[HideInInspector] public Buffer[] buffers;
	[SerializeField] Transform ice = default;
	[HideInInspector] public Transform longWaveParent;
	[HideInInspector] public float tempInfluence;

	void Start() {
		longWaveParent = new GameObject("Long Wave Ray").transform;
		tempInfluence = (float) (World.temp[2] - World.startingTemp[2]) / World.maxTempChange;
		Debug.Log($"Arctic temp influence is: {tempInfluence}");

		buffers = ice.GetComponentsInChildren<Buffer>();
		int totalHealth = buffers.Length * buffers[0].health;
		for (int i = 0; i < Math.Floor(tempInfluence * totalHealth);) { //TODO: with temp instead
			var buff = buffers[Random.Range(0, buffers.Length)];
			if (buff.health > 0) {
				buff.health--;
				i++;
				buff.AssignSprite();
			}
		}
	}

	protected override void GameOver() {
		base.GameOver();
		Debug.Log($"Remaining {buffers.Select(b => b.health).Aggregate((sum, b) => b + sum)} ice of total {buffers.Length * 5} ice");
		// TriggerUpdate(() => World.albedo.Update(World.Region.Arctic, World.Region.City, ProcessScore()));
	}

	double ProcessScore() => (Math.Log(Math.E * (5 * buffers.Length - damage) / 30d) / 3 + .75) / 1000d; // returns scale of 0-1ish
}
