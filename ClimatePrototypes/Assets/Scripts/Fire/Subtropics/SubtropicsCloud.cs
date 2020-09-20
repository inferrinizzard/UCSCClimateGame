using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class SubtropicsCloud : MonoBehaviour {
	private Vector3 velocity;

	public float speed;

	// Update is called once per frame
	void Update() {
		CheckVelocity();
		CheckDestroyEvent();

		transform.position += velocity * speed * Time.deltaTime;
	}

	private void CheckVelocity() {
		if (PopulateWorld.Instance.dir == PopulateWorld.WindDir.NE) {
			velocity = new Vector3(-1, -1, 0);
		} else if (PopulateWorld.Instance.dir == PopulateWorld.WindDir.NW) {
			velocity = new Vector3(1, -1, 0);
		} else if (PopulateWorld.Instance.dir == PopulateWorld.WindDir.SE) {
			velocity = new Vector3(-1, 1, 0);
		} else if (PopulateWorld.Instance.dir == PopulateWorld.WindDir.SW) {
			velocity = new Vector3(1, 1, 0);
		} else {
			velocity = Vector3.zero;
		}
	}

	/// <summary> object pool for clouds, out of sight, destroy</summary>
	void CheckDestroyEvent() {
		if (transform.position.y < -7) {
			//Destroy(gameObject);
		}
	}

}
