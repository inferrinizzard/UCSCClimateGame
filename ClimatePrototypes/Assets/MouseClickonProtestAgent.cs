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

    private int counter = 0;
    private Vector3Int spawnOne;

    public Vector3Int firstSpawnLocation;
    
    void Start()
    {
        agentTransform = gameObject.GetComponent<Transform>();
        spawnOne = firstSpawnLocation;
    }

    void Update()
    {
        // if selected, spawn clicked state of selected character
        if (Input.GetButtonDown("Fire1"))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray,Mathf.Infinity);
        
            if(hit.collider != null && hit.collider.transform == agentTransform && counter < 6)
            {
                // spawn agent to be placed
                
                if (counter % 3 ==0) Instantiate(agentPlacedPrefab1, tilemap.GetCellCenterWorld(spawnOne), transform.rotation);
                else if (counter % 3 == 1) Instantiate(agentPlacedPrefab2, tilemap.GetCellCenterWorld(spawnOne), transform.rotation);
                else if(counter % 3 == 2) Instantiate(agentPlacedPrefab3, tilemap.GetCellCenterWorld(spawnOne), transform.rotation);
                
                counter++;
                spawnOne += new Vector3Int(2, 0, 0);

            }

        }
    }
}
