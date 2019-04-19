using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassScript : MonoBehaviour
{
    public GameObject TreePrefab;
    public StatController sc;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        if (Input.GetKey("p") && GlobalStatics.CashMoney >= 25f)
        {
            GlobalStatics.CashMoney -= 25;
            sc.CashChange(-25f);

            Vector3 mouseLocation = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseLocation.z = -.2f;

            GameObject NewTree = Instantiate(TreePrefab, mouseLocation, transform.rotation) as GameObject;
        }
    }
}
