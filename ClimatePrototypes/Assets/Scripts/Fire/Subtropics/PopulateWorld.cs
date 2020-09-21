using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using TMPro;

using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class PopulateWorld : MonoBehaviour {
	[Header("References")]
	public TextMeshProUGUI windSpeedTextUI;
	public Transform WindDirArrowUI;

	public GameObject cloudPrefab, cellPrefab, waterPrefab;
	public GameObject playerPrefab;

	public Sprite[] reservoirSprites;

	[HideInInspector] public GameObject[] reservoirs = new GameObject[2];

	public enum WindDir { NE, NW, SE, SW }
	public WindDir dir = WindDir.NE;
	public int windSpeed = 10;

	[Space, Header("Serialized Fields")]
	private GameObject player;

	/// <summary>
	/// Backend data
	/// according to player performance, there are three difficulty
	/// </summary>
	[Space]
	[Header("Backend")]
	[SerializeField] private int reservoirTotalSize;
	[SerializeField] private int spreadability = 3;
	public int difficulty = 3;
	[SerializeField] private int performance;
	[Range(0, 100)] public int treeDensity;

	public static PopulateWorld Instance;
	private GridLayout gridLayout;

	private void Awake() {
		Instance = this;
	}

	public Vector3Int topleftCell = new Vector3Int(-36, 16, 0);
	public Vector3Int topRightCell = new Vector3Int(32, 16, 0);
	public Vector3Int bottomleftCell = new Vector3Int(-36, -20, 0);
	public Vector3Int bottomrightCell = new Vector3Int(32, -20, 0);

	private Tilemap tilemap;

	private GameObject[, ] cellArray; // central cell data structure
	[SerializeField] private int width, height;

	private int cloudNum = 10; // have 10 at all times, 5 will be visible on screen
	private GameObject[] clouds;

	void Start() {
		clouds = new GameObject[cloudNum];
		tilemap = transform.GetComponent<Tilemap>();
		gridLayout = transform.parent.GetComponentInParent<GridLayout>();
		PopulatePlayer();
		PopulateVanillaWorld();
		PopulateWater();
		PopulateTree();
		PopulateCloud();
		StartCoroutine(WaitForFire(0)); // first fire mutation
		StartCoroutine(UpdateWindDirection());
	}

	IEnumerator UpdateWindDirection(float timer = 30) {
		yield return new WaitForSeconds(timer);
		dir = new [] { WindDir.NE, WindDir.NW, WindDir.SE, WindDir.SW }[Random.Range(0, 4)];
		windSpeed = Random.Range(5, 18);
		StartCoroutine(UpdateWindDirection());
	}

	void PopulateCloud() {
		GameObject cloudParent = new GameObject();
		cloudParent.name = "Clouds";

		Vector3 topLeftCloudPos = new Vector3(-16, 6, 0);
		Vector3 topRightCloudPos = new Vector3(14, 6, 0);
		Vector3 bottomRightCloudPos = new Vector3(14, -8, 0);

		float x = (topRightCloudPos.x - topLeftCloudPos.x) / (cloudNum - 1);
		float y = (topRightCloudPos.y - bottomRightCloudPos.y);

		for (int i = 0; i < cloudNum; i++) {
			//Debug.Log("clouds");
			clouds[i] = Instantiate(cloudPrefab, new Vector3(topLeftCloudPos.x + i * x, Random.Range(topRightCloudPos.y, topRightCloudPos.y + 10), 0), Quaternion.identity);
			clouds[i].transform.parent = cloudParent.transform;
		}
	}

	private void Update() {
		GUIUpdate();
		GetCellIDData(IdentityManager.Identity.Fire);
	}

	void GUIUpdate() {
		//windSpeedTextUI.text = $"{windSpeed} km/h";
		// arrow size
		float smooth = 5.0f;
		float arrowSize = 0.3f + (windSpeed - 5) / 2 * 0.1f;
		if (windSpeed == 0) {
			arrowSize = 0.3f;
		}

		Vector3 targetSize = new Vector3(arrowSize, arrowSize, 0);
		WindDirArrowUI.localScale = Vector3.Slerp(WindDirArrowUI.localScale, targetSize, Time.deltaTime * smooth);

		// arrow dir
		float tiltAngle = Array.IndexOf(new [] { WindDir.NE, WindDir.SE, WindDir.SW, WindDir.NW }, dir) * -90 + 135;
		Quaternion target = Quaternion.Euler(0, 0, tiltAngle);
		WindDirArrowUI.rotation = Quaternion.Slerp(WindDirArrowUI.rotation, target, Time.deltaTime * smooth);
	}

	private void PopulatePlayer() {
		player = Instantiate(playerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
		player.SetActive(true);
	}
	private void PopulateVanillaWorld() {
		GameObject cellParent = new GameObject();
		cellParent.name = "Cells";

		// get corner positions of the world 
		width = topRightCell.x - topleftCell.x + 1;
		height = topleftCell.y - bottomleftCell.y + 1;
		cellArray = new GameObject[width + 1, height + 1];

		for (var i = 0; i <= width; i++) {
			for (var j = 0; j <= height; j++) {
				Vector3Int posInt = new Vector3Int(topleftCell.x + i, bottomleftCell.y + j, 0);
				Vector3 pos = tilemap.GetCellCenterWorld(posInt);
				// instantiate and construct 2d array
				GameObject go = Instantiate(cellPrefab, pos, Quaternion.identity);
				go.transform.parent = cellParent.transform;
				go.GetComponent<IdentityManager>().id = IdentityManager.Identity.Green; // default to green
				cellArray[i, j] = go;
			}
		}
	}

	/// <summary> Create water cells </summary>
	private void PopulateWater() {
		// TODO: water size according to precipitation
		reservoirTotalSize = 0;
		int reservoirGen = Random.Range(0, 5);

		for (int r = 0; r < 2; r++) {
			var(waterHeight, waterWidth) = new [] {
				(1, 1), (4, 3), (3, 2), (1, 2), (2, 5)
			}[reservoirGen];

			int waterX = 0, waterY = 0;
			if (r == 1) // this is the last reservoir being generated
				(waterX, waterY) = (6, 5);
			else
				(waterX, waterY) = (15, 8);

			// at the easiest level, we increase reservoir size
			if (difficulty == 1) {
				waterHeight *= 2;
				waterWidth *= 2;
			}

			for (int i = waterX; i < waterX + waterWidth; i++) {
				for (int j = waterY; j < waterY + waterHeight; j++) {
					GameObject go = cellArray[i, j];
					go.GetComponent<IdentityManager>().id = IdentityManager.Identity.Water;
				}
			}

			reservoirTotalSize += waterHeight * waterWidth;

			Vector3 pos1 = tilemap.GetCellCenterWorld(GetVector3IntFromCellArray(waterX, waterY));
			Vector3 pos2 = tilemap.GetCellCenterWorld(GetVector3IntFromCellArray(waterX + waterWidth, waterY + waterHeight));
			Vector3 reservoirPos = (pos1 + pos2) / 2 + new Vector3(-0.25f, -0.25f, 0);
			GameObject watergo = Instantiate(waterPrefab, reservoirPos, Quaternion.identity);
			watergo.SetActive(true);

			watergo.GetComponent<SpriteRenderer>().sprite = reservoirSprites[reservoirGen];
			watergo.GetComponent<SpriteRenderer>().size = new Vector2(waterWidth / 4, waterHeight / 4);
			watergo.transform.localScale = new Vector3(0.4f, 0.4f, 1) * (difficulty == 1 ? 2 : 1);

			reservoirs[r] = watergo;
		}
	}

	private void PopulateTree() {
		// hand authored three regions of mountain areas filled with trees
		// Region1   left top
		int width1 = Random.Range(width / 4, width / 3); // right
		int height1 = Random.Range(height / 3, height / 2); // bottom

		// Region2    right
		int width2_0 = Random.Range(width / 3 + 4, width / 2); // left
		int width2_1 = width; // right
		int height2_0 = Random.Range(height / 6, height / 5); // bottom
		int height2_1 = height * 2 / 3; // top 

		// Region3     bottom
		int width3_0 = Random.Range(width / 9, width / 6); // left
		int width3_1 = Random.Range(width * 8 / 9, width * 9 / 10); // right
		int height3_0 = Random.Range(height * 2 / 3 + 4, height * 4 / 5); // top
		int height3_1 = height; // bottom

		GenerateTreeInRegion(0, width1, 0, height1);
		GenerateTreeInRegion(width2_0, width2_1, height2_0, height2_1);
		GenerateTreeInRegion(width3_0, width3_1, height3_0, height3_1);
	}

	void GenerateTreeInRegion(int w0, int w1, int h0, int h1) {
		for (var i = w0; i <= w1; i++) {
			for (var j = h0; j <= h1; j++) {
				GameObject go = cellArray[i, j];
				if (go.GetComponent<IdentityManager>().id == IdentityManager.Identity.Green) {
					int seed = Random.Range(0, 100);
					if (seed < treeDensity) {
						go.GetComponent<IdentityManager>().id = IdentityManager.Identity.Tree;
						go.GetComponent<IdentityManager>().fireVariance = 1; // if fire happens, set variance type to tree fire 
					}
				}
			}
		}
	}

	/// <summary> Mutates grass to fire</summary>
	void MutateToFire() {
		// pick a random cell
		// if green, mutates to fire
		int randomX = Random.Range(0, width);
		int randomY = Random.Range(0, height);
		GameObject go = cellArray[randomX, randomY];
		IdentityManager goID = go.GetComponent<IdentityManager>();
		//if (goID.id is IdentityManager.Identity.Green || goID.id is IdentityManager.Identity.Tree )  // can mutate green or tree
		if (goID.id is IdentityManager.Identity.Tree) {
			//Debug.Log("fire id" + goID.id);
			goID.id = IdentityManager.Identity.Fire;
		}
	}

	IEnumerator WaitForFire(float s) {
		yield return new WaitForSeconds(s);
		MutateToFire();
		StartCoroutine(WaitForFire(Random.Range(4f, 5f)));
	}

	/// <summary> Returns cell neighbors - 4 dir</summary>
	public GameObject[] GetNeighbors(GameObject cell) {
		Vector3Int cellPosition = gridLayout.WorldToCell(cell.transform.position);

		int x = cellPosition.x - topleftCell.x; // convert pos vec3int to correct index in array
		int y = cellPosition.y - bottomleftCell.y;

		// GameObject[] neighbours = new [] {
		// 		(0, 1), (1, 0), (0, -1), (-1, 0)
		// 	}.Select(d => (x + d.Item1, y + d.Item2))
		// 	.Select(pos =>
		// 		pos.Item1 >= 0 && pos.Item1 <= width &&
		// 		pos.Item2 >= 0 && pos.Item2 <= height ?
		// 		cellArray[pos.Item1, pos.Item2] : null
		// 	).Where(go => go != null).ToArray();

		// return dir.ToString().Select(d => Array.IndexOf(new int[] { 'S', 'E', 'W', 'N' }, d))
		// 	.Select(n => n < neighbours.Length ? neighbours[n] : null)
		// 	.Where(n => n != null).ToArray();

		return dir.ToString().Select(d => new [] {
				(0, -1), (1, 0), (-1, 0), (0, 1)
			}[Array.IndexOf(new int[] { 'S', 'E', 'W', 'N' }, d)])
			.Select(pos =>
				pos.Item1 >= 0 && pos.Item1 <= width &&
				pos.Item2 >= 0 && pos.Item2 <= height ?
				cellArray[pos.Item1, pos.Item2] : null
			).Where(go => go != null).ToArray();
		// return neighbours;
	}

	/// <summary> Returns cell radius - outwards 2+ </summary>
	public GameObject[] GetRadius(GameObject cell) {
		Vector3Int cellPosition = gridLayout.WorldToCell(cell.transform.position);

		int x = cellPosition.x - topleftCell.x; // convert pos vec3int to correct index in array
		int y = cellPosition.y - bottomleftCell.y;

		int index = 0;
		int r = 1; // radius

		GameObject[] radius = new GameObject[9]; // if r = 1

		for (int a = -r; a <= r; a++) {
			for (int b = -r; b <= r; b++) {
				if (x + a >= 0 && x + a <= width && y + b <= height && y + b >= 0)
					radius[index] = cellArray[x + a, y + b];
				index++;
			}
		}
		return radius;
	}

	/// <summary> Mutate cell and update ID</summary>
	/// <param name="cell"></param>
	/// <param name="targetID"></param>
	public void MutateCell(GameObject cell, IdentityManager.Identity targetID) {
		cell.GetComponent<IdentityManager>().id = targetID;
	}

	public GameObject getCellObjectAtLoc(Vector3 worldPos) {
		Vector3Int cellLoc = gridLayout.WorldToCell(worldPos);
		int x = cellLoc.x - topleftCell.x; // convert pos vec3int to correct index in array
		int y = cellLoc.y - bottomleftCell.y;
		return cellArray[x, y];
	}

	/// <summary> Returns cell vec3int value given cell array 2d index</summary>
	/// <param name="i"></param>
	/// <param name="j"></param>
	/// <returns></returns>
	public Vector3Int GetVector3IntFromCellArray(int i, int j) {
		return new Vector3Int(topleftCell.x + i, bottomleftCell.y + j, 0);
	}

	public float GetCellIDData(IdentityManager.Identity id) {
		int totalCell = 0;
		int cellWithIDCount = 0;

		foreach (var cell in cellArray) {
			totalCell++;
			if (cell.GetComponent<IdentityManager>().id == id) {
				cellWithIDCount++;
			}
		}

		performance = 100 - 100 * cellWithIDCount / totalCell;
		Debug.Log("player performance is " + performance);
		return performance;
	}

	/// <summary> Tied to backend model</summary>
	public void QueryWorldData() {
		Debug.Log("reservoir size is " + reservoirTotalSize);
		Debug.Log("tree density is " + treeDensity);
		Debug.Log("fire spread / moisture loss rate is " + spreadability);
	}

	public void LoadNewGame() {
		if (performance >= 80) {
			difficulty = 3;
		} else if (performance >= 60) {
			difficulty = 2;
		} else {
			difficulty = 1;
		}
	}
}
