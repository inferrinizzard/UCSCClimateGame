using System.Collections;
using System.Collections.Generic;

using Pathfinding;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

public class VolunteerActions {
	public static void Plant(Volunteer v) {
		v.anim.SetBool("isShoveling", true);
		ForestController.Instance.StartCoroutine(TreeGrow(v, ForestController.Instance.activeTiles[ForestController.Instance.volunteers.IndexOf(v)])); //TODO: kinda jank
	}

	public static IEnumerator TreeGrow(Volunteer v, Vector3Int tilePos) {
		ForestGrid.ClearHover(tilePos);
		float[] times = new [] { 1, 1.5f, 2f, 2f };
		for (int i = 0; i < times.Length; i++) {
			yield return new WaitForSeconds(times[i]);
			if (i == 0) {
				v.anim.SetBool("isShoveling", false);
				v.anim.SetBool("isWalking", true);
				v.AssignTarget(v.origin);
			}
			ForestGrid.map.SetTile(tilePos, ForestGrid.trees[i + 3]);
		}
	}
}
