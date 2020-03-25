using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class FireController : MonoBehaviour {
	[SerializeField] int numFires = 5;
	public static float damage = 0;
	float timer = 60f;
	[SerializeField] GameObject firePrefab = default;
	[SerializeField] Text damageText = default;
	[SerializeField] Text timerText = default;
	[SerializeField] Slider waterSlider = default;
	[SerializeField] HoseSpray spray = default;

	void Start() {
		timerText.text = string.Format("{00}", timer);
		Spawn();
	}

	void Update() {
		timerText.text = string.Format("{00}", Mathf.Floor(timer -= Time.deltaTime));
		damageText.text = $"Damage: {damage}";
		waterSlider.value = spray.curWater / spray.maxWater;

		if (spray.curWater <= 0) {
			if (timer > 0) {
				// EnablePrompt();
				// StartCoroutine("Blink");
				// TODO: refill
			} else {
				// pause and prompt here
				World.co2.Update(World.Region.Fire, World.Region.City, damage / 100);
			}
		}
	}

	void Spawn() {
		for (int i = 0; i < numFires; i++) {
			Fire newFire = Instantiate(firePrefab,
				RandomPoint(
					Func.Lambda((Vector3 vec) => Mathf.Max(vec.x, vec.y) / 2)
					(firePrefab.GetComponent<SpriteRenderer>().bounds.min)),
				Quaternion.identity).GetComponent<Fire>();
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

	// Vector3 RandomPoint(float margin = 0) =>
	// 		new Vector3(Random.Range(margin - Screen.width / 2, Screen.width / 2 - margin),
	// 			Random.Range(margin - Screen.height / 2, Screen.height / 2 - margin - 64), 0);
	Vector3 RandomPoint(float margin = 0) {
		Vector3 randomPos = Camera.main.ViewportToWorldPoint(new Vector2(Random.value, Random.value));
		// randomPos.x = Mathf.Clamp(randomPos.x, margin - Screen.width / 2, Screen.width / 2 - margin);
		// randomPos.y = Mathf.Clamp(randomPos.y, margin - Screen.height / 2, Screen.height / 2 - 48 - margin);
		randomPos.z = 0;
		return randomPos;
	}
}
