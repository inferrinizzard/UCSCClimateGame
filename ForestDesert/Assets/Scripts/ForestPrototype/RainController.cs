using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainController : MonoBehaviour
{
    public GameObject RainImage;
    public DesertShifter ds;

    // Chance is the percent chance for it to rain every minute.
    // 25 would be a 25% chance to rain over the period of a minute
    public float Chance = 50f;

    // The pity timer will make sure it rains at least every x minutes
    // Ramps up the percent chance over the course of this time
    public float PityTimer = 3f;

    // The rate at which to ramp up the percent chance
    public AnimationCurve PityCurve;

    private float CurrPityTime = 0f;
    private bool bRaining = false;
    private float CurrRainTime = 0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!bRaining)
        {
            CurrPityTime += Time.deltaTime;
            float percentChance = Time.deltaTime * (Chance + PityCurve.Evaluate(CurrPityTime / (PityTimer * 60f)) * (6000f - Chance));
            percentChance /= 60f;

            if (Random.Range(0f, 100f) < percentChance)
            {
                CauseRain();
            }
        }
        else
        {
            CurrRainTime += Time.deltaTime;
            if (CurrRainTime >= .8f)
            {
                bRaining = false;
                RainImage.SetActive(false);
            }
        }
    }

    void CauseRain()
    {
        RainImage.SetActive(true);
        ds.Shift(-3f);
        CurrPityTime = 0f;
        CurrRainTime = 0f;
        bRaining = true;
    }
}
