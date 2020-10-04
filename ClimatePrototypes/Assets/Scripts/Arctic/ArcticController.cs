using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Math = System.Math;

using UnityEngine;
using UnityEngine.UI;

public class ArcticController : RegionController {
	public static ArcticController Instance { get => instance as ArcticController; } // static instance

	[SerializeField] SeasonCycle cycle = default;
	public bool summer { get => cycle.isSummer; } // used for state changes in region
	/// <summary> present Buffers </summary>
	[HideInInspector] public Buffer[] buffers;
	[SerializeField] Transform ice = default;
	[HideInInspector] public Transform longWaveParent;
	[HideInInspector] public float tempInfluence;

	void Start() {
		longWaveParent = new GameObject("Long Wave Ray").transform;
		// init temp influence, drives game difficulty
		tempInfluence = (float) (World.temp[2] - World.startingTemp[2]) / World.maxTempChange;
		Debug.Log($"Arctic temp influence is: {tempInfluence}");

		buffers = ice.GetComponentsInChildren<Buffer>();
		int totalHealth = buffers.Length * buffers[0].health;
		// dock health proportional to warming
		for (int i = 0; i < Math.Floor(tempInfluence * totalHealth);) {
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
		// arctic does not affect model
		// TriggerUpdate(() => World.albedo.Update(World.Region.Arctic, World.Region.City, effect));
	}
}
