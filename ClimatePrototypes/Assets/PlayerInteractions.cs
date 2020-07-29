using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInteractions : MonoBehaviour
{
    [Header("References")]
    public float speed = 10;

    public Animator bladeAnimator;

    public TextMeshProUGUI leftWaterUI;

    private int water;
    private int maxWater = 50;
    private bool filling;
    
    //private GameObject myLine = new GameObject();
    private Color highlightColor = new Color(176,0,132,255);
    private Color normalColor = new Color(255,119,221,255);
    private SpriteRenderer playerRenderer;

    private static bool selected = true;
    
    private static List<Transform> playerPath = new List<Transform>();

    public bool moving;
    private Transform targetRegion;


    public GameObject line;
    private LineRenderer newLine;

    private GameObject playerCell;
    private IdentityManager.Identity playerCellID;
    private IdentityManager.Moisture playerCellMoisture;
    
    // Start is called before the first frame update
    void Start()
    {
        playerRenderer = GetComponent<SpriteRenderer>();
        playerRenderer.color = normalColor;
        
        // draw line
        
        newLine = line.GetComponent<LineRenderer>();
        water = maxWater;
    }

    // Update is called once per frame
    void Update()
    {
        GFXUpdate();
        leftWaterUI.text = water.ToString();
        
        //// Selection
        if (selected)
        {
            playerRenderer.color = highlightColor;

        }
        else
        {
            playerRenderer.color = normalColor;
        }
        
        //// Pathfinding
        // if path is not empty, exhaust the path
        if (playerPath.Count != 0)
        {
            float step = speed * Time.deltaTime;
            // Are we currently moving towards a region?
            if (moving)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetRegion.position, step);
                
                // align helicopter head with velocity
                                   
                /*
                Quaternion rotation = Quaternion.LookRotation(targetRegion.position, Vector3.left);
                transform.rotation = rotation;*/
                
                Vector3 vectorToTarget = targetRegion.position - transform.position;
                float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg - 90f;  // sprite off by 90f
                Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
                transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * speed);
                
                if (transform.position == targetRegion.position)
                {
                    moving = false;
                    playerPath.Remove(targetRegion);
                }
                
            }
            else
            {
                targetRegion = playerPath[0];
                
                moving = true;
            }
            
        }
        if (targetRegion)
            DrawPlayerPath();
        
        //// World Interaction        
        // check what cell player is on top of 
        playerCell = PopulateWorld.Instance.getCellObjectAtLoc(gameObject.transform.position);
        playerCellID = playerCell.GetComponent<IdentityManager>().id;
        playerCellMoisture = playerCell.GetComponent<IdentityManager>().moisture;
        if (playerCellID == IdentityManager.Identity.Fire)
        {
            //PopulateWorld.Instance.MutateCell(playerCell, IdentityManager.Identity.Green);
            //playerCellMoisture = IdentityManager.Moisture.Moist;
            
            // kill all immediate neighbors fire, radius buffer
            foreach (var neighbor in PopulateWorld.Instance.GetRadius(playerCell))
            {
                IdentityManager.Identity neighborID = neighbor.GetComponent<IdentityManager>().id;
                if (neighborID == IdentityManager.Identity.Fire && neighbor != null && water >= 0)
                {
                    PopulateWorld.Instance.MutateCell(neighbor, IdentityManager.Identity.Green);
                    neighbor.GetComponent<IdentityManager>().moisture = IdentityManager.Moisture.Moist;
                    water--;  // use 1 water per cell
                }
                    
            }
        }
        else if (playerCellID == IdentityManager.Identity.Water)
        {
            // replemish water
            if (!filling && water < maxWater )
            {
                filling = true;
                water += 1;
                StartCoroutine(FillWater());
            }
            
        }
        
        

    }

    void GFXUpdate()
    {
        // rotate blades
        bladeAnimator.SetBool("isMoving", playerPath.Count != 0);
    }

    IEnumerator FillWater()
    {
        yield return new WaitForSeconds(0.1f);
        filling = false;

    }

    /// <summary>
    /// Select player
    /// </summary>
    private void OnMouseDown()
    {
        Debug.Log("player selected");
        // Toggle select status
        selected = !selected;

    }
    
    
    
    void DrawPlayerPath()
    {
        newLine.material = new Material(Shader.Find("Sprites/Default"));
        newLine.widthMultiplier = 0.1f;


        int seg = playerPath.Count;
        Vector3[] positions = new Vector3[seg + 1];
        positions[0] = gameObject.transform.position; // first point in line must be current player pos
        for (int i = 1; i < seg+1; i++)
        {
            positions[i] = playerPath[i-1].position;
        }
        newLine.positionCount = positions.Length;
        newLine.SetPositions(positions);
        
    }

    public static bool addDestinationToPath(Transform region)
    {
        // if player is in selected and if region is not already in path 
        if (selected && !playerPath.Contains(region))
        {
            playerPath.Add(region);
            
            //PrintPlayerPath();         
            return true;
        }
        else
        {
            return false;
        }
        
            
    }
    
    /// <summary>
    /// Utility prints
    /// </summary>
    static void PrintPlayerPath()
    {
        string pathString = "";
        foreach (Transform t in playerPath)
        {
            pathString += t.position.ToString() + ", ";
            
        }
        Debug.Log(pathString);
    }
    
    
    
}
