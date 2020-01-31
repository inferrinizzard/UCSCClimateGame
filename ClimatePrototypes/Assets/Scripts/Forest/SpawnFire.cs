using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnFire : MonoBehaviour
{
    public GameObject bigFire;
    public GameObject mediumFire;
    public GameObject smallFire;
    public FireSpawnManager spawnValues;
    public Canvas canvas;
    int count = 1;
    // Start is called before the first frame update
    void Start()
    {
        Spawn();
    }

    // Update is called once per frame
    void Spawn()
    {
        int currentSpawnIndex = 0;

        for(int i = 0; i < spawnValues.prefabs; i++)
        {
            if (World.averageTemp > 90)
            {
                GameObject newBigFire = Instantiate(bigFire, spawnValues.spawnPoints[currentSpawnIndex], Quaternion.identity);
                newBigFire.transform.SetParent(canvas.transform);
                newBigFire.name = spawnValues.name + count;
                currentSpawnIndex = (currentSpawnIndex + 1) % spawnValues.spawnPoints.Length;
            }else if (World.averageTemp < 70)
            {
                GameObject newSmallFire = Instantiate(smallFire, spawnValues.spawnPoints[currentSpawnIndex], Quaternion.identity);
                newSmallFire.transform.SetParent(canvas.transform);
                newSmallFire.name = spawnValues.name + count;
                currentSpawnIndex = (currentSpawnIndex + 1) % spawnValues.spawnPoints.Length;
            }
            else
            {
                GameObject newMediumFire = Instantiate(mediumFire, spawnValues.spawnPoints[currentSpawnIndex], Quaternion.identity);
                newMediumFire.transform.SetParent(canvas.transform);
                newMediumFire.name = spawnValues.name + count;
                currentSpawnIndex = (currentSpawnIndex + 1) % spawnValues.spawnPoints.Length;
            }
            
        }
    }
}
