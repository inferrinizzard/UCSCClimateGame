using System.Collections;
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
    public CameraManager camManager;
    public Sprite forestIcon;
    public Sprite facilityIcon;
    public Sprite factoryIcon;
    


    public float budget;
    public TextMeshProUGUI budgetText;
    public TextMeshProUGUI minionText;

    public int minionNumber;

    private Transform agentTransform;
    private bool scriptActive;
    private SpriteRenderer regionIcon;
    
    private bool employStatus;
    private bool assignStatus;

    void Start()
    {
        regionIcon = gameObject.transform.Find("RegionIcon").GetComponent<SpriteRenderer>();
        budget = 50f;
        minionNumber = 0; 
        budgetText.text = "$ " + budget.ToString();
        minionText.text = "Minion: " + minionNumber.ToString();

        agentTransform = gameObject.GetComponent<Transform>();
        scriptActive = true;
        

    }

    void Update()
    {
        employStatus = gameObject.GetComponent<VolunteerState>().amIEmployed;
        assignStatus = gameObject.GetComponent<VolunteerState>().amIAssigned;
        
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
        if (Input.GetButtonDown("Fire1") && scriptActive && employStatus && !assignStatus)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray,Mathf.Infinity);
        
            if(hit.collider != null && hit.collider.transform == agentTransform)
            {
                //Debug.Log("hit agent");

                if (plantTree.canPlantTree && budget >= 10f)
                {
                    int camInt = camManager.getCurrentRegion();  // agent current location
                    UpdateAgentRegionUI(camInt);
                    gameObject.GetComponent<VolunteerState>().amIAssigned = true;
                    
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
