using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Boo.Lang;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class PopulateWorld : MonoBehaviour
{
    private static PopulateWorld _instance;
    
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
    
    Vector3Int topleftCell = new Vector3Int(-18, 8, 0);
    Vector3Int topRightCell = new Vector3Int(16, 8, 0);
    Vector3Int bottomleftCell = new Vector3Int(-18, -10, 0);
        
    Vector3Int bottomrightCell = new Vector3Int(16, -10, 0);
    
    private Tilemap tilemap;

    public GameObject cellPrefab;
    
    private GameObject[,] cellArray;  // central cell data structure
    [SerializeField] private int width;
    [SerializeField] private int height;

    private bool waiting;
    // Start is called before the first frame update
    void Start()
    {
        tilemap = transform.GetComponent<Tilemap>();
        gridLayout = transform.parent.GetComponentInParent<GridLayout>();
        PopulateVanillaWorld();
        PopulateWater();
        StartCoroutine(WaitForFire(0));  // first fire mutation
    }

    private void Update()
    {
        if (!waiting)
        {
            // randomly start fire
            waiting = true;
            float timer = UnityEngine.Random.Range(4.0f, 5.0f);
            StartCoroutine(WaitForFire(timer));

        }
    }


    private void PopulateVanillaWorld()
    {
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
                IdentityManager goID = go.GetComponent<IdentityManager>();
                goID.id = IdentityManager.Identity.Green;  // default to green
                cellArray[i, j] = go;
            }
            
        }


    }

    private void PopulateWater()
    {
        // TODO: water size according to precipitation
        int waterX = 6;
        int waterY = 5;
        for (int i = waterX; i <= waterX + 4; i++)
        {
            for (int j = waterY; j <= waterY + 3; j++)
            {
                GameObject go = cellArray[i, j];
                IdentityManager goID = go.GetComponent<IdentityManager>();
                goID.id = IdentityManager.Identity.Water;
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
        if (goID.id is IdentityManager.Identity.Green)
        {
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
         neighbors[0] = (cellArray[x, y + 1]);
         
        }

        if(x >= 0 && x <= width && y <= height && y > 0) neighbors[1] =(cellArray[x, y - 1]);
        if(x < width && x >= 0 && y <= height && y >= 0) neighbors[2] =(cellArray[x + 1, y]);
        if(x > -0 && x <= width && y <= height && y >= 0) neighbors[3] =(cellArray[x - 1, y]);

        return neighbors;
    }
    
    /// <summary>
    /// Returns cell neighbors - 8 dir + self
    /// </summary>
    public GameObject[] GetRadius(GameObject cell)
    {
        Vector3Int cellPosition = gridLayout.WorldToCell(cell.transform.position);

        int x = cellPosition.x - topleftCell.x;  // convert pos vec3int to correct index in array
        int y = cellPosition.y - bottomleftCell.y;

        GameObject[] radius = new GameObject[9];
        if (x >= 0 && x <= width && y >= 0 && y < height)
        {
            radius[0] = (cellArray[x, y + 1]);
         
        }

        if(x >= 0 && x <= width && y <= height && y > 0) radius[1] =(cellArray[x, y - 1]);
        if(x < width && x >= 0 && y <= height && y >= 0) radius[2] =(cellArray[x + 1, y]);
        if(x > -0 && x <= width && y <= height && y >= 0) radius[3] =(cellArray[x - 1, y]);
        
        if(x >= 0 && x < width && y <= height && y > 0) radius[4] =(cellArray[x+1, y - 1]);
        if(x < width && x >= 0 && y < height && y >= 0) radius[5] =(cellArray[x + 1, y+1]);
        if(x > -0 && x <= width && y < height && y >= 0) radius[6] =(cellArray[x - 1, y+1]);
        if(x > -0 && x <= width && y <= height && y > 0) radius[7] =(cellArray[x - 1, y-1]);
        radius[8] = cell;

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
    
    
}
