using System.Collections;
//using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MouseClick : MonoBehaviour
{
    public Tilemap tilemap;
    public GameObject maleClickedPrefab;
    public PlantTree plantTree;
    private Vector3 offset;

    public float budget;
    public TextMeshProUGUI budgetText;
    public TextMeshProUGUI minionText;

    public int minionNumber;

    void Start()
    {
        budget = 50f;
        minionNumber = 0; 
        budgetText.text = "$ " + budget.ToString();
        minionText.text = "Minion: " + minionNumber.ToString();
        offset = new Vector3(0f, 0.5f, 0f);
    }

    void Update()
    {
        // TODO
        // make this class usable for placed AgentClicked to move cell position
        
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
            
            // if clicked on agent spawn prefab
            //if (cellPosition == new Vector3Int(-8, 0, 0) || cellPosition == new Vector3Int(-8, 1, 0)) 
            if (cellPosition == new Vector3Int(-4, -4, 0))
            {
                // do not spawn sprite if already on place agent mode
                if (plantTree.canPlantTree && budget >= 10f)
                {
                    Debug.Log("male clicked");
                    Vector3 spawnPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    spawnPosition.z = -1;
                    Instantiate(maleClickedPrefab, spawnPosition + offset, transform.rotation);
                    budget -= 10f;
                    minionNumber += 1;
                    if (minionNumber == 5)
                    {
                        GetComponent<SpriteRenderer>().color = Color.clear;
                    }
                
                    plantTree.canPlantTree = false;
                    plantTree.canPlantAgent = true;
                }
                
            }
            
            // update budget text
            budgetText.text = "$" + budget.ToString();
            minionText.text = "Minion: " + minionNumber.ToString();
        }
    }
}
