using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[System.Serializable]
public class FireController : MonoBehaviour {
	[SerializeField] int numFires = 5;
	[SerializeField] GameObject firePrefab = default;
	private float temp;
	// Start is called before the first frame update
	void Start() {
		Spawn();
	}

	// Update is called once per frame
	void Spawn() {
		for (int i = 0; i < numFires; i++) {
			FadeFire newFire = Instantiate(firePrefab,
				RandomPoint(
					Func.Lambda((Vector3 vec) => Mathf.Max(vec.x, vec.y) / 2)
					(firePrefab.GetComponent<SpriteRenderer>().bounds.min)),
				Quaternion.identity).GetComponent<FadeFire>();
			if (World.averageTemp < 70) {
				float temp = (Random.Range(.5f, .8f));
				newFire.transform.localScale *= temp;
				newFire.health *= temp;
			} else if (World.averageTemp < 90) {
				float temp = (Random.Range(.8f, 1f));
				newFire.transform.localScale *= temp;
				newFire.health *= temp;
			} else if (World.averageTemp > 90) {
				float temp = (Random.Range(1f, 2f));
				newFire.transform.localScale *= temp;
				newFire.health *= temp;
			}
			// if (World.averageTemp > 25) // do nothing
			newFire.transform.SetParent(transform);
			newFire.name += $" {i}";
		}
	}

	Vector3 RandomPoint(float margin = 0) {
		Vector3 randomPos = Camera.main.ViewportToWorldPoint(new Vector2(Random.value, Random.value));
		// randomPos.x = Mathf.Clamp(randomPos.x, margin - Screen.width / 2, Screen.width / 2 - margin);
		// randomPos.y = Mathf.Clamp(randomPos.y, margin - Screen.height / 2, Screen.height / 2 - 48 - margin);
		randomPos.z = 0;
		return randomPos;
	}
}
