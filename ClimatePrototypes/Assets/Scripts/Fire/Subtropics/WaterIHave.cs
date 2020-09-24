using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class WaterIHave : MonoBehaviour {
	private static Transform waterTransform;
	// Start is called before the first frame update
	void Start() {
		waterTransform = transform;
	}

	/// <summary>
	/// Called by StartFire.cs
	/// use a fraction of water to kill a fire
	/// </summary>
	public static void UseWater() {
		// every fire cost 1, total is 3 
		Vector3 waterChange = new Vector3(-1.0f, 0, 0);
		waterTransform.localScale += waterChange;
		// TODO update position 
	}

	/// <summary> Check if have enough water to put out one fire </summary>
	public static bool EnoughWater() {
		// water left
		float left = waterTransform.localScale.x;
		// water I need
		float need = 1.0f;

		return need > left ? false : true;
	}

	/// <summary> Replenish water </summary>
	public static void AddWater() {
		// water left
		float left = waterTransform.localScale.x;
		// water I get
		// float get = 1.0f;
		float full = 3.0f;

		if (left < full) {
			Vector3 waterChange = new Vector3(1.0f, 0, 0);
			waterTransform.localScale += waterChange;
		}
	}
}
