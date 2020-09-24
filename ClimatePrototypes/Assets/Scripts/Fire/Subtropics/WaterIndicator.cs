using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class WaterIndicator : MonoBehaviour {
	public Transform heliTranform;
	public Vector3 waterPosition;
	public int reservoirIndex;
	public float speed = 10f;

	private void Start() {
		//heliTranform = gameObject.transform.parent;
		waterPosition = SubtropicsController.Instance.world.reservoirs[reservoirIndex].transform.position;
	}

	void FixedUpdate() {

		// Get degrees between two vectors
		Vector3 targetDir = waterPosition - heliTranform.position;
		Vector3 currentDir = (heliTranform.position - transform.position);

		float crossP = Vector3.Cross(targetDir, currentDir).z;

		//Debug.DrawRay(heliTranform.position, targetDir, Color.blue);
		//Debug.DrawRay(heliTranform.position, currentDir, Color.green);

		float angle = Vector3.Angle(currentDir, targetDir);
		float rotateSpeed = Mathf.Sign(crossP) * this.speed;

		//Debug.Log("angle is " + angle);

		if (Mathf.Abs(180 - angle) > 15) // dampling
			transform.RotateAround(heliTranform.transform.position, new Vector3(0, 0, 1), rotateSpeed * Time.fixedDeltaTime);

		// point local right to parent object - red-axis-x
		transform.up = -(heliTranform.transform.position - transform.position);
		// rotate locally in z 20 to correct for the fact that point does not allign with local left
		//gameObject.transform.Rotate(0f, 0.0f, 100f, Space.Self);   
	}
}
