using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class FireController : RegionController {
	[SerializeField] int numFires = 5;
	[SerializeField] float spawnDelayMin = 10;
	public static float damage = 0;
	public static readonly float damageLimit = 100;
	float timer = 60f;
	[SerializeField] GameObject firePrefab = default;
	[SerializeField] Text damageText = default, timerText = default;
	[SerializeField] Slider waterSlider = default;
	[SerializeField] HoseSpray spray = default;
	[SerializeField] WaterPrompt prompt = default;
	bool hovering = false;
	IEnumerator flash = null;
	float margin;

	void Start() {
		timerText.text = string.Format("{00}", timer);

		margin = Func.Lambda((Vector3 vec) => Mathf.Max(vec.x, vec.y) / 2) (firePrefab.GetComponent<SpriteRenderer>().bounds.max);
		for (int i = 0; i < numFires; i++)
			StartCoroutine(SpawnFire());
	}

	void Update() {
		timerText.text = string.Format("{00}", Mathf.Floor(timer -= Time.deltaTime));
		damageText.text = $"Damage: {damage}";
		waterSlider.value = spray.currentWater / spray.maxWater;

		if (timer > 0) {
			if (spray.currentWater <= 0)
				prompt.SetActive(true);
		} else {
			timerText.text = "0";
			prompt.SetActive(false);
			Cursor.visible = true;
			Pause();
			TriggerUpdate(() =>
				World.co2.Update(World.Region.Fire, World.Region.City, damage / damageLimit) //log-linear
			);
		}

		if (hovering && flash == null)
			StartCoroutine(flash = PromptFlash(2));
	}

	IEnumerator SpawnFire() {
		Fire newFire = Instantiate(firePrefab, RandomPoint(margin), Quaternion.identity, transform).GetComponent<Fire>();
		float temp = .5f;
		if (World.averageTemp < 10)
			temp *= Random.Range(.5f, .8f);
		else if (World.averageTemp < 20)
			temp *= Random.Range(.8f, 1f);
		else if (World.averageTemp > 20)
			temp *= Random.Range(1f, 2f);

		newFire.transform.localScale *= temp;
		newFire.health *= temp;

		yield return new WaitForSeconds(Random.Range(spawnDelayMin, spawnDelayMin * 1.5f));
		if (timer > 0)
			StartCoroutine(SpawnFire());
	}

	Vector3 RandomPoint(float margin = 0) {
		Vector3 randomPos = Camera.main.ViewportToWorldPoint(new Vector2(Random.value, Random.value));
		var min = Camera.main.ViewportToWorldPoint(new Vector2(0, 0));
		var max = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));
		return new Vector3(Mathf.Clamp(randomPos.x, margin + min.x, max.x - margin), Mathf.Clamp(randomPos.y, margin + min.y, max.y - margin), 0);
	}

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

	public void AddWater(bool add) {
		if (add) {
			if (World.money > 10) {
				hovering = false;
				spray.currentWater += spray.addWater;
				World.money -= 10;
			}
		} else
			spray.currentWater = .1f;
		prompt.SetActive(false);
	}
}
