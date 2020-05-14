using System.Collections;
using System.Collections.Generic;

using Pathfinding;

using UnityEngine;

public class Volunteer : MonoBehaviour {
	AIDestinationSetter setter;
	Transform pathTarget;
	AIGoalReached targetGoal;

	void Awake() {
		setter = GetComponent<AIDestinationSetter>();

		pathTarget = new GameObject($"Path Target[{name}]").transform;
		pathTarget.parent = ForestController.Instance.agentParent;
		targetGoal = pathTarget.gameObject.AddComponent<AIGoalReached>();
		targetGoal.myAgent = gameObject;
	}

	void Update() {
		if ((pathTarget.position - transform.position).sqrMagnitude < .1 && setter.target != null) {
			setter.target = null;
			Debug.Log("reached");
		}
	}

	public void AssignTarget(Vector3 targetPos) {
		this.print(targetPos, pathTarget, setter, targetGoal);

		pathTarget.position = targetPos;
		setter.target = pathTarget;
	}
}
