using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Pathfinding;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

public class Volunteer : PathfindingAgent {
	// now all in parent+adj class, preserving in case of special logic
}

public class VolunteerActions {
	public static int GetBubble(UnityAction<Volunteer> action) {
		if (action == Capture)
			return 1;
		else if (action == Protest)
			return 3;
		return 2;
	}

	public static void Plant(Volunteer v) {
		v.anim.SetTrigger("Shoveling");
		var task = ForestController.Instance.volunteers[v.ID];
		ForestController.Instance.StartCoroutine(TreeGrow(task.volunteer, task.activeTile.Value));
	}

	public static IEnumerator TreeGrow(Volunteer v, Vector3Int tilePos) {
		ForestGrid.ClearHover(tilePos);
		yield return new WaitForSeconds(1);
		v.anim.ResetTrigger("Shoveling");
		v.anim.SetTrigger("Walking");
		v.AssignTarget(v.origin);
		ForestGrid.currentTrees.Add(new ForestTree(tilePos));
		// if (i == 2)
		// 	ForestController.Instance.activeTrees.Add(tilePos);
	}

	public static IEnumerator WaitAndReturn(PathfindingAgent agent, float duration = 1) {
		yield return new WaitForSeconds(duration);
		agent.transform.localScale = Vector3.one;
		agent.anim.SetTrigger("Walking");
		agent.AssignTarget(agent.origin);
	}

	public static void Protest(Volunteer v) {
		v.anim.SetTrigger("Protesting");
		ForestController.Instance.StartCoroutine(WaitAndReturn(v, 3));
	}

	public static void Clear(Volunteer v) {
		v.anim.SetTrigger("Shoveling");
		var task = ForestController.Instance.volunteers[v.ID];
		ForestController.Instance.StartCoroutine(ClearAndReturn(v, task.activeTile.Value));
	}

	static IEnumerator ClearAndReturn(Volunteer v, Vector3Int tilePos) {
		yield return ForestController.Instance.StartCoroutine(VolunteerActions.WaitAndReturn(v, 3));
		ForestGrid.ClearHover(tilePos);
		ForestGrid.map.SetTile(tilePos, ForestGrid.empty);
		ForestGrid.RemoveTree(tilePos);
	}
	public static void Capture(Volunteer v) {
		// v.anim.SetTrigger("Facility");
		ForestController.Instance.StartCoroutine(CaptureAndReturn(v, 3));
	}

	static IEnumerator CaptureAndReturn(Volunteer v, float time, int steps = 20) {
		ForestController.Instance.StartCoroutine(WaitAndReturn(v, time));
		// for (var(start, step) = (Time.time, 0f); step < time; step = Time.time - start) {
		// yield return null;
		for (int i = steps; i > 0; i--) {
			yield return new WaitForSeconds(time / steps);
			ForestController.Instance.damage = Mathf.Max(-100, ForestController.Instance.damage - 1);
		}
	}
}
