using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class HoseSpray : MonoBehaviour {
	[HideInInspector] public float currentWater = 5000f;
	public float maxWater = 5000f, addWater = 500f;
	[SerializeField] float waterRate = 10f;
	Camera cam;
	Collider2D col;

	void Start() {
		cam = Camera.main;
		Cursor.visible = false;

		col = GetComponentInChildren<Collider2D>();
		col.gameObject.SetActive(false);

		currentWater = maxWater;
	}

	void Update() {
		Cursor.visible = false;
		if (Input.GetMouseButtonDown(0) && currentWater > 0)
			col.gameObject.SetActive(true);
		else if (Input.GetMouseButtonUp(0) || currentWater <= 0)
			col.gameObject.SetActive(false);
		else if (Input.GetMouseButton(0) && currentWater > 0) {
			currentWater -= waterRate;
			List<Collider2D> hits = new List<Collider2D>();
			if (col.OverlapCollider((new ContactFilter2D()).NoFilter(), hits) > 0)
				foreach (Collider2D fire in hits)
					fire.GetComponent<Fire>()?.Fade();
		}

		if (currentWater > 0)
			transform.position = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x - 200f, Input.mousePosition.y, cam.nearClipPlane));
		else
			Cursor.visible = true;
	}
}
