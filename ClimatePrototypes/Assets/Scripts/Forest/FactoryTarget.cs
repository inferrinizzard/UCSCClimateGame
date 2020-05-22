using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Events;

public class FactoryTarget : MonoBehaviour {
	BoxCollider2D col;
	void Start() {
		col = GetComponent<BoxCollider2D>();
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
		if (ForestController.Instance.hasSelected) {
			ForestController.Instance.SetVolunteerTarget(Camera.main.ScreenToWorldPoint(Input.mousePosition), VolunteerActions.Protest);
		}
	}
}
