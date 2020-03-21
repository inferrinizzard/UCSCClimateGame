using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlantTree : MonoBehaviour
{
    public Tilemap tilemap;

    public GameObject treeStage1;
    public bool canPlantTree; // flag to track if placing agent or place tree
    public GameObject agentPlacedPrefab;
    public GameObject shadowPrefab;
    private GameObject currentShadow;
    
    // This map needs to be updated if ground tilemap is changed
    public Dictionary<Vector3Int, bool> groundGridInfo = new Dictionary<Vector3Int, bool>();
    
    public Dictionary<Vector3Int, bool> gridTreeInfo = new Dictionary<Vector3Int, bool>();
    public Dictionary<Vector3Int, GameObject> gridTreePrefab = new Dictionary<Vector3Int, GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        canPlantTree = true;
        
        // set up grid data structure
        for (int i = -4; i <= 3; i++)
        {
            for (int j = -3; j <= 2; j++)
            {
                groundGridInfo.Add(new Vector3Int(i, j, 0),true);
            }
        }
        
        
    }

    // Update is called once per frame
    void Update()
    {
        // Hightlight tiles
        Vector3 mousePositionWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPositionHover = tilemap.WorldToCell(mousePositionWorld);
        
        Vector3 spawnHoverPosition = tilemap.GetCellCenterWorld(cellPositionHover);
        spawnHoverPosition.z = -1;
        
        if (groundGridInfo.ContainsKey(cellPositionHover))
        {
            // Dangerous, changing tile directly
            /*Tile hoverTile = tilemap.GetTile<Tile>(cellPositionHover);
            Debug.Log(hoverTile);
            hoverTile.color = Color.green;*/
            if (canPlantTree)
            {
                
            }
            else
            {
                // place shadow
                if (currentShadow is null)
                {
                    currentShadow = Instantiate(shadowPrefab, spawnHoverPosition, transform.rotation);
                }
                else
                {
                    currentShadow.transform.position = spawnHoverPosition;
                }
            }
            
        }

        
        if (Input.GetButtonDown("Fire1"))
        {
            Vector3 clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPosition = tilemap.WorldToCell(clickPosition);
            
            //Debug.Log("cell position clicked is:" + cellPosition);
            if (!groundGridInfo.ContainsKey(cellPosition)) return;
            
            Vector3 spawnPosition = tilemap.GetCellCenterWorld(cellPosition);
            spawnPosition.z = -1;
            // plant tree
            if (canPlantTree)
            {
                if (!gridTreeInfo.ContainsKey(cellPosition))
                {
                    GameObject go = Instantiate(treeStage1, spawnPosition, transform.rotation);
                    gridTreeInfo.Add(cellPosition, true);
                    gridTreePrefab.Add(cellPosition, go);
                    
                    
                }
            }
            else  // order agent
            {
                // cut down current tile tree
                if (gridTreeInfo.ContainsKey(cellPosition))
                {
                    gridTreePrefab[cellPosition].GetComponent<TreeGrowth>().treeStage = 5;
                    gridTreePrefab[cellPosition].GetComponent<TreeGrowth>().UpdateTreeVFX(5);
                }
                // draw tree beneath agent
                spawnPosition.z = -2;
                Instantiate(agentPlacedPrefab, spawnPosition, transform.rotation);
                Destroy( GameObject.FindWithTag("AgentClicked"));
                canPlantTree = true;
                currentShadow = null;
                Destroy( GameObject.FindWithTag("Shadow"));
                
                

            }
            
            
        }
    }
}
