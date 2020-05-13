using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class ForestController : MonoBehaviour {
	public static ForestController Instance;
	[SerializeField] GameObject volunteerPrefab;

	VolunteerUI selected;
	[HideInInspector] public bool hasSelected = false;
	public VolunteerUI Selected { set { selected = value; hasSelected = value != null; } }

	void Awake() {
		Instance = this;
	}

	void Start() {

	}

	void Update() {

	}

	public void SetTarget(Vector3 pos) {
		Debug.Log(pos);

		var newVolunteer = GameObject.Instantiate(volunteerPrefab, Camera.main.ScreenToWorldPoint(selected.transform.position), Quaternion.identity, transform);
		newVolunteer.transform.position = new Vector3(newVolunteer.transform.position.x, newVolunteer.transform.position.y, 0);
		newVolunteer.SetActive(true);
		Selected = null;
	}
}
