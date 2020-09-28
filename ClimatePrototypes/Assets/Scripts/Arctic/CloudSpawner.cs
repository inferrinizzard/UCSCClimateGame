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
		cloudParent = new GameObject($"{type.ToString()} Cloud").transform;
		StartCoroutine(SpawnCloud());
	}

	IEnumerator SpawnCloud() {
		while (canSpawn) {
			if (ArcticController._visited > 0)
				Instantiate(cloudPrefab, transform.position + transform.up * Random.Range(-0.5f, 1), Quaternion.identity, cloudParent);
			yield return new WaitForSeconds(Random.Range(5f, cloudSpawnWaitSeconds));
		}
	}
}
