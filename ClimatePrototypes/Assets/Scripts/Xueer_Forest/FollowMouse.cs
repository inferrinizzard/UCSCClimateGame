using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMouse : MonoBehaviour
{
    
    //private Vector3 offset;
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
         Vector3 newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
         newPosition.z = -1;
         transform.position = newPosition;
         
         
    }
}
