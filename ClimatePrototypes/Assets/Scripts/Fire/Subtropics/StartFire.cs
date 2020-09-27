// using System;
// using System.Collections;
// using System.Collections.Generic;

// using UnityEngine;

// public class StartFire : MonoBehaviour {
// 	private bool isOnFire;
// 	private SpriteRenderer fireRenderer;
// 	private Transform fireTransform;
// 	private Color oriFireColor;
// 	private bool waiting;

// 	void Start() {

// 		isOnFire = false;
// 		fireRenderer = GetComponent<SpriteRenderer>(); // grab my own spriteRenderer
// 		oriFireColor = fireRenderer.color;
// 		fireTransform = GetComponent<Transform>();

// 		//InitiateFire();
// 	}

// 	// Update is called once per frame
// 	void Update() {
// 		if (isOnFire) {
// 			GrowFire();
// 		}
// 		if (!isOnFire && !waiting) {
// 			// randomly start fire
// 			waiting = true;
// 			float timer = UnityEngine.Random.Range(1f, 6.0f);
// 			StartCoroutine(WaitFor(timer));
// 		}
// 	}

// 	IEnumerator WaitFor(float s) {
// 		yield return new WaitForSeconds(s);
// 		InitiateFire();
// 		waiting = false;
// 	}

// 	/// <summary> Initiate the fire </summary>
// 	private void InitiateFire() {
// 		// change fireRenderer from brown(default) to red
// 		//fireRenderer.color = Color.red;

// 		isOnFire = true;
// 	}

// 	/// <summary> Kill the fire </summary>
// 	public void KillFire() {
// 		if (isOnFire && WaterIHave.EnoughWater()) {
// 			fireRenderer.color = oriFireColor;
// 			isOnFire = false;
// 		}
// 	}

// 	/// <summary>
// 	/// Grow the fire
// 	/// growth direction, growth speed
// 	/// Pre: if in on fire
// 	/// </summary>
// 	private void GrowFire() {
// 		// Case 1: grow in all direction, slow speed
// 		//Vector3 scaleChange = new Vector3(0.01f, 0.01f, 0);
// 		//fireTransform.localScale += scaleChange;
// 		Color cacheFireColor = fireRenderer.color;
// 		fireRenderer.color = new Color(cacheFireColor.r + 0.005f, cacheFireColor.g - 0.005f, cacheFireColor.b - 0.005f);

// 		// Case 2: wind direction
// 	}

// 	/// <summary> Put out a fire </summary>
// 	private void OnMouseDown() {
// 		PlayerInteractions.addDestinationToPath(gameObject.transform);
// 	}

// 	/// <summary> If another active fire touches me, I start fire. </summary>
// 	/// <param name="other"></param>
// 	private void OnTriggerEnter2D(Collider2D other) {
// 		if (!isOnFire && other.gameObject.tag == "Fire") {
// 			GrowFire();
// 			isOnFire = true;
// 		}
// 	}

// 	public void UseWater() {
// 		/*if(WaterIHave.EnoughWater())
// 		    WaterIHave.UseWater();*/
// 		WaterIHave.UseWater();
// 	}

// }
