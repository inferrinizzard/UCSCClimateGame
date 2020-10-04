using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class SolarRadiationSpawner : MonoBehaviour { // TODO: rename to be consistent with LongWaveSpawner
	public enum Radiation { LongWave, ShortWave }

	[SerializeField] float ballEmitWaitSeconds = 2f;
	[SerializeField] GameObject ballPrefab = default;
	Transform radiationParent;

	void Start() {
		radiationParent = new GameObject("Solar Radiation").transform;
		StartCoroutine(EmitBall(3));
	}

	IEnumerator EmitBall(float delay = 0) {
		yield return new WaitForSeconds(delay);
		yield return new WaitForSeconds(ballEmitWaitSeconds);
		if (ArcticController.Instance.summer)
			Instantiate(ballPrefab, transform.position + Vector3.right * Random.Range(5, -5), Quaternion.identity, radiationParent);
		StartCoroutine(EmitBall());
	}
}
