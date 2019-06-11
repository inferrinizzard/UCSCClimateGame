using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Handles spawning arrows when the mouse is clicked
public class ArrowHandler : MonoBehaviour
{
    // Reference to the prefab to spawn
    public Wind prefab;

    // References the current prefab to change (null if the mouse isn't pressed)
    private Wind currWind;
    // Start is called before the first frame update
    void Start()
    {
        currWind = null;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mouseLocation = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseLocation.z = 0f;
        // When the mouse is pressed, spawn a wind at its location
        if (Input.GetMouseButtonDown(0))
        {
            Wind temp = Instantiate(prefab, mouseLocation, transform.rotation) as Wind;
            currWind = temp;
            currWind.StartPos = mouseLocation;
        }

        // If we have a wind in currWind, update it's rotation and scale
        if(currWind != null)
        {
            Vector3 diff = mouseLocation - currWind.transform.position;
            diff.Normalize();

            // Gets the angle from the mouse position
            float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
            currWind.transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);
            currWind.Size = (currWind.StartPos - mouseLocation).magnitude + .4f;
        }

        // When we let go of the mouse button, activate the curr wind and set the reference to null
        if (Input.GetMouseButtonUp(0))
        {
            currWind.WindActive = true;
            currWind = null;
        }
    }
}
