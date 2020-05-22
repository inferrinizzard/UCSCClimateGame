using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MouseClickonProtestAgent : MonoBehaviour
{
    public GameObject agentPlacedPrefab1;
    public GameObject agentPlacedPrefab2;
    public GameObject agentPlacedPrefab3;
    private Transform agentTransform;
    public Tilemap tilemap;
    
    public CameraManager camManager;
    public Sprite forestIcon;
    public Sprite facilityIcon;
    public Sprite factoryIcon;

    private int counter = 0;
    private Vector3Int spawnOne;

    public Vector3Int firstSpawnLocation;
    private SpriteRenderer regionIcon;
    
    private bool employStatus;
    private bool assignStatus;
    
    void Start()
    {
        regionIcon = gameObject.transform.Find("RegionIcon").GetComponent<SpriteRenderer>();
        agentTransform = gameObject.GetComponent<Transform>();
        //spawnOne = firstSpawnLocation;
        
    }

    void Update()
    {
        employStatus = gameObject.GetComponent<VolunteerState>().amIEmployed;
        assignStatus = gameObject.GetComponent<VolunteerState>().amIAssigned;
            
        spawnOne = firstSpawnLocation;
        // if selected, spawn clicked state of selected character
        // pre-req:  purchased but not assigned task
        if (Input.GetButtonDown("Fire1") && employStatus && !assignStatus)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray,Mathf.Infinity);
        
            if(hit.collider != null && hit.collider.transform == agentTransform && counter < 6)
            {
                int camInt = camManager.getCurrentRegion();  // agent current location
                UpdateAgentRegionUI(camInt);
                gameObject.GetComponent<VolunteerState>().amIAssigned = true;
                
                // spawn agent to be placed
                
                
                if (counter % 3 ==0) Instantiate(agentPlacedPrefab1, tilemap.GetCellCenterWorld(spawnOne), transform.rotation);
                else if (counter % 3 == 1) Instantiate(agentPlacedPrefab2, tilemap.GetCellCenterWorld(spawnOne), transform.rotation);
                else if(counter % 3 == 2) Instantiate(agentPlacedPrefab3, tilemap.GetCellCenterWorld(spawnOne), transform.rotation);
                
                counter++;
                
                spawnOne += new Vector3Int(2, 0, 0);

            }
        }
    }
    
    void UpdateAgentRegionUI(int region)
    {
        if (region == 1)
        {
            regionIcon.sprite = facilityIcon;
        }else if (region == 2)
        {
            regionIcon.sprite = facilityIcon = forestIcon;
        }else if (region == 3)
        {
            regionIcon.sprite = factoryIcon;
        }
    }
}
