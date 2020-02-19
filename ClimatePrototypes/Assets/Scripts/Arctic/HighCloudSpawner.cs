using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class HighCloudSpawner : MonoBehaviour
{
    public GameObject highCloudPrefab;
    [SerializeField]
    private bool canSpawn = true;
    public float highCloudSpawnWaitSeconds = 3f;
    Transform highCloudParrent;
    private void Start()
    {
        highCloudParrent = new GameObject().transform;
        highCloudParrent.name = "High Cloud";
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
        Instantiate(highCloudPrefab, transform.position + transform.up * Random.Range(-0.5f, 1), Quaternion.identity, highCloudParrent);
        StartCoroutine(SpawnLowCloudWait());
    }
    
    IEnumerator SpawnLowCloudWait() {
        canSpawn = false;
        yield return new WaitForSeconds(Random.Range(0.5f, highCloudSpawnWaitSeconds));
        canSpawn = true;
    }
}