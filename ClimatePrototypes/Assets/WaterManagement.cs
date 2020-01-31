using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterManagement : MonoBehaviour
{
    public float maxWater = 10f;
    public float remainingWater = 10f;

    private void Start() {
        
    }

    void useWater(float used)
    {
        remainingWater -= used;
        remainingWater = remainingWater < 0f ? 0f: remainingWater;
    }

    void addWater(float adding)
    {
        remainingWater += adding;
        remainingWater = remainingWater > 10f ? 10f: remainingWater;
    }

}
