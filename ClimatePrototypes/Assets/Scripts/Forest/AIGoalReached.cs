using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class AIGoalReached : MonoBehaviour { // TODO: you can probably get rid of this class
	public GameObject myAgent;
	void OnTriggerEnter2D(Collider2D other) {
		Debug.Log("trigger entered :  " + other.name);
		if (myAgent != null && other.name == myAgent.name)
			myAgent.GetComponent<Animator>().SetInteger("animState", 0);
	}
}
