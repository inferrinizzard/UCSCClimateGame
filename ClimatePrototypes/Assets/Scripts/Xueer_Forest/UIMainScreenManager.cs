using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

public class UIMainScreenManager : MonoBehaviour
{
    public Transform agentColliderTransform;
    public Transform stationColliderTransform;
    public Transform forestColliderTransform;
    public Transform factoryColliderTransform;
    public Transform confirmColliderTransform;
    
    public GameObject TutorialTextBackground;
    public GameObject TutorialTextTMP;
    public TextMeshProUGUI selectedRegionText;

    public Animator agentAnimator;
    
    private enum Region
    {
        Station,
        Factory,
        Forest,
        None
    }

    private bool walking;

    private Region selectedRegion;

    private void Start()
    {
        TutorialTextBackground.SetActive(false);
        TutorialTextTMP.SetActive(false);

        selectedRegion = Region.None;
    }

    private void FixedUpdate()
    {
        if(walking)
        {
            agentColliderTransform.position += Vector3.right * Time.fixedDeltaTime;
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray,Mathf.Infinity);
           
            if(hit.collider != null && hit.collider.transform == agentColliderTransform)
            {
                TutorialTextBackground.SetActive(true);
                TutorialTextTMP.SetActive(true);
            }
            if(hit.collider != null && hit.collider.transform == stationColliderTransform)
            {
                TutorialTextBackground.SetActive(true);
                TutorialTextTMP.SetActive(true);
                selectedRegionText.text = "CO2 STATION";
                selectedRegion = Region.Station;
            }
            if(hit.collider != null && hit.collider.transform == forestColliderTransform)
            {
                TutorialTextBackground.SetActive(true);
                TutorialTextTMP.SetActive(true);
                selectedRegionText.text = "FOREST";
                selectedRegion = Region.Forest;
            }
            if(hit.collider != null && hit.collider.transform == factoryColliderTransform)
            {
                TutorialTextBackground.SetActive(true);
                TutorialTextTMP.SetActive(true);
                selectedRegionText.text = "FACTORY";
                selectedRegion = Region.Factory;
            }
            if(hit.collider != null && hit.collider.transform == confirmColliderTransform)
            {
                if (selectedRegion != Region.None)
                {
                    TutorialTextBackground.SetActive(false);
                    TutorialTextTMP.SetActive(false);
                    selectedRegionText.text = "";
                    
                    // Animation of agent walking to the right side of screen
                    agentAnimator.SetBool("LoadRegion", true);
                    walking = true;

                }
                
            }
        }
    }

    
}
