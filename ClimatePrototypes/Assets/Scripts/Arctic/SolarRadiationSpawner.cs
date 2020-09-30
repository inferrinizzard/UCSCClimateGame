using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class SolarRadiationSpawner : MonoBehaviour {
	[SerializeField] float ballEmitWaitSeconds = 2f;
	[SerializeField] GameObject ballPrefab = default;
	Transform radiationParent;

	public enum Radiation { LongWave, ShortWave }

	void Start() {
		radiationParent = new GameObject("Solar Radiation").transform;
		StartCoroutine(EmitBall(3));
	}

	IEnumerator EmitBall(float delay = 0) {
		yield return new WaitForSeconds(delay);
		yield return new WaitForSeconds(ballEmitWaitSeconds);
		if (ArcticController.Instance.dayNight.isDayTime)
			Instantiate(ballPrefab, transform.position + Vector3.right * Random.Range(5, -5), Quaternion.identity, radiationParent);
		StartCoroutine(EmitBall());
	}
}
