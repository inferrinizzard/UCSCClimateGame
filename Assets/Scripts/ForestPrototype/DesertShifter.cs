using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesertShifter : MonoBehaviour
{
    public float MinDesertLine = 7f;
    public float MinTreeLine = -6f;
    public float DesertGrowthRate = 1f;

    private Vector3 ShiftTargetPos;
    private bool MoveMePlease;
    // Start is called before the first frame update
    void Start()
    {
        ShiftTargetPos = transform.position;
        MoveMePlease = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (MoveMePlease)
            MoveHelper();
        else
        {
            Vector3 CurrPos = transform.position;
            CurrPos.x = CurrPos.x - DesertGrowthRate * Time.deltaTime;

            transform.position = CurrPos;
        }
    }

    private void MoveHelper()
    {
        transform.position = Vector3.Lerp(transform.position, ShiftTargetPos, 1.3f * Time.deltaTime);
        //Debug.Log("Position: " + transform.position.y);

        if(Mathf.Abs(transform.position.x - ShiftTargetPos.x) < .1f)
        {
            MoveMePlease = false;
        }
    }

    public void ShiftRight(float dist = 1f)
    {
        Vector3 CurrPos = transform.position;

        if (CurrPos.x + dist <= MinDesertLine)
        {
            ShiftTargetPos = new Vector3(CurrPos.x + dist, CurrPos.y, CurrPos.z);
            MoveMePlease = true;
        }
        else if (Mathf.Abs(CurrPos.x - MinDesertLine) > .1f)
        {
            ShiftTargetPos = new Vector3(MinDesertLine, CurrPos.y, CurrPos.z);
            MoveMePlease = true;
        }
    }

    public void ShiftLeft(float dist = 1f)
    {
        Vector3 CurrPos = transform.position;

        if (CurrPos.x - dist >= MinTreeLine)
        {
            ShiftTargetPos = new Vector3(CurrPos.x - dist, CurrPos.y, CurrPos.z);
            MoveMePlease = true;
        }
        else if (Mathf.Abs(CurrPos.x - MinTreeLine) > .1f)
        {
            ShiftTargetPos = new Vector3(MinTreeLine, CurrPos.y, CurrPos.z);
            MoveMePlease = true;
        }
    }
}
