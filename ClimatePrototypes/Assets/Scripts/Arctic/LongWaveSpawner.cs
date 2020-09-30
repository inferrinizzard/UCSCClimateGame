using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class LongWaveSpawner : MonoBehaviour {
	float ballEmitWaitSeconds = 5.0f;
	[SerializeField] GameObject longWavePrefab = default;

	void Start() => StartCoroutine(EmitBall(1f));

	IEnumerator EmitBall(float waitTime) {
		yield return new WaitForSeconds(waitTime);
		for (int i = 0; i < (ArcticController.Instance.summer ? 2 : 3); i++)
			Instantiate(longWavePrefab, transform.position, Quaternion.identity, ArcticController.Instance.longWaveParent);
		StartCoroutine(EmitBall(ballEmitWaitSeconds * (ArcticController.Instance.summer ? 1 : 2 / 3f)));
	}
}
