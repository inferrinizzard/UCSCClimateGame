using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class LowCloudSpawner : MonoBehaviour {
	[SerializeField] GameObject lowCloudPrefab = default;
	[SerializeField] bool canSpawn = true;
	private float lowCloudSpawnWaitSeconds = 6f;
	Transform lowCloudParent;

	private void Start() {
		lowCloudParent = new GameObject().transform;
		lowCloudParent.name = "Low Cloud";
	}

	private void OnEnable() {
		canSpawn = true;
	}

	void Update() {
		if (canSpawn)
			SpawnLowCloud();
	}

	private void SpawnLowCloud() {
		Instantiate(lowCloudPrefab, transform.position + transform.up * Random.Range(0, 1.5f), Quaternion.identity, lowCloudParent);
		StartCoroutine(SpawnLowCloudWait());
	}

	IEnumerator SpawnLowCloudWait() {
		canSpawn = false;
		yield return new WaitForSeconds(Random.Range(lowCloudSpawnWaitSeconds - 2f, lowCloudSpawnWaitSeconds));
		canSpawn = true;
	}
}
