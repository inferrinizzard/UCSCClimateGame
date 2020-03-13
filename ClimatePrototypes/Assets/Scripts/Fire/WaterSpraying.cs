using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class WaterSpraying : MonoBehaviour {
	public GameObject particle;
	GameObject cloneWater = null;
	public static float damage = 0;
	[SerializeField] Text damageText = default;
	[SerializeField] Text timerText = default;
	[SerializeField] Slider waterSlider = default;
	float timer = 60f;

	public float maxWater = 5000f;
	float curWater = 5000f;

	public float waterRate = 1f;

	private void Start() {
		curWater = maxWater;
		timerText.text = string.Format("{00}", timer);
	}
	void Update() {
		timerText.text = string.Format("{00}", Mathf.Floor(timer -= Time.deltaTime));
		damageText.text = $"Damage: {damage}";

		Vector2 mousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
		Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);

		if (timer > 0)
			if (cloneWater == null) {
				if (Input.GetMouseButtonDown(0) && curWater > 0) {
					cloneWater = Instantiate(particle, worldPos, Quaternion.identity);
				}
			} else {
				if (Input.GetMouseButtonUp(0) || curWater <= 0) {
					Destroy(cloneWater);
					cloneWater = null;
				}
				if (Input.GetMouseButton(0) && curWater > 0) {
					curWater -= waterRate;
					waterSlider.value = curWater / maxWater;
					// Ray ray = Camera.main.ScreenPointToRay(mousePos);
					foreach (Collider2D col in Physics2D.OverlapCircleAll(worldPos, 1))
						col.GetComponent<FadeFire>()?.Fade();
					cloneWater.transform.position = worldPos;
				}
			}

		if (timer < 0 || curWater <= 0) {
			Destroy(cloneWater);
			cloneWater = null;
			World.UpdateFactor("co2", damage / 100);
		}
	}

	void OnDrawGizmos() {
		Gizmos.color = Color.red;
		Gizmos.DrawSphere(new Vector2(Input.mousePosition.x, Input.mousePosition.y), 1);
	}
}
