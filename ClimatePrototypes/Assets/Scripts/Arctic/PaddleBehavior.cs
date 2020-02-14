using System;
using System.Collections;
using System.Collections.Generic;
using MathNet.Numerics.Distributions;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

public class PaddleBehavior : MonoBehaviour {
	private Rigidbody2D paddleRb2d;
	public float paddleSpeed = 5f;
	private float horizontalInput = 0f;
	//public float stageWidth = 11f;
	void Start() {
		paddleRb2d = GetComponent<Rigidbody2D>();
	}

	private void Move(float input) {
		paddleRb2d.velocity = Vector3.right * input * paddleSpeed;
	}

	private void Update() {
		horizontalInput = Input.GetAxis("Horizontal");

	}

	private void FixedUpdate() {
		if (Mathf.Abs(horizontalInput) > 0.1f)
			Move(horizontalInput);
		else {
			Move(0f);
		}
	}
}
