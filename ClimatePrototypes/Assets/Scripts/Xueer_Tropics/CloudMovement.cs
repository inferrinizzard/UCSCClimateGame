using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudMovement : MonoBehaviour
{
    [SerializeField]
    private Transform cloudTransform;
    private float movementSpeed = -1f;
    // Update is called once per frame
    void Update()
    {
        float verticalSpeed = Input.GetAxis("Vertical");
        float horizontalSpeed = Input.GetAxis("horizontal");
        
        Vector3 vertical2D = new Vector3(0, 1, 0);
        Vector3 horizontal2D = new Vector3(1, 0, 0);
        cloudTransform.Translate(vertical2D * verticalSpeed * movementSpeed * Time.deltaTime);
         cloudTransform.Translate(horizontal2D * horizontalSpeed * movementSpeed * Time.deltaTime);
    }
}
