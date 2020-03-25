using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class HoseSpray : MonoBehaviour {
	[SerializeField] float maxWater = 5000f;
	public float curWater = 5000f;
	[SerializeField] float incWater = 500f;
	Camera cam;
	Collider2D col;

	void Start() {
		cam = Camera.main;
		Cursor.visible = false;

		col = GetComponentInChildren<Collider2D>();
		col.gameObject.SetActive(false);

		curWater = maxWater;
	}

	void Update() {
		transform.position = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x - 200f, Input.mousePosition.y, cam.nearClipPlane));

		if (Input.GetMouseButtonDown(0) && curWater > 0)
			col.gameObject.SetActive(true);
		else if (Input.GetMouseButtonUp(0) || curWater <= 0)
			col.gameObject.SetActive(false);
		else if (Input.GetMouseButton(0) && curWater > 0) {
			curWater--;
			List<Collider2D> hits = new List<Collider2D>();
			if (col.OverlapCollider((new ContactFilter2D()).NoFilter(), hits) > 0)
				foreach (Collider2D fire in hits)
					fire.GetComponent<Fire>()?.Fade();
		}
	}
}
