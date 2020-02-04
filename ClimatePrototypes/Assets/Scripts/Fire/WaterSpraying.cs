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
	[SerializeField] Slider waterSlider = default;

	public float maxWater = 1000f;
	float curWater = 1000f;

	float waterRate = 3f;

	private void Start() { }
	void Update() {
		damageText.text = $"Damage: {damage}";

		Vector2 mousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
		Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);

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
				Ray ray = Camera.main.ScreenPointToRay(mousePos);
				Collider2D[] cols = Physics2D.OverlapCircleAll(worldPos, 1);
				foreach (Collider2D col in cols) {
					FadeFire fadeFire = col.GetComponent<FadeFire>();
					if (fadeFire != null)
						fadeFire.Fade();
				}
				cloneWater.transform.position = worldPos;
			}
		}
	}

	void OnDrawGizmos() {
		Gizmos.color = Color.red;
		Gizmos.DrawSphere(new Vector2(Input.mousePosition.x, Input.mousePosition.y), 1);
	}
}
