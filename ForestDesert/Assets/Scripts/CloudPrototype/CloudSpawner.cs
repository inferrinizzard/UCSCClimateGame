using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudSpawner : MonoBehaviour
{
    // Cloud prefab to spawn
    public Cloud prefab;
    // Curve that stores the % chance to spawn each second (increases over time -- pity timer)
    public AnimationCurve SpawnCurve;

    // The amount of time it takes to get to a 100% chance to spawn a cloud
    public float PityTime = 6f;
    // The current % chance to spawn a cloud each second
    public float CurrentChance = 0f;
    // Whether or not we are in the vertical or horizontal prototype
    public bool bHorizontal = false;

    private float currPityTime = 0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        currPityTime += Time.deltaTime;

        // Evaluate our current spawn chance based on a pity timer
        CurrentChance = SpawnCurve.Evaluate(currPityTime / PityTime) * 100f;

        // If random value between 0 and 100 is less than the percent chance times delta time, spawn a cloud
        if (Random.Range(0f, 100f) < CurrentChance * Time.deltaTime)
        {
            Vector3 spawnPos = transform.position;
            if(bHorizontal)
            {
                spawnPos.x += Random.Range(0, 20f);
                spawnPos.y -= Random.Range(5, 8f);
            }
            else
            {
                spawnPos.x += Random.Range(0, 20f);
                spawnPos.y -= Random.Range(0, 10f);
            }
            Cloud newSpawn = Instantiate(prefab, spawnPos, transform.rotation) as Cloud;
            currPityTime = 0f;
        }
    }
}
