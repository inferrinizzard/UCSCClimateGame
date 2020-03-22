using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Random = UnityEngine.Random;

public class HighCloudSpawner : MonoBehaviour {
	[SerializeField] GameObject highCloudPrefab = default;
	[SerializeField] bool canSpawn = true;
	private float highCloudSpawnWaitSeconds = 8f;
	Transform highCloudParent;
	private void Start() {
		highCloudParent = new GameObject().transform;
		highCloudParent.name = "High Cloud";
	}

	private void OnEnable() {
		canSpawn = true;
	}

	void Update() {
		if (canSpawn)
			SpawnHighCloud();
	}

	private void SpawnHighCloud() {
		Instantiate(highCloudPrefab, transform.position + transform.up * Random.Range(-0.5f, 1), Quaternion.identity, highCloudParent);
		StartCoroutine(SpawnLowCloudWait());
	}

	IEnumerator SpawnLowCloudWait() {
		canSpawn = false;
		yield return new WaitForSeconds(Random.Range(5f, highCloudSpawnWaitSeconds));
		canSpawn = true;
	}
}
