using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.Tilemaps;

public class AgentMovement : MonoBehaviour
{
    public SpriteRenderer plantActionSprite;
    public SpriteRenderer cutActionSprite;
    public PlantTree plantTreeScript;
    public Tilemap tilemap;
    public Vector3Int myLocation;

    public bool amISelected;
    // Start is called before the first frame update
    void Start()
    {
        amISelected = false;
        tilemap = plantTreeScript.tilemap;
        
    }

    // Update is called once per frame
    void Update()
    {
        // if clicked on this agent, highlight him
        if (Input.GetButtonDown("Fire1"))
        {
            Vector3 clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPosition = tilemap.WorldToCell(clickPosition);

            if (tilemap.WorldToCell((transform.position)) == cellPosition)
            {
                amISelected = true;
                // clear old selected agent
                if (plantTreeScript.agentSelected != null)
                {
                    AgentMovement oldSelectedAgent = plantTreeScript.agentSelected.GetComponent<AgentMovement>();
                    oldSelectedAgent.amISelected = false;
                    oldSelectedAgent.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255);
                }

                plantTreeScript.agentSelected = gameObject;
                
                GetComponent<SpriteRenderer>().color = Color.yellow;
                
                // select future tiles
                
                // enable action bar
                plantActionSprite.color = Color.yellow;
                cutActionSprite.color = Color.yellow;
                ;
            }
        } 
        // deselect current agent
        if (Input.GetButtonDown("Fire2"))
        {
            Vector3 clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPosition = tilemap.WorldToCell(clickPosition);

            if (tilemap.WorldToCell((transform.position)) == cellPosition)
            {
                amISelected = false;
                plantTreeScript.agentSelected = null;

                GetComponent<SpriteRenderer>().color = new Color(255,255,255);
                // disable action bar
                plantActionSprite.color = Color.clear;
                cutActionSprite.color = Color.clear;
            }
        }
        
        
    }
}
