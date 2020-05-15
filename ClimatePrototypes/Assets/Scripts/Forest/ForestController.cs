using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Pathfinding;

using UnityEngine;
using UnityEngine.Events;

public class ForestController : MonoBehaviour {
	public static ForestController Instance;
	[SerializeField] GameObject volunteerPrefab = default;
	[HideInInspector] public VolunteerUI selected;
	public bool hasSelected { get => selected != null; }

	[HideInInspector] public Transform agentParent, utility;
	public List<VolunteerTask> volunteers = new List<VolunteerTask>();
	public List<Vector3Int> activeTiles { get => volunteers.Where(v => v.activeTile != null).Select(v => v.activeTile.Value).ToList(); }

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

	public void SetTarget(Vector3 pos, UnityAction<Volunteer> onReached) {
		var newVolunteer = GameObject.Instantiate(volunteerPrefab, Camera.main.ScreenToWorldPoint(selected.transform.position), Quaternion.identity, agentParent).GetComponent<Volunteer>();
		newVolunteer.ID = volunteers.Count;
		newVolunteer.name += $" {newVolunteer.ID}";
		newVolunteer.transform.position = new Vector3(newVolunteer.transform.position.x, newVolunteer.transform.position.y, 0);
		newVolunteer.gameObject.SetActive(true);

		volunteers.Add(new VolunteerTask(newVolunteer, selected));
		selected.gameObject.SetActive(false);
		selected = null;

		newVolunteer.AssignTarget(pos);
		newVolunteer.OnReached.AddListener(onReached);
	}

	public void SetTarget(Vector3Int pos, UnityAction<Volunteer> onReached) {
		SetTarget((Vector3) pos, onReached);
		volunteers[volunteers.Count - 1].activeTile = pos;
	}
}

public class VolunteerTask {
	public Volunteer volunteer;
	public VolunteerUI UI;
	public Vector3Int? activeTile;
	public VolunteerTask(Volunteer v, VolunteerUI vUI, Vector3Int? tile = null) => (volunteer, UI, activeTile) = (v, vUI, tile);
}
