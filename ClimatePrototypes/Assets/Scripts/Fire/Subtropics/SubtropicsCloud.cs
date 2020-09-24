using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
		velocity = SubtropicsController.Instance.wind.dir.ToString()
			.Select(d => (
				new Dictionary<char, Vector3> { { 'S', Vector3.down },
					{ 'N', Vector3.up },
					{ 'W', Vector3.left },
					{ 'E', Vector3.right }
				}
			) [d]).Aggregate((acc, d) => acc + d);
	}

	/// <summary> object pool for clouds, out of sight, destroy</summary>
	void CheckDestroyEvent() {
		if (transform.position.y < -7) {
			//Destroy(gameObject);
		}
	}

}
