using System.Collections;
using System.Collections.Generic;

using Pathfinding;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

public class VolunteerActions {
	public static void Plant(Volunteer v) {
		v.anim.SetTrigger("Shoveling");
		var task = ForestController.Instance.volunteers[v.ID];
		ForestController.Instance.StartCoroutine(TreeGrow(task.volunteer, task.activeTile.Value));
	}

	public static IEnumerator TreeGrow(Volunteer v, Vector3Int tilePos) {
		ForestGrid.ClearHover(tilePos);
		float[] times = new [] { 1, 1.5f, 2f, 2f }; //TODO: proximity logic
		for (int i = 0; i < times.Length; i++) {
			yield return new WaitForSeconds(times[i]);
			if (i == 0) {
				v.anim.ResetTrigger("Shoveling");
				v.anim.SetTrigger("Walking");
				v.AssignTarget(v.origin);
			}
			ForestGrid.map.SetTile(tilePos, ForestGrid.trees[i + 3]);
		}
	}

	public static void Protest(Volunteer v) {
		v.anim.SetBool("isProtesting", true);
	}
	public static void Capture(Volunteer v) {
		// v.anim.SetBool("isFacility", true);
	}
}
