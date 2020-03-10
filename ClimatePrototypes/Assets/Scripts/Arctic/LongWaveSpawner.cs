using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongWaveSpawner : MonoBehaviour
{
    private bool canEmit = true;
    //private bool hasEmit = false;
    private float ballEmitWaitSeconds = 5.0f;
    public GameObject longWavePrefab;
    private int numberofLongWave = 3;

    Transform longWaveParent;
    void Start()
    {
        longWaveParent = new GameObject().transform;
        longWaveParent.name = "Long Wave Ray";
        StartCoroutine(EmitBallWait(1f));
    }
    
    void Update() {
        if ( canEmit)
            EmitBall();
        
    }
    private void EmitBall() {
        for (int i = 0; i < numberofLongWave; i++)
        {
            Instantiate(longWavePrefab, transform.position, Quaternion.identity, longWaveParent);

        }
        StartCoroutine(EmitBallWait(ballEmitWaitSeconds));
        //hasEmit = true;
    }

    IEnumerator EmitBallWait(float waitTime) {
        canEmit = false;
        yield return new WaitForSeconds(waitTime);
        canEmit = true;
    }

}
