using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    // The temperature too start at. This value is set once and never changed
    public float StartTemperature = 75f;

    // Holds the colors to display at max cold or max hot
    public Color HotRed;
    public Color IceBlue;

    // Speed at which to sample perlin noise
    [Range(0, .1f)]
    public float SampleX = .0355f;

    // Starting displacement of perlin noise
    [Range(0, 100)]
    public float Displacement = 0;

    // Total amount temp can deviate. Starting temp would be halfway through the deviation range
    [Range(0, 100)]
    public float TempRange = 25f;

    private SpriteRenderer sr;
    private Color StartColor;

    // Keeps track of current time for perlin noise
    private float CurrTime;
    // The current temperature of this water
    public float Temperature;
    // The base temperature (start temp + cloud deviation)
    private float baseTemperature;
    // Short term temperature deviation cause by storms sucking up warm water
    public float Deviation = 0f;
    // Whether or not we are currently deviating temperature
    private bool bDeviating = false;
    // Time tracker for temp deviation
    private float DeviationTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        StartColor = sr.color;
        HotRed = new Color(169f/255, 45f/255, 135f/255, 1f);
        //IceBlue = new Color(120, 160, 195);

        CurrTime = 0f;
        Displacement = Random.Range(0f, 100f);

        StartTemperature = GlobalStatics.Temperature;
    }

    // Update is called once per frame
    void Update()
    {
        CurrTime += Time.deltaTime;

        if(!bDeviating) // If we aren't deviating, quickly bring its value down to 0
            Deviation = Mathf.Lerp(Deviation, 0f, .45f * Time.deltaTime);
        else // If we are deviating, SLOWLY bring its value down to 0
        {
            DeviationTime += Time.deltaTime;
            Deviation = Mathf.Lerp(Deviation, 0f, .2f * Time.deltaTime);
            if (DeviationTime > 3f)
            {
                bDeviating = false;
            }
        }

        // Calculate base temperature
        // StartTemp + PerlinNoise
        baseTemperature = StartTemperature + (Mathf.PerlinNoise(Displacement, CurrTime * SampleX) * TempRange - TempRange/2f);

        // Final temp = basetemp + deviation value
        Temperature = baseTemperature + Deviation;

        // Figure out what color we should be displaying based on deviation
        UpdateColor();
    }

    void UpdateColor()
    {
        float diff = Temperature - 75f;
        diff = Mathf.Clamp(diff, -15f, 15f);

        if (diff > 0f)
        {
            sr.color = Color.Lerp(StartColor, HotRed, diff / 15f);
        }
        else
        {
            sr.color = Color.Lerp(StartColor, IceBlue, -1f * diff / 15f);
        }
    }

    // Called when a cloud is over this water. Deviates the temp by d degrees
    public void Deviate(float d)
    {
        bDeviating = true;
        Deviation += d;

        DeviationTime = 0f;
    }
}
