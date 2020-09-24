using System;
using System.Collections;
using System.Collections.Generic;

using UnityEditor;

using UnityEngine;
using UnityEngine.UI;

public class PlayerInteractions : MonoBehaviour {
	[Header("References")]
	public float speed = 10;

	public Animator bladeAnimator;

	public Text leftWaterUI;
	public Transform leftWaterBarUI;
	public TrailRenderer waterTR;

	private int water;
	private int maxWater = 50;
	private bool filling;
	private float haveNotUsedWaterIn = 0;

	//private GameObject myLine = new GameObject();
	private Color highlightColor = new Color(176, 0, 132, 255);
	private Color normalColor = new Color(255, 119, 221, 255);
	private SpriteRenderer playerRenderer;

	private static List<Transform> playerPath = new List<Transform>();

	public bool moving;
	private Transform targetRegion;

	public GameObject line;
	private LineRenderer newLine;

	private GameObject playerCell;
	private IdentityManager.Identity playerCellID;
	private IdentityManager.Moisture playerCellMoisture;

	// Start is called before the first frame update
	void Start() {
		playerRenderer = GetComponent<SpriteRenderer>();
		playerRenderer.color = normalColor;
		waterTR.enabled = false;

		// draw line

		newLine = line.GetComponent<LineRenderer>();
		water = maxWater;
	}

	// Update is called once per frame
	void Update() {
		GFXUpdate();
		leftWaterUI.text = water.ToString();
		int height = water * 10;
		leftWaterBarUI.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(120, height); // set width is 120. water [0,50], height [0,500]
		leftWaterBarUI.gameObject.GetComponent<RectTransform>().localPosition = new Vector3(30, -(500 - height) / 2, 0);

		//// Pathfinding
		// if path is not empty, exhaust the path
		if (playerPath.Count != 0) {
			float step = speed * Time.deltaTime;
			// Are we currently moving towards a region?
			if (moving) {
				targetRegion = playerPath[0];

				transform.position = Vector3.MoveTowards(transform.position, targetRegion.position, step); // move player
				Camera.main.transform.position = Vector3.MoveTowards(Camera.main.transform.position, Camera.main.transform.position + targetRegion.position - transform.position, step); // move camera

				// align helicopter head with velocity

				/*
				Quaternion rotation = Quaternion.LookRotation(targetRegion.position, Vector3.left);
				transform.rotation = rotation;*/

				Vector3 vectorToTarget = targetRegion.position - transform.position;
				float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg - 90f; // sprite off by 90f
				Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
				transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * speed);

				if (transform.position == targetRegion.position) {
					moving = false;
					playerPath.Remove(targetRegion);
				}
			} else {
				targetRegion = playerPath[0];
				moving = true;
			}
		}
		if (targetRegion)
			DrawPlayerPath();

		//// World Interaction        
		// check what cell player is on top of 
		playerCell = SubtropicsController.Instance.world.getCellObjectAtLoc(gameObject.transform.position);
		playerCellID = playerCell.GetComponent<IdentityManager>().id;
		playerCellMoisture = playerCell.GetComponent<IdentityManager>().moisture;

		//SubtropicsController.Instance.world.MutateCell(playerCell, IdentityManager.Identity.Green);
		//playerCellMoisture = IdentityManager.Moisture.Moist;

		// kill all immediate neighbors fire, radius buffer
		foreach (var neighbor in SubtropicsController.Instance.world.GetRadius(playerCell)) {
			if (neighbor != null) {
				IdentityManager.Identity neighborID = neighbor.GetComponent<IdentityManager>().id;
				if (neighborID == IdentityManager.Identity.Fire && neighbor != null && water > 0) {

					// check nature of the cell
					if (neighbor.GetComponent<IdentityManager>().fireVariance == 1) // if tree
					{
						neighbor.GetComponent<TreeID>().burnt = true;
						SubtropicsController.Instance.world.MutateCell(neighbor, IdentityManager.Identity.Tree);
					} else {
						SubtropicsController.Instance.world.MutateCell(neighbor, IdentityManager.Identity.Green);
					}
					neighbor.GetComponent<IdentityManager>().moisture = IdentityManager.Moisture.Moist;
					water--; // use 1 water per cell
					haveNotUsedWaterIn = 0; // reset timer
					waterTR.enabled = true;
				}
			}
		}

		if (playerCellID == IdentityManager.Identity.Water) {
			// replemish water
			if (!filling && water < maxWater) {
				filling = true;
				water += 1;
				StartCoroutine(FillWater());
			}
		}
	}

	void GFXUpdate() {
		// rotate blades
		// bladeAnimator.SetBool("isMoving", playerPath.Count != 0);
		bladeAnimator.SetBool("isMoving", true);

		// trail renderer
		haveNotUsedWaterIn += Time.deltaTime;

		if (haveNotUsedWaterIn >= 1f) // turn off renderer if not used water in 1 sec
		{
			waterTR.enabled = false;
		}
	}

	IEnumerator FillWater() {
		yield return new WaitForSeconds(0.1f);
		filling = false;
	}

	void DrawPlayerPath() {
		//newLine.material = new Material(Shader.Find("Sprites/Default"));
		newLine.material = AssetDatabase.GetBuiltinExtraResource<Material>("Default-Particle.mat");
		newLine.material.SetTextureScale("_MainTex", new Vector2(10f, 1.0f));
		newLine.widthMultiplier = 0.1f;

		int seg = playerPath.Count;
		Vector3[] positions = new Vector3[seg + 1];
		positions[0] = gameObject.transform.position; // first point in line must be current player pos
		for (int i = 1; i < seg + 1; i++) {
			positions[i] = playerPath[i - 1].position;
		}
		newLine.positionCount = positions.Length;
		newLine.SetPositions(positions);
	}

	public static bool addDestinationToPath(Transform region) {
		// if region is not already in path 
		if (!playerPath.Contains(region)) {
			playerPath.Clear();
			playerPath.Add(region);

			//PrintPlayerPath();         
			return true;
		}
		return false;
	}

	/// <summary> {1} </summary>
	static void PrintPlayerPath() {
		string pathString = "";
		foreach (Transform t in playerPath) {
			pathString += t.position.ToString() + ", ";

		}
		Debug.Log(pathString);
	}

	/// <summary>
	/// Pulse effect when colliding with cloud
	/// collision with cell prefab is disabled in physics setting
	/// </summary>
	/// <param name="other"></param>
	private void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.tag == "Cloud") {
			//FindObjectOfType<RippleEffect>().Emit(Camera.main.WorldToViewportPoint(transform.position));
			StartCoroutine(FindObjectOfType<CameraShake>().Shake(0.40f, .10f));
			StartCoroutine(SlowDown(3f)); // if hit cloud, slows down to 1/4 speed for 3 sec
		}
	}

	IEnumerator SlowDown(float duration) {
		float elapsed = 0.0f;
		float slowSpeed = speed / 4;

		while (elapsed < duration) {
			speed = slowSpeed;
			elapsed += Time.deltaTime;
			yield return null;
		}

		// speed = Mathf.Lerp(slowSpeed,4 * slowSpeed, 0.25f); 
		speed = 4 * slowSpeed;
	}

	/// <summary>  player performance in a float [0,1]</summary>
	/// <returns></returns>
	/*public float GetPlayerPerformance()
	{
	    Debug.Log(VAR);
	}*/
}
