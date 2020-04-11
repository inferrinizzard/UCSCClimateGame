using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlantTree : MonoBehaviour
{
    public SpriteRenderer plantActionSprite;
    public SpriteRenderer cutActionSprite;
    public GameObject agentSelected;
    public Tilemap tilemap;

    public GameObject treeStage1;
    public bool canPlantTree; // flag to track if placing agent or place tree
    public bool canPlantAgent;
    public GameObject agentPlacedPrefab;
    public GameObject shadowPrefab;
    private GameObject currentShadow;

    private bool movedAgent = false;
    
    // This map needs to be updated if ground tilemap is changed
    public Dictionary<Vector3Int, bool> groundGridInfo = new Dictionary<Vector3Int, bool>();
    //public Dictionary<GameObject, bool> agentsInfo = new Dictionary<GameObject, bool>();
    
    public Dictionary<Vector3Int, bool> gridTreeInfo = new Dictionary<Vector3Int, bool>();
    public Dictionary<Vector3Int, GameObject> gridTreePrefab = new Dictionary<Vector3Int, GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        agentSelected = null;
        plantActionSprite.color = Color.clear;
        cutActionSprite.color = Color.clear;
        
        canPlantTree = true;
        canPlantAgent = false;
        
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
            if (canPlantTree && plantActionSprite.color == Color.red && agentSelected != null )
            {
                if (!gridTreeInfo.ContainsKey(cellPosition))
                {
                    GameObject go = Instantiate(treeStage1, spawnPosition, transform.rotation);
                    gridTreeInfo.Add(cellPosition, true);
                    gridTreePrefab.Add(cellPosition, go);
                    
                    agentSelected.transform.position = spawnPosition;
                }

                
            }
            else  
            {
                // spawn agent the first time
                if (canPlantAgent)
                {
                    Instantiate(agentPlacedPrefab, spawnPosition, transform.rotation);
                    canPlantAgent = false;
                }
                
                // cut down current tile tree
                if (gridTreeInfo.ContainsKey(cellPosition) && cutActionSprite.color == Color.red && agentSelected != null)
                {
                    gridTreePrefab[cellPosition].GetComponent<TreeGrowth>().treeStage = 5;
                    gridTreePrefab[cellPosition].GetComponent<TreeGrowth>().UpdateTreeVFX(5);
                    
                    // move action agent to the tile
                    // TODO: coroutine to move agent overtime
                    //if (!movedAgent) StartCoroutine(MoveAgentToTile(spawnPosition));
                    agentSelected.transform.position = spawnPosition;
                    
                }
                // draw tree beneath agent
                spawnPosition.z = -2;

                
                Destroy( GameObject.FindWithTag("AgentClicked"));
                canPlantTree = true;
                currentShadow = null;
                Destroy( GameObject.FindWithTag("Shadow"));
                
                

            }
        }
        
        if (agentSelected != null)
        {
            // use plant action
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                plantActionSprite.color = Color.red;
                cutActionSprite.color = new Color(255,255,255);
            } // use cut action
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                cutActionSprite.color = Color.red;
                plantActionSprite.color = new Color(255,255,255);
            }
        }
    }

    /*IEnumerator MoveAgentToTile(GameObject agent, Vector3 destination)
    {
        Vector3 dir = destination - agent.transform.position;
        agent.transform.position += dir.normalized * Time.deltaTime;

        if (destination == agent.transform.position)
        {
            movedAgent = true;
        }
    }*/
}
