using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Pathfinding;

using UnityEngine;
using UnityEngine.Events;

public class ForestController : RegionController {
	[SerializeField] GameObject volunteerPrefab = default, uiPanel = default, emissionsTracker = default;
	[SerializeField] int numActive;
	[HideInInspector] public VolunteerUI selected;
	public bool hasSelected { get => selected != null && !overUI; }

	[HideInInspector] public bool overUI = false;

	[HideInInspector] public Transform agentParent, utility;
	public List<VolunteerTask> volunteers = new List<VolunteerTask>();
	public List<Vector3Int> activeTiles { get => volunteers.Where(v => v.activeTile != null).Select(v => v.activeTile.Value).ToList(); }
	public List<Vector3Int> activeTrees = new List<Vector3Int>();

	protected override void GameOver() {
		StopAllCoroutines();
		// stop volunteers
		// stop loggers
		// stop trees?
	}

	public void UIHover(bool over) => overUI = over;

	protected override void Start() {
		base.Start();
		agentParent = new GameObject("Agent Parent").transform;
		agentParent.parent = transform;
		utility = new GameObject("Utility").transform;
		utility.parent = transform;

		uiPanel.GetComponentsInChildren<VolunteerUI>().Skip(numActive).ToList().ForEach(v => v.Deactivate());
	}

	public PathfindingAgent NewAgent(GameObject prefab, Vector3 pos, Vector3 target) {
		var newAgent = GameObject.Instantiate(prefab, pos, Quaternion.identity, agentParent).GetComponent<PathfindingAgent>();
		newAgent.transform.position = new Vector3(newAgent.transform.position.x, newAgent.transform.position.y, 0);
		newAgent.gameObject.SetActive(true);

		newAgent.AssignTarget(target);

		return newAgent;
	}

	public void SetVolunteerTarget(Vector3 pos, UnityAction<Volunteer> onReached) {
		var newVolunteer = NewAgent(volunteerPrefab, Camera.main.ScreenToWorldPoint(selected.transform.position), pos) as Volunteer;
		newVolunteer.ID = volunteers.Count;
		newVolunteer.name += $" {newVolunteer.ID}";

		volunteers.Add(new VolunteerTask(newVolunteer, selected, onReached, pos : pos));
		// selected.gameObject.SetActive(false);
		selected.AssignBubble(onReached);
		selected = null;

		newVolunteer.OnReached.AddListener((PathfindingAgent agent) => onReached.Invoke(agent as Volunteer));
		newVolunteer.OnReturn.AddListener(() => volunteers[newVolunteer.ID].UI.Reset());
	}

	public void SetVolunteerTarget(Vector3Int pos, UnityAction<Volunteer> onReached) {
		SetVolunteerTarget((Vector3) pos, onReached);
		volunteers[volunteers.Count - 1].activeTile = pos;
	}
}

public class VolunteerTask { //TODO: do these get cleared?
	public Volunteer volunteer;
	public VolunteerUI UI;
	public Vector3Int? activeTile;
	public Vector3 target;
	public UnityAction<Volunteer> action;
	public VolunteerTask(Volunteer v, VolunteerUI vUI, UnityAction<Volunteer> vAction, Vector3Int? tile = null, Vector3 pos = default) => (volunteer, UI, action, activeTile, target) = (v, vUI, vAction, tile, pos);
}
