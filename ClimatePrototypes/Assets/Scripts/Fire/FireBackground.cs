using UnityEngine;

public class FireBackground : MonoBehaviour {
	/// <summary> Background sprite references </summary>
	SpriteRenderer baseBG, burntBG;
	void Start() {
		var sr = GetComponentsInChildren<SpriteRenderer>();
		(baseBG, burntBG) = (sr[0], sr[1]);
	}

	void Update() {
		if (FireController.damage < FireController.damageLimit * .75)
			baseBG.color = new Color(1, 1, 1, (1 - EaseMethods.QuadEaseIn(FireController.damage, 0, 1, FireController.damageLimit * .75f)));
		else if (FireController.damage > FireController.damageLimit * .75)
			baseBG.color = Color.clear;
	}
}
