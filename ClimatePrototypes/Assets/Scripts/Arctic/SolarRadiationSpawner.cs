using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarRadiationSpawner : MonoBehaviour
{
    private bool canEmit = true;
    public float ballEmitWaitSeconds = 1f;
    public GameObject ballPrefab;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (canEmit) EmitBall();
    }
    private void EmitBall()
    {
        Instantiate(ballPrefab, transform.position, Quaternion.identity);
        Instantiate(ballPrefab, transform.position, Quaternion.identity);
        Instantiate(ballPrefab, transform.position, Quaternion.identity);
        StartCoroutine(EmitBallWait());
    }

    IEnumerator EmitBallWait()
    {
        canEmit = false;
        yield return new WaitForSeconds(ballEmitWaitSeconds);
        canEmit = true;
    }
}
