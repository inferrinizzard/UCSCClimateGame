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
	private Vector2 screenBounds;
	private float paddleWidth;

	void Start() {
		paddleRb2d = GetComponent<Rigidbody2D>();
		screenBounds =
			Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
		paddleWidth = transform.GetComponent<SpriteRenderer>().bounds.size.x;
	}

	private void Move(float input) {
		paddleRb2d.velocity = Vector3.right * input * paddleSpeed;
	}

	private void Update() {
		horizontalInput = Input.GetAxis("Horizontal");
	}

	private void LateUpdate() {
		Vector3 viewPos = transform.position;
		viewPos.x = Mathf.Clamp(viewPos.x, screenBounds.x * -1 + paddleWidth / 2, screenBounds.x - paddleWidth / 2);
		transform.position = viewPos;
	}

	private void FixedUpdate() {
		if (Mathf.Abs(horizontalInput) > 0.1f)
			Move(horizontalInput);
		else {
			Move(0f);
		}
	}
}
