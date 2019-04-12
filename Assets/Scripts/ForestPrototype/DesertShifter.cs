using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesertShifter : MonoBehaviour
{
    public float MinDesertLine = 3f;
    public float MinTreeLine = -3f;

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
    }

    private void MoveHelper()
    {
        transform.position = Vector3.Lerp(transform.position, ShiftTargetPos, 1.3f * Time.deltaTime);
        //Debug.Log("Position: " + transform.position.y);

        if(Mathf.Abs(transform.position.y - ShiftTargetPos.y) < .1f)
        {
            MoveMePlease = false;
        }
    }

    public void ShiftUp()
    {
        Vector3 CurrPos = transform.position;

        if (CurrPos.y + 1f <= MinDesertLine)
        {
            ShiftTargetPos = new Vector3(CurrPos.x, CurrPos.y + 1f, CurrPos.z);
            MoveMePlease = true;
        }
        else if (Mathf.Abs(CurrPos.y - MinDesertLine) > .1f)
        {
            ShiftTargetPos = new Vector3(CurrPos.x, MinDesertLine, CurrPos.z);
            MoveMePlease = true;
        }
    }

    public void ShiftDown()
    {
        Vector3 CurrPos = transform.position;

        if (CurrPos.y - 1f >= MinTreeLine)
        {
            ShiftTargetPos = new Vector3(CurrPos.x, CurrPos.y - 1f, CurrPos.z);
            MoveMePlease = true;
        }
        else if (Mathf.Abs(CurrPos.y - MinTreeLine) > .1f)
        {
            ShiftTargetPos = new Vector3(CurrPos.x, MinTreeLine, CurrPos.z);
            MoveMePlease = true;
        }
    }
}
