using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class LongWaveSpawner : MonoBehaviour {
	float ballEmitWaitSeconds = 5.0f;
	[SerializeField] GameObject longWavePrefab = default;

	Transform longWaveParent;
	void Start() {
		longWaveParent = new GameObject("Long Wave Ray").transform;
		StartCoroutine(EmitBall(1f));
	}

	IEnumerator EmitBall(float waitTime) {
		yield return new WaitForSeconds(waitTime);
		for (int i = 0; i < (ArcticController.Instance.dayNight.isDayTime ? 2 : 3); i++)
			Instantiate(longWavePrefab, transform.position, Quaternion.identity, longWaveParent);
		StartCoroutine(EmitBall(ballEmitWaitSeconds));
	}
}
