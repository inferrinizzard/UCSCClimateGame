using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Tilemaps;

public class ForestGrid : MonoBehaviour {
	Tilemap map;
	Vector3Int hoverCell;
	[SerializeField] TileBase hoverTile;

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
					map.SetTile(new Vector3Int(newHover.x, newHover.y, 1), hoverTile);
				}
			} else {
				map.SetTile(new Vector3Int(hoverCell.x, hoverCell.y, 1), null);
			}
			hoverCell = newHover;

			if (Input.GetMouseButtonDown(0)) {
				ForestController.Instance.SetTarget(hoverCell);
			}
		}

	}
}
