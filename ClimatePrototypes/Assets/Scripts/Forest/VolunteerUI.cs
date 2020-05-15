using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class VolunteerUI : MonoBehaviour {
	bool selected { get => ForestController.Instance.selected == this; }
	Animator animator;

	void Start() {
		animator = GetComponent<Animator>();
	}

	public void SelectUI() {
		ForestController.Instance.selected = selected != this ? this : null;
		if (selected) {
			animator.ResetTrigger("Idle");
			animator.SetTrigger("Selected");
		} else {
			animator.ResetTrigger("Selected");
			animator.SetTrigger("Idle");
		}
		transform.GetChild(0).gameObject.SetActive(selected);
	}

	public void Reset() {
		transform.GetChild(0).gameObject.SetActive(selected);
		gameObject.SetActive(true);
	}
}
