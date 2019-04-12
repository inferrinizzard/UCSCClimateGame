using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudSpawner : MonoBehaviour
{
    public Cloud prefab;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < 20; ++i)
        {
            for(int j = 0; j < 10; ++j)
            {
                if(Random.Range(0f, 1000f) < .05)
                {
                    Vector3 spawnPos = transform.position;
                    spawnPos.x += i;
                    spawnPos.y -= j;
                    Cloud newSpawn = Instantiate(prefab, spawnPos, transform.rotation) as Cloud;
                }
            }
        }
    }
}
