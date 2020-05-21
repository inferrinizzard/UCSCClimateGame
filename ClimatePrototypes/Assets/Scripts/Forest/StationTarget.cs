using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Events;

public class StationTarget : MonoBehaviour {
	BoxCollider2D col;
	Dictionary<Transform, bool> subtargets = new Dictionary<Transform, bool>();
	int awaitSubTargetReached = 0;

	void Start() {
		col = GetComponent<BoxCollider2D>();
		foreach (Transform child in transform)
			subtargets.Add(child, false);
	}

	void Update() {
		if (awaitSubTargetReached > 0)
			foreach (var kvp in subtargets)
				foreach (var task in ForestController.Instance.volunteers)
					if ((task.volunteer.transform.position - kvp.Key.position).sqrMagnitude < .05) {
						subtargets[kvp.Key] = false;
						awaitSubTargetReached--;
						return;
					}
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.CompareTag("Player"))
			if (other.TryGetComponent(out Volunteer v))
				awaitSubTargetReached++;
	}

	void OnMouseDown() {
		if (ForestController.Instance.hasSelected) {
			var selectedTarget = subtargets.Where(kvp => !kvp.Value).OrderBy(kvp => kvp.Key.position.y).ElementAt(0).Key;
			subtargets[selectedTarget] = true;
			ForestController.Instance.SetTarget(selectedTarget.position, VolunteerActions.Capture);
		}
	}
}
