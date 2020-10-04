using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Pathfinding;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ForestController : RegionController {
	public static ForestController Instance { get => instance as ForestController; } // static instance

	[SerializeField] GameObject volunteerPrefab = default, uiPanel = default;
	[HideInInspector] public VolunteerUI selected;
	public int numActive;
	[HideInInspector] public bool overUI = false;
	public bool hasSelected { get => selected != null && !overUI; }

	[HideInInspector] public Transform agentParent, utility;
	public List<VolunteerTask> volunteers = new List<VolunteerTask>();
	public List<Vector3Int> activeTiles { get => volunteers.Where(v => v.activeTile != null).Select(v => v.activeTile.Value).ToList(); }
	public List<Vector3Int> activeTrees = new List<Vector3Int>();

	[SerializeField] Slider emissionsTracker = default;

	public void UIHover(bool over) => overUI = over;

	void Start() {
		damage = 100;
		agentParent = new GameObject("Agent Parent").transform;
		agentParent.parent = transform;
		utility = new GameObject("Utility").transform;
		utility.parent = transform;

		uiPanel.GetComponentsInChildren<VolunteerUI>().Skip(numActive).ToList().ForEach(v => v.Deactivate());
	}

	protected override void Update() {
		base.Update();
		emissionsTracker.value = damage / 200f; // TODO: fix slider visual logic, positive and negative but from the middle out
	}

	protected override void GameOver() {
		base.GameOver();
		StopAllCoroutines();
		double effect = damage / 200;
		TriggerUpdate(() => World.co2.Update(region, delta : effect * 1.18)); // [-1.18, 1.18]
	}

	/// <summary> Generates new volunteer / logger on click </summary>
	public PathfindingAgent NewAgent(GameObject prefab, Vector3 pos, Vector3 target) {
		var newAgent = GameObject.Instantiate(prefab, pos, Quaternion.identity, agentParent).GetComponent<PathfindingAgent>();
		newAgent.transform.position = new Vector3(newAgent.transform.position.x, newAgent.transform.position.y, 0);
		newAgent.gameObject.SetActive(true);

		newAgent.AssignTarget(target);

		return newAgent;
	}

	/// <summary> Creates volunteer and applies path target </summary>
	public void SetVolunteerTarget(Vector3 pos, UnityAction<Volunteer> onReached) {
		var newVolunteer = NewAgent(volunteerPrefab, Camera.main.ScreenToWorldPoint(selected.transform.position), pos) as Volunteer;
		newVolunteer.ID = volunteers.Count;
		newVolunteer.name += $" {newVolunteer.ID}";

		volunteers.Add(new VolunteerTask(newVolunteer, selected, onReached, pos : pos));
		// selected.gameObject.SetActive(false);
		selected.AssignBubble(onReached);
		selected = null;

		newVolunteer.OnReached.AddListener((PathfindingAgent agent) => onReached.Invoke(agent as Volunteer));
		newVolunteer.OnReturn.AddListener(() => {
			volunteers[newVolunteer.ID]?.UI.Reset();
			volunteers.RemoveAt(newVolunteer.ID);
		});
	}

	/// <summary> Vector3Int overload (for ForestGrid) </summary>
	public void SetVolunteerTarget(Vector3Int pos, UnityAction<Volunteer> onReached) {
		SetVolunteerTarget((Vector3) pos, onReached);
		volunteers[volunteers.Count - 1].activeTile = pos;
	}
}

// [System.Serializable]
public class VolunteerTask {
	public Volunteer volunteer;
	public VolunteerUI UI;
	public Vector3Int? activeTile;
	public Vector3 target;
	public UnityAction<Volunteer> action;
	public VolunteerTask(Volunteer v, VolunteerUI vUI, UnityAction<Volunteer> vAction, Vector3Int? tile = null, Vector3 pos = default) => (volunteer, UI, action, activeTile, target) = (v, vUI, vAction, tile, pos);
}
