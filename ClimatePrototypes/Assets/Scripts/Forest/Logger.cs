﻿using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Logger : PathfindingAgent {
	[HideInInspector] public Vector3Int choppingTile;
}

public static class LoggerActions {
	public static void Chop(Logger l) {
		l.anim.SetTrigger("Chopping");
		l.transform.localScale = new Vector3(-1, 1, 1);
		ForestController.Instance.StartCoroutine(ChopAndReturn(l));
	}

	public static IEnumerator ChopAndReturn(Logger l) {
		yield return ForestController.Instance.StartCoroutine(VolunteerActions.WaitAndReturn(l, 1));
		ForestGrid.map.SetTile(l.choppingTile, ForestGrid.stump);
	}
}
