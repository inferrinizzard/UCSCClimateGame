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
		animator.SetBool("isSelected", selected);
	}
}
