using System.Collections;
//using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class MouseClickOnAgent : MonoBehaviour
{
    public Tilemap tilemap;
    public GameObject agentClickedPrefab;
    public PlantTree plantTree;


    public float budget;
    public TextMeshProUGUI budgetText;
    public TextMeshProUGUI minionText;

    public int minionNumber;

    private Transform agentTransform;
    private bool scriptActive;

    void Start()
    {
        budget = 50f;
        minionNumber = 0; 
        budgetText.text = "$ " + budget.ToString();
        minionText.text = "Minion: " + minionNumber.ToString();

        agentTransform = gameObject.GetComponent<Transform>();
        scriptActive = true;
    }

    void Update()
    {
        // TODO
        // make this class usable for placed AgentClicked to move cell position
        
        // if there is an agent selected currently, disable this function
        if (plantTree.agentSelected != null)
        {
            //GetComponent<SpriteRenderer>().color = Color.grey;
            scriptActive = false;
        }
        else
        {
            //GetComponent<SpriteRenderer>().color = new Color(255, 255, 255);
            scriptActive = true;
        }
        
        // if selected, spawn clicked state of selected character
        if (Input.GetButtonDown("Fire1") && scriptActive)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray,Mathf.Infinity);
        
            if(hit.collider != null && hit.collider.transform == agentTransform)
            {
                //Debug.Log("hit agent");

                if (plantTree.canPlantTree && budget >= 10f)
                {
                    // spawn agent to be placed
                    Vector3 spawnPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    //spawnPosition.z = -1;
                    agentClickedPrefab.SetActive(true);
                    Instantiate(agentClickedPrefab, spawnPosition, transform.rotation);
                    budget -= 10f;
                    minionNumber += 1;
                    if (minionNumber == 5)
                    {
                        //GetComponent<Image>().color = Color.clear;
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
