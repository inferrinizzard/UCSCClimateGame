using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class FireController : MonoBehaviour {
	[SerializeField] int numFires = 5;
	public static float damage = 0;
	[SerializeField] float damageLimit = 100;
	float timer = 60f;
	[SerializeField] GameObject firePrefab = default;
	[SerializeField] Text damageText = default;
	[SerializeField] Text timerText = default;
	[SerializeField] Slider waterSlider = default;
	[SerializeField] HoseSpray spray = default;
	[SerializeField] WaterPrompt prompt = default;
	bool hovering = false;
	IEnumerator flash = null;

	void Start() {
		timerText.text = string.Format("{00}", timer);
		Spawn();
	}

	void Update() {
		timerText.text = string.Format("{00}", Mathf.Floor(timer -= Time.deltaTime));
		damageText.text = $"Damage: {damage}";
		waterSlider.value = spray.currentWater / spray.maxWater;

		if (spray.currentWater <= 0) {
			if (timer > 0) {
				prompt.SetActive(true);
			} else {
				// TODO: pause and prompt here
				World.co2.Update(World.Region.Fire, World.Region.City, damage / damageLimit);
			}
		}

		if (hovering && flash == null)
			StartCoroutine(flash = PromptFlash(2));
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
	// TODO: margins

	IEnumerator PromptFlash(float speed) {
		Slider preview = Instantiate(waterSlider.gameObject, waterSlider.transform.parent).GetComponentInChildren<Slider>();
		Destroy(preview.transform.GetChild(1).gameObject);
		preview.transform.SetSiblingIndex(0);
		preview.value = (spray.currentWater + spray.addWater) / spray.maxWater;
		Image previewBar = preview.GetComponentInChildren<Image>();
		previewBar.color = Color.red;

		float start = Time.time;

		while (hovering) {
			yield return null;
			previewBar.color = new Color(1, 0, 0, 1 - Mathf.PingPong(Time.time * speed, 1));
		}
		Destroy(preview.gameObject);
		flash = null;
	}

	public void SetHovering(bool status) => hovering = status;

	public void AddWater() {
		hovering = false;
		spray.currentWater += spray.addWater;
		World.money -= 10;
		prompt.SetActive(false);
	}
}
