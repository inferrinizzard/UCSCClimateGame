using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Trees : MonoBehaviour
{
	(int, GameObject)[,] grid = new (int, GameObject)[6, 8];
	(int, int)[] initTrees = new (int, int)[] { (0, 4), (1, 1), (2, 3), (1, 5), (3, 0), (3, 7), (4, 2), (4, 5) };

	Vector3Int gridOffset = new Vector3Int(-4, +2, 0);

	public GameObject trees;

	Grid _grid;
	// Start is called before the first frame update
	void Start()
	{
		_grid = GetComponent<Grid>();
		initTrees.ToList().ForEach(x => grid[x.Item1, x.Item2] = (2, PlantTree(x.Item2, -x.Item1)));

		// foreach (Transform t in trees.GetComponentsInChildren<Transform>())
		// 	Debug.Log($"{t.name}: {_grid.WorldToCell(t.position) + gridOffset}");
		//map tree pos to grid
	}

	GameObject RandomTree() => trees.transform.GetChild((int)UnityEngine.Random.Range(0, trees.transform.childCount)).gameObject;

	GameObject PlantTree(int row, int col)
	{
		GameObject newTree = Instantiate(RandomTree(), _grid.CellToWorld(new Vector3Int(row, col, 0) + gridOffset) + _grid.cellSize / 2f, Quaternion.identity);
		newTree.SetActive(true);
		return newTree;
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetButtonDown("Fire1"))
		{
			Vector3Int gridClick = Vector3Int.Scale(_grid.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition)) - gridOffset, new Vector3Int(1, -1, 1));
			Debug.Log(gridClick);
			if (grid[gridClick.x, gridClick.y].Item1 == 2)
			{
				Destroy(grid[gridClick.x, gridClick.y].Item2);
				grid[gridClick.x, gridClick.y] = (0, null);
			}
			else
			{
				grid[gridClick.x, gridClick.y] = (2, PlantTree(gridClick.x, -gridClick.y));
			}
		}

	}
}
