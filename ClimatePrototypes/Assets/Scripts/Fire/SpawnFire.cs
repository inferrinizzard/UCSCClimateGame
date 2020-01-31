using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpawnFire : MonoBehaviour {
	public FireSpawnManager spawnValues;
	[SerializeField] GameObject firePrefab = default;
	// Start is called before the first frame update
	void Start() {
		Spawn();
	}

	// Update is called once per frame
	void Spawn() {
		for (int i = 0; i < spawnValues.prefabs; i++) {
			GameObject newFire = Instantiate(firePrefab, RandomPoint(Func.Lambda((Vector3 vec) => Mathf.Max(vec.x, vec.y) / 2)(firePrefab.GetComponent<SpriteRenderer>().bounds.max)), Quaternion.identity);
			// GameObject newFire = Instantiate(mediumFire, spawnValues.spawnPoints[i % spawnValues.spawnPoints.Length], Quaternion.identity);
			// if (World.averageTemp > 90) { // do nothing
			if (World.averageTemp < 70) {
				newFire.transform.localScale *= .75f;
			} else {
				newFire.transform.localScale *= .5f;
			}
			newFire.transform.SetParent(transform);
			newFire.name += $" {i}";
		}
	}

	Vector3 RandomPoint(float margin = 0) {
		Vector3 randomPos = Camera.main.ViewportToWorldPoint(new Vector2(Random.value, Random.value * .9f));
		// randomPos.x = Mathf.Clamp(randomPos.x, margin - Screen.width / 2, Screen.width / 2 - margin);
		// randomPos.y = Mathf.Clamp(randomPos.y, margin - Screen.height / 2, Screen.height / 2 - 48 - margin);
		randomPos.z = 0;
		return randomPos;
	}
}
