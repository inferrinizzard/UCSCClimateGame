using System.Collections;
using System.Collections.Generic;
using MathNet.Numerics.Distributions;
using UnityEngine;

public class PaddleBehavior : MonoBehaviour
{
    private Rigidbody2D paddleRb2d;
    private bool canEmit = true;
    public float ballEmitWaitSeconds = 1f;
    public GameObject ballPrefab;
    
    // Start is called before the first frame update
    void Start()
    {
        paddleRb2d = GetComponent<Rigidbody2D>();
        paddleRb2d.velocity = new Vector2(-2, 0);


    }

    // Update is called once per frame
    void FixedUpdate()
    {
       Move();
       if (canEmit) EmitBall();
    }

    private void Move()
    {
        if (Mathf.Abs(transform.position.x) >= 6.0f)
        {
            paddleRb2d.velocity = -paddleRb2d.velocity;
        }
    }

    private void EmitBall()
    {
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
