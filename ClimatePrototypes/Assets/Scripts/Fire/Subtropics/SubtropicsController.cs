using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;
using Math = System.Math;

using GlobalWorld = World;

public class SubtropicsController : RegionController {
	public static SubtropicsController Instance { get => instance as SubtropicsController; }

	public PlayerInteractions player;
	[HideInInspector] public Wind wind;
	[HideInInspector] public int difficulty = 3;
	[HideInInspector] public SubtropicsWorld world;
	public static SubtropicsWorld World { get => Instance.world; }

	protected override void Start() {
		base.Start();
		wind = GetComponentInChildren<Wind>();
		world = GetComponentInChildren<SubtropicsWorld>();
	}

	protected override void Update() {
		base.Update();
	}

	protected override void GameOver() {
		base.GameOver();
		// Debug.Log(GetFirePercentage());
		double effect = GetFirePercentage();
		TriggerUpdate(() => GlobalWorld.co2.Update(region, delta: -effect)); // [-1, 0]
		// TriggerUpdate(() => GlobalWorld.co2.Update(region, delta: -Math.Min(1, Math.Log(effect)))); // [-1, 0]
	}

	public float GetFirePercentage() {
		var(fire, trees) = world.cellArray.Cast<GameObject>().Select(obj =>
			(obj.GetComponent<IdentityManager>().id == IdentityManager.Identity.Fire ? 1 : 0, obj.GetComponent<IdentityManager>().id == IdentityManager.Identity.Tree ? 1 : 0)
		).Aggregate((tup, obj) => (tup.Item1 + obj.Item1, tup.Item2 + obj.Item2));

		return fire / (float) trees;
	}
}
