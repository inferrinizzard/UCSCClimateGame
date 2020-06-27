using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public GameObject ForestCam;
    public GameObject FacilityCam;
    public GameObject FactoryCam;
    
    public int getCurrentRegion()
    {
        // 1 is facility
        // 2 is forest
        // 3 is factory
        if (ForestCam.active)
        {
            return 2;
        }else if (FacilityCam.active)
        {
            return 1;
        }else if (FactoryCam.active)
        {
            return 3;
        }
        else
        {
            return 0;
            Debug.LogWarning("No camera active right now");
        }
        
    }
}
