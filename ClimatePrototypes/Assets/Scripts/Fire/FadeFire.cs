using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeFire : MonoBehaviour {

	public float fadeRate = 1f;
	public float health = 100f;
	Vector3 start, end;

	int step = 0;
	// Start is called before the first frame update
	void Start() {
		health = 100f;
		start = transform.localScale;
		end = Vector3.one * .05f;
	}

	void Update() {
		if (step++ % 20 == 0)
			WaterSpraying.damage += 1;
		if (health <= 0)
			Destroy(gameObject);
	}

	public void Fade() {
		health -= fadeRate;
		transform.localScale = Vector3.Slerp(start, end, (100f - health) / 100f);
	}
}
