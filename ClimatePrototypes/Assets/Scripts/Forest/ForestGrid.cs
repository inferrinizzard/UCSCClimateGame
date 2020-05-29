using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Tilemaps;

public class ForestGrid : MonoBehaviour {
	public static Tilemap map;
	[SerializeField] TileBase[] _trees = default;
	public static TileBase[] trees;
	public static TileBase stump { get => trees[0]; }
	public static TileBase dead { get => trees[1]; }
	public static TileBase empty { get => trees[2]; }
	public static TileBase sprout { get => trees[3]; }
	Vector3Int hoverCell;
	[SerializeField] TileBase hoverTile = default;
	public static List<ForestTree> currentTrees = new List<ForestTree>();
	public static float growthTime = 5;

	void Awake() {
		trees = _trees;
	}

	void Start() {
		map = GetComponentInChildren<Tilemap>();
		// Debug.Log(map.cellBounds); //boundsInt

		for (var(i, max) = (0, (int) map.size.x * map.size.y / 2 * 1); i < max; i++) {
			var randomPos = (Vector3) (map.cellBounds.max - map.cellBounds.min);
			randomPos.Scale(new Vector3(Random.value, Random.value, Random.value));
			var randomPosInt = Vector3Int.FloorToInt(randomPos) + map.cellBounds.min;
			if (currentTrees.Any(tree => tree.pos == randomPosInt))
				continue;
			currentTrees.Add(new ForestTree(randomPosInt, _trees[Random.Range(2, 6)]));
		}
	}

	void Update() {
		if (ForestController.Instance.hasSelected) {
			Vector3Int newHover = map.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
			if (map.cellBounds.Contains(newHover)) {
				if (newHover != hoverCell) {
					ClearHover(hoverCell);
					map.SetTile(new Vector3Int(newHover.x, newHover.y, 1), hoverTile);
					map.SetColor(new Vector3Int(newHover.x, newHover.y, 1), new Color(1, 1, 1, .3f));
				}
			} else {
				ClearHover(hoverCell);
			}
			hoverCell = newHover;

			if (Input.GetMouseButtonDown(0) && map.cellBounds.Contains(hoverCell) && !ForestController.Instance.activeTiles.Contains(hoverCell)) {
				if (map.GetTile(hoverCell) == empty)
					ForestController.Instance.SetVolunteerTarget(hoverCell, VolunteerActions.Plant);
				else if (map.GetTile(hoverCell) == stump || map.GetTile(hoverCell) == dead)
					ForestController.Instance.SetVolunteerTarget(hoverCell, VolunteerActions.Clear);
			}
		}
	}

	public static void ClearHover(Vector3Int cell) {
		map.SetTile(new Vector3Int(cell.x, cell.y, 1), null);
		map.SetColor(new Vector3Int(cell.x, cell.y, 1), Color.white);
	}
}

public class ForestTree { // TODO: do these get cleared?
	TileBase _tile;
	TileBase tile {
		get => _tile;
		set {
			_tile = value;
			ForestGrid.map.SetTile(pos, _tile);
			index = ForestGrid.trees.ToList().IndexOf(_tile);
		}
	}
	// TileBase tile { get=>ForestGrid.map.GetTile(pos); set=>ForestGrid.map.SetTile(pos, value);} 
	public Vector3Int pos;
	int index = -1;
	public bool alive { get => index > 1; }

	public ForestTree(Vector3Int pos, TileBase tile = null) {
		this.tile = tile ?? ForestGrid.sprout;
		// Debug.Log(this.tile);
		this.pos = pos;
		ForestController.Instance.StartCoroutine(Grow(0));
		NeighbourCount();
	}

	IEnumerator Grow(float time) {
		yield return new WaitForSeconds(time);
		if (index == ForestGrid.trees.Length - 1)
			tile = ForestGrid.dead;
		else {
			float awaitTime = (ForestGrid.growthTime + (Random.value - .25f) * 2) * (1 + .5f * NeighbourCount() / 4f);
			if (index == 4) {
				ForestController.Instance.activeTrees.Add(pos);
				awaitTime += index / 4f;
			}
			tile = ForestGrid.trees[index + 1];
			ForestController.Instance.StartCoroutine(Grow(awaitTime));
		}
	}

	float NeighbourCount() {
		int count = 0;
		bool IsTree(TileBase tile) => tile != null && tile != ForestGrid.dead && tile != ForestGrid.stump && tile != ForestGrid.empty;
		if (IsTree(ForestGrid.map.GetTile(pos + Vector3Int.up)))
			count++;
		if (IsTree(ForestGrid.map.GetTile(pos + Vector3Int.left)))
			count++;
		if (IsTree(ForestGrid.map.GetTile(pos + Vector3Int.right)))
			count++;
		if (IsTree(ForestGrid.map.GetTile(pos + Vector3Int.down)))
			count++;
		return count;
	}
}
