using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Boo.Lang;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class PopulateWorld : MonoBehaviour
{
    [Header("References")] public TextMeshProUGUI windDirText;
    public GameObject cloudPrefab;
    public GameObject cellPrefab;
    public GameObject waterPrefab;
    public GameObject playerPrefab;
    
    public GameObject watergo;
    
    [Range(0, 100)] public int treeDensity;
    public enum windDir 
    {
        Calm,
        NE,
        NW,
        SE,
        SW
    }
    public windDir dir = windDir.Calm;
    
    [Space]
    
    [Header("SerializedFields")]
    
    private static PopulateWorld _instance;
    private GameObject player;
    
    public static PopulateWorld Instance { get { return _instance; } }

    private GridLayout gridLayout;
    
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }
    
    public Vector3Int fire1 = new Vector3Int(0, 0, 0);
    public Vector3Int water1 = new Vector3Int(6, 2, 0);
    public Vector3Int water2 = new Vector3Int(6, 3, 0);
    public Vector3Int water3 = new Vector3Int(6, 1, 0);
    
    /*Vector3Int topleftCell = new Vector3Int(-18, 8, 0);
    Vector3Int topRightCell = new Vector3Int(16, 8, 0);
    Vector3Int bottomleftCell = new Vector3Int(-18, -10, 0);
    Vector3Int bottomrightCell = new Vector3Int(16, -10, 0);*/
    
    public Vector3Int topleftCell = new Vector3Int(-36, 16, 0);
    public Vector3Int topRightCell = new Vector3Int(32, 16, 0);
    public Vector3Int bottomleftCell = new Vector3Int(-36, -20, 0);
    public Vector3Int bottomrightCell = new Vector3Int(32, -20, 0);
    
    private Tilemap tilemap;
    private float udpateWindTimer;
    
    
    private GameObject[,] cellArray;  // central cell data structure
    [SerializeField] private int width;
    [SerializeField] private int height;
    
    private int cloudNum = 10; // have 10 at all times, 5 will be visible on screen
    private GameObject[] clouds;
    private bool waiting;
    // Start is called before the first frame update
    void Start()
    {
        clouds = new GameObject[cloudNum];
        tilemap = transform.GetComponent<Tilemap>();
        gridLayout = transform.parent.GetComponentInParent<GridLayout>();
        PopulatePlayer();
        PopulateVanillaWorld();
        PopulateWater();
        PopulateTree();
        PopulateCloud();
        StartCoroutine(WaitForFire(0));  // first fire mutation
    }

    void UpdateWindDirection()
    {
        udpateWindTimer += Time.deltaTime;
        if (udpateWindTimer >= 30)  // change wind dir every 30 sec
        {
            udpateWindTimer = 0;
            int seed = Random.Range(0, 4);
            switch (seed)
            {
                case 0:
                    dir = windDir.Calm;
                    break;
                case 1:
                    dir = windDir.NE;
                    break;
                case 2:
                    dir = windDir.NW;
                    break;
                case 3:
                    dir = windDir.SE;
                    break;
                case 4:
                    dir = windDir.SW;
                    break;
                default:
                    dir = windDir.Calm;
                    break;

            }
        }
    }

    void PopulateCloud()
    {
        GameObject cloudParent = new GameObject();
        cloudParent.name = "Clouds";
        
        Vector3 topLeftCloudPos = new Vector3(-16, 6, 0);
        Vector3 topRightCloudPos = new Vector3(14, 6, 0);
        Vector3 bottomRightCloudPos = new Vector3(14, -8, 0);

        float x = (topRightCloudPos.x - topLeftCloudPos.x) / (cloudNum - 1);
        float y = (topRightCloudPos.y - bottomRightCloudPos.y);
        
        
        for (int i = 0; i < cloudNum; i++)
        {
            //Debug.Log("clouds");
            clouds[i] = Instantiate(cloudPrefab, new Vector3(topLeftCloudPos.x + i * x, Random.Range(topRightCloudPos.y, topRightCloudPos.y + 10) ,0), Quaternion.identity);
            clouds[i].transform.parent = cloudParent.transform;
        }
    }
    

    private void Update()
    {
        GUIUpdate();
        UpdateWindDirection();
        
        if (!waiting)
        {
            // randomly start fire
            waiting = true;
            float timer = UnityEngine.Random.Range(4.0f, 5.0f);
            StartCoroutine(WaitForFire(timer));

        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            RePopulateWorld();
        }
    }

    void GUIUpdate()
    {
        windDirText.text = dir.ToString();
    }
    
    private void PopulatePlayer()
    {
        player = Instantiate(playerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        player.SetActive(true);
    }
    private void PopulateVanillaWorld()
    {
        GameObject cellParent = new GameObject();
        cellParent.name = "Cells"; 
        
        // get corner positions of the world 

        width = topRightCell.x - topleftCell.x + 1;
        height = topleftCell.y - bottomleftCell.y + 1;
        cellArray = new GameObject[width+1, height+1];

        for (var i = 0; i <= width; i++)
        {
            for (var j = 0; j <= height; j++)
            {
                Vector3Int posInt = new Vector3Int(topleftCell.x + i, bottomleftCell.y + j, 0);
                Vector3 pos = tilemap.GetCellCenterWorld(posInt);
                // instantiate and construct 2d array
                GameObject go = Instantiate(cellPrefab, pos, Quaternion.identity);
                go.transform.parent = cellParent.transform;
                IdentityManager goID = go.GetComponent<IdentityManager>();
                goID.id = IdentityManager.Identity.Green;  // default to green
                cellArray[i, j] = go;
            }
            
        }
    }
    
    
    /// <summary>
    /// Create water cells
    /// VFX: generate a water reservoir prefab in the correct ratio
    /// </summary>
    private void PopulateWater()
    {
        // TODO: water size according to precipitation
        // currently art asset is 1:2, here size is 3:6
        int waterX = 6;  // upper left corner location in array
        int waterY = 5;
        int waterWidth = 6;
        int waterHeight = 3;
        for (int i = waterX; i < waterX + waterWidth; i++)
        {
            for (int j = waterY; j < waterY + waterHeight; j++)
            {
                GameObject go = cellArray[i, j];
                IdentityManager goID = go.GetComponent<IdentityManager>();
                goID.id = IdentityManager.Identity.Water;
            }
        }
        //Vector3Int cornerPos = new Vector3Int(waterX, waterY, 0);
        Vector3Int cornerPos = GetVector3IntFromCellArray(waterX, waterY);
        Vector3 pos1 = tilemap.GetCellCenterWorld(cornerPos);
        Vector3 pos2 = tilemap.GetCellCenterWorld(new Vector3Int(cornerPos.x + 1, cornerPos.y + 1, 0));
        float unitWidth = pos2.x - pos1.x;
        Vector3 reservoirPos = new Vector3(pos1.x + (waterWidth / 2 - 0.25f) * unitWidth, pos1.y + (waterHeight / 2 + 0f) * unitWidth,0);
        watergo = Instantiate(waterPrefab, reservoirPos, Quaternion.identity);
        watergo.SetActive(true);

    }
    
    

    private void PopulateTree()
    {
        // hand authored three regions of mountain areas filled with trees
        // Region1   left top
        int width1 = Random.Range(width / 4, width / 3);  // right
        int height1 = Random.Range(height /3, height / 2);   // bottom
        
        // Region2    right
        int width2_0 = Random.Range(width / 3 + 4, width / 2);   // left
        int width2_1 = width;   // right
        int height2_0 = Random.Range(height / 6, height / 5);  // bottom
        int height2_1 = height *2 / 3;  // top 
        
        // Region3     bottom
        int width3_0 = Random.Range(width /9, width / 6);  // left
        int width3_1 = Random.Range(width * 8/9, width *9/ 10);  // right
        int height3_0 = Random.Range(height * 2 / 3 + 4, height * 4 / 5); // top
        int height3_1 = height;  // bottom
        
        GenerateTreeInRegion(0,width1, 0, height1);
        GenerateTreeInRegion(width2_0,width2_1, height2_0, height2_1);
        GenerateTreeInRegion(width3_0,width3_1, height3_0, height3_1);
        
        
        
    }

    void GenerateTreeInRegion(int w0, int w1, int h0, int h1)
    {
        for (var i = w0; i <= w1; i++)
        {
            for (var j = h0; j <= h1; j++)
            {
                GameObject go = cellArray[i, j];
                if (go.GetComponent<IdentityManager>().id == IdentityManager.Identity.Green)
                {
                    int seed = Random.Range(0, 100);
                    if (seed < treeDensity)
                    {
                        go.GetComponent<IdentityManager>().id = IdentityManager.Identity.Tree;
                        go.GetComponent<IdentityManager>().SetFireVariance(1);  // if fire happens, set variance type to tree fire 
                    }
                }
            }
        }
        
    }
    
    /// <summary>
    /// Mutates grass to fire
    /// </summary>
    void MutateToFire()
    {
        // pick a random cell
        // if green, mutates to fire
        int randomX = Random.Range(0, width);
        int randomY = Random.Range(0, height);
        GameObject go = cellArray[randomX, randomY];
        IdentityManager goID = go.GetComponent<IdentityManager>();
        //if (goID.id is IdentityManager.Identity.Green || goID.id is IdentityManager.Identity.Tree )  // can mutate green or tree
        if (goID.id is IdentityManager.Identity.Tree )
        {
            //Debug.Log("fire id" + goID.id);
            goID.id = IdentityManager.Identity.Fire;
        }
    }
    
    IEnumerator WaitForFire(float s)
    {
        yield return new WaitForSeconds(s);
        MutateToFire();
        waiting = false;
    }
    
    /// <summary>
    /// Returns cell neighbors - 4 dir
    /// </summary>
    public GameObject[] GetNeighbors(GameObject cell)
    {
        Vector3Int cellPosition = gridLayout.WorldToCell(cell.transform.position);

        int x = cellPosition.x - topleftCell.x;  // convert pos vec3int to correct index in array
        int y = cellPosition.y - bottomleftCell.y;

        GameObject[] neighbors = new GameObject[4];
        if (x >= 0 && x <= width && y >= 0 && y < height)
        {
         neighbors[0] = (cellArray[x, y + 1]); // up
         
        }

        if(x >= 0 && x <= width && y <= height && y > 0) neighbors[1] =(cellArray[x, y - 1]);  // left
        if(x < width && x >= 0 && y <= height && y >= 0) neighbors[2] =(cellArray[x + 1, y]);  // right
        if(x > -0 && x <= width && y <= height && y >= 0) neighbors[3] =(cellArray[x - 1, y]); // down
        
        GameObject[] neighborsDir = new GameObject[2];
        if (dir == windDir.Calm)  // no dir
        {
            return neighbors;
        }
        else if (dir == windDir.SW)  // up right
        {
            neighborsDir[0] = neighbors[0];
            neighborsDir[1] = neighbors[2];
            return neighborsDir;
        }
        else if (dir == windDir.SE)  // up left
        {
            neighborsDir[0] = neighbors[0];
            neighborsDir[1] = neighbors[1];
            return neighborsDir;
        }
        else if (dir == windDir.NW)  // down right
        {
            neighborsDir[0] = neighbors[3];
            neighborsDir[1] = neighbors[2];
            return neighborsDir;
        }
        else  // up right
        {
            neighborsDir[0] = neighbors[3];
            neighborsDir[1] = neighbors[1];
            return neighborsDir;
        }
        
        //return neighbors;
    }
    
    /// <summary>
    /// Returns cell radius - outwards 2+ 
    /// </summary>
    public GameObject[] GetRadius(GameObject cell)
    {
        Vector3Int cellPosition = gridLayout.WorldToCell(cell.transform.position);

        int x = cellPosition.x - topleftCell.x;  // convert pos vec3int to correct index in array
        int y = cellPosition.y - bottomleftCell.y;


        int index = 0;
        int r = 1;  // radius
        
        GameObject[] radius = new GameObject[9];  // if r = 1

        for (int a = -r; a <= r; a++)
        {
            for (int b = -r; b <= r; b++)
            {
                if( x+a >= 0 && x+a <= width && y+b <=height && y+b >= 0 )
                    radius[index] = cellArray[x + a, y + b];
                index++;
            }
        }
        return radius;
    }
    
    /// <summary>
    /// Mutate cell and update ID
    /// </summary>
    /// <param name="cell"></param>
    /// <param name="targetID"></param>
    public void MutateCell(GameObject cell, IdentityManager.Identity targetID)
    {
        cell.GetComponent<IdentityManager>().id = targetID;
    }

    public GameObject getCellObjectAtLoc(Vector3 worldPos)
    {
        Vector3Int cellLoc = gridLayout.WorldToCell(worldPos);
        int x = cellLoc.x - topleftCell.x;  // convert pos vec3int to correct index in array
        int y = cellLoc.y - bottomleftCell.y;
        return cellArray[x, y];

    }
    
    /// <summary>
    /// Returns cell vec3int value given cell array 2d index
    /// </summary>
    /// <param name="i"></param>
    /// <param name="j"></param>
    /// <returns></returns>
    public Vector3Int GetVector3IntFromCellArray(int i, int j)
    {
        return new Vector3Int(topleftCell.x + i, bottomleftCell.y + j, 0);
    }


    private void RePopulateWorld()
    {
        Destroy(player);
        for (var i = 0; i <= width; i++)
        {
            for (var j = 0; j <= height; j++)
            {
                Destroy(cellArray[i, j]);
            }
            
        }
        PopulateVanillaWorld();
        PopulateWater();
        PopulateTree();
        PopulatePlayer();
        StartCoroutine(WaitForFire(0));
        
    }
    
    
}
