using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubtropicsCloud : MonoBehaviour
{
    private Vector3 velocity;

    public float speed;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckVelocity();
        CheckDestroyEvent();

        transform.position += velocity * speed * Time.deltaTime;
    }

    private void CheckVelocity()
    {
        if (PopulateWorld.Instance.dir == PopulateWorld.windDir.NE)
        {
            velocity = new Vector3(-1, -1, 0);
        }
        else if (PopulateWorld.Instance.dir == PopulateWorld.windDir.NW)
        {
            velocity = new Vector3(1, -1, 0);
        }
        else if (PopulateWorld.Instance.dir == PopulateWorld.windDir.SE)
        {
            velocity = new Vector3(-1, 1, 0);
        }
        else if (PopulateWorld.Instance.dir == PopulateWorld.windDir.SW)
        {
            velocity = new Vector3(1, 1, 0);
        }
        else
        {
            velocity = Vector3.zero;
        }
    }
    
    /// <summary>
    /// object pool for clouds, out of sight, destroy
    /// </summary>
    void CheckDestroyEvent()
    {
        if (transform.position.y < -7)
        {
            Destroy(gameObject);
        }
    }
}
