using System.Collections;
using System.Collections.Generic;

using Pathfinding;

using UnityEngine;
using UnityEngine.Events;

public class ForestController : MonoBehaviour {
	public static ForestController Instance;
	[SerializeField] GameObject volunteerPrefab = default;
	public Sprite[] trees;

	[HideInInspector] public VolunteerUI selected;
	public bool hasSelected { get => selected != null; }

	[HideInInspector] public Transform agentParent, utility;
	public List<Vector3Int> activeTiles = new List<Vector3Int>();
	List<Volunteer> volunteers = new List<Volunteer>();

	void Awake() {
		Instance = this;
	}

	void Start() {
		agentParent = new GameObject("Agent Parent").transform;
		agentParent.parent = transform;
		utility = new GameObject("Utility").transform;
		utility.parent = transform;
	}

	void Update() {

	}

	public void SetTarget(Vector3 pos) {
		Debug.Log(pos);

		var newVolunteer = GameObject.Instantiate(volunteerPrefab, Camera.main.ScreenToWorldPoint(selected.transform.position), Quaternion.identity, agentParent).GetComponent<Volunteer>();
		volunteers.Add(newVolunteer);
		newVolunteer.name += $" {volunteers.Count}";
		newVolunteer.transform.position = new Vector3(newVolunteer.transform.position.x, newVolunteer.transform.position.y, 0);
		newVolunteer.gameObject.SetActive(true);
		selected = null;

		newVolunteer.AssignTarget(pos);
		newVolunteer.OnReached.AddListener(() => Debug.Log("reached2"));
	}

	public void SetTarget(Vector3Int pos) {
		activeTiles.Add(pos);
		SetTarget((Vector3) pos);
	}
}
