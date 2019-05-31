using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesertShifter : MonoBehaviour
{
    private bool MoveMePlease;

    // Start is called before the first frame update
    void Start()
    {
        MoveMePlease = false;

        float TargetPos = 18f * (1f - GlobalStatics.DesertCoverage/100f) - 9f;

        Vector3 newPos = transform.position;
        newPos.x = TargetPos;

        transform.position = newPos;
    }

    // Update is called once per frame
    void Update()
    {
        if (MoveMePlease)
        {
            float TargetPos = 18f * (1f - GlobalStatics.DesertCoverage / 100f) - 9f;

            Vector3 newPos = transform.position;
            newPos.x = TargetPos;

            transform.position = Vector3.Lerp(transform.position, newPos, 1.3f * Time.deltaTime);

            if (Mathf.Abs(transform.position.x - newPos.x) < .1f)
            {
                MoveMePlease = false;
            }
        }
    }

    public void Shift(float xPercent)
    {
        GlobalStatics.DesertCoverage = Mathf.Clamp(xPercent + GlobalStatics.DesertCoverage, 15f, 85f);
        MoveMePlease = true;
    }
}
