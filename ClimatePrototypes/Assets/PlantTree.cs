using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlantTree : MonoBehaviour
{
    public Tilemap tilemap;

    public GameObject treeStage1;
    
    public Dictionary<Vector3Int, bool> gridTreeInfo = new Dictionary<Vector3Int, bool>();
    // Start is called before the first frame update
    void Start()
    {
        
        
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Vector3 clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPosition = tilemap.WorldToCell(clickPosition);
            if (!gridTreeInfo.ContainsKey(cellPosition))
            {
                gridTreeInfo.Add(cellPosition, true);
                Vector3 spawnPosition = tilemap.GetCellCenterWorld(cellPosition);
                spawnPosition.z = -1;
                Instantiate(treeStage1, spawnPosition, transform.rotation);
            }
            
        }
    }
}
