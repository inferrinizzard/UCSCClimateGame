using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class SmokeVignette : MonoBehaviour {
	public GameObject smokeStage1;
	public GameObject smokeStage2;
	public GameObject smokeStage3;
	// Start is called before the first frame update
	void Start() {

	}

	// Update is called once per frame
	void Update() {
		if (WaterSpraying.damage > 100f) {
			smokeStage1.SetActive(true);
		} else if (WaterSpraying.damage > 300) {
			smokeStage2.SetActive(true);
		} else if (WaterSpraying.damage > 500) {
			smokeStage3.SetActive(true);;
		} else {
			smokeStage1.SetActive(false);
			smokeStage2.SetActive(false);
			smokeStage3.SetActive(false);
		}
	}
}
