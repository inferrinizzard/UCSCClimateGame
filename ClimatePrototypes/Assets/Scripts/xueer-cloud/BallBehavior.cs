using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallBehavior : MonoBehaviour
{
    public float upForce = 5f;

    public float sideForce = 2f;
    // Start is called before the first frame update
    void Start()
    {
        
        Vector2 force = new Vector2(Random.Range(-sideForce, sideForce), - Random.Range(upForce * 0.8f, upForce));
        GetComponent<Rigidbody2D>().velocity = force;
    }
    
}
