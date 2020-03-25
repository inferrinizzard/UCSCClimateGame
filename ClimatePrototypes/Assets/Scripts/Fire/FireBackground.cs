using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class FireBackground : MonoBehaviour {
	SpriteRenderer baseSR, burntSR;
	void Start() {
		var sr = GetComponentsInChildren<SpriteRenderer>();
		(baseSR, burntSR) = (sr[0], sr[1]);
	}

	void Update() {
		if (FireController.damage < FireController.damageLimit * .75)
			baseSR.color = new Color(1, 1, 1, 1 - EaseMethods.QuadEaseIn(FireController.damage, 0, 1, FireController.damageLimit * .75f));
	}
}
