using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class fillSlider : MonoBehaviour
{

    public WaterManagement water;
    public Image fillImage;

    public Slider slider;

    void Wake()
    {
        slider = GetComponent<Slider>();
        
    }

    // Update is called once per frame
    void Update()
    {   
        float fillValue = water.remainingWater / water.maxWater;
        slider.value = fillValue;
        
    }
}
