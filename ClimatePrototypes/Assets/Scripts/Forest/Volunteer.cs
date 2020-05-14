using System.Collections;
using System.Collections.Generic;

using Pathfinding;

using UnityEngine;
using UnityEngine.Events;

public class Volunteer : MonoBehaviour {
	AIDestinationSetter setter;
	Transform pathTarget;
	AIGoalReached targetGoal;
	AIPath pathfinder;
	Vector3 origin;
	Animator anim;
	public UnityEvent OnReached;

	void Awake() {
		setter = GetComponent<AIDestinationSetter>();
		anim = GetComponent<Animator>();
		pathfinder = GetComponent<AIPath>();
		origin = transform.position;

		pathTarget = new GameObject($"Path Target[{name}]").transform;
		pathTarget.parent = ForestController.Instance.agentParent;
		targetGoal = pathTarget.gameObject.AddComponent<AIGoalReached>();
		targetGoal.myAgent = gameObject;
	}

	void Update() {
		if ((pathTarget.position - transform.position).sqrMagnitude < .1 && setter.target != null) {
			ReachedTarget(); // TODO: event listener
		}
	}

	public void AssignTarget(Vector3 targetPos) {
		this.print(targetPos, pathTarget, setter, targetGoal);

		pathfinder.enabled = true;
		pathTarget.position = targetPos;
		setter.target = pathTarget;

		anim.SetBool("isWalking", true);
		transform.localScale = new Vector3(targetPos.x < transform.position.x ? -1 : 1, 1, 1);
	}

	void ReachedTarget() {
		setter.target = null;
		pathfinder.enabled = false;
		anim.SetBool("isWalking", false);
		BeginAction();
	}

	void BeginAction() {
		OnReached.Invoke();
	}
}
