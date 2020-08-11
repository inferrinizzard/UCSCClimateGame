using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Events;

public class Factory : MonoBehaviour {
	BoxCollider2D col;
	int step = 0;
	int damageMultiplier = 1;
	public static int protesters = 0;

	void Start() {
		col = GetComponent<BoxCollider2D>();
	}

	void FixedUpdate() {
		if (step++ % 10 == 0)
			ForestController.Instance.damage += damageMultiplier * ((1 - protesters / (ForestController.Instance as ForestController).numActive) * 2 / 3f + 1 / 3f);
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.CompareTag("Player")) {
			if (other.TryGetComponent(out Volunteer v)) {
				Debug.Log(other.gameObject);
				v.OnReached.Invoke(v);
				v.Stop();
			}
		}
	}

	void OnMouseDown() {
		if ((ForestController.Instance as ForestController).hasSelected) {
			(ForestController.Instance as ForestController).SetVolunteerTarget(Camera.main.ScreenToWorldPoint(Input.mousePosition), VolunteerActions.Protest);
		}
	}
}
