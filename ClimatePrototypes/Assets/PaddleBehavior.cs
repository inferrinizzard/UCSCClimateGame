using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaddleBehavior : MonoBehaviour
{
    private Rigidbody2D paddleRb2d;

    private Transform paddleTransform;
    // Start is called before the first frame update
    void Start()
    {
        paddleRb2d = GetComponent<Rigidbody2D>();
        paddleTransform = GetComponent<Transform>();


    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    private void Move()
    {
        while (Mathf.Abs(transform.position.x) < 7)
        {
            paddleRb2d.velocity = new Vector2(Random.Range(0.0f, 5.0f), 0.0f);
        }
        
    }
}
