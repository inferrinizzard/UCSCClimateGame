using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class VolunteerUI : MonoBehaviour {
	[SerializeField] GameObject worldPrefab;
	bool selected = false;
	Animator animator;

	GameObject volunteer;

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

	public void SpawnVolunteer() {
		volunteer = GameObject.Instantiate(worldPrefab, Camera.main.ScreenToWorldPoint(transform.position), Quaternion.identity); //needs parent

	}
}
