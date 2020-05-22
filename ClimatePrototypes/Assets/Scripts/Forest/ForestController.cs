using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Pathfinding;

using UnityEngine;
using UnityEngine.Events;

public class ForestController : MonoBehaviour {
	public static ForestController Instance;
	[SerializeField] GameObject volunteerPrefab = default, uiPanel = default;
	[SerializeField] int numActive;
	[HideInInspector] public VolunteerUI selected;
	public bool hasSelected { get => selected != null && !overUI; }

	[HideInInspector] public bool overUI = false;

	[HideInInspector] public Transform agentParent, utility;
	public List<VolunteerTask> volunteers = new List<VolunteerTask>();
	public List<Vector3Int> activeTiles { get => volunteers.Where(v => v.activeTile != null).Select(v => v.activeTile.Value).ToList(); }

	void Awake() => Instance = this;

	public void UIHover(bool over) => overUI = over;

	void Start() {
		agentParent = new GameObject("Agent Parent").transform;
		agentParent.parent = transform;
		utility = new GameObject("Utility").transform;
		utility.parent = transform;

		uiPanel.GetComponentsInChildren<VolunteerUI>().Skip(numActive).ToList().ForEach(v => v.Deactivate());
	}

	void Update() {

	}

	public void SetVolunteerTarget(Vector3 pos, UnityAction<Volunteer> onReached) {
		var newVolunteer = GameObject.Instantiate(volunteerPrefab, Camera.main.ScreenToWorldPoint(selected.transform.position), Quaternion.identity, agentParent).GetComponent<Volunteer>();
		newVolunteer.ID = volunteers.Count;
		newVolunteer.name += $" {newVolunteer.ID}";
		newVolunteer.transform.position = new Vector3(newVolunteer.transform.position.x, newVolunteer.transform.position.y, 0);
		newVolunteer.gameObject.SetActive(true);

		volunteers.Add(new VolunteerTask(newVolunteer, selected, onReached, pos : pos));
		// selected.gameObject.SetActive(false);
		selected.AssignBubble(onReached);
		selected = null;

		newVolunteer.AssignTarget(pos);
		newVolunteer.OnReached.AddListener((PathfindingAgent agent) => onReached.Invoke(agent as Volunteer));
		newVolunteer.OnReturn.AddListener(() => volunteers[newVolunteer.ID].UI.Reset());
	}

	public void SetVolunteerTarget(Vector3Int pos, UnityAction<Volunteer> onReached) {
		SetVolunteerTarget((Vector3) pos, onReached);
		volunteers[volunteers.Count - 1].activeTile = pos;
	}
}

public class VolunteerTask {
	public Volunteer volunteer;
	public VolunteerUI UI;
	public Vector3Int? activeTile;
	public Vector3 target;
	public UnityAction<Volunteer> action;
	public VolunteerTask(Volunteer v, VolunteerUI vUI, UnityAction<Volunteer> vAction, Vector3Int? tile = null, Vector3 pos = default) => (volunteer, UI, action, activeTile, target) = (v, vUI, vAction, tile, pos);
}
