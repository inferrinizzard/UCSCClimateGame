using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongWaveSpawner : MonoBehaviour
{
    public bool cloudHit = false;
    private bool canEmit = true;
    private bool hasEmit = false;
    public float ballEmitWaitSeconds = 10f;
    public GameObject longWavePrefab;

    Transform longWaveParent;
    // Start is called before the first frame update
    void Start()
    {
        longWaveParent = new GameObject().transform;
        longWaveParent.name = "Long Wave Ray";
    }

    // Update is called once per frame
    void Update() {
        if (cloudHit && canEmit && !hasEmit)
            EmitBall();
        
    }
    private void EmitBall() {
        //for (int i = 0; i < numBalls; i++)
        Instantiate(longWavePrefab, transform.position, Quaternion.identity, longWaveParent);
        //StartCoroutine(EmitBallWait());
        hasEmit = true;
    }

    IEnumerator EmitBallWait() {
        canEmit = false;
        yield return new WaitForSeconds(ballEmitWaitSeconds);
        canEmit = true;
    }

}
