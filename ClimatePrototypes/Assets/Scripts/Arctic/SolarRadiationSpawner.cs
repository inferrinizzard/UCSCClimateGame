using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class SolarRadiationSpawner : MonoBehaviour {
	[SerializeField] float ballEmitWaitSeconds = 2f;
	[SerializeField] GameObject ballPrefab = default;
	Transform radiationParent;

	public enum Radiation { LongWave, ShortWave }

	void Start() {
		StartCoroutine(EmitBall(3));
		radiationParent = new GameObject("Solar Radiation").transform;
	}

	IEnumerator EmitBall(float delay = 0) {
		yield return new WaitForSeconds(delay);
		yield return new WaitForSeconds(ballEmitWaitSeconds);
		Instantiate(ballPrefab, transform.position + Vector3.right * Random.Range(5, -5), Quaternion.identity, radiationParent);
		StartCoroutine(EmitBall());
	}
}
