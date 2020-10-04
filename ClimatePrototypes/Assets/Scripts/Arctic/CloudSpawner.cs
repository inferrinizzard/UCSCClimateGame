using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Random = UnityEngine.Random;

public class CloudSpawner : MonoBehaviour {
	public enum CloudType { High, Low }

	[SerializeField] CloudType type = CloudType.High;
	[SerializeField] GameObject cloudPrefab = default;
	[SerializeField] bool canSpawn = true;
	[SerializeField] float cloudSpawnWaitSeconds = 8f;
	Transform cloudParent;
	bool left = true;

	void Start() {
		cloudParent = new GameObject($"{type.ToString()} Cloud").transform;
		if (canSpawn)
			StartCoroutine(SpawnCloud());
		if (Random.value >.5) {
			left = false;
			transform.position = Vector3.Scale(transform.position, new Vector3(-1, 1, 1));
		}
	}

	IEnumerator SpawnCloud() {
		yield return new WaitForSeconds(Random.Range(3f, cloudSpawnWaitSeconds) * (1 - .5f * ArcticController.Instance.tempInfluence) * (ArcticController.Instance.summer ? 1 : .8f));
		Instantiate(cloudPrefab, transform.position + transform.up * Random.Range(-0.5f, 1f), Quaternion.identity, cloudParent).GetComponent<Cloud>().flipped = !left;
		StartCoroutine(SpawnCloud());
	}
}
