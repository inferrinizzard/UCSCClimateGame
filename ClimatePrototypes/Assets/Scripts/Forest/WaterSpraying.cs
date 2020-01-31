using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterSpraying : MonoBehaviour
{
    public GameObject particle;
    GameObject cloneWater;

    private void Start()
    {
    }
    void Update()
    {
        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f);
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 wordPos;
            Ray ray = Camera.main.ScreenPointToRay(mousePos);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1000f))
            {
                wordPos = hit.point;
            }
            else
            {
                wordPos = Camera.main.ScreenToWorldPoint(mousePos);
            }
            cloneWater = Instantiate(particle, wordPos, Quaternion.identity);
        }
        if (Input.GetMouseButtonUp(0))
        {
            Destroy(cloneWater);
        }
    }
}