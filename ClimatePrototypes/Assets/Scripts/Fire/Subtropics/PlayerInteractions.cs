using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEditor;

using UnityEngine;
using UnityEngine.UI;

public class PlayerInteractions : MonoBehaviour {
	[SerializeField] float speed = 10;
	[Header("References")]
	[SerializeField] Text leftWaterUI = default;
	[SerializeField] Transform leftWaterBarUI = default;
	Animator bladeAnimator;
	TrailRenderer waterTR;

	int water, maxWater = 50;
	bool filling = false, slow = false;
	float lastUsedWater = 0;

	Color highlightColor = new Color(176, 0, 132, 255), normalColor = new Color(255, 119, 221, 255);
	SpriteRenderer playerRenderer;

	public Vector3? target;

	[SerializeField] GameObject line = default;
	LineRenderer newLine;

	void Start() {
		bladeAnimator = GetComponentInChildren<Animator>();
		waterTR = GetComponentInChildren<TrailRenderer>();
		playerRenderer = GetComponent<SpriteRenderer>();
		playerRenderer.color = normalColor;
		waterTR.enabled = false;

		// draw line
		newLine = line.GetComponent<LineRenderer>();
		//newLine.material = new Material(Shader.Find("Sprites/Default"));
		newLine.material = AssetDatabase.GetBuiltinExtraResource<Material>("Default-Particle.mat");
		newLine.material.SetTextureScale("_MainTex", new Vector2(10f, 1.0f));
		newLine.widthMultiplier = 0.1f;

		bladeAnimator.SetBool("isMoving", true);
		water = maxWater;
	}

	void Update() {
		lastUsedWater += Time.deltaTime;

		if (lastUsedWater >= 1f) // turn off renderer if not used water in 1 sec
			waterTR.enabled = false;

		leftWaterUI.text = water.ToString();
		int height = water * 10;
		(leftWaterBarUI as RectTransform).sizeDelta = new Vector2(120, height); // set width is 120. water [0,50], height [0,500]
		(leftWaterBarUI as RectTransform).localPosition = new Vector3(30, -(500 - height) / 2, 0);
		// convert to slider

		Path();
		Extinguish();
	}

	void Path() {
		if (target != null) {
			float step = speed * Time.deltaTime;
			// Are we currently moving towards a region?
			transform.position = Vector3.MoveTowards(transform.position, target.Value, step); // move player
			Camera.main.transform.position = Vector3.MoveTowards(Camera.main.transform.position, Camera.main.transform.position + target.Value - transform.position, step); // move camera
			// TODO: nest camera under player

			Vector3 vectorToTarget = target.Value - transform.position;
			float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg - 90f;
			Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
			transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * speed);

			if (transform.position == target)
				target = null;
			DrawPlayerPath();
		}
	}

	void Extinguish() {
		// check what cell player is on top of 
		var playerCell = SubtropicsController.World.GetCell(gameObject.transform.position).GetComponent<IdentityManager>();

		//SubtropicsController.World.MutateCell(playerCell, IdentityManager.Identity.Green);
		//playerCell.moisture = IdentityManager.Moisture.Moist;

		// kill all immediate neighbors fire, radius buffer
		foreach (var neighbor in SubtropicsController.World.GetRadius(playerCell.gameObject)) {
			IdentityManager neighborID = neighbor.GetComponent<IdentityManager>();
			if (neighborID.id == IdentityManager.Identity.Fire && water > 0) {
				// check nature of the cell
				if (neighborID.fireVariance == 1) // if tree
				{
					neighbor.GetComponent<TreeID>().burnt = true;
					SubtropicsController.World.MutateCell(neighbor, IdentityManager.Identity.Tree);
				} else {
					SubtropicsController.World.MutateCell(neighbor, IdentityManager.Identity.Green);
				}
				neighborID.moisture = IdentityManager.Moisture.Moist;
				water--; // use 1 water per cell
				lastUsedWater = 0; // reset timer
				waterTR.enabled = true;
			}
		}

		if (!filling && water < maxWater) {
			filling = true;
			water += 1;
			StartCoroutine(FillWater());
		}
	}

	IEnumerator FillWater() {
		yield return new WaitForSeconds(0.1f);
		filling = false;
	}

	void DrawPlayerPath() {
		if (target != null) {
			newLine.positionCount = 2;
			newLine.SetPositions(new Vector3[] { transform.position, target.Value });
		} else
			newLine.positionCount = 0;
	}

	/// <summary>
	/// Pulse effect when colliding with cloud
	/// collision with cell prefab is disabled in physics setting
	/// </summary>
	/// <param name="other"></param>
	void OnTriggerEnter2D(Collider2D other) {
		if (other.transform.TryGetComponent(out SubtropicsCloud cloud)) {
			//Camera.maib.GetComponent<RippleEffect>().Emit(Camera.main.WorldToViewportPoint(transform.position));
			StartCoroutine(Camera.main.GetComponent<CameraShake>().Shake(0.40f, .10f));
			if (!slow)
				StartCoroutine(SlowDown(3f)); // if hit cloud, slows down to 1/4 speed for 3 sec
		}
	}

	IEnumerator SlowDown(float duration, float factor = 4) {
		float slowSpeed = speed / factor;
		slow = true;
		for (float elapsed = 0.0f; elapsed < duration; elapsed += Time.deltaTime) {
			speed = slowSpeed;
			yield return null;
		}
		// speed = Mathf.Lerp(slowSpeed,factor * slowSpeed, 0.25f); 
		slow = false;
		speed = factor * slowSpeed;
	}
}
