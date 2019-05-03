using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudSpawner : MonoBehaviour
{
    public Cloud prefab;
    public AnimationCurve SpawnCurve;

    public float PityTime = 6f;
    public float CurrentChance = 0f;
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

        CurrentChance = SpawnCurve.Evaluate(currPityTime / PityTime) * 100f;

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
