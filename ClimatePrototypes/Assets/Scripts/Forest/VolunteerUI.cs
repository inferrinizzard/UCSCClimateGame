using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class VolunteerUI : MonoBehaviour {
	bool selected = false;
	Animator animator;

	void Start() {
		animator = GetComponent<Animator>();
	}

	void Update() {

	}

	public void SelectUI() {
		animator.SetBool("isSelected", selected = !selected);
		Debug.Log(selected);
		ForestController.Instance.selected = selected ? this : null;
	}
}
