using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Events;

public class Station : MonoBehaviour {
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
				foreach (var task in (ForestController.Instance as ForestController).volunteers)
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
		if ((ForestController.Instance as ForestController).hasSelected) {
			var selectedTarget = subtargets.Where(kvp => !kvp.Value).OrderBy(kvp => kvp.Key.position.y).ElementAt(0).Key;
			subtargets[selectedTarget] = true;
			(ForestController.Instance as ForestController).SetVolunteerTarget(selectedTarget.position, VolunteerActions.Capture);
		}
	}
}
