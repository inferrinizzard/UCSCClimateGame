using System.Collections;
using System.Collections.Generic;

using Pathfinding;

using UnityEngine;
using UnityEngine.Events;

public class PathfindingAgent : MonoBehaviour {
	protected AIDestinationSetter setter;
	protected Transform pathTarget;
	protected AIGoalReached targetGoal;
	protected AIPath pathfinder;
	[HideInInspector] public Vector3 origin;
	[HideInInspector] public Animator anim;
	public AgentUnityEvent OnReached;
	public UnityEvent OnReturn;
	[HideInInspector] public int ID = -1;

	void Awake() {
		setter = GetComponent<AIDestinationSetter>();
		anim = GetComponent<Animator>();
		pathfinder = GetComponent<AIPath>();
		origin = transform.position;

		pathTarget = new GameObject($"Path Target[{name}]").transform;
		pathTarget.parent = ForestController.Instance.agentParent;
		targetGoal = pathTarget.gameObject.AddComponent<AIGoalReached>();
		targetGoal.myAgent = gameObject;

		(OnReached, OnReturn) = (new AgentUnityEvent(), new UnityEvent());
		OnReturn.AddListener(Return);
	}

	void Update() {
		if (setter.target != null && (pathTarget.position - transform.position).sqrMagnitude < .1) {
			ReachedTarget(); // TODO: event listener
		}
	}

	public void AssignTarget(Vector3 targetPos) {
		// pathfinder.enabled = true;
		pathfinder.canSearch = true;
		pathfinder.isStopped = false;
		pathTarget.position = targetPos;
		setter.target = pathTarget;

		anim.SetTrigger("Walking");
		transform.localScale = new Vector3(targetPos.x < transform.position.x ? -1 : 1, 1, 1);
	}

	public void Stop() {
		pathfinder.isStopped = true;
		pathfinder.canSearch = false;
	}

	void ReachedTarget() {
		// pathfinder.canMove = false;
		// pathfinder.enabled = false;
		pathfinder.isStopped = true;
		transform.localScale = Vector3.one;
		anim.ResetTrigger("Walking");
		if (setter.target.position == origin)
			OnReturn.Invoke();
		else
			OnReached.Invoke(this);
		setter.target = null;
	}

	protected virtual void Return() {
		Destroy(pathTarget.gameObject);
		Destroy(gameObject);
	}
}

public class AgentUnityEvent : UnityEvent<PathfindingAgent> { }
