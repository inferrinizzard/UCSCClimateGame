using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

public class SubtropicsController : RegionController {
	public static SubtropicsController Instance { get => instance as SubtropicsController; }

	[HideInInspector] public Wind wind;
	public int difficulty = 3;
	public SubtropicsWorld world;

	protected override void Start() {
		base.Start();
		wind = GetComponent<Wind>();
		world = GetComponent<SubtropicsWorld>();
	}

	protected override void Update() {
		base.Update();
	}

	protected override void GameOver() {
		Debug.Log(GetFirePercentage());
	}

	public float GetFirePercentage() {
		var(fire, trees) = SubtropicsController.Instance.world.cellArray.Cast<GameObject>().Select(obj =>
			(obj.GetComponent<IdentityManager>().id == IdentityManager.Identity.Fire ? 1 : 0, obj.GetComponent<IdentityManager>().id == IdentityManager.Identity.Tree ? 1 : 0)
		).Aggregate((tup, obj) => (tup.Item1 + obj.Item1, tup.Item2 + obj.Item2));

		return fire / (float) trees;
	}
}
