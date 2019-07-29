using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowHandler : MonoBehaviour
{
    public Wind prefab;

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
        if (Input.GetMouseButtonDown(0))
        {
            Wind temp = Instantiate(prefab, mouseLocation, transform.rotation) as Wind;
            currWind = temp;
            currWind.StartPos = mouseLocation;
        }

        if(currWind != null)
        {
            Vector3 diff = mouseLocation - currWind.transform.position;
            diff.Normalize();

            float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
            currWind.transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);
            currWind.Size = (currWind.StartPos - mouseLocation).magnitude + .4f;
        }

        if (Input.GetMouseButtonUp(0))
        {
            currWind.WindActive = true;
            currWind = null;
        }
    }
}
