using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class SubtropicsController : RegionController {
	public static SubtropicsController Instance { get => instance as SubtropicsController; }

	[HideInInspector] public Wind wind;

	void Start() {
		base.Start();
		wind = GetComponent<Wind>();
	}

	protected override void Update() {
		base.Update();
	}
}
