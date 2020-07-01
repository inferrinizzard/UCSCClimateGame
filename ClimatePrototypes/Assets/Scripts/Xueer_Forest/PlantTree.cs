using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;
using UnityEngine.Tilemaps;

// BAD NAMING: this class has unintentionally becoming a god class due to scope keep expanding. T
// Should be refactored
public class PlantTree : MonoBehaviour
{
    public SpriteRenderer plantActionSprite;
    public SpriteRenderer cutActionSprite;
    public GameObject agentSelected;
    public Tilemap tilemap;
    public GameObject AIGoalPrefab;

    public GameObject treeStage1;
    public bool canPlantTree; // flag to track if placing agent or place tree
    public bool canPlantAgent;
    public GameObject agentPlacedPrefab;
    public GameObject agentClickedPrefab;
    public GameObject shadowPrefab;
    private GameObject currentShadow;

    private bool movedAgent = false;
    
    // This map needs to be updated if ground tilemap is changed
    public Dictionary<Vector3Int, bool> groundGridInfo = new Dictionary<Vector3Int, bool>();
    public Dictionary<Vector3Int, bool> gridTreeInfo = new Dictionary<Vector3Int, bool>();  // dic for tree data
    public Dictionary<Vector3Int, GameObject> gridTreePrefab = new Dictionary<Vector3Int, GameObject>();  // dictionary for grabbing tree prefab asset 
    

    void Start()
    {
        agentSelected = null;
        //plantActionSprite.color = Color.clear;
        //cutActionSprite.color = Color.clear;
        
        canPlantTree = true;
        canPlantAgent = false;
        
        // set up grid data structure
        // hard coded for now, using cell position vector3int and the actual tilemap painting.
        for (int i = -4; i <= 3; i++)
        {
            for (int j = -3; j <= 2; j++)
            {
                groundGridInfo.Add(new Vector3Int(i, j, 0),true);
            }
        }
        
        
    }
    
    // change tree VFX
    public void CutTreeAt(Vector3Int loc)
    {
        gridTreePrefab[loc].GetComponent<TreeGrowth>().treeStage = 6;  
        
    }
    
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
            if (!canPlantTree || agentSelected != null)
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
            
            // DO NOT DELETE, useful debug info
            Debug.Log("cell position clicked is:" + cellPosition);
            
            if (!groundGridInfo.ContainsKey(cellPosition)) return;
            
            Vector3 spawnPosition = tilemap.GetCellCenterWorld(cellPosition);
            spawnPosition.z = -1;
            // plant tree  implementation moved to Plant.cs
            if (canPlantTree && plantActionSprite.color == Color.red && agentSelected != null )
            {
                /*if (!gridTreeInfo.ContainsKey(cellPosition))
                {
                    GameObject go = Instantiate(treeStage1, spawnPosition, transform.rotation);
                    gridTreeInfo.Add(cellPosition, true);
                    gridTreePrefab.Add(cellPosition, go);
                    
                    agentSelected.transform.position = spawnPosition;
                }*/
            }
            else  
            {
                // spawn agent the first time
                if (canPlantAgent)
                {
                    GameObject currentGO = Instantiate(agentPlacedPrefab, spawnPosition, transform.rotation);
                    GameObject currentGoal = Instantiate(AIGoalPrefab, spawnPosition, transform.rotation);
                    currentGO.GetComponent<AIDestinationSetter>().target = currentGoal.transform;
                    currentGoal.GetComponent<AIGoalReached>().myAgent = currentGO;
                    agentClickedPrefab.SetActive(false);
                    canPlantAgent = false;
                }
                
                // move selectedAgent to selected target tile
                if (agentSelected != null)
                {
                    Transform currentAgentGoal = agentSelected.GetComponent<AIDestinationSetter>().target;
                    currentAgentGoal.position = spawnPosition;
                    agentSelected.GetComponent<Animator>().SetInteger("animState", 2);
                }
                spawnPosition.z = -2;

                
                Destroy( GameObject.FindWithTag("AgentClicked"));
                canPlantTree = true;
                currentShadow = null;
                Destroy( GameObject.FindWithTag("Shadow"));
                
            }
        }
    }
}
