using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class SubtropicsWorld : MonoBehaviour {
	public GameObject cloudPrefab, cellPrefab, waterPrefab;
	public Sprite[] reservoirSprites;

	[HideInInspector] public GameObject[] reservoirs = new GameObject[2];

	[Space, Header("Serialized Fields")]

	/// <summary>
	/// Backend data
	/// according to player performance, there are three difficulty
	/// </summary>
	[SerializeField] int reservoirTotalSize;
	[SerializeField] int spreadability = 3;
	[Range(0, 100)] public int treeDensity;

	GridLayout gridLayout;

	public Vector3Int topLeftCell = new Vector3Int(-36, 16, 0);
	public Vector3Int topRightCell = new Vector3Int(32, 16, 0);
	public Vector3Int bottomLeftCell = new Vector3Int(-36, -20, 0);
	public Vector3Int bottomRightCell = new Vector3Int(32, -20, 0);

	Tilemap tilemap;

	public GameObject[, ] cellArray; // central cell data structure
	[SerializeField] int width, height;

	int cloudNum = 10; // have 10 at all times, 5 will be visible on screen
	GameObject[] clouds;

	void Start() {
		clouds = new GameObject[cloudNum];
		tilemap = transform.GetComponent<Tilemap>();
		gridLayout = this.GetComponentInParent<GridLayout>();
		PopulateVanillaWorld();
		PopulateWater();
		PopulateTree();
		PopulateCloud();
		StartCoroutine(WaitForFire(0)); // first fire mutation
	}

	void PopulateCloud() {
		GameObject cloudParent = new GameObject("Clouds");
		var bottomBound = gridLayout.CellToWorld(bottomLeftCell);
		var topBound = gridLayout.CellToWorld(topRightCell);
		var centre = (bottomBound + topBound) / 2;
		var dist = (topBound - bottomBound) / 2;

		for (int i = 0; i < cloudNum; i++) {
			clouds[i] = Instantiate(cloudPrefab, new Vector3(Random.Range(-dist.x, dist.x), Random.Range(-dist.y, dist.y), 0) + centre, Quaternion.identity);
			clouds[i].transform.parent = cloudParent.transform;
		}
	}

	void PopulateVanillaWorld() {
		GameObject cellParent = new GameObject("Cells");

		// get corner positions of the world 
		width = topRightCell.x - topLeftCell.x + 1;
		height = topLeftCell.y - bottomLeftCell.y + 1;
		cellArray = new GameObject[width + 1, height + 1];

		for (var i = 0; i <= width; i++) {
			for (var j = 0; j <= height; j++) {
				Vector3Int posInt = new Vector3Int(topLeftCell.x + i, bottomLeftCell.y + j, 0);
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
	void PopulateWater() {
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
			if (SubtropicsController.Instance.difficulty == 1) {
				waterHeight *= 2;
				waterWidth *= 2;
			}

			for (int i = waterX; i < waterX + waterWidth; i++)
				for (int j = waterY; j < waterY + waterHeight; j++)
					cellArray[i, j].GetComponent<IdentityManager>().id = IdentityManager.Identity.Water;

			reservoirTotalSize += waterHeight * waterWidth;

			Vector3 pos1 = tilemap.GetCellCenterWorld(GetVector3IntFromCellArray(waterX, waterY));
			Vector3 pos2 = tilemap.GetCellCenterWorld(GetVector3IntFromCellArray(waterX + waterWidth, waterY + waterHeight));
			Vector3 reservoirPos = (pos1 + pos2) / 2 + new Vector3(-0.25f, -0.25f, 0);
			GameObject water = Instantiate(waterPrefab, reservoirPos, Quaternion.identity);
			water.SetActive(true);

			water.GetComponent<SpriteRenderer>().sprite = reservoirSprites[reservoirGen];
			water.GetComponent<SpriteRenderer>().size = new Vector2(waterWidth / 4, waterHeight / 4);
			water.transform.localScale = new Vector3(0.4f, 0.4f, 1) * (SubtropicsController.Instance.difficulty == 1 ? 2 : 1);

			reservoirs[r] = water;
			SubtropicsController.Instance.player.GetComponentsInChildren<WaterArrow>() [r].waterPosition = water.transform.position;
		}
	}

	void PopulateTree() {
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
				IdentityManager I = cellArray[i, j].GetComponent<IdentityManager>();
				if (I.id == IdentityManager.Identity.Green) {
					if (Random.value * 100 < treeDensity) {
						I.id = IdentityManager.Identity.Tree;
						I.fireVariance = 1; // if fire happens, set variance type to tree fire 
					}
				}
			}
		}
	}

	/// <summary> Mutates grass to fire</summary>
	void MutateToFire() {
		// pick a random cell
		// if green, mutates to fire
		IdentityManager I = cellArray[Random.Range(0, width), Random.Range(0, height)].GetComponent<IdentityManager>();
		//if (I.id is IdentityManager.Identity.Green || I.id is IdentityManager.Identity.Tree )  // can mutate green or tree
		if (I.id is IdentityManager.Identity.Tree) {
			//Debug.Log("fire id" + I.id);
			I.id = IdentityManager.Identity.Fire;
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

		int x = cellPosition.x - topLeftCell.x; // convert pos vec3int to correct index in array
		int y = cellPosition.y - bottomLeftCell.y;

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

		return SubtropicsController.Instance.wind.dir.ToString().Select(d => (
				new Dictionary<char, Vector2Int> { { 'S', Vector2Int.down },
					{ 'N', Vector2Int.up },
					{ 'W', Vector2Int.left },
					{ 'E', Vector2Int.right }
				}
			) [d])
			.Select(pos =>
				pos.x >= 0 && pos.x <= width &&
				pos.y >= 0 && pos.y <= height ?
				cellArray[pos.x, pos.y] : null
			).Where(go => go != null).ToArray();
		// return neighbours;
	}

	/// <summary> Returns cell radius - outwards 2+ </summary>
	public List<IdentityManager> GetRadius(Vector3 pos) {
		Vector3Int cellPosition = gridLayout.WorldToCell(pos);

		int x = cellPosition.x - topLeftCell.x; // convert pos vec3int to correct index in array
		int y = cellPosition.y - bottomLeftCell.y;

		int r = 1; // radius

		List<IdentityManager> area = new List<IdentityManager>();

		for (int a = -r; a <= r; a++)
			for (int b = -r; b <= r; b++)
				if (x + a >= 0 && x + a <= width && y + b <= height && y + b >= 0)
					area.Add(cellArray[x + a, y + b].GetComponent<IdentityManager>());
		return area;
	}

	public GameObject GetCell(Vector3 worldPos) {
		Vector3Int cellLoc = gridLayout.WorldToCell(worldPos);
		int x = cellLoc.x - topLeftCell.x; // convert pos vec3int to correct index in array
		int y = cellLoc.y - bottomLeftCell.y;
		return cellArray[x, y];
	}

	/// <summary> Returns cell vec3int value given cell array 2d index</summary>
	/// <param name="i"></param>
	/// <param name="j"></param>
	/// <returns></returns>
	public Vector3Int GetVector3IntFromCellArray(int i, int j) {
		return new Vector3Int(topLeftCell.x + i, bottomLeftCell.y + j, 0);
	}

	/// <summary> Tied to backend model</summary>
	public void QueryWorldData() {
		Debug.Log("reservoir size is " + reservoirTotalSize);
		Debug.Log("tree density is " + treeDensity);
		Debug.Log("fire spread / moisture loss rate is " + spreadability);
	}

	// public void LoadNewGame() {
	// 	if (performance >= 80) {
	// 		difficulty = 3;
	// 	} else if (performance >= 60) {
	// 		difficulty = 2;
	// 	} else {
	// 		difficulty = 1;
	// 	}
	// }
}
