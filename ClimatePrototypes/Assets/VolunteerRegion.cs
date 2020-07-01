using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolunteerRegion : MonoBehaviour
{
    public MouseClickOnAgent forestScript;

    public MouseClickonProtestAgent facilityandfactoryScript;
    public CameraManager camManager;

    // Update is called once per frame
    void Update()
    {
        int camInt = camManager.getCurrentRegion();  // agent current location
        
        if (camInt == 1) // facility
        {
            forestScript.enabled = false;
            facilityandfactoryScript.firstSpawnLocation = new Vector3Int(-21,-3,0);
            facilityandfactoryScript.enabled = true;
            
        }else if (camInt == 2)
        {
            forestScript.enabled = true;
            facilityandfactoryScript.enabled = false;
        }else if (camInt == 3)  // factory
        {
            forestScript.enabled = false;
            facilityandfactoryScript.firstSpawnLocation = new Vector3Int(11,-2,0);
            facilityandfactoryScript.enabled = true;
            
        }
        
    }
}
