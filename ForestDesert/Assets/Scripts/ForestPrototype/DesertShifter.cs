using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesertShifter : MonoBehaviour
{
    // True when we should be moving the desert in a direction
    // We move the desert when a tree fully grows near the edge of the border
    private bool MoveMePlease;

    // Start is called before the first frame update
    void Start()
    {
        MoveMePlease = false;

        // Calculate the x position of the border by the % coverage in global statics
        float TargetPos = 18f * (1f - GlobalStatics.DesertCoverage/100f) - 9f;

        // Move us to that target position
        Vector3 newPos = transform.position;
        newPos.x = TargetPos;

        transform.position = newPos;
    }

    // Update is called once per frame
    void Update()
    {
        // If we should be shifting
        if (MoveMePlease)
        {
            // Calculate where we should be shifting to
            float TargetPos = 18f * (1f - GlobalStatics.DesertCoverage / 100f) - 9f;

            Vector3 newPos = transform.position;
            newPos.x = TargetPos;

            // Lerp us towards that targer position
            transform.position = Vector3.Lerp(transform.position, newPos, 1.3f * Time.deltaTime);

            // If we are less than 0.1 units away, stop moving as we're close enough
            if (Mathf.Abs(transform.position.x - newPos.x) < .1f)
            {
                MoveMePlease = false;
            }
        }
    }

    // Shifts the desert by adding the input parameter to the DesertCoverage percent stored in globalstatics
    public void Shift(float xPercent)
    {
        GlobalStatics.DesertCoverage = Mathf.Clamp(xPercent + GlobalStatics.DesertCoverage, 15f, 85f);
        MoveMePlease = true;
    }
}
