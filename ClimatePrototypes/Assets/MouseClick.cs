using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MouseClick : MonoBehaviour
{
    public Tilemap tilemap;
    public GameObject maleClickedPrefab;
    public PlantTree plantTree;
    private Vector3 offset;
    void Start()
    {
        offset = new Vector3(0f, 0.5f, 0f);
        
    }

    void Update()
    {
        // TODO
        // make this class usable for placed agent to move cell position
        
        // if selected, spawn clicked state of selected character
        if (Input.GetButtonDown("Fire1"))
        {
            // raycast not working
            /*Vector3 clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit hit;
            //Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            Vector3 forward = transform.TransformDirection(Vector3.forward) * 10;
            Debug.DrawRay(clickPosition, forward, Color.red);
            Ray ray = new Ray(clickPosition, forward);
            if (Physics.Raycast(ray, out hit)) {
                Debug.Log("mouse hit something");
                if (hit.transform.name == "Male") Debug.Log("Male is clicked by mouse");
            }
            else
            {
                Debug.Log(Input.mousePosition);
            }*/
            
            Vector3 clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPosition = tilemap.WorldToCell(clickPosition);
            
            // if clicked on agent
            if (cellPosition == new Vector3Int(-8, 0, 0) || cellPosition == new Vector3Int(-8, 1, 0))
            {
                // do not spawn sprite if already on place agent mode
                if (plantTree.canPlantTree)
                {
                    Debug.Log("male clicked");
                    Vector3 spawnPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    spawnPosition.z = -1;
                    Instantiate(maleClickedPrefab, spawnPosition + offset, transform.rotation);
                
                    plantTree.canPlantTree = false;
                }
                
            }
        }
    }
}
