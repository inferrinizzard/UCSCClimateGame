using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Tilemaps;

public class ForestGrid : MonoBehaviour {
	public static Tilemap map;
	[SerializeField] TileBase[] _trees = default;
	public static TileBase[] trees;
	Vector3Int hoverCell;
	[SerializeField] TileBase hoverTile = default;

	void Awake() {
		trees = _trees;
	}

	void Start() {
		map = GetComponentInChildren<Tilemap>();
		// Debug.Log(map.cellBounds); //boundsInt
	}

	void Update() {
		if (ForestController.Instance.hasSelected) {
			Vector3Int newHover = map.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
			if (map.cellBounds.Contains(newHover)) {
				if (newHover != hoverCell) {
					map.SetTile(new Vector3Int(hoverCell.x, hoverCell.y, 1), null);
					map.SetColor(new Vector3Int(hoverCell.x, hoverCell.y, 1), Color.white);
					map.SetTile(new Vector3Int(newHover.x, newHover.y, 1), hoverTile);
					map.SetColor(new Vector3Int(newHover.x, newHover.y, 1), new Color(1, 1, 1, .3f));
				}
			} else {
				map.SetTile(new Vector3Int(hoverCell.x, hoverCell.y, 1), null);
				map.SetColor(new Vector3Int(hoverCell.x, hoverCell.y, 1), Color.white);
			}
			hoverCell = newHover;

			if (Input.GetMouseButtonDown(0) && !ForestController.Instance.activeTiles.Contains(hoverCell)) {
				ForestController.Instance.SetTarget(hoverCell, VolunteerActions.Plant);
			}
		}

	}
}
