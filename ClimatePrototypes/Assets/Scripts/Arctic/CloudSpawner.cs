using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Random = UnityEngine.Random;

public class CloudSpawner : MonoBehaviour {
	[SerializeField] CloudType type = CloudType.High;
	[SerializeField] GameObject cloudPrefab = default;
	[SerializeField] bool canSpawn = true;
	private float cloudSpawnWaitSeconds = 8f;
	Transform cloudParent;

	enum CloudType { High, Low }

	private void Start() {
		cloudParent = new GameObject().transform;
		cloudParent.name = $"{type.ToString()} Cloud";
	}

	private void OnEnable() {
		canSpawn = true;
	}

	void Update() {
		if (canSpawn && ArcticController._visited > 0)
			SpawnCloud();
	}

	private void SpawnCloud() {
		Instantiate(cloudPrefab, transform.position + transform.up * Random.Range(-0.5f, 1), Quaternion.identity, cloudParent);
		// Instantiate(lowCloudPrefab, transform.position + transform.up * Random.Range(0, 1.5f), Quaternion.identity, lowCloudParent);
		StartCoroutine(SpawnCloudWait());
	}

	IEnumerator SpawnCloudWait() {
		canSpawn = false;
		yield return new WaitForSeconds(Random.Range(5f, cloudSpawnWaitSeconds));
		// yield return new WaitForSeconds(Random.Range(lowCloudSpawnWaitSeconds - 2f, lowCloudSpawnWaitSeconds));
		canSpawn = true;
	}
}
