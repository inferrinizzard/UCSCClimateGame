using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class WaterArrow : MonoBehaviour {
	Transform player;
	[HideInInspector] public Vector3 waterPosition;
	[SerializeField] int reservoirIndex = 0;
	[SerializeField] float speed = 10f;

	void Start() {
		player = SubtropicsController.Instance.player;
		waterPosition = SubtropicsController.World?.reservoirs[reservoirIndex].transform.position ?? Vector3.zero;
	}

	void FixedUpdate() {
		// Get degrees between two vectors
		Vector3 targetDir = waterPosition - player.position;
		Vector3 currentDir = (player.position - transform.position);

		float crossP = Vector3.Cross(targetDir, currentDir).z;

		float angle = Vector3.Angle(currentDir, targetDir);
		float rotateSpeed = Mathf.Sign(crossP) * speed;

		if (Mathf.Abs(180 - angle) > 15) // damping
			transform.RotateAround(player.transform.position, new Vector3(0, 0, 1), rotateSpeed * Time.fixedDeltaTime);

		// point local right to parent object - red-axis-x
		transform.up = -(player.transform.position - transform.position);
	}
}
