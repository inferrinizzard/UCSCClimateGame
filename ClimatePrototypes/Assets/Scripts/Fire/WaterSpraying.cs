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

	private void Start() { }
	void Update() {
		damageText.text = $"Damage: {damage}";

		Vector2 mousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
		Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);

		if (cloneWater == null) {
			if (Input.GetMouseButtonDown(0)) {
				cloneWater = Instantiate(particle, worldPos, Quaternion.identity);
			}
		} else {
			if (Input.GetMouseButton(0)) {
				Ray ray = Camera.main.ScreenPointToRay(mousePos);
				Collider2D[] cols = Physics2D.OverlapCircleAll(worldPos, 1);
				foreach (Collider2D col in cols) {
					FadeFireC fadeFire = col.GetComponent<FadeFireC>();
					if (fadeFire != null)
						fadeFire.Fade();
				}
				cloneWater.transform.position = worldPos;
			}
			if (Input.GetMouseButtonUp(0)) {
				Destroy(cloneWater);
				cloneWater = null;
			}
		}
	}

	void OnDrawGizmos() {
		Gizmos.color = Color.red;
		Gizmos.DrawSphere(new Vector2(Input.mousePosition.x, Input.mousePosition.y), 1);
	}
}
