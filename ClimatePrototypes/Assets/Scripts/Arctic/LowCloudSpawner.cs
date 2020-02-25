using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LowCloudSpawner : MonoBehaviour
{
    public GameObject lowCloudPrefab;
    [SerializeField]
    private bool canSpawn = true;
    public float lowCloudSpawnWaitSeconds = 8f;
    Transform lowCloudParrent;
    private void Start()
    {
        lowCloudParrent = new GameObject().transform;
        lowCloudParrent.name = "Low Cloud";
    }
    
    private void OnEnable()
    {
        canSpawn = true;
    }
    
    void Update() {
        if (canSpawn)
            SpawnLowCloud();
    }

    private void SpawnLowCloud() {
        Instantiate(lowCloudPrefab, transform.position + transform.up * Random.Range(0, 1.5f), Quaternion.identity, lowCloudParrent);
        StartCoroutine(SpawnLowCloudWait());
    }
    
    IEnumerator SpawnLowCloudWait() {
        canSpawn = false;
        yield return new WaitForSeconds(Random.Range(6f, lowCloudSpawnWaitSeconds));
        canSpawn = true;
    }
}
